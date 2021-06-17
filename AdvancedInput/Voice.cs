using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace AdvancedInput
{
    public struct VoicePart
    {
        public SoundEffect Effect;
        public double Duration;

        public static implicit operator VoicePart(SoundEffect input)
        {
            return new VoicePart()
            {
                Effect = input,
                Duration = input.Duration.TotalSeconds,
            };
        }
    }

    public class Voice
    {
        private static List<SoundEffect> _voicePoint = new List<SoundEffect>();
        private static List<SoundEffect> _voiceNumber = new List<SoundEffect>();
        private static SoundEffect _voice0To60;
        private static SoundEffect _voiceNewRecord;

        public static void LoadContent(ContentManager content)
        {
            for (int i = 0; i < 10; i++)
                _voiceNumber.Add(content.Load<SoundEffect>($"Voice\\{i}"));

            for (int i = 0; i < 10; i++)
                _voicePoint.Add(content.Load<SoundEffect>($"Voice\\{i}p"));

            _voice0To60 = content.Load<SoundEffect>("Voice\\0to60");

            _voiceNewRecord = content.Load<SoundEffect>("Voice\\newrecord");
        }

        private static Queue<VoicePart> _voiceParts = new Queue<VoicePart>();
        private static float _timeTillNextEffect;
        private static bool _play;
        private static VoicePart _currentVoicePart;
        public static void Speak(float number)
        {
            _voiceParts.Clear();

            //_voiceParts.Enqueue(_voice0To60);

            int whole = (int)number;

            //add the whole number
            int dec = (int)((number - whole) * 1000);
            _voiceParts.Enqueue(_voicePoint[whole]);

            int d1, d2, d3;
            d1 = (int)(dec / 100);
            d2 = (int)((dec / 10) - (d1 * 10));
            d3 = (int)(dec - (d1 * 100 + d2 * 10));

            _voiceParts.Enqueue(_voiceNumber[d1]);
            _voiceParts.Enqueue(_voiceNumber[d2]);
            _voiceParts.Enqueue(_voiceNumber[d3]);
            _currentVoicePart = _voiceParts.Dequeue();
            _timeTillNextEffect = (float)_currentVoicePart.Duration;
            _currentVoicePart.Effect.Play();
            _play = true;
        }

        public static void Update(float dt)
        {
            if (_play)
            {
                _timeTillNextEffect -= dt;

                if (_timeTillNextEffect <= 0)
                {
                    if (_voiceParts.Count > 0)
                    {
                        _currentVoicePart = _voiceParts.Dequeue();
                        _timeTillNextEffect = (float)_currentVoicePart.Duration;
                        _currentVoicePart.Effect.Play();
                    }
                    else
                        _play = false;
                }
            }
        }
    }
}
