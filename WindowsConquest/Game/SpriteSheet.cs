using System;
using System.Collections.Generic;
using System.Drawing;

namespace WindowsConquest.Game
{
    public class SpriteSheet
    {
        private readonly Bitmap _sheet;
        private readonly Dictionary<string, Rectangle> _regions;

        public SpriteSheet(Bitmap sheet, Dictionary<string, Rectangle> regions)
        {
            _sheet = sheet;
            _regions = regions;
        }

        public Bitmap GetSprite(string name)
        {
            if (!_regions.ContainsKey(name)) return null;
            Rectangle src = _regions[name];
            return _sheet.Clone(src, _sheet.PixelFormat);
        }
        public static List<Rectangle> AutoSlice(Bitmap bmp)
        {
            var rects = new List<Rectangle>();
            bool[,] visited = new bool[bmp.Width, bmp.Height];

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (visited[x, y]) continue;

                    Color c = bmp.GetPixel(x, y);
                    if (c.A > 0) // non-transparent
                    {
                        // Flood-fill to find bounding box
                        int minX = x, minY = y, maxX = x, maxY = y;
                        var stack = new Stack<Point>();
                        stack.Push(new Point(x, y));

                        while (stack.Count > 0)
                        {
                            var p = stack.Pop();
                            if (p.X < 0 || p.Y < 0 || p.X >= bmp.Width || p.Y >= bmp.Height) continue;
                            if (visited[p.X, p.Y]) continue;

                            Color cc = bmp.GetPixel(p.X, p.Y);
                            if (cc.A == 0) continue;

                            visited[p.X, p.Y] = true;
                            minX = Math.Min(minX, p.X);
                            minY = Math.Min(minY, p.Y);
                            maxX = Math.Max(maxX, p.X);
                            maxY = Math.Max(maxY, p.Y);

                            stack.Push(new Point(p.X + 1, p.Y));
                            stack.Push(new Point(p.X - 1, p.Y));
                            stack.Push(new Point(p.X, p.Y + 1));
                            stack.Push(new Point(p.X, p.Y - 1));
                        }

                        rects.Add(Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1));
                    }
                }
            }
            return rects;
        }

    }
}
