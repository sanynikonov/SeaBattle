using Moq;
using System;
using System.Collections.Generic;
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
    }
}
