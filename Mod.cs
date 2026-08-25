using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace GarageCapacityManager
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(GarageCapacityManager)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public Setting settings;
        public static Mod INSTANCE;

        public void OnLoad(UpdateSystem updateSystem)
        {
            INSTANCE = this;
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            settings = new Setting(this);
            settings.RegisterInOptionsUI();

            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(settings));
            AssetDatabase.global.LoadSettings(nameof(GarageCapacityManager), settings, new Setting(this));

            // VOLATILE: [SystemUpdatePhase.ModificationEnd]
            // Absolute Mastery of Update Order (Knowledge Base #8). 
            // Garages are structurally modified and updated in the Modification phase, not GameSimulation.
            updateSystem.UpdateAt<Systems.GarageCapacityScaleSystem>(SystemUpdatePhase.ModificationEnd);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (settings != null)
            {
                settings.UnregisterInOptionsUI();
                settings = null;
            }
        }
    }
}