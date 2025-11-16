using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Game.GameObjects.PlayerCharacter;

namespace IT008_Game.Game.Scenes
{
    internal class TutorialScene : GameScene
    {
        public static new string Name => "Tutorial";

        public TutorialScene()
        {
            Player player = new Player();
            Children.Add(player);

            var backBtn = new Button
            {
                Text = "Main Menu",
                Location = new Point(
                    GameManager.VirtualWidth / 2 - 200 / 2, 
                    GameManager.VirtualHeight - 100
                 ),
                Width = 200,
                Height = 50,
            };
            backBtn.Click += BackBtn_Click;

            Controls.Add(backBtn);

            AudioManager.PlayTutorialMusic();
        }

        private void BackBtn_Click(object? sender, EventArgs e)
        {
            SceneManager.ChangeScene(MainMenuScene.Name);
        }

        public override void UnLoad()
        {
            base.UnLoad();
            AudioManager.StopAllAudio();
        }

        public override void Draw(Graphics g)
        {
            g.Clear(Color.CornflowerBlue);

            var text = "WASD/Arrow keys to move\nSpace/Left Mouse to shoot\nF to dash\nObjective: Shoot the bad guys";
            using var font = new Font("Segoe UI", 14, FontStyle.Regular);
            using var br = new SolidBrush(Color.White);
            PointF position = new Point(50, GameManager.VirtualHeight / 2);

            g.DrawString(text, font, br, position);

            base.Draw(g);
        }
    }
}
