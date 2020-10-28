using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameEventArgs
    {
        public Field FirstPlayerField { get; }
        public Field SecondPlayerField { get; }

        public GameEventArgs(Field firstPlayerField, Field secondPlayerField)
        {
            FirstPlayerField = firstPlayerField;
            SecondPlayerField = secondPlayerField;
        }
    }
}
