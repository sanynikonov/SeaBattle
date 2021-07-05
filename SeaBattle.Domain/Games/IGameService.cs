using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IGameService
    {
        void StartGame();
        BoardStatus MakeMove(Point coordinates);
        BoardStatus MakeMove(IFindCellStrategy strategy);
    }
}
