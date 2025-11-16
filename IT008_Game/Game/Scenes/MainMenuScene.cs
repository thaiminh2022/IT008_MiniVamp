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
                Text = "THE GAME",
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18)
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

            flow.Controls.Add(goToGameButton);


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
