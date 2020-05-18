using PdfDocument = PdfiumViewer.PdfDocument;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SchoolDisplay
{
    /// <summary>
    /// A class representing our PDF file storage.
    /// </summary>
    public class PdfRepository
    {
        private readonly string directoryPath;

        public PdfRepository(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                this.directoryPath = directoryPath;
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory {directoryPath} does not exist.");
            }
        }

        /// <summary>
        /// Get a IEnumerable of all available PDF files.
        /// </summary>
        /// <returns>A string list of PDF file names.</returns>
        public IEnumerable<string> ListAllFiles()
        {
            // hide implementation details by only returning the filename, not the path.
            return Directory.EnumerateFiles(directoryPath, "*.pdf").Select(file => Path.GetFileName(file));
        }

        /// <summary>
        /// Retrieves a PdfDocument by its file name asynchronously by copying it into RAM.
        /// </summary>
        /// <param name="fileName">The PDF file name.</param>
        /// <returns>A PdfDocument representing the PDF file.</returns>
        public async Task<PdfDocument> GetDocumentAsync(string fileName)
        {
            MemoryStream pdfCopy = new MemoryStream();

            using (FileStream fs = File.OpenRead(Path.Combine(directoryPath, fileName)))
            {
                await fs.CopyToAsync(pdfCopy);
            }

            return PdfDocument.Load(pdfCopy);
        }

    }
}
