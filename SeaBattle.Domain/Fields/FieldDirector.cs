using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class FieldDirector
    {
        public void BuildClassicField(IFieldBuilder fieldBuilder)
        {
            if (fieldBuilder == null)
            {
                throw new ArgumentNullException(nameof(fieldBuilder));
            }

            var dimension = 10;
            var classicShipStorage =
                new Dictionary<ShipType, int>
                {
                    { ShipType.SingleDeck, 4 },
                    { ShipType.DoubleDeck, 3 },
                    { ShipType.ThreeDeck, 2 },
                    { ShipType.FourDeck, 1 }
                };

            fieldBuilder.SetDimension(dimension);
            fieldBuilder.SetShipsStorage(classicShipStorage);
        }

        public void SetShipsRandomly(IFieldBuilder fieldBuilder)
        {
            if (fieldBuilder == null)
            {
                throw new ArgumentNullException(nameof(fieldBuilder));
            }

            var dimension = fieldBuilder.Result.Dimension;
            var shipStorage = fieldBuilder.Result.ShipsCount;

            var shipsPositions = GetRandomShipsPositions(dimension, shipStorage);

            foreach (var (first, second) in shipsPositions)
            {
                fieldBuilder.SetShip(first, second);
            }
        }
        
        private IEnumerable<(Point first, Point second)> GetRandomShipsPositions(int dimension, IReadOnlyDictionary<ShipType, int> shipStorage)
        {
            List<Point> freePoints = new List<Point>();

            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    freePoints.Add(new Point(i, j));
                }
            }

            var shipsPositions = new List<(Point first, Point second)>();

            Point firstPoint;
            Point lastPoint;

            var ships = shipStorage.SelectMany(x => Enumerable.Repeat(x.Key, x.Value));
            
            foreach (var shipType in ships)
            {
                (firstPoint, lastPoint) = FindAppropriateShipPosition(shipType, freePoints);

                var shipPoints = firstPoint.InclusiveRangeTo(lastPoint);

                var pointsToClaim = shipPoints
                    .SelectMany(p => p.GetNeighbours())
                    .Distinct()
                    .Where(p => p.IsInRange(0, dimension))
                    .Concat(shipPoints);

                foreach (var point in pointsToClaim)
                {
                    freePoints.Remove(point);
                }

                shipsPositions.Add((firstPoint, shipPoints.Last()));
            }

            return shipsPositions;
        }

        private (Point first, Point last) FindAppropriateShipPosition(ShipType shipType, List<Point> freePoints)
        {
            var random = new Random();
            var availiblePoints = new List<Point>(freePoints);
            var shipLength = (int)shipType;

            Point firstPoint;
            Point lastPoint;

            bool isHorizontalDirection;
            int randomIndex;

            do
            {
                if (!availiblePoints.Any())
                {
                    throw new FieldDirectorException($"Director couldn't find appropriate place for the ship of type {shipType}");
                }

                randomIndex = random.Next(availiblePoints.Count);
                firstPoint = availiblePoints[randomIndex];

                isHorizontalDirection = random.Next(2) == 0;
                lastPoint = isHorizontalDirection
                    ? new Point(firstPoint.X, firstPoint.Y + shipLength - 1)
                    : new Point(firstPoint.X + shipLength - 1, firstPoint.Y);

                availiblePoints.Remove(firstPoint);
            }
            while (!ShipPositionIsValid(firstPoint, lastPoint, freePoints));

            return (firstPoint, lastPoint);
        }

        private bool ShipPositionIsValid(Point firstPoint, Point lastPoint, List<Point> freePoints)
        {
            var shipPoints = firstPoint.InclusiveRangeTo(lastPoint);

            return shipPoints.All(p => freePoints.Contains(p));
        }
    }
}
