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
        public static string? CurrentSceneName { get; private set; }

        static Dictionary<string, Func<GameScene>> _sceneData = [];

        public static void Setup(Form f)
        {
            _mainForm = f;
            _sceneData = new(){
                { MainGameScene.Name, () => new MainGameScene() },
                { MainMenuScene.Name, () => new MainMenuScene() },
                { TutorialScene.Name, () => new TutorialScene() },
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
            var sceneFunc = _sceneData[sceneName];
            if (sceneFunc is null)
            {
                Console.WriteLine("Scene does not exists");
                return false;
            }


            _mainForm.Controls.Clear();
            CurrentScene?.UnLoad();

            var scene = sceneFunc();
            CurrentScene = scene;
            CurrentSceneName = sceneName;
            CurrentScene.Load();

            _mainForm.Controls.AddRange([.. scene.Controls]);
            _mainForm.Text = sceneName;
            _mainForm.Focus();
            _mainForm.Invalidate();


            return true;
        }

        public static void RestartCurrentScene()
        {
            if (CurrentScene is null || _mainForm is null || CurrentSceneName is null)
                return;

            ChangeScene(CurrentSceneName);
        }
    }


}
