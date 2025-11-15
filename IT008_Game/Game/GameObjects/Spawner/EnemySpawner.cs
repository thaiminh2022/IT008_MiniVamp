using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.EnemyTypes;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using IT008_Game.Game.Scenes;
using System.Numerics;



namespace IT008_Game.Game.GameObjects.Spawner
{
    using EnemyData = Func<GameObject>;
    internal class EnemySpawner : GameObject
    {
        /// <summary>
        /// Each wave have a worth, each enemy have a cost.
        /// When spawn an enemy: worth - cost, 
        /// </summary>
        public record Wave
        {
            // Replace this with enemy base class later
            public readonly List<EnemyData> PossibleEnemies;
            public readonly int WaveWorth;
            public float WaveTimeBtwSpawn = 1f;

            public Wave(List<EnemyData> possibleEnemies, int waveWorth)
            {
                PossibleEnemies = possibleEnemies;
                WaveWorth = waveWorth;
            }
        }

        enum SpawnerState
        {
            /// <summary>
            /// Spawner waiting to be called next wave
            /// </summary>
            Ready,

            /// <summary>
            /// Spawner spawing enemy
            /// </summary>
            Spawning,

            /// <summary>
            /// Spawner waiting for player to finish killing enemy
            /// </summary>
            Waiting,

            /// <summary>
            /// This happens when there are no more waves
            /// </summary>
            Finished,
        }


        List<Wave> _waves;
        Random _rng;
        SpawnerState _currentState;
        int _currentWaveIdx = -1;

        float _timeBtwSpawn;
        int _currentWaveWorth;

        public EnemySpawner(Player _player)
        {
            var wave1 = new Wave([
                new EnemyData(() => {
                    var enemy = new Enemy(_player);
                    enemy.Sprite.Transform.Position = GetRandomPostition();
                    return enemy;
                }),
                new EnemyData(() => {
                    var enemy = new Enemy_Normal(_player);
                    enemy.Sprite.Transform.Position = GetRandomPostition();
                    return enemy;
                })
            ], 10);

            var wave2 = new Wave([
                new EnemyData(() => {
                        var enemy = new Enemy(_player);
                        enemy.Sprite.Transform.Position = GetRandomPostition();
                        return enemy;
                    })
            ], 10);

            // Setup wave data here
            _waves = [
                wave1,
                wave2
            ];
            _rng = new Random();
            _currentState = SpawnerState.Ready;
        }

        public void NextWave()
        {
            if (_currentState != SpawnerState.Ready)
                return;

            if (_currentWaveIdx + 1 < _waves.Count) {
                _currentWaveIdx++;

                _currentWaveWorth = _waves[_currentWaveIdx].WaveWorth;

                _currentState = SpawnerState.Spawning;
                Console.WriteLine("New wave caled");
            }
            else
            {
                _currentState = SpawnerState.Finished;
                Console.WriteLine("Wave finished");
            }
        }

        void HandleSpawning()
        {
            if (_currentState != SpawnerState.Spawning) {
                return;
            }

            if (_timeBtwSpawn <= 0)
            {
                if (_currentWaveWorth <= 0)
                {
                    _currentState = SpawnerState.Waiting;
                    return;
                }

                var wave = _waves[_currentWaveIdx];
                var enemyIdx = _rng.Next(wave.PossibleEnemies.Count);
                var data  = wave.PossibleEnemies[enemyIdx];

                var enemy = data() as Enemy;
                _currentWaveWorth -= enemy.EnemyWeight;

                Children.Add(enemy);
                if (SceneManager.CurrentScene is MainGameScene mg)
                {
                    mg.EnemyList.Add(enemy);

                }
                _timeBtwSpawn = wave.WaveTimeBtwSpawn;
            }else
            {
                _timeBtwSpawn -= GameTime.DeltaTime;
            }
        }
        public void TrackEnemy()
        {
            if (_currentState != SpawnerState.Waiting)
                return;

            if (Children.Count == 0)
            {
                _currentState = SpawnerState.Ready;
                NextWave();
            }
        }
        public override void Update()
        {
            HandleSpawning();
            TrackEnemy();

            base.Update();
        }

        public Vector2 GetRandomPostition(float margin = 100f)
        {
            var width = GameManager.VirtualWidth;
            var height = GameManager.VirtualHeight;

            int side = _rng.Next(4);

            switch (side) {
                case 0: // Left
                {
                    var x = 0f - margin;
                    var y = (float)(_rng.NextDouble() * height);
                    return new Vector2(x, y);
                }
                case 1: // Right
                {
                    var x = width + margin;
                    var y = (float)(_rng.NextDouble() * height);
                    return new Vector2(x, y);
                }
                case 2: // Top
                {
                    var x = (float)(_rng.NextDouble() * width);
                    var y = 0 - margin;
                    return new Vector2(x, y);
                }
                case 3: // Bottom
                {
                    var x = (float)(_rng.NextDouble() * width);
                    var y = height + margin;
                    return new Vector2(x, y);
                }
                default:
                    throw new NotImplementedException();
            }

        }
    }
}
