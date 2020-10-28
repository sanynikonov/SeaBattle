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

            GameStarted?.Invoke(this);
        }

        public override void MakeMove(Point coordinates)
        {
            base.MakeMove(coordinates);

            MoveWasMade?.Invoke(this);
        }

        public override void MakeMove(IShootStrategy strategy)
        {
            base.MakeMove(strategy);

            MoveWasMade?.Invoke(this);
        }

        protected override void EndGame()
        {
            base.EndGame();

            GameEnded?.Invoke(this);
        }
    }
}
