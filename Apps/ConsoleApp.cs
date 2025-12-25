using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;

namespace WindOS.Apps
{
    public class ConsoleApp : App
    {
        private string input = "";
        private string output = "";

        public ConsoleApp() : base("Console") { }

        public override void Update()
        {
             if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.Enter)
                {
                    output += "\n> " + input;
                    // Echo for now
                    output += "\nCommand not found: " + input;
                    input = "";
                }
                else if (key.Key == ConsoleKeyEx.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                }
                else if (key.KeyChar != 0)
                {
                    input += key.KeyChar;
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawFilledRectangle(Color.Black, 50, 50, 800, 500);

            string[] lines = output.Split('\n');
            int y = 60;
            // Draw last 20 lines
            int start = Math.Max(0, lines.Length - 20);
            for(int i=start; i<lines.Length; i++)
            {
                canvas.DrawString(lines[i], PCScreenFont.Default, Color.LightGreen, 60, y);
                y += 20;
            }

            canvas.DrawString("> " + input + "_", PCScreenFont.Default, Color.LightGreen, 60, 530);
        }
    }
}
