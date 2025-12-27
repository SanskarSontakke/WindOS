using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using System.Collections.Generic;
using Point = System.Drawing.Point;

namespace WindOS.Apps
{
    // Full-screen Snake for 640x480
    public class SnakeApp : App
    {
        private List<Point> snake = new List<Point>();
        private Point food;
        private int score = 0;
        private int direction = 0; // 0=Right, 1=Down, 2=Left, 3=Up
        private bool gameOver = false;
        private Random random = new Random();
        private int frameCount = 0;

        private const int GridSize = 16;
        private const int Cols = 40; // 640/16
        private const int Rows = 28; // 448/16 (leaving room for header)

        public SnakeApp() : base("Snake") { Reset(); }

        private void Reset()
        {
            snake.Clear();
            snake.Add(new Point(10, 10));
            snake.Add(new Point(9, 10));
            snake.Add(new Point(8, 10));
            food = new Point(random.Next(1, Cols - 1), random.Next(2, Rows - 1));
            score = 0;
            direction = 0;
            gameOver = false;
        }

        public override void Update()
        {
            if (gameOver)
            {
                if (KeyboardManager.TryReadKey(out KeyEvent k) && k.Key == ConsoleKeyEx.Spacebar) Reset();
                return;
            }

            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.UpArrow && direction != 1) direction = 3;
                else if (key.Key == ConsoleKeyEx.DownArrow && direction != 3) direction = 1;
                else if (key.Key == ConsoleKeyEx.LeftArrow && direction != 0) direction = 2;
                else if (key.Key == ConsoleKeyEx.RightArrow && direction != 2) direction = 0;
            }

            if (++frameCount >= 5)
            {
                frameCount = 0;
                Move();
            }
        }

        private void Move()
        {
            Point head = snake[0];
            Point newHead = new Point(
                head.X + (direction == 0 ? 1 : direction == 2 ? -1 : 0),
                head.Y + (direction == 1 ? 1 : direction == 3 ? -1 : 0)
            );

            if (newHead.X < 0 || newHead.X >= Cols || newHead.Y < 2 || newHead.Y >= Rows)
            {
                gameOver = true;
                return;
            }

            for (int i = 0; i < snake.Count; i++)
            {
                if (snake[i].X == newHead.X && snake[i].Y == newHead.Y)
                {
                    gameOver = true;
                    return;
                }
            }

            snake.Insert(0, newHead);

            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                score += 10;
                food = new Point(random.Next(1, Cols - 1), random.Next(2, Rows - 1));
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        public override void Draw(Canvas canvas)
        {
            // Header
            canvas.DrawFilledRectangle(Kernel.GreenPen, 0, 0, Kernel.ScreenWidth, 30);
            canvas.DrawString("SNAKE - Score: " + score + " [ESC Exit]", PCScreenFont.Default, Kernel.BlackPen, 10, 8);

            if (gameOver)
            {
                canvas.DrawString("GAME OVER!", PCScreenFont.Default, Kernel.RedPen, Kernel.ScreenWidth / 2 - 45, Kernel.ScreenHeight / 2);
                canvas.DrawString("Press SPACE to restart", PCScreenFont.Default, Kernel.WhitePen, Kernel.ScreenWidth / 2 - 90, Kernel.ScreenHeight / 2 + 30);
                return;
            }

            // Draw snake
            for (int i = 0; i < snake.Count; i++)
            {
                canvas.DrawFilledRectangle(Kernel.LimePen, snake[i].X * GridSize, 32 + snake[i].Y * GridSize, GridSize - 1, GridSize - 1);
            }

            // Draw food
            canvas.DrawFilledRectangle(Kernel.RedPen, food.X * GridSize, 32 + food.Y * GridSize, GridSize - 1, GridSize - 1);
        }
    }
}
