using Cosmos.Core.Memory;
using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.Graphics;
using System;
using Sys = Cosmos.System;
using System.Drawing;
using IL2CPU.API.Attribs;
using Cosmos.System.Graphics.Fonts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.IO;
using Cosmos.Core;
using System.Reflection;
using System.Resources;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Cosmos.Core_Plugs.System.Diagnostics;
using Cosmos.HAL;
using System.Text;
using Cosmos.System.Network;
using System.Net.Http;

//Kernel------------------------------------------------------------------------------------
namespace WindOS
{
    public class Kernel : Sys.Kernel
    {
        //System----------------------------------------------------------------------------
        public CosmosVFS VFS;
        public CommandManager commandManager;
        public Canvas canvas;
        public MouseState prevMouseState;
        public UInt32 pX, pY;
        public Int32 currentWallpaper = 0;
        public Int32 currentActiveApplication;
        public static FPSCounter fpsCounter = new FPSCounter();
        public static Clock clock = new Clock();
        public static Console console = new Console();
        public static Menu menu = new Menu();
        public static Background background = new Background();
        public static TabBar tabBar = new TabBar();
        public static Mouse mouse = new Mouse();
        public static Delay delay = new Delay();
        public static LoadingScreen loadingScreen = new LoadingScreen();
        //Menu------------------------------------------------------------------------------
        public class Menu
        {
            public Menu()
            {
                ;
            }

            public void Draw(Canvas _canvas, bool _menuOn)
            {
                if (_menuOn == true)
                {
                    _canvas.DrawImage(menuBitmap, 0, 40);
                }
            }
        }
        public bool mouseOnWallpaperMenuButton = false;
        public bool mouseOnClockMenuButton = false;
        public bool mouseOnConsoleMenuButton = false;
        public bool menuOn = false;
        public bool mouseOnMenu = false;
        public bool prevMouseOnMenu = false;
        //System Classes--------------------------------------------------------------------
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

            public void Update(Canvas _canvas, int _x, int _y)
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

                Draw(_canvas, _x, _y);
            }

