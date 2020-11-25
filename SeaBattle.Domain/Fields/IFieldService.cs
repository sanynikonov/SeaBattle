using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle.Domain
{
    public interface IFieldService
    {
        //Field GetFieldCopy(Field field); - Этот метод нам  не нужен здесь. Перенесла его в класс Field
        void OpenCell(Field field, Point coordinates);
        Point[] GetDamagedShipsCheckedDecksCoordinates(Field field);
    }
}
