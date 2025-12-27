using System;
using Cosmos.System.Graphics;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using System.IO;
using System.Collections.Generic;

namespace WindOS.Apps
{
    // Full-screen File Explorer for 640x480
    public class FileExplorerApp : App
    {
        private string currentPath = @"0:\";
        private List<string> entries = new List<string>();
        private int selectedIndex = 0;
        private int scrollOffset = 0;
        private const int MaxVisible = 20;

        public FileExplorerApp() : base("Explorer") { Refresh(); }

        private void Refresh()
        {
            entries.Clear();
            try
            {
                foreach (var d in Directory.GetDirectories(currentPath)) entries.Add("[D] " + Path.GetFileName(d));
                foreach (var f in Directory.GetFiles(currentPath)) entries.Add(Path.GetFileName(f));
            }
            catch { entries.Add("[Error reading directory]"); }
            selectedIndex = 0;
            scrollOffset = 0;
        }

        public override void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.DownArrow && selectedIndex < entries.Count - 1)
                {
                    selectedIndex++;
                    if (selectedIndex >= scrollOffset + MaxVisible) scrollOffset++;
                }
                else if (key.Key == ConsoleKeyEx.UpArrow && selectedIndex > 0)
                {
                    selectedIndex--;
                    if (selectedIndex < scrollOffset) scrollOffset--;
                }
                else if (key.Key == ConsoleKeyEx.Enter && entries.Count > 0)
                {
                    string sel = entries[selectedIndex];
                    if (sel.StartsWith("[D] "))
                    {
                        currentPath = Path.Combine(currentPath, sel.Substring(4));
                        Refresh();
                    }
                    else if (sel.EndsWith(".txt"))
                    {
                        Kernel.processManager.StartApp("Notepad", Path.Combine(currentPath, sel));
                    }
                }
                else if (key.Key == ConsoleKeyEx.Backspace)
                {
                    try
                    {
                        var parent = Directory.GetParent(currentPath);
                        if (parent != null) { currentPath = parent.FullName; Refresh(); }
                    }
                    catch { }
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            // Header
            canvas.DrawFilledRectangle(Kernel.DarkBluePen, 0, 0, Kernel.ScreenWidth, 25);
            canvas.DrawString("FILES: " + currentPath + " [BS=Up] [ESC=Exit]", PCScreenFont.Default, Kernel.WhitePen, 10, 6);

            // File list
            int y = 35;
            for (int i = scrollOffset; i < entries.Count && i < scrollOffset + MaxVisible; i++)
            {
                if (i == selectedIndex)
                    canvas.DrawFilledRectangle(Kernel.BluePen, 0, y, Kernel.ScreenWidth, 20);

                var e = entries[i];
                var pen = e.StartsWith("[D]") ? Kernel.YellowPen : Kernel.WhitePen;
                canvas.DrawString(e, PCScreenFont.Default, pen, 10, y + 3);
                y += 22;
            }

            // Status
            canvas.DrawString($"{entries.Count} items", PCScreenFont.Default, Kernel.GrayPen, 10, Kernel.ScreenHeight - 20);
        }
    }
}