            private void Draw(Canvas _canvas, int _x, int _y)
            {
                _canvas.DrawString($"FPS: {fps:F2}", PCScreenFont.Default, Color.White, _x, _y);
            }
        }
        public class Stopwatch
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
        public class Uptime
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
        public class Background
        {
            public Background()
            {
                ;
            }

            public void Draw(Canvas _canvas, int _currentWallpaper)
            {
                switch (_currentWallpaper)
                {
                    case 0:
                        _canvas.DrawImage(Wallpaper010Bitmap, 0, 0);
                        break;
                    case 1:
                        _canvas.DrawImage(Wallpaper020Bitmap, 0, 0);
                        break;
                    case 2:
                        _canvas.DrawImage(Wallpaper030Bitmap, 0, 0);
                        break;
                    case 3:
                        _canvas.DrawImage(Wallpaper040Bitmap, 0, 0);
                        break;
                    case 4:
                        _canvas.DrawImage(Wallpaper050Bitmap, 0, 0);
                        break;
                    default:
                        break;
                }
            }
        }
        public class Mouse
        {
            public Mouse()
            {
                ;
            }

            public void Draw(Canvas _canvas)
            {
                _canvas.DrawImageAlpha(cursorBitmap, (Int32)MouseManager.X, (Int32)MouseManager.Y);
            }
        }
        public class Delay
        {
            public Delay()
            {
                ;
            }

            public void Halt(int _timeinMS)
            {
                for (int i = 0; i < _timeinMS * 10000; i++)
                {
                    ;
                    ;
                    ;
                    ;
                    ;
                }
            }
        }
        public class LoadingScreen
        {
            public LoadingScreen()
            {
                ;
            }

            public void Draw(Canvas _canvas)
            {
                _canvas.DrawFilledRectangle(Color.WhiteSmoke, 0, 0, 1280, 720);
                _canvas.Display();
                delay.Halt(15000);
                for (int i = 0; i < 450; i++)
                {
                    _canvas.DrawFilledRectangle(Color.Green, 415, 335, i, 50);
                    _canvas.DrawRectangle(Color.Black, 415, 335, 450, 50);
                    _canvas.Display();
                    delay.Halt(200);
                }
            }
        }
        //TabBar----------------------------------------------------------------------------
        public class TabBar
        {
            public TabBar()
            {
                ;
            }

            public void Draw( Canvas _canvas)
            {
                _canvas.DrawFilledRectangle(Color.CadetBlue, 0, 0, 1280, 40);
                _canvas.DrawFilledRectangle(Color.OrangeRed, 0, 0, 40, 40);
            }
        }
        //Clock-----------------------------------------------------------------------------
        public class Clock
        {
            public Clock()
            {
                ;
            }

            private void Draw(Canvas _canvas, Stopwatch _stopWatch, Uptime _upTime)
            {
                _canvas.DrawLine(Color.Black, 0, 0, 0, 720);
                _canvas.DrawLine(Color.Black, 1280, 0, 1280, 720);
                _canvas.DrawImage(clockBitmap, 1, 0);
                _canvas.DrawString(DateTime.Now.ToString(), PCScreenFont.Default, Color.Black, 190, 140);
                _canvas.DrawString((DateTime.UtcNow).ToString(), PCScreenFont.Default, Color.Black, 190, 202);
                _canvas.DrawString((DateTime.Today).ToString(), PCScreenFont.Default, Color.Black, 190, 265);
                _canvas.DrawString((_stopWatch.Elapsed()).ToString(), PCScreenFont.Default, Color.Black, 1049, 443);
                _canvas.DrawString((_upTime.Elapsed()).ToString(), PCScreenFont.Default, Color.WhiteSmoke, 1049, 156);
            }

            public void Update(Canvas _canvas, bool _menuOn, Stopwatch _stopWatch, Uptime _upTime)
            {
                if (!_menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(898, 537, 189, 96))) && (MouseManager.MouseState == MouseState.Left))
                    _stopWatch.Start();
                else if (!_menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(1087, 537, 191, 96))) && (MouseManager.MouseState == MouseState.Left))
                    _stopWatch.Stop();
                else if (!_menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(896, 636, 382, 84))) && (MouseManager.MouseState == MouseState.Left))
                    _stopWatch.Reset();

                Draw(_canvas, _stopWatch, _upTime);
            }
        }
        Stopwatch stopWatch = new Stopwatch();
        Uptime upTime = new Uptime();
        //Console---------------------------------------------------------------------------
        public class Console
        {
            private string _input;
            private string _response;

            public Console()
            {
                ;
            }

            public void Draw(Canvas _canvas)
            {
                _canvas.DrawImage(consoleBitmap, 3, 2);
                _canvas.DrawLine(Color.LimeGreen, 0, 0, 0, 720);
                _canvas.DrawLine(Color.LimeGreen, 1, 0, 1, 720);
                _canvas.DrawLine(Color.LimeGreen, 2, 0, 2, 720);
                _canvas.DrawLine(Color.LimeGreen, 3, 0, 3, 720);
                _canvas.DrawLine(Color.LimeGreen, 4, 0, 4, 720);
                _canvas.DrawLine(Color.LimeGreen, 1280, 0, 1280, 720);
                _canvas.DrawLine(Color.LimeGreen, 1279, 0, 1279, 720);
                _canvas.DrawLine(Color.LimeGreen, 1278, 0, 1278, 720);
                _canvas.DrawLine(Color.LimeGreen, 1277, 0, 1277, 720);
                _canvas.DrawLine(Color.LimeGreen, 1276, 0, 1276, 720);
                _canvas.DrawLine(Color.LimeGreen, 1275, 0, 1275, 720);
                _canvas.DrawString(_input, PCScreenFont.Default, Color.LightGreen, 100, 100);
                string[] lines = _response.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    _canvas.DrawString(lines[i], PCScreenFont.Default, Color.BlueViolet, 100, 175 + (i * 25));
                }
            }

            public void Update(bool _menuOn, CommandManager _commandManager)
            {
                if (KeyboardManager.TryReadKey(out KeyEvent keyEvent) && !_menuOn)
                {
                    if (keyEvent.Key == ConsoleKeyEx.Backspace && _input.Length > 0)
                        _input = _input.Substring(0, _input.Length - 1);
                    else if (keyEvent.Key == ConsoleKeyEx.Enter)
                    {
                        _response = null;
                        _response = _commandManager.processInput(_input);
                        _input = null;
                    }
                    else if (keyEvent.KeyChar != 0)
                        _input += keyEvent.KeyChar;
                }
            }
        }
        public string input;
        public string response;
        //Fetch Resources(Wallpapers, Mouse Icon, Menu)-------------------------------------
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper01.bmp")] public static byte[] Wallpaper010Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper02.bmp")] public static byte[] Wallpaper020Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper03.bmp")] public static byte[] Wallpaper030Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper04.bmp")] public static byte[] Wallpaper040Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper05.bmp")] public static byte[] Wallpaper050Stream;
        [ManifestResourceStream(ResourceName = "WindOS.cursor.bmp")] public static byte[] CursorStream;
        [ManifestResourceStream(ResourceName = "WindOS.Menu.bmp")] public static byte[] MenuStream;
        [ManifestResourceStream(ResourceName = "WindOS.Clock.bmp")] public static byte[] ClockStream;
        [ManifestResourceStream(ResourceName = "WindOS.Settings.bmp")] public static byte[] ConsoleStream;
        [ManifestResourceStream(ResourceName = "WindOS.OpenSans-Bold.ttf")] public static byte[] FontStreamByte;
        //Convert Resource Streams to Bitmap------------------------------------------------
        public static Bitmap Wallpaper010Bitmap = new Bitmap(Wallpaper010Stream);
        public static Bitmap Wallpaper020Bitmap = new Bitmap(Wallpaper020Stream);
        public static Bitmap Wallpaper030Bitmap = new Bitmap(Wallpaper030Stream);
        public static Bitmap Wallpaper040Bitmap = new Bitmap(Wallpaper040Stream);
        public static Bitmap Wallpaper050Bitmap = new Bitmap(Wallpaper050Stream);
        public static Bitmap cursorBitmap = new Bitmap(CursorStream);
        public static Bitmap menuBitmap = new Bitmap(MenuStream);
        public static Bitmap clockBitmap = new Bitmap(ClockStream);
        public static Bitmap consoleBitmap = new Bitmap(ConsoleStream);
        //Initilize-CGS----------------------------------------------------------------------
        protected override void BeforeRun()
        {
            //Initilize-CGS------------------------------------------------------------------
            this.canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1280, 720, ColorDepth.ColorDepth32));
            MouseManager.ScreenHeight = (UInt32)canvas.Mode.Height;
            MouseManager.ScreenWidth = (UInt32)canvas.Mode.Width;
            //Initilize-VFS------------------------------------------------------------------
            this.VFS = new CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(this.VFS);
            //Initilize-Commsnd-Manager------------------------------------------------------
            this.commandManager = new CommandManager();
            //Loading-Screen-----------------------------------------------------------------
            loadingScreen.Draw(canvas);
        }
        //Run-OS-----------------------------------------------------------------------------
        protected override void Run()
        {
            HandleGUIInputs();
            return;
        }
        //Handle-Everything------------------------------------------------------------------
        public void HandleGUIInputs()
        {
            //Handle-Mouse-------------------------------------------------------------------
            if (pX != MouseManager.X || pY != MouseManager.Y || prevMouseState != MouseManager.MouseState)
            {
                if (!mouseOnMenu && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 0, 40, 40))) && (MouseManager.MouseState == MouseState.Left))
                {
                    menuOn = !menuOn;
                    mouseOnMenu = true;
                }
                else if (!(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 0, 40, 40))))
                    mouseOnMenu = false;

                if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(10, 50, 190, 60))) && (MouseManager.MouseState == MouseState.Left))
                    Cosmos.System.Power.Shutdown();

                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(10, 120, 190, 60))) && (MouseManager.MouseState == MouseState.Left))
                    Cosmos.System.Power.Reboot();

                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 190, 200, 60))) && (MouseManager.MouseState == MouseState.Left) && !mouseOnClockMenuButton)
                {
                    currentActiveApplication = 1;
                    mouseOnClockMenuButton = true;
                }
                else if (currentActiveApplication != 2 && menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 260, 200, 60))) && (MouseManager.MouseState == MouseState.Left && !mouseOnConsoleMenuButton))
                {
                    while (KeyboardManager.TryReadKey(out KeyEvent keyEvent))
                        ;
                    input = null;
                    response = null;
                    currentActiveApplication = 2;
                    mouseOnConsoleMenuButton = true;
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 330, 200, 60))) && (MouseManager.MouseState == MouseState.Left) && !mouseOnWallpaperMenuButton)
                {
                    if (currentActiveApplication == 0)
                    {
                        if (currentWallpaper == 4)
                            currentWallpaper = 0;
                        else
                            currentWallpaper++;
                    }
                    else
                    {
                        currentActiveApplication = 0;
                    }
                    mouseOnWallpaperMenuButton = true;
                }
                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 330, 200, 60))) && mouseOnWallpaperMenuButton)
                    mouseOnWallpaperMenuButton = false;

                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 190, 200, 60))) && mouseOnClockMenuButton)
                    mouseOnClockMenuButton = false;

                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 330, 200, 60))) && mouseOnConsoleMenuButton)
                    mouseOnConsoleMenuButton = false;

            }
            //Handle-Application-------------------------------------------------------------
            switch (currentActiveApplication)
            {
                case 0:
                    background.Draw(canvas, currentWallpaper);
                    break;
                case 1:
                    clock.Update(canvas, menuOn, stopWatch, upTime);
                    break;
                case 2:
                    console.Update(menuOn, commandManager);
                    console.Draw(canvas);
                    break;
                default:
                    background.Draw(canvas, currentWallpaper);
                    break;
            }
            //Draw-System-Components----------------------------------------------------------
            tabBar.Draw(canvas);
            menu.Draw(canvas, menuOn);
            fpsCounter.Update(canvas, 1150, 5);
            mouse.Draw(canvas);
            canvas.Display();
            //Adjust-System-Components--------------------------------------------------------
            pX = MouseManager.X;
            pY = MouseManager.Y;
            prevMouseOnMenu = mouseOnMenu;
            Heap.Collect();
        }
    }
}

