﻿using PdfiumViewer;
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

        public CyclicPdfService(PdfRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Returns the next PDF document in the cycle.
        /// Throws exceptions if the PDF document cannot be found or opened.
        /// </summary>
        /// <exception cref="FileNotFoundException">If no files are available.</exception>
        /// <exception cref="PdfAccessException">If something went wrong while opening a file.</exception>
        public PdfDocument GetNextDocument()
        {
            return repository.GetDocument(GetNextFileName());
        }

        /// <exception cref="FileNotFoundException">If no files are available.</exception>
        private string GetNextFileName()
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
                IEnumerable<string> alphabeticallyNextFiles = availableFiles.Where(f => f.CompareTo(currentFile) > 0);

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
    }
}
