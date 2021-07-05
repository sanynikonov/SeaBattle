using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameService : IGameService
    {
        protected readonly IFieldService fieldService;
      
        public Field FirstPlayerFieldCopy => GameStateHolder.Instance.FirstPlayer.Field.CloneField(); // fieldService.GetFieldCopy(firstPlayerField);
        public Field SecondPlayerFieldCopy => GameStateHolder.Instance.SecondPlayer.Field.CloneField(); // fieldService.GetFieldCopy(secondPlayerField);

        public GameService(GameStartInfo startInfo, IFieldService fieldService)
        {
            GameStateHolder.Instance.FirstPlayer = startInfo.FirstPlayer;
            GameStateHolder.Instance.SecondPlayer = startInfo.SecondPlayer;

            this.fieldService = fieldService;
        }

        public virtual BoardStatus MakeMove(Point coordinates)
        {
            GameStateHolder.Instance.AssertGameStarted();
            GameStateHolder.Instance.AssertGameIsNotEnded();

            fieldService.OpenCell(GameStateHolder.Instance.CurrentPlayerOppositeField, coordinates);

            if (CheckGameEnd())
            {
                EndGame();
            }
            else
            {
                GameStateHolder.Instance.SwitchCurrentPlayer();
            }

            return new BoardStatus
            {
                FirstFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(GameStateHolder.Instance.FirstPlayer.Field),
                SecondFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(GameStateHolder.Instance.SecondPlayer.Field)
            };
        }

        public virtual BoardStatus MakeMove(IFindCellStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            var coordinates = strategy.FindCell(GameStateHolder.Instance.CurrentPlayerOppositeField);

            return MakeMove(coordinates);
        }

        public virtual void StartGame()
        {
            GameStateHolder.Instance.AssertGameIsNotEnded();

            if (GameStateHolder.Instance.CurrentState == GameState.Started)
            {
                throw new GameServiceException("Game is already started.");
            }

            GameStateHolder.Instance.CurrentState = GameState.Started;

            GameStateHolder.Instance.CurrentPlayer = GameStateHolder.Instance.FirstPlayer;
        }

        protected virtual void EndGame()
        {
            GameStateHolder.Instance.Winner = GameStateHolder.Instance.CurrentPlayer;
            GameStateHolder.Instance.CurrentState = GameState.Ended;
        }

        protected bool CheckGameEnd()
        {
            return GameStateHolder.Instance.CurrentPlayerOppositeField.Cells
                .Cast<Cell>()
                .Where(x => x.HasDeck)
                .All(x => x.IsOpened);
        }
    }
}
