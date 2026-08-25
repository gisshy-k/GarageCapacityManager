using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;

namespace GarageCapacityManager
{
    [FileLocation("ModsSettings/GarageCapacityManager/GarageCapacityManager")]
    [SettingsUIShowGroupName(GarageCapacityGroup)]
    public class Setting : ModSetting
    {
        public const string MainTab = "Main";
        public const string GarageCapacityGroup = "Garage Capacities";
        public const string ResetGroup = "Reset";

        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection(MainTab, GarageCapacityGroup)]
        public bool EnableGarageCapacities { get; set; }

        private bool HideGarageCapacities() => !EnableGarageCapacities;

        // Represents 0% to 100% capacity scaling (0.0 to 1.0)
        [SettingsUISlider(min = 0f, max = 1f, step = 0.1f, unit = "floatSingleFraction")]
        [SettingsUISection(MainTab, GarageCapacityGroup)]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(HideGarageCapacities))]
        public float GarageSpotsPerHousehold { get; set; }

        // Represents 0% to 100% capacity scaling (0.0 to 1.0)
        [SettingsUISlider(min = 0f, max = 1f, step = 0.1f, unit = "floatSingleFraction")]
        [SettingsUISection(MainTab, GarageCapacityGroup)]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(HideGarageCapacities))]
        public float GarageSpotsPerWorker { get; set; }

        [SettingsUIButton]
        [SettingsUISection(MainTab, ResetGroup)]
        public bool ResetButton { set { SetDefaults(); } }

        public override void SetDefaults()
        {
            EnableGarageCapacities = true;
            GarageSpotsPerHousehold = 0.5f; // Default 50%
            GarageSpotsPerWorker = 0.3f;    // Default 30%
        }

        public override void Apply()
        {
            base.Apply();
            Systems.GarageCapacityScaleSystem.RequireGlobalUpdate = true;
        }
    }
}