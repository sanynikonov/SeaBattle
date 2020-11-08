using AutoFixture;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace SeaBattle.Domain.Tests.Fields
{
    public class FieldTest
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void Clone_Field_FieldAndCopyAreNotSame()
        {
            var cells = fixture.Create<Cell[,]>();
            var expected = new Field { Cells = cells };

            var actual = expected.Clone();

            Assert.NotSame(expected, actual);
        }

        [Fact]
        public void Clone_Cells_CellsArrayAndCopyAreNotSame()
        {
            var expected = fixture.Create<Cell[,]>();
            var field = new Field { Cells = expected };

            var copy = field.Clone() as Field;
            var actual = copy.Cells;

            Assert.NotSame(expected, actual);
        }

        [Fact]
        public void Clone_Cells_CellsArrayAndCopyAreEqual()
        {
            var expected = fixture.Create<Cell[,]>();
            var field = new Field { Cells = expected };

            var copy = field.Clone() as Field;
            var actual = copy.Cells;

            AssertCellsAreEqual(expected, actual);
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
