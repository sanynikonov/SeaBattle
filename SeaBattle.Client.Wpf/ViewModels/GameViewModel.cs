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

        public EventBasedGameService GameService { get; set; }
        public BoardStatus Status { get; set; }

        public void AutoShoot()
        {
            var strategyFactory = new BestSuitFindCellStrategyFactory();

            var wreckedDecks = GameService.CurrentPlayer == GameService.FirstPlayer
                ? Status?.SecondFieldWoundedShipsCoordinates
                : Status?.FirstFieldWoundedShipsCoordinates;

            Status = GameService.MakeMove(strategyFactory.GetFindCellStrategy(wreckedDecks));
        }

        public void InitGame()
        {
            var builder1 = new FieldBuilder();
            var builder2 = new FieldBuilder();

            var ships = new Dictionary<ShipType, int>
            {
                { ShipType.SingleDeck, SingleDeckShipsCount },
                { ShipType.DoubleDeck, DoubleDeckShipsCount },
                { ShipType.ThreeDeck, ThreeDeckShipsCount },
                { ShipType.FourDeck, FourDeckShipsCount }
            };

            builder1.SetDimension(FieldSize);
            builder1.SetShipsStorage(new Dictionary<ShipType, int>(ships));
            
            builder2.SetDimension(FieldSize);
            builder2.SetShipsStorage(new Dictionary<ShipType, int>(ships));

            var director = new FieldDirector();
            director.SetShipsRandomly(builder1);
            director.SetShipsRandomly(builder2);

            var info = new GameStartInfo(new Player { Name = "Cock" }, new Player { Name = "Faggot" }, builder1.Result, builder2.Result);

            GameService = new EventBasedGameService(info, new FieldService());
        }

        public void MakeMove(int cellX, int cellY)
        {
            if (GameService.CurrentPlayerOppositeField.Cells[cellX, cellY].IsOpened)
                return;

            Status = GameService.MakeMove(new Point(cellX, cellY));

            AutoShoot();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
