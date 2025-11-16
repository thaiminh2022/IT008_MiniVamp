using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.Scenes;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace IT008_Game
{
    /// <summary>
    /// This is the entry of the game
    /// </summary>
    public partial class GameForm : Form
    {
        private bool _isAdjustingSize = false;
        public GameForm()
        {
            InitializeComponent();

            // setup form options
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            // fixed game size
            ClientSize = new Size(GameManager.VirtualWidth, GameManager.VirtualHeight);
            ResizeEnd += (_, _) => KeepAspect();

            // scene setups
            GameInput.Setup(this);
            SceneManager.Setup(this);
            SceneManager.ChangeScene(MainMenuScene.Name);

            // game loop
            var timer = new Timer();

            // fps (adjust base on ur screen refresh rate
            float fps = 165f;
            timer.Interval = (int)(1f / fps * 1000f);
            timer.Tick += (_, _) =>
            {
                GameTime.Tick();
                SceneManager.SceneUpdate();
                GameInput.Tick();

                if (GameTime.TimeScale > 0)
                    Invalidate();
            };
            timer.Start();
        }

        /// <summary>
        /// Try to keep the screen aspect ratio at 16:9, might change this behavior later
        /// </summary>
        private void KeepAspect()
        {
            if (_isAdjustingSize) return;   // prevent loop
            _isAdjustingSize = true;

            // target aspect from your virtual size
            float target = GameManager.VirtualWidth / (float)GameManager.VirtualHeight;

            int cw = this.ClientSize.Width;
            int ch = this.ClientSize.Height;

            // figure out which side user mostly changed
            float current = cw / (float)ch;

            if (current > target)
            {
                // too wide → adjust width
                cw = (int)(ch * target);
            }
            else
            {
                // too tall → adjust height
                ch = (int)(cw / target);
            }

            this.ClientSize = new Size(cw, ch);

            _isAdjustingSize = false;

            GameManager.VirtualWidth = ClientSize.Width;
            GameManager.VirtualHeight = ClientSize.Height;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // we already drawing the background color
            return;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.SmoothingMode = SmoothingMode.None;
            SceneManager.SceneDraw(g);
        }
    }
}
