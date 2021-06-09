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
        //how fast the eletricity changes
        private const float SparkFrequency = .04f;

        //how many points each spark is made up of
        private const int NumberSparkPoints = 5;
        //the amount of deviation between sparks
        //a bigger number is more zigzag
        private const float SparkPointDeviation = .5f;

        //the gap that the sparks can jump
        public const float SparkGap = 80;

        //when there are no conduits how long is a spark
        public const float MaxAirSparkLenght = 80;
        //when there is no other conduit to connect to
        //how many sparks should we produce
        public const int NumberOfAirSparks = 3;

        /// <summary>
        /// the minimum of light a conduit can give off
        /// ie the ciurcle of light
        /// </summary>
        public const float MinLightPower = .5f;

        /// <summary>
        /// the amount of variation we can have on light
        /// LightCalculation is MinLightPower + minLightPower
        /// </summary>
        public const float LightPowerVariation = 2;

        private List<Spark> _sparks = new List<Spark>();

        private List<Conduit> _powerSource = new List<Conduit>();

        private float _sparkTimer = 0;
        private Texture2D _sprite;
        private Texture2D _spriteGlow;
        private Texture2D _spriteDirectionData;
        private Vector2 _spriteGlowCenter;

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
            _spriteGlow = game.Content.Load<Texture2D>("imgs\\background\\circle_light");
            _spriteDirectionData = game.Content.Load<Texture2D>("imgs\\WorldAssets\\LightDirectionCircle");
            _spriteGlowCenter = new Vector2(_spriteGlow.Width * .5f, _spriteGlow.Height * .5f);
        }
        private Random _rd = new Random();


        private Conduit _current, _target;
        private void GenerateSparks()
        {
            Vector2 vec;
            float dir;
            float lenght;


            for (int sources = 0; sources < _powerSource.Count; sources++)
            {
                _current = _powerSource[sources];

                if (_current.Powered)
                    _current.GlowIntencity = (float)_rd.NextDouble() * LightPowerVariation + MinLightPower;

                if (_current.Active)
                {
                    
                    _current.Active = false;
                    if (_current.Powered || _current.HaveRecivedPower)
                    {

                        bool connected = false;
                        if (!_current.HaveRecivedPower)
                        {
                            for (int target = 0; target < _powerSource.Count; target++)
                            {
                                if (sources != target)
                                {
                                    _target = _powerSource[target];
                                    if (MathHelper.Distance(_current.Position.X, _target.Position.X) <= _current.SparkGap * _target.Atractivness && MathHelper.Distance(_current.Position.Y, _target.Position.Y) <= _current.SparkGap * _target.Atractivness)
                                    {

                                       
                                        if (!_target.Powered)
                                        {
                                            _target.HaveRecivedPower = true;
                                            _target.PowerReciveAmount = _current.AmountOfPower;
                                        }

                                        if (!_current.SparkedThisUpdate)
                                        {
                                            Vector2 v = _current.Position;
                                            for (int i = 0; i < NumberSparkPoints - 1; i++)
                                            {
                                                vec = _target.Position - _current.Position;
                                                dir = (float)Math.Atan2(vec.Y, vec.X) + SparkPointDeviation * ((float)_rd.NextDouble() * 2 - 1);
                                                lenght = vec.Length() / NumberSparkPoints;
                                                _sparks.Add(new Spark() { Start = v, Direction = dir, Lenght = lenght, Sacle = 1f, });
                                                Vector2 newV = Vector2.Transform(new Vector2(lenght, 0), Matrix.CreateRotationZ(dir));
                                                v += newV;
                                            }
                                            vec = _target.Position - v;
                                            dir = (float)Math.Atan2(vec.Y, vec.X);
                                            lenght = vec.Length();
                                            _sparks.Add(new Spark() { Start = v, Direction = dir, Lenght = lenght, Sacle = 1f, });
                                            _target.SparkedThisUpdate = true;
                                        }
                                        
                                        connected = true;
                                    }
                                }
                            }
                        }

                        if (!connected || _target.ForceAirSparks) //if we are not connected we want to create random sparks through the air
                        {

                            for (int p = 0; p < _current.NumberOfAirSparks; p++)
                            {
                                Vector2 v = _current.Position;
                                lenght = (float)_rd.NextDouble() * _current.MaxAirSparkLenght;
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
            }



        }

        public void PostUpdate()
        {
            for (int sources = 0; sources < _powerSource.Count; sources++)
            {
                _current = _powerSource[sources];
                _current.HaveRecivedPower = false;
                _current.PowerReciveAmount = 0;
                _current.SparkedThisUpdate = false;
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

            foreach (Conduit c in _powerSource)
                if (c.Powered || c.HaveRecivedPower)
                    sb.Draw(_spriteGlow, c.Position - camera, null, Colour * .8f, 0f, _spriteGlowCenter, c.GlowIntencity,SpriteEffects.None, 0f);
        }

        public void DrawDirecionLightMap(SpriteBatch sb, Vector2 camera = new Vector2())
        {

            foreach (Conduit c in _powerSource)
                if (c.Powered || c.HaveRecivedPower)
                    sb.Draw(_spriteDirectionData, c.Position - camera, null, Color.White * .8f, 0f, _spriteGlowCenter, c.GlowIntencity, SpriteEffects.None, 0f);
        }
        /*
        public void DrawLightMapPoligons(SpriteBatch sb, Vector2 camera = new Vector2())
        {
            Poligons.Begin();

            for (int i = 0; i < _sparks.Count; i++)
            {

                Poligons.DrawLine(_sparks[i].Start - camera,
                  _sparks[i].Start - camera + Vector2.Transform(new Vector2(_sparks[i].Lenght, 0), Matrix.CreateRotationZ(_sparks[i].Direction)), Color.White);
            }
        
        }
        */

        public void Draw(SpriteBatch sb, Vector2 camera = new Vector2())
        {
            for (int i = 0; i < _sparks.Count; i++)
            {
                sb.Draw(_sprite, _sparks[i].Start - camera, new Rectangle(0, 0, Convert.ToInt32(_sparks[i].Lenght), 20), Color.White, _sparks[i].Direction, new Vector2(0, 10), _sparks[i].Sacle, SpriteEffects.None, 0f);
              
            }
    

         
        }
        
    }
}
