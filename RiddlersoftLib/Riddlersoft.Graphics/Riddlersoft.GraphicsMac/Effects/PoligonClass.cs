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
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            _effect.Projection = halfPixelOffset * projection;
            _effect.VertexColorEnabled = true;
        }

        public static void Begin()
        {
            _effect.CurrentTechnique.Passes[0].Apply();
        }

        public static void DrawLine(Riddlersoft.Core.Line line, Color col)
        {
            VertexPositionColor[] verts = new VertexPositionColor[]
            {
                new VertexPositionColor() { Position = new Vector3(line.Start, 0), Color =  col},
                new VertexPositionColor() { Position = new Vector3(line.End, 0), Color =  col},
            };
            _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, 1);
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color col)
        {
            VertexPositionColor[] verts = new VertexPositionColor[]
            {
                new VertexPositionColor() { Position = new Vector3(start, 0), Color =  col},
                new VertexPositionColor() { Position = new Vector3(end, 0), Color =  col},
            };
            _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, 1);
        }

        public static void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color col)
        {
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
