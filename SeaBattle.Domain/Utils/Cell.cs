using System;

namespace SeaBattle.Domain
{
    public class Cell : ICloneable
    {
        public bool HasDeck { get; set; } = false;
        public bool IsOpened { get; set; } = false;

        public object Clone()
        {
            return new Cell { HasDeck = HasDeck, IsOpened = IsOpened };
        }
    }
}
