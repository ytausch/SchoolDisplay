using System;

namespace SchoolDisplay
{
    class BadConfigException : Exception
    {
        public BadConfigException(string message) : base(message)
        {
        }

        public BadConfigException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
