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
                throw new Exception($"{nameof(shipsAmount)} is null.");
            }

            if (FieldWillHaveTooBigShipsDensity(shipsAmount, Result.Dimension))
            {
                throw new Exception("Passed ships amount will cause too big ships density.");
            }

            availibleShipsToPlace = shipsAmount;
        }

        public void SetDimension(int dimension)
        {
            if (dimension <= 0)
            {
                throw new Exception("Dimension should be more than zero.");
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
                throw new Exception("Arguments cause diagonal ship direction.");
            }

            var shipLength = GetShipLength(first, second);

            if (!Enum.TryParse<ShipType>(shipLength.ToString(), out var shipType))
            {
                throw new Exception("Unknown ship type.");
            }

            if (!availibleShipsToPlace.TryGetValue(shipType, out var shipsLeftCount) || shipsLeftCount == 0)
            {
                throw new Exception("No availible ships for current ship type.");
            }

            var cells = GetCellsToPlaceShip(first, second);

            if (cells.Any(c => c.HasDeck))
            {
                throw new Exception("There is already set ship on this coordinates.");
            }

            cells.ForEach(c => c.HasDeck = true);

            availibleShipsToPlace[shipType]--;
        }

        private static bool FieldWillHaveTooBigShipsDensity(Dictionary<ShipType, int> shipsAmount, int dimension)
        {
            const double highestAvailibleDensityCoef = 1.22;

            var decksCount = shipsAmount.Sum(x => (int)x.Key * x.Value);
            var maxNeighbourCellsCount = shipsAmount.Sum(x => ((int)x.Key * 2 + 6) * x.Value);

            return decksCount + maxNeighbourCellsCount / Math.Pow(dimension, 2) > highestAvailibleDensityCoef;
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
                throw new Exception("Coordinates are out of range.");
            }
        }
    }
}
