using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class FieldBuilder : IFieldBuilder
    {
        private Dictionary<ShipType, int> availibleShipsToPlace = new Dictionary<ShipType, int>();

        public Field Result { get; private set; } = new Field();

        public void Reset()
        {
            Result = new Field();
        }

        public void SetShipsStorage(Dictionary<ShipType, int> shipsAmount)
        {
            if (shipsAmount == null)
            {
                throw new FieldBuilderException($"{nameof(shipsAmount)} is null.");
            }

            if (ShipsStorageCausesHugeShipsDensity(shipsAmount, Result.Dimension))
            {
                throw new FieldBuilderException("Passed ships amount will cause too big ships density.");
            }

            availibleShipsToPlace = shipsAmount;

            Result.ShipsCount = shipsAmount;
        }

        public void SetDimension(int dimension)
        {
            if (dimension <= 0)
            {
                throw new FieldBuilderException("Dimension should be more than zero.");
            }

            Result.Cells = new Cell[dimension, dimension];

            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    Result.Cells[i, j] = new Cell();
                }
            }
        }

        public void SetShip(Point first, Point second)
        {
            ThrowExceptionIfCoordinatesAreOutOfRange(first);
            ThrowExceptionIfCoordinatesAreOutOfRange(second);

            if (first.X != second.X && first.Y != second.Y)
            {
                throw new FieldBuilderException("Arguments cause diagonal ship direction.");
            }

            var shipLength = GetShipLength(first, second);

            if (!Enum.TryParse<ShipType>(shipLength.ToString(), out var shipType))
            {
                throw new FieldBuilderException("Unknown ship type.");
            }

            if (!availibleShipsToPlace.TryGetValue(shipType, out var shipsLeftCount) || shipsLeftCount == 0)
            {
                throw new FieldBuilderException("No availible ships for current ship type.");
            }

            var cells = GetCellsToPlaceShip(first, second);

            if (cells.Any(c => c.HasDeck))
            {
                throw new FieldBuilderException("There is already set ship on this coordinates.");
            }

            if (NeighbourCellsHaveDeck(first, second))
            {
                throw new FieldBuilderException("Neighbour cells have decks.");
            }

            cells.ForEach(c => c.HasDeck = true);

            availibleShipsToPlace[shipType]--;
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

        private bool NeighbourCellsHaveDeck(Point first, Point second)
        {
            IEnumerable<Point> shipCoordinates;

            if (first.X == second.X)
            {
                shipCoordinates = Enumerable.Range(first.Y, first.Y - second.Y).Select(y => new Point(first.X, y));
            }
            else
            {
                shipCoordinates = Enumerable.Range(first.X, first.X - second.X).Select(x => new Point(x, first.Y));
            }

            return NeighbourCellsHaveDeck(shipCoordinates);
        }

        private bool NeighbourCellsHaveDeck(IEnumerable<Point> shipCoordinates)
        {
            return shipCoordinates
                .Select(p => GetNeighbours(p.X, p.Y))
                .SelectMany(x => x)
                .Any(p => p.X >= 0 && p.X < Result.Dimension
                        && p.Y >= 0 && p.Y < Result.Dimension
                        && Result.Cells[p.X, p.Y].HasDeck
                        && !shipCoordinates.Contains(p));
        }

        private static bool ShipsStorageCausesHugeShipsDensity(Dictionary<ShipType, int> shipsAmount, int dimension)
        {
            const double highestAvailibleDensityCoef = 1.21;

            var decksCount = shipsAmount.Sum(x => (int)x.Key * x.Value);
            var maxNeighbourCellsCount = shipsAmount.Sum(x => ((int)x.Key * 2 + 6) * x.Value);
            var densityCoef = (decksCount + maxNeighbourCellsCount) / Math.Pow(dimension, 2);

            return densityCoef > highestAvailibleDensityCoef;
        }

        private static int GetShipLength(Point first, Point second)
        {
            return second.X - first.X > 0 ? second.X - first.X + 1 : second.Y - second.Y + 1;
        }

        private List<Cell> GetCellsToPlaceShip(Point first, Point second)
        {
            var cells = new List<Cell>();

            if (first.X == second.X)
            {
                for (int i = 0; i < second.Y - first.Y; i++)
                {
                    cells.Add(Result.Cells[first.X, i]);
                }
            }
            else if (first.Y == second.Y)
            {
                for (int i = 0; i < second.X - first.X; i++)
                {
                    cells.Add(Result.Cells[i, first.Y]);
                }
            }

            return cells.ToList();
        }

        private void ThrowExceptionIfCoordinatesAreOutOfRange(Point coordinates)
        {
            if (coordinates.X < 0 || coordinates.X >= Result.Dimension ||
                coordinates.Y < 0 || coordinates.Y >= Result.Dimension)
            {
                throw new FieldBuilderException("Coordinates are out of range.");
            }
        }
    }
}
