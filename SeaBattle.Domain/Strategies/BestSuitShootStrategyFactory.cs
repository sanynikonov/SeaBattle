using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class BestSuitShootStrategyFactory
    {
        private readonly IFieldService fieldService;

        public BestSuitShootStrategyFactory(IFieldService fieldService)
        {
            this.fieldService = fieldService;
        }

        public IShootStrategy GetShootStrategy(Point[] wreckedDecks)
        {
            if (wreckedDecks == null || !wreckedDecks.Any())
            {
                return new RandomShootStrategy(fieldService);
            }

            foreach (var deck in wreckedDecks)
            {
                if (wreckedDecks.Any(d => AreNeighbours(d, deck)))
                {
                    var neighbourDeck = wreckedDecks
                        .FirstOrDefault(d => AreNeighbours(d, deck));

                    var isHorizontalDirection = deck.X - neighbourDeck.X == 0;

                    return new CorrectDirectionShootStrategy(fieldService, deck, isHorizontalDirection);
                }
            }

            return new ShootAroundWreckedShipStrategy(fieldService, wreckedDecks.First());
        }

        private bool AreNeighbours(Point first, Point second)
        {
            return Math.Abs(first.X - second.X) == 1 || Math.Abs(first.Y - second.Y) == 1;
        }
    }
}
