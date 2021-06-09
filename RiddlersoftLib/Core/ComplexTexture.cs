using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Core
{
    public class ComplexTexture
    {
        public Rectangle? SourceRectangle;
        public float Scale;
        public Vector2 Origin;
        public SpriteEffects Effect;
        public float Depth;
        public float Rotation;
        public Color Color;
        public Texture2D Texture { get; private set; }

       public static implicit operator ComplexTexture(Texture2D texture)
        {
            return new ComplexTexture()
            {
                Texture = texture,
                Scale = 1,
                SourceRectangle = null,
                Origin = Vector2.Zero,
                Effect = SpriteEffects.None,
                Rotation = 0,
                Color = Color.White,
                Depth = 0,
            };
        }

        public void Draw(SpriteBatch sb, Vector2 pos, float fade)
        {
            sb.Draw(this.Texture, pos, this.SourceRectangle, this.Color * fade, this.Rotation, this.Origin, this.Scale, this.Effect, this.Depth);
        }

        
    }
}
