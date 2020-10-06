using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDisplay.Data.Settings
{
    class Settings
    {
        public string PdfDirectoryPath { get; }
        public float ScrollTick { get; }
        public int PauseTime { get; }
        public int MinDisplayTime { get; }
        public int ErrorDisplayDelay { get; }
        public int EmptyPollingDelay { get; }
        public bool DisplayAlwaysOn { get; }
        public TimeSpan DisplayStartTime { get; }
        public TimeSpan DisplayStopTime { get; }
        public bool DisplayOnWeekend { get; }

        public Settings()
        {
            PdfDirectoryPath = SettingsLoader.GetSettingsString("PdfDirectoryPath");
            ScrollTick = (float)SettingsLoader.GetNonNegativeSettingsInt("ScrollSpeed") / 10;
            PauseTime = SettingsLoader.GetNonNegativeSettingsInt("PauseTime");
            MinDisplayTime = SettingsLoader.GetNonNegativeSettingsInt("MinDisplayTime");
            DisplayAlwaysOn = SettingsLoader.GetSettingsBool("DisplayAlwaysOn");
            DisplayStartTime = SettingsLoader.GetSettingsTimeFrame("DisplayStartTime");
            DisplayStopTime = SettingsLoader.GetSettingsTimeFrame("DisplayStopTime");
            DisplayOnWeekend = SettingsLoader.GetSettingsBool("ActiveOnWeekends");
            ErrorDisplayDelay = SettingsLoader.GetNonNegativeSettingsInt("ErrorDisplayDelay");
            EmptyPollingDelay = SettingsLoader.GetNonNegativeSettingsInt("EmptyPollingDelay");
        }
    }
}
