﻿using PdfiumViewer;
using System;
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
        // configuration values
        readonly string pdfFilePath;
        readonly int pollingInterval;    // in ms
        readonly int scrollSpeed;        // in 3px per x ms
        readonly int pauseTime;          // in ms

        // true if a PDF file is currently displayed, false if not
        bool pdfOnScreen = false;
        int scrollTop = 0;              // Keep track of scroll height

        // Environment.TickCount value of last polling event
        int lastPolling = 0;

        Timer clockTimer;
        Timer pollingTimer;             // only used in pdf loading error state
        Timer scrollTimer;
        FileSystemWatcher fsWatcher;

        public MainForm()
        {
            InitializeComponent();

            SetupForm();
            SetupClockTimer();

            try
            {
                pdfFilePath = GetSettingsString("PdfFilePath");
                pollingInterval = GetNonNegativeSettingsInt("PollingInterval") * 1000;
                scrollSpeed = GetNonNegativeSettingsInt("ScrollSpeed");
                pauseTime = GetNonNegativeSettingsInt("PauseTime");
            }
            catch (BadConfigException ex)
            {
                ShowError(ex.Message);
                return;
            }

            LoadPdf();

            try
            {
                SetupFileSystemWatcher();
            }
            catch (BadConfigException ex)
            {
                ShowError(ex.Message);
                return;
            }

            SetupPollingTimer();
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

        private void LoadPdf()
        {
            // create a copy in RAM to not keep the file open
            MemoryStream pdfCopy = new MemoryStream();

            try
            {
                using (FileStream fs = File.OpenRead(pdfFilePath))
                {
                    fs.CopyTo(pdfCopy);
                }

                PdfDocument document = PdfDocument.Load(pdfCopy);
                pdfRenderer.Load(document);
                SetupScrollTimer();
            }
            catch
            {
                // Since a lot of unexcepted Exceptions can happen when opening a PDF file,
                // we exceptionally catch all of them here for maximum reliability.

                // This is a general error message because we don't want to display technical details publicly.
                ShowError(Properties.Resources.PdfAccessError);
                return;
            }

            HideError();
        }

        private void SetupScrollTimer()
        {
            scrollTimer = new Timer();
            scrollTimer.Tick += ScrollOneLine;
            scrollTimer.Interval = scrollSpeed;
            scrollTimer.Enabled = true;
        }

        private void SetupFileSystemWatcher()
        {
            try
            {
                fsWatcher = new FileSystemWatcher(Path.GetDirectoryName(pdfFilePath), Path.GetFileName(pdfFilePath));
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException
                    || ex is ArgumentNullException
                    || ex is PathTooLongException)
                {
                    throw new BadConfigException(Properties.Resources.InvalidPathError);
                }
            }

            fsWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

            fsWatcher.Changed += FsWatcher_OnChanged;
            fsWatcher.Created += FsWatcher_OnChanged;
            fsWatcher.Renamed += FsWatcher_OnChanged;
            fsWatcher.Deleted += FsWatcher_OnChanged;

            // events will run in the UI thread
            fsWatcher.SynchronizingObject = this;
            fsWatcher.EnableRaisingEvents = true;
        }

        private void SetupPollingTimer()
        {
            if (pollingInterval == 0)
            {
                // polling disabled
                return;
            }

            pollingTimer = new Timer();
            pollingTimer.Interval = pollingInterval;
            pollingTimer.Tick += PollingTimer_Tick;
            // do not enable timer: pollingTimer is only used in case of an error
        }

        private void FsWatcher_OnChanged(Object source, FileSystemEventArgs e)
        {
            LoadPdf();
        }

        private void PollingTimer_Tick(object sender, EventArgs e)
        {
            DoPolling();
        }

        private void DoPolling()
        {
            lastPolling = Environment.TickCount;
            LoadPdf();
        }

        private void ShowError(string text)
        {
            pdfRenderer.Visible = false;
            lblErrors.Text = text;
            lblErrors.Visible = true;

            pdfOnScreen = false;

            // pollingTimer will be null if there is a config error
            if (pollingTimer != null)
            {
                pollingTimer.Enabled = true;
            }
        }

        private void HideError()
        {
            lblErrors.Visible = false;
            pdfRenderer.Visible = true;

            pdfOnScreen = true;

            if (pollingTimer != null)
            {
                // when scrolling is active, we handle polling in JumpUpOrReload instead of PollingTimer_Tick.
                pollingTimer.Enabled = false;
            }
        }

        private void JumpUpOrReload()
        {
            // this is overflow-safe! Environment.TickCount will overflow after around 50 days.
            if (pollingInterval > 0 && Environment.TickCount - lastPolling >= pollingInterval)
            {
                DoPolling();
            }
            else
            {
                // jump up
                pdfRenderer.PerformScroll(ScrollAction.Home, Orientation.Vertical);
            }
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
            var oneBelow = new PdfRectangle(0, new Rectangle(new Point(1, scrollTop), new Size(1, pdfRenderer.Height)));
            pdfRenderer.ScrollIntoView(oneBelow);

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
            scrollTop = 0;
            JumpUpOrReload();
            scrollTimer.Start();
        }
    }
}
