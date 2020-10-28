using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IFieldBuilder
    {
        void Reset();
        void SetShipsStorage(Dictionary<ShipType, int> shipsAmount);
        void SetDimension(int dimension);
        void SetShip(Point first, Point second);
        Field Result { get; }
    }
}
