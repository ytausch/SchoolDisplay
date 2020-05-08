using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolDisplay
{
    public partial class MainForm : Form
    {
        Timer clockTimer;

        public MainForm()
        {
            InitializeComponent();

            SetupForm();
            SetupClockTimer();

            try
            {
                GetPdfPathFromConfig();
            }
            catch (DisplayableException e)
            {
                ShowError(e.Message);
            }
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

        private string GetPdfPathFromConfig()
        {
            string pdfPath = GetSettingsItem("PdfFilePath");

            if (pdfPath == null)
            {
                throw new DisplayableException(Properties.Resources.ConfigLoadError);
            }

            return pdfPath;
        }

        private string GetSettingsItem(string name)
        {
            try
            {
                return ConfigurationManager.AppSettings.Get(name);
            }
            catch (ConfigurationException)
            {
                throw new DisplayableException(Properties.Resources.ConfigLoadError);
            }
        }

        private void ShowError(string text)
        {
            pdfRenderer.Visible = false;
            lblErrors.Text = text;
            lblErrors.Visible = true;
        }
    }
}
