using System;
using System.IO;
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

        Timer retryTimer;

        readonly Clock clock;
        readonly DisplayStatusHandler displayStatusHandler;
        readonly Scroller scroller;
        readonly CyclicPdfService pdfService;

        public MainForm()
        {
            InitializeComponent();

            SetupForm();
            clock = new Clock(lblClock);

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

            scroller = new Scroller(pdfRenderer, scrollTick, pauseTime);
            scroller.FileEndReached += PdfEndReached;

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

        private void PdfEndReached(object sender, EventArgs e) => LoadNextPdf();

        private void RetryTimer_Tick(object sender, EventArgs e) => LoadNextPdf();

        private void PdfService_OnInvalidate(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                // run on UI thread
                LoadNextPdf();
            });
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
            scroller.Restart();

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

        private void ShowError(string text)
        {
            pdfRenderer.Visible = false;
            lblErrors.Text = text;
            lblErrors.Visible = true;

            scroller.Stop();
        }

        private void HideError()
        {
            lblErrors.Visible = false;
            pdfRenderer.Visible = true;
        }
    }
}
