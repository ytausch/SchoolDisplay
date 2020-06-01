using PdfiumViewer;
using System;
using System.Collections.Generic;

namespace SchoolDisplay.Data.Pdf
{
    public interface IPdfRepository
    {
        event EventHandler<ChangedPdfEventArgs> DataChanged;

        IEnumerable<string> ListAllFiles();

        IPdfDocument GetDocument(string fileName);
    }
}
