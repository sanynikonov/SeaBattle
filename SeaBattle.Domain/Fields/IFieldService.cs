using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IFieldService
    {
        Field GetFieldCopy(Field field);
        void OpenCell(Field field, Point coordinates);
        Point[] GetDamagedShipsCheckedDecksCoordinates(Field field);
    }
}
