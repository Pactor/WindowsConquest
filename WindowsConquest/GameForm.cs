using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsConquest.Game;

namespace WindowsConquest
{
    public partial class GameForm : Form
    {
        private int _frameCount = 0;
        private int _fps = 0;
        private DateTime _lastFpsTime = DateTime.Now;
        private bool _isDragging = false;
        private Point _lastMousePos;


        private PictureBox _pictureBox;
        private Timer _timer;
        private TileMap _map;

        private int _tileSize = 160;
        private int _cameraX = 0;
        private int _cameraY = 0;

        private const int ScrollSpeed = 128;
        private const int ZoomStep = 4;
        private const int MinTileSize = 128;
        private const int MaxTileSize = 256;
        // Key state for smooth hold-to-move
        private bool _moveLeft, _moveRight, _moveUp, _moveDown;

        public GameForm()
        {
            InitializeComponent();
            _timer = new Timer { Interval = 30 };
            _timer.Tick += (s, e) =>
            {
                EdgeScroll();       // continuous edge scroll
                ClampCamera();      // keep camera inside the map
                _pictureBox.Invalidate();
            };
            _timer.Start();
            SetupUI();
            LoadGame();
        }

        private void SetupUI()
        {
            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;

            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            _pictureBox.Paint += PictureBox_Paint;

            // 🟢 Add this line so the camera clamps correctly after resizing
            _pictureBox.Resize += (s, e) => { ClampCamera(); _pictureBox.Invalidate(); };

            Controls.Add(_pictureBox);

            _timer = new Timer { Interval = 30 };
            _timer.Tick += (s, e) =>
            {
                int step = Math.Max(24, _tileSize / 2);
                if (_moveLeft) _cameraX -= step;
                if (_moveRight) _cameraX += step;
                if (_moveUp) _cameraY -= step;
                if (_moveDown) _cameraY += step;

                ClampCamera();
                _pictureBox.Invalidate();
            };
            _timer.Start();
        }


        private void ClampCamera()
        {
            if (_map == null) return;

            int mapPixelW = _map.Width * _tileSize;
            int mapPixelH = _map.Height * _tileSize;
            int viewW = _pictureBox.ClientSize.Width;
            int viewH = _pictureBox.ClientSize.Height;

            // Clamp so the viewport always stays within map bounds
            int maxCamX = Math.Max(0, mapPixelW - viewW);
            int maxCamY = Math.Max(0, mapPixelH - viewH);

            if (_cameraX < 0) _cameraX = 0;
            else if (_cameraX > maxCamX) _cameraX = maxCamX;

            if (_cameraY < 0) _cameraY = 0;
            else if (_cameraY > maxCamY) _cameraY = maxCamY;
        }




        private void EdgeScroll()
        {
            const int edge = 20; // pixels from edge to start scrolling
            var mouse = _pictureBox.PointToClient(Cursor.Position);

            if (mouse.X >= 0 && mouse.X <= _pictureBox.Width &&
                mouse.Y >= 0 && mouse.Y <= _pictureBox.Height)
            {
                if (mouse.X < edge) _cameraX -= ScrollSpeed;
                if (mouse.X > _pictureBox.Width - edge) _cameraX += ScrollSpeed;
                if (mouse.Y < edge) _cameraY -= ScrollSpeed;
                if (mouse.Y > _pictureBox.Height - edge) _cameraY += ScrollSpeed;
            }
        }

        private void LoadGame()
        {
            string grassPath = Data.GameConfig.GetAssetFile("tiles/grass.png");
            _map = new TileMap(1000, 1000, grassPath);
            CenterCamera();
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (_map != null)
                _map.Draw(e.Graphics, _cameraX, _cameraY, _tileSize, _pictureBox.ClientRectangle);

            // FPS
            _frameCount++;
            var now = DateTime.Now;
            if ((now - _lastFpsTime).TotalSeconds >= 1)
            {
                _fps = _frameCount;
                _frameCount = 0;
                _lastFpsTime = now;
            }
            using (var font = new Font("Consolas", 10))
            using (var brush = new SolidBrush(Color.Yellow))
                e.Graphics.DrawString($"FPS: {_fps}", font, brush, 5, 5);
        }


        private void CenterCamera()
        {
            if (_map == null) return;

            int mapPixelW = _map.Width * _tileSize;
            int mapPixelH = _map.Height * _tileSize;

            int viewW = _pictureBox.ClientSize.Width;
            int viewH = _pictureBox.ClientSize.Height;

            _cameraX = Math.Max(0, (mapPixelW - viewW) / 2);
            _cameraY = Math.Max(0, (mapPixelH - viewH) / 2);
        }
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left: _moveLeft = true; break;
                case Keys.Right: _moveRight = true; break;
                case Keys.Up: _moveUp = true; break;
                case Keys.Down: _moveDown = true; break;

                // Zoom with keyboard
                case Keys.Oemplus: // '+'
                case Keys.Add:
                    if (_tileSize < MaxTileSize) _tileSize += 8;
                    ClampCamera();
                    _pictureBox.Invalidate();
                    break;

                case Keys.OemMinus: // '-'
                case Keys.Subtract:
                    if (_tileSize > MinTileSize) _tileSize -= 8;
                    ClampCamera();
                    _pictureBox.Invalidate();
                    break;
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left: _moveLeft = false; break;
                case Keys.Right: _moveRight = false; break;
                case Keys.Up: _moveUp = false; break;
                case Keys.Down: _moveDown = false; break;
            }
        }

    }
}
