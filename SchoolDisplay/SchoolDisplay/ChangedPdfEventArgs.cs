using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDisplay
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
