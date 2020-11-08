using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class ShootAroundWreckedShipStrategy : IShootStrategy
    {
        private readonly Point wreckedDeckCoordinates;
        private readonly IFieldService fieldService;

        public ShootAroundWreckedShipStrategy(IFieldService fieldService, Point wreckedDeckCoordinates)
        {
            this.fieldService = fieldService;
            this.wreckedDeckCoordinates = wreckedDeckCoordinates;
        }

        public void Shoot(Field field)
        {
            var coordinates = FindNeighbourCellToOpen(field);

            fieldService.OpenCell(field, coordinates);
        }

        private Point FindNeighbourCellToOpen(Field field)
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
                  p => p.X >= 0 && p.X < field.Dimension
                    && p.Y >= 0 && p.Y < field.Dimension
                    && !field.Cells[p.X, p.Y].IsOpened)
                .ToArray();

            var index = random.Next(0, validPoints.Length);

            return validPoints[index];
        }
    }
}
