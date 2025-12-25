using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.Graphics;
using System;
using System.Drawing;
using WindOS.Apps;
using WindOS.System;
using IL2CPU.API.Attribs;

namespace WindOS
{
    public class Kernel : Cosmos.System.Kernel
    {
        public CosmosVFS VFS;
        public Canvas canvas;
        public static ProcessManager processManager;
        public static Menu startMenu;
        public static LockScreen lockScreen;

        // Resources
        [ManifestResourceStream(ResourceName = "WindOS.cursor.bmp")] public static byte[] CursorStream;
        public static Bitmap cursorBitmap;

        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper01.bmp")] public static byte[] Wallpaper01Stream;
        public static Bitmap Wallpaper01;

        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper02.bmp")] public static byte[] Wallpaper02Stream;
        public static Bitmap Wallpaper02;

        private bool isMenuOpen = false;
        private int fps = 0;
        private int frames = 0;
        private DateTime lastTime;

        private Color[] solidColors = { Color.Teal, Color.Black, Color.DarkBlue, Color.Purple, Color.DarkRed, Color.DarkSlateGray };

        protected override void BeforeRun()
        {
            // Initialize File System
            VFS = new CosmosVFS();
            Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(VFS);

            // Load Config
            ConfigManager.Load();

            // Initialize Graphics
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1280, 720, ColorDepth.ColorDepth32));
            MouseManager.ScreenHeight = (uint)canvas.Mode.Height;
            MouseManager.ScreenWidth = (uint)canvas.Mode.Width;

            // Load Resources
            try
            {
                cursorBitmap = new Bitmap(CursorStream);
                Wallpaper01 = new Bitmap(Wallpaper01Stream);
                Wallpaper02 = new Bitmap(Wallpaper02Stream);
            }
            catch
            {
                // Fallback
            }

            // Initialize System
            processManager = new ProcessManager();
            processManager.RegisterApp(new ClockApp());
            processManager.RegisterApp(new CalculatorApp());
            processManager.RegisterApp(new NotepadApp());
            processManager.RegisterApp(new FileExplorerApp());
            processManager.RegisterApp(new ConsoleApp());
            processManager.RegisterApp(new SettingsApp());

            startMenu = new Menu(processManager);
            lockScreen = new LockScreen();

            lastTime = DateTime.Now;
        }

        protected override void Run()
        {
            try
            {
                // Update FPS
                frames++;
                if ((DateTime.Now - lastTime).TotalSeconds >= 1)
                {
                    fps = frames;
                    frames = 0;
                    lastTime = DateTime.Now;
                }

                // Lock Screen
                if (lockScreen.IsLocked)
                {
                    lockScreen.Update();
                    lockScreen.Draw(canvas);
                    DrawCursor((int)MouseManager.X, (int)MouseManager.Y);
                    canvas.Display();

                    if (frames % 60 == 0) Cosmos.Core.Memory.Heap.Collect();
                    return;
                }

                // Main Logic

                // Toggle Menu Input Logic
                if (MouseManager.MouseState == MouseState.Left)
                {
                    // Check Start Button
                    if (MouseManager.X < 50 && MouseManager.Y > 680)
                    {
                        if (!isMenuOpen)
                        {
                            isMenuOpen = true;
                            startMenu.Open();
                        }
                        else
                        {
                            isMenuOpen = false;
                            startMenu.Close();
                        }
                        // Debounce
                        while (MouseManager.MouseState == MouseState.Left);
                    }
                }

                // Wallpaper Toggle (Legacy/Shortcut)
                if (MouseManager.MouseState == MouseState.Left && MouseManager.X > 1230 && MouseManager.Y < 50)
                {
                    ConfigManager.WallpaperIndex++;
                    if (ConfigManager.WallpaperIndex > 5) ConfigManager.WallpaperIndex = 0;
                    while (MouseManager.MouseState == MouseState.Left);
                }

                // Update Menu (Animation)
                startMenu.Update();
                if (isMenuOpen && !startMenu.IsVisible()) isMenuOpen = false; // Closed by logic inside menu

                if (isMenuOpen)
                {
                    startMenu.HandleInput();
                }
                else
                {
                    processManager.Update();
                }

                // Draw
                DrawBackground();

                // Current App
                processManager.Draw(canvas);

                // Taskbar
                DrawTaskbar();

                // Menu (Draw over everything)
                if (startMenu.IsVisible())
                {
                    startMenu.Draw(canvas);
                }

                // FPS
                canvas.DrawString("FPS: " + fps, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.Yellow, 1200, 10);

                // Mouse
                DrawCursor((int)MouseManager.X, (int)MouseManager.Y);

                canvas.Display();

                // Memory Management optimization
                if (frames % 60 == 0)
                {
                    Cosmos.Core.Memory.Heap.Collect();
                }
            }
            catch (Exception e)
            {
                // BSOD
                canvas.DrawFilledRectangle(Color.Blue, 0, 0, 1280, 720);
                canvas.DrawString("BSOD: " + e.Message, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, 100, 100);
                canvas.Display();
                System.Threading.Thread.Sleep(5000);
            }
        }

        private void DrawBackground()
        {
             if (ConfigManager.WallpaperIndex == 0 && Wallpaper01 != null)
             {
                 canvas.DrawImage(Wallpaper01, 0, 0);
             }
             else if (ConfigManager.WallpaperIndex == 1 && Wallpaper02 != null)
             {
                 canvas.DrawImage(Wallpaper02, 0, 0);
             }
             else
             {
                 // Solid Colors based on index
                 int colorIndex = ConfigManager.WallpaperIndex - 2;
                 if (colorIndex < 0) colorIndex = 0;
                 if (colorIndex >= solidColors.Length) colorIndex = 0;

                 canvas.DrawFilledRectangle(solidColors[colorIndex], 0, 0, 1280, 720);
             }
        }

        private void DrawTaskbar()
        {
            canvas.DrawFilledRectangle(ConfigManager.TaskbarColor, 0, 680, 1280, 40);

            // Start Button
            canvas.DrawFilledRectangle(isMenuOpen ? ConfigManager.ThemeColor : Color.Blue, 0, 680, 50, 40);
            canvas.DrawString("Start", Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, 5, 690);

            // Running Apps indicators
            int x = 60;
            foreach(var app in processManager.Apps)
            {
                if (app == processManager.CurrentApp)
                {
                    canvas.DrawFilledRectangle(Color.FromArgb(100, 255, 255, 255), x, 685, 100, 30); // Semi-transparent highlight
                }

                // Small indicator if running (active)
                if (app.IsRunning)
                {
                     canvas.DrawLine(ConfigManager.ThemeColor, x, 715, x+100, 715);
                }

                canvas.DrawString(app.Name, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, x+5, 690);
                x += 110;
            }

            // Clock on taskbar
            string time = DateTime.Now.ToString("HH:mm");
            canvas.DrawString(time, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, 1200, 690);
        }

        private void DrawCursor(int x, int y)
        {
            if (cursorBitmap != null)
            {
                canvas.DrawImageAlpha(cursorBitmap, x, y);
            }
            else
            {
                // Fallback
                canvas.DrawLine(Color.White, x, y, x, y + 15);
                canvas.DrawLine(Color.White, x, y, x + 10, y + 10);
                canvas.DrawLine(Color.White, x, y + 15, x + 10, y + 10);
            }
        }
    }
}
