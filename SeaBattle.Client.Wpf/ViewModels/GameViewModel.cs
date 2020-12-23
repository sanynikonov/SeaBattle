using SeaBattle.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SeaBattle.Client.Wpf
{
    public class GameViewModel : INotifyPropertyChanged
    {
        #region menu

        private int _fieldSize = 10;
        public int FieldSize
        {
            get { return _fieldSize; }
            set { _fieldSize = value; OnPropertyChanged(nameof(FieldSize)); }
        }

        private int _singleDeckShipsCount = 4;
        public int SingleDeckShipsCount
        {
            get { return _singleDeckShipsCount; }
            set { _singleDeckShipsCount = value; OnPropertyChanged(nameof(SingleDeckShipsCount)); }
        }

        private int _doubleDeckShipsCount = 3;
        public int DoubleDeckShipsCount
        {
            get { return _doubleDeckShipsCount; }
            set { _doubleDeckShipsCount = value; OnPropertyChanged(nameof(DoubleDeckShipsCount)); }
        }

        private int _threeDeckShipsCount = 2;
        public int ThreeDeckShipsCount
        {
            get { return _threeDeckShipsCount; }
            set { _threeDeckShipsCount = value; OnPropertyChanged(nameof(ThreeDeckShipsCount)); }
        }

        private int _fourDeckShipsCount = 1;
        public int FourDeckShipsCount
        {
            get { return _fourDeckShipsCount; }
            set { _fourDeckShipsCount = value; OnPropertyChanged(nameof(FourDeckShipsCount)); }
        }

        #endregion

        public event GameEventHandler GameStarted;
        public event GameEventHandler GameEnded;
        public event GameEventHandler MoveWasMade;

        private IGameService _gameService { get; set; }
        public BoardStatus Status { get; set; }

        public void AutoShoot()
        {
            ShootByStrategy();

            MakeBotMove();
        }

        private void ShootByStrategy()
        {
            var strategyFactory = new BestSuitFindCellStrategyFactory();

            var wreckedDecks = _gameService.CurrentPlayer == _gameService.FirstPlayer
                ? Status?.SecondFieldWoundedShipsCoordinates
                : Status?.FirstFieldWoundedShipsCoordinates;

            Status = _gameService.MakeMove(strategyFactory.GetFindCellStrategy(wreckedDecks));
        }

        public void InitGame()
        {
            var humanFieldBuilder = new FieldBuilder();
            var botFieldBuilder = new FieldBuilder();

            var ships = new Dictionary<ShipType, int>
            {
                { ShipType.SingleDeck, SingleDeckShipsCount },
                { ShipType.DoubleDeck, DoubleDeckShipsCount },
                { ShipType.ThreeDeck, ThreeDeckShipsCount },
                { ShipType.FourDeck, FourDeckShipsCount }
            };

            humanFieldBuilder.SetDimension(FieldSize);
            humanFieldBuilder.SetShipsStorage(new Dictionary<ShipType, int>(ships));
            
            botFieldBuilder.SetDimension(FieldSize);
            botFieldBuilder.SetShipsStorage(new Dictionary<ShipType, int>(ships));

            var director = new FieldDirector();
            director.SetShipsRandomly(humanFieldBuilder);
            director.SetShipsRandomly(botFieldBuilder);

            var info = new GameStartInfo(new Player { Name = "Human" }, new Player { Name = "Bot" }, humanFieldBuilder.Result, botFieldBuilder.Result);

            var gameService = new EventBasedGameService(info, new FieldService());

            gameService.GameStarted += GameStarted;
            gameService.GameEnded += GameEnded;
            gameService.MoveWasMade += MoveWasMade;

            _gameService = gameService;
        }

        public void StartGame()
        {
            _gameService.StartGame();
        }

        public void MakeMove(int cellX, int cellY)
        {
            var oppositeField = _gameService.CurrentPlayer == _gameService.FirstPlayer
                ? _gameService.SecondPlayerFieldCopy
                : _gameService.FirstPlayerFieldCopy;

            if (oppositeField.Cells[cellX, cellY].IsOpened || _gameService.CurrentState == GameState.Ended)
                return;

            Status = _gameService.MakeMove(new Point(cellX, cellY));

            MakeBotMove();
        }

        private void MakeBotMove()
        {
            if (_gameService.CurrentState != GameState.Ended)
                ShootByStrategy();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
