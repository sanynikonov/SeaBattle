using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameStateHolder
    {
        public static GameStateHolder Instance { get; private set; }

        private GameStateHolder() { }

        public Player Winner { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public Player FirstPlayer { get; private set; }
        public Player SecondPlayer { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.NotStarted;

        public Field CurrentPlayerOppositeField => CurrentPlayer == FirstPlayer ? secondPlayerField : firstPlayerField;

        public void SwitchCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == FirstPlayer ? SecondPlayer : FirstPlayer;
        }

        public void AssertGameStarted()
        {
            if (CurrentState != GameState.Started)
            {
                throw new GameServiceException("Game is not started yet.");
            }
        }

        public void AssertGameIsNotEnded()
        {
            if (GameStateHolder.Instance.CurrentState == GameState.Ended)
            {
                throw new GameServiceException("Game is already ended.");
            }
        }
    }
}
