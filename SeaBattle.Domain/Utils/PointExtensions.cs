using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public static class PointExtensions
    {
        public static Point[] GetNeighbours(this Point point)
        {
            var x = point.X;
            var y = point.Y;

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

        public static IEnumerable<Point> InclusiveRangeTo(this Point first, Point second)
        {
            var isHorizontal = first.IsHorizontalWith(second);
            var distanceRange = Enumerable.Range(0, first.CountDistanceBetween(second));

            return isHorizontal
                ? distanceRange.Select(i => new Point(first.X, first.Y + i))
                : distanceRange.Select(i => new Point(first.X + i, first.Y));
        }

        public static int CountDistanceBetween(this Point first, Point second)
        {
            return Math.Abs(second.X - first.X) != 0
                ? Math.Abs(second.X - first.X) + 1
                : Math.Abs(second.Y - first.Y) + 1;
        }

        public static bool IsInRange(this Point point, int min, int max)
        {
            return point.X >= min && point.X < max
                && point.Y >= min && point.Y < max;
        }

        public static bool IsVerticalWith(this Point first, Point second)
        {
            return first.Y == second.Y;
        }

        public static bool IsHorizontalWith(this Point first, Point second)
        {
            return first.X == second.X;
        }

        public static bool IsNeighbourTo(this Point first, Point second)
        {
            return Math.Abs(first.X - second.X) == 1 || Math.Abs(first.Y - second.Y) == 1;
        }
    }
}
