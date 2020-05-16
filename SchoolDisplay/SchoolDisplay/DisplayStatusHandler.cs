using System;
using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace SchoolDisplay
{
    class DisplayStatusHandler
    {
        // Constants
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xF170;
        private const int DISPLAY_ON = -1;
        private const int DISPLAY_OFF = 2;

        // Settings
        private readonly TimeSpan StartTime;
        private readonly TimeSpan StopTime;
        private readonly bool DisplayActiveOnWeekend;

        // Other
        private readonly int WindowHandle;
        private Timer DisplayTimer;

        // Status
        private int CurrentStatus;

        public DisplayStatusHandler(int windowHandle, TimeSpan startTime, TimeSpan stopTime, bool displayActiveOnWeekend)
        {
            WindowHandle = windowHandle;
            StartTime = startTime;
            StopTime = stopTime;
            DisplayActiveOnWeekend = displayActiveOnWeekend;

            SetupDisplayTimer();
            SetDisplayStatus(determineRequiredStatus()); // Set Initial Status
        }

        private void SetDisplayStatus(int status)
        {
            SendMessage(WindowHandle, WM_SYSCOMMAND, SC_MONITORPOWER, status);
            CurrentStatus = status;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private void SetupDisplayTimer()
        {
            DisplayTimer = new Timer();
            DisplayTimer.Interval = 60000;
            DisplayTimer.Tick += SetRequiredDisplayStatus;
            DisplayTimer.Start();
        }

        private void SetRequiredDisplayStatus(Object source, EventArgs e)
        {
            var requiredStatus = determineRequiredStatus();

            if (CurrentStatus != requiredStatus)
                SetDisplayStatus(requiredStatus);
        }

        private int determineRequiredStatus()
        {
            var status = DISPLAY_ON;
            var date = DateTime.Now;
            var now = date.TimeOfDay;

            if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday) && !DisplayActiveOnWeekend)
            {
                status = DISPLAY_OFF;
            }
            else if ((now <= StartTime) || (now >= StopTime))
            {
                status = DISPLAY_OFF;
            }

            return status;
        }

        // Turn display on if program is closed
        ~DisplayStatusHandler()
        {
            SetDisplayStatus(DISPLAY_ON);
        }
    }
}
