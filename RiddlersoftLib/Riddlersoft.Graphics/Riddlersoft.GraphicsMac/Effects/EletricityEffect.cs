using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Riddlersoft.Graphics.Effects;

using Microsoft.Xna.Framework.Input;

namespace Riddlersoft.Graphics.Effects
{
    public class EletricityEffect
    {
        private const float SparkFrequency = .05f;
        private const int NumberSparkPoints = 5;
        private const float SparkPointDeviation = .5f;

        public const float MaxAirSparkLenght = 80;
        public const int NumberOfAirSparks = 3;
        public const float SparkGap = 80;

        private List<Spark> _sparks = new List<Spark>();

        private List<Conduit> _powerSource = new List<Conduit>();

        private float _sparkTimer = 0;
        private Texture2D _sprite;
        private Texture2D _spriteGlow;

        public Color Colour = new Color(188, 0, 153);

        public void Clear()
        {
            _powerSource.Clear();
            _sparks.Clear();
        }
        public void AddConduit(Conduit conduit)
        {
            _powerSource.Add(conduit);
        }

        public void AddConduit(Vector2 pos, bool powered)
        {
            _powerSource.Add(new Conduit() { Position = pos, Powered = powered, SparkGap = SparkGap});
        }

        public EletricityEffect(Game game)
        {
           
            Poligons.Setup(game);
            _sprite = game.Content.Load<Texture2D>("sparkline");
            _spriteGlow = game.Content.Load<Texture2D>("sparklineglow");
        }
        private Random _rd = new Random();
        private void GenerateSparks()
        {
            Vector2 vec;
            float dir;
            float lenght;
          

            for (int sources = 0; sources < _powerSource.Count; sources++)
                if (_powerSource[sources].Active)
                {
                    _powerSource[sources].Active = false;
                    if (_powerSource[sources].Powered || _powerSource[sources].HaveRecivedPower)
                    {

                        bool connected = false;
                        if (!_powerSource[sources].HaveRecivedPower)
                        {
                            for (int target = 0; target < _powerSource.Count; target++)

                                if (sources != target)
                                {
                                    if (MathHelper.Distance(_powerSource[sources].Position.X, _powerSource[target].Position.X) <= _powerSource[sources].SparkGap && MathHelper.Distance(_powerSource[sources].Position.Y, _powerSource[target].Position.Y) <= _powerSource[sources].SparkGap)
                                    //     if (Vector2.DistanceSquared(_powerSource[sources].Position, _powerSource[target].Position) <= _powerSource[sources].SparkGap + (_rd.NextDouble() * 20))
                                    {
                                        if (!_powerSource[target].Powered)
                                        {
                                            _powerSource[target].HaveRecivedPower = true;
                                            _powerSource[target].PowerReciveAmount = _powerSource[sources].AmountOfPower;
                                        }
                                        // for (int pass = 0; pass < 2; pass++)
                                        {
                                            Vector2 v = _powerSource[sources].Position;
                                            for (int i = 0; i < NumberSparkPoints - 1; i++)
                                            {
                                                vec = _powerSource[target].Position - _powerSource[sources].Position;
                                                dir = (float)Math.Atan2(vec.Y, vec.X) + SparkPointDeviation * ((float)_rd.NextDouble() * 2 - 1);
                                                lenght = vec.Length() / NumberSparkPoints;
                                                _sparks.Add(new Spark() { Start = v, Direction = dir, Lenght = lenght, Sacle = 1f, });
                                                Vector2 newV = Vector2.Transform(new Vector2(lenght, 0), Matrix.CreateRotationZ(dir));
                                                //_sparks.Add(v + newV);
                                                v += newV;

                                            }
                                            //_sparks.Add(v);
                                            vec = _powerSource[target].Position - v;
                                            dir = (float)Math.Atan2(vec.Y, vec.X);
                                            lenght = vec.Length();
                                            _sparks.Add(new Spark() { Start = v, Direction = dir, Lenght = lenght, Sacle = 1f, });
                                        }
                                        connected = true;
                                    }


                                }
                        }
                  
                        if (!connected || _powerSource[sources].ForceAirSparks) //if we are not connected we want to create random sparks through the air
                        {

                            for (int p = 0; p < _powerSource[sources].NumberOfAirSparks; p++)
                            {
                                Vector2 v = _powerSource[sources].Position;
                                lenght = (float)_rd.NextDouble() * _powerSource[sources].MaxAirSparkLenght;
                                dir = (float)_rd.NextDouble() * MathHelper.TwoPi;
                                Vector2 t = Vector2.Transform(new Vector2(lenght, 0), Matrix.CreateRotationZ(dir)) + v;
                                for (int i = 0; i < NumberSparkPoints - 1; i++)
                                {
                                    vec = t - v;
                                    dir = (float)Math.Atan2(vec.Y, vec.X) + SparkPointDeviation * ((float)_rd.NextDouble() * 2 - 1);
                                    lenght = vec.Length() / NumberSparkPoints;
                                    _sparks.Add(new Spark() { Start = v, Direction = dir, Lenght = lenght, Sacle = 1f - (float)i / NumberOfAirSparks, });
                                    Vector2 newV = Vector2.Transform(new Vector2(lenght, 0), Matrix.CreateRotationZ(dir));
                                    //_sparks.Add(v + newV);
                                    v += newV;

                                }
                            }
                        }
                    }
                }

         

            for (int sources = 0; sources < _powerSource.Count; sources++)
            {
            //    _powerSource[sources].ForceAirSparks = false;
            }
        }

