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
                if (wreckedDecks.Any(d => d.IsNeighbourTo(deck)))
                {
                    var neighbourDeck = wreckedDecks
                        .FirstOrDefault(d => d.IsNeighbourTo(deck));

                    var isHorizontalDirection = deck.IsHorizontalWith(neighbourDeck);

                    return new CorrectDirectionShootStrategy(fieldService, deck, isHorizontalDirection);
                }
            }

            return new ShootAroundWreckedShipStrategy(fieldService, wreckedDecks.First());
        }
    }
}
