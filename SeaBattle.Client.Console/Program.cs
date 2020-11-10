using SeaBattle.Domain;
using System.Collections.Generic;

namespace SeaBattle.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var fieldBuilder = new FieldBuilder();

            fieldBuilder.SetDimension(5);
            fieldBuilder.SetShipsStorage(new Dictionary<ShipType, int> { { ShipType.SingleDeck, 2 } });
            fieldBuilder.SetShip(new Point(0, 0), new Point(0, 0));
            fieldBuilder.SetShip(new Point(4, 4), new Point(4, 4));

            var firstField = fieldBuilder.Result;

            fieldBuilder.Reset();

            fieldBuilder.SetDimension(5);
            fieldBuilder.SetShipsStorage(new Dictionary<ShipType, int> { { ShipType.SingleDeck, 2 } });
            fieldBuilder.SetShip(new Point(0, 0), new Point(0, 0));
            fieldBuilder.SetShip(new Point(4, 4), new Point(4, 4));

            var secondField = fieldBuilder.Result;

            var gameInfo = new GameStartInfo(
                new Player { Name = "Kostia" }, 
                new Player { Name = "Voronin" }, 
                firstField, 
                secondField);

            //var preparationView = new GamePreparationView(fieldBuilder);

            //var gameInfo = preparationView.Run();

            var fieldService = new FieldService();

            var gameFactory = new ConsoleGameFactory();

            var game = gameFactory.CreateGame(gameInfo, fieldService);

            var gameView = new ConsoleGameView(game);

            gameView.Run();
        }
    }
}
