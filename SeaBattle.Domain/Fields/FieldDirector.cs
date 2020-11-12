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

            var shipsPositions = new List<(Point first, Point second)>();

            foreach (var pair in shipStorage.OrderBy(x => (int)x.Key))
            {
                for (int i = 0; i < pair.Value; i++)
                {
                    Point firstPoint;
                    bool isHorizontalDirection;
                    do
                    {
                        firstPoint = new Point(random.Next(dimension), random.Next(dimension));
                        isHorizontalDirection = random.Next(2) == 0;
                        checkedPoints.Add(firstPoint);
                    }
                    while (!IsFirstPointValidToPlaceShip(firstPoint, pair.Key, isHorizontalDirection, unavaliableToPlaceShip)
                        || checkedPoints.Contains(firstPoint));

                    var shipLength = (int)pair.Key;

                    var shipPoints = Enumerable.Range(0, shipLength)
                        .Select(x => isHorizontalDirection
                            ? new Point(firstPoint.X + shipLength, firstPoint.Y)
                            : new Point(firstPoint.X, firstPoint.Y + shipLength));                    

                    var pointsToClaim = shipPoints
                        .Select(p => GetPointWithNeighbours(p.X, p.Y))
                        .SelectMany(x => x)
                        .Distinct()
                        .Where(p => p.X >= 0 && p.X < dimension && p.Y >= 0 && p.Y < dimension);

                    foreach (var point in pointsToClaim)
                    {
                        unavaliableToPlaceShip[point.X, point.Y] = true;
                    }

                    shipsPositions.Add((firstPoint, shipPoints.Last()));
                    checkedPoints.Clear();
                }
            }

            return shipsPositions;
        }

        private static Point[] GetPointWithNeighbours(int x, int y)
        {
            return new Point[]
            {
                new Point(x - 1, y - 1),
                new Point(x - 1, y),
                new Point(x - 1, y + 1),
                new Point(x, y - 1),
                new Point(x, y),
                new Point(x, y + 1),
                new Point(x + 1, y - 1),
                new Point(x + 1, y),
                new Point(x + 1, y + 1)
            };
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
