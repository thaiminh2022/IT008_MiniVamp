using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using IT008_Game.Game.Scenes;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Spawner
{
    using EnemyData = Func<float, GameObject>;

    internal class EnemySpawner : GameObject
    {
        public event EventHandler? OnWaveEnd;

        public record Wave
        {
            public readonly List<EnemyData> PossibleEnemies;
            public int WaveWorth;
            public float WaveTimeBtwSpawn = 1f;
            public float DifficultyLevel = 1f;

            public Wave(List<EnemyData> possibleEnemies, int waveWorth, float timeBtwSpawn = 1f)
            {
                PossibleEnemies = possibleEnemies;
                WaveWorth = waveWorth;
                WaveTimeBtwSpawn = timeBtwSpawn;
            }
        }

        protected enum SpawnerState
        {
            Ready,
            Spawning,
            Waiting,
            Finished
        }

        protected List<Wave> _waves;
        protected Random _rng;
        protected SpawnerState _currentState = SpawnerState.Ready;

        protected int _currentWaveIdx = -1;
        protected float _timeBtwSpawn;
        protected int _currentWaveWorth;


        protected int _waveDisplayNumber = 0;


        public EnemySpawner(Player player)
        {
            _waves = [];
            _rng = new Random();
            BuildSpawner(player);
        }

        protected virtual void BuildSpawner(Player player)
        {
            // Setup wave data here
            _currentState = SpawnerState.Ready;
        }

        public virtual void NextWave()
        {
            if (_currentState != SpawnerState.Ready)
                return;

            if (_currentWaveIdx + 1 < _waves.Count)
            {
                _currentWaveIdx++;

                var wave = _waves[_currentWaveIdx];
                _currentWaveWorth = wave.WaveWorth;

                _timeBtwSpawn = wave.WaveTimeBtwSpawn;

                AudioManager.PlayFightingMusic();

                if (SceneManager.CurrentScene is MainGameScene mg)
                    mg.ShowWave(_waveDisplayNumber);

                _waveDisplayNumber += 1;
                Console.WriteLine("New wave called");

                _currentState = SpawnerState.Spawning;
            }
            else
            {
                _currentState = SpawnerState.Finished;
                Console.WriteLine("All waves finished.");
            }
        }

        void HandleSpawning()
        {
            if (_currentState != SpawnerState.Spawning)
                return;


            if (_currentWaveWorth <= 0)
            {
                _currentState = SpawnerState.Waiting;
                return;
            }


            if (_timeBtwSpawn <= 0)
            {
                var wave = _waves[_currentWaveIdx];
                var enemyIdx = _rng.Next(wave.PossibleEnemies.Count);
                var data = wave.PossibleEnemies[enemyIdx];
                var enemy = data(wave.DifficultyLevel);

                if (enemy is IEnemy enemyData)
                {
                    _currentWaveWorth -= enemyData.GetWeight();
                }

                if (SceneManager.CurrentScene is MainGameScene mg)
                    mg.EnemyList.Add(enemy);

                _timeBtwSpawn = wave.WaveTimeBtwSpawn;
            }
            else
            {
                _timeBtwSpawn -= GameTime.DeltaTime;
            }
        }

        public void TrackEnemy()
        {
            if (_currentState != SpawnerState.Waiting)
                return;

            if (SceneManager.CurrentScene is MainGameScene mg)
            {
                if (mg.EnemyList.Count == 0)
                {
                    _currentState = SpawnerState.Ready;
                    NextWave();
                    OnWaveEnd?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public override void Update()
        {
            HandleSpawning();
            TrackEnemy();
            base.Update();
        }

        public Vector2 GetRandomPostition(float margin = 50f)
        {
            var width = GameManager.VirtualWidth;
            var height = GameManager.VirtualHeight;

            int side = _rng.Next(4);

            return side switch
            {
                0 => new Vector2(0 - margin, Rand(height)),
                1 => new Vector2(width + margin, Rand(height)),
                2 => new Vector2(Rand(width), 0 - margin),
                3 => new Vector2(Rand(width), height + margin),
                _ => throw new NotImplementedException(),
            };
        }

        public Vector2 GetRandomPosInsideScreen(int margin = 50)
        {
            int width = GameManager.VirtualWidth;
            int height = GameManager.VirtualHeight;

            var x = _rng.Next(margin, width - margin);
            var y = _rng.Next(margin, height - margin);

            return new Vector2(x, y);
        }

        private float Rand(float max) => (float)(_rng.NextDouble() * max);
        
    }

}
