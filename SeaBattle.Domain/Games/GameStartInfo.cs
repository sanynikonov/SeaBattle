using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameStartInfo
    {
        public Player FirstPlayer { get; }
        public Player SecondPlayer { get; }

        public GameStartInfo(Player firstPlayer, Player secondPlayer)
        {
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
        }
    }
}
