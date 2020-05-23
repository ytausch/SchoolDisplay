using PdfiumViewer;
using SchoolDisplay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SchoolDisplayTests
{
    internal class FakePdfRepository : IPdfRepository
    {
        public event EventHandler<ChangedPdfEventArgs> DataChanged;

        public List<string> FileList { get; set; }

        public FakePdfRepository(List<string> fileList)
        {
            FileList = fileList;
        }

        public void RaiseDataChangedEvent(ChangedPdfEventArgs args)
        {
            DataChanged?.Invoke(this, args);
        }

        public IPdfDocument GetDocument(string fileName)
        {
            // generate a fake PDF document according to the file name
            if (FileList.Contains(fileName))
            {
                return new FakePdfDocument(fileName);
            }
            else
            {
                throw new FileNotFoundException($"This FakePdfRepository does not contain { fileName }.");
            }
        }

        public IEnumerable<string> ListAllFiles()
        {
            return FileList;
        }
    }
}
