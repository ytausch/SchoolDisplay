using System;

namespace SchoolDisplay
{
    class PdfAccessException : Exception
    {
        public PdfAccessException(string message) : base(message)
        {
        }

        public PdfAccessException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
