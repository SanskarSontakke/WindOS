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

        public Menu(ProcessManager processManager)
        {
            pm = processManager;
            items.Add("Clock");
            items.Add("Calculator");
            items.Add("Notepad");
            items.Add("Explorer");
            items.Add("Console");
            items.Add("Shutdown");
            items.Add("Reboot");
        }

        public void HandleInput()
        {
            if (MouseManager.MouseState == MouseState.Left)
            {
                int y = 600 - (items.Count * 40);
                for (int i = 0; i < items.Count; i++)
                {
                    if (MouseManager.X >= 0 && MouseManager.X <= 200 &&
                        MouseManager.Y >= y && MouseManager.Y <= y + 40)
                    {
                        string item = items[i];
                        if (item == "Shutdown") Cosmos.System.Power.Shutdown();
                        else if (item == "Reboot") Cosmos.System.Power.Reboot();
                        else
                        {
                            pm.StartApp(item);
                        }
                        return; // Click handled
                    }
                    y += 40;
                }
            }
        }

        public void Draw(Cosmos.System.Graphics.Canvas canvas)
        {
            // Draw menu background
            int menuHeight = items.Count * 40;
            int startY = 680 - menuHeight; // Above taskbar

            canvas.DrawFilledRectangle(System.Drawing.Color.DarkGray, 0, startY, 200, menuHeight);

            for (int i = 0; i < items.Count; i++)
            {
                canvas.DrawString(items[i], Cosmos.System.Graphics.Fonts.PCScreenFont.Default, System.Drawing.Color.White, 10, startY + 10 + (i * 40));
                canvas.DrawLine(System.Drawing.Color.Gray, 0, startY + (i * 40), 200, startY + (i * 40));
            }
        }
    }
}
