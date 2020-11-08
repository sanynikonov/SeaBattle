using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameStartInfo
    {
        public Player FirstPlayer { get; }
        public Player SecondPlayer { get; }
        public Field FirstPlayerField { get; }
        public Field SecondPlayerField { get; }

        public GameStartInfo(Player firstPlayer, Player secondPlayer, Field firstPlayerField, Field secondPlayerField)
        {
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            FirstPlayerField = firstPlayerField;
            SecondPlayerField = secondPlayerField;
        }
    }
}
