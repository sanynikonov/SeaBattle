﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SeaBattle.Domain
{
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
    }
}
