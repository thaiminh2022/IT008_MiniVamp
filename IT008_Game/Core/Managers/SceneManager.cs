using IT008_Game.Core.Components;
using IT008_Game.Game.Scenes;

namespace IT008_Game.Core.Managers
{
    /// <summary>
    /// Manage scene switching
    /// </summary>
    internal static class SceneManager
    {
        static Form? _mainForm;
        public static GameScene? CurrentScene { get; private set; }
        static Dictionary<string, GameScene> _sceneData = [];

        public static void Setup(Form f)
        {
            _mainForm = f;
            _sceneData = new Dictionary<string, GameScene>{
                { MainGameScene.Name, new MainGameScene() },
                { MainMenuScene.Name, new MainMenuScene() },
            };
        }

        public static void SceneUpdate()
        {
            CurrentScene?.Update();
        }

        public static void SceneDraw(Graphics g)
        {
            CurrentScene?.Draw(g);
        }

        /// <summary>
        /// This function unload the current scene (if exists) and load the next scene
        /// </summary>
        /// <param name="sceneName">The scene it's loading, please try using the Name property of scene</param>
        /// <returns>true if sucess, false if not</returns>
        public static bool ChangeScene(string sceneName)
        {
            if (_mainForm == null) return false;

            // scene name does not exits, not loading lol
            var scene = _sceneData[sceneName];
            if (scene is null)
            {
                Console.WriteLine("Scene does not exists");
                return false;
            }


            _mainForm.Controls.Clear();
            CurrentScene?.UnLoad();

            CurrentScene = scene;
            CurrentScene.Load();

            _mainForm.Controls.AddRange([.. scene.Controls]);
            _mainForm.Text = sceneName;
            _mainForm.Focus();


            return true;
        }

        public static void RestartCurrentScene()
        {
            if (CurrentScene is null || _mainForm is null)
                return;

            _mainForm.Controls.Clear();

            CurrentScene.UnLoad();
            CurrentScene.Load();

            _mainForm.Controls.AddRange([.. CurrentScene.Controls]);
            _mainForm.Focus();
        }
    }


}
