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

        private void OpenCellsNearDestroyedShip(Point coordinates)
        {
            var shipCells = GetShipCells(coordinates).ToList();

            var array = field.Cells.Cast<Cell>().ToArray();

            foreach (var cell in shipCells)
            {
                var absoluteIndex = Array.IndexOf(array, cell);

                var point = new Point { X = absoluteIndex / field.Dimension, Y = absoluteIndex % field.Dimension };

                OpenCellsAround(point);
            }
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

            TryOpenCell(x - 1, y - 1);
            TryOpenCell(x - 1, y);
            TryOpenCell(x - 1, y + 1);

            TryOpenCell(x, y - 1);
            //cells[x, y].IsOpened = true;
            TryOpenCell(x, y + 1);

            TryOpenCell(x + 1, y - 1);
            TryOpenCell(x + 1, y);
            TryOpenCell(x + 1, y + 1);
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
