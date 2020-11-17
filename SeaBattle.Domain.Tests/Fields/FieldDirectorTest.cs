using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SeaBattle.Domain.Tests.Fields
{
    public class FieldDirectorTest
    {
        private readonly FieldDirector _fieldDirector = new FieldDirector();
        private readonly Mock<IFieldBuilder> _mockFieldBuilder = new Mock<IFieldBuilder>();

        [Fact]
        public void BuildClassicField_WithNullFieldBuilder_ThrowsException()
        {
            IFieldBuilder fieldBuilder = null;

            void action() => _fieldDirector.BuildClassicField(fieldBuilder);

            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void BuildClassicField_WithCorrectBuilder_SetsDimension()
        {
            var expected = 10;

            _fieldDirector.BuildClassicField(_mockFieldBuilder.Object);

            _mockFieldBuilder.Verify(builder => builder.SetDimension(expected), Times.Once);
        }

        [Fact]
        public void BuildClassicField_WithCorrectBuilder_SetsShipsStorage()
        {
            var expected =
                new Dictionary<ShipType, int>
                {
                    { ShipType.SingleDeck, 4 },
                    { ShipType.DoubleDeck, 3 },
                    { ShipType.ThreeDeck, 2 },
                    { ShipType.FourDeck, 1 }
                };

            _fieldDirector.BuildClassicField(_mockFieldBuilder.Object);

            _mockFieldBuilder.Verify(builder => builder.SetShipsStorage(expected), Times.Once);
        }

        [Fact]
        public void SetShipsRandomly_WithNullFieldBuilder_ThrowsException()
        {
            IFieldBuilder fieldBuilder = null;

            void action() => _fieldDirector.SetShipsRandomly(fieldBuilder);

            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void SetShipsRandomly_WithCorrectBuilder_SetsShipsFromPresetShipsStorage()
        {
            var fieldBuilder = new FieldBuilder();
            var shipsStorage = new Dictionary<ShipType, int> { { ShipType.SingleDeck, 1 }, { ShipType.DoubleDeck, 1 } };
            var expected = 3;

            fieldBuilder.SetDimension(5);
            fieldBuilder.SetShipsStorage(shipsStorage);

            _fieldDirector.SetShipsRandomly(fieldBuilder);
            var actual = fieldBuilder.Result.Cells.Cast<Cell>().Count(c => c.HasDeck);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetShipsRandomly_WithCorrectBuilder_SetShipsHaveFreeSpaceAround()
        {
            var dimension = 5;
            var fieldBuilder = new FieldBuilder();
            fieldBuilder.SetDimension(dimension);
            var shipsStorage = new Dictionary<ShipType, int> { { ShipType.SingleDeck, 3 } };
            fieldBuilder.SetShipsStorage(shipsStorage);

            _fieldDirector.SetShipsRandomly(fieldBuilder);

            var cells = fieldBuilder.Result.Cells.Cast<Cell>();

            var coordinates = cells.Where(c => c.HasDeck).Select(c => c.Coordinates);

            foreach (var cellCoordinates in coordinates)
            {
                var neighbourCellsHaveDecks = cellCoordinates
                    .GetNeighbours()
                    .Where(p => p.IsInRange(0, dimension))
                    .Select(p => fieldBuilder.Result.Cells[p.X, p.Y])
                    .All(c => c.HasDeck);

                Assert.False(neighbourCellsHaveDecks);
            }
        }
    }
}
