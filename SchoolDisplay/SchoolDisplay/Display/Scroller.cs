using PdfiumViewer;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace SchoolDisplay.Display
{
    class Scroller
    {
        private readonly PdfRenderer PdfRenderer;
        private readonly float Unit;
        private readonly int PauseTime;

        public event EventHandler FileEndReached;
        private Timer ScrollTimer;
        private float ScrollTop;

        public Scroller(PdfRenderer pdfRenderer, float unit, int pauseTime)
        {
            PdfRenderer = pdfRenderer;
            Unit = unit;
            PauseTime = pauseTime;

            ScrollTop = 0;

            ScrollTimer = new Timer();
            ScrollTimer.Tick += ScrollOneUnit;
            ScrollTimer.Interval = 5;
        }

        private void OnFileEndReached()
        {
            FileEndReached?.Invoke(this, EventArgs.Empty);
        }

        private async void ScrollOneUnit(object sender, EventArgs e)
        {
            // Jump one unit
            ScrollTop -= Unit;
            PdfRenderer.SetDisplayRectLocation(new Point(1, (int)Math.Round(ScrollTop)));

            // Check if end of document is reached
            var currentPos = PdfRenderer.DisplayRectangle.Top;
            var documentHeight = PdfRenderer.DisplayRectangle.Height - PdfRenderer.Height;
            if (Math.Abs(currentPos) != documentHeight)
            {
                return;
            }

            // Sleep and load next PDF
            ScrollTimer.Stop();
            await Task.Delay(PauseTime);
            OnFileEndReached();
        }

        public void Stop()
        {
            ScrollTimer.Stop();
        }

        public void Restart()
        {
            ScrollTop = 0;
            ScrollTimer.Start();
        }

    }
}
