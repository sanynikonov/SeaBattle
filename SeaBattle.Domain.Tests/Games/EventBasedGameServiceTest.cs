using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SeaBattle.Domain.Tests.Games
{
    public class EventBasedGameServiceTest
    {
        private readonly Mock<IFieldService> _mockFieldService = new Mock<IFieldService>();

        private readonly GameStartInfo _info;
        private readonly EventBasedGameService _gameService;

        private readonly Point _deckCoordinates = new Point(0, 0);

        public EventBasedGameServiceTest()
        {
            var firstField = new Field { Cells = new Cell[5, 5] };

            for (int i = 0; i < firstField.Dimension; i++)
            {
                for (int j = 0; j < firstField.Dimension; j++)
                {
                    firstField.Cells[i, j] = new Cell { Coordinates = new Point(i, j) };
                }
            }

            firstField.Cells[_deckCoordinates.X, _deckCoordinates.Y].HasDeck = true;

            var secondField = firstField.Clone() as Field;

            _info = new GameStartInfo(
                new Player { Name = "Bruno" },
                new Player { Name = "Borat" },
                firstField,
                secondField);

            _gameService = new EventBasedGameService(_info, _mockFieldService.Object);

            _mockFieldService
                .Setup(service => service.OpenCell(It.IsAny<Field>(), It.IsAny<Point>()))
                .Callback<Field, Point>((field, coords) => field.Cells[coords.X, coords.Y].IsOpened = true);
        }

        [Fact]
        public void StartGame_WhenGameIsNotStarted_CallsEventThatGameIsStarted()
        {
            var actual = false;
            _gameService.GameStarted += (service, args) => actual = true;

            _gameService.StartGame();

            Assert.True(actual);
        }

        [Fact]
        public void StartGame_SetsBoardStatusForArgs_SearchesWreckedDecksUsingFieldService()
        {
            _gameService.StartGame();

            _mockFieldService.Verify(service => service.GetDamagedShipsCheckedDecksCoordinates(_info.FirstPlayerField));
            _mockFieldService.Verify(service => service.GetDamagedShipsCheckedDecksCoordinates(_info.SecondPlayerField));
        }

        [Fact]
        public void MakeMove_GameIsNotEnded_DoesNotCallEventThatGameIsEnded()
        {
            var actual = false;
            var coordinates = new Point(1, 1);
            _gameService.GameEnded += (service, args) => actual = true;
            _gameService.StartGame();

            _gameService.MakeMove(coordinates);

            Assert.False(actual);
        }

        [Fact]
        public void MakeMove_GameEnds_CallsEventThatGameIsEnded()
        {
            var actual = false;
            _gameService.GameEnded += (service, args) => actual = true;
            _gameService.StartGame();

            _gameService.MakeMove(_deckCoordinates);

            Assert.True(actual);
        }

        [Fact]
        public void MakeMove_GameEnds_DoesNotCallEventThatMoveWasMade()
        {
            var actual = false;
            _gameService.MoveWasMade += (service, args) => actual = true;
            _gameService.StartGame();

            _gameService.MakeMove(_deckCoordinates);

            Assert.False(actual);
        }

        [Fact]
        public void MakeMove_WhenGameIsStarted_CallsEventThatMoveWasMade()
        {
            var actual = false;
            var coordinates = new Point(1, 1);
            _gameService.MoveWasMade += (service, args) => actual = true;
            _gameService.StartGame();

            _gameService.MakeMove(coordinates);

            Assert.True(actual);
        }

        [Fact]
        public void MakeMove_GameEnds_SearchesWreckedDecksUsingFieldServiceBothForGameEndAndMakingMoveStatuses()
        {
            _gameService.StartGame();

            _gameService.MakeMove(_deckCoordinates);

            _mockFieldService.Verify(service => service.GetDamagedShipsCheckedDecksCoordinates(_info.FirstPlayerField));
            _mockFieldService.Verify(service => service.GetDamagedShipsCheckedDecksCoordinates(_info.SecondPlayerField));
        }
    }
}
