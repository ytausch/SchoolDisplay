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
        private const int MOUSEEVENTF_MOVE = 0x0001;

        // Settings
        private readonly TimeSpan StartTime;
        private readonly TimeSpan StopTime;
        private readonly bool DisplayActiveOnWeekend;
        private readonly bool AlwaysOn;

        // Other
        private readonly int WindowHandle;
        private Timer DisplayTimer;

        // Status
        private int CurrentStatus;

        public DisplayStatusHandler(int windowHandle, bool alwaysOn, TimeSpan startTime, TimeSpan stopTime, bool displayActiveOnWeekend)
        {
            WindowHandle = windowHandle;
            StartTime = startTime;
            StopTime = stopTime;
            DisplayActiveOnWeekend = displayActiveOnWeekend;
            AlwaysOn = alwaysOn;

            if (!AlwaysOn)
            {
                SetupDisplayTimer();
                SetDisplayStatus(determineRequiredStatus()); // Set Initial Status
            }
        }

        private void SetDisplayStatus(int status)
        {
            if (status == DISPLAY_ON)
            {
                mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
            }
            else
            {
                SendMessage(WindowHandle, WM_SYSCOMMAND, SC_MONITORPOWER, status);
            }
            CurrentStatus = status;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);


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
