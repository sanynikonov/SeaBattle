using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class BestSuitFindCellStrategyFactory
    {
        public IFindCellStrategy GetFindCellStrategy(Point[] wreckedDecks)
        {
            if (wreckedDecks == null || !wreckedDecks.Any())
            {
                return new RandomFindCellStrategy();
            }

            foreach (var deck in wreckedDecks)
            {
                if (wreckedDecks.Any(d => d.IsNeighbourTo(deck)))
                {
                    var neighbourDeck = wreckedDecks
                        .FirstOrDefault(d => d.IsNeighbourTo(deck));

                    var isHorizontalDirection = deck.IsHorizontalWith(neighbourDeck);

                    return new CorrectDirectionFindCellStrategy(deck, isHorizontalDirection);
                }
            }

            return new FindCellAroundWreckedShipStrategy(wreckedDecks.First());
        }
    }
}
