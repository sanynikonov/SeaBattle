using SeaBattle.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattle.Client
{
    public static class ConsoleHelper
    {
        public static int GetIntAnswer()
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

        public static (Point first, Point last) GetShipPositionAnswer()
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

        public static Point GetPointAnswer()
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
