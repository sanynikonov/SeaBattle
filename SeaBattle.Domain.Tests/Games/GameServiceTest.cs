using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SeaBattle.Domain.Tests.Games
{
    public class GameServiceTest
    {
        private readonly Mock<IFieldService> _mockFieldService = new Mock<IFieldService>();

        private readonly GameStartInfo _info;
        private readonly GameService _gameService;

        private readonly Point _deckCoordinates = new Point(0, 0);

        public GameServiceTest()
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

            _gameService = new GameService(_info, _mockFieldService.Object);
        }

        [Fact]
        public void StartGame_ThatIsStartedAlready_ThrowsException()
        {
            _gameService.StartGame();

            void action() => _gameService.StartGame();

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void StartGame_ThatIsEndedAlready_ThrowsException()
        {
            _gameService.StartGame();
            _gameService.MakeMove(_deckCoordinates);

            void action() => _gameService.StartGame();

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void StartGame_WhenGameIsNotStarted_ChangesStateOnStarted()
        {
            var expected = GameState.Started;

            _gameService.StartGame();
            var actual = _gameService.CurrentState;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StartGame_WhenGameIsNotStarted_SetsFirstPlayerAsCurrentPlayer()
        {
            var expected = _gameService.FirstPlayer;

            _gameService.StartGame();
            var actual = _gameService.CurrentPlayer;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void MakeMove_WhenGameIsNotStarted_ThrowsException()
        {
            var coordinates = new Point(1, 1);

            void action() => _gameService.MakeMove(coordinates);

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void MakeMove_WhenGameIsEnded_ThrowsException()
        {
            _mockFieldService
                .Setup(service => service.OpenCell(It.IsAny<Field>(), It.IsAny<Point>()))
                .Callback<Field, Point>((field, coords) => field.Cells[coords.X, coords.Y].IsOpened = true);

            var coordinates = new Point(1, 1);
            _gameService.StartGame();
            _gameService.MakeMove(_deckCoordinates);

            void action() => _gameService.MakeMove(coordinates);

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void MakeMove_WhenGameIsStarted_OpensCellOfSecondPlayerFieldUsingFieldService()
        {
            var coordinates = new Point(1, 1);
            _gameService.StartGame();

            _gameService.MakeMove(coordinates);

            _mockFieldService.Verify(service => service.OpenCell(_info.SecondPlayerField, coordinates));
        }

        [Fact]
        public void MakeMove_WhenGameIsStarted_SetsSecondPlayerAsCurrentPlayer()
        {
            var coordinates = new Point(1, 1);
            var expected = _gameService.SecondPlayer;
            _gameService.StartGame();

            _gameService.MakeMove(coordinates);
            var actual = _gameService.CurrentPlayer;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void MakeMove_SecondPlayerMakesMove_OpensCellOfFirstPlayerFieldUsingFieldService()
        {
            var coordinates = new Point(1, 1);
            _gameService.StartGame();
            _gameService.MakeMove(coordinates);

            _gameService.MakeMove(coordinates);

            _mockFieldService.Verify(service => service.OpenCell(_info.FirstPlayerField, coordinates));
        }

        [Fact]
        public void MakeMove_SecondPlayerMakesMove_SetsFirstPlayerAsCurrentPlayer()
        {
            var coordinates = new Point(1, 1);
            var expected = _gameService.FirstPlayer;
            _gameService.StartGame();
            _gameService.MakeMove(coordinates);

            _gameService.MakeMove(coordinates);
            var actual = _gameService.CurrentPlayer;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void MakeMove_DamagesShip_ReturnsCoordinatesOfDamagedDeckOfShip()
        {
            var expected = new Point(0, 1);
            _info.SecondPlayerField.Cells[expected.X, expected.Y].HasDeck = true;
            _gameService.StartGame();

            _gameService.MakeMove(expected);

            _mockFieldService.Verify(service => service.GetDamagedShipsCheckedDecksCoordinates(_info.FirstPlayerField));
            _mockFieldService.Verify(service => service.GetDamagedShipsCheckedDecksCoordinates(_info.SecondPlayerField));
        }

        [Fact]
        public void MakeMove_WithNullStrategy_ThrowsException()
        {
            IFindCellStrategy strategy = null;
            _gameService.StartGame();

            void action() => _gameService.MakeMove(strategy);

            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void MakeMove_SetStrategy_SearchesPointByStrategy()
        {
            var mockStrategy = new Mock<IFindCellStrategy>();
            _gameService.StartGame();

            _gameService.MakeMove(mockStrategy.Object);

            mockStrategy.Verify(strategy => strategy.FindCell(_info.SecondPlayerField));
        }
    }
}
