using Cosmos.Core.Memory;
using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.Graphics;
using CosmosKernel.Commands;
using System;
using Sys = Cosmos.System;
using System.Drawing;
using IL2CPU.API.Attribs;
using Cosmos.System.Graphics.Fonts;
using System.Collections.Generic;
using System.Threading;
using System.Drawing.Text;
using System.IO;
using Cosmos.Core;
using System.Reflection;
using System.Resources;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Cosmos.Core_Plugs.System.Diagnostics;

namespace WindOS
{
    public class Kernel : Sys.Kernel
    {
        //System-------------------------------------------
        public CosmosVFS VFS;
        public CommandManager commandManager;
        public Canvas canvas;
        public MouseState prevMouseState;
        public UInt32 pX, pY;
        public Int32 currentWallpaper = 0;
        public Int32 currentActiveApplication;
        private static FPSCounter fpsCounter = new FPSCounter();
        //Menu---------------------------------------------
        public bool mouseOnWallpaperMenuButton = false;
        public bool mouseOnClockMenuButton = false;
        public bool mouseOnConsoleMenuButton = false;
        public bool menuOn = false;
        public bool mouseOnMenu = false;
        public bool prevMouseOnMenu = false;
        //System Classes-----------------------------------
        public class FPSCounter
        {
            private int frameCount;
            private DateTime lastTime;
            private double fps;

            public FPSCounter()
            {
                frameCount = 0;
                lastTime = DateTime.Now;
                fps = 0.0;
            }

            public void Update()
            {
                frameCount++;
                DateTime now = DateTime.Now;
                TimeSpan elapsedTime = now - lastTime;

                if (elapsedTime.TotalSeconds >= 1)
                {
                    fps = frameCount / elapsedTime.TotalSeconds;
                    frameCount = 0;
                    lastTime = now;
                }
            }

            public void Draw(Canvas canvas, int x, int y)
            {
                canvas.DrawString($"FPS: {fps:F2}", PCScreenFont.Default, Color.White, x, y);
            }
        }
        class Stopwatch
        {
            private DateTime _startTime;
            private TimeSpan _elapsedTime;
            private bool _isRunning;

            public Stopwatch()
            {
                _elapsedTime = TimeSpan.Zero;
                _isRunning = false;
            }

            public void Start()
            {
                if (!_isRunning)
                {
                    _startTime = DateTime.Now;
                    _isRunning = true;
                }
            }

            public void Stop()
            {
                if (_isRunning)
                {
                    _elapsedTime += DateTime.Now - _startTime;
                    _isRunning = false;
                }
            }

            public void Resume()
            {
                if (!_isRunning)
                {
                    _startTime = DateTime.Now;
                    _isRunning = true;
                }
            }

            public void Reset()
            {
                _elapsedTime = TimeSpan.Zero;
                if (_isRunning)
                {
                    _startTime = DateTime.Now;
                }
            }

            public TimeSpan Elapsed()
            {
                if (_isRunning)
                {
                    return _elapsedTime + (DateTime.Now - _startTime);
                }
                else
                {
                    return _elapsedTime;
                }
            }
        }
        class Uptime
        {
            private DateTime _startTime;

            public Uptime()
            {
                _startTime = DateTime.Now;
            }

