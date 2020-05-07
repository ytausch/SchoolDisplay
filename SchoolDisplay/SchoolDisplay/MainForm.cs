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
        public MainForm()
        {
            InitializeComponent();

            SetupForm();

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

        private string GetPdfPathFromConfig()
        {
            string pdfPath;

            try
            {
                pdfPath = ConfigurationManager.AppSettings.Get("PdfFilePath");
            }
            catch (ConfigurationException)
            {
                throw new DisplayableException(Properties.Resources.ConfigLoadError);
            }

            if (pdfPath == null)
            {
                throw new DisplayableException(Properties.Resources.ConfigLoadError);
            }

            return pdfPath;
        }

        private void ShowError(string text)
        {
            pdfRenderer.Visible = false;
            lblErrors.Text = text;
            lblErrors.Visible = true;
        }
    }
}
