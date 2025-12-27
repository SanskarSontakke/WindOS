using System;
using Cosmos.System.Graphics;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;

namespace WindOS.System
{
    public class LockScreen
    {
        public bool IsLocked { get; private set; } = true;
        private string pin = "";

        public void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.Enter)
                {
                    if (pin == ConfigManager.PIN) { IsLocked = false; pin = ""; }
                    else { pin = ""; }
                }
                else if (key.Key == ConsoleKeyEx.Backspace && pin.Length > 0)
                    pin = pin.Substring(0, pin.Length - 1);
                else if (char.IsDigit(key.KeyChar))
                    pin += key.KeyChar;
            }
        }

        public void Draw(Canvas canvas)
        {
            canvas.DrawFilledRectangle(Kernel.BlackPen, 0, 0, Kernel.ScreenWidth, Kernel.ScreenHeight);
            
            int cx = Kernel.ScreenWidth / 2;
            int cy = Kernel.ScreenHeight / 2;
            
            canvas.DrawString(DateTime.Now.ToString("HH:mm"), PCScreenFont.Default, Kernel.WhitePen, cx - 25, cy - 60);
            canvas.DrawString("WindOS Locked", PCScreenFont.Default, Kernel.WhitePen, cx - 55, cy - 20);
            canvas.DrawString("Enter PIN:", PCScreenFont.Default, Kernel.GrayPen, cx - 45, cy + 10);
            canvas.DrawString(new string('*', pin.Length) + "_", PCScreenFont.Default, Kernel.WhitePen, cx - 30, cy + 35);
        }
    }
}
