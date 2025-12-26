using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using System.IO;
using System.Collections.Generic;

namespace WindOS.Apps
{
    public class FileExplorerApp : App
    {
        private string currentPath = @"0:\";
        private List<string> entries = new List<string>();
        private int selectedIndex = 0;
        private bool isCreatingFolder = false;
        private string folderNameInput = "";

        public FileExplorerApp() : base("Explorer")
        {
            Refresh();
        }

        private void Refresh()
        {
            entries.Clear();
            try
            {
                var dirs = Directory.GetDirectories(currentPath);
                foreach (var d in dirs) entries.Add("[DIR] " + d);
                var files = Directory.GetFiles(currentPath);
                foreach (var f in files) entries.Add(f);
            }
            catch
            {
                entries.Add("Error reading " + currentPath);
            }
        }

        public override void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (isCreatingFolder)
                {
                    if (key.Key == ConsoleKeyEx.Enter)
                    {
                        if (!string.IsNullOrEmpty(folderNameInput))
                        {
                            try
                            {
                                Directory.CreateDirectory(Path.Combine(currentPath, folderNameInput));
                            }
                            catch { }
                            isCreatingFolder = false;
                            folderNameInput = "";
                            Refresh();
                        }
                    }
                    else if (key.Key == ConsoleKeyEx.Escape)
                    {
                        isCreatingFolder = false;
                        folderNameInput = "";
                    }
                    else if (key.KeyChar != 0)
                    {
                        folderNameInput += key.KeyChar;
                    }
                    return;
                }

                if (key.Key == ConsoleKeyEx.DownArrow)
                {
                    if (selectedIndex < entries.Count - 1) selectedIndex++;
                }
                else if (key.Key == ConsoleKeyEx.UpArrow)
                {
                    if (selectedIndex > 0) selectedIndex--;
                }
                else if (key.Key == ConsoleKeyEx.Enter)
                {
                    if (entries.Count > 0)
                    {
                        string selected = entries[selectedIndex];
                        if (selected.StartsWith("[DIR] "))
                        {
                            // Navigate
                            string dir = selected.Substring(6); // Remove prefix
                            currentPath = dir;
                            selectedIndex = 0;
                            Refresh();
                        }
                        else
                        {
                            // Open file Logic
                            if (selected.EndsWith(".txt") || selected.EndsWith(".wnd"))
                            {
                                WindOS.Kernel.processManager.StartApp("Notepad", selected);
                            }
                            // Custom file type example
                            else if (selected.EndsWith(".calc"))
                            {
                                WindOS.Kernel.processManager.StartApp("Calculator");
                            }
                        }
                    }
                }
                else if (key.Key == ConsoleKeyEx.Backspace)
                {
                    // Go up
                    try
                    {
                        var parent = Directory.GetParent(currentPath);
                        if (parent != null)
                        {
                            currentPath = parent.FullName;
                            selectedIndex = 0;
                            Refresh();
                        }
                    }
                    catch { }
                }
                else if (key.Key == ConsoleKeyEx.F2)
                {
                    // New Folder
                    isCreatingFolder = true;
                }
                else if (key.Key == ConsoleKeyEx.Delete)
                {
                    // Delete File/Folder
                    if (entries.Count > 0 && selectedIndex < entries.Count)
                    {
                         string selected = entries[selectedIndex];
                         try
                         {
                             if (selected.StartsWith("[DIR] "))
                             {
                                 string dir = selected.Substring(6);
                                 Directory.Delete(dir, true); // Recursive delete
                             }
                             else
                             {
                                 File.Delete(selected);
                             }
                             Refresh();
                         }
                         catch { }
                    }
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawFilledRectangle(Color.LightSlateGray, 200, 200, 500, 400);

            // Header
            canvas.DrawFilledRectangle(Color.DarkSlateGray, 200, 200, 500, 30);
            canvas.DrawString(currentPath, PCScreenFont.Default, Color.White, 210, 205);
            canvas.DrawString("F2: New Folder | BS: Up | Del: Delete", PCScreenFont.Default, Color.LightGray, 380, 205);

            if (isCreatingFolder)
            {
                 canvas.DrawFilledRectangle(Color.Black, 250, 350, 400, 50);
                 canvas.DrawString("New Folder: " + folderNameInput, PCScreenFont.Default, Color.White, 260, 365);
            }
            else
            {
                int y = 240;
                // Scroll logic could be added here
                for (int i = 0; i < entries.Count; i++)
                {
                    if (y > 580) break; // Clip

                    if (i == selectedIndex)
                        canvas.DrawFilledRectangle(Color.Blue, 210, y, 480, 20);

                    canvas.DrawString(entries[i], PCScreenFont.Default, Color.Black, 215, y + 2);
                    y += 25;
                }
            }
        }
    }
}
