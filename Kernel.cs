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

        [ManifestResourceStream(ResourceName = "WindOS.cursor.bmp")] public static byte[] CursorStream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper01.bmp")] public static byte[] Wallpaper01Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper02.bmp")] public static byte[] Wallpaper02Stream;

        // 640x480 resolution
        public const int ScreenWidth = 640;
        public const int ScreenHeight = 480;

        // State
        private bool showMenu = false;
        private int fps = 0;
        private int frames = 0;
        private DateTime lastTime;
        private string fpsText = "0";

        // Cached Pens - shared by all apps
        public static Pen WhitePen;
        public static Pen BlackPen;
        public static Pen YellowPen;
        public static Pen BluePen;
        public static Pen RedPen;
        public static Pen GreenPen;
        public static Pen GrayPen;
        public static Pen LimePen;
        public static Pen DarkBluePen;
        public static Pen LightGrayPen;
        public static Pen CyanPen;

        protected override void BeforeRun()
        {
            // Initialize Pens once
            WhitePen = new Pen(Color.White);
            BlackPen = new Pen(Color.Black);
            YellowPen = new Pen(Color.Yellow);
            BluePen = new Pen(Color.Blue);
            RedPen = new Pen(Color.Red);
            GreenPen = new Pen(Color.Green);
            GrayPen = new Pen(Color.Gray);
            LimePen = new Pen(Color.Lime);
            DarkBluePen = new Pen(Color.DarkBlue);
            LightGrayPen = new Pen(Color.LightGray);
            CyanPen = new Pen(Color.Cyan);

            // File System
            VFS = new CosmosVFS();
            Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(VFS);
            ConfigManager.Load();

            // Graphics
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(ScreenWidth, ScreenHeight, ColorDepth.ColorDepth32));
            MouseManager.ScreenHeight = (uint)ScreenHeight;
            MouseManager.ScreenWidth = (uint)ScreenWidth;

            // Register Apps
            processManager = new ProcessManager();
            processManager.RegisterApp(new ClockApp());
            processManager.RegisterApp(new CalculatorApp());
            processManager.RegisterApp(new NotepadApp());
            processManager.RegisterApp(new FileExplorerApp());
            processManager.RegisterApp(new ConsoleApp());
            processManager.RegisterApp(new SettingsApp());
            processManager.RegisterApp(new PaintApp());
            processManager.RegisterApp(new SnakeApp());
            processManager.RegisterApp(new SystemMonitorApp());

            startMenu = new Menu(processManager);
            lockScreen = new LockScreen();
            lastTime = DateTime.Now;
        }

        protected override void Run()
        {
            // FPS counter
            frames++;
            if ((DateTime.Now - lastTime).TotalSeconds >= 1)
            {
                fps = frames;
                fpsText = fps.ToString();
                frames = 0;
                lastTime = DateTime.Now;
                Cosmos.Core.Memory.Heap.Collect();
            }

            // Clamp cursor to screen
            if (MouseManager.X >= ScreenWidth) MouseManager.X = (uint)(ScreenWidth - 1);
            if (MouseManager.Y >= ScreenHeight) MouseManager.Y = (uint)(ScreenHeight - 1);

            // === LOCK SCREEN ===
            if (lockScreen.IsLocked)
            {
                lockScreen.Update();
                lockScreen.Draw(canvas);
                DrawCursor();
                canvas.Display();
                return;
            }

            // === APP RUNNING - FULL SCREEN MODE ===
            if (processManager.CurrentApp != null)
            {
                // ESC to close app
                if (KeyboardManager.TryReadKey(out KeyEvent key) && key.Key == ConsoleKeyEx.Escape)
                {
                    processManager.StopCurrentApp();
                }
                else
                {
                    processManager.Update();
                    // App draws EVERYTHING (full screen)
                    canvas.DrawFilledRectangle(BlackPen, 0, 0, ScreenWidth, ScreenHeight);
                    processManager.Draw(canvas);
                }
                
                // FPS in corner
                canvas.DrawString(fpsText, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, YellowPen, ScreenWidth - 30, 5);
                DrawCursor();
                canvas.Display();
                return;
            }

            // === DESKTOP MODE (no app running) ===
            
            // Input
            if (MouseManager.MouseState == MouseState.Left)
            {
                // Menu button (bottom-left corner)
                if (MouseManager.X < 60 && MouseManager.Y > ScreenHeight - 30)
                {
                    showMenu = !showMenu;
                    if (showMenu) startMenu.Open();
                    else startMenu.Close();
                    while (MouseManager.MouseState == MouseState.Left) { }
                }
            }

            // Update menu
            if (showMenu)
            {
                startMenu.HandleInput();
                if (!startMenu.IsVisible()) showMenu = false;
            }

            // Draw desktop
            canvas.DrawFilledRectangle(DarkBluePen, 0, 0, ScreenWidth, ScreenHeight);

            // Simple taskbar
            canvas.DrawFilledRectangle(GrayPen, 0, ScreenHeight - 30, ScreenWidth, 30);
            canvas.DrawFilledRectangle(showMenu ? GreenPen : BluePen, 0, ScreenHeight - 30, 60, 30);
            canvas.DrawString("Menu", Cosmos.System.Graphics.Fonts.PCScreenFont.Default, WhitePen, 10, ScreenHeight - 22);

            // Time
            canvas.DrawString(DateTime.Now.ToString("HH:mm"), Cosmos.System.Graphics.Fonts.PCScreenFont.Default, WhitePen, ScreenWidth - 50, ScreenHeight - 22);

            // Menu overlay
            if (showMenu)
            {
                startMenu.Draw(canvas);
            }

            // FPS
            canvas.DrawString(fpsText, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, YellowPen, ScreenWidth - 30, 5);

            DrawCursor();
            canvas.Display();
        }

        private void DrawCursor()
        {
            int x = (int)MouseManager.X;
            int y = (int)MouseManager.Y;
            // Simple triangle cursor
            canvas.DrawLine(WhitePen, x, y, x, y + 12);
            canvas.DrawLine(WhitePen, x, y, x + 8, y + 8);
            canvas.DrawLine(WhitePen, x, y + 12, x + 8, y + 8);
        }
    }
}
