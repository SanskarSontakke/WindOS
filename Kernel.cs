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
        public Int32 currentActiveApplication = 0;

        public bool clockActive = false;
        public bool clockEnabled = false;
        public bool mouseOnClockIcon = false;
        public bool mouseOnClockMenuButton = false;
        public Int32 clockXpos = 300;
        public Int32 clockYpos = 300;
        public Int32 clockIconXpos = 40;
        public Int32 clockIconYpos = 0;

        public bool consoleActive = false;
        public bool consoleEnabled = false;
        public bool mouseOnConsoleIcon = false;
        public bool mouseOnConsoleMenuButton = false;
        public Int32 consoleXpos = 400;
        public Int32 consoleYpos = 400;

        public static String cursorImageString = "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";
        
        Bitmap cursorImageBitmap = new Bitmap(Convert.FromBase64String(cursorImageString));
        
        void DelayInMS(int ms)
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
                    currentActiveApplication = 1;
                }
                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 120, 200, 40))) && mouseOnClockMenuButton)
                {
                    mouseOnClockMenuButton = false;
                }
                else if (menuOn && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 160, 200, 40))) && (MouseManager.MouseState == MouseState.Left && !mouseOnConsoleMenuButton))
                {
                    consoleXpos = 300;
                    consoleYpos = 300;
                    consoleActive = true;
                    consoleEnabled = true;
                    mouseOnConsoleMenuButton = true;
                    currentActiveApplication = 2;
                }
                else if (menuOn && !(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(0, 160, 200, 40))) && mouseOnClockMenuButton)
                {
                    mouseOnConsoleMenuButton = false;
                }
            }

            if (pX != MouseManager.X || pY != MouseManager.Y || prevMouseState != MouseManager.MouseState || menuOn != prevMenuOn || mouseOnMenu != prevMouseOnMenu)
            {
                DrawTabBar();

                switch (currentActiveApplication)
                {
                    case 0:
                        if (clockEnabled)
                        {
                            DrawClock();
                            HandleClockInputs();
                        }
                        if (consoleEnabled)
                        {
                            DrawConsole();
                            HandleConsoleInputs();
                        }
                        break;

                    case 1:
                        if(consoleActive)
                            DrawConsole();
                        if (clockActive)
                        {
                            DrawClock();
                        }
                        HandleClockInputs();
                        if(consoleEnabled)
                            HandleConsoleInputs();
                        break;

                    case 2:
                        if (clockActive)
                            DrawClock();
                        if (consoleActive)
                        {
                            DrawConsole();
                        }
                        HandleConsoleInputs();
                        if(clockEnabled)
                            HandleClockInputs();
                        break;
                }

                DrawMenu();
                DrawMouse();
                this.canvas.Display();
            }

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
            if (consoleEnabled)
                DrawConsoleIcon();
            if (clockEnabled)
                DrawClockIcon();
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
                    clockIconXpos = 40 + (i * 40); 
                    clockIconYpos = 0;
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
                currentActiveApplication = 1;
            repositionClock:
                if (clockEnabled && !menuOn && clockActive && (MouseManager.MouseState == MouseState.Left))
                {
                    clockXpos = clockXpos + ((Int32)MouseManager.X - prevX);
                    clockYpos = clockYpos + ((Int32)MouseManager.Y - prevY);
                    prevX = (Int32)MouseManager.X;
                    prevY = (Int32)MouseManager.Y;
                    DrawTabBar(); 
                    if (consoleActive)
                        DrawConsole();
                    DrawClock();
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
                currentActiveApplication = 0;
            }
            else if (clockEnabled && !menuOn && clockActive && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(clockXpos + 160, clockYpos, 20, 20))) && (MouseManager.MouseState == MouseState.Left))
            {
                clockActive = false;
            }
            else if (!mouseOnClockIcon && !menuOn && clockEnabled && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(clockIconXpos, clockIconYpos, 40, 40))) && (MouseManager.MouseState == MouseState.Left))
            {
                clockActive = !clockActive;
                mouseOnClockIcon = true;
            }
            else if (!(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(40, 0, 40, 40))))
            {
                mouseOnClockIcon = false;
            }
        }
    
        public void DrawConsole()
        {
            this.canvas.DrawFilledRectangle(Color.SpringGreen, consoleXpos, consoleYpos, 200, 200);
        }

        public void HandleConsoleInputs()
        {

            Int32 prevX = (Int32)MouseManager.X;
            Int32 prevY = (Int32)MouseManager.Y;

            if (consoleEnabled && !menuOn && consoleActive && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(consoleXpos, consoleYpos, 150, 20))) && (MouseManager.MouseState == MouseState.Left))
            {
                currentActiveApplication = 2;
            repositionConsole:
                if (consoleEnabled && !menuOn && consoleActive && (MouseManager.MouseState == MouseState.Left))
                {
                    consoleXpos = consoleXpos + ((Int32)MouseManager.X - prevX);
                    consoleYpos = consoleYpos + ((Int32)MouseManager.Y - prevY);
                    prevX = (Int32)MouseManager.X;
                    prevY = (Int32)MouseManager.Y;
                    DrawTabBar();
                    if(clockActive)
                        DrawClock();
                    DrawConsole();
                    DrawMouse();
                    this.canvas.Display();
                    Heap.Collect();
                    goto repositionConsole;
                }
            }/*
            else if (consoleEnabled && !menuOn && consoleActive && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(clockXpos + 180, clockYpos, 20, 20))) && (MouseManager.MouseState == MouseState.Left))
            {
                consoleActive = false;
                consoleEnabled = false;
            currentActiveApplication = 0;
            }
            else if (consoleEnabled && !menuOn && consoleActive && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(clockXpos + 160, clockYpos, 20, 20))) && (MouseManager.MouseState == MouseState.Left))
            {
                consoleActive = false;
            currentActiveApplication = 0;
            }
            else if (!mouseOnClockIcon && !menuOn && consoleEnabled && (new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(40, 0, 40, 40))) && (MouseManager.MouseState == MouseState.Left))
            {
                consoleActive = !consoleActive;
                mouseOnConsoleIcon = true;
            }
            else if (!(new Rectangle((Int32)MouseManager.X, (Int32)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(40, 0, 40, 40))))
            {
                mouseOnConsoleIcon = false;
            }*/
        }

        public void DrawConsoleIcon()
        {
            for (int i = 0; i < 10; i++)
            {
                if (this.canvas.GetPointColor(45 + (i * 40), 0) == Color.DeepSkyBlue)
                {
                    this.canvas.DrawFilledRectangle(Color.SpringGreen, 40 + (i * 40), 0, 40, 40);
                    i = 100;
                }
            }
        }
    }
}