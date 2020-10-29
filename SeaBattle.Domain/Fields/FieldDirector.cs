using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class FieldDirector
    {
        public void BuildClassicField(IFieldBuilder fieldBuilder)
        {
            var dimension = 10;

            var storage = new Dictionary<ShipType, int>
            {
                { ShipType.SingleDeck, 4 },
                { ShipType.DoubleDeck, 3 },
                { ShipType.ThreeDeck, 2 },
                { ShipType.FourDeck, 1 }
            };

            fieldBuilder.SetDimension(dimension);
            fieldBuilder.SetShipsStorage(storage);
        }
    }
}
