using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Riddlersoft.Graphics.Particals.Modifyers;

namespace Riddlersoft.Graphics.Particals.Emitters
{
    public abstract class Emitter : EmitterInterface
    {
        protected static Random _random = new Random();

        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public List<Partical> Particals;
        private float _releaseAmount = 1;
        public float ReleaseAmount { get { return _releaseAmount; } set { _releaseAmount = MathHelper.Clamp(value, .01f, 1000); } }
        
        public FloatRange Initial_LifeTime { get; set; }
        public ColourRange Initial_Colour { get; set; }
        public Vector2Range Initial_Velocity { get; set; }
        public FloatRange Initial_Rotaiton { get; set; }
        public FloatRange Initial_Angulor_Velocity { get; set; }
        public FloatRange Initial_Scale { get; set; }

        public Texture2D ParticalTexture { get; private set; }
        protected Vector2 ParticalCenter { get; private set; }

        public string TexturePath { get; set; } = string.Empty;

        public int MaxParticles = 1000;

        public List<Modifyer> Modifyers;

        public void Clear()
        {
            Particals.Clear();
        }

        public void SetTexture(Texture2D texture, string path = "")
        {
            ParticalTexture = texture;
            ParticalCenter = new Vector2(
                texture.Width * .5f, texture.Height * .5f);
            TexturePath = path;
        }

        public void AddModifyer(Modifyer modifyer)
        {
            Modifyers.Add(modifyer);
        }

        protected Emitter()
        {
            Particals = new List<Partical>();
            Initial_Colour = new ColourRange(Color.White, Color.White);
            Initial_LifeTime = new FloatRange(1, 1);
            Initial_Velocity = new Vector2Range(new Vector2(5,0), new Vector2(5,0));
            Initial_Rotaiton = new FloatRange(0, 0);
            Initial_Angulor_Velocity = new FloatRange(0, 0);
            Initial_Scale = new FloatRange(1, 1);

            Modifyers = new List<Modifyer>();
        }

        private float triggerTimer = 0;
        public void Trigger(Point position, int amount)
        {
            if (!Enabled)
                return;

            if (_releaseAmount < 1)
            {
                triggerTimer += _releaseAmount;
                if (triggerTimer >= 1)
                {
                    triggerTimer -= 1;
                    if (Particals.Count < MaxParticles)
                        TriggerParticals(position.ToVector2(), amount);
                }
                return;
            }
            if (Particals.Count < MaxParticles)
                TriggerParticals(position.ToVector2(), amount * (int)ReleaseAmount);
        }

        public void Trigger(Vector2 position, int amount)
        {
            if (!Enabled)
                return;

            if (_releaseAmount < 1)
            {
                triggerTimer += _releaseAmount;
                if (triggerTimer >= 1)
                {
                    triggerTimer -= 1;
                    if (Particals.Count < MaxParticles)
                        TriggerParticals(position, amount);
                }
                return;
            }
            if (Particals.Count < MaxParticles)
                TriggerParticals(position, amount * (int)ReleaseAmount);
        }

        /// <summary>
        /// generate a partical
        /// </summary>
        /// <param name="Position">the position to generate at</param>
        /// <returns></returns>
        protected Partical GenerateParticalProperties(Vector2 position)
        {
            float scale = Initial_Scale.GetValue((float)_random.NextDouble());
            return new Partical()
            {
                Position = position,
                Colour = Initial_Colour.GetValue((float)_random.NextDouble()),
                LifeTime = Initial_LifeTime.GetValue((float)_random.NextDouble()),
                AngulorRotation = Initial_Angulor_Velocity.GetValue((float)_random.NextDouble()),
                Rotation = Initial_Rotaiton.GetValue((float)_random.NextDouble()),
                Velocity = Initial_Velocity.GetValue((float)_random.NextDouble(), (float)_random.NextDouble()),
                InititalScale = scale,
                Scale = scale,
                Fade = 1,
            };
        }

        protected abstract void TriggerParticals(Vector2 position, int amount);

        public void Update(float dt)
        {
            foreach(Partical p in Particals)
            {
                p.Position += p.Velocity;
                p.Rotation += p.AngulorRotation;
                p.LifeTime -= dt;
                p.Age += dt;
            }

            for (int i = Particals.Count - 1; i >= 0; i--)
                if (Particals[i].LifeTime < 0)
                    Particals.RemoveAt(i);
                else
                    foreach (Modifyer mod in Modifyers)
                        mod.Processes(Particals[i], dt);
        }

        public void Render(SpriteBatch sb)
        {
            foreach (Partical p in Particals)
            {
                sb.Draw(ParticalTexture, p.Position, null, p.Colour * p.Fade, p.Rotation, ParticalCenter, p.Scale, 
                    SpriteEffects.None, 0f);
            }
        }

        public void Render(SpriteBatch sb, Vector2 camera)
        {
            foreach (Partical p in Particals)
            {
                sb.Draw(ParticalTexture, p.Position - camera, null, p.Colour * p.Fade, p.Rotation, ParticalCenter, p.Scale,
                    SpriteEffects.None, 0f);
            }

        }
    }
}
