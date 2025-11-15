using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.EnemyTypes;
using IT008_Game.Game.Scenes;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.Xml;



namespace IT008_Game.Game.GameObjects.Spawner
{
    using static IT008_Game.Game.GameObjects.Spawner.EnemySpawner;
    using EnemyData = Func<float,GameObject>;
    internal class EnemySpawner : GameObject
    {
        /// <summary>
        /// Each wave have a worth, each CurrentWaitingEnemy have a cost.
        /// When spawn an CurrentWaitingEnemy: worth - cost, 
        /// </summary>
        public record Wave
        {
            // Replace this with CurrentWaitingEnemy base class later
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
            /// <summary>
            /// Spawner waiting to be called next wave
            /// </summary>
            Ready,

            /// <summary>
            /// Spawner spawing CurrentWaitingEnemy
            /// </summary>
            Spawning,

            /// <summary>
            /// Spawner waiting for player to finish killing CurrentWaitingEnemy
            /// </summary>
            Waiting,

            /// <summary>
            /// This happens when there are no more waves
            /// </summary>
            Finished,
        }


        protected List<Wave> _waves;
        protected Random _rng;
        protected SpawnerState _currentState;

        protected int _currentWaveIdx = -1;
        protected float _timeBtwSpawn;
        protected int _currentWaveWorth;

        protected List<Enemy> _enemiesToSpawn;


        public EnemySpawner(Player _player)
        {
            SpawnerInternalBuilder(_player);
        }

        protected virtual void SpawnerInternalBuilder(Player _player)
        {
            var wave1 = new Wave([
                new EnemyData(x => {
                    var enemy = new Enemy(_player,x);
                    enemy.Sprite.Transform.Position = GetRandomPostition();
                    return enemy;
                }),
                new EnemyData(x => {
                    var enemy = new Enemy_Normal(_player,x);
                    enemy.Sprite.Transform.Position = GetRandomPostition();
                    return enemy;
                })
            ], 10);

            var wave2 = new Wave([
                new EnemyData(x => {
                        var enemy = new Enemy(_player,x);
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

        public virtual void NextWave()
        {
            if (_currentState != SpawnerState.Ready)
                return;

            if (_currentWaveIdx + 1 < _waves.Count) {
                _currentWaveIdx++;
                _enemiesToSpawn = new List<Enemy>();

                _currentWaveWorth = _waves[_currentWaveIdx].WaveWorth;

                while (_currentWaveWorth > 0)
                {
                    var wave = _waves[_currentWaveIdx];
                    var enemyIdx = _rng.Next(wave.PossibleEnemies.Count);
                    var data = wave.PossibleEnemies[enemyIdx];

                    _enemiesToSpawn.Add(data(wave.DifficultyLevel) as Enemy);
                    _currentWaveWorth -= _enemiesToSpawn.Last().EnemyWeight;
                    _timeBtwSpawn = wave.WaveTimeBtwSpawn;
                }

                _currentState = SpawnerState.Spawning;
                Console.WriteLine("New wave caled");
            }
            else
            {
                _currentState = SpawnerState.Finished;
                Console.WriteLine("Wave finished");
            }
        }

        private Sprite2D Crosshair = new Sprite2D(
             AssetsBundle.LoadImageBitmap("dino.png")
        );
       
        void HandleSpawning()
        {
            if (_currentState != SpawnerState.Spawning) {
                return;
            }

            if (_enemiesToSpawn is null || _enemiesToSpawn.Count <= 0)
            {
                _currentState = SpawnerState.Waiting;
                Console.WriteLine("Finished Spawning wave, waiting for player to clear enemies");
                return;
            }
            var enemy = _enemiesToSpawn[0];
            var wave = _waves[_currentWaveIdx];

            if (_timeBtwSpawn <= 0)
            {    
                Children.Add(enemy);
                if (SceneManager.CurrentScene is MainGameScene mg)
                {
                   mg.EnemyList.Add(enemy);

                }

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

        public Vector2 GetRandomPostition(float margin = -30f)
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
