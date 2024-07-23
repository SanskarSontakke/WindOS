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
using System.Drawing.Imaging;
using Sys = Cosmos.System;
using System.IO;

namespace CosmosKernel
{
    public class Kernel : Sys.Kernel
    {
        public CosmosVFS VFS;
        public CommandManager commandManager;
        public Canvas canvas;
        public MouseState prevMouseState;
        public UInt32 pX, pY;
        public bool menuOn = false;
        public bool prevMenuOn = false;
        public bool mouseOnMenu = false;
        public bool prevMouseOnMenu = false;
        public bool clockActive = false;
        public bool clockEnabled = false;
        public bool mouseOnClockIcon = false;
        public bool mouseOnClockMenuButton = false;
        public Int32 clockXpos = 300;
        public Int32 clockYpos = 300;

        public static String cursorImageString = "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";
        public static String clockIconImageString = "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";

        Bitmap cursorImageBitmap = new Bitmap(Convert.FromBase64String(cursorImageString));
        Bitmap clockIconImageBitmap = new Bitmap(Convert.FromBase64String(clockIconImageString));

        void DelayInMS(int ms) // Stops the code for milliseconds and then resumes it (Basically It's delay)
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

                if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(10, 50, 190, 40))) && (MouseManager.MouseState == MouseState.Left))
                {
                    Cosmos.System.Power.Shutdown();
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(10, 90, 190, 40))) && (MouseManager.MouseState == MouseState.Left))
                {
                    Cosmos.System.Power.Reboot();
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 120, 200, 40))) && (MouseManager.MouseState == MouseState.Left && !mouseOnClockMenuButton))
                {
                    clockXpos = 300;
                    clockYpos = 300;
                    clockActive = true;
                    clockEnabled = true;
                    mouseOnClockMenuButton = true;
                }
                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 120, 200, 40))) && mouseOnClockMenuButton)
                {
                    mouseOnClockMenuButton = false;
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 160, 200, 40))) && (MouseManager.MouseState == MouseState.Left))
                {
                    ;
                }
            }

            if (pX != MouseManager.X || pY != MouseManager.Y || prevMouseState != MouseManager.MouseState || menuOn != prevMenuOn || mouseOnMenu != prevMouseOnMenu)
            {
                DrawTabBar();
                if (clockActive && clockEnabled)
                    DrawClock();
                if (clockEnabled)
                    DrawClockIcon();
                DrawMenu();
                DrawMouse();
                this.canvas.Display();
            }

            if (clockEnabled)
                HandleClockInputs();

            pX = MouseManager.X;
            pY = MouseManager.Y;
            prevMenuOn = menuOn;
            prevMouseOnMenu = mouseOnMenu;

            Heap.Collect();
        }

        public void DrawMouse()
        {

            this.canvas.DrawImageAlpha(cursorImageBitmap, (Int32)MouseManager.X, (Int32)MouseManager.Y);
        }

        public void DrawTabBar()
        {
            this.canvas.DrawFilledRectangle(Color.DeepSkyBlue, 0, 0, (Int32)canvas.Mode.Width, 40);
            this.canvas.DrawFilledRectangle(Color.OrangeRed, 0, 0, 40, 40);
            this.canvas.DrawFilledRectangle(Color.AliceBlue, 0, 40, (Int32)canvas.Mode.Width, (Int32)canvas.Mode.Height);
        }

        public void DrawMenu()
        {
            if (menuOn == true)
            {
                this.canvas.DrawFilledRectangle(Color.OrangeRed, 0, 0, 40, 40);
                this.canvas.DrawFilledRectangle(Color.Gray, 0, 40, 210, 220);

                this.canvas.DrawFilledRectangle(Color.SkyBlue, 10, 50, 190, 40);
                this.canvas.DrawFilledRectangle(Color.IndianRed, 10, 90, 190, 40);
                this.canvas.DrawFilledRectangle(Color.Green, 10, 130, 190, 40);
                this.canvas.DrawFilledRectangle(Color.Orange, 10, 170, 190, 40);
                this.canvas.DrawFilledRectangle(Color.YellowGreen, 10, 210, 190, 40);

                this.canvas.DrawString("Power Off", PCScreenFont.Default, Color.Black, 15, 60);
                this.canvas.DrawString("Reboot", PCScreenFont.Default, Color.Black, 15, 100);
                this.canvas.DrawString("Clock", PCScreenFont.Default, Color.Black, 15, 140);
                this.canvas.DrawString("Console", PCScreenFont.Default, Color.Black, 15, 180);
            }
        }

        public void DrawClock()
        {
            this.canvas.DrawFilledRectangle(Color.DarkGray, clockXpos, clockYpos, 200, 200);
            this.canvas.DrawFilledRectangle(Color.LightCoral, clockXpos, clockYpos, 180, 20);
            this.canvas.DrawFilledRectangle(Color.IndianRed, clockXpos + 180, clockYpos, 20, 20);
            this.canvas.DrawFilledRectangle(Color.GreenYellow, clockXpos + 160, clockYpos, 20, 20);
            this.canvas.DrawString(DateTime.Today.ToString(), PCScreenFont.Default, Color.AntiqueWhite, clockXpos + 50, clockYpos + 50);
            this.canvas.DrawString(TimeOnly.FromDateTime(DateTime.Now).ToString(), PCScreenFont.Default, Color.AntiqueWhite, clockXpos + 50, clockYpos + 75);
            this.canvas.DrawFilledRectangle(Color.DarkGray, clockXpos + 130, clockYpos + 50, 70, 30);
        }

        public void DrawClockIcon()
        {
            for (int i = 0; i < 10; i++)
            {
                if (this.canvas.GetPointColor(45 + (i * 40), 0) == Color.DeepSkyBlue)
                {
                    this.canvas.DrawFilledRectangle(Color.Green, 40 + (i * 40), 0, 40, 40);
                    this.canvas.DrawImageAlpha(clockIconImageBitmap, 40 + (i * 40), 0);
                    i = 100;
                }
            }
        }

        public void HandleClockInputs()
        {
            Int32 prevX = (Int32)MouseManager.X;
            Int32 prevY = (Int32)MouseManager.Y;

            if (clockEnabled && !menuOn && clockActive && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(clockXpos, clockYpos, 150, 20))) && (MouseManager.MouseState == MouseState.Left))
            {
            repositionClock:
                if (clockEnabled && !menuOn && clockActive && (MouseManager.MouseState == MouseState.Left))
                {
                    clockXpos = clockXpos + ((Int32)MouseManager.X - prevX);
                    clockYpos = clockYpos + ((Int32)MouseManager.Y - prevY);
                    prevX = (Int32)MouseManager.X;
                    prevY = (Int32)MouseManager.Y;
                    DrawTabBar();
                    DrawClock();
                    DrawClockIcon();
                    DrawMouse();
                    this.canvas.Display();
                    Heap.Collect();
                    goto repositionClock;
                }
            }
            else if (clockEnabled && !menuOn && clockActive && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(clockXpos + 180, clockYpos, 20, 20))) && (MouseManager.MouseState == MouseState.Left))
            {
                clockActive = false;
                clockEnabled = false;
            }
            else if (clockEnabled && !menuOn && clockActive && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(clockXpos + 160, clockYpos, 20, 20))) && (MouseManager.MouseState == MouseState.Left))
            {
                clockActive = false;
            }
            else if (!mouseOnClockIcon && !menuOn && clockEnabled && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(40, 0, 40, 40))) && (MouseManager.MouseState == MouseState.Left))
            {
                clockActive = !clockActive;
                mouseOnClockIcon = true;
            }
            else if (!(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(40, 0, 40, 40))))
            {
                mouseOnClockIcon = false;
            }
        }
    }
}