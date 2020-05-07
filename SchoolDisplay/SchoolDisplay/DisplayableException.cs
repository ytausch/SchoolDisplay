using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDisplay
{
    class DisplayableException : Exception
    {
        public DisplayableException(string message) : base(message)
        {
        }
    }
}
