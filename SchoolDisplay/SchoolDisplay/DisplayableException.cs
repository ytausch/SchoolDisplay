using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDisplay
{
    class BadConfigException : Exception
    {
        public BadConfigException(string message) : base(message)
        {
        }
    }
}
