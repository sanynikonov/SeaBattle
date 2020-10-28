using System;
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
    }
}
