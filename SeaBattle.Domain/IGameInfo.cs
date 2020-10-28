using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IGameInfo
    {
        Player FirstPlayer { get; }
        Player SecondPlayer { get; }

        Field FirstPlayerField { get; }
        Field SecondPlayerField { get; }

        GameState CurrentState { get; }
        Player Winner { get; }
        Player CurrentPlayer { get; }
    }
}
