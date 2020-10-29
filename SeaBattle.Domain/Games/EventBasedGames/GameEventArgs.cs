using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public class GameEventArgs
    {
        public BoardStatus BoardStatus { get; }

        public GameEventArgs(BoardStatus boardStatus)
        {
            BoardStatus = boardStatus;
        }
    }
}
