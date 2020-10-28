using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameService : IGameBehaviour, IGameInfo
    {
        private readonly IFieldService firstField;
        private readonly IFieldService secondField;

        private IFieldService CurrentField => CurrentPlayer == FirstPlayer ? firstField : secondField;

        public GameState CurrentState { get; private set; } = GameState.NotStarted;
        public Player Winner { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public Field FirstPlayerField => firstField.FieldCopy;
        public Field SecondPlayerField => secondField.FieldCopy;

        public Player FirstPlayer { get; private set; }
        public Player SecondPlayer { get; private set; }

        public GameService(GameStartInfo startInfo)
        {
            FirstPlayer = startInfo.FirstPlayer;
            SecondPlayer = startInfo.SecondPlayer;
            firstField = startInfo.FirstPlayerFieldService;
            secondField = startInfo.SecondPlayerFieldService;
        }

        public void MakeMove(Point coordinates)
        {
            CurrentField.OpenCell(coordinates);
            SwitchCurrentPlayer();
        }

        public virtual void MakeMove(IShootStrategy strategy)
        {
            strategy.Shoot(CurrentField);
            SwitchCurrentPlayer();
        }

        public virtual void StartGame()
        {
            if (CurrentState == GameState.Started)
            {
                throw new Exception("Game is already started.");
            }

            if (CurrentState == GameState.Ended)
            {
                throw new Exception("Game is already ended.");
            }

            CurrentState = GameState.Started;
        }

        protected void CheckGameEnd()
        {

        }

        protected virtual void SwitchCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == FirstPlayer ? SecondPlayer : FirstPlayer;
        }

        protected virtual void EndGame()
        {
            if (CurrentState == GameState.Ended)
            {
                throw new Exception("Game is ended already.");
            }

            if (CurrentState == GameState.NotStarted)
            {
                throw new Exception("Game is not started yet.");
            }

            CurrentState = GameState.Ended;
        }
    }
}
