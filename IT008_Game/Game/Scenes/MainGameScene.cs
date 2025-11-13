using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects;
using IT008_Game.Game.GameObjects.Boss;
using IT008_Game.Game.GameObjects.Spawner;


namespace IT008_Game.Game.Scenes
{
    internal sealed class MainGameScene : GameScene
    {
        public static new string Name => "Game";
        TableLayoutPanel? pauseMenu;
        Player? player;

        public GameObjectList EnemyList { get; private set; } = [];
        public GameObjectList BulletList { get; private set; } = [];

        public override void Load()
        {
            // We create the player, and enemies
            player = new();
            var spawner = new EnemySpawner(player);

            Children.AddRange([
                player,
                spawner,
            ]);

            EnemyList.AddRange([
                 new Enemy()
            ]);

            spawner.NextWave();

            DrawPauseMenu();
        }

        public override void UnLoad()
        {
            // We have to clean every objects when scene exit
            foreach (var item in EnemyList)
            {
                item.Destroy();
            }
            foreach (var item in BulletList)
            {
                item.Destroy();
            }

            base.UnLoad();
        }

        private void DrawPauseMenu()
        {
            // MENU
            pauseMenu = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Visible = false,
                BackColor = Color.Transparent
            };
            pauseMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            pauseMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 80));

            pauseMenu.Controls.Add(new Label()
            {
                Text = "Paused",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18)
            }, 0, 0);

            var continueBtn = new Button();
            continueBtn.Text = "Continue";
            continueBtn.Size = new Size(300, 50);
            continueBtn.Click += (s, e) =>
            {
                ResumeGame();
                pauseMenu.Visible = false;
            };

            var mainMenuButton = new Button();
            mainMenuButton.Text = "Main Menu";
            mainMenuButton.Size = new Size(300, 50);
            mainMenuButton.Click += (s, e) =>
            {
                pauseMenu.Visible = false;
                SceneManager.ChangeScene(MainMenuScene.Name);
                ResumeGame();
            };

            var flowLayout = new FlowLayoutPanel();
            flowLayout.Anchor = AnchorStyles.None;
            flowLayout.FlowDirection = FlowDirection.TopDown;
            flowLayout.AutoSize = true;
            flowLayout.Controls.AddRange([continueBtn, mainMenuButton]);

            pauseMenu.Controls.Add(flowLayout, 0, 1);
            Controls.AddRange([
                new Label() {
                    Text = "Esc to pause/unpause\nWASD/Arrow keys to move\nX to destroy the player\nSpace to shoot some dino (check terminal)",
                    BackColor = Color.Transparent,
                    AutoSize = true
                },
                pauseMenu
            ]);
        }

        public override void Update()
        {
            // UPDATE THREAD
            if (GameInput.GetKeyDown(Keys.X))
            {
                player?.Destroy();
            }

            EnemyList.Update();
            BulletList.Update();


            // EXAMPLE BULLET COLLISION
            for (int i = 0; i < EnemyList.Count; i++)
            {
                var enemy = EnemyList[i] as Enemy;

                for (int j = 0; j < BulletList.Count; j++)
                {
                    var bullet = BulletList[j] as Bullet;
                    if (enemy.Sprite.CollidesWith(bullet.Sprite) && !bullet.WillDestroyNextFrame)
                    {
                        Console.WriteLine("hit");
                        bullet.Destroy();
                        enemy.Damage();
                    }
                }
            }

            // BASE
            base.Update();


            // UI THREAD
            if (pauseMenu is not null && GameInput.GetKeyDown(Keys.Escape))
            {
                pauseMenu.Visible = !pauseMenu.Visible;

                if (pauseMenu.Visible == false)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }

            }

        }
        public override void Draw(Graphics g)
        {
            EnemyList.Draw(g);
            BulletList.Draw(g);

            base.Draw(g);
        }

        private void PauseGame()
        {
            GameTime.TimeScale = 0;
        }

        private void ResumeGame()
        {
            GameTime.TimeScale = 1;
        }
    }
}
