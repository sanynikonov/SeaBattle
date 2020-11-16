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
            var unavaliableToPlaceShip = new bool[dimension, dimension];
            var random = new Random();
            var checkedPoints = new List<Point>();

            List<Point> availiblePoints;
            Point firstPoint;
            bool isHorizontalDirection;

            var shipsPositions = new List<(Point first, Point second)>();

            foreach (var pair in shipStorage.OrderBy(x => (int)x.Key))
            {
                for (int i = 0; i < pair.Value; i++)
                {
                    availiblePoints =
                        Enumerable.Repeat(0, dimension)
                            .Select(x =>
                                Enumerable.Repeat(0, dimension)
                                    .Select(y => new Point(x, y)))
                            .SelectMany(x => x)
                            .Where(p => !unavaliableToPlaceShip[p.X, p.Y])
                            .ToList();

                    do
                    {
                        firstPoint = availiblePoints[random.Next(availiblePoints.Count)];
                        isHorizontalDirection = random.Next(2) == 0;
                        availiblePoints.Remove(firstPoint);
                    }
                    while (!IsFirstPointValidToPlaceShip(firstPoint, pair.Key, isHorizontalDirection, unavaliableToPlaceShip)
                        || checkedPoints.Contains(firstPoint));

                    var shipLength = (int)pair.Key;

                    var shipPoints = Enumerable.Range(0, shipLength)
                        .Select(x => isHorizontalDirection
                            ? new Point(firstPoint.X + shipLength, firstPoint.Y)
                            : new Point(firstPoint.X, firstPoint.Y + shipLength));

                    var pointsToClaim = shipPoints
                        .Select(p => p.GetNeighbours())
                        .SelectMany(x => x)
                        .Distinct()
                        .Where(p => p.IsInRange(0, dimension))
                        .Concat(shipPoints);

                    foreach (var point in pointsToClaim)
                    {
                        unavaliableToPlaceShip[point.X, point.Y] = true;
                    }

                    shipsPositions.Add((firstPoint, shipPoints.Last()));
                }
            }

            return shipsPositions;
        }

        private IEnumerable<(Point first, Point second)> GetRandomShipsPositions1(int dimension, IReadOnlyDictionary<ShipType, int> shipStorage)
        {
            var random = new Random();

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
            bool isHorizontalDirection;
            int randomIndex;
            var ships = shipStorage.SelectMany(x => Enumerable.Repeat(x.Key, x.Value));
            
            foreach (var shipType in ships)
            {
                var availiblePoints = new List<Point>(freePoints);

                do
                {
                    if (!availiblePoints.Any())
                    {
                        throw new Exception($"Director couldn't find appropriate place for the ship of type {shipType}");
                    }

                    randomIndex = random.Next(availiblePoints.Count);

                    firstPoint = availiblePoints[randomIndex];
                    isHorizontalDirection = random.Next(2) == 0;
                        
                    availiblePoints.Remove(firstPoint);
                }
                while (FirstPointIsValidToPlaceShip(firstPoint, shipType, isHorizontalDirection, freePoints));

                var shipPoints = isHorizontalDirection
                    ? Enumerable.Range(0, dimension).Select(i => new Point(firstPoint.X, firstPoint.Y + i))
                    : Enumerable.Range(0, dimension).Select(i => new Point(firstPoint.X + i, firstPoint.Y));

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

        private bool FirstPointIsValidToPlaceShip(Point firstPoint, ShipType shipType, bool isHorizontalDirection, List<Point> freePoints)
        {
            var shipLength = (int)shipType;

            var shipPoints = isHorizontalDirection
                ? Enumerable.Range(0, shipLength).Select(i => new Point(firstPoint.X, firstPoint.Y + i))
                : Enumerable.Range(0, shipLength).Select(i => new Point(firstPoint.X + i, firstPoint.Y));

            return shipPoints.All(p => freePoints.Contains(p));
        }


        private bool IsFirstPointValidToPlaceShip(Point point, ShipType shipType, bool isHorizontalDirection, bool[,] unavaliableToPlaceShip)
        {
            var shipLength = (int)shipType;

            for (int i = 0; i < shipLength; i++)
            {
                if ((isHorizontalDirection && !unavaliableToPlaceShip[point.X + i, point.Y]) ||
                    (!isHorizontalDirection && !unavaliableToPlaceShip[point.X, point.Y + i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
