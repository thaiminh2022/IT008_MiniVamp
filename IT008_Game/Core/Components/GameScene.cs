using IT008_Game.Core.System;

namespace IT008_Game.Core.Components
{
    /// <summary>
    /// The base class for all the game scene
    /// </summary>
    internal abstract class GameScene
    {
        public static string Name => throw new NotImplementedException();
        public readonly List<Control> Controls = [];

        public GameObjectList Children = [];

        public abstract void Load();
        public virtual void UnLoad()
        {
            foreach (var child in Children)
            {
                child.Destroy();
            }
            Controls.Clear();
        }

        public virtual void Update()
        {
            Children.Update();
        }
        public virtual void Draw(Graphics g)
        {
            Children.Draw(g);
        }

    }

}
