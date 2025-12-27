using System;
using Cosmos.System.Graphics;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using System.Collections.Generic;
using System.Drawing;

namespace WindOS.Apps
{
    // Full-screen Paint for 640x480
    public class PaintApp : App
    {
        private List<DrawPoint> points = new List<DrawPoint>();
        private Color currentColor = Color.White;
        private int brushSize = 4;

        private struct DrawPoint { public int X, Y, Size; public Color C; }

        private static readonly Color[] palette = { Color.White, Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Black };

        public PaintApp() : base("Paint") { }

        public override void Update()
        {
            if (MouseManager.MouseState == MouseState.Left)
            {
                int my = (int)MouseManager.Y;

                // Toolbar (top 30px)
                if (my < 30)
                {
                    int mx = (int)MouseManager.X;
                    // Color palette
                    for (int i = 0; i < palette.Length; i++)
                    {
                        if (mx >= 10 + i * 35 && mx < 40 + i * 35) currentColor = palette[i];
                    }
                    // Brush sizes
                    if (mx >= 250 && mx < 280) brushSize = 2;
                    if (mx >= 290 && mx < 320) brushSize = 6;
                    if (mx >= 330 && mx < 360) brushSize = 12;
                    // Clear
                    if (mx >= 400 && mx < 480) { points.Clear(); while (MouseManager.MouseState == MouseState.Left) { } }
                }
                else
                {
                    // Drawing
                    points.Add(new DrawPoint { X = (int)MouseManager.X, Y = my, Size = brushSize, C = currentColor });
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            // Canvas area (below toolbar)
            canvas.DrawFilledRectangle(Kernel.BlackPen, 0, 30, Kernel.ScreenWidth, Kernel.ScreenHeight - 30);

            // Draw all points
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                canvas.DrawFilledCircle(new Pen(p.C), p.X, p.Y, p.Size);
            }

            // Toolbar
            canvas.DrawFilledRectangle(Kernel.GrayPen, 0, 0, Kernel.ScreenWidth, 30);

            // Color palette
            for (int i = 0; i < palette.Length; i++)
            {
                canvas.DrawFilledRectangle(new Pen(palette[i]), 10 + i * 35, 3, 25, 24);
            }

            // Brush sizes
            canvas.DrawFilledCircle(Kernel.WhitePen, 265, 15, 2);
            canvas.DrawFilledCircle(Kernel.WhitePen, 305, 15, 6);
            canvas.DrawFilledCircle(Kernel.WhitePen, 345, 15, 10);

            // Clear button
            canvas.DrawFilledRectangle(Kernel.RedPen, 400, 3, 80, 24);
            canvas.DrawString("Clear", PCScreenFont.Default, Kernel.WhitePen, 420, 8);

            // ESC hint
            canvas.DrawString("[ESC]", PCScreenFont.Default, Kernel.BlackPen, Kernel.ScreenWidth - 50, 8);
        }
    }
}
