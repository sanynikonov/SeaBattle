using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameServiceException : Exception
    {
        public GameServiceException() : base() { }

        public GameServiceException(string message) : base(message) { }

        public GameServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
