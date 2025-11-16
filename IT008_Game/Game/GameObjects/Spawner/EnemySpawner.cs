using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.Boss.Introduction;
using IT008_Game.Game.GameObjects.Boss.Secondary;
using IT008_Game.Game.GameObjects.EnemyTypes;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using IT008_Game.Game.Scenes;
using System.Collections.Generic;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Spawner
{
    using EnemyData = Func<float, GameObject>;

    internal class EnemySpawner : GameObject
    {
        public record Wave
        {
            public readonly List<EnemyData> PossibleEnemies;
            public int WaveWorth;
            public float WaveTimeBtwSpawn = 1f;
            public float DifficultyLevel = 1f;

            public Wave(List<EnemyData> possibleEnemies, int waveWorth)
            {
                PossibleEnemies = possibleEnemies;
                WaveWorth = waveWorth;
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
        protected SpawnerState _currentState;

        protected int _currentWaveIdx = -1;
        protected float _timeBtwSpawn;
        protected int _currentWaveWorth;
        protected int _waveDisplayNumber = 1;

        protected List<Enemy> _enemiesToSpawn;

        public EnemySpawner(Player player)
        {
            BuildSpawner(player);
        }

        protected virtual void BuildSpawner(Player player)
        {
            var wave1 = new Wave([
                new EnemyData(x => {
                    var enemy = new Enemy(player, x);
                    enemy.Sprite.Transform.Position = GetRandomPostition();
                    return enemy;
                }),
                new EnemyData(x => {
                    var enemy = new Enemy_Normal(player, x);
                    enemy.Sprite.Transform.Position = GetRandomPostition();
                    return enemy;
                })
            ], 10);

            var wave2 = new Wave([
                new EnemyData(x => {
                    var enemy = new Enemy(player, x);
                    enemy.Sprite.Transform.Position = GetRandomPostition();
                    return enemy;
                })
            ], 10);

            // every boss weight is 100
            var wave3 = new Wave([
                 new EnemyData(() => {
                        return new IntroductionBoss(_player);
                    })
            ], 100);

            var wave4 = new Wave([
                 new EnemyData(() => {
                        return new SecondaryBoss(_player);
                    })
            ], 100);

            // Setup wave data here
            _waves = [
                wave1,
                wave2,
                wave3,
                wave4,
            ];
            _rng = new Random();
            _currentState = SpawnerState.Ready;
        }

        public virtual void NextWave()
        {
            if (_currentState != SpawnerState.Ready)
                return;

            if (_currentWaveIdx + 1 < _waves.Count)
            {
                _currentWaveIdx++;
                _enemiesToSpawn = new List<Enemy>();

                var wave = _waves[_currentWaveIdx];
                _currentWaveWorth = wave.WaveWorth;

                while (_currentWaveWorth > 0)
                {
                    var enemyIdx = _rng.Next(wave.PossibleEnemies.Count);
                    var data = wave.PossibleEnemies[enemyIdx];

                    var enemy = data(wave.DifficultyLevel) as Enemy;
                    _enemiesToSpawn.Add(enemy);

                    _currentWaveWorth -= enemy.EnemyWeight;
                }

                _timeBtwSpawn = wave.WaveTimeBtwSpawn;
                _currentState = SpawnerState.Spawning;
                Console.WriteLine("New wave caled");

                AudioManager.PlayFightingMusic();

                // 🔥 Tell scene to show wave text
                if (SceneManager.CurrentScene is MainGameScene mg)
                    mg.ShowWave(_waveDisplayNumber);
                _waveDisplayNumber += 1;
                Console.WriteLine("New wave called");
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
            {
                return;

            if (_enemiesToSpawn.Count <= 0)
            {
                _currentState = SpawnerState.Waiting;
                return;
            }

            var wave = _waves[_currentWaveIdx];

            if (_timeBtwSpawn <= 0)
            {
                var enemy = _enemiesToSpawn[0];

                Children.Add(enemy);
                if (SceneManager.CurrentScene is MainGameScene mg)
                    mg.EnemyList.Add(enemy);

                _enemiesToSpawn.RemoveAt(0);
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
                }
            }
        }
        public override void Update()
        {
            HandleSpawning();
            TrackEnemy();
            base.Update();
        }

        public Vector2 GetRandomPostition(float margin = -50f)
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

        private float Rand(float max) => (float)(_rng.NextDouble() * max);

        
    }

}
