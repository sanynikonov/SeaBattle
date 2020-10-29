using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class FieldService : IFieldService
    {
        private readonly Field field;

        public FieldService(Field field)
        {
            this.field = field;
        }

        public Field FieldCopy => field.Clone() as Field;

        public void OpenCell(Point coordinates)
        {
            if (coordinates.X >= field.Dimension || coordinates.Y >= field.Dimension)
            {
                throw new Exception("Coordinates out of range");
            }

            if (field.Cells[coordinates.X, coordinates.Y].IsOpened)
            {
                throw new Exception("Cell is already opened");
            }

            field.Cells[coordinates.X, coordinates.Y].IsOpened = true;

            if (ShipIsDestroyed(coordinates))
            {
                OpenCellsNearDestroyedShip(coordinates);
            }
        }

        public Point[] GetWreckedDecksOfDamagedShips()
        {
            var ships = GetShipsCoordinates();

            var damagedButNotKilledShips = ships
                .Where(ship => ship.Any(deck => deck.IsOpened) && ship.Any(deck => !deck.IsOpened));

            var damagedDecks = damagedButNotKilledShips
                .SelectMany(x => x)
                .Where(x => x.CurrentState == CellState.OpenedWithDeck);

            return GetCellsCoordinates(damagedDecks).ToArray();
        }

        private IEnumerable<Cell[]> GetShipsCoordinates()
        {
            var ships = new List<Cell[]>();
            Cell[] ship;

            for (int i = 0; i < field.Dimension; i++)
            {
                for (int j = 0; j < field.Dimension; j++)
                {
                    if (field.Cells[i, j].HasDeck && !ships.Any(x => x.Contains(field.Cells[i, j])))
                    {
                        ship = GetShipCells(new Point(i, j)).ToArray();
                        ships.Add(ship);
                    }
                }
            }

            return ships;
        }

        private bool FieldHasCoordinates(int x, int y)
        {
            return x >= 0 && x < field.Dimension && y >= 0 && y < field.Dimension;
        }

        private static Point[] GetNeighbours(int x, int y)
        {
            return new Point[]
            {
                new Point(x - 1, y - 1),
                new Point(x - 1, y),
                new Point(x - 1, y + 1),
                new Point(x, y - 1),
                new Point(x, y + 1),
                new Point(x + 1, y - 1),
                new Point(x + 1, y),
                new Point(x + 1, y + 1)
            };
        }

        private void OpenCellsNearDestroyedShip(Point coordinates)
        {
            var shipCells = GetShipCells(coordinates).ToList();

            var cellsCoordinates = GetCellsCoordinates(shipCells);

            foreach (var point in cellsCoordinates)
            {
                OpenCellsAround(point);
            }
        }

        private IEnumerable<Point> GetCellsCoordinates(IEnumerable<Cell> cell)
        {
            var array = field.Cells.Cast<Cell>().ToArray();

            var absoluteIndeces = cell.Select(x => Array.IndexOf(array, x));

            return absoluteIndeces.Select(index => new Point { X = index / field.Dimension, Y = index % field.Dimension });
        }

        private void OpenCellsAround(Point coordinates)
        {
            void TryOpenCell(int x, int y)
            {
                if (x >= 0 && x < field.Dimension && y >= 0 && y < field.Dimension)
                {
                    field.Cells[x, y].IsOpened = true;
                }
            }

            var x = coordinates.X;
            var y = coordinates.Y;

            var neighbours = GetNeighbours(x, y);

            foreach (var coords in neighbours)
            {
                TryOpenCell(coords.X, coords.Y);
            }
        }

        private bool ShipIsDestroyed(Point coordinates)
        {
            if (!field.Cells[coordinates.X, coordinates.Y].HasDeck)
            {
                return false;
            }

            var shipCells = GetShipCells(coordinates);

            return shipCells.All(cell => cell.IsOpened);
        }

        private IEnumerable<Cell> GetShipCells(Point coordinates)
        {
            IEnumerable<Cell> GetShipHorizontally(Point coordinates)
            {
                var cells = new List<Cell>();

                int deltaX = coordinates.X - 1;
                while (deltaX >= 0 && field.Cells[deltaX, coordinates.Y].HasDeck)
                {
                    cells.Add(field.Cells[deltaX, coordinates.Y]);
                    deltaX--;
                }

                cells.Reverse();
                cells.Add(field.Cells[coordinates.X, coordinates.Y]);

                deltaX = coordinates.X + 1;
                while (deltaX < field.Cells.GetLength(0) && field.Cells[deltaX, coordinates.Y].HasDeck)
                {
                    cells.Add(field.Cells[deltaX, coordinates.Y]);
                    deltaX++;
                }

                return cells;
            }

            IEnumerable<Cell> GetShipVertically(Point coordinates)
            {
                var cells = new List<Cell>();

                int deltaY = coordinates.Y - 1;
                while (deltaY >= 0 && field.Cells[coordinates.X, deltaY].HasDeck)
                {
                    cells.Add(field.Cells[coordinates.X, deltaY]);
                    deltaY--;
                }

                cells.Reverse();
                cells.Add(field.Cells[coordinates.X, coordinates.Y]);

                deltaY = coordinates.Y - 1;
                while (deltaY < field.Cells.GetLength(1) && field.Cells[coordinates.X, deltaY].HasDeck)
                {
                    cells.Add(field.Cells[coordinates.X, deltaY]);
                    deltaY++;
                }

                return cells;
            }


            var shipIsPlacedHorizontally = (coordinates.X - 1 >= 0 && field.Cells[coordinates.X - 1, coordinates.Y].HasDeck)
                || (coordinates.X + 1 < field.Cells.GetLength(0) && field.Cells[coordinates.X - 1, coordinates.Y].HasDeck);

            if (shipIsPlacedHorizontally)
            {
                return GetShipHorizontally(coordinates);
            }
            else
            {
                return GetShipVertically(coordinates);
            }
        }
    }
}
