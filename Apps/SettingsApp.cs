using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using WindOS.System;

namespace WindOS.Apps
{
    public class SettingsApp : App
    {
        private int currentTab = 0; // 0: Personalization, 1: Security, 2: System
        private string[] tabs = { "Personalization", "Security", "System" };

        // Input buffers
        private string pinInput = "";
        private bool isTypingPin = false;

        public SettingsApp() : base("Settings")
        {
            pinInput = ConfigManager.PIN;
        }

        public override void Update()
        {
            // Tab switching
            if (MouseManager.MouseState == MouseState.Left)
            {
                if (MouseManager.Y >= 100 && MouseManager.Y <= 140)
                {
                    if (MouseManager.X >= 100 && MouseManager.X <= 300) currentTab = 0;
                    else if (MouseManager.X >= 300 && MouseManager.X <= 500) currentTab = 1;
                    else if (MouseManager.X >= 500 && MouseManager.X <= 700) currentTab = 2;
                }

                // Content Interaction
                if (currentTab == 0) // Personalization
                {
                    // Colors - Simple Palette
                    if (CheckClick(120, 200, 30, 30)) { ConfigManager.ThemeColor = Color.Teal; }
                    if (CheckClick(160, 200, 30, 30)) { ConfigManager.ThemeColor = Color.Crimson; }
                    if (CheckClick(200, 200, 30, 30)) { ConfigManager.ThemeColor = Color.DarkOrange; }

                    // Taskbar Colors
                    if (CheckClick(120, 300, 30, 30)) { ConfigManager.TaskbarColor = Color.Navy; }
                    if (CheckClick(160, 300, 30, 30)) { ConfigManager.TaskbarColor = Color.Black; }
                    if (CheckClick(200, 300, 30, 30)) { ConfigManager.TaskbarColor = Color.DarkSlateGray; }

                    // Wallpaper
                    if (CheckClick(120, 400, 100, 30))
                    {
                        ConfigManager.WallpaperIndex++;
                        if (ConfigManager.WallpaperIndex > 5) ConfigManager.WallpaperIndex = 0;
                        while(MouseManager.MouseState == MouseState.Left); // debounce
                    }
                }
                else if (currentTab == 1) // Security
                {
                    if (CheckClick(120, 200, 200, 30)) // PIN box
                    {
                        isTypingPin = true;
                        while(MouseManager.MouseState == MouseState.Left);
                    }
                    else if (MouseManager.Y > 140) // Click elsewhere
                    {
                        isTypingPin = false;
                        ConfigManager.PIN = pinInput;
                    }
                }

                // Save Button
                if (CheckClick(600, 500, 100, 40))
                {
                    ConfigManager.Save();
                }
            }

            // Typing
            if (isTypingPin && currentTab == 1)
            {
                 if (KeyboardManager.TryReadKey(out KeyEvent key))
                {
                    if (key.Key == ConsoleKeyEx.Backspace && pinInput.Length > 0)
                        pinInput = pinInput.Substring(0, pinInput.Length - 1);
                    else if (char.IsDigit(key.KeyChar))
                        pinInput += key.KeyChar;
                    else if (key.Key == ConsoleKeyEx.Enter)
                    {
                        isTypingPin = false;
                        ConfigManager.PIN = pinInput;
                    }
                }
            }
        }

        private bool CheckClick(int x, int y, int w, int h)
        {
            return MouseManager.X >= x && MouseManager.X <= x + w &&
                   MouseManager.Y >= y && MouseManager.Y <= y + h;
        }

        public override void Draw(Canvas canvas)
        {
            // Window
            canvas.DrawFilledRectangle(Color.WhiteSmoke, 100, 100, 700, 450);

            // Tabs
            for(int i=0; i<3; i++)
            {
                canvas.DrawFilledRectangle(currentTab == i ? Color.LightBlue : Color.Gray, 100 + (i*200), 100, 200, 40);
                canvas.DrawString(tabs[i], PCScreenFont.Default, Color.Black, 150 + (i*200), 110);
            }

            // Content
            int y = 160;
            if (currentTab == 0)
            {
                canvas.DrawString("Theme Color:", PCScreenFont.Default, Color.Black, 120, y);
                canvas.DrawFilledRectangle(Color.Teal, 120, y+40, 30, 30);
                canvas.DrawFilledRectangle(Color.Crimson, 160, y+40, 30, 30);
                canvas.DrawFilledRectangle(Color.DarkOrange, 200, y+40, 30, 30);

                y += 100;
                canvas.DrawString("Taskbar Color:", PCScreenFont.Default, Color.Black, 120, y);
                canvas.DrawFilledRectangle(Color.Navy, 120, y+40, 30, 30);
                canvas.DrawFilledRectangle(Color.Black, 160, y+40, 30, 30);
                canvas.DrawFilledRectangle(Color.DarkSlateGray, 200, y+40, 30, 30);

                y += 100;
                canvas.DrawFilledRectangle(Color.LightGray, 120, y, 100, 30);
                canvas.DrawString("Next Wallpaper", PCScreenFont.Default, Color.Black, 125, y+5);
                canvas.DrawString($"Current Index: {ConfigManager.WallpaperIndex}", PCScreenFont.Default, Color.Black, 240, y+5);
            }
            else if (currentTab == 1)
            {
                canvas.DrawString("Security PIN:", PCScreenFont.Default, Color.Black, 120, y);
                canvas.DrawFilledRectangle(isTypingPin ? Color.White : Color.LightGray, 120, y+40, 200, 30);
                canvas.DrawString(pinInput, PCScreenFont.Default, Color.Black, 130, y+45);

                canvas.DrawString("Screen Timeout: " + ConfigManager.ScreenTimeout + "s", PCScreenFont.Default, Color.Black, 120, y+100);
            }
            else if (currentTab == 2)
            {
                canvas.DrawString("Language: " + ConfigManager.Language, PCScreenFont.Default, Color.Black, 120, y);
                canvas.DrawString("System Version: 1.0.0", PCScreenFont.Default, Color.Black, 120, y+40);
            }

            // Save Button
            canvas.DrawFilledRectangle(Color.Green, 600, 500, 100, 40);
            canvas.DrawString("Save", PCScreenFont.Default, Color.White, 630, 510);
        }
    }
}
