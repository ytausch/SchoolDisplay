using System;
using System.Configuration;
using System.Globalization;

namespace SchoolDisplay.Data.Settings
{
    class SettingsLoader
    {
        public static string GetSettingsString(string settingName)
        {
            string value;
            try
            {
                value = ConfigurationManager.AppSettings.Get(settingName);
            }
            catch (ConfigurationException)
            {
                throw new BadConfigException(Properties.Resources.ConfigLoadError);
            }

            if (value != null)
                return value;

            throw new BadConfigException(String.Format(Properties.Resources.ConfigMissingKeyError, settingName));
        }

        public static int GetNonNegativeSettingsInt(string settingName)
        {
            string value = GetSettingsString(settingName);
            if (int.TryParse(value, out int i) && i >= 0)
                return i;
            
            throw new BadConfigException(string.Format(Properties.Resources.ConfigInvalidValueError, settingName));
        }

        public static bool GetSettingsBool(string settingName)
        {
            if (bool.TryParse(GetSettingsString(settingName), out bool b))
                return b;

            throw new BadConfigException(String.Format(Properties.Resources.ConfigInvalidValueError, settingName));
        }

        public static TimeSpan GetSettingsTimeFrame(string settingName)
        {
            string s = GetSettingsString(settingName);
            if (DateTime.TryParseExact(s, "HH:mm", null, DateTimeStyles.None, out DateTime span) && span.TimeOfDay != TimeSpan.Zero)
                return span.TimeOfDay;

            throw new BadConfigException(String.Format(Properties.Resources.ConfigInvalidValueError, settingName));
        }
    }
}
