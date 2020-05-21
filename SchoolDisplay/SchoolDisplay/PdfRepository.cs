﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfDocument = PdfiumViewer.PdfDocument;

namespace SchoolDisplay
{
    /// <summary>
    /// A class representing a PDF file storage.
    /// </summary>
    public class PdfRepository
    {
        private readonly string directoryPath;
        private readonly FileSystemWatcher fsWatcher;

        public event EventHandler<ChangedPdfEventArgs> DataChanged;

        public PdfRepository(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                this.directoryPath = directoryPath;

                fsWatcher = SetupFileSystemWatcher();
                fsWatcher.EnableRaisingEvents = true;
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory {directoryPath} is invalid or does not exist.");
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
        /// Retrieves a PdfDocument by its file name by copying it into RAM.
        /// </summary>
        /// <param name="fileName">The PDF file name.</param>
        /// <returns>A PdfDocument representing the PDF file.</returns>
        /// <exception cref="PdfAccessException">If something went wrong during PDF access. Contains a data field "fileName".</exception>
        public PdfDocument GetDocument(string fileName)
        {
            try
            {
                return GetDocumentUnsafe(fileName);
            }
            catch (Exception e)
            {
                // Since a lot of unexcepted Exceptions can happen when opening a PDF file,
                // we exceptionally catch all of them here for maximum reliability.

                var exception = new PdfAccessException("Error opening PDF file", e);
                exception.Data.Add("fileName", fileName);
                throw exception;
            }
        }

        private PdfDocument GetDocumentUnsafe(string fileName)
        {
            MemoryStream pdfCopy = new MemoryStream();

            using (FileStream fs = File.OpenRead(Path.Combine(directoryPath, fileName)))
            {
                fs.CopyTo(pdfCopy);
            }

            PdfDocument document = PdfDocument.Load(pdfCopy);
            ValidatePdf(document);

            return document;
        }

        private void ValidatePdf(PdfDocument document)
        {
            if (document.PageCount == 0)
            {
                throw new ArgumentException("PDF file has no pages.");
            }
        }

        private FileSystemWatcher SetupFileSystemWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(directoryPath, "*.pdf");

            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            watcher.Changed += FsWatcher_OnChanged;
            watcher.Created += FsWatcher_OnChanged;
            watcher.Deleted += FsWatcher_OnChanged;
            watcher.Renamed += FsWatcher_OnRenamed;

            return watcher;
        }

        private void FsWatcher_OnChanged(Object source, FileSystemEventArgs e)
        {
            DataChanged?.Invoke(this, new ChangedPdfEventArgs(e.Name));
        }

        private void FsWatcher_OnRenamed(Object source, RenamedEventArgs e)
        {
            DataChanged?.Invoke(this, new ChangedPdfEventArgs(e.OldName));
        }
    }
}
