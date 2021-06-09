using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Riddlersoft.Graphics
{
    public class Texture2DSwip
    {
        public enum TextureState { Compile, Decomplie, None}

        private TextureState State = TextureState.None;

        public TextureState CurrentState { get { return State; } }

        public class Partical
        {
            public Vector2 Position;
            public Rectangle Part;
            public float LifeTime;
        }

        public float Speed = .016f;


        private float _amount = 0;
        public Texture2D Sprite;

        
        private Texture2DSwip(Texture2D sprite)
        {
            Sprite = sprite;
            SetState(TextureState.None);
        }

        public void SetState(TextureState state)
        {
            State = state;
            if (State == TextureState.Compile)
            {
                _amount = .2f;
            }
            if (state == TextureState.Decomplie)
            {
                _amount = 0f;
            }
        }

       public static implicit operator Texture2DSwip(Texture2D sprite)
        {
            return new Texture2DSwip(sprite);
        }


        public static implicit operator Texture2D(Texture2DSwip input)
        {
            return input.Sprite;
        }
        float pixelSize = 5;
        Point lastpixels = new Point(0,0);

        List<Partical> _particals = new List<Partical>();

        float dir = 0;
        public void Draw(SpriteBatch sb, Vector2 pos, Color colour)
        {
           
            switch (State)
            {
                case TextureState.Decomplie:
                    _amount += Speed * .1f;

                    break;
                case TextureState.Compile:
                    _amount -= Speed * .1f;
                    break;
            }
                
           

            int hPixels = (int)((Sprite.Height * Sprite.Width) / pixelSize);
            int pixel = 0;
            if (State == TextureState.Compile)
                pixel = (int)((Sprite.Height * Sprite.Width * _amount) / pixelSize);
            else
                pixel = (int)((Sprite.Height * Sprite.Width * _amount) / pixelSize);

            int y = (int)(pixel / (Sprite.Width / pixelSize));
            int x = (int)(pixel - y * (Sprite.Width / pixelSize));
            x *= (int)pixelSize;
            y *= (int)pixelSize;

            if (_amount <= 0 && State == TextureState.Compile)
            {
                _amount = 0;
                State = TextureState.None;
            }

            if (y >= Sprite.Height)
            {
                State = TextureState.None;
                return;
            }

            if (new Point(x,y) != lastpixels)
            {
                if (State == TextureState.Decomplie || State == TextureState.Compile && _amount > .05f)
                {
                    Vector2 p = new Vector2(x, y);
                    if (State == TextureState.Compile)
                        p.Y -= 80;
                    _particals.Add(new Partical()
                    {
                        Position = p,
                        Part = new Rectangle(x, y, (int)pixelSize, (int)pixelSize),
                        LifeTime = .6f,
                    });
                }
            }
            lastpixels = new Point(x, y);

            sb.Draw(Sprite, pos + new Vector2(x, y), new Rectangle(x, y, Sprite.Width - x, (int)pixelSize), Color.White);

            y += (int)pixelSize;
            
            sb.Draw(Sprite, pos + new Vector2(0, y), new Rectangle(0, y, Sprite.Width, Sprite.Height - y), Color.White);

            if (State == TextureState.Compile)
                dir = 2;
            else
                if (State == TextureState.Decomplie)
                dir = -2;

            for (int i = _particals.Count - 1; i >= 0; i--)
            {
                _particals[i].Position.Y += dir;
                _particals[i].LifeTime -= Speed;
                sb.Draw(Sprite, _particals[i].Position + pos, _particals[i].Part, Color.White * (_particals[i].LifeTime * 2));
                if (_particals[i].LifeTime <= 0)
                    _particals.RemoveAt(i);
            }

        }
    }
}
