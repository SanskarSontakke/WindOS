using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using WindOS.System;

namespace WindOS.Apps
{
    public class CalculatorApp : App
    {
        private string display = "0";
        private string currentOp = "";
        private double firstOperand = 0;
        private bool newEntry = true;

        // Button layout
        private string[] buttons = { "7", "8", "9", "/", "4", "5", "6", "*", "1", "2", "3", "-", "C", "0", "=", "+" };
        private int btnSize = 60;
        private int startX = 400;
        private int startY = 200;

        public CalculatorApp() : base("Calculator") { }

        public override void Update()
        {
            // Simple click detection
            if (MouseManager.MouseState == MouseState.Left)
            {
                // Check clicks on buttons
                int x = 0, y = 0;
                for (int i = 0; i < buttons.Length; i++)
                {
                    int btnX = startX + (x * (btnSize + 10));
                    int btnY = startY + 100 + (y * (btnSize + 10));

                    if (MouseManager.X >= btnX && MouseManager.X <= btnX + btnSize &&
                        MouseManager.Y >= btnY && MouseManager.Y <= btnY + btnSize)
                    {
                        ProcessInput(buttons[i]);
                        // Simple debounce
                        while (MouseManager.MouseState == MouseState.Left) { }
                        return;
                    }

                    x++;
                    if (x > 3) { x = 0; y++; }
                }
            }
        }

        private void ProcessInput(string input)
        {
            double val;
            if (double.TryParse(input, out val)) // Number
            {
                if (newEntry || display == "0")
                {
                    display = input;
                    newEntry = false;
                }
                else
                {
                    display += input;
                }
            }
            else // Operator or Command
            {
                if (input == "C")
                {
                    display = "0";
                    firstOperand = 0;
                    currentOp = "";
                    newEntry = true;
                }
                else if (input == "=")
                {
                    if (!string.IsNullOrEmpty(currentOp))
                    {
                        double secondOperand = double.Parse(display);
                        double result = 0;
                        switch (currentOp)
                        {
                            case "+": result = firstOperand + secondOperand; break;
                            case "-": result = firstOperand - secondOperand; break;
                            case "*": result = firstOperand * secondOperand; break;
                            case "/": if (secondOperand != 0) result = firstOperand / secondOperand; break;
                        }
                        display = result.ToString();
                        currentOp = "";
                        newEntry = true;
                    }
                }
                else // Operator
                {
                    firstOperand = double.Parse(display);
                    currentOp = input;
                    newEntry = true;
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            // Draw Window Background
            canvas.DrawFilledRectangle(Color.LightGray, startX - 20, startY - 20, (btnSize + 10) * 4 + 30, (btnSize + 10) * 4 + 140);

            // Draw Display
            canvas.DrawFilledRectangle(Color.White, startX, startY, (btnSize + 10) * 4 - 10, 60);
            canvas.DrawString(display, PCScreenFont.Default, Color.Black, startX + 10, startY + 20);

            // Draw Buttons
            int x = 0, y = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                int btnX = startX + (x * (btnSize + 10));
                int btnY = startY + 100 + (y * (btnSize + 10));

                canvas.DrawFilledRectangle(Color.DarkGray, btnX, btnY, btnSize, btnSize);
                canvas.DrawString(buttons[i], PCScreenFont.Default, Color.White, btnX + 20, btnY + 20);

                x++;
                if (x > 3) { x = 0; y++; }
            }
        }
    }
}
