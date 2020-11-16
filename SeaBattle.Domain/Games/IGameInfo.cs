using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IGameInfo
    {
        Player FirstPlayer { get; }
        Player SecondPlayer { get; }

        Field FirstPlayerFieldCopy { get; }
        Field SecondPlayerFieldCopy { get; }

        GameState CurrentState { get; }
        Player CurrentPlayer { get; }
        Player Winner { get; }
    }
}
