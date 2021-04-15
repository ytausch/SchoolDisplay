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
        
        private Timer retryTimer;
        private readonly Scroller scroller;
        private readonly CyclicPdfService pdfService;
        private readonly Settings settings;

        public MainForm()
        {
            InitializeComponent();

            SetupForm();
            new Clock(lblClock);

            try
            {
                settings = new Settings();
            }
            catch (BadConfigException ex)
            {
                ShowError(ex.Message);
                return;
            }
            
            new DisplayStatusHandler(this.Handle.ToInt32(), settings.DisplayAlwaysOn, settings.DisplayStartTime, settings.DisplayStopTime, settings.DisplayOnWeekend);

            try
            {
                pdfService = new CyclicPdfService(new PdfRepository(settings.PdfDirectoryPath));
            }
            catch (DirectoryNotFoundException)
            {
                ShowError(Properties.Resources.InvalidPathError);
                return;
            }

            pdfService.OnInvalidate += PdfService_OnInvalidate;

            SetupRetryTimer();

            scroller = new Scroller(pdfRenderer, settings.ScrollTick, settings.PauseTime, settings.MinDisplayTime);
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

                StartRetryTimer(settings.EmptyPollingDelay);
                return;
            }
            catch (DirectoryNotFoundException)
            {
                // error reading directory
                ShowError(Properties.Resources.DirectoryReadError);

                StartRetryTimer(settings.EmptyPollingDelay);
                return;
            }
            catch (PdfAccessException e)
            {
                // something else went wrong when opening an existing PDF file.
                string fileName = e.Data.Contains("fileName") ? (string)e.Data["fileName"] : "unknown";

                ShowError(string.Format(Properties.Resources.PdfAccessError, fileName));

                StartRetryTimer(settings.ErrorDisplayDelay);
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

            if (scroller != null)
            {
                scroller.Stop();
            }  
        }

        private void HideError()
        {
            lblErrors.Visible = false;
            pdfRenderer.Visible = true;
        }
    }
}
