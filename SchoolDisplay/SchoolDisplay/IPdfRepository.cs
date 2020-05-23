using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDisplay
{
    public interface IPdfRepository
    {
        event EventHandler<ChangedPdfEventArgs> DataChanged;

        IEnumerable<string> ListAllFiles();

        IPdfDocument GetDocument(string fileName);
    }
}
