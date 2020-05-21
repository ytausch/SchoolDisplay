using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolDisplay
{
    public class CyclicPdfService
    {
        private readonly PdfRepository repository;

        private string currentFile;     // the last file that was handed out via GetNextFileName()

        /// <summary>
        /// This event occurs if the last document handed out via GetNextDocument() is no longer valid.
        /// (it could be deleted or changed)
        /// Handlers have to reload by calling GetNextDocument() again.
        /// </summary>
        public event EventHandler OnInvalidate;
        private bool invalidateInvoked;

        public CyclicPdfService(PdfRepository repository)
        {
            this.repository = repository;
            repository.DataChanged += Repository_DataChanged;
        }

        /// <summary>
        /// Returns the next PDF document in the cycle.
        /// Throws exceptions if the PDF document cannot be found or opened.
        /// If the last file changed in the meantime, allow it to be returned again.
        /// </summary>
        /// <exception cref="FileNotFoundException">If no files are available.</exception>
        /// <exception cref="PdfAccessException">If something went wrong while opening a file.</exception>
        public PdfDocument GetNextDocument()
        {
            try
            {
                // if OnInvalidate was invoked previously, allow GetNextDocument() to return the same file again
                return repository.GetDocument(GetNextFileName(includeCurrentFile: invalidateInvoked));
            }
            finally
            {
                // re-enable event
                invalidateInvoked = false;
            }
        }

        /// <exception cref="FileNotFoundException">If no files are available.</exception>
        private string GetNextFileName(bool includeCurrentFile = false)
        {
            IEnumerable<string> availableFiles = repository.ListAllFiles();

            if (!availableFiles.Any())
            {
                currentFile = null;
                throw new FileNotFoundException("No files available!");
            }

            if (currentFile != null)
            {
                // determine which files come alphabetically after currentFile
                IEnumerable<string> alphabeticallyNextFiles;
                
                if (includeCurrentFile)
                {
                    // ">=" ensures that currentFile itself is included if available
                    alphabeticallyNextFiles = availableFiles.Where(f => f.CompareTo(currentFile) >= 0);
                }
                else
                {
                    alphabeticallyNextFiles = availableFiles.Where(f => f.CompareTo(currentFile) > 0);
                }

                if (alphabeticallyNextFiles.Any())
                {
                    // if there are any files after currentFile, use the next file (i.e. the file that comes first in the alphabet)
                    currentFile = alphabeticallyNextFiles.Min();
                }
                else
                {
                    // otherwise, take the first file (i.e. roll over to the beginning)
                    currentFile = availableFiles.Min();
                }
            }
            else
            {
                // First Call -> return first file name in the culture-dependent alphabet
                currentFile = availableFiles.Min();
            }

            return currentFile;
        }

        private async void Repository_DataChanged(object sender, ChangedPdfEventArgs e)
        {
            if (!invalidateInvoked && (currentFile == null || e.FileName == currentFile))
            {
                // OnInvalidate should only be invoked once.
                invalidateInvoked = true;

                // reloading the PDF just after the file was changed can fail if the write is not completed -> wait 2 seconds
                await Task.Delay(2000);

                OnInvalidate?.Invoke(this, EventArgs.Empty);
            }

            // else: no interesting file changed or event already invoked
        }
    }
}
