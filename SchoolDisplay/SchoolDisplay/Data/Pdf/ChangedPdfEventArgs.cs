using System;

namespace SchoolDisplay.Data.Pdf
{
    public class ChangedPdfEventArgs : EventArgs
    {
        public string FileName { get; }

        public ChangedPdfEventArgs(string fileName)
        {
            FileName = fileName;
        }   
    }
}
