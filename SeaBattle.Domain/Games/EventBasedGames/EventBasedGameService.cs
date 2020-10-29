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

        public EventBasedGameService(GameStartInfo startInfo) : base(startInfo)
        {
        }

        public override void StartGame()
        {
            base.StartGame();

            var status = new BoardStatus
            {
                FirstFieldWoundedShipsCoordinates = firstFieldService.GetWreckedDecksOfDamagedShips(),
                SecondFieldWoundedShipsCoordinates = secondFieldService.GetWreckedDecksOfDamagedShips()
            };

            GameStarted?.Invoke(this, new GameEventArgs(status));
        }

        public override BoardStatus MakeMove(Point coordinates)
        {
            var status = base.MakeMove(coordinates);

            MoveWasMade?.Invoke(this, new GameEventArgs(status));

            return status;
        }

        public override BoardStatus MakeMove(IShootStrategy strategy)
        {
            var status = base.MakeMove(strategy);

            MoveWasMade?.Invoke(this, new GameEventArgs(status));

            return status;
        }

        protected override void EndGame()
        {
            base.EndGame();

            var status = new BoardStatus
            {
                FirstFieldWoundedShipsCoordinates = firstFieldService.GetWreckedDecksOfDamagedShips(),
                SecondFieldWoundedShipsCoordinates = secondFieldService.GetWreckedDecksOfDamagedShips()
            };

            GameEnded?.Invoke(this, new GameEventArgs(status));
        }
    }
}
