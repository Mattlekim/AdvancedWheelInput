using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Riddlersoft.Graphics
{
    public enum FadeState { None, In, Out}

    public static class ScreenFade
    {
        private static Texture2D _blackDot;
        private static int _width;
        private static int _height;

        private static float _fade = 0;

        public static FadeState CurrentState { get { return _state; } }
        private static FadeState _state = FadeState.None;

        private static Action _onComplete;
        public static void SetUp(int screenWidth, int screenHeight, GraphicsDevice device)
        {
            _blackDot = new Texture2D(device, 1, 1);
            _blackDot.SetData<Color>(new Color[1] { Color.White });
            _width = screenWidth;
            _height = screenHeight;
        }

        public static void Update(float dt)
        {
            if (_state == FadeState.None)
                return;

            if (_state == FadeState.In)
            {
                _fade += dt;
                if (_fade >= 1)
                {
                    _fade = 1;
                    _state = FadeState.None;
                    if (_onComplete != null)
                        _onComplete();
                }
            }

            if (_state == FadeState.Out)
            {
                _fade -= dt;
                if (_fade <= 0)
                {
                    _fade = 0;
                    _state = FadeState.None;
                    if (_onComplete != null)
                        _onComplete();
                }
            }
        }

        public static void Clear()
        {
            _fade = 0;
            _state = FadeState.None;
        }

        public static void FadeIn(Action onCompleate)
        {
            _onComplete = onCompleate;
            _fade = 0;
            _state = FadeState.In;
        }

        public static void FadeOut(Action onCompleate)
        {
            _onComplete = onCompleate;
            _fade = 1;
            _state = FadeState.Out;
        }

        public static void Draw(SpriteBatch sb)
        {
            sb.Draw(_blackDot, new Rectangle(0, 0, _width, _height), Color.Black * _fade);
        }

    }
}
