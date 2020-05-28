﻿using PdfiumViewer;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace SchoolDisplay.Display
{
    class Scroller
    {
        private readonly PdfRenderer pdfRenderer;
        private readonly float unit;
        private readonly int pauseTime;
        private readonly Timer scrollTimer;

        public event EventHandler FileEndReached;
        private float scrollTop = 0;

        public Scroller(PdfRenderer pdfRenderer, float unit, int pauseTime)
        {
            this.pdfRenderer = pdfRenderer;
            this.unit = unit;
            this.pauseTime = pauseTime;

            scrollTimer = new Timer();
            scrollTimer.Tick += ScrollOneUnit;
            scrollTimer.Interval = 5;
        }

        private void OnFileEndReached()
        {
            FileEndReached?.Invoke(this, EventArgs.Empty);
        }

        private async void ScrollOneUnit(object sender, EventArgs e)
        {
            // Jump one unit
            scrollTop -= unit;
            pdfRenderer.SetDisplayRectLocation(new Point(1, (int)Math.Round(scrollTop)));

            // Check if end of document is reached
            var currentPos = pdfRenderer.DisplayRectangle.Top;
            var documentHeight = pdfRenderer.DisplayRectangle.Height - pdfRenderer.Height;
            if (Math.Abs(currentPos) != documentHeight)
            {
                return;
            }

            // Sleep and load next PDF
            scrollTimer.Stop();
            await Task.Delay(pauseTime);
            OnFileEndReached();
        }

        public void Stop()
        {
            scrollTimer.Stop();
        }

        public void Restart()
        {
            scrollTop = 0;
            scrollTimer.Start();
        }

    }
}
