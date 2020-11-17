using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class FieldDirectorException : Exception
    {
        public FieldDirectorException() : base() { }

        public FieldDirectorException(string message) : base(message) { }

        public FieldDirectorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
