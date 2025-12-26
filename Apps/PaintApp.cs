using System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using System.Collections.Generic;

namespace WindOS.Apps
{
    public class PaintApp : App
    {
        private Color currentColor = Color.Black;
        private int brushSize = 5;
        // We can't easily store a full bitmap for the canvas state in memory without eating RAM fast in Cosmos if we create a new Bitmap every frame.
        // A better approach for Cosmos paint is to store a list of "DrawPoints" or just rely on the fact that we clear screen every frame?
        // Wait, Cosmos Canvas clears every frame in the Kernel loop usually?
        // In this architecture, `processManager.Draw(canvas)` is called every frame.
        // If we want persistent drawing, we need to store the draw commands.

        private List<DrawPoint> points = new List<DrawPoint>();

        private struct DrawPoint
        {
            public int X;
            public int Y;
            public Color Color;
            public int Size;
        }

        public PaintApp() : base("Paint") { }

        public override void Update()
        {
            if (MouseManager.MouseState == MouseState.Left)
            {
                // Toolbar interaction
                if (MouseManager.Y < 50)
                {
                    // Palette
                    if (CheckClick(10, 10, 30, 30)) currentColor = Color.Black;
                    if (CheckClick(50, 10, 30, 30)) currentColor = Color.Red;
                    if (CheckClick(90, 10, 30, 30)) currentColor = Color.Green;
                    if (CheckClick(130, 10, 30, 30)) currentColor = Color.Blue;
                    if (CheckClick(170, 10, 30, 30)) currentColor = Color.White; // Eraser

                    // Brush Size
                    if (CheckClick(250, 10, 30, 30)) brushSize = 5;
                    if (CheckClick(290, 10, 30, 30)) brushSize = 10;
                    if (CheckClick(330, 10, 30, 30)) brushSize = 20;

                    // Clear
                    if (CheckClick(400, 10, 80, 30)) points.Clear();
                }
                else
                {
                    // Drawing
                    // Add point if mouse moved or clicked
                    // Optimization: Don't add if exactly same as last point
                    points.Add(new DrawPoint
                    {
                        X = (int)MouseManager.X,
                        Y = (int)MouseManager.Y,
                        Color = currentColor,
                        Size = brushSize
                    });
                }
            }
        }

        private bool CheckClick(int x, int y, int w, int h)
        {
            return MouseManager.X >= x && MouseManager.X <= x + w &&
                   MouseManager.Y >= y && MouseManager.Y <= y + h;
        }

        public override void Draw(Canvas canvas)
        {
            // Canvas Background
            canvas.DrawFilledRectangle(Color.White, 0, 50, 1280, 670);

            // Draw all points
            // Note: This might get slow with thousands of points.
            // A real paint app in Cosmos usually writes to an off-screen buffer/bitmap,
            // but for simplicity we re-draw the list.
            foreach (var p in points)
            {
                canvas.DrawFilledCircle(p.Color, p.X, p.Y, p.Size / 2);
            }

            // Toolbar
            canvas.DrawFilledRectangle(Color.LightGray, 0, 0, 1280, 50);

            // Colors
            canvas.DrawFilledRectangle(Color.Black, 10, 10, 30, 30);
            canvas.DrawFilledRectangle(Color.Red, 50, 10, 30, 30);
            canvas.DrawFilledRectangle(Color.Green, 90, 10, 30, 30);
            canvas.DrawFilledRectangle(Color.Blue, 130, 10, 30, 30);
            canvas.DrawFilledRectangle(Color.White, 170, 10, 30, 30); canvas.DrawRectangle(Color.Black, 170, 10, 30, 30); // Eraser border

            // Sizes
            canvas.DrawFilledCircle(Color.Black, 265, 25, 2);
            canvas.DrawFilledCircle(Color.Black, 305, 25, 5);
            canvas.DrawFilledCircle(Color.Black, 345, 25, 10);

            // Clear
            canvas.DrawFilledRectangle(Color.DarkRed, 400, 10, 80, 30);
            canvas.DrawString("Clear", PCScreenFont.Default, Color.White, 415, 18);

            // Current Indicator
            canvas.DrawString($"Color", PCScreenFont.Default, currentColor, 500, 18);
            canvas.DrawString($"Size: {brushSize}", PCScreenFont.Default, Color.Black, 580, 18);
        }
    }
}
