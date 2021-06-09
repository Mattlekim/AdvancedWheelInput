using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Riddlersoft.Graphics.Text
{
    public class TextChar
    {
        public Vector2 Position = Vector2.Zero;
        public float Rotation = 0f;
        public float RotationVelocity = 0f;
        public float LifeTime = 0f;
        public float Scale = 1f;
        public Color Colour = Color.White;
        
        public string character = string.Empty;
        public float Opacity = 1f;
        public Vector2 Velocity = Vector2.Zero;
        public Vector2 Center;

        //i want to allow textures to replace letters
        //so here all texture requred stuff
        public Texture2D Sprite;

        /// <summary>
        /// set up with a sprite
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="pos"></param>
        public TextChar(Texture2D texture, Vector2 pos, float scale = 1)
        {
            Sprite = texture;
            Position = pos + new Vector2(texture.Width * .5f, texture.Height * .3f);
            Scale = scale;
        }

        public void SetUp(SpriteFont font)
        {
            if (Sprite == null) //no texture therfore its a letter
            {
                Center = font.MeasureString(character) * .5f;
                Position += Center;
                return;
            }

            //now texture setup stuff

            Center = new Vector2(Sprite.Width * .5f, Sprite.Height * .5f);
         //   Position += Center;

        }

        /// <summary>
        /// set up with a letter
        /// </summary>
        /// <param name="c"></param>
        /// <param name="pos"></param>
        public TextChar(char c, Vector2 pos)
        {
            character = c.ToString();
            Position = pos;
        }

        public void Update(float dt)
        {
            Position += Velocity;
            Rotation += RotationVelocity;
            LifeTime += dt;
        }

    }
}
