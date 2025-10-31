namespace IT008_Game.Core.System
{
    enum Axis
    {
        Horizontal,
        Vertical,
    }

    internal static class GameInput
    {
        
        
        private static HashSet<Keys> _currentKeys = [];
        private static HashSet<Keys> _previousKeys = [];



        public static void Setup(Form form)
        {
            form.KeyPreview = true;

            form.KeyDown += (_, e) =>
            {
                _currentKeys.Add(e.KeyData);
               
            };
            form.KeyUp += (_, e) =>
            {
                _currentKeys.Remove(e.KeyData);
            };

            form.Deactivate += (_, _) =>
            {
                _currentKeys.Clear();
            };
        }

        public static void Tick()
        {
            _previousKeys.Clear();
            foreach (Keys key in _currentKeys) {
                _previousKeys.Add(key);
            }
        }

        public static bool GetKey(Keys k) => _currentKeys.Contains(k);
        public static bool GetKeyDown(Keys k) => _currentKeys.Contains(k) && !_previousKeys.Contains(k);
        public static bool GetKeyUp(Keys k) => !_currentKeys.Contains(k) && _previousKeys.Contains(k);
        public static float GetAxis(Axis axis)
        {
            if (axis == Axis.Horizontal)
            {
                int v = 0;
                if (GetKey(Keys.Left) || GetKey(Keys.A)) v -= 1;
                if (GetKey(Keys.Right) || GetKey(Keys.D)) v += 1;
                return v;
            }

            if (axis == Axis.Vertical)
            {
                int v = 0;
                if (GetKey(Keys.Up) || GetKey(Keys.W)) v -= 1;   
                if (GetKey(Keys.Down) || GetKey(Keys.S)) v += 1;
                return v;

            }

            return 0f;
        }
    
    }
}