//Command-Manager-Base----------------------------------------------------------------------
namespace WindOS
{
    public class Command
    {
        public readonly String name;

        public Command(String name) { this.name = name; }

        public virtual String execute(String[] args) { return ""; }
    }
}

//Command-Manager---------------------------------------------------------------------------
namespace WindOS
{
    public class CommandManager
    {
        public List<Command> commands;

        public CommandManager()
        {
            this.commands = new List<Command>(4)
            {
                new Help("help"),
                new About("about"),
                new Reboot("reboot"),
                new Shutdown("shutdown"),
            };
        }

        public String processInput(String input)
        {

            String[] split = input.Split(' ');
            String label = split[0];
            List<String> args = new List<String>();
            int ctr = 0;

            foreach (String s in split)
            {
                if (ctr != 0)
                    args.Add(s);
                ++ctr;
            }

            foreach (Command cmd in this.commands)
            {
                if (cmd.name == label)
                    return cmd.execute(args.ToArray());
            }

            return "Your command \"" + label + "\" does not exist";
        }
    }
}

//Help-Command------------------------------------------------------------------------------
namespace WindOS
{
    public class Help : Command
    {

        public Help(String name) : base(name) { }

        public override String execute(String[] args)
        {
            return "    Commands :                                                       \n" +
                   "          Command     Use.                                           \n" +
                   "        1. help      : Provide help.                                 \n" +
                   "        2. about     : About.                                        \n" +
                   "        3. shutdown  : Shutdown.                                     \n" +
                   "        4. reboot    : Reboot.                                       \n";
        }
    }
}

//About-Command-----------------------------------------------------------------------------
namespace WindOS
{
    public class About : Command
    {

        public About(String name) : base(name) { }

        public override String execute(String[] args)
        {
            return "About Center : \n" +
                   "    Version : 0.9.3 \n" +
                   "    Made by : Sanskar \n" +
                   "    Made using : COSMOS\n" +
                   "    Mode : Cosmos Graphics Subsystem(CGS) (GUI)\n";
        }
    }
}

//Shutdown-Command--------------------------------------------------------------------------
namespace WindOS
{
    public class Shutdown : Command
    {

        public Shutdown(String name) : base(name) { }

        public override String execute(String[] args)
        {
            Cosmos.System.Power.Shutdown();
            return "Shutting down";
        }
    }
}

//Reboot-Command----------------------------------------------------------------------------
namespace WindOS
{
    public class Reboot : Command
    {

        public Reboot(String name) : base(name) { }

        public override String execute(String[] args)
        {
            Cosmos.System.Power.Reboot();
            return "Rebooting";
        }
    }
}