using SeaBattle.Domain;
using System;
using System.Collections.Generic;

namespace SeaBattle.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var fieldBuilder = new FieldBuilder();

            var preparationView = new GamePreparationView(fieldBuilder);

            var gameInfo = preparationView.Run();

            var fieldService = new FieldService();

            var gameFactory = new ConsoleGameFactory();

            var game = gameFactory.CreateGame(gameInfo, fieldService);

            var gameView = new ConsoleGameView(game);

            gameView.Run();
        }
    }
}
