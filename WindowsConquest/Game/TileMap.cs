using System.Drawing;

namespace WindowsConquest.Game
{
    public class TileMap
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Image _grass;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public TileMap(int width, int height, string grassPath)
        {
            _width = width;
            _height = height;
            _grass = Image.FromFile(grassPath);
        }

        public void Draw(Graphics g, int camX, int camY, int tileSize, Rectangle viewport)
        {
            // -------- 1) Fill the entire viewport with grass using modulo tiling
            int firstTileX = -(camX % tileSize);
            if (firstTileX > 0) firstTileX -= tileSize; // ensure it starts <= 0
            int firstTileY = -(camY % tileSize);
            if (firstTileY > 0) firstTileY -= tileSize;

            int startCol = camX / tileSize;
            int startRow = camY / tileSize;

            for (int y = firstTileY, r = startRow; y < viewport.Height; y += tileSize, r++)
            {
                for (int x = firstTileX, c = startCol; x < viewport.Width; x += tileSize, c++)
                {
                    g.DrawImage(_grass, new Rectangle(x, y, tileSize, tileSize));
                }
            }

            // -------- 2) Draw real-map overlays (coords) only inside bounds
            using (var font = new Font("Consolas", 10))
            using (var brush = new SolidBrush(Color.White))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int y = firstTileY, r = startRow; y < viewport.Height; y += tileSize, r++)
                {
                    for (int x = firstTileX, c = startCol; x < viewport.Width; x += tileSize, c++)
                    {
                        if (c >= 0 && r >= 0 && c < _width && r < _height)
                        {
                            g.DrawString($"{c},{r}", font, brush, new RectangleF(x, y, tileSize, tileSize), format);
                        }
                    }
                }
            }

            // -------- 3) Red border showing true map bounds
            int mapPixelW = _width * tileSize;
            int mapPixelH = _height * tileSize;
            int borderX = -camX;
            int borderY = -camY;
            using (var pen = new Pen(Color.Red, 2))
            {
                g.DrawRectangle(pen, borderX, borderY, mapPixelW, mapPixelH);
            }
        }

    }
}
