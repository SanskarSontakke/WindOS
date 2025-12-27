using System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace WindOS.Apps
{
    // Full-screen Clock App for 640x480
    public class ClockApp : App
    {
        public ClockApp() : base("Clock") { }
        public override void Update() { }

        public override void Draw(Canvas canvas)
        {
            // Centered large clock
            int cx = Kernel.ScreenWidth / 2;
            int cy = Kernel.ScreenHeight / 2;

            canvas.DrawString("CLOCK", PCScreenFont.Default, Kernel.CyanPen, cx - 25, 20);
            canvas.DrawString(DateTime.Now.ToString("HH:mm:ss"), PCScreenFont.Default, Kernel.WhitePen, cx - 40, cy - 20);
            canvas.DrawString(DateTime.Now.ToString("dddd"), PCScreenFont.Default, Kernel.GrayPen, cx - 35, cy + 20);
            canvas.DrawString(DateTime.Now.ToString("yyyy-MM-dd"), PCScreenFont.Default, Kernel.GrayPen, cx - 50, cy + 50);
            canvas.DrawString("[ESC] Exit", PCScreenFont.Default, Kernel.GrayPen, 10, Kernel.ScreenHeight - 20);
        }
    }
}
