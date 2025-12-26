using System.Collections.Generic;
using WindOS.Apps;
using WindOS.System;
using Cosmos.System;

namespace WindOS
{
    public class Menu
    {
        private List<string> items = new List<string>();
        private ProcessManager pm;

        // Animation
        private int currentHeight = 0;
        private int targetHeight = 0;
        private bool isOpen = false;

        public Menu(ProcessManager processManager)
        {
            pm = processManager;
            items.Add("Settings");
            items.Add("Clock");
            items.Add("Calculator");
            items.Add("Notepad");
            items.Add("Paint");
            items.Add("Snake");
            items.Add("Monitor");
            items.Add("Explorer");
            items.Add("Console");
            items.Add("Shutdown");
            items.Add("Reboot");

            targetHeight = items.Count * 40;
        }

        public void Open()
        {
            isOpen = true;
            // Reset animation if needed, or let it slide up
        }

        public void Close()
        {
            isOpen = false;
        }

        public void HandleInput()
        {
            if (MouseManager.MouseState == MouseState.Left && isOpen)
            {
                int menuHeight = items.Count * 40;
                int startY = 680 - menuHeight; // Above taskbar

                // Check if click is inside menu
                if (MouseManager.X >= 0 && MouseManager.X <= 200 &&
                    MouseManager.Y >= startY && MouseManager.Y <= 680)
                {
                    int clickedIndex = (int)((MouseManager.Y - startY) / 40);
                    if (clickedIndex >= 0 && clickedIndex < items.Count)
                    {
                         string item = items[clickedIndex];
                        if (item == "Shutdown") Cosmos.System.Power.Shutdown();
                        else if (item == "Reboot") Cosmos.System.Power.Reboot();
                        else
                        {
                            pm.StartApp(item);
                            Close();
                        }

                        // Wait for release
                        while(MouseManager.MouseState == MouseState.Left);
                    }
                }
                else
                {
                    // Click outside, close
                    if (MouseManager.Y < 680) // Ignore taskbar clicks here, kernel handles toggle
                        Close();
                }
            }
        }

        public void Update()
        {
            // Animation Logic
            if (isOpen)
            {
                if (currentHeight < targetHeight) currentHeight += 20; // Speed
                if (currentHeight > targetHeight) currentHeight = targetHeight;
            }
            else
            {
                if (currentHeight > 0) currentHeight -= 20;
                if (currentHeight < 0) currentHeight = 0;
            }
        }

        public bool IsVisible()
        {
            return currentHeight > 0;
        }

        public void Draw(Cosmos.System.Graphics.Canvas canvas)
        {
            if (currentHeight <= 0) return;

            int startY = 680 - currentHeight;

            // Draw menu background
            canvas.DrawFilledRectangle(ConfigManager.TaskbarColor, 0, startY, 200, currentHeight);

            // Draw items (only if fully open or clip? Simple approach: draw all clipped)
            // Ideally we draw only what fits, but we just draw them relative to startY

            // We need to shift items so they appear to slide up.
            // Standard slide up: content moves with window.

            int y = startY;
            for (int i = 0; i < items.Count; i++)
            {
                // Simple highlight on hover
                if (MouseManager.X >= 0 && MouseManager.X <= 200 && MouseManager.Y >= y && MouseManager.Y <= y + 40)
                {
                    canvas.DrawFilledRectangle(ConfigManager.ThemeColor, 0, y, 200, 40);
                }

                canvas.DrawString(items[i], Cosmos.System.Graphics.Fonts.PCScreenFont.Default, System.Drawing.Color.White, 10, y + 10);
                canvas.DrawLine(System.Drawing.Color.Gray, 0, y + 40, 200, y + 40);
                y += 40;
            }
        }
    }
}
