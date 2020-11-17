using SeaBattle.Domain;
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
                answer = ConsoleHelper.GetIntAnswer();

                switch (answer)
                {
                    case 0:
                        Console.WriteLine("Are you sure you want to live...");
                        return;
                    case 1:
                        lastBoardStatus = MakeMove();
                        break;
                    case 2:
                        lastBoardStatus = MakeMove(lastBoardStatus);
                        break;
                    default:
                        Console.WriteLine("Unknown answer.");
                        break;
                }
            }
        }

        private BoardStatus MakeMove(BoardStatus lastBoardStatus)
        {
            var wreckedDecks = game.CurrentPlayer == game.FirstPlayer
                ? lastBoardStatus?.SecondFieldWoundedShipsCoordinates
                : lastBoardStatus?.FirstFieldWoundedShipsCoordinates;

            var strategy = strategyFactory.GetFindCellStrategy(wreckedDecks);

            return game.MakeMove(strategy);
        }

        private BoardStatus MakeMove()
        {
            var point = ConsoleHelper.GetPointAnswer();
            return game.MakeMove(point);
        }
    }
}
