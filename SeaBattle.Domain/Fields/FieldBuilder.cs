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

            availibleShipsToPlace = new Dictionary<ShipType, int>(shipsAmount);

            Result.ShipsCount = new Dictionary<ShipType, int>(shipsAmount);
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
                    Result.Cells[i, j] = new Cell { Coordinates = new Point(i, j) };
                }
            }
        }

        public void SetShip(Point first, Point second)
        {
            ThrowExceptionIfCoordinatesAreOutOfRange(first);
            ThrowExceptionIfCoordinatesAreOutOfRange(second);

            if (!first.IsHorizontalWith(second) && !first.IsVerticalWith(second))
            {
                throw new FieldBuilderException("Arguments cause diagonal ship direction.");
            }

            var shipLength = first.CountDistanceBetween(second);

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

        private bool NeighbourCellsHaveDeck(Point first, Point second)
        {
            IEnumerable<Point> shipCoordinates;

            shipCoordinates = first.InclusiveRangeTo(second);

            return NeighbourCellsHaveDeck(shipCoordinates);
        }

        private bool NeighbourCellsHaveDeck(IEnumerable<Point> shipCoordinates)
        {
            return shipCoordinates
                .Select(p => p.GetNeighbours())
                .SelectMany(x => x)
                .Any(p => p.IsInRange(0, Result.Dimension)
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

        private List<Cell> GetCellsToPlaceShip(Point first, Point second)
        {
            var points = first.InclusiveRangeTo(second);

            return points.Select(p => Result.Cells[p.X, p.Y]).ToList();
        }

        private void ThrowExceptionIfCoordinatesAreOutOfRange(Point coordinates)
        {
            if (!coordinates.IsInRange(0, Result.Dimension))
            {
                throw new FieldBuilderException("Coordinates are out of range.");
            }
        }
    }
}
