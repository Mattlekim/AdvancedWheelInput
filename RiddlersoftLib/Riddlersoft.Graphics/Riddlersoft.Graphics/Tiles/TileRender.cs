using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Riddlersoft.Graphics.Config;

namespace Riddlersoft.Graphics.Tiles
{
    public class TileRender
    {
        public static Texture2D Texture;

        public static Dictionary<TextureResolution, int> ResolutionLookup;

        public static TextureResolution CurrentTextureResultion;

        private static string[] _texturePaths = new string[4];

        public static void SetTextures(string small, string medium, string large, string superLarge)
        {
            _texturePaths[0] = small;
            _texturePaths[1] = medium;
            _texturePaths[2] = large;
            _texturePaths[3] = superLarge;

            ResolutionLookup = new Dictionary<TextureResolution, int>();
            ResolutionLookup.Add(TextureResolution._720, 80);
            ResolutionLookup.Add(TextureResolution._1080,160);
            ResolutionLookup.Add(TextureResolution._2k, 160);
            ResolutionLookup.Add(TextureResolution._4k, 240);
        }

        public static void UpdateTextureTexture(TextureResolution tr, ContentManager content)
        {
            CurrentTextureResultion = tr;
            if (_texturePaths[(int)tr] == null)
                throw new NullReferenceException("this texture has no path");

            try
            {
                Texture = content.Load<Texture2D>(_texturePaths[(int)tr]);
            }
            catch
            {
                throw new Exception("File not found");
            }
        }

        public Point RenderLocation;

        List<Point> _sources;
        List<Rectangle> _framesSource;
        List<Rectangle> _framesRender;
        private float _frameDelay;
        private float _CurrentFrameTime;
        private TileAnimaionFunction _function = TileAnimaionFunction.Stop;
        private bool _animate = false;
        private bool _renderWhenNotAnimating = true;
        private bool _forward = true;
        private int _currentFrame;

        public void UpdateTextureResolution(Vector2 renderPos, int tileRenderSize)
        {
            renderPos *= tileRenderSize;
            int tileSourceSize = ResolutionLookup[CurrentTextureResultion];
            for (int i = 0; i < _sources.Count; i++)
            {
                _framesSource.Add(new Rectangle(_sources[i].X * tileSourceSize, _sources[i].Y * tileSourceSize, tileSourceSize, tileSourceSize));
                _framesRender.Add(new Rectangle(Convert.ToInt32(renderPos.X), Convert.ToInt32(renderPos.Y), tileRenderSize, tileRenderSize));
            }
        }

        public TileRender(List<Point> sourceTexturePosition, Vector2 renderPos,  float framedelay, bool animate, bool forward = true, TileAnimaionFunction function = TileAnimaionFunction.Stop)
        {
            if (sourceTexturePosition == null)
                throw new ArgumentNullException();

            _framesSource = new List<Rectangle>();
            _framesRender = new List<Rectangle>();

            _sources = new List<Point>();
            _sources.AddRange(sourceTexturePosition);

            _frameDelay = framedelay;
            _animate = animate;
            _forward = forward;
            _function = function;
        }
        public void Update(float dt)
        {
            if (_framesSource.Count == 1)
                return;

            _CurrentFrameTime += dt;
            if (_CurrentFrameTime > _frameDelay)
            {
                _CurrentFrameTime -= _frameDelay;

                if (_forward)
                    _currentFrame++;
                else
                    _CurrentFrameTime--;

                if (_currentFrame >= _framesSource.Count) //reach end
                {
                    if (_function == TileAnimaionFunction.Stop)
                    {
                        _currentFrame--;
                        _animate = false;
                        return;
                    }
                    if (_function == TileAnimaionFunction.Loop)
                    {
                        _currentFrame = 0;
                        return;
                    }
                    _currentFrame = _framesSource.Count - 1;
                }
            }
        }

        public void Render(ref SpriteBatch sb)
        {
            sb.Draw(Texture, _framesRender[_currentFrame], _framesSource[_currentFrame], Color.White);
        }
    }
}
