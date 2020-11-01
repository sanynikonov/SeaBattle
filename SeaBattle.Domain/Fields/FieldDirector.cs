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
            var placingStatus = new bool[dimension, dimension];
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
                    while (!IsFirstPointValidToPlaceShip(firstPoint, pair.Key, isHorizontalDirection, placingStatus));

                    var shipLength = (int)pair.Key;

                    var lastPoint = isHorizontalDirection
                        ? new Point(firstPoint.X + shipLength, firstPoint.Y)
                        : new Point(firstPoint.X, firstPoint.Y + shipLength);

                    for (int j = 0; j < shipLength; j++)
                    {
                        if (isHorizontalDirection) placingStatus[firstPoint.X + j, firstPoint.Y] = true;
                        else placingStatus[firstPoint.X, firstPoint.Y + j] = true;
                    }

                    shipsPositions.Add((firstPoint, lastPoint));
                    checkedPoints.Clear();
                }
            }

            return shipsPositions;
        }

        private bool IsFirstPointValidToPlaceShip(Point point, ShipType shipType, bool isHorizontalDirection, bool[,] boardPlacingStatus)
        {
            var shipLength = (int)shipType;

            for (int i = 0; i < shipLength; i++)
            {
                if ((isHorizontalDirection && !boardPlacingStatus[point.X + i, point.Y]) ||
                    (!isHorizontalDirection && !boardPlacingStatus[point.X, point.Y + i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
