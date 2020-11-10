using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IFindCellStrategy
    {
        Point FindCell(Field field);
    }
}
