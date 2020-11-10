using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class FindCellAroundWreckedShipStrategy : IFindCellStrategy
    {
        private readonly Point wreckedDeckCoordinates;

        public FindCellAroundWreckedShipStrategy(Point wreckedDeckCoordinates)
        {
            this.wreckedDeckCoordinates = wreckedDeckCoordinates;
        }

        public Point FindCell(Field field)
        {
            var random = new Random();

            var potentialPoints = new Point[]
            {
                new Point(wreckedDeckCoordinates.X + 1, wreckedDeckCoordinates.Y),
                new Point(wreckedDeckCoordinates.X - 1, wreckedDeckCoordinates.Y),
                new Point(wreckedDeckCoordinates.X, wreckedDeckCoordinates.Y + 1),
                new Point(wreckedDeckCoordinates.X, wreckedDeckCoordinates.Y - 1),
            };

            var validPoints = potentialPoints.Where(
                  p => p.IsInRange(0, field.Dimension) && !field.Cells[p.X, p.Y].IsOpened)
                .ToArray();

            var index = random.Next(0, validPoints.Length);

            return validPoints[index];
        }
    }
}
