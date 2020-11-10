using AutoFixture;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SeaBattle.Domain.Tests.Fields
{
    public class FieldServiceTest
    {
        private readonly Fixture fixture = new Fixture();
        private readonly FieldService fieldService = new FieldService();

        [Fact]
        public void GetFieldCopy_OfNull_ThrowsException()
        {
            Field field = null;

            void action() => fieldService.GetFieldCopy(field);

            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void GetFieldCopy_OfRandomField_ReturnsCopyOfCellsArray()
        {
            var expected = fixture.Create<Cell[,]>();
            var field = new Field { Cells = expected };

            var copy = field.Clone() as Field;
            var actual = copy.Cells;

            AssertCellsAreEqual(expected, actual);
        }

        [Fact]
        public void OpenCell_ForNullField_ThrowsException()
        {
            Field field = null;
            var point = new Point();

            void action() => fieldService.OpenCell(field, point);

            Assert.Throws<ArgumentNullException>(action);
        }

        [Theory]
        [InlineData(5, -1, 0)]
        [InlineData(5, 0, -1)]
        [InlineData(5, 5, 2)]
        [InlineData(5, 3, 10)]
        [InlineData(7, 9, 0)]
        [InlineData(7, 5, 7)]
        public void OpenCell_WithCoordinatesOutRange_ThrowsException(int dimension, int x, int y)
        {
            var field = new Field { Cells = new Cell[dimension, dimension] };
            var coordinates = new Point(x, y);

            void action() => fieldService.OpenCell(field, coordinates);

            Assert.Throws<Exception>(action);
        }

        [Theory]
        [InlineData(5, 0, 0)]
        [InlineData(5, 1, 0)]
        [InlineData(5, 0, 3)]
        [InlineData(5, 4, 4)]
        [InlineData(4, 1, 3)]
        [InlineData(8, 7, 5)]
        public void OpenCell_ThatIsOpenedAlready_ThrowsException(int dimension, int x, int y)
        {
            var cells = new Cell[dimension, dimension];
            cells[x, y] = new Cell { IsOpened = true };
            var field = new Field { Cells = cells };
            var coordinates = new Point(x, y);

            void action() => fieldService.OpenCell(field, coordinates);

            Assert.Throws<Exception>(action);
        }

        private void AssertCellsAreEqual(Cell[,] expected, Cell[,] actual)
        {
            for (int i = 0; i < expected.GetLength(0); i++)
            {
                for (int j = 0; j < expected.GetLength(0); j++)
                {
                    Assert.Equal(expected[i, j].HasDeck, actual[i, j].HasDeck);
                    Assert.Equal(expected[i, j].IsOpened, actual[i, j].IsOpened);
                }
            }
        }
    }
}
