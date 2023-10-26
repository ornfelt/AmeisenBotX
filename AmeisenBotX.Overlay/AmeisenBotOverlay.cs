using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Overlay
{
    public class AmeisenBotOverlay
    {
        /// <summary>
        /// Initializes a new instance of the AmeisenBotOverlay class with the specified main window handle.
        /// </summary>
        public AmeisenBotOverlay(IntPtr mainWindowHandle)
        {
            LinesToRender = new();
            RectanglesToRender = new();

            OverlayWindow = new(mainWindowHandle)
            {
                IsTopmost = true,
                IsVisible = true,
                FPS = 30
            };

            OverlayWindow.Create();

            Gfx = new(OverlayWindow.Handle, OverlayWindow.Width, OverlayWindow.Height);
            Gfx.Setup();
        }

        /// <summary>
        /// Gets or sets the Graphics object used for drawing.
        /// </summary>
        public Graphics Gfx { get; }

        /// <summary>
        /// Gets or sets the StickyWindow that overlays the current window.
        /// </summary>
        public StickyWindow OverlayWindow { get; }

        /// <summary>
        /// Gets the list of lines to be rendered, where each line is defined by a SolidBrush
        /// and a pair of Points representing the starting and ending points of the line.
        /// </summary>
        private List<(SolidBrush, (Point, Point))> LinesToRender { get; }

        /// <summary>
        /// Gets the list of tuples containing solid brushes and the coordinates of the points
        /// forming the rectangle, which are to be rendered.
        /// </summary>
        private List<(SolidBrush, (Point, Point))> RectanglesToRender { get; }

        /// <summary>
        /// Adds a line to the list of lines to render.
        /// </summary>
        /// <param name="x1">The x-coordinate of the starting point of the line.</param>
        /// <param name="y1">The y-coordinate of the starting point of the line.</param>
        /// <param name="x2">The x-coordinate of the ending point of the line.</param>
        /// <param name="y2">The y-coordinate of the ending point of the line.</param>
        /// <param name="color">The color of the line.</param>
        public void AddLine(int x1, int y1, int x2, int y2, System.Drawing.Color color)
        {
            (SolidBrush, (Point, Point)) rectangle = (Gfx.CreateSolidBrush(color.R, color.G, color.B, color.A), (new Point(x1, y1), new Point(x2, y2)));

            if (!LinesToRender.Contains(rectangle))
            {
                LinesToRender.Add(rectangle);
            }
        }

        /// <summary>
        /// Adds a rectangle to the list of rectangles to render if it does not already exist.
        /// </summary>
        public void AddRectangle(int x, int y, int w, int h, System.Drawing.Color color)
        {
            (SolidBrush, (Point, Point)) line = (Gfx.CreateSolidBrush(color.R, color.G, color.B, color.A), (new Point(x, y), new Point(x + w, y + h)));

            if (!RectanglesToRender.Contains(line))
            {
                RectanglesToRender.Add(line);
            }
        }

        /// <summary>
        /// Clears the lines to render and calls the Draw() method.
        /// </summary>
        public void Clear()
        {
            if (LinesToRender.Count > 0)
            {
                LinesToRender.Clear();
            }

            Draw();
        }

        /// <summary>
        /// Draws the lines and rectangles on the overlay window.
        /// </summary>
        public void Draw()
        {
            Gfx.Resize(OverlayWindow.Width, OverlayWindow.Height);

            if (Gfx.IsInitialized)
            {
                Gfx.BeginScene();
                Gfx.ClearScene();

                for (int i = 0; i < LinesToRender.Count; ++i)
                {
                    Gfx.DrawLine(LinesToRender[i].Item1, new(LinesToRender[i].Item2.Item1, LinesToRender[i].Item2.Item2), 2f);
                }

                for (int i = 0; i < RectanglesToRender.Count; ++i)
                {
                    Gfx.FillRectangle(RectanglesToRender[i].Item1, RectanglesToRender[i].Item2.Item1.X, RectanglesToRender[i].Item2.Item1.Y, RectanglesToRender[i].Item2.Item2.X, RectanglesToRender[i].Item2.Item2.Y);
                }

                LinesToRender.Clear();
                RectanglesToRender.Clear();

                Gfx.EndScene();
            }
        }

        /// <summary>
        /// Disposes of the graphics object and the overlay window, and waits for the overlay window thread to finish.
        /// </summary>
        public void Exit()
        {
            Gfx.Dispose();
            OverlayWindow.Dispose();
            OverlayWindow.Join();
        }
    }
}