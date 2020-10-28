using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IFieldService
    {
        Field FieldCopy { get; }
        void OpenCell(Point coordinates);
        int GetShips
    }
}
