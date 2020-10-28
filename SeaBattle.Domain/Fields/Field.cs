using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SeaBattle.Domain
{
    public class Field : ICloneable
    {
        public IReadOnlyDictionary<int, int> ShipsCount { get; set; }
        public int Dimension => Cells.GetLength(0);
        public Cell[,] Cells { get; set; }

        public object Clone()
        {
            var copy = new Field { ShipsCount = ShipsCount, Cells = new Cell[Dimension, Dimension] };

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    copy.Cells[i, j] = Cells[i, j].Clone() as Cell;
                }
            }

            return copy;
        }
    }
}
