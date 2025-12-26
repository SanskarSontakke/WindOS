using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using System.IO;
using System.Collections.Generic;
using WindOS.System;

namespace WindOS.Apps
{
    public class NotepadApp : App, IArgConsumer
    {
        private string content = "";
        private string filename = "";
        private bool isTypingFilename = false;
        private const int MAX_CHARS_PER_LINE = 65;

        public NotepadApp() : base("Notepad") { }

        public void ConsumeArgs(object args)
        {
            if (args is string path)
            {
                filename = path;
                try
                {
                    if (File.Exists(path))
                    {
                        content = File.ReadAllText(path);
                    }
                    else
                    {
                        content = "";
                    }
                }
                catch
                {
                    content = "Error reading file.";
                }
            }
        }

        public override void OnStart()
        {
            if (string.IsNullOrEmpty(filename))
            {
                 content = "";
                 filename = "";
            }
        }

        public override void OnStop()
        {
            if (string.IsNullOrEmpty(filename)) content = "";
        }

        public override void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.F1) // Save
                {
                    if (string.IsNullOrEmpty(filename))
                    {
                        isTypingFilename = true;
                    }
                    else
                    {
                        SaveFile();
                    }
                }
                else if (isTypingFilename)
                {
                    if (key.Key == ConsoleKeyEx.Enter)
                    {
                        if (!string.IsNullOrEmpty(filename))
                        {
                            if (!filename.Contains(@":\")) filename = @"0:\" + filename;
                            SaveFile();
                            isTypingFilename = false;
                        }
                    }
                    else if (key.Key == ConsoleKeyEx.Escape)
                    {
                        isTypingFilename = false;
                    }
                    else if (key.Key == ConsoleKeyEx.Backspace && filename.Length > 0)
                    {
                        filename = filename.Substring(0, filename.Length - 1);
                    }
                    else if (key.KeyChar != 0)
                    {
                        filename += key.KeyChar;
                    }
                }
                else
                {
                    // Editing content
                    if (key.Key == ConsoleKeyEx.Backspace && content.Length > 0)
                    {
                        content = content.Substring(0, content.Length - 1);
                    }
                    else if (key.Key == ConsoleKeyEx.Enter)
                    {
                        content += "\n";
                    }
                    else if (key.KeyChar != 0)
                    {
                        content += key.KeyChar;
                    }
                }
            }
        }

        private void SaveFile()
        {
            try
            {
                File.WriteAllText(filename, content);
            }
            catch (Exception)
            {
                content += "\n[Error Saving]";
            }
        }

        public override void Draw(Canvas canvas)
        {
            // Background
            canvas.DrawFilledRectangle(Color.White, 100, 100, 600, 400);

            // Header
            canvas.DrawFilledRectangle(Color.DarkBlue, 100, 100, 600, 30);
            string title = string.IsNullOrEmpty(filename) ? "Untitled" : filename;
            canvas.DrawString($"Notepad - {title} (F1 Save)", PCScreenFont.Default, Color.White, 110, 105);

            if (isTypingFilename)
            {
                canvas.DrawFilledRectangle(Color.LightGray, 200, 200, 400, 50);
                canvas.DrawString("Save As: " + filename, PCScreenFont.Default, Color.Black, 210, 215);
            }
            else
            {
                // Content Rendering with Line Wrapping
                int y = 140;
                string[] lines = content.Split('\n');

                foreach (var line in lines)
                {
                    // Basic split by char count for visualization (primitive wrap)
                    for (int i = 0; i < line.Length; i += MAX_CHARS_PER_LINE)
                    {
                        if (y > 480) break; // Clip

                        string chunk = line.Substring(i, Math.Min(MAX_CHARS_PER_LINE, line.Length - i));
                        canvas.DrawString(chunk, PCScreenFont.Default, Color.Black, 110, y);
                        y += 20;
                    }
                    if (line.Length == 0) y += 20; // Empty line
                }
            }
        }
    }
}
