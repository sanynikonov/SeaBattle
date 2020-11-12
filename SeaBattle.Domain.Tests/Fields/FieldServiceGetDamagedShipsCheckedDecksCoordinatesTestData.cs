using System.Collections;
using System.Collections.Generic;

namespace SeaBattle.Domain.Tests.Fields
{
    public class FieldServiceGetDamagedShipsCheckedDecksCoordinatesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                5,
                new List<Point>(),
                new List<Point>(),
                new List<Point>()
            };

            yield return new object[]
            {
                5,
                new List<Point>
                {
                    new Point(2, 2),
                    new Point(2, 1),
                },
                new List<Point>
                {
                    new Point(2, 1),
                },
                new List<Point>
                {
                    new Point(2, 1),
                }
            };

            yield return new object[]
            {
                5,
                new List<Point>
                {
                    new Point(2, 2),
                    new Point(2, 1),
                },
                new List<Point>
                {
                    new Point(2, 2),
                    new Point(2, 1),
                },
                new List<Point>()
            };

            yield return new object[]
            {
                5,
                new List<Point>
                {
                    new Point(0, 0),
                    new Point(0, 1),

                    new Point(3, 3),
                    new Point(4, 3)
                },
                new List<Point>
                {
                    new Point(0, 0),

                    new Point(4, 3)
                },
                new List<Point>
                {
                    new Point(0, 0),

                    new Point(4, 3)
                }
            };

            yield return new object[]
            {
                5,
                new List<Point>
                {
                    new Point(0, 0),
                    new Point(0, 1),

                    new Point(3, 3),
                    new Point(4, 3)
                },
                new List<Point>
                {
                    new Point(0, 0),
                    new Point(0, 1),

                    new Point(4, 3)
                },
                new List<Point>
                {
                    new Point(4, 3)
                }
            };

            yield return new object[]
            {
                5,
                new List<Point>
                {
                    new Point(0, 0),
                    new Point(0, 1),
                    new Point(0, 2),

                    new Point(3, 3),
                    new Point(4, 3)
                },
                new List<Point>
                {
                    new Point(0, 0),
                    new Point(0, 1),

                    new Point(4, 3)
                },
                new List<Point>
                {
                    new Point(0, 0),
                    new Point(0, 1),

                    new Point(4, 3)
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
