using System;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace IT008_Game.Core.Managers
{
    internal static class AudioManager
    {
        // ========= PATH HELPERS =========

        private static string BuildAssetsPath(string relativePath)
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets/audio",
                relativePath);
        }

        private static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }

        // ========= MUSIC (single track, loopable) =========

        private static IWavePlayer? _musicOutput;
        private static AudioFileReader? _musicReader;
        private static bool _musicLoop;
        private static string _currentMusicPath;
        private static bool _musicStopping;
        private static float _musicVolume = 1.0f;   // 0..1
        private static EventHandler<StoppedEventArgs>? _musicPlaybackHandler;

        // SFX 
        static Random _rng = new Random();

        public static float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Clamp01(value);
                if (_musicReader != null)
                    _musicReader.Volume = _musicVolume;
            }
        }

        private static void PlayMusicInternal(string relativePath, bool loop = true)
        {
            // If same track is already playing, do nothing
            if (_currentMusicPath == relativePath &&
                _musicOutput != null &&
                _musicOutput.PlaybackState == PlaybackState.Playing)
            {
                return;
            }
            Console.WriteLine($"Playing music: {relativePath}");

            _currentMusicPath = relativePath;
            _musicStopping = false;

            StopMusic(); // clean previous music if any

            string fullPath = BuildAssetsPath(relativePath);

            _musicReader = new AudioFileReader(fullPath);
            _musicReader.Volume = _musicVolume;

            _musicOutput = new WaveOutEvent();
            _musicLoop = loop;

            // create handler ONCE per track and keep reference
            _musicPlaybackHandler = (sender, args) =>
            {
                // If this stop was triggered manually, do nothing
                if (_musicStopping)
                    return;

                // Natural end → loop if requested
                if (_musicLoop && _musicReader != null && _musicOutput != null)
                {
                    _musicReader.Position = 0;
                    _musicOutput.Play();
                }
            };

            _musicOutput.PlaybackStopped += _musicPlaybackHandler;
            _musicOutput.Init(_musicReader);
            _musicOutput.Play();
        }

        public static void PlayMainMenuMusic()
            => PlayMusicInternal(@"music/xDeviruchi - Title Theme.wav");

        public static void PlayFightingMusic()
            => PlayMusicInternal(@"music/xDeviruchi-And-The-Journey-Begins.wav");

        public static void PlayBossFight1Music()
            => PlayMusicInternal(@"music/xDeviruchi - Exploring The Unknown.wav");

        public static void PlayBossFight2Music()
            => PlayMusicInternal(@"music/xDeviruchi - The Icy Cave.wav");

        public static void PlayTutorialMusic() => PlayMusicInternal(@"music/xDeviruchi - Take some rest and eat some food.wav");
        public static void StopMusic()
        {
            _musicLoop = false;
            _musicStopping = true;

            if (_musicOutput != null)
            {
                if (_musicPlaybackHandler != null)
                    _musicOutput.PlaybackStopped -= _musicPlaybackHandler;

                _musicOutput.Stop();
                _musicOutput.Dispose();
                _musicOutput = null;
            }

            if (_musicReader != null)
            {
                _musicReader.Dispose();
                _musicReader = null;
            }

            _musicPlaybackHandler = null;
            _musicStopping = false; // reset for next time
        }

        // ========= SFX (mixer, many at once) =========

        private static IWavePlayer? _sfxOutput;
        private static MixingSampleProvider? _sfxMixer;
        private static float _sfxVolume = 1.0f;     // master SFX volume 0..1

        public static float SfxVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Clamp01(value);
        }

        static AudioManager()
        {
            InitSfx();
        }

        private static void InitSfx()
        {
            // Target format for ALL SFX in the mixer
            var format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

            _sfxMixer = new MixingSampleProvider(format)
            {
                ReadFully = true
            };

            _sfxOutput = new WaveOutEvent();
            _sfxOutput.Init(_sfxMixer);
            _sfxOutput.Play();
        }

        /// <summary>
        /// Ensure input matches the mixer's WaveFormat (sample rate + channels).
        /// </summary>
        private static ISampleProvider ConvertToSfxFormat(ISampleProvider input)
        {
            if (_sfxMixer == null)
                return input;

            var target = _sfxMixer.WaveFormat;
            ISampleProvider result = input;

            // 1) Resample if needed
            if (result.WaveFormat.SampleRate != target.SampleRate)
            {
                result = new WdlResamplingSampleProvider(result, target.SampleRate);
            }

            // 2) Channel conversion
            if (result.WaveFormat.Channels != target.Channels)
            {
                if (result.WaveFormat.Channels == 1 && target.Channels == 2)
                {
                    result = new MonoToStereoSampleProvider(result);
                }
                else
                {
                    // Add other conversions if you ever need them
                    throw new NotSupportedException(
                        $"Channel conversion {result.WaveFormat.Channels} -> {target.Channels} not supported.");
                }
            }

            return result;
        }

        /// <summary>
        /// Play a one-shot SFX. volume: 0..1, scaled by SfxVolume.
        /// </summary>
        public static void PlaySfx(string relativePath, float volume = 1.0f)
        {
            if (_sfxMixer == null)
                return;

            string fullPath = BuildAssetsPath(relativePath);

            var reader = new AudioFileReader(fullPath); // file's native format → float

            ISampleProvider sp = reader;
            sp = ConvertToSfxFormat(sp);               // match mixer format

            // Apply per-sound + master SFX volume
            var vol = new VolumeSampleProvider(sp)
            {
                Volume = volume * Clamp01(_sfxVolume)
            };

            _sfxMixer.AddMixerInput(vol);

            // NOTE: For a long-running game with lots of SFX,
            // you can later wrap this in an AutoDisposeSampleProvider
            // to Dispose() the reader when finished.
        }

        // Convenience wrappers for your existing sounds:

        public static void PlayShoot(float volume = .7f)
            => PlaySfx(@"sfx/shoot.wav", volume);

        public static void PlayHit(float volume = .7f)
            => PlaySfx(@"sfx/hit.wav", volume);

        public static void PlaySwordSlash(float volume = 3f)
        {
            var sfxIndex = _rng.Next(1, 7);
            var path = @$"sfx/boss/sword_slash/WHSH_MOVEMENT-Wind Sweep Swish_HY_PC-00{sfxIndex}.wav";
            PlaySfx(path, volume);
        }

        public static void PlayManaExplosion(float volume = 3f)
        {
            var sfxIndex = _rng.Next(1, 7);
            var path = @$"sfx/boss/mana_explosion/DSGNImpt_EXPLOSION-Mana Bomb_HY_PC-00{sfxIndex}.wav";
            PlaySfx(path, volume);
        }
        public static void PlaySlam(float volume = 3f)
        {
            var path = @$"sfx/boss/DSGNImpt_EXPLOSION-Bass Hit_HY_PC-006.wav";
            PlaySfx(path, volume);
        }
        public static void PlayBubble(float volume = 3f)
        {
            var path = @$"sfx/boss/MAGSpel_CAST-Sphere Up_HY_PC-001.wav";
            PlaySfx(path, volume);
        }

        public static void PlayBite(float volume = 3f)
        {
            var path = @$"sfx/boss/FGHTImpt_MELEE-Kick Block_HY_PC-004.wav";
            PlaySfx(path, volume);
        }
        public static void PlayBuff(float volume = 3f)
        {
            var path = @$"sfx/DSGNSynth_BUFF-Generic Buff_HY_PC-001";
            PlaySfx(path, volume);
        }
        public static void PlayDebuff(float volume = 3f)
        {
            var path = @$"sfx/DSGNSynth_BUFF-Failed Buff_HY_PC-003";
            PlaySfx(path, volume);
        }
        public static void PlayPlayerHit(float volume = 3f)
        {
            var path = @$"sfx/player/FGHTImpt_MELEE-Gut Kick_HY_PC-001.wav";
            PlaySfx(path, volume);
        }
        public static void PlayPlayerDash(float volume = 3f)
        {
            var path = @$"sfx/player/DSGNMisc_MOVEMENT-Bit Sweep_HY_PC-001.wav";
            PlaySfx(path, volume);
        }
        public static void PlayPlayerDashAvailable(float volume = 3f)
        {
            var path = @$"sfx/player/MAGSpel_CAST-Skill Ready_HY_PC-006.wav";
            PlaySfx(path, volume);
        }
        public static void PlayExplosion(float volume = 1f)
        {
            var path = @$"sfx/explosion.wav";
            PlaySfx(path, volume);
        }

        public static void StopAllSfx()
        {
            if (_sfxOutput != null)
            {
                _sfxOutput.Stop();
                _sfxOutput.Dispose();
                _sfxOutput = null;
            }

            _sfxMixer = null;

            // Re-init so we can play again later
            InitSfx();
        }

        public static void StopAllAudio()
        {
            StopMusic();
            StopAllSfx();
        }
    }
}
