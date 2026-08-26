using Colossal.Serialization.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies; // REQUIRED for Renter and WorkProvider
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using GarageCapacityManager.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GarageCapacityManager.Systems
{
    // VOLATILE: [Game.Simulation.ParkingLaneDataSystem] / [ModificationEndBarrier]
    // Absolute Mastery of Update Order: Intercept right after vanilla calculates base geometry[cite: 5].
    [UpdateAfter(typeof(ParkingLaneDataSystem))]
    public partial class GarageCapacityScaleSystem : GameSystemBase
    {
        public static bool RequireGlobalUpdate = false;

        private EntityQuery m_TargetGaragesQuery;
        private EntityQuery m_AllGaragesQuery;
        private EntityQuery m_AllModifiedGaragesQuery;

        private bool m_WasEnabled;
        private int m_GraceFrames = -1;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_TargetGaragesQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<GarageLane>() },
                Any = new[] { ComponentType.ReadOnly<Updated>(), ComponentType.ReadOnly<PathfindUpdated>() },
                None = new[] { ComponentType.ReadOnly<Deleted>(), ComponentType.ReadOnly<Temp>() }
            });

            m_AllGaragesQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<GarageLane>() },
                None = new[] { ComponentType.ReadOnly<Deleted>(), ComponentType.ReadOnly<Temp>() }
            });

            m_AllModifiedGaragesQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<GarageLane>(), ComponentType.ReadOnly<OriginalGarageCapacity>() }
            });

            m_WasEnabled = Mod.INSTANCE.settings.EnableGarageCapacities;
        }

        protected override void OnGameLoaded(Context serializationContext)
        {
            // Grace Period Pattern: Wait 60 frames for vanilla data to stabilize[cite: 5].
            m_GraceFrames = 60;
        }

        protected override void OnUpdate()
        {
            bool isEnabled = Mod.INSTANCE.settings.EnableGarageCapacities;

            // --- Vanilla Fallback Principle ---[cite: 5]
            if (m_WasEnabled && !isEnabled)
            {
                RevertToVanilla();
                m_WasEnabled = false;
                return;
            }
            m_WasEnabled = isEnabled;

            if (!isEnabled) return;

            if (m_GraceFrames > 0)
            {
                m_GraceFrames--;
                return;
            }
            else if (m_GraceFrames == 0)
            {
                m_GraceFrames = -1;
                RequireGlobalUpdate = true;
            }

            bool runGlobal = RequireGlobalUpdate;
            if (RequireGlobalUpdate) RequireGlobalUpdate = false;

            var queryToProcess = runGlobal ? m_AllGaragesQuery : m_TargetGaragesQuery;

            if (queryToProcess.IsEmptyIgnoreFilter) return;

            var commandBufferSystem = World.GetOrCreateSystemManaged<ModificationEndBarrier>();
            var ecb = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            // DOTS Source Generator Bug Workaround: Direct job execution[cite: 5].
            Dependency = new CalculateDemographicsCapacityJob
            {
                MultiplierH = Mod.INSTANCE.settings.GarageSpotsPerHousehold,
                MultiplierW = Mod.INSTANCE.settings.GarageSpotsPerWorker,
                OwnerLookup = SystemAPI.GetComponentLookup<Owner>(isReadOnly: true),
                BuildingLookup = SystemAPI.GetComponentLookup<Building>(isReadOnly: true),
                PrefabRefLookup = SystemAPI.GetComponentLookup<PrefabRef>(isReadOnly: true),
                BuildingPropertyDataLookup = SystemAPI.GetComponentLookup<BuildingPropertyData>(isReadOnly: true),
                SpawnableBuildingDataLookup = SystemAPI.GetComponentLookup<SpawnableBuildingData>(isReadOnly: true),
                SignatureBuildingDataLookup = SystemAPI.GetComponentLookup<SignatureBuildingData>(isReadOnly: true),
                ParkingFacilityDataLookup = SystemAPI.GetComponentLookup<ParkingFacilityData>(isReadOnly: true),
                OriginalCapLookup = SystemAPI.GetComponentLookup<OriginalGarageCapacity>(isReadOnly: true),
                RenterLookup = SystemAPI.GetBufferLookup<Renter>(isReadOnly: true),
                WorkProviderLookup = SystemAPI.GetComponentLookup<WorkProvider>(isReadOnly: true),
                Ecb = ecb
            }.ScheduleParallel(queryToProcess, Dependency);

            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        [BurstCompile]
        internal partial struct CalculateDemographicsCapacityJob : IJobEntity
        {
            public float MultiplierH;
            public float MultiplierW;

            [ReadOnly] public ComponentLookup<Owner> OwnerLookup;
            [ReadOnly] public ComponentLookup<Building> BuildingLookup;
            [ReadOnly] public ComponentLookup<PrefabRef> PrefabRefLookup;
            [ReadOnly] public ComponentLookup<BuildingPropertyData> BuildingPropertyDataLookup;
            [ReadOnly] public ComponentLookup<SpawnableBuildingData> SpawnableBuildingDataLookup;
            [ReadOnly] public ComponentLookup<SignatureBuildingData> SignatureBuildingDataLookup;
            [ReadOnly] public ComponentLookup<ParkingFacilityData> ParkingFacilityDataLookup;
            [ReadOnly] public ComponentLookup<OriginalGarageCapacity> OriginalCapLookup;
            [ReadOnly] public BufferLookup<Renter> RenterLookup;
            [ReadOnly] public ComponentLookup<WorkProvider> WorkProviderLookup;

            public EntityCommandBuffer.ParallelWriter Ecb;

            public void Execute(Entity entity, [ChunkIndexInQuery] int chunkIndex, ref GarageLane garageLane)
            {
                ushort baseVanillaCapacity;

                if (OriginalCapLookup.TryGetComponent(entity, out var backup))
                {
                    baseVanillaCapacity = backup.VanillaCapacity;
                }
                else
                {
                    baseVanillaCapacity = garageLane.m_VehicleCapacity;
                    if (baseVanillaCapacity == 0) return;

                    Ecb.AddComponent(chunkIndex, entity, new OriginalGarageCapacity { VanillaCapacity = baseVanillaCapacity });
                }

                Entity currentOwner = entity;
                Entity buildingEntity = Entity.Null;

                while (OwnerLookup.TryGetComponent(currentOwner, out Owner owner))
                {
                    currentOwner = owner.m_Owner;
                    if (BuildingLookup.HasComponent(currentOwner))
                    {
                        buildingEntity = currentOwner;
                        break;
                    }
                }

                if (buildingEntity == Entity.Null) return;
                if (!PrefabRefLookup.TryGetComponent(buildingEntity, out PrefabRef prefabRef)) return;
                Entity prefabEntity = prefabRef.m_Prefab;

                if (ParkingFacilityDataLookup.HasComponent(prefabEntity)) return;

                // SMART RICO FILTER
                bool isRicoBuilding = SpawnableBuildingDataLookup.HasComponent(prefabEntity) ||
                                      SignatureBuildingDataLookup.HasComponent(prefabEntity);

                if (!isRicoBuilding) return;

                // --- Demographics Calculation (Mixed-Use Support) ---
                float calculatedCapacity = 0f;
                bool hasCalculated = false;

                // 1. Residential Logic (Static Prefab Data)
                if (BuildingPropertyDataLookup.TryGetComponent(prefabEntity, out BuildingPropertyData propertyData))
                {
                    if (propertyData.m_ResidentialProperties > 0)
                    {
                        calculatedCapacity += propertyData.m_ResidentialProperties * MultiplierH;
                        hasCalculated = true;
                    }
                }

                // 2. Workplace Logic (Dynamic Renters Data)
                if (RenterLookup.TryGetBuffer(buildingEntity, out DynamicBuffer<Renter> renters))
                {
                    int maxWorkers = 0;
                    foreach (var renter in renters)
                    {
                        if (WorkProviderLookup.TryGetComponent(renter.m_Renter, out WorkProvider workProvider))
                        {
                            maxWorkers += workProvider.m_MaxWorkers;
                        }
                    }

                    if (maxWorkers > 0)
                    {
                        calculatedCapacity += maxWorkers * MultiplierW;
                        hasCalculated = true;
                    }
                }

                // Apply changes
                if (hasCalculated)
                {
                    ushort newCapacity = (ushort)calculatedCapacity;
                                     
                    if (garageLane.m_VehicleCapacity != newCapacity)
                    {
                        garageLane.m_VehicleCapacity = newCapacity;
                        Ecb.SetComponent(chunkIndex, entity, garageLane);
                    }
                }
            }
        }

        private void RevertToVanilla()
        {
            if (m_AllModifiedGaragesQuery.IsEmptyIgnoreFilter) return;

            var entities = m_AllModifiedGaragesQuery.ToEntityArray(Allocator.Temp);
            var originalCaps = m_AllModifiedGaragesQuery.ToComponentDataArray<OriginalGarageCapacity>(Allocator.Temp);
            var garageLanes = m_AllModifiedGaragesQuery.ToComponentDataArray<GarageLane>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity e = entities[i];
                GarageLane lane = garageLanes[i];

                lane.m_VehicleCapacity = originalCaps[i].VanillaCapacity;
                EntityManager.SetComponentData(e, lane);
                EntityManager.RemoveComponent<OriginalGarageCapacity>(e);
            }

            entities.Dispose();
            originalCaps.Dispose();
            garageLanes.Dispose();

            Mod.log.Info("Successfully reverted all RICO garage capacities to Vanilla limits.");
        }
    }
}