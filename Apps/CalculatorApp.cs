using System;
using Cosmos.System.Graphics;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;

namespace WindOS.Apps
{
    // Full-screen Calculator for 640x480
    public class CalculatorApp : App
    {
        private string display = "0";
        private string currentOp = "";
        private double firstOperand = 0;
        private bool newEntry = true;

        private static readonly string[] buttons = { "7", "8", "9", "/", "4", "5", "6", "*", "1", "2", "3", "-", "C", "0", "=", "+" };
        private const int BtnSize = 60;
        private const int BtnGap = 10;

        public CalculatorApp() : base("Calculator") { }

        public override void Update()
        {
            if (MouseManager.MouseState == MouseState.Left)
            {
                int startX = (Kernel.ScreenWidth - (4 * BtnSize + 3 * BtnGap)) / 2;
                int startY = 150;

                for (int i = 0; i < 16; i++)
                {
                    int col = i % 4;
                    int row = i / 4;
                    int bx = startX + col * (BtnSize + BtnGap);
                    int by = startY + row * (BtnSize + BtnGap);

                    if (MouseManager.X >= bx && MouseManager.X < bx + BtnSize &&
                        MouseManager.Y >= by && MouseManager.Y < by + BtnSize)
                    {
                        ProcessInput(buttons[i]);
                        while (MouseManager.MouseState == MouseState.Left) { }
                        return;
                    }
                }
            }
        }

        private void ProcessInput(string input)
        {
            if (double.TryParse(input, out _))
            {
                if (newEntry || display == "0") { display = input; newEntry = false; }
                else if (display.Length < 12) display += input;
            }
            else if (input == "C")
            {
                display = "0"; firstOperand = 0; currentOp = ""; newEntry = true;
            }
            else if (input == "=")
            {
                if (!string.IsNullOrEmpty(currentOp) && double.TryParse(display, out double second))
                {
                    double result = currentOp switch
                    {
                        "+" => firstOperand + second,
                        "-" => firstOperand - second,
                        "*" => firstOperand * second,
                        "/" => second != 0 ? firstOperand / second : 0,
                        _ => second
                    };
                    display = result.ToString();
                    currentOp = "";
                    newEntry = true;
                }
            }
            else
            {
                if (double.TryParse(display, out firstOperand))
                {
                    currentOp = input;
                    newEntry = true;
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawString("CALCULATOR", PCScreenFont.Default, Kernel.CyanPen, Kernel.ScreenWidth / 2 - 45, 20);

            // Display
            int startX = (Kernel.ScreenWidth - (4 * BtnSize + 3 * BtnGap)) / 2;
            canvas.DrawFilledRectangle(Kernel.LightGrayPen, startX, 80, 4 * BtnSize + 3 * BtnGap, 50);
            canvas.DrawString(display, PCScreenFont.Default, Kernel.BlackPen, startX + 10, 100);

            // Buttons
            int startY = 150;
            for (int i = 0; i < 16; i++)
            {
                int col = i % 4;
                int row = i / 4;
                int bx = startX + col * (BtnSize + BtnGap);
                int by = startY + row * (BtnSize + BtnGap);

                canvas.DrawFilledRectangle(Kernel.GrayPen, bx, by, BtnSize, BtnSize);
                canvas.DrawString(buttons[i], PCScreenFont.Default, Kernel.WhitePen, bx + 25, by + 22);
            }

            canvas.DrawString("[ESC] Exit", PCScreenFont.Default, Kernel.GrayPen, 10, Kernel.ScreenHeight - 20);
        }
    }
}
