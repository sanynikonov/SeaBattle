using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace SeaBattle.Domain
{
    [DebuggerDisplay("X: {X}; Y: {Y}")]
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Point first, Point second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(Point first, Point second)
        {
            return !first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point point))
            {
                return false;
            }
            
            return X == point.X && Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public Point[] GetNeighbours()
        {
            return new Point[]
            {
                new Point(X - 1, Y - 1),
                new Point(X - 1, Y),
                new Point(X - 1, Y + 1),
                new Point(X, Y - 1),
                new Point(X, Y + 1),
                new Point(X + 1, Y - 1),
                new Point(X + 1, Y),
                new Point(X + 1, Y + 1)
            };
        }

        public IEnumerable<Point> InclusiveRangeTo(Point second)
        {
            var first = this;
            var isHorizontal = first.IsHorizontalWith(second);
            var distanceRange = Enumerable.Range(0, first.CountDistanceBetween(second));

            return isHorizontal
                ? distanceRange.Select(i => new Point(first.X, first.Y + i))
                : distanceRange.Select(i => new Point(first.X + i, first.Y));
        }

        public int CountDistanceBetween(Point second)
        {
            var first = this;
            return Math.Abs(second.X - first.X) != 0
                ? Math.Abs(second.X - first.X) + 1
                : Math.Abs(second.Y - first.Y) + 1;
        }

        public bool IsInRange(int min, int max)
        {
            return this.X >= min && this.X < max
                && this.Y >= min && this.Y < max;
        }

         public bool IsVerticalWith(Point second)
        {
            return this.Y == second.Y;
        }

         public bool IsHorizontalWith(Point second)
        {
            return this.X == second.X;
        }

        public bool IsNeighbourTo(Point second)
        {
            return Math.Abs(this.X - second.X) == 1 || Math.Abs(this.Y - second.Y) == 1;
        }
    }
}
