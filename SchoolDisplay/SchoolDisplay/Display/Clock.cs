using System;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace SchoolDisplay.Display
{
    class Clock
    {
        private readonly Label label;
        private readonly Timer timer;

        public Clock(Label label)
        {
            this.label = label;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += this.ClockTimer_Tick;
            timer.Start();
        }

        private void ClockTimer_Tick(Object source, EventArgs e) => label.Text = DateTime.Now.ToString("HH:mm:ss");
    }
}
