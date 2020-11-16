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
                FirstFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(FirstPlayerFieldCopy),
                SecondFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(SecondPlayerFieldCopy)
            };

            GameStarted?.Invoke(this, new GameEventArgs(status));
        }

        public override BoardStatus MakeMove(Point coordinates)
        {
            var status = base.MakeMove(coordinates);

            if (CurrentState != GameState.Ended)
            {
                MoveWasMade?.Invoke(this, new GameEventArgs(status));
            }

            return status;
        }

        public override BoardStatus MakeMove(IFindCellStrategy strategy)
        {
            var status = base.MakeMove(strategy);

            if (CurrentState != GameState.Ended)
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
                FirstFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(FirstPlayerFieldCopy),
                SecondFieldWoundedShipsCoordinates = fieldService.GetDamagedShipsCheckedDecksCoordinates(SecondPlayerFieldCopy),
            };

            GameEnded?.Invoke(this, new GameEventArgs(status));
        }
    }
}
