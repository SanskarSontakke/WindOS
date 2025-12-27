using System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace WindOS.Apps
{
    // Full-screen System Monitor for 640x480
    public class SystemMonitorApp : App
    {
        public SystemMonitorApp() : base("Monitor") { }

        public override void Update() { }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawString("SYSTEM MONITOR [ESC Exit]", PCScreenFont.Default, Kernel.CyanPen, Kernel.ScreenWidth / 2 - 100, 20);

            uint totalRAM = Cosmos.Core.CPU.GetAmountOfRAM();
            uint usedRAM = Cosmos.Core.GCImplementation.GetUsedRAM() / (1024 * 1024);
            TimeSpan uptime = TimeSpan.FromMilliseconds(Cosmos.Core.CPU.GetCPUUptime());

            int y = 80;
            int x = 50;

            canvas.DrawString("=== MEMORY ===", PCScreenFont.Default, Kernel.YellowPen, x, y);
            y += 30;
            canvas.DrawString($"Total RAM: {totalRAM} MB", PCScreenFont.Default, Kernel.WhitePen, x, y);
            y += 25;
            canvas.DrawString($"Used RAM:  {usedRAM} MB", PCScreenFont.Default, Kernel.WhitePen, x, y);
            y += 25;
            canvas.DrawString($"Free RAM:  {totalRAM - usedRAM} MB", PCScreenFont.Default, Kernel.GreenPen, x, y);

            y += 50;
            canvas.DrawString("=== SYSTEM ===", PCScreenFont.Default, Kernel.YellowPen, x, y);
            y += 30;
            canvas.DrawString($"Uptime: {uptime:hh\\:mm\\:ss}", PCScreenFont.Default, Kernel.WhitePen, x, y);
            y += 25;
            canvas.DrawString($"Time: {DateTime.Now:HH:mm:ss}", PCScreenFont.Default, Kernel.WhitePen, x, y);

            y += 50;
            canvas.DrawString("=== ABOUT ===", PCScreenFont.Default, Kernel.YellowPen, x, y);
            y += 30;
            canvas.DrawString("WindOS v1.0", PCScreenFont.Default, Kernel.WhitePen, x, y);
            y += 25;
            canvas.DrawString("Built on Cosmos OS", PCScreenFont.Default, Kernel.GrayPen, x, y);
        }
    }
}
