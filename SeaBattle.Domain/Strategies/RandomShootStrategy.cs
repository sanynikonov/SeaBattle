using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class RandomShootStrategy : IShootStrategy
    {
        public void Shoot(IFieldService fieldService)
        {
            var coordinates = FindRandomCellToOpen(fieldService);

            fieldService.OpenCell(coordinates);
        }

        private Point FindRandomCellToOpen(IFieldService fieldService)
        {
            var random = new Random();
            var field = fieldService.FieldCopy;

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
