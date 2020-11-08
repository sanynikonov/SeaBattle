using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class FieldBuilderException : Exception
    {
        public FieldBuilderException() : base() { }

        public FieldBuilderException(string message) : base(message) { }

        public FieldBuilderException(string message, Exception innerException) : base(message, innerException) { }

    }
}
