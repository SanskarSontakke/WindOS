using System;
using Cosmos.System.Graphics;
using System.Drawing;
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
                    // Use ConfigManager PIN
                    if (pin == ConfigManager.PIN)
                    {
                        IsLocked = false;
                        pin = ""; // Clear for next time
                    }
                    else
                    {
                        pin = "";
                    }
                }
                else if (key.Key == ConsoleKeyEx.Backspace && pin.Length > 0)
                {
                    pin = pin.Substring(0, pin.Length - 1);
                }
                else if (char.IsDigit(key.KeyChar))
                {
                    pin += key.KeyChar;
                }
            }
        }

        public void Draw(Canvas canvas)
        {
            canvas.DrawFilledRectangle(Color.Black, 0, 0, 1280, 720);

            // Time
            string time = DateTime.Now.ToString("HH:mm");
            canvas.DrawString(time, PCScreenFont.Default, Color.White, 600, 200);

            canvas.DrawString("WindOS Locked", PCScreenFont.Default, Color.White, 600, 300);
            canvas.DrawString("Enter PIN: " + new string('*', pin.Length), PCScreenFont.Default, Color.White, 580, 350);
        }
    }
}
