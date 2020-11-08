using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IShootStrategy
    {
        void Shoot(Field field);
    }
}
