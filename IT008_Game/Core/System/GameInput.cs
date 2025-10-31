using System.Numerics;

namespace IT008_Game.Core.System
{
    enum Axis
    {
        Horizontal,
        Vertical,
    }

    internal static class GameInput
    {

        // 0 = left, 1 = right, 2 = middle (Unity style)
        private static bool[] _mouseCurrent = new bool[3];
        private static bool[] _mousePrev = new bool[3];

        private static HashSet<Keys> _currentKeys = [];
        private static HashSet<Keys> _previousKeys = [];

        public static Vector2 MousePosition;


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

            form.MouseMove += (_, e) =>
            {
                MousePosition.X = e.X;
                MousePosition.Y = e.Y;
            };

            form.MouseDown += (_, e) =>
            {
                var b = MouseBtnToInt(e.Button);
                if (b >= 0) _mouseCurrent[b] = true;
            };

            form.MouseUp += (_, e) =>
            {
                var b = MouseBtnToInt(e.Button);
                if (b >= 0) _mouseCurrent[b] = false;
            };

        }

        private static int MouseBtnToInt(MouseButtons buttons)
        {
            return buttons switch
            {
                MouseButtons.Left => 0,
                MouseButtons.Right => 1,
                MouseButtons.Middle => 2,
                MouseButtons.None => -1,
                _ => throw new NotImplementedException("This mouse button value is not implemented"),
            };
        }

        public static void Tick()
        {
            _previousKeys.Clear();
            foreach (Keys key in _currentKeys) {
                _previousKeys.Add(key);
            }

            for (int i = 0; i < _mouseCurrent.Length; i++)
                _mousePrev[i] = _mouseCurrent[i];
        }

        /// <summary>
        /// Check if the left/right/middle mouse button is pressed
        /// </summary>
        /// <param name="btn">The mouse button to check</param>
        /// <returns>true if pressed</returns>
        public static bool GetMouseButton(MouseButtons btn)
        {
            var b = MouseBtnToInt(btn);
            if (b < 0) return false;

            return _mouseCurrent[b];
        }

        /// <summary>
        /// Check if the left/right/middle mouse button was pressed this frame
        /// </summary>
        /// <param name="btn">The mouse button to check</param>
        /// <returns>true if pressed once this frame</returns>
        public static bool GetMouseButtonDown(MouseButtons btn)
        {
            var b = MouseBtnToInt(btn);
            if (b < 0) return false;

            return _mouseCurrent[b] && !_mousePrev[b];
        }
        /// <summary>
        /// Check if the left/right/middle mouse button was released this frame
        /// </summary>
        /// <param name="btn">The mouse button to check</param>
        /// <returns>true if released once this frame</returns>
        public static bool GetMouseButtonUp(MouseButtons btn)
        {
            var b = MouseBtnToInt(btn);
            if (b < 0) return false;

            return  !_mouseCurrent[b] && _mousePrev[b];
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
