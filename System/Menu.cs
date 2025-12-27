using System.Drawing;
using WindOS.Apps;
using WindOS.System;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace WindOS
{
    public class Menu
    {
        private ProcessManager pm;
        private bool isOpen = false;

        // Simple app list
        private static readonly string[] items = { "Clock", "Calculator", "Notepad", "Paint", "Snake", "Console", "Files", "Monitor", "Settings", "Shutdown" };
        private const int ItemCount = 10;
        private const int ItemHeight = 20;
        private const int MenuWidth = 100;

        private static Pen menuBgPen = new Pen(Color.FromArgb(50, 50, 70));
        private static Pen hoverPen = new Pen(Color.FromArgb(80, 100, 140));

        public Menu(ProcessManager processManager) { pm = processManager; }

        public void Open() { isOpen = true; }
        public void Close() { isOpen = false; }
        public bool IsVisible() { return isOpen; }

        public void HandleInput()
        {
            if (MouseManager.MouseState != MouseState.Left || !isOpen) return;

            int menuTop = Kernel.ScreenHeight - 30 - (ItemCount * ItemHeight);
            int menuBottom = Kernel.ScreenHeight - 30;

            if (MouseManager.X < MenuWidth && MouseManager.Y >= menuTop && MouseManager.Y < menuBottom)
            {
                int idx = (int)((MouseManager.Y - menuTop) / ItemHeight);
                if (idx >= 0 && idx < ItemCount)
                {
                    LaunchApp(idx);
                    while (MouseManager.MouseState == MouseState.Left) { }
                }
            }
            else if (MouseManager.Y < menuBottom)
            {
                Close();
            }
        }

        private void LaunchApp(int idx)
        {
            string[] appNames = { "Clock", "Calculator", "Notepad", "Paint", "Snake", "Console", "Explorer", "Monitor", "Settings", "" };
            if (idx == 9) Cosmos.System.Power.Shutdown();
            else
            {
                pm.StartApp(appNames[idx]);
                Close();
            }
        }

        public void Draw(Canvas canvas)
        {
            if (!isOpen) return;

            int menuTop = Kernel.ScreenHeight - 30 - (ItemCount * ItemHeight);
            canvas.DrawFilledRectangle(menuBgPen, 0, menuTop, MenuWidth, ItemCount * ItemHeight);

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            for (int i = 0; i < ItemCount; i++)
            {
                int y = menuTop + (i * ItemHeight);
                if (mx < MenuWidth && my >= y && my < y + ItemHeight)
                {
                    canvas.DrawFilledRectangle(hoverPen, 0, y, MenuWidth, ItemHeight);
                }
                canvas.DrawString(items[i], PCScreenFont.Default, Kernel.WhitePen, 5, y + 4);
            }
        }
    }
}
