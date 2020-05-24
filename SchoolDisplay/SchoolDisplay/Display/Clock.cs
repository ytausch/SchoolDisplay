using System;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace SchoolDisplay.Display
{
    class Clock
    {
        private readonly Label Label;
        private readonly Timer Timer;

        public Clock(Label label)
        {
            Label = label;
            Timer = new Timer();
            Timer.Interval = 1000;
            Timer.Tick += this.ClockTimer_Tick;
            Timer.Start();
        }

        private void ClockTimer_Tick(Object source, EventArgs e) => Label.Text = DateTime.Now.ToString("HH:mm:ss");
    }
}
