using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using IT008_Game.Game.GameObjects.Spawner;


namespace IT008_Game.Game.Scenes
{
    internal sealed class MainGameScene : GameScene
    {
        public static new string Name => "Game";
        TableLayoutPanel? pauseMenu;
        Player? player;
        public EnemySpawner? enemySpawner;

        public GameObjectList EnemyList { get; private set; } = [];
        public GameObjectList BulletList { get; private set; } = [];

        public GameObjectList EnemyBulletList { get; private set; } = [];

        public override void Load()
        {
            // We create the player, and enemies
            player = new();
            enemySpawner = new EnemySpawner(player);


            Children.AddRange([
                player,
                enemySpawner,
            ]);

            //EnemyList.AddRange([
            //     new Enemy(player)
            //]);

            enemySpawner.NextWave();

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
            foreach (var item in EnemyBulletList)
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
            Controls.Add(pauseMenu);
        }


        public override void Update()
        {
            // UPDATE THREAD
            if (GameInput.GetKeyDown(Keys.X))
            {
                player?.Destroy();
            }

            //anything to do with spawning waves
            enemySpawner.Update();


            EnemyList.Update();
            BulletList.Update();
            EnemyBulletList.Update();


            // ENEMY ON BULLET COLLISION
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
                        enemy.Damaged();
                    }
                }
            }
            // PLAYER ON BULLET COLLISION
            for (int j = 0; j < EnemyBulletList.Count; j++)
            {
                var bullet = EnemyBulletList[j] as Bullet;
                if (player.Sprite.CollidesWith(bullet.Sprite) && !bullet.WillDestroyNextFrame)
                {
                    Console.WriteLine("player got hit");
                    bullet.Destroy();
                    //player.Damaged();
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
            EnemyBulletList.Draw(g);

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
