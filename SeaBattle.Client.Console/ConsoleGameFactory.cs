using SeaBattle.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Client
{
    public class ConsoleGameFactory
    {
        public GameService CreateGame(GameStartInfo startInfo)
        {
            var game = new EventBasedGameService(startInfo);

            game.GameStarted += PrintInvitation;
            game.GameStarted += PrintBoard;

            game.MoveWasMade += PrintBoard;

            game.GameEnded += PrintFinalSpeech;

            return game;
        }

        private static void PrintBoard(GameService sender, GameEventArgs args)
        {
            var sb = new StringBuilder();

            int dimension = sender.FirstPlayerField.Dimension;

            var domainRow = Enumerable.Range(0, dimension).Aggregate("", (tmp, x) => tmp + x);

            sb.Append("\t  " + domainRow + "\t\t  " + domainRow + "\n");

            for (int i = 0; i < dimension; i++)
            {
                sb.Append($"\t{i}|");

                sb.Append(DrawFieldRow(sender.FirstPlayerField, i));

                sb.Append($"\t\t{i}|");

                sb.Append(DrawFieldRow(sender.SecondPlayerField, i));

                sb.Append("\n");
            }

            Console.WriteLine(sb.ToString());
        }

        private static string DrawFieldRow(Field field, int row)
        {
            var builder = new StringBuilder();

            for (int column = 0; column < field.Dimension; column++)
            {
                switch (field.Cells[row, column].CurrentState)
                {
                    case CellState.Closed:
                        builder.Append(' ');
                        break;
                    case CellState.OpenedEmpty:
                        builder.Append('·');
                        break;
                    case CellState.OpenedWithDeck:
                        builder.Append('X');
                        break;
                    default:
                        throw new Exception("Unrecognized cell state.");
                }
            }

            return builder.ToString();
        }

        private static void PrintInvitation(GameService sender, GameEventArgs args)
        {
            Console.WriteLine("Squadrons of ships crossed in the middle of the sea." +
                "\nDestroy enemies and snatch victory in the battle!" +
                "\n");

            Console.WriteLine($"{sender.FirstPlayer.Name} or {sender.SecondPlayer.Name} - who will win???" +
                $"\n");
        }

        private static void PrintFinalSpeech(GameService sender, GameEventArgs args)
        {
            var loser = sender.Winner == sender.FirstPlayer ? sender.SecondPlayer : sender.FirstPlayer;

            Console.WriteLine("Game is ended." +
                $"\n{sender.Winner} destroyed all the ships of {loser.Name}'s squadron. Congratulations to the winner!" +
                "\n");

            Console.WriteLine($"I hope you enjoyed this game and join it soon. See you next time!" +
                $"\n");
        }
    }
}
