﻿using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace SchoolDisplay
{
    public partial class MainForm : Form
    {
        /* configuration values */
        readonly string pdfDirectoryPath;
        readonly int scrollSpeed;        // in 3px per x ms
        readonly int pauseTime;          // in ms

        bool pdfOnScreen = false;       // true if a PDF file is currently displayed, false if not
        int scrollTop = 0;              // Keep track of scroll height

        Timer clockTimer;
        Timer scrollTimer;

        readonly CyclicPdfService pdfService;

        public MainForm()
        {
            InitializeComponent();

            SetupForm();
            SetupClockTimer();

            try
            {
                // TODO: Move settings methods to static SettingsHelper class
                pdfDirectoryPath = GetSettingsString("PdfDirectoryPath");
                scrollSpeed = GetNonNegativeSettingsInt("ScrollSpeed");
                pauseTime = GetNonNegativeSettingsInt("PauseTime");
            }
            catch (BadConfigException ex)
            {
                ShowError(ex.Message);
                return;
            }

            try
            {
                pdfService = new CyclicPdfService(new PdfRepository(pdfDirectoryPath));
            }
            catch (DirectoryNotFoundException)
            {
                ShowError(Properties.Resources.InvalidPathError);
                return;
            }

            SetupScrollTimer();
            LoadNextPdf();
        }

        private void SetupForm()
        {
            GoFullscreen();

            // no mouse cursor
            Cursor.Hide();
        }

        private void GoFullscreen()
        {
            // on top of everything else
            TopMost = true;

            // no window borders
            FormBorderStyle = FormBorderStyle.None;

            // fill entire screen
            WindowState = FormWindowState.Maximized;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Please note that "KeyPreview" of MainForm has to be set to true for this to work.
            // It was done in the Designer view.
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void SetupClockTimer()
        {
            clockTimer = new Timer();
            clockTimer.Interval = 1000;
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();
        }

        private void ClockTimer_Tick(Object source, EventArgs e)
        {
            lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private string GetSettingsString(string name)
        {
            string s;
            try
            {
                s = ConfigurationManager.AppSettings.Get(name);
            }
            catch (ConfigurationException)
            {
                throw new BadConfigException(Properties.Resources.ConfigLoadError);
            }

            if (s == null)
            {
                throw new BadConfigException(String.Format(Properties.Resources.ConfigMissingKeyError, name));
            }

            return s;
        }

        private int GetNonNegativeSettingsInt(string name)
        {
            string s = GetSettingsString(name);
            if (int.TryParse(s, out int i) && i >= 0)
            {
                return i;
            }
            else
            {
                throw new BadConfigException(String.Format(Properties.Resources.ConfigInvalidValueError, name));
            }
        }

        private void LoadNextPdf()
        {
            try
            {
                pdfRenderer.Load(pdfService.GetNextDocument());
            }
            catch
            {
                // Since a lot of unexcepted Exceptions can happen when opening a PDF file,
                // we exceptionally catch all of them here for maximum reliability.

                // This is a general error message because we don't want to display technical details publicly.
                // This line also executes when there is no PDF available.
                ShowError(Properties.Resources.PdfAccessError);
                return;
            }

            ResetAndStartScrollTimer();

            HideError();
        }

        private void SetupScrollTimer()
        {
            scrollTimer = new Timer();
            scrollTimer.Tick += ScrollOneLine;
            scrollTimer.Interval = scrollSpeed;
            // do not enable timer: loadPdf will trigger that with ResetAndStartScrollTimer()
        }

        private void ResetAndStartScrollTimer()
        {
            scrollTop = 0;
            scrollTimer.Start();
        }

        private void ShowError(string text)
        {
            pdfRenderer.Visible = false;
            lblErrors.Text = text;
            lblErrors.Visible = true;

            pdfOnScreen = false;
        }

        private void HideError()
        {
            lblErrors.Visible = false;
            pdfRenderer.Visible = true;

            pdfOnScreen = true;
        }

        private async void ScrollOneLine(object sender, EventArgs e)
        {
            // Check if pdf loaded successfully
            if (!pdfOnScreen)
            {
                scrollTimer.Stop();
                return;
            }

            // Jump one unit
            scrollTop -= 1;
            pdfRenderer.SetDisplayRectLocation(new Point(1, scrollTop));

            // Check if end of document is reached
            var currentPos = pdfRenderer.DisplayRectangle.Top;
            var documentHeight = pdfRenderer.DisplayRectangle.Height - pdfRenderer.Height;
            if (Math.Abs(currentPos) != documentHeight)
            {
                return;
            }

            // Sleep and jump back up
            scrollTimer.Stop();
            await Task.Delay(pauseTime);
            LoadNextPdf();
        }
    }
}
