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

        public ConsoleApp() : base("Console")
        {
             output = "WindOS Console v1.0\nType 'help' for commands.\n";
        }

        public override void Update()
        {
             if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.Enter)
                {
                    output += "\n> " + input;
                    ProcessCommand(input);
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

        private void ProcessCommand(string cmd)
        {
            string cleanCmd = cmd.Trim().ToLower();
            if (string.IsNullOrEmpty(cleanCmd)) return;

            if (cleanCmd == "help")
            {
                output += "\nAvailable commands:\n help - Show this message\n cls - Clear screen\n echo [text] - Echo text\n reboot - Reboot system\n shutdown - Shutdown system";
            }
            else if (cleanCmd == "cls" || cleanCmd == "clear")
            {
                output = "";
            }
            else if (cleanCmd.StartsWith("echo "))
            {
                output += "\n" + cmd.Substring(5);
            }
            else if (cleanCmd == "reboot")
            {
                 Cosmos.System.Power.Reboot();
            }
             else if (cleanCmd == "shutdown")
            {
                 Cosmos.System.Power.Shutdown();
            }
            else
            {
                output += "\nCommand not found: " + cleanCmd;
            }
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawFilledRectangle(Color.Black, 50, 50, 800, 500);

            // Header bar
            canvas.DrawFilledRectangle(Color.DimGray, 50, 50, 800, 20);
            canvas.DrawString("Console", PCScreenFont.Default, Color.White, 55, 52);

            string[] lines = output.Split('\n');
            int y = 80;
            // Draw last 20 lines
            int maxLines = 20;
            int start = Math.Max(0, lines.Length - maxLines);
            for(int i=start; i<lines.Length; i++)
            {
                canvas.DrawString(lines[i], PCScreenFont.Default, Color.LightGreen, 60, y);
                y += 20;
            }

            canvas.DrawString("> " + input + "_", PCScreenFont.Default, Color.LightGreen, 60, 530);
        }
    }
}
