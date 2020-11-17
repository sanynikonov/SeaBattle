using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    class FieldServiceException : Exception
    {
        public FieldServiceException() : base() { }

        public FieldServiceException(string message) : base(message) { }

        public FieldServiceException(string message, Exception innerException) : base(message, innerException) { }

    }
}
