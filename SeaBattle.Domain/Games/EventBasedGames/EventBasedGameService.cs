using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class EventBasedGameService : GameService
    {
        public event GameEventHandler GameStarted;
        public event GameEventHandler GameEnded;
        public event GameEventHandler MoveWasMade;

        public EventBasedGameService(GameStartInfo startInfo, IFieldService fieldService) : base(startInfo, fieldService)
        {
        }

        public override void StartGame()
        {
            base.StartGame();

            var status = new BoardStatus
            {
                FirstFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(GameStateHolder.Instance.FirstPlayer.Field),
                SecondFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(GameStateHolder.Instance.SecondPlayer.Field),
            };

            GameStarted?.Invoke(this, new GameEventArgs(status));
        }

        public override BoardStatus MakeMove(Point coordinates)
        {
            var status = base.MakeMove(coordinates);

            if (GameStateHolder.Instance.CurrentState != GameState.Ended)
            {
                MoveWasMade?.Invoke(this, new GameEventArgs(status));
            }

            return status;
        }

        protected override void EndGame()
        {
            base.EndGame();

            var status = new BoardStatus
            {
                FirstFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(GameStateHolder.Instance.FirstPlayer.Field),
                SecondFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(GameStateHolder.Instance.SecondPlayer.Field),
            };

            GameEnded?.Invoke(this, new GameEventArgs(status));
        }
    }
}
