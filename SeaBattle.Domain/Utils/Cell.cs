using System;

namespace SeaBattle.Domain
{
    public class Cell : ICloneable
    {
        internal bool HasDeck { get; set; } = false;
        public bool IsOpened { get; set; } = false;

        public CellState CurrentState
        {
            get
            {
                if (!IsOpened)
                    return CellState.Closed;

                if (HasDeck)
                    return CellState.OpenedWithDeck;

                return CellState.OpenedEmpty;
            }
        }

        public object Clone()
        {
            return new Cell { HasDeck = HasDeck, IsOpened = IsOpened };
        }
    }
}
