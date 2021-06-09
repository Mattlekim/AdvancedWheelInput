using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Core
{
    public static class ConsoleGui
    {
        private static List<string> _lines;
        private static List<float> _lineLifeTime;
        private static SpriteFont _font;
        private static Vector2 _drawLocation;

        public static Color TextColor { get; set; } = Color.White;
        public static float LineDisplayDuration { get; set; } = 3f;
        public static float LineSpaceing { get; set; } = 25;
        public static float TextScale = .5f;


        public static bool IsIntialized { get; private set; } = false;
        public static void Intalize(Vector2 location, SpriteFont font)
        {
            _drawLocation = location;
            _font = font;
            _lines = new List<string>();
            _lineLifeTime = new List<float>();
            IsIntialized = true;
        }

        public static void WriteLine(string text)
        {
            _lines.Add(text);
            _lineLifeTime.Add(LineDisplayDuration);
        }

        public static void Update(float dt)
        {
            for (int i = _lineLifeTime.Count - 1; i >= 0; i--)
            {
                _lineLifeTime[i] -= dt;
                if (_lineLifeTime[i] < 0)
                {
                    _lines.RemoveAt(i);
                    _lineLifeTime.RemoveAt(i);
                }
            }
        }

        public static void Draw(SpriteBatch sb)
        {
            Color col = TextColor;
            for (int i = 0; i < _lines.Count; i++)
            {
                float fade = MathHelper.Clamp(_lineLifeTime[i], 0, 1);
                if (_lines[i].Length <= 0)
                    continue;

                if (_lines[i][0] == '[' && _lines[i][2] == ']')
                {
                    switch (_lines[i][1])
                    {
                        case 'r':
                            col = Color.Red;
                            break;

                        case 'g':
                            col = Color.Green;
                            break;

                        case 'y':
                            col = Color.Yellow;
                            break;
                    }

                    float drawpos = _drawLocation.X - _font.MeasureString(_lines[i].Substring(3)).X * TextScale;
                    sb.DrawString(_font, _lines[i].Substring(3),new Vector2(drawpos, _drawLocation.Y + LineSpaceing * i),
                   col * fade, 0f, Vector2.Zero, TextScale,
                   SpriteEffects.None, 0f);
                    continue;
                }
                
                sb.DrawString(_font, _lines[i], new Vector2(_drawLocation.X - _font.MeasureString(_lines[i]).X * TextScale, _drawLocation.Y + LineSpaceing * i),
                    col * fade, 0f, Vector2.Zero, TextScale,
                    SpriteEffects.None, 0f);
            }
        }
    }
}
