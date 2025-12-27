using System;
using System.IO;
using Cosmos.System.Graphics;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using WindOS.System;

namespace WindOS.Apps
{
    // Full-screen Notepad for 640x480
    public class NotepadApp : App, IArgConsumer
    {
        private string content = "";
        private string filename = "";
        private const int MaxCharsPerLine = 75;

        public NotepadApp() : base("Notepad") { }

        public void ConsumeArgs(object args)
        {
            if (args is string path)
            {
                filename = path;
                try { content = File.Exists(path) ? File.ReadAllText(path) : ""; }
                catch { content = "[Error reading file]"; }
            }
        }

        public override void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.F1) // Save
                {
                    if (!string.IsNullOrEmpty(filename))
                    {
                        try { File.WriteAllText(filename, content); }
                        catch { }
                    }
                }
                else if (key.Key == ConsoleKeyEx.Backspace && content.Length > 0)
                    content = content.Substring(0, content.Length - 1);
                else if (key.Key == ConsoleKeyEx.Enter)
                    content += "\n";
                else if (key.KeyChar != 0 && key.Key != ConsoleKeyEx.Escape)
                    content += key.KeyChar;
            }
        }

        public override void Draw(Canvas canvas)
        {
            // Header
            canvas.DrawFilledRectangle(Kernel.DarkBluePen, 0, 0, Kernel.ScreenWidth, 25);
            string title = string.IsNullOrEmpty(filename) ? "Untitled" : filename;
            canvas.DrawString("NOTEPAD - " + title + " [F1 Save] [ESC Exit]", PCScreenFont.Default, Kernel.WhitePen, 10, 6);

            // Content area
            canvas.DrawFilledRectangle(Kernel.LightGrayPen, 0, 25, Kernel.ScreenWidth, Kernel.ScreenHeight - 25);

            string[] lines = content.Split('\n');
            int y = 35;

            for (int i = 0; i < lines.Length && y < Kernel.ScreenHeight - 20; i++)
            {
                string line = lines[i];
                // Wrap long lines
                for (int j = 0; j < line.Length && y < Kernel.ScreenHeight - 20; j += MaxCharsPerLine)
                {
                    string chunk = line.Substring(j, Math.Min(MaxCharsPerLine, line.Length - j));
                    canvas.DrawString(chunk, PCScreenFont.Default, Kernel.BlackPen, 10, y);
                    y += 16;
                }
                if (line.Length == 0) y += 16;
            }

            // Cursor
            canvas.DrawString("_", PCScreenFont.Default, Kernel.BlackPen, 10 + ((content.Length % MaxCharsPerLine) * 8), y - 16);
        }
    }
}
