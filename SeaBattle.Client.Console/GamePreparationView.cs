using SeaBattle.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Client
{
    public class GamePreparationView
    {
        private readonly IFieldBuilder _fieldBuilder;

        public GamePreparationView(IFieldBuilder fieldBuilder)
        {
            _fieldBuilder = fieldBuilder;
        }

        public GameStartInfo Run()
        {
            Field firstField;
            Field secondField;

            Console.WriteLine("Hello! Welcome to the 'Sea Battle 1.0.0'");

            Console.WriteLine("Enter first player name:");
            var firstPlayerName = Console.ReadLine();

            Console.WriteLine("Enter second player name:");
            var secondPlayerName = Console.ReadLine();

            var fieldDirector = new FieldDirector();

            Console.WriteLine("Would you like to configure game settings? If not, you may play with classic game settings. (y/n)");
            var answer = Console.ReadLine();
            if (answer == "n")
            {
                fieldDirector.BuildClassicField(_fieldBuilder);
            }
            else
            {
                Console.WriteLine("Game settings.");
                CreateFieldDimension();
                CreateShipsStorage();
            }

            SetShipsForPlayer(firstPlayerName, fieldDirector);
            firstField = _fieldBuilder.Result;

            ResetBuilderForSecondField();

            SetShipsForPlayer(secondPlayerName, fieldDirector);
            secondField = _fieldBuilder.Result;

            return new GameStartInfo
            (
                new Player { Name = firstPlayerName },
                new Player { Name = secondPlayerName },
                firstField,
                secondField
            );
        }

        private void SetShipsForPlayer(string playerName, FieldDirector fieldDirector)
        {
            Console.WriteLine($"{playerName}, would you like to set ships manually? If not, you may start with random ships placement. (y/n)");
            var answer = Console.ReadLine();

            if (answer == "n")
            {
                fieldDirector.SetShipsRandomly(_fieldBuilder);
            }
            else
            {
                var shipsCount = _fieldBuilder.Result.ShipsCount.Select(x => x.Value).Count();
                SetShips(playerName, shipsCount);
            }
        }

        private void ResetBuilderForSecondField()
        {
            var dimension = _fieldBuilder.Result.Dimension;
            var shipsStorage = _fieldBuilder.Result.ShipsCount;

            _fieldBuilder.Reset();
            _fieldBuilder.SetDimension(dimension);
            _fieldBuilder.SetShipsStorage(new Dictionary<ShipType, int>(shipsStorage));
        }

        private void PrintInvitation()
        {

        }

        private void CreateFieldDimension()
        {
            Console.WriteLine("Choose field dimension:");

            int dimension;
            do
            {
                dimension = GetIntAnswer();
            }
            while (!TrySetDimension(dimension));
        }

        private bool TrySetDimension(int dimension)
        {
            try
            {
                _fieldBuilder.SetDimension(dimension);
            }
            catch
            {
                Console.WriteLine("Set another dimension.");
                return false;
            }

            return true;
        }

        private void CreateShipsStorage()
        {
            Console.WriteLine("Set ships storage");

            var shipTypes = Enum.GetValues(typeof(ShipType)).Cast<ShipType>();
            var shipsStorage = new Dictionary<ShipType, int>();

            do
            {
                int count;
                foreach (var shipType in shipTypes)
                {
                    Console.WriteLine($"Choose how many {shipType}s you want to have on the board:");
                    count = GetIntAnswer();
                    shipsStorage.Add(shipType, count);
                }
            }
            while (!TrySetShipsStorage(shipsStorage));
        }

        private bool TrySetShipsStorage(Dictionary<ShipType, int> shipsStorage)
        {
            try
            {
                _fieldBuilder.SetShipsStorage(shipsStorage);
            }
            catch
            {
                Console.WriteLine("Set another ship storage.");
                return false;
            }

            return true;
        }

        private void SetShips(string playerName, int shipsCount)
        {
            Console.WriteLine($"{playerName}, please set ships.");
            Point first;
            Point last;

            while (shipsCount != 0)
            {
                do
                {
                    Console.WriteLine("Set ship.");

                    (first, last) = GetShipPositionAnswer();
                }
                while (!TrySetShip(first, last));

                shipsCount--;
            }
        }

        private bool TrySetShip(Point first, Point last)
        {
            try
            {
                _fieldBuilder.SetShip(first, last);
            }
            catch
            {
                Console.WriteLine("Unable to set this ship. Set another.");
                return false;
            }

            return true;
        }

        private int GetIntAnswer()
        {
            string answer;
            int dimension;
            do
            {
                answer = Console.ReadLine();
            }
            while (!int.TryParse(answer, out dimension));

            return dimension;
        }

        private (Point first, Point last) GetShipPositionAnswer()
        {
            string[] numbers;

            do
            {
                Console.WriteLine("Write four numbers separated by space: " +
                    "two for coordinates of first ship point and two for last:");

                numbers = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            while (numbers.Length != 4 || numbers.Any(x => !int.TryParse(x, out _)));

            var result = numbers.Select(x => Convert.ToInt32(x)).ToArray();

            return (new Point(result[0], result[1]), new Point(result[2], result[3]));
        }
    }
}
