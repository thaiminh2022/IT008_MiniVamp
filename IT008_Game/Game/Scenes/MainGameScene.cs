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
        
        TableLayoutPanel pauseMenu;
        TableLayoutPanel upgradeMenu;
        TableLayoutPanel lostMenu;
        Button? upBtn1, upBtn2, upBtn3;

        Player player;
        public EnemySpawnerEndless enemySpawner;

        public GameObjectList EnemyList { get; private set; } = [];
        public GameObjectList BulletList { get; private set; } = [];

        public GameObjectList EnemyBulletList { get; private set; } = [];

        public MainGameScene()
        {
            // We create the player, and enemies
            player = new();

            player.LevelSystem.LevelUp += LevelSystem_LevelUp;
            enemySpawner = new EnemySpawnerEndless(player);

            enemySpawner.OnWaveEnd += EnemySpawner_OnWaveEnd;


            Children.AddRange([
                player,
                enemySpawner,
            ]);


            pauseMenu = DrawPauseMenu();
            upgradeMenu = DrawUpgradeMenu();
            lostMenu = DrawLostMenu();
            AudioManager.PlayFightingMusic();


            enemySpawner.NextWave();
        }

        private void EnemySpawner_OnWaveEnd(object? sender, EventArgs e)
        {
            player.LevelSystem.AddLevel();
        }

        private void LevelSystem_LevelUp(object? sender, EventArgs e)
        {
            if (upgradeMenu is null) return;
            upgradeMenu.Visible = true;
            var upgrades = player?.LevelSystem?.GetUpgrades();

            if (upgrades is not null)
            {
                SetUpgrade(upBtn1, upgrades[0]);
                SetUpgrade(upBtn2, upgrades[1]);
                SetUpgrade(upBtn3, upgrades[2]);
                GameTime.TimeScale = 0;
            }
        }
        private void SetUpgrade(Button? btn, PlayerUpgrade up)
        {
            if (btn is null) return;

            var sign = up.Strat switch
            {
                UpgradeType.Addition => "+",
                UpgradeType.AddPercentage => "+%",
                UpgradeType.SubPercentage => "-%",
                UpgradeType.Subtraction => "-",
                _ => throw new NotImplementedException()
            };

            btn.Text = $"{up.Option} {sign} {up.Value}";
        
        }

        public override void UnLoad()
        {
            enemySpawner.OnWaveEnd -= EnemySpawner_OnWaveEnd;
            player.LevelSystem.LevelUp -= LevelSystem_LevelUp;
            upBtn1.Click -= SelectUpgrade1;
            upBtn2.Click -= SelectUpgrade3;
            upBtn3.Click -= SelectUpgrade3;


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
            AudioManager.StopAllAudio();

            base.UnLoad();
        }

        private TableLayoutPanel DrawPauseMenu()
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

            return pauseMenu;
        }

        private TableLayoutPanel DrawUpgradeMenu()
        {
            var width = 600;
            var height = 200;

            upgradeMenu = new TableLayoutPanel()
            {
                Anchor = AnchorStyles.None,
                Location = new Point(
                    GameManager.VirtualWidth / 2 - width / 2,
                    GameManager.VirtualHeight / 2 - height / 2
                ),
                BackColor = Color.Transparent,
                Width = width,
                Height = height,
            };

            upgradeMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            upgradeMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            upgradeMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            upgradeMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            upgradeMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));

             upBtn1 = new Button()
            {
                Dock = DockStyle.Fill,
            };
            upBtn2 = new Button()
            {
                Dock = DockStyle.Fill,
            };
            upBtn3 = new Button()
            {
                Dock = DockStyle.Fill,
            };

            upBtn1.Click += SelectUpgrade1;
            upBtn2.Click += SelectUpgrade2;
            upBtn3.Click += SelectUpgrade3;

            upgradeMenu.Controls.Add(upBtn1, 0, 1);
            upgradeMenu.Controls.Add(upBtn2, 1, 1);
            upgradeMenu.Controls.Add(upBtn3, 2, 1);

            var title = new Label
            {
                Text = "Upgrade",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 20),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White
            };
            upgradeMenu.Controls.Add(title, 0, 0);
            upgradeMenu.SetColumnSpan(title, 3);

            Controls.Add(upgradeMenu);
            upgradeMenu.Visible = false;

            return upgradeMenu;
        }

        private TableLayoutPanel DrawLostMenu()
        {
            // MENU
            lostMenu = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Visible = false,
                BackColor = Color.Transparent,

            };
            lostMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            lostMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            lostMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 70));

            lostMenu.Controls.Add(new Label()
            {
                Text = "GAMEOVER",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18)
            }, 0, 0);

            lostMenu.Controls.Add(new Label()
            {
                Text = $"SCORE: {player.LevelSystem.Level}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12)
            }, 0, 1);


            var continueBtn = new Button();
            continueBtn.Text = "Restart";
            continueBtn.Size = new Size(300, 50);
            continueBtn.Click += (s, e) =>
            {
                ResumeGame();
                lostMenu.Visible = false;
                SceneManager.RestartCurrentScene();
            };

            var mainMenuButton = new Button();
            mainMenuButton.Text = "Main Menu";
            mainMenuButton.Size = new Size(300, 50);
            mainMenuButton.Click += (s, e) =>
            {
                lostMenu.Visible = false;
                ResumeGame();

                SceneManager.ChangeScene(MainMenuScene.Name);
            };

            var flowLayout = new FlowLayoutPanel();
            flowLayout.Anchor = AnchorStyles.None;
            flowLayout.FlowDirection = FlowDirection.TopDown;
            flowLayout.AutoSize = true;
            flowLayout.Controls.AddRange([continueBtn, mainMenuButton]);

            lostMenu.Controls.Add(flowLayout, 0, 2);
            Controls.Add(lostMenu);
            lostMenu.Visible = false;

            return lostMenu;
        }

        private void SelectUpgrade3(object? sender, EventArgs e)
        {
            if (upgradeMenu is null) return;
            upgradeMenu.Visible = false;
           
            player?.LevelSystem.SelectUpgrade(2);
            GameTime.TimeScale = 1;
        }

        private void SelectUpgrade2(object? sender, EventArgs e)
        {
            if (upgradeMenu is null) return;
            upgradeMenu.Visible = false;
            player?.LevelSystem.SelectUpgrade(1);
            GameTime.TimeScale = 1;
        }

        private void SelectUpgrade1(object? sender, EventArgs e)
        {
            if (upgradeMenu is null) return;
            upgradeMenu.Visible = false;
            player?.LevelSystem.SelectUpgrade(0);
            GameTime.TimeScale = 1;
        }

        public override void Update()
        {

            EnemyList.Update();
            BulletList.Update();
            EnemyBulletList.Update();


            // ENEMY ON BULLET COLLISION
            for (int i = 0; i < EnemyList.Count; i++)
            {
                var enemy = EnemyList[i] as IEnemy;
                var enemySprite = enemy.GetSprite();

                for (int j = 0; j < BulletList.Count; j++)
                {
                    var bullet = BulletList[j] as Bullet;
                    if (enemySprite.CollidesWith(bullet.Sprite) && !bullet.WillDestroyNextFrame)
                    {
                        Console.WriteLine("hit");
                        bullet.Destroy();
                        enemy.Damage(player.Damage);
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
                    player.HealthSystem.SubstractValue(5f);
                }
            }


            // BASE
            base.Update();


            // UI THREAD
            if (waveTextTimer > 0)
            {
                waveTextTimer -= GameTime.DeltaTime;
            }

            if (pauseMenu is not null && GameInput.GetKeyDown(Keys.Escape))
            {
                pauseMenu.Visible = !pauseMenu.Visible;

                if (pauseMenu.Visible == false)
                {
                    ResumeGame();
                }

            }

            if ((pauseMenu?.Visible ?? false) || (upgradeMenu?.Visible ?? false) || (lostMenu?.Visible ?? false))
            {
                GameTime.TimeScale = 0;
            }


        }
        public void ShowGameOver()
        {
            lostMenu.Visible = true;
        }

        public override void Draw(Graphics g)
        {
            g.Clear(Color.CornflowerBlue);


            EnemyList.Draw(g);

            BulletList.Draw(g);
            EnemyBulletList.Draw(g);
            base.Draw(g);

            if (waveTextTimer > 0)
            {
                var font = new Font("Segoe UI", 36, FontStyle.Bold);
                var size = g.MeasureString(waveText, font);

                g.DrawString(
                    waveText,
                    font,
                    Brushes.White,
                    (GameManager.VirtualWidth - size.Width) / 2,
                    50
                );
            }
        }


        private void ResumeGame()
        {
            GameTime.TimeScale = 1;
        }

        private string waveText = "";
        private float waveTextTimer = 0f;
        private float waveTextDuration = 2f;

        public void ShowWave(int wave)
        {
            waveText = $"WAVE {wave}";
            waveTextTimer = waveTextDuration;
        }
       
    }
}