            public TimeSpan Elapsed()
            {
                return DateTime.Now - _startTime;
            }
        }
        //Clock--------------------------------------------
        Stopwatch stopWatch = new Stopwatch();
        Uptime upTime = new Uptime();
        //Fetch Resources(Wallpapers, Mouse Icon, Menu)----
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper01.bmp")] public static byte[] Wallpaper010Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper02.bmp")] public static byte[] Wallpaper020Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper03.bmp")] public static byte[] Wallpaper030Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper04.bmp")] public static byte[] Wallpaper040Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper05.bmp")] public static byte[] Wallpaper050Stream;
        [ManifestResourceStream(ResourceName = "WindOS.cursor.bmp")] public static byte[] CursorStream;
        [ManifestResourceStream(ResourceName = "WindOS.Menu.bmp")] public static byte[] MenuStream;
        [ManifestResourceStream(ResourceName = "WindOS.Clock.bmp")] public static byte[] ClockStream;
        //Convert Resource Streams to Bitmap---------------
        public static Bitmap Wallpaper010Bitmap = new Bitmap(Wallpaper010Stream);
        public static Bitmap Wallpaper020Bitmap = new Bitmap(Wallpaper020Stream);
        public static Bitmap Wallpaper030Bitmap = new Bitmap(Wallpaper030Stream);
        public static Bitmap Wallpaper040Bitmap = new Bitmap(Wallpaper040Stream);
        public static Bitmap Wallpaper050Bitmap = new Bitmap(Wallpaper050Stream);
        public static Bitmap cursorBitmap = new Bitmap(CursorStream);
        public static Bitmap menuBitmap = new Bitmap(MenuStream);
        public static Bitmap clockBitmap = new Bitmap(ClockStream);

        protected override void BeforeRun()
        {
            this.VFS = new CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(this.VFS);
            this.commandManager = new CommandManager();
            System.Console.Write(DateTime.Now);
            System.Console.Write(DateTime.Today);

            this.canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1280, 720, ColorDepth.ColorDepth32));
            this.canvas.Clear(Color.AliceBlue);
            this.canvas.Display();

            MouseManager.ScreenHeight = (UInt32)canvas.Mode.Height;
            MouseManager.ScreenWidth = (UInt32)canvas.Mode.Width;
        }

        protected override void Run()
        {
            HandleGUIInputs();
            return;

            String response;
            System.Console.WriteLine("\n");
            String input = System.Console.ReadLine();
            response = this.commandManager.processInput(input);

            System.Console.WriteLine(response);
        }

        public void DelayInMS(int ms)
        {
            for (int i = 0; i < ms * 100000; i++)
            {
                ;
                ;
                ;
                ;
                ;
            }
        }

        public void HandleGUIInputs()
        {
            if (pX != MouseManager.X || pY != MouseManager.Y || prevMouseState != MouseManager.MouseState)
            {
                if (!mouseOnMenu && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 0, 40, 40))) && (MouseManager.MouseState == MouseState.Left))
                {
                    menuOn = !menuOn;
                    mouseOnMenu = true;
                }
                else if (!(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 0, 40, 40))))
                {
                    mouseOnMenu = false;
                }

                if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(10, 50, 190, 60))) && (MouseManager.MouseState == MouseState.Left))
                {
                    Cosmos.System.Power.Shutdown();
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(10, 120, 190, 60))) && (MouseManager.MouseState == MouseState.Left))
                {
                    Cosmos.System.Power.Reboot();
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 190, 200, 60))) && (MouseManager.MouseState == MouseState.Left) && !mouseOnClockMenuButton)
                {
                    if (currentActiveApplication != 1)
                        currentActiveApplication = 1;
                    else if (currentActiveApplication == 1)
                        currentActiveApplication = 0;
                    mouseOnClockMenuButton = true;
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 260, 200, 60))) && (MouseManager.MouseState == MouseState.Left && !mouseOnConsoleMenuButton))
                {
                    if (currentActiveApplication != 2)
                        currentActiveApplication = 2;
                    else if (currentActiveApplication == 2)
                        currentActiveApplication = 0;
                    mouseOnConsoleMenuButton = true;
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 330, 200, 60))) && (MouseManager.MouseState == MouseState.Left) && !mouseOnWallpaperMenuButton)
                {
                    if (currentWallpaper == 4)
                        currentWallpaper = 0;
                    else
                        currentWallpaper++;
                    mouseOnWallpaperMenuButton = true;
                }
                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 330, 200, 60))) && mouseOnWallpaperMenuButton)
                {
                    mouseOnWallpaperMenuButton = false;
                }
                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 190, 200, 60))) && mouseOnClockMenuButton)
                {
                    mouseOnClockMenuButton = false;
                }
                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 330, 200, 60))) && mouseOnConsoleMenuButton)
                {
                    mouseOnConsoleMenuButton = false;
                }
            }

            switch (currentActiveApplication)
            {
                case 0:
                    DrawBackground();
                    break;
                case 1:
                    Clock();
                    break;
                case 2:
                    Console();
                    break;
                default:
                    DrawBackground();
                    break;
            }
            DrawTabBar();
            DrawMenu();
            DrawMouse();
            fpsCounter.Update();
            fpsCounter.Draw(canvas, 1150, 5);
            this.canvas.Display();

            pX = MouseManager.X;
            pY = MouseManager.Y;
            prevMouseOnMenu = mouseOnMenu;

            Heap.Collect();
        }

        public void DrawMouse()
        {
            this.canvas.DrawImageAlpha(cursorBitmap, (Int32)MouseManager.X, (Int32)MouseManager.Y);
        }

        public void DrawTabBar()
        {
            this.canvas.DrawFilledRectangle(Color.CadetBlue, 0, 0, 1280, 40);
            this.canvas.DrawFilledRectangle(Color.OrangeRed, 0, 0, 40, 40);
        }

        public void DrawBackground()
        {
            switch (currentWallpaper)
            {
                case 0:
                    this.canvas.DrawImage(Wallpaper010Bitmap, 0, 0);
                    break;
                case 1:
                    this.canvas.DrawImage(Wallpaper020Bitmap, 0, 0);
                    break;
                case 2:
                    this.canvas.DrawImage(Wallpaper030Bitmap, 0, 0);
                    break;
                case 3:
                    this.canvas.DrawImage(Wallpaper040Bitmap, 0, 0);
                    break;
                case 4:
                    this.canvas.DrawImage(Wallpaper050Bitmap, 0, 0);
                    break;
                default:
                    break;
            }
        }

        public void DrawMenu()
        {
            if (menuOn == true)
            {
                this.canvas.DrawImage(menuBitmap, 0, 40);
            }
        }

        public void Clock()
        {
            this.canvas.DrawLine(Color.Black, 0, 0, 0, 720);
            this.canvas.DrawImage(clockBitmap, 1, 0);
            this.canvas.DrawLine(Color.Black, 1280, 0, 1280, 720);
            this.canvas.DrawString(DateTime.Now.ToString(), PCScreenFont.Default, Color.Black, 190, 140);
            this.canvas.DrawString((DateTime.UtcNow).ToString(), PCScreenFont.Default, Color.Black, 190, 202);
            this.canvas.DrawString((DateTime.Today).ToString(), PCScreenFont.Default, Color.Black, 190, 265);
            this.canvas.DrawString((stopWatch.Elapsed()).ToString(), PCScreenFont.Default, Color.Black, 1049, 443);
            this.canvas.DrawString((upTime.Elapsed()).ToString(), PCScreenFont.Default, Color.WhiteSmoke, 1049, 156);

            if (!menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(898, 537, 189, 96))) && (MouseManager.MouseState == MouseState.Left))
                stopWatch.Start();
            else if (!menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(1087, 537, 191, 96))) && (MouseManager.MouseState == MouseState.Left))
                stopWatch.Stop();
            else if (!menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(896, 636, 382, 84))) && (MouseManager.MouseState == MouseState.Left))
                stopWatch.Reset();
        }

        public void Console()
        {
            this.canvas.Clear(Color.GreenYellow);
        }
    }
}