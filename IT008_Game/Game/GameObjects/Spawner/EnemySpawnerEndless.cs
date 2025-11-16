using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static IT008_Game.Game.GameObjects.Spawner.EnemySpawner;

namespace IT008_Game.Game.GameObjects.Spawner
{
    
    using EnemyData = Func<float,GameObject>;
    internal class EnemySpawnerEndless : EnemySpawner
    {
        public EnemySpawnerEndless(Player player) : base(player)
        {
            
        }

        protected override void BuildSpawner(Player _player)
        {
            
            var wave = new Wave([
               new EnemyData(x => {
                        var enemy = new Enemy(_player,x);
                        enemy.Sprite.Transform.Position = GetRandomPostition();
                        return enemy;
                    })
           ], 2);

            _waves = [
                wave
            ];
            _rng = new Random();
            _currentState = SpawnerState.Ready;
        }
        public override void NextWave()
        {
            if (_currentState != SpawnerState.Ready)
                return;

            if (_currentWaveIdx + 1 < _waves.Count)
            {
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

                if (SceneManager.CurrentScene is MainGameScene mg)
                    mg.ShowWave(_waveDisplayNumber);
                _waveDisplayNumber += 1;
                

                Console.WriteLine("New wave called");

                _currentState = SpawnerState.Spawning;
                Console.WriteLine("New wave caled");
            }
            else
            {
                _currentWaveIdx = -1;
                foreach ( var wave in _waves)
                {
                    wave.WaveWorth += 2;
                    wave.DifficultyLevel += 0.1f;
                }
                Console.WriteLine("Wave finished");
                NextWave();

               
            }
        }
    }
}
