using IT008_Game.Core.System;

namespace IT008_Game.Core.Components
{
    internal class AnimatedSprite2D : Sprite2D
    {
        public readonly Dictionary<string, Animation2D> Animations;
        public Animation2D? CurrentAnimation { get; private set; }
        public Animation2D? NextAnimation { get; private set; }

        public int CurrentFrame { get; private set; } = 0;

        private float _tickTime = 0;
        private bool _isPlaying = false;

        public AnimatedSprite2D(Dictionary<string, Animation2D> animations) : base(null)
        {
            Transform = new();
            Animations = animations;
        }
        public AnimatedSprite2D() : this([]) { }

        public void Play(string animName)
        {
            // Console.WriteLine("Animation queue changed");

            if (CurrentAnimation != Animations[animName])
                NextAnimation = Animations[animName];
        }

        /// <summary>
        /// Check if current animation finished
        /// </summary>
        /// <returns>true if current animation finished</returns>
        public bool AnimationFinished()
        {
            if (CurrentAnimation == null)
                return false;

            return CurrentFrame + 1 == CurrentAnimation.TotalFrame
                    && !CurrentAnimation.Config.Loop;
        }

        public void Stop(bool resetFrame = false)
        {
            _isPlaying = false;

            if (resetFrame)
                CurrentFrame = 0;
        }

        public Animation2D AddAnimation(string imagePath, string name, AnimationConfig config)
        {
            var image = AssetsBundle.LoadImageBitmap(imagePath);
            var anim = new Animation2D(image, config);
            Animations.Add(name, anim);
            return anim;
        }

        public void Update()
        {
            // Doing this so animation changed on next frame
            if (NextAnimation is not null)
            {
                CurrentAnimation = NextAnimation;
                Texture = CurrentAnimation.Texture;
                CurrentFrame = 0;
                _tickTime = CurrentAnimation.GetSecondsPerFrame();
                _isPlaying = true;
                NextAnimation = null;
            }


            if (CurrentAnimation is null || !_isPlaying)
                return;

            Region = CurrentAnimation.GetFrameRect(CurrentFrame);

            if (_tickTime <= 0)
            {
                CurrentFrame++;
                if (CurrentFrame + 1 == CurrentAnimation.TotalFrame
                    && !CurrentAnimation.Config.Loop
                )
                {
                    Stop();
                    return;
                }

                CurrentFrame %= CurrentAnimation.TotalFrame;
                _tickTime = CurrentAnimation.GetSecondsPerFrame();
            }
            else
            {
                _tickTime -= 1 * GameTime.DeltaTime * CurrentAnimation.Config.Speed;
            }
        }
    }
}
