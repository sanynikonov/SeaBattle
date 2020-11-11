using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Theory]
        [InlineData(5, 1, 1)]
        [InlineData(5, 0, 3)]
        [InlineData(5, 4, 4)]
        [InlineData(8, 7, 0)]
        [InlineData(8, 2, 4)]
        [InlineData(8, 0, 0)]
        public void OpenCell_ByCorrectCoordinates_SetsOpenedStatusForCell(int dimension, int x, int y)
        {
            var cells = new Cell[dimension, dimension];
            cells[x, y] = new Cell { IsOpened = false };
            var field = new Field { Cells = cells };
            var coordinates = new Point(x, y);

            fieldService.OpenCell(field, coordinates);
            var actual = field.Cells[x, y].IsOpened;

            Assert.True(actual);
        }

        [Theory]
        [InlineData(5, 1, 1, new[] { 1 }, new[] { 1 })]
        [InlineData(5, 0, 3, new[] { 0, 1 }, new[] { 3, 3 })]
        [InlineData(5, 4, 4, new[] { 2, 3, 4 }, new[] { 4, 4, 4 })]
        [InlineData(8, 7, 0, new[] { 7, 7 }, new[] { 0, 1 })]
        [InlineData(8, 2, 4, new[] { 2 }, new[] { 4 })]
        [InlineData(8, 0, 0, new[] { 0, 0, 0, 0 }, new[] { 0, 1, 2, 3 })]
        public void OpenCell_DetroyLastShipDeck_OpensCellsAroundDestroyedShip(int dimension, int x, int y, int[] shipXs, int[] shipYs)
        {
            var cells = InstantiateEmptyCellsArray(dimension);

            var shipCoords = shipXs.Select((x, i) => new Point(x, shipYs[i])).ToArray();

            for (int i = 0; i < shipCoords.Length; i++)
            {
                cells[shipCoords[i].X, shipCoords[i].Y] = new Cell { HasDeck = true, IsOpened = true };
            }

            cells[x, y].IsOpened = false;

            var field = new Field { Cells = cells };
            var coordinates = new Point(x, y);
            var neighboursCoords = shipCoords
                .Select(p => p.GetNeighbours())
                .SelectMany(x => x)
                .Distinct()
                .Where(p => p.IsInRange(0, dimension));

            fieldService.OpenCell(field, coordinates);
            var neigbours = neighboursCoords.Select(p => field.Cells[p.X, p.Y]);

            Assert.True(neigbours.All(n => n.IsOpened));
            //Assert.Collection(neigbours, c => Assert.True(c.IsOpened));
        }

        private Cell[,] InstantiateEmptyCellsArray(int dimension)
        {
            var cells = new Cell[dimension, dimension];

            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    cells[i, j] = new Cell();
                }
            }

            return cells;
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
