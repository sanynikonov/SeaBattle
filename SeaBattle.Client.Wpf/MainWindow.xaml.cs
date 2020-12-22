using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using SeaBattle.Domain;

namespace SeaBattle.Client.Wpf
{
    public partial class MainWindow : Window
    {
        private GameViewModel _gameViewModel;

        public MainWindow()
        {
            ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
            Show();
            DrawGrid(1, paintSurfacePlayer1);  // for beauty
            DrawGrid(1, paintSurfacePlayer2);

            _gameViewModel = new GameViewModel();
            DataContext = _gameViewModel;
        }
        private void AutoFireButton_Click(object sender, RoutedEventArgs e)
        {
            _gameViewModel.AutoShoot();
        }

        private void OnClickOnEnemyField(object sender, MouseButtonEventArgs e)
        {
            var point = GetCellCoordinates(e);

            _gameViewModel.MakeMove(point.X, point.Y);
        }

        private Domain.Point GetCellCoordinates(MouseButtonEventArgs e)
        {
            var absolute = paintSurfacePlayer2.TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));
            var p = e.GetPosition(this);
            double relativeX = p.X - absolute.X;
            double relativeY = p.Y - absolute.Y;

            int fieldSize = _gameViewModel.FieldSize;
            double width = paintSurfacePlayer2.ActualWidth;
            double height = paintSurfacePlayer2.ActualHeight;

            double stepX = width / fieldSize;
            double stepY = height / fieldSize;

            int cellX = Convert.ToInt32(Math.Floor(relativeX / stepX));
            int cellY = Convert.ToInt32(Math.Floor(relativeY / stepY));

            if (cellX >= fieldSize)
                cellX = fieldSize;

            if (cellY >= fieldSize)
                cellY = fieldSize;

            return new Domain.Point(cellX, cellY);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _gameViewModel.InitGame();
            }
            catch (FieldBuilderException)
            {
                MessageBox.Show("Unacceptable parameters for game start. Please provide another.");
                return;
            }

            _gameViewModel.GameService.GameStarted += (field, args) => DisplayField(field);
            _gameViewModel.GameService.MoveWasMade += (field, args) => DisplayField(field);

            _gameViewModel.GameService.StartGame();
        }

        private void DisplayField(GameService gameService)
        {
            Canvas targetSurface;
            if (gameService.CurrentPlayer == gameService.SecondPlayer)
                targetSurface = paintSurfacePlayer1;
            else if (gameService.CurrentPlayer == gameService.FirstPlayer)
                targetSurface = paintSurfacePlayer2;
            else
                throw new Exception();


            targetSurface.Children.Clear();
            DrawGrid(gameService.CurrentPlayerOppositeField.Dimension, targetSurface);
            DrawShips(gameService.CurrentPlayerOppositeField, targetSurface);
        }

        private void DrawShips(Field field, Canvas paintSurface)
        {
            Color blue = new Color() { R = 70, G = 70, B = 200, A = 255 };
            Color red = new Color() { R = 200, G = 70, B = 70, A = 255 };
            Color green = new Color() { R = 30, G = 150, B = 30, A = 255 };

            int width = Convert.ToInt32(paintSurface.ActualWidth);
            int height = Convert.ToInt32(paintSurface.ActualHeight);

            int xStep = width / field.Dimension;
            int yStep = height / field.Dimension;

            for (int x = 0; x < field.Dimension; x++)
                for (int y = 0; y < field.Dimension; y++)
                {
                    switch (field.Cells[x, y].CurrentState)
                    {
                        case CellState.Closed:
                            continue;
                        case CellState.OpenedEmpty:
                            DrawX(x, y, xStep, yStep, blue, paintSurface);
                            break;
                        case CellState.OpenedWithDeck:
                            DrawRect(x, y, xStep, yStep, red, paintSurface);
                            break;
                    }
                }
        }

        private void DrawX(int x, int y, int xStep, int yStep, Color color, Canvas paintSurface)
        {
            DrawLine(
                Convert.ToInt32((x + 0.25) * xStep), Convert.ToInt32((y + 0.25) * yStep),
                Convert.ToInt32((x + 0.75) * xStep), Convert.ToInt32((y + 0.75) * yStep),
                color, paintSurface);

            DrawLine(
                Convert.ToInt32((x + 0.75) * xStep), Convert.ToInt32((y + 0.25) * yStep),
                Convert.ToInt32((x + 0.25) * xStep), Convert.ToInt32((y + 0.75) * yStep),
                color, paintSurface);
        }

        private void DrawRect(int x, int y, int xStep, int yStep, Color color, Canvas paintSurface)
        {
            DrawLine(
                Convert.ToInt32((x + 0.85) * xStep), Convert.ToInt32((y + 0.15) * yStep),
                Convert.ToInt32((x + 0.15) * xStep), Convert.ToInt32((y + 0.15) * yStep),
                color, paintSurface);

            DrawLine(
                Convert.ToInt32((x + 0.15) * xStep), Convert.ToInt32((y + 0.85) * yStep),
                Convert.ToInt32((x + 0.15) * xStep), Convert.ToInt32((y + 0.15) * yStep),
                color, paintSurface);

            DrawLine(
                Convert.ToInt32((x + 0.85) * xStep), Convert.ToInt32((y + 0.15) * yStep),
                Convert.ToInt32((x + 0.85) * xStep), Convert.ToInt32((y + 0.85) * yStep),
                color, paintSurface);

            DrawLine(
                Convert.ToInt32((x + 0.15) * xStep), Convert.ToInt32((y + 0.85) * yStep),
                Convert.ToInt32((x + 0.85) * xStep), Convert.ToInt32((y + 0.85) * yStep),
                color, paintSurface);
        }

        private void DrawLine(int x1, int y1, int x2, int y2, Color color, Canvas paintSurface)
        {
            Line line = new Line() { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2 };
            line.Stroke = new SolidColorBrush(color);
            paintSurface.Children.Add(line);
        }

        private void DrawGrid(int size, Canvas paintSurface)
        {
            Color black = new Color() { R = 0, G = 0, B = 0, A = 255 };

            int width = Convert.ToInt32(paintSurface.ActualWidth);
            int height = Convert.ToInt32(paintSurface.ActualHeight);

            int xStep = width / size;
            int yStep = height / size;

            for (int i = 0; i <= size; i++)
            {
                DrawLine(0, i * yStep, size * xStep, i * yStep, black, paintSurface);
                DrawLine(i * xStep, 0, i * xStep, size * yStep, black, paintSurface);
            }
        }
    }
}
