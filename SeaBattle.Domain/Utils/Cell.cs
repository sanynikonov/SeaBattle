using System;

namespace SeaBattle.Domain
{
    public class Cell
    {
        internal bool HasDeck { get; set; } = false;
        public bool IsOpened { get; set; } = false;
        public Point Coordinates { get; set; }

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
    }
}
