using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public delegate void GameEventHandler(GameService sender, GameEventArgs args);
}
