using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class CorrectDirectionShootStrategy : IShootStrategy
    {
        private readonly Point wreckedDeckCoordinates;
        private readonly bool shipIsSetHorizontally;
        private readonly IFieldService fieldService;

        public CorrectDirectionShootStrategy(IFieldService fieldService, Point wreckedDeckCoordinates, bool shipIsSetHorizontally)
        {
            this.fieldService = fieldService;
            this.wreckedDeckCoordinates = wreckedDeckCoordinates;
            this.shipIsSetHorizontally = shipIsSetHorizontally;
        }

        public void Shoot(Field field)
        {
            var coordinates = FindNeighbourCellToOpen(field);

            fieldService.OpenCell(field, coordinates);
        }

        private Point FindNeighbourCellToOpen(Field field)
        {
            var random = new Random();

            var potentialCoordinates = GetNeighbourUnopenedCells(field);

            var index = random.Next(0, potentialCoordinates.Length);

            return potentialCoordinates[index];
        }

        private Point[] GetNeighbourUnopenedCells(Field field)
        {
            Point[] potentialCoordinates;

            if (shipIsSetHorizontally)
            {
                int minX = wreckedDeckCoordinates.X - 1;
                while (minX >= 0 && field.Cells[minX, wreckedDeckCoordinates.Y].IsOpened) minX--;

                int maxX = wreckedDeckCoordinates.X + 1;
                while (maxX < field.Dimension && field.Cells[maxX, wreckedDeckCoordinates.Y].IsOpened) maxX++;

                potentialCoordinates = new[]
                {
                    new Point(minX, wreckedDeckCoordinates.Y),
                    new Point(maxX, wreckedDeckCoordinates.Y),
                };
            }
            else
            {
                int minY = wreckedDeckCoordinates.X - 1;
                while (minY >= 0 && field.Cells[wreckedDeckCoordinates.X, minY].IsOpened) minY--;

                int maxY = wreckedDeckCoordinates.X + 1;
                while (maxY < field.Dimension && field.Cells[wreckedDeckCoordinates.X, maxY].IsOpened) maxY++;

                potentialCoordinates = new[]
                {
                    new Point(wreckedDeckCoordinates.X, minY),
                    new Point(wreckedDeckCoordinates.X, maxY),
                };
            }

            return potentialCoordinates.Where(p => !field.Cells[p.X, p.Y].IsOpened).ToArray();
        }
    }
}
