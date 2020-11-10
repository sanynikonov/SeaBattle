using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class RandomFindCellStrategy : IFindCellStrategy
    {
        public Point FindCell(Field field)
        {
            var random = new Random();

            int randomX;
            int randomY;

            do
            {
                randomX = random.Next(0, field.Dimension);
                randomY = random.Next(0, field.Dimension);
            }
            while (field.Cells[randomX, randomY].IsOpened);

            return new Point(randomX, randomY);
        }
    }
}
