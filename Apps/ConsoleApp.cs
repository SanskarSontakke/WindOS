using System;
using Cosmos.System.Graphics;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;

namespace WindOS.Apps
{
    // Full-screen Console for 640x480
    public class ConsoleApp : App
    {
        private string input = "";
        private string output = "WindOS Console v1.0\nType 'help' for commands.\n";

        public ConsoleApp() : base("Console") { }

        public override void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.Enter)
                {
                    output += "> " + input + "\n";
                    ProcessCommand(input);
                    input = "";
                }
                else if (key.Key == ConsoleKeyEx.Backspace && input.Length > 0)
                    input = input.Substring(0, input.Length - 1);
                else if (key.KeyChar != 0 && key.Key != ConsoleKeyEx.Escape)
                    input += key.KeyChar;
            }
        }

        private void ProcessCommand(string cmd)
        {
            string c = cmd.Trim().ToLower();
            if (c == "help") output += "Commands: help, cls, time, reboot, shutdown\n";
            else if (c == "cls") output = "";
            else if (c == "time") output += DateTime.Now.ToString() + "\n";
            else if (c == "reboot") Cosmos.System.Power.Reboot();
            else if (c == "shutdown") Cosmos.System.Power.Shutdown();
            else if (c != "") output += "Unknown: " + c + "\n";
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawString("CONSOLE [ESC to Exit]", PCScreenFont.Default, Kernel.CyanPen, 10, 10);

            // Output area
            string[] lines = output.Split('\n');
            int maxLines = 25;
            int start = Math.Max(0, lines.Length - maxLines);
            int y = 40;

            for (int i = start; i < lines.Length && y < Kernel.ScreenHeight - 40; i++)
            {
                canvas.DrawString(lines[i], PCScreenFont.Default, Kernel.GreenPen, 10, y);
                y += 16;
            }

            // Input line
            canvas.DrawString("> " + input + "_", PCScreenFont.Default, Kernel.LimePen, 10, Kernel.ScreenHeight - 30);
        }
    }
}
