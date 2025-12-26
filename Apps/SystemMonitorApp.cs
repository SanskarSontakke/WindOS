using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using System.Collections.Generic;

namespace WindOS.Apps
{
    public class SystemMonitorApp : App
    {
        private List<int> memoryHistory = new List<int>();
        private int maxHistory = 100;
        private int updateRate = 60; // Every 1 sec (60 frames)
        private int frameCount = 0;

        public SystemMonitorApp() : base("Monitor") { }

        public override void Update()
        {
            frameCount++;
            if (frameCount >= updateRate)
            {
                frameCount = 0;

                // Get Memory Usage in MB
                uint usedRAM = Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024);
                memoryHistory.Add((int)usedRAM);

                if (memoryHistory.Count > maxHistory)
                {
                    memoryHistory.RemoveAt(0);
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawFilledRectangle(Color.Black, 0, 0, 1280, 720);

            // Header
            canvas.DrawFilledRectangle(Color.DarkBlue, 0, 0, 1280, 40);
            canvas.DrawString("System Monitor", PCScreenFont.Default, Color.White, 20, 10);

            // Stats
            uint totalRAM = Cosmos.Core.CPU.GetAmountOfRAM();
            uint usedRAM = Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024);

            canvas.DrawString($"Total RAM: {totalRAM} MB", PCScreenFont.Default, Color.White, 50, 80);
            canvas.DrawString($"Used RAM: {usedRAM} MB", PCScreenFont.Default, Color.White, 50, 110);
            canvas.DrawString($"Available RAM: {totalRAM - usedRAM} MB", PCScreenFont.Default, Color.White, 50, 140);

            // Uptime
            // Note: Uptime class from original Kernel was removed/refactored,
            // but we can just use TickCount
            TimeSpan uptime = TimeSpan.FromMilliseconds(Cosmos.Core.CPU.GetCPUUptime());
            canvas.DrawString($"Uptime: {uptime.ToString(@"hh\:mm\:ss")}", PCScreenFont.Default, Color.White, 50, 200);

            // Graph
            int graphX = 50;
            int graphY = 400;
            int graphW = 600;
            int graphH = 200;

            canvas.DrawRectangle(Color.White, graphX, graphY, graphW, graphH);
            canvas.DrawString("Memory Usage History", PCScreenFont.Default, Color.White, graphX, graphY - 20);

            if (memoryHistory.Count > 1)
            {
                int stepX = graphW / maxHistory;
                for (int i = 0; i < memoryHistory.Count - 1; i++)
                {
                    int val1 = memoryHistory[i];
                    int val2 = memoryHistory[i+1];

                    // Normalize to height (assume max 512MB for graph scale or purely relative)
                    // Let's assume 256MB is top of graph for visibility
                    int h1 = (int)((val1 / 256.0) * graphH);
                    int h2 = (int)((val2 / 256.0) * graphH);

                    // Flip Y
                    int y1 = graphY + graphH - h1;
                    int y2 = graphY + graphH - h2;

                    int x1 = graphX + (i * stepX);
                    int x2 = graphX + ((i + 1) * stepX);

                    canvas.DrawLine(Color.Green, x1, y1, x2, y2);
                }
            }
        }
    }
}
