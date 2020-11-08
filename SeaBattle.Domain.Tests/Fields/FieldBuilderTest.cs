using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SeaBattle.Domain.Tests.Fields
{
    public class FieldBuilderTest
    {
        [Fact]
        public void Reset_AfterGetResult_PreviousAndNewResultsAreNotSame()
        {
            var builder = new FieldBuilder();
            var expected = builder.Result;

            builder.Reset();
            var actual = builder.Result;

            Assert.NotSame(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void SetDimension_LessOrEqualZero_ThrowsException(int dimension)
        {
            var builder = new FieldBuilder();

            void action() => builder.SetDimension(dimension);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(20)]
        public void SetDimension_HigherThanZero_CreatesCellsArrayWithSetDimensions(int dimension)
        {
            var builder = new FieldBuilder();
            var expected = dimension;

            builder.SetDimension(dimension);
            var actual = builder.Result.Cells;

            Assert.Equal(expected, actual.GetLength(0));
            Assert.Equal(expected, actual.GetLength(1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(20)]
        public void SetDimension_HigherThanZero_IntializesArrayWithDefaultCells(int dimension)
        {
            var builder = new FieldBuilder();

            builder.SetDimension(dimension);
            var actual = builder.Result.Cells;

            AssertCellsAreDefault(actual);
        }

        [Fact]
        public void SetShipsStorage_WithNull_ThrowsException()
        {
            var builder = new FieldBuilder();
            Dictionary<ShipType, int> storage = null;

            void action() => builder.SetShipsStorage(storage);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Fact]
        //TODO: Create ClassData test cases for different storages
        public void SetShipsStorage_WithTooManyShips_ThrowsException()
        {
            var builder = new FieldBuilder();
            builder.SetDimension(3);
            var storage = new Dictionary<ShipType, int> { { ShipType.FourDeck, 1 } };

            void action() => builder.SetShipsStorage(storage);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Fact]
        //TODO: Create ClassData test cases for different storages
        public void SetShipsStorage_WithCorrectStorage_SetsForResultShips()
        {
            var builder = new FieldBuilder();
            builder.SetDimension(6);
            var expected = new Dictionary<ShipType, int> { { ShipType.FourDeck, 1 }, { ShipType.SingleDeck, 2 } };

            builder.SetShipsStorage(expected);
            var actual = builder.Result.ShipsCount;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1, 0, 0, 0, 5)]
        [InlineData(0, -1, 0, 0, 5)]
        [InlineData(0, 0, -1, 0, 5)]
        [InlineData(0, 0, 0, -1, 5)]
        [InlineData(5, 0, 0, 0, 5)]
        [InlineData(0, 5, 0, 0, 5)]
        [InlineData(0, 0, 5, 0, 5)]
        [InlineData(0, 0, 0, 5, 5)]
        public void SetShip_WithCoordinatesOutOfRange_ThrowsException(int x1, int y1, int x2, int y2, int dimension)
        {
            var firstPoint = new Point(x1, y1);
            var lastPoint = new Point(x2, y2);
            var builder = new FieldBuilder();
            builder.SetDimension(dimension);

            void action() => builder.SetShip(firstPoint, lastPoint);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Theory]
        [InlineData(0, 1, 2, 3, 5)]
        //X
        // X
        [InlineData(2, 0, 0, 2, 5)]
        //   X
        //  X
        // X
        [InlineData(0, 0, 2, 5, 10)]
        //XX
        //  XX
        //    XX
        public void SetShip_Diagonally_ThrowsException(int x1, int y1, int x2, int y2, int dimension)
        {
            var firstPoint = new Point(x1, y1);
            var lastPoint = new Point(x2, y2);
            var builder = new FieldBuilder();
            builder.SetDimension(dimension);

            void action() => builder.SetShip(firstPoint, lastPoint);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Theory]
        [InlineData(0, 0, 0, 6, 10)]
        //XXXXXXX
        [InlineData(0, 0, 6, 0, 10)]
        //Same, but vertically
        [InlineData(1, 1, 1, 7, 10)]
        //
        // XXXXXXX
        [InlineData(1, 1, 7, 1, 10)]
        //Same, but vertically
        public void SetShip_OfIncorrectShipType_ThrowsException(int x1, int y1, int x2, int y2, int dimension)
        {
            var firstPoint = new Point(x1, y1);
            var lastPoint = new Point(x2, y2);
            var builder = new FieldBuilder();
            builder.SetDimension(dimension);

            void action() => builder.SetShip(firstPoint, lastPoint);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Theory]
        [InlineData(0, 1, 1, 1, 6)]
        // X
        // X
        [InlineData(0, 2, 0, 3, 6)]
        //  XX
        [InlineData(0, 1, 2, 1, 6)]
        // X
        // X
        // X
        [InlineData(0, 0, 0, 2, 6)]
        //XXX
        //TODO: Create ClassData test cases for different storages
        public void SetShip_ThatIsNotInStorage_ThrowsException(int x1, int y1, int x2, int y2, int dimension)
        {
            var firstPoint = new Point(x1, y1);
            var lastPoint = new Point(x2, y2);
            var storage = new Dictionary<ShipType, int> { { ShipType.FourDeck, 1 }, { ShipType.SingleDeck, 2 } };

            var builder = new FieldBuilder();
            builder.SetDimension(dimension);
            builder.SetShipsStorage(storage);

            void action() => builder.SetShip(firstPoint, lastPoint);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, ShipType.SingleDeck, 5)]
        //X
        [InlineData(0, 0, 0, 3, ShipType.FourDeck, 5)]
        //XXXX
        [InlineData(0, 0, 1, 0, ShipType.DoubleDeck, 5)]
        //X
        //X
        public void SetShip_ButAllShipsOfItsTypeAreSet_ThrowsException(int x1, int y1, int x2, int y2, ShipType shipType, int dimension)
        {
            var firstPoint = new Point(x1, y1);
            var lastPoint = new Point(x2, y2);
            var storage = new Dictionary<ShipType, int> { { shipType, 0 } };

            var builder = new FieldBuilder();
            builder.SetDimension(dimension);
            builder.SetShipsStorage(storage);

            void action() => builder.SetShip(firstPoint, lastPoint);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, ShipType.SingleDeck, 0, 0, 5)]
        [InlineData(0, 0, 0, 1, ShipType.DoubleDeck, 0, 0, 5)] //X0
        [InlineData(0, 0, 0, 1, ShipType.DoubleDeck, 0, 1, 5)] //0X
        [InlineData(1, 0, 0, 2, ShipType.ThreeDeck, 1, 0, 5)] //0XX
        [InlineData(1, 0, 0, 2, ShipType.ThreeDeck, 1, 1, 5)] //X0X
        [InlineData(1, 0, 0, 2, ShipType.ThreeDeck, 1, 2, 5)] //XX0
        [InlineData(0, 1, 3, 1, ShipType.FourDeck, 0, 1, 5)] //0XXX
        [InlineData(0, 1, 3, 1, ShipType.FourDeck, 1, 1, 5)] //X0XX
        [InlineData(0, 1, 3, 1, ShipType.FourDeck, 2, 1, 5)] //XX0X
        [InlineData(0, 1, 3, 1, ShipType.FourDeck, 3, 1, 5)] //XXX0
        public void SetShip_OnOcuppiedPlace_ThrowsException(int x1, int y1, int x2, int y2, ShipType shipType, int occupiedX, int occupiedY, int dimension)
        {
            var firstPoint = new Point(x1, y1);
            var lastPoint = new Point(x2, y2);
            var storage = new Dictionary<ShipType, int> { { shipType, 1 } };

            var builder = new FieldBuilder();
            builder.SetDimension(dimension);
            builder.SetShipsStorage(storage);

            builder.Result.Cells[occupiedX, occupiedY].HasDeck = true;

            void action() => builder.SetShip(firstPoint, lastPoint);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, ShipType.SingleDeck, 1, 0, 5)]
        //X-
        //0-
        [InlineData(0, 0, 0, 0, ShipType.SingleDeck, 1, 1, 5)]
        //X-
        //-0
        [InlineData(0, 0, 0, 0, ShipType.SingleDeck, 0, 1, 5)]
        //X0
        //--
        [InlineData(0, 0, 0, 1, ShipType.DoubleDeck, 0, 2, 5)]
        //XX0
        //---
        [InlineData(0, 0, 0, 1, ShipType.DoubleDeck, 1, 2, 5)]
        //XX-
        //--0
        [InlineData(0, 0, 0, 1, ShipType.DoubleDeck, 1, 1, 5)]
        //XX-
        //-0-
        [InlineData(1, 1, 0, 2, ShipType.ThreeDeck, 0, 1, 5)]
        //-0---
        //-XXX-
        //-----
        [InlineData(1, 1, 0, 2, ShipType.ThreeDeck, 1, 4, 5)]
        //-----
        //-XXX0
        //-----
        [InlineData(1, 1, 0, 2, ShipType.ThreeDeck, 2, 0, 5)]
        //-----
        //-XXX-
        //0----
        [InlineData(0, 1, 3, 1, ShipType.FourDeck, 2, 0, 5)]
        //-X-
        //-X-
        //0X-
        //-X-
        //---
        [InlineData(0, 1, 3, 1, ShipType.FourDeck, 4, 2, 5)]
        //-X-
        //-X-
        //-X-
        //-X-
        //--0
        [InlineData(0, 1, 3, 1, ShipType.FourDeck, 0, 2, 5)]
        //-X0
        //-X-
        //-X-
        //-X-
        //---
        public void SetShip_ButNeighbourCellHasDeck_ThrowsException(int x1, int y1, int x2, int y2, ShipType shipType, int occupiedX, int occupiedY, int dimension)
        {
            var firstPoint = new Point(x1, y1);
            var lastPoint = new Point(x2, y2);
            var storage = new Dictionary<ShipType, int> { { shipType, 1 } };

            var builder = new FieldBuilder();
            builder.SetDimension(dimension);
            builder.SetShipsStorage(storage);

            builder.Result.Cells[occupiedX, occupiedY].HasDeck = true;

            void action() => builder.SetShip(firstPoint, lastPoint);

            Assert.Throws<FieldBuilderException>(action);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, ShipType.SingleDeck, 5)]
        //X
        [InlineData(0, 0, 0, 3, ShipType.FourDeck, 5)]
        //XXXX
        [InlineData(0, 0, 1, 0, ShipType.DoubleDeck, 5)]
        //X
        //X
        public void SetShip_Correctly_MarksCellsTheyHaveDeck(int x1, int y1, int x2, int y2, ShipType shipType, int dimension)
        {
            var firstPoint = new Point(x1, y1);
            var lastPoint = new Point(x2, y2);
            var storage = new Dictionary<ShipType, int> { { shipType, 1 } };

            var builder = new FieldBuilder();
            builder.SetDimension(dimension);
            builder.SetShipsStorage(storage);

            builder.SetShip(firstPoint, lastPoint);

            //TODO: create extension method for point to get points between two passed
            var points = x2 - x1 != 0
                ? Enumerable.Range(x1, x2).Select(i => new Point(i, y1))
                : Enumerable.Range(y1, y2).Select(i => new Point(x1, i));

            var cells = builder.Result.Cells;
            foreach (var point in points)
            {
                Assert.True(cells[point.X, point.Y].HasDeck);
            }
        }

        private static void AssertCellsAreDefault(Cell[,] actual)
        {
            foreach (var cell in actual)
            {
                Assert.False(cell.HasDeck);
                Assert.False(cell.IsOpened);
            }
        }
    }
}
