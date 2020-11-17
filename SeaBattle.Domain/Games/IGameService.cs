using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IGameService
    {
        Player FirstPlayer { get; }
        Player SecondPlayer { get; }

        Field FirstPlayerFieldCopy { get; }
        Field SecondPlayerFieldCopy { get; }

        Player CurrentPlayer { get; }
        Player Winner { get; }
        GameState CurrentState { get; }

        void StartGame();
        BoardStatus MakeMove(Point coordinates);
        BoardStatus MakeMove(IFindCellStrategy strategy);
    }
}