        public void PostUpdate()
        {
            for (int sources = 0; sources < _powerSource.Count; sources++)
            {
                _powerSource[sources].HaveRecivedPower = false;
                _powerSource[sources].PowerReciveAmount = 0;
            }
        }

        public void Update(float dt)
        {

           
            _sparkTimer += dt;
            if (_sparkTimer >= SparkFrequency)
            {
                _sparks.Clear();
                _sparkTimer = 0;
                GenerateSparks();
            }
        }

        public void DrawLightMap(SpriteBatch sb, Vector2 camera = new Vector2())
        {



            for (int i = 0; i < _sparks.Count; i++)
            {
                sb.Draw(_sprite, _sparks[i].Start - camera, new Rectangle(0, 0, Convert.ToInt32(_sparks[i].Lenght), 20), Colour, _sparks[i].Direction, new Vector2(0, 10), _sparks[i].Sacle * 3f, SpriteEffects.None, 0f);
                sb.Draw(_spriteGlow, _sparks[i].Start - camera, new Rectangle(0, 0, Convert.ToInt32(_sparks[i].Lenght), 20), Colour, _sparks[i].Direction, new Vector2(0, 10), _sparks[i].Sacle * 3f, SpriteEffects.None, 0f);
            }
            sb.End();

            Poligons.Begin();

            for (int i = 0; i < _sparks.Count; i++)
            {

                Poligons.DrawLine(_sparks[i].Start - camera,
                  _sparks[i].Start - camera + Vector2.Transform(new Vector2(_sparks[i].Lenght, 0), Matrix.CreateRotationZ(_sparks[i].Direction)), Color.White);
            }

            sb.Begin();
        }

        public void Draw(SpriteBatch sb, Vector2 camera = new Vector2())
        {
           

           
            for (int i = 0; i < _sparks.Count; i++)
            {
                sb.Draw(_sprite, _sparks[i].Start - camera, new Rectangle(0, 0, Convert.ToInt32(_sparks[i].Lenght), 20), Colour, _sparks[i].Direction, new Vector2(0, 10), _sparks[i].Sacle, SpriteEffects.None, 0f);
                sb.Draw(_spriteGlow, _sparks[i].Start - camera, new Rectangle(0, 0, Convert.ToInt32(_sparks[i].Lenght), 20), Colour, _sparks[i].Direction, new Vector2(0, 10), _sparks[i].Sacle, SpriteEffects.None, 0f);
            }
                sb.End();

            Poligons.Begin();

            for (int i = 0; i < _sparks.Count; i++)
            {

                Poligons.DrawLine(_sparks[i].Start - camera,
                  _sparks[i].Start - camera + Vector2.Transform(new Vector2(_sparks[i].Lenght, 0), Matrix.CreateRotationZ(_sparks[i].Direction)), Color.White);
            }

            sb.Begin();
        }
    }
}
