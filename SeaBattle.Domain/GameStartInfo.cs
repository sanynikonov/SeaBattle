using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameStartInfo
    {
        public Player FirstPlayer { get; }
        public Player SecondPlayer { get; }
        public IFieldService FirstPlayerFieldService { get; }
        public IFieldService SecondPlayerFieldService { get; }

        public GameStartInfo(Player firstPlayer, Player secondPlayer, IFieldService firstPlayerFieldService, IFieldService secondPlayerFieldService)
        {
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            FirstPlayerFieldService = firstPlayerFieldService;
            SecondPlayerFieldService = secondPlayerFieldService;
        }
    }
}
