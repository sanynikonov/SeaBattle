using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IGameBehaviour
    {
        void StartGame();
        void MakeMove(Point coordinates);
        void MakeMove(IShootStrategy strategy);
    }
}
