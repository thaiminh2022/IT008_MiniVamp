using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Game.GameObjects.Boss.Introduction;
using IT008_Game.Game.GameObjects.Boss.Secondary;
using IT008_Game.Game.GameObjects.EnemyTypes;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using IT008_Game.Game.Scenes;

namespace IT008_Game.Game.GameObjects.Spawner
{
    
    internal class EnemySpawnerEndless : EnemySpawner
    {
        public EnemySpawnerEndless(Player player) : base(player) { }

        protected override void BuildSpawner(Player _player)
        {
            GameObject SpawnEnemy(float x)
            {
                var enemy = new Enemy(_player, x);
                enemy.Sprite.Transform.Position = GetRandomPostition();
                return enemy;
            }

            GameObject SpawnShotgun(float x)
            {
                var enemy = new Enemy_Shotgun(_player, x);
                enemy.Sprite.Transform.Position = GetRandomPostition();
                return enemy;
            }
            GameObject SpawnShotgunPosion(float x)
            {
                var enemy = new Enemy_ShotgunPoison(_player, x);
                enemy.Sprite.Transform.Position = GetRandomPostition();
                return enemy;
            }
            GameObject SpawnEnemyShooterCircle(float x)
            {
                var enemy = new Enemy_ShooterCircle(_player, x);
                enemy.Sprite.Transform.Position = GetRandomPosInsideScreen(100);
                return enemy;
            }

            GameObject SpawnYamaguchi(float _)
            {
                var enemy = new IntroductionBoss(_player);
                return enemy;
            }
            GameObject SpawnDiddy(float _)
            {
                var diddy = new SecondaryBoss(_player);
                return diddy;
            }


            // boss always 100 coins

            var wave1 = new Wave([SpawnEnemy], 25);
            var wave2 = new Wave([SpawnEnemy, SpawnShotgun], 35);
            var wave3 = new Wave([SpawnEnemy, SpawnShotgun, SpawnEnemyShooterCircle], 45, 2f);
            var wave4 = new Wave([SpawnEnemy, SpawnShotgun, SpawnEnemyShooterCircle], 55, 1f);
            var bosswave1 = new Wave([SpawnYamaguchi], 100);
            var wave6 = new Wave([SpawnShotgunPosion, SpawnShotgun, SpawnEnemyShooterCircle,SpawnEnemy], 65, 1f);
            var wave7 = new Wave([SpawnShotgun, SpawnEnemyShooterCircle, SpawnEnemy], 75, 1.5f);
            var wave8 = new Wave([SpawnShotgunPosion, SpawnShotgun, SpawnEnemyShooterCircle, SpawnEnemy], 90, 1.5f);
            var wave9 = new Wave([SpawnShotgunPosion, SpawnShotgun, SpawnEnemyShooterCircle, SpawnEnemy], 105, 2f);
            var bosswave = new Wave([SpawnDiddy], 100);


            _waves = [
                wave1,
                wave2,
                wave3,
                wave4 ,
                bosswave1 ,
                wave6 ,
                wave7 ,
                wave8 ,
                wave9 ,
                bosswave ,
            ];

            _currentState = SpawnerState.Ready;
        }
        public override void NextWave()
        {
            if (_currentState != SpawnerState.Ready)
                return;

            if (_currentWaveIdx + 1 < _waves.Count)
            {
                _currentWaveIdx++;
                _currentWaveWorth = _waves[_currentWaveIdx].WaveWorth;
                _timeBtwSpawn = _waves[_currentWaveIdx].WaveTimeBtwSpawn;
                
                if (SceneManager.CurrentScene is MainGameScene mg)
                    mg.ShowWave(_waveDisplayNumber);

                _waveDisplayNumber += 1;

                _currentState = SpawnerState.Spawning;
                Console.WriteLine("New wave caled");
            }
            else
            {
                // LOL loop lại wtf
                _currentWaveIdx = -1;
                foreach (var wave in _waves)
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
