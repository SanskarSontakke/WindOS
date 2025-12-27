using System;
using Cosmos.System.Graphics;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using WindOS.System;

namespace WindOS.Apps
{
    // Full-screen Settings for 640x480
    public class SettingsApp : App
    {
        private int selectedOption = 0;
        private string pinInput = "";
        private bool editingPin = false;

        public SettingsApp() : base("Settings") { pinInput = ConfigManager.PIN; }

        public override void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (editingPin)
                {
                    if (key.Key == ConsoleKeyEx.Enter) { ConfigManager.PIN = pinInput; editingPin = false; ConfigManager.Save(); }
                    else if (key.Key == ConsoleKeyEx.Backspace && pinInput.Length > 0) pinInput = pinInput.Substring(0, pinInput.Length - 1);
                    else if (char.IsDigit(key.KeyChar)) pinInput += key.KeyChar;
                }
                else
                {
                    if (key.Key == ConsoleKeyEx.DownArrow && selectedOption < 3) selectedOption++;
                    else if (key.Key == ConsoleKeyEx.UpArrow && selectedOption > 0) selectedOption--;
                    else if (key.Key == ConsoleKeyEx.Enter)
                    {
                        if (selectedOption == 0) editingPin = true;
                        else if (selectedOption == 1) { ConfigManager.WallpaperIndex = (ConfigManager.WallpaperIndex + 1) % 4; }
                        else if (selectedOption == 2) ConfigManager.Save();
                        else if (selectedOption == 3) Cosmos.System.Power.Reboot();
                    }
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawString("SETTINGS [ESC Exit]", PCScreenFont.Default, Kernel.CyanPen, Kernel.ScreenWidth / 2 - 75, 20);

            string[] options = {
                "PIN: " + (editingPin ? pinInput + "_" : new string('*', pinInput.Length)),
                "Wallpaper Index: " + ConfigManager.WallpaperIndex,
                "Save Settings",
                "Reboot System"
            };

            int startY = 100;
            for (int i = 0; i < options.Length; i++)
            {
                int y = startY + i * 40;
                if (i == selectedOption)
                    canvas.DrawFilledRectangle(Kernel.BluePen, 100, y - 5, 440, 30);
                
                canvas.DrawString(options[i], PCScreenFont.Default, Kernel.WhitePen, 110, y);
            }

            canvas.DrawString("Use UP/DOWN to navigate, ENTER to select", PCScreenFont.Default, Kernel.GrayPen, 150, Kernel.ScreenHeight - 50);
        }
    }
}
