using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using System.Collections.Generic;

namespace WindOS.Apps
{
    public class SnakeApp : App
    {
        private List<Point> snake = new List<Point>();
        private Point food;
        private int score = 0;
        private int direction = 0; // 0: Right, 1: Down, 2: Left, 3: Up
        private bool gameOver = false;
        private Random random = new Random();

        private int speed = 5; // Frames to skip
        private int frameCount = 0;
        private int gridSize = 20;
        private int cols = 64; // 1280 / 20
        private int rows = 36; // 720 / 20 - header

        public SnakeApp() : base("Snake")
        {
            Reset();
        }

        private void Reset()
        {
            snake.Clear();
            snake.Add(new Point(10, 10));
            snake.Add(new Point(9, 10));
            snake.Add(new Point(8, 10));
            GenerateFood();
            score = 0;
            direction = 0;
            gameOver = false;
        }

        private void GenerateFood()
        {
            int x = random.Next(1, cols - 1);
            int y = random.Next(3, rows - 1); // Avoid header
            food = new Point(x, y);
        }

        public override void Update()
        {
            if (gameOver)
            {
                if (KeyboardManager.TryReadKey(out KeyEvent key))
                {
                    if (key.Key == ConsoleKeyEx.Spacebar || key.Key == ConsoleKeyEx.Enter)
                    {
                        Reset();
                    }
                }
                return;
            }

            // Input
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.UpArrow && direction != 1) direction = 3;
                else if (key.Key == ConsoleKeyEx.DownArrow && direction != 3) direction = 1;
                else if (key.Key == ConsoleKeyEx.LeftArrow && direction != 0) direction = 2;
                else if (key.Key == ConsoleKeyEx.RightArrow && direction != 2) direction = 0;
            }

            // Movement Logic
            frameCount++;
            if (frameCount >= speed)
            {
                frameCount = 0;
                Move();
            }
        }

        private void Move()
        {
            Point head = snake[0];
            Point newHead = head;

            switch (direction)
            {
                case 0: newHead.X++; break; // Right
                case 1: newHead.Y++; break; // Down
                case 2: newHead.X--; break; // Left
                case 3: newHead.Y--; break; // Up
            }

            // Collision with Walls
            if (newHead.X < 0 || newHead.X >= cols || newHead.Y < 2 || newHead.Y >= rows)
            {
                gameOver = true;
                return;
            }

            // Collision with Self
            foreach (var part in snake)
            {
                if (part.X == newHead.X && part.Y == newHead.Y)
                {
                    gameOver = true;
                    return;
                }
            }

            snake.Insert(0, newHead);

            // Eat Food
            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                score += 10;
                GenerateFood();
                // Don't remove tail -> grow
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawFilledRectangle(Color.Black, 0, 0, 1280, 720);

            // Header
            canvas.DrawFilledRectangle(Color.DarkGreen, 0, 0, 1280, 40);
            canvas.DrawString($"Snake Game - Score: {score}", PCScreenFont.Default, Color.White, 20, 10);

            if (gameOver)
            {
                canvas.DrawString("GAME OVER", PCScreenFont.Default, Color.Red, 600, 300);
                canvas.DrawString("Press Space to Restart", PCScreenFont.Default, Color.White, 550, 350);
                return;
            }

            // Draw Snake
            foreach (var part in snake)
            {
                canvas.DrawFilledRectangle(Color.Lime, part.X * gridSize, part.Y * gridSize, gridSize - 1, gridSize - 1);
            }

            // Draw Food
            canvas.DrawFilledRectangle(Color.Red, food.X * gridSize, food.Y * gridSize, gridSize - 1, gridSize - 1);
        }
    }
}
