using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Riddlersoft.Graphics.Effects
{
    public static class Poligons
    {
        private static BasicEffect _effect;
        private static Game _game;
        public static void Setup(Game game)
        {
            _game = game;
            _effect = new BasicEffect(game.GraphicsDevice);
            _effect.World = Matrix.Identity;
            _effect.View = Matrix.Identity;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            game.GraphicsDevice.RasterizerState = rs;
            Viewport viewport = game.GraphicsDevice.Viewport;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, 1280, 720, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            _effect.Projection = halfPixelOffset * projection;
            _effect.VertexColorEnabled = true;
        }

        public static void SetTexture(ref Texture2D tex)
        {
            _effect.Texture = tex;
            _effect.CurrentTechnique.Passes[0].Apply();
        }

        public static void Begin(bool textureEnabled = false)
        {
            _effect.TextureEnabled = textureEnabled;
            _effect.CurrentTechnique.Passes[0].Apply();
            MinPoint = null;
        }

        public static void DrawLine(Riddlersoft.Core.Line line, Color col)
        {
            _effect.TextureEnabled = false;
            _effect.VertexColorEnabled = true;
            _effect.CurrentTechnique.Passes[0].Apply();
            VertexPositionColor[] verts = new VertexPositionColor[]
            {
                new VertexPositionColor() { Position = new Vector3(line.Start, 0), Color =  col},
                new VertexPositionColor() { Position = new Vector3(line.End, 0), Color =  col},
            };
            _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, 1);
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color col)
        {
            _effect.TextureEnabled = false;
            _effect.CurrentTechnique.Passes[0].Apply();
            VertexPositionColor[] verts = new VertexPositionColor[]
            {
                new VertexPositionColor() { Position = new Vector3(start, 0), Color =  col},
                new VertexPositionColor() { Position = new Vector3(end, 0), Color =  col},
            };
            _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, 1);
        }

        private static float Min(float a, float b, float c)
        {
            if (a <= b && a <= c)
                return a;

            if (b <= a && b <= c)
                return b;

            return c;
        }

        private static float Max(float a, float b, float c)
        {
            if (a > b && a > c)
                return a;

            if (b > a && b > c)
                return b;

            return c;
        }

        private static float _scale;
        private static Vector2 _min;
        public static Vector2 CalculateTextureCoordinate(Vector2 pos, Vector2 origin, float width, float heigh)
        {
            pos.X = pos.X - _min.X + origin.X * _scale;
            pos.Y = pos.Y - _min.Y + origin.Y * _scale;

            pos.X = pos.X / (width * _scale);
            pos.Y = pos.Y / (heigh * _scale);
            return pos;
        }

        public static Vector2? MinPoint;

        public static void SetUpGpuForRendering()
        {
            _effect.CurrentTechnique.Passes[0].Apply();
        }

        public static void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color col, Texture2D texture, Vector2 origin, float scale)
        {
            _effect.TextureEnabled = true;
            _effect.Texture = texture;
            _effect.CurrentTechnique.Passes[0].Apply();
            _scale = scale;

            if (MinPoint == null)
                _min = new Vector2(Min(p1.X, p2.X, p3.X), Min(p1.Y, p2.Y, p3.Y));
            else
                _min = MinPoint.Value;

            float width = texture.Width;
            float heigh = texture.Height;

            //_effect.CurrentTechnique.Passes[0].Apply();
            VertexPositionColorTexture[] verts = new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture() { Position = new Vector3(p1, 0), Color =  col, TextureCoordinate = CalculateTextureCoordinate(p1, origin, width, heigh)},
                new VertexPositionColorTexture() { Position = new Vector3(p2, 0), Color =  col, TextureCoordinate = CalculateTextureCoordinate(p2, origin, width, heigh)},
                new VertexPositionColorTexture() { Position = new Vector3(p3, 0), Color =  col, TextureCoordinate = CalculateTextureCoordinate(p3, origin, width, heigh)},
            };
            _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, verts, 0, 1);
        }



        public static void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color col)
        {
            _effect.TextureEnabled = false;
            _effect.CurrentTechnique.Passes[0].Apply();
            //_effect.Texture = null;
            VertexPositionColor[] verts = new VertexPositionColor[]
            {
                new VertexPositionColor() { Position = new Vector3(p1, 0), Color =  col},
                new VertexPositionColor() { Position = new Vector3(p2, 0), Color =  col},
                new VertexPositionColor() { Position = new Vector3(p3, 0), Color =  col},
            };
            _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, verts, 0, 1);
        }
    }
}
