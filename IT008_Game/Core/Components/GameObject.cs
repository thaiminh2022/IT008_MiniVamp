using IT008_Game.Core.System;

namespace IT008_Game.Core.Components
{
    internal abstract class GameObject : IDisposable
    {
        public bool WillDestroyNextFrame { get; private set; } = false;

        public GameObjectList Children { get; private set; } = [];
        public GameObjectList? Parent { get; set; }

        public virtual void Update()
        {
            Children.Update();
        }
        public virtual void Draw(Graphics g)
        {
            Children.Draw(g);
        }

        public virtual void OnDestroy() { }
        public void Destroy()
        {
            if (WillDestroyNextFrame) return;
            WillDestroyNextFrame = true;

            foreach (var child in Children)
            {
                child.Destroy();
            }

        }
        public void Dispose()
        {
            OnDestroy();
        }
    }
}
