using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using WindOS.System;

namespace WindOS.Apps
{
    public class ClockApp : App
    {
        private bool showStopwatch = false;
        private bool showTimer = false;

        // Stopwatch state
        private DateTime stopwatchStart;
        private TimeSpan stopwatchElapsed;
        private bool stopwatchRunning = false;

        // Timer state
        private TimeSpan timerDuration = TimeSpan.Zero;
        private DateTime timerStart;
        private bool timerRunning = false;
        private string timerInput = "";

        public ClockApp() : base("Clock") { }

        public override void Update()
        {
            if (MouseManager.MouseState == MouseState.Left)
            {
                // Simple tabs at bottom
                if (MouseManager.Y > 650)
                {
                    if (MouseManager.X > 200 && MouseManager.X < 400) { showStopwatch = false; showTimer = false; }
                    if (MouseManager.X > 500 && MouseManager.X < 700) { showStopwatch = true; showTimer = false; }
                    if (MouseManager.X > 800 && MouseManager.X < 1000) { showStopwatch = false; showTimer = true; }
                }

                // Interaction
                if (showStopwatch)
                {
                    if (new Rectangle((int)MouseManager.X, (int)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(500, 400, 100, 50))) // Start/Stop
                    {
                        if (stopwatchRunning)
                        {
                            stopwatchElapsed += DateTime.Now - stopwatchStart;
                            stopwatchRunning = false;
                        }
                        else
                        {
                            stopwatchStart = DateTime.Now;
                            stopwatchRunning = true;
                        }
                        // Debounce
                        while(MouseManager.MouseState == MouseState.Left);
                    }
                    if (new Rectangle((int)MouseManager.X, (int)MouseManager.Y, 1, 1).IntersectsWith(new Rectangle(650, 400, 100, 50))) // Reset
                    {
                        stopwatchRunning = false;
                        stopwatchElapsed = TimeSpan.Zero;
                    }
                }
                else if (showTimer)
                {
                    // Basic timer control logic would go here
                }
            }

            if (showTimer && !timerRunning)
            {
                if (KeyboardManager.TryReadKey(out KeyEvent key))
                {
                     if (char.IsDigit(key.KeyChar))
                     {
                         timerInput += key.KeyChar;
                     }
                     else if (key.Key == ConsoleKeyEx.Enter && timerInput.Length > 0)
                     {
                         int seconds = int.Parse(timerInput);
                         timerDuration = TimeSpan.FromSeconds(seconds);
                         timerStart = DateTime.Now;
                         timerRunning = true;
                         timerInput = "";
                     }
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
             canvas.DrawFilledRectangle(Color.Black, 0, 0, 1280, 720);

             // Tabs
             canvas.DrawFilledRectangle(Color.Gray, 200, 650, 200, 50);
             canvas.DrawString("Clock", PCScreenFont.Default, Color.White, 270, 665);

             canvas.DrawFilledRectangle(Color.Gray, 500, 650, 200, 50);
             canvas.DrawString("Stopwatch", PCScreenFont.Default, Color.White, 550, 665);

             canvas.DrawFilledRectangle(Color.Gray, 800, 650, 200, 50);
             canvas.DrawString("Timer", PCScreenFont.Default, Color.White, 870, 665);

             if (showStopwatch)
             {
                 TimeSpan total = stopwatchElapsed;
                 if (stopwatchRunning) total += (DateTime.Now - stopwatchStart);

                 canvas.DrawString(total.ToString(@"hh\:mm\:ss\.fff"), PCScreenFont.Default, Color.White, 550, 300);

                 canvas.DrawFilledRectangle(Color.Green, 500, 400, 100, 50);
                 canvas.DrawString(stopwatchRunning ? "Stop" : "Start", PCScreenFont.Default, Color.Black, 520, 415);

                 canvas.DrawFilledRectangle(Color.Red, 650, 400, 100, 50);
                 canvas.DrawString("Reset", PCScreenFont.Default, Color.Black, 670, 415);
             }
             else if (showTimer)
             {
                 if (timerRunning)
                 {
                     TimeSpan remaining = timerDuration - (DateTime.Now - timerStart);
                     if (remaining <= TimeSpan.Zero)
                     {
                         remaining = TimeSpan.Zero;
                         timerRunning = false;
                         // Alarm sound could go here
                     }
                     canvas.DrawString(remaining.ToString(@"hh\:mm\:ss"), PCScreenFont.Default, Color.White, 550, 300);
                 }
                 else
                 {
                     canvas.DrawString("Enter Seconds: " + timerInput, PCScreenFont.Default, Color.White, 500, 300);
                 }
             }
             else
             {
                 string time = DateTime.Now.ToString("HH:mm:ss");
                 canvas.DrawString(time, PCScreenFont.Default, Color.White, 580, 300);
                 canvas.DrawString(DateTime.Now.ToString("D"), PCScreenFont.Default, Color.Gray, 550, 350);
             }
        }
    }
}
