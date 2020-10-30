using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class BoardStatus
    {
        public Point[] FirstFieldWoundedShipsCoordinates { get; set; }
        public Point[] SecondFieldWoundedShipsCoordinates { get; set; }
    }
}
