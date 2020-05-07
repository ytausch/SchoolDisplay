namespace SchoolDisplay
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.pdfRenderer = new PdfiumViewer.PdfRenderer();
            this.lblErrors = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pdfRenderer
            // 
            this.pdfRenderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pdfRenderer.Location = new System.Drawing.Point(0, 0);
            this.pdfRenderer.Name = "pdfRenderer";
            this.pdfRenderer.Page = 0;
            this.pdfRenderer.Rotation = PdfiumViewer.PdfRotation.Rotate0;
            this.pdfRenderer.Size = new System.Drawing.Size(800, 450);
            this.pdfRenderer.TabIndex = 0;
            this.pdfRenderer.Text = "pdfRenderer";
            this.pdfRenderer.ZoomMode = PdfiumViewer.PdfViewerZoomMode.FitWidth;
            // 
            // lblErrors
            // 
            this.lblErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrors.Location = new System.Drawing.Point(0, 0);
            this.lblErrors.Margin = new System.Windows.Forms.Padding(0);
            this.lblErrors.Name = "lblErrors";
            this.lblErrors.Size = new System.Drawing.Size(800, 450);
            this.lblErrors.TabIndex = 1;
            this.lblErrors.Text = "Errors";
            this.lblErrors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblErrors.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblErrors);
            this.Controls.Add(this.pdfRenderer);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Display";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private PdfiumViewer.PdfRenderer pdfRenderer;
        private System.Windows.Forms.Label lblErrors;
    }
}

