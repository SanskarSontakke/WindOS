using Cosmos.Core.Memory;
using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.Graphics;
using CosmosKernel.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Drawing.Text;
using Sys = Cosmos.System;
using System.IO;
using Cosmos.Core;
using IL2CPU.API.Attribs;
using System.Reflection;
using System.Resources;
using System.Reflection.Metadata;

namespace WindOS
{
    public class Kernel : Sys.Kernel
    {
        public CosmosVFS VFS;
        public CommandManager commandManager;
        public Canvas canvas;
        public MouseState prevMouseState;
        public UInt32 pX, pY;
        public int currentWallpaper = 0;
        public bool mouseOnWallpaperMenuButton = false;

        public bool menuOn = false;
        public bool mouseOnMenu = false;
        public bool prevMouseOnMenu = false;
        public Int32 currentActiveApplication = 0;

        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper010.bmp")] public static byte[] Wallpaper010Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper020.bmp")] public static byte[] Wallpaper020Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper030.bmp")] public static byte[] Wallpaper030Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper040.bmp")] public static byte[] Wallpaper040Stream;
        [ManifestResourceStream(ResourceName = "WindOS.Wallpaper050.bmp")] public static byte[] Wallpaper050Stream;
        [ManifestResourceStream(ResourceName = "WindOS.cursor.bmp")] public static byte[] CursorStream;
        [ManifestResourceStream(ResourceName = "WindOS.Menu.bmp")] public static byte[] MenuStream;

        public static Bitmap Wallpaper010Bitmap = new Bitmap(Wallpaper010Stream);
        public static Bitmap Wallpaper020Bitmap = new Bitmap(Wallpaper020Stream);
        public static Bitmap Wallpaper030Bitmap = new Bitmap(Wallpaper030Stream);
        public static Bitmap Wallpaper040Bitmap = new Bitmap(Wallpaper040Stream);
        public static Bitmap Wallpaper050Bitmap = new Bitmap(Wallpaper050Stream);
        public static Bitmap cursorBitmap = new Bitmap(CursorStream);
        public static Bitmap menuBitmap = new Bitmap(MenuStream);

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
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 190, 200, 60))) && (MouseManager.MouseState == MouseState.Left))
                {
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 260, 200, 60))) && (MouseManager.MouseState == MouseState.Left))
                {
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
                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 190, 200, 60))))
                {
                }
            }


            DrawTabBar();

            DrawMenu();
            DrawMouse();
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
            this.canvas.DrawFilledRectangle(Color.OrangeRed, 0, 0, 40, 40);
        }

        public void DrawMenu()
        {
            if (menuOn == true)
            {
                this.canvas.DrawImage(menuBitmap, 0, 40);
            }
        }
    }
}