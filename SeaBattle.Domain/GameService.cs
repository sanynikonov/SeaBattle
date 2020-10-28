using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameService : IGameBehaviour, IGameInfo
    {
        private readonly IFieldService firstField;
        private readonly IFieldService secondField;

        private IFieldService CurrentPlayerOppositeField => CurrentPlayer == FirstPlayer ? secondField : firstField;

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

        public virtual void MakeMove(Point coordinates)
        {
            AssertGameStarted();
            AssertGameIsNotEnded();

            CurrentPlayerOppositeField.OpenCell(coordinates);

            if (CheckGameEnd())
            {
                EndGame();
            }
            else
            {
                SwitchCurrentPlayer();
            }
        }

        public virtual void MakeMove(IShootStrategy strategy)
        {
            AssertGameStarted();
            AssertGameIsNotEnded();

            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            strategy.Shoot(CurrentPlayerOppositeField);
            
            if (CheckGameEnd())
            {
                EndGame();
            }
            else
            {
                SwitchCurrentPlayer();
            }
        }

        public virtual void StartGame()
        {
            AssertGameIsNotEnded();

            if (CurrentState == GameState.Started)
            {
                throw new Exception("Game is already started.");
            }

            CurrentState = GameState.Started;
        }

        protected virtual void EndGame()
        {
            Winner = CurrentPlayer;
            CurrentState = GameState.Ended;
        }

        protected bool CheckGameEnd()
        {
            return CurrentPlayerOppositeField.FieldCopy.Cells
                .Cast<Cell>()
                .Where(x => x.HasDeck)
                .All(x => x.IsOpened);
        }

        protected void SwitchCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == FirstPlayer ? SecondPlayer : FirstPlayer;
        }

        private void AssertGameIsNotEnded()
        {
            if (CurrentState == GameState.Ended)
            {
                throw new Exception("Game is already ended.");
            }
        }

        private void AssertGameStarted()
        {
            if (CurrentState != GameState.Started)
            {
                throw new Exception("Game is not started yet.");
            }
        }
    }
}
