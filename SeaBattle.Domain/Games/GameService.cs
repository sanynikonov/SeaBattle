using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameService : IGameBehaviour, IGameInfo
    {
        protected readonly IFieldService firstFieldService;
        protected readonly IFieldService secondFieldService;

        private IFieldService CurrentPlayerOppositeField => CurrentPlayer == FirstPlayer ? secondFieldService : firstFieldService;

        public GameState CurrentState { get; private set; } = GameState.NotStarted;
        public Player Winner { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public Field FirstPlayerField => firstFieldService.FieldCopy;
        public Field SecondPlayerField => secondFieldService.FieldCopy;

        public Player FirstPlayer { get; private set; }
        public Player SecondPlayer { get; private set; }

        public GameService(GameStartInfo startInfo)
        {
            FirstPlayer = startInfo.FirstPlayer;
            SecondPlayer = startInfo.SecondPlayer;
            firstFieldService = startInfo.FirstPlayerFieldService;
            secondFieldService = startInfo.SecondPlayerFieldService;
        }

        public virtual BoardStatus MakeMove(Point coordinates)
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

            return new BoardStatus
            {
                FirstFieldWoundedShipsCoordinates = firstFieldService.GetWreckedDecksOfDamagedShips(),
                SecondFieldWoundedShipsCoordinates = secondFieldService.GetWreckedDecksOfDamagedShips()
            };
        }

        public virtual BoardStatus MakeMove(IShootStrategy strategy)
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

            return new BoardStatus
            {
                FirstFieldWoundedShipsCoordinates = firstFieldService.GetWreckedDecksOfDamagedShips(),
                SecondFieldWoundedShipsCoordinates = secondFieldService.GetWreckedDecksOfDamagedShips()
            };
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
