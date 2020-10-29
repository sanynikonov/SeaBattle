using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IGameBehaviour
    {
        void StartGame();
        BoardStatus MakeMove(Point coordinates);
        BoardStatus MakeMove(IShootStrategy strategy);
    }
}
