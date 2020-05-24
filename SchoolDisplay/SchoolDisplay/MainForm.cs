using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SchoolDisplay.Data.Pdf;
using SchoolDisplay.Data.Settings;
using SchoolDisplay.Display;
using Timer = System.Windows.Forms.Timer;

namespace SchoolDisplay
{
    public partial class MainForm : Form
    {
        /* configuration values */
        readonly string pdfDirectoryPath;
        readonly float scrollTick;
        readonly int pauseTime;             // in ms
        readonly int errorDisplayDelay;      // in ms
        readonly int emptyPollingDelay;  // in ms
        readonly bool displayAlwaysOn;
        readonly TimeSpan displayStartTime;
        readonly TimeSpan displayStopTime;
        readonly bool displayOnWeekend;

        bool pdfOnScreen = false;       // true if a PDF file is currently displayed, false if not
        float scrollTop = 0;              // Keep track of scroll height

        Timer clockTimer;
        Timer retryTimer;
        Timer scrollTimer;
        
        DisplayStatusHandler displayStatusHandler;

        readonly CyclicPdfService pdfService;

        public MainForm()
        {
            InitializeComponent();

            SetupForm();
            SetupClockTimer();

            try
            {
                pdfDirectoryPath = SettingsLoader.GetSettingsString("PdfDirectoryPath");
                scrollTick = (float) SettingsLoader.GetNonNegativeSettingsInt("ScrollSpeed")/10;
                pauseTime = SettingsLoader.GetNonNegativeSettingsInt("PauseTime");
                displayAlwaysOn = SettingsLoader.GetSettingsBool("DisplayAlwaysOn");
                displayStartTime = SettingsLoader.GetSettingsTimeFrame("DisplayStartTime");
                displayStopTime = SettingsLoader.GetSettingsTimeFrame("DisplayStopTime");
                displayOnWeekend = SettingsLoader.GetSettingsBool("ActiveOnWeekends");
                errorDisplayDelay = SettingsLoader.GetNonNegativeSettingsInt("ErrorDisplayDelay");
                emptyPollingDelay = SettingsLoader.GetNonNegativeSettingsInt("EmptyPollingDelay");
            }
            catch (BadConfigException ex)
            {
                ShowError(ex.Message);
                return;
            }
            
            displayStatusHandler = new DisplayStatusHandler(this.Handle.ToInt32(), displayAlwaysOn, displayStartTime, displayStopTime, displayOnWeekend);

            try
            {
                pdfService = new CyclicPdfService(new PdfRepository(pdfDirectoryPath));
            }
            catch (DirectoryNotFoundException)
            {
                ShowError(Properties.Resources.InvalidPathError);
                return;
            }

            pdfService.OnInvalidate += PdfService_OnInvalidate;

            SetupRetryTimer();
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

        private void LoadNextPdf()
        {
            retryTimer.Stop();

            try
            {
                pdfRenderer.Load(pdfService.GetNextDocument());
            }
            catch (FileNotFoundException)
            {
                // no PDF file available
                ShowError(Properties.Resources.NoAnnouncementsAvailable);

                StartRetryTimer(emptyPollingDelay);
                return;
            }
            catch (PdfAccessException e)
            {
                // something else went wrong when opening an existing PDF file.
                string fileName = e.Data.Contains("fileName") ? (string)e.Data["fileName"] : "unknown";

                ShowError(string.Format(Properties.Resources.PdfAccessError, fileName));

                StartRetryTimer(errorDisplayDelay);
                return;
            }

            // everything went fine!

            ResetAndStartScrollTimer();

            HideError();
        }

        private void SetupRetryTimer()
        {
            retryTimer = new Timer();
            retryTimer.Tick += RetryTimer_Tick;
            // do not enable timer: LoadNextPdf will do that when an error occurs
        }

        private void StartRetryTimer(int interval)
        {
            retryTimer.Interval = interval;
            retryTimer.Start();
        }

        private void RetryTimer_Tick(object sender, EventArgs e)
        {
            LoadNextPdf();
        }

        private void PdfService_OnInvalidate(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                // run on UI thread
                LoadNextPdf();
            });
        }

        private void SetupScrollTimer()
        {
            scrollTimer = new Timer();
            scrollTimer.Tick += ScrollOneLine;
            scrollTimer.Interval = 5;
            // do not enable timer: LoadNextPdf will trigger that with ResetAndStartScrollTimer()
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
            scrollTop -= scrollTick;
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
            LoadNextPdf();
        }
    }
}
