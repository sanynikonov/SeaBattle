using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class RandomShootStrategy : IShootStrategy
    {
        private readonly IFieldService fieldService;

        public RandomShootStrategy(IFieldService fieldService)
        {
            this.fieldService = fieldService;
        }

        public void Shoot(Field field)
        {
            var coordinates = FindRandomCellToOpen(field);

            fieldService.OpenCell(field, coordinates);
        }

        private Point FindRandomCellToOpen(Field field)
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
