using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;

namespace IT008_Game.Game.Scenes
{
    internal sealed class MainMenuScene : GameScene
    {
        public static new string Name = "Main Menu";

        Bitmap bgImage;

        public MainMenuScene()
        {
            bgImage = AssetsBundle.LoadImageBitmap("dino_vsurvivors_bg_1280x720.png");

            AudioManager.StopAllAudio();
            AudioManager.PlayMainMenuMusic();

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                BackgroundImage = bgImage
            };

            table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 90));

            var label = new Label()
            {
                Text = "MINI VAMP",
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                ForeColor = Color.Violet,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 24, FontStyle.Bold)
            };

            var flow = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.None,
                Anchor = AnchorStyles.None,
                AutoSize = true
            };

            var goToGameButton = new Button
            {
                Text = "Go to game",
                Size = new Size(300, 50),
                Font = new Font("Segoe UI", 15),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            goToGameButton.Click += (_, _) => SceneManager.ChangeScene(MainGameScene.Name);

            var tutorialBtn = new Button
            {
                Text = "Tutorial",
                Size = new Size(300, 50),
                Font = new Font("Segoe UI", 15),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            tutorialBtn.Click += (_, _) => SceneManager.ChangeScene(TutorialScene.Name);

            var volumeGroup = new TableLayoutPanel
            {
                Width = 300,
                Height = 100,
                ColumnCount = 2,
                RowCount = 2,
            };

            volumeGroup.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            volumeGroup.RowStyles.Add(new RowStyle(SizeType.Percent, 50));


            var musicLabel = new Label
            {
                Text = "Music",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Width = 100,
                Height = 50,
            };
            var musicVolume = new TrackBar()
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(AudioManager.MusicVolume * 100),
                Width = 200,
                Height = 50,
                TickStyle = TickStyle.None,
                SmallChange = 1,
                LargeChange = 10,
            };
            musicVolume.Scroll += (_, _) =>
            {
                float volume = musicVolume.Value / 100f;
                AudioManager.MusicVolume = volume;
            };

            volumeGroup.Controls.Add(musicLabel);
            volumeGroup.Controls.Add(musicVolume, 1, 0);

            var sfxLabel = new Label
            {
                Text = "Sound Effects",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Width = 100,
                Height = 50,
            };
            var sfxVolume = new TrackBar()
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(AudioManager.SfxVolume * 100),
                Width = 200,
                Height = 50,
                TickStyle = TickStyle.None,
                SmallChange = 1,
                LargeChange = 10,
            };
            sfxVolume.Scroll += (_, _) =>
            {
                float volume = sfxVolume.Value / 100f;
                AudioManager.SfxVolume = volume;
            };

            volumeGroup.Controls.Add(sfxLabel, 0, 1);
            volumeGroup.Controls.Add(sfxVolume, 1, 1);


            flow.Controls.AddRange([goToGameButton, tutorialBtn, volumeGroup]);


            table.Controls.Add(label, 0, 0);
            table.Controls.Add(flow, 0, 1);
            Controls.Add(table);
        }

        public override void Draw(Graphics g)
        {
            
            //g.DrawImage(bgImage, Point.Empty);
            base.Draw(g);
        }

        public override void UnLoad()
        {
            AudioManager.StopAllAudio();

            base.UnLoad();
        }
    }
}
