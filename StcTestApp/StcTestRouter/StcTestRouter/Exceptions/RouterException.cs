using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StcTestRouter.Exceptions
{
    public class RouterException : Exception
    {
        public RouterException(string message)
        : base(message) { }
    }
}
