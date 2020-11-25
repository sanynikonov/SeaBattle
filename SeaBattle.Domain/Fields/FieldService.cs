using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class FieldService : IFieldService
    {
        // Этот метод нам не нужен здесь. Перенесла его в класс Field
        // public Field GetFieldCopy(Field field)
        // {
        //     AssertIsNotNull(field);

        //     return field.Clone() as Field;
        // }  

        public void OpenCell(Field field, Point coordinates)
        {
            AssertIsNotNull(field);

            if (!coordinates.IsInRange(0, field.Dimension))
            {
                throw new FieldServiceException("Coordinates out of range");
            }

            if (field.Cells[coordinates.X, coordinates.Y].IsOpened)
            {
                throw new FieldServiceException("Cell is already opened");
            }

            field.Cells[coordinates.X, coordinates.Y].IsOpened = true;

            if (ShipIsDestroyed(field, coordinates))
            {
                OpenCellsNearDestroyedShip(field, coordinates);
            }
        }

        public Point[] GetDamagedShipsCheckedDecksCoordinates(Field field)
        {
            AssertIsNotNull(field);

            // var ships = GetShipsCoordinates(field);
            var shipCells = GetShipsCoordinates(field);

            // var damagedButNotKilledShips = shipCoordinates
            //     .Where(ship => ship.Any(deck => deck.IsOpened) && ship.Any(deck => !deck.IsOpened));

            var damagedButNotKilledShipCells = shipCells
                .Where(ship => ship.Any(deck => deck.IsOpened) && ship.Any(deck => !deck.IsOpened));

            var damagedDecks = damagedButNotKilledShipCells
                .SelectMany(x => x)
                .Where(x => x.CurrentState == CellState.OpenedWithDeck);

            return GetCellsCoordinates(field, damagedDecks).ToArray();
        }

        private IEnumerable<Cell[]> GetShipsCoordinates(Field field)
        {
            var ships = new List<Cell[]>();
            Cell[] ship;

            for (int i = 0; i < field.Dimension; i++)
            {
                for (int j = 0; j < field.Dimension; j++)
                {
                    if (field.Cells[i, j].HasDeck && !ships.Any(x => x.Contains(field.Cells[i, j])))
                    {
                        ship = GetShipCells(field, new Point(i, j)).ToArray();
                        ships.Add(ship);
                    }
                }
            }

            return ships;
        }

        private void OpenCellsNearDestroyedShip(Field field, Point coordinates)
        {
            //var shipCells = GetShipCells(field, coordinates).ToList();
            var shipCells = GetShipCells(field, coordinates);

            var cellsCoordinates = GetCellsCoordinates(field, shipCells);

            foreach (var point in cellsCoordinates)
            {
                OpenCellsAround(field, point);
            }
        }

        private IEnumerable<Point> GetCellsCoordinates(Field field, IEnumerable<Cell> cell)
        {
            var array = field.Cells.Cast<Cell>().ToArray();

            var absoluteIndeces = cell.Select(x => Array.IndexOf(array, x));

            return absoluteIndeces.Select(index => new Point { X = index / field.Dimension, Y = index % field.Dimension });
        }

        private void OpenCellsAround(Field field, Point coordinates)
        {
            void TryOpenCell(Point point)
            {
                if (point.IsInRange(0, field.Dimension))
                {
                    field.Cells[point.X, point.Y].IsOpened = true;
                }
            }

            var neighbours = coordinates.GetNeighbours();

            foreach (var coords in neighbours)
            {
                TryOpenCell(coords);
            }
        }

        private bool ShipIsDestroyed(Field field, Point coordinates)
        {
            if (!field.Cells[coordinates.X, coordinates.Y].HasDeck)
            {
                return false;
            }

            var shipCells = GetShipCells(field, coordinates);

            return shipCells.All(cell => cell.IsOpened);
        }

        private IEnumerable<Cell> GetShipCells(Field field, Point coordinates)
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
                while (deltaX < field.Dimension && field.Cells[deltaX, coordinates.Y].HasDeck)
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

                deltaY = coordinates.Y + 1;
                while (deltaY < field.Dimension && field.Cells[coordinates.X, deltaY].HasDeck)
                {
                    cells.Add(field.Cells[coordinates.X, deltaY]);
                    deltaY++;
                }

                return cells;
            }


            var shipIsPlacedHorizontally = (coordinates.X - 1 >= 0 && field.Cells[coordinates.X - 1, coordinates.Y].HasDeck)
                || (coordinates.X + 1 < field.Dimension && field.Cells[coordinates.X + 1, coordinates.Y].HasDeck);

            if (shipIsPlacedHorizontally)
            {
                return GetShipHorizontally(coordinates);
            }
            else
            {
                return GetShipVertically(coordinates);
            }
        }

        private void AssertIsNotNull(Field field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
        }
    }
}
