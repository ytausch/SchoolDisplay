using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace SchoolDisplayTests
{
    class FakePdfDocument : IPdfDocument
    {
        public string FileName { get; }

        public FakePdfDocument(string fileName)
        {
            FileName = fileName;
        }

        /* Not implemented methods */
        public int PageCount => throw new NotImplementedException();

        public PdfBookmarkCollection Bookmarks => throw new NotImplementedException();

        public IList<SizeF> PageSizes => throw new NotImplementedException();

        public PrintDocument CreatePrintDocument()
        {
            throw new NotImplementedException();
        }

        public PrintDocument CreatePrintDocument(PdfPrintMode printMode)
        {
            throw new NotImplementedException();
        }

        public PrintDocument CreatePrintDocument(PdfPrintSettings settings)
        {
            throw new NotImplementedException();
        }

        public void DeletePage(int page)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public PdfInformation GetInformation()
        {
            throw new NotImplementedException();
        }

        public PdfPageLinks GetPageLinks(int page, Size size)
        {
            throw new NotImplementedException();
        }

        public string GetPdfText(int page)
        {
            throw new NotImplementedException();
        }

        public string GetPdfText(PdfTextSpan textSpan)
        {
            throw new NotImplementedException();
        }

        public IList<PdfRectangle> GetTextBounds(PdfTextSpan textSpan)
        {
            throw new NotImplementedException();
        }

        public Point PointFromPdf(int page, PointF point)
        {
            throw new NotImplementedException();
        }

        public PointF PointToPdf(int page, Point point)
        {
            throw new NotImplementedException();
        }

        public Rectangle RectangleFromPdf(int page, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        public RectangleF RectangleToPdf(int page, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds, bool forPrinting)
        {
            throw new NotImplementedException();
        }

        public void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds, PdfRenderFlags flags)
        {
            throw new NotImplementedException();
        }

        public Image Render(int page, float dpiX, float dpiY, bool forPrinting)
        {
            throw new NotImplementedException();
        }

        public Image Render(int page, float dpiX, float dpiY, PdfRenderFlags flags)
        {
            throw new NotImplementedException();
        }

        public Image Render(int page, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            throw new NotImplementedException();
        }

        public Image Render(int page, int width, int height, float dpiX, float dpiY, PdfRenderFlags flags)
        {
            throw new NotImplementedException();
        }

        public Image Render(int page, int width, int height, float dpiX, float dpiY, PdfRotation rotate, PdfRenderFlags flags)
        {
            throw new NotImplementedException();
        }

        public void RotatePage(int page, PdfRotation rotation)
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        public PdfMatches Search(string text, bool matchCase, bool wholeWord)
        {
            throw new NotImplementedException();
        }

        public PdfMatches Search(string text, bool matchCase, bool wholeWord, int page)
        {
            throw new NotImplementedException();
        }

        public PdfMatches Search(string text, bool matchCase, bool wholeWord, int startPage, int endPage)
        {
            throw new NotImplementedException();
        }
    }
}
