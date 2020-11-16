using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameService : IGameBehaviour, IGameInfo
    {
        protected readonly IFieldService fieldService;
        protected readonly Field firstPlayerField;
        protected readonly Field secondPlayerField;

        private Field CurrentPlayerOppositeField => CurrentPlayer == FirstPlayer ? secondPlayerField : firstPlayerField;

        public GameState CurrentState { get; private set; } = GameState.NotStarted;
        public Player Winner { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public Field FirstPlayerFieldCopy => fieldService.GetFieldCopy(firstPlayerField);
        public Field SecondPlayerFieldCopy => fieldService.GetFieldCopy(secondPlayerField);

        public Player FirstPlayer { get; private set; }
        public Player SecondPlayer { get; private set; }

        public GameService(GameStartInfo startInfo, IFieldService fieldService)
        {
            FirstPlayer = startInfo.FirstPlayer;
            SecondPlayer = startInfo.SecondPlayer;
            firstPlayerField = startInfo.FirstPlayerField;
            secondPlayerField = startInfo.SecondPlayerField;

            this.fieldService = fieldService;
        }

        public virtual BoardStatus MakeMove(Point coordinates)
        {
            AssertGameStarted();
            AssertGameIsNotEnded();

            fieldService.OpenCell(CurrentPlayerOppositeField, coordinates);

            if (CheckGameEnd())
            {
                EndGame();
            }
            else
            {
                SwitchCurrentPlayer();
            }

            return new BoardStatus
            {
                FirstFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(FirstPlayerFieldCopy),
                SecondFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(SecondPlayerFieldCopy)
            };
        }

        public virtual BoardStatus MakeMove(IFindCellStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            var coordinates = strategy.FindCell(CurrentPlayerOppositeField);

            return MakeMove(coordinates);
        }

        public virtual void StartGame()
        {
            AssertGameIsNotEnded();

            if (CurrentState == GameState.Started)
            {
                throw new Exception("Game is already started.");
            }

            CurrentState = GameState.Started;

            CurrentPlayer = FirstPlayer;
        }

        protected virtual void EndGame()
        {
            Winner = CurrentPlayer;
            CurrentState = GameState.Ended;
        }

        protected bool CheckGameEnd()
        {
            return CurrentPlayerOppositeField.Cells
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
