using PdfiumViewer;
using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace SchoolDisplay
{
    public partial class MainForm : Form
    {
        // configuration values
        string pdfFilePath;
        int pollingInterval;

        Timer clockTimer;

        bool pdfOnScreen = false;

        public MainForm()
        {
            InitializeComponent();

            SetupForm();
            SetupClockTimer();

            try
            {
                pdfFilePath = GetSettingsString("PdfFilePath");
                pollingInterval = GetNonNegativeSettingsInt("PollingInterval");
            }
            catch (BadConfigException e)
            {
                ShowError(e.Message);
                return;
            }

            LoadPdf();
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
            clockTimer.Tick += new EventHandler(OnClockUpdateEvent);
            clockTimer.Interval = 1000;
            clockTimer.Start();
        }

        private void OnClockUpdateEvent(Object source, EventArgs e)
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
    }
}
