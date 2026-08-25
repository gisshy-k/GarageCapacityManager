using System.Collections.Generic;
using Colossal;

namespace GarageCapacityManager
{
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Garage Capacity Manager" },
                { m_Setting.GetOptionTabLocaleID(Setting.MainTab), "Settings" },

                { m_Setting.GetOptionGroupLocaleID(Setting.GarageCapacityGroup), "Garage Capacities" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableGarageCapacities)), "Enable Realistic Garage Capacities" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableGarageCapacities)),
                    "Replaces vanilla parking capacities with dynamic capacities based on demographics. Toggle OFF to return to vanilla capacities safely."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.GarageSpotsPerHousehold)), "Capacity Ratio per Household" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.GarageSpotsPerHousehold)),
                    "Ratio of parking spots per household (0.0 to 1.0). 1.0 equals 100% (1 spot per household)."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.GarageSpotsPerWorker)), "Capacity Ratio per Worker" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.GarageSpotsPerWorker)),
                    "Ratio of parking spots per worker (0.0 to 1.0). 1.0 equals 100% (1 spot per worker)."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetButton)), "Reset Settings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetButton)), "Reset settings to default values." },
            };
        }

        public void Unload() { }
    }
}