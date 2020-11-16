﻿using SeaBattle.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Client
{
    public class ConsoleGameView
    {
        private readonly GameService game;
        private readonly BestSuitFindCellStrategyFactory strategyFactory = new BestSuitFindCellStrategyFactory();


        private const string Menu = "Actions:\n" +
            "\n\t0 - exit" +
            "\n\t1 - make move" +
            "\n\t2 - make auto move" +
            "\n";

        public ConsoleGameView(GameService game)
        {
            this.game = game;
        }

        public void Run()
        {
            game.StartGame();

            int answer = -1;
            BoardStatus lastBoardStatus = null;

            while (game.CurrentState != GameState.Ended && answer != 0)
            {
                Console.WriteLine($"{game.CurrentPlayer.Name}, it's your turn now!");

                Console.WriteLine(Menu);
                Console.WriteLine("Your choice");
                answer = GetIntAnswer();

                switch (answer)
                {
                    case 0:
                        Console.WriteLine("Are you sure you want to live...");
                        return;
                    case 1:
                        var point = GetPointAnswer();
                        lastBoardStatus = game.MakeMove(point);
                        break;
                    case 2:
                        var wreckedDecks = game.CurrentPlayer == game.FirstPlayer
                            ? lastBoardStatus?.SecondFieldWoundedShipsCoordinates
                            : lastBoardStatus?.FirstFieldWoundedShipsCoordinates;
                        var strategy = strategyFactory.GetFindCellStrategy(wreckedDecks);
                        lastBoardStatus = game.MakeMove(strategy);
                        break;
                    default:
                        Console.WriteLine("Unknown answer.");
                        break;
                }
            }
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

        private Point GetPointAnswer()
        {
            string[] numbers;

            do
            {
                Console.WriteLine("Write two numbers separated by space: " +
                    "coordinates of the point to open:");

                numbers = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            while (numbers.Length != 2 || numbers.Any(x => !int.TryParse(x, out _)));

            var result = numbers.Select(x => Convert.ToInt32(x)).ToArray();

            return new Point(result[0], result[1]);
        }
    }
}
