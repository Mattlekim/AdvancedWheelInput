using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Riddlersoft.Graphics.Particals.Emitters;
using Riddlersoft.Graphics.Particals.Modifyers;

using Riddlersoft.Core.Xml;
namespace Riddlersoft.Graphics.Particals
{
    public class ParticalEffect
    {
        public static ParticalEffect LoadFromStream(FileStream stream, GraphicsDevice device)
        {
            ParticalEffect effect = new ParticalEffect();

            CustomXmlReader reader = CustomXmlReader.Create(stream);
            
            reader.Read(); //read beging on doc
            reader.Read(); //read empty space
            reader.Read(); //read start element

            effect.Name = reader.ReadAttributeString("Name");
            Emitter e = null;
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Name == "Emitter")
                {
                    
                    string t = reader.ReadAttributeString("Type");
                    switch (t)
                    {
                        case "Circle":
                            e = new CircleEmitter(10);
                            CircleEmitter ce = (CircleEmitter)e;
                            ce.Radius = reader.ReadAttributeFloat("Radius");
                            ce.ParticalSpeed = new FloatRange(reader.ReadAttributeFloat("Speed_1"), reader.ReadAttributeFloat("Speed_2"));
                            ce.Function = reader.ReadAttributeEnum<EmmiterFunction>("Function");
                            ce.Offset = reader.ReadAttributeVector2("Offset");
                            break;
                        case "Cone":
                            e = new ConeEmitter(10);
                            ConeEmitter co = (ConeEmitter)e;
                            co.Radius = reader.ReadAttributeFloat("Radius");
                            co.ParticalSpeed = new FloatRange(reader.ReadAttributeFloat("Speed_1"), reader.ReadAttributeFloat("Speed_2"));
                            co.Function = reader.ReadAttributeEnum<EmmiterFunction>("Function");
                            co.MinRadious = reader.ReadAttributeFloat("MinAngle");
                            co.MaxRadious = reader.ReadAttributeFloat("MaxAngle");
                            co.Offset = reader.ReadAttributeVector2("Offset");
                            break;
                        case "Rectangle":
                            e = new RectangleEmitter(new Rectangle(0, 0, 10, 10));
                            break;

                        default:
                            e = new RectangleEmitter(new Rectangle(0, 0, 10, 10));
                            break;
                    }

                    e.Name = reader.ReadAttributeString("Name");
                    e.Initial_Angulor_Velocity = new FloatRange(reader.ReadAttributeFloat("I_Angulor_Vel_1"), reader.ReadAttributeFloat("I_Angulor_Vel_2"));

                    e.Initial_Colour = new ColourRange(reader.ReadAttributeColor("I_Colour_1"), reader.ReadAttributeColor("I_Colour_2"));

                    e.Initial_LifeTime = new FloatRange(reader.ReadAttributeFloat("I_LifeTime_1"), reader.ReadAttributeFloat("I_LifeTime_2"));

                    e.Initial_Rotaiton = new FloatRange(reader.ReadAttributeFloat("I_Rotaiton_1"), reader.ReadAttributeFloat("I_Rotaiton_2"));

                    e.Initial_Scale = new FloatRange(reader.ReadAttributeFloat("I_Scale_1"), reader.ReadAttributeFloat("I_Scale_2"));

                    e.Initial_Velocity = new Vector2Range(reader.ReadAttributeVector2("I_Vel_1"), reader.ReadAttributeVector2("I_Vel_2"));

                    e.TexturePath = reader.ReadAttributeString("TexturePath");

                    e.ReleaseAmount = reader.ReadAttributeFloat("ReleaseAmount");

                    if (e.TexturePath == string.Empty)
                    {
                        Texture2D tex = new Texture2D(device, 1, 1);
                        tex.SetData<Color>(new Color[] { Color.White });
                        e.SetTexture(tex, string.Empty);
                    }
                    else
                        using (FileStream fs = File.OpenRead(e.TexturePath))
                            e.SetTexture(Texture2D.FromStream(device, fs));
                    e.TexturePath = reader.ReadAttributeString("TexturePath");
                    effect.AddEmitter(e);

                   
                }

                if (reader.IsEmptyElement && reader.Name == "Modifyer")
                {
                    string type = reader.ReadAttributeString("Type");

                    
                    switch (type)
                    {
                        case "LinearColourModifyer":
                            LinearColourModifyer lcm = new LinearColourModifyer(e);
                            lcm.InitialColor = reader.ReadAttributeColor("Initial");
                            lcm.EndColor = reader.ReadAttributeColor("End");
                            e.AddModifyer(lcm);
                            break;

                        case "LinearFadeModifyer":
                            LinearFadeModifyer lfm = new LinearFadeModifyer(e);
                            lfm.InitialFade = reader.ReadAttributeFloat("Initial");
                            lfm.EndFade = reader.ReadAttributeFloat("End");
                            e.AddModifyer(lfm);
                            break;

                        case "LinearGravityModifyer":
                            LinearGravityModifyer lgm = new LinearGravityModifyer(e);
                            lgm.Gravity = reader.ReadAttributeVector2("Gravity");

                            e.AddModifyer(lgm);
                            break;

                        case "LinearScaleModifyer":
                            LinearScaleModifyer lsm = new LinearScaleModifyer(e);
                            lsm.EndScale = reader.ReadAttributeFloat("End");
                            e.AddModifyer(lsm);
                            break;

                        case "StateColourModifyer":
                            StateColourModifyer scm = new StateColourModifyer(e);
                            scm.InitialColor = reader.ReadAttributeColor("Initial");
                            scm.MiddleColour = reader.ReadAttributeColor("Middle");
                            scm.EndColour = reader.ReadAttributeColor("End");
                            scm.MiddleColourPosition = reader.ReadAttributeFloat("MiddlePosition");
                            e.AddModifyer(scm);
                            break;

                        case "StateFadeModifyer":
                            StateFadeModifyer sfm = new StateFadeModifyer(e);
                            sfm.InitialFade = reader.ReadAttributeFloat("Initial");
                            sfm.MiddleStateFade = reader.ReadAttributeFloat("Middle");
                            sfm.EndFade = reader.ReadAttributeFloat("End");
                            sfm.MiddleLifeTime = reader.ReadAttributeFloat("MiddlePosition");
                            e.AddModifyer(sfm);
                            break;

                        case "DampaningModifyer":
                            DampaningModifyer dm = new DampaningModifyer(1, e);
                            dm.AngulorDampaningAmount = reader.ReadAttributeFloat("DampenAngulorAmount");
                            dm.DampaningAmount = reader.ReadAttributeFloat("DampenAmount");
                            dm.DampenVelocity = reader.ReadAttributeBool("EnableDamping");
                            dm.DampenAngulorVelocity = reader.ReadAttributeBool("EnableAnuglorDamping");
                            e.AddModifyer(dm);
                            break;

                        case "OsolatingModifyer":
                            OsolatingModifyer om = new OsolatingModifyer();
                            om.Amount = reader.ReadAttributeFloat("Amount");
                            om.Speed = reader.ReadAttributeFloat("Speed");
                            om.XAxis = reader.ReadAttributeBool("XAxis");
                            om.YAxis = reader.ReadAttributeBool("YAxis");
                            e.AddModifyer(om);
                            break;

                        case "RotatingOsolatingModifyer":
                            RotatingOsolatingModifyer rom = new RotatingOsolatingModifyer();
                            rom.Amount = reader.ReadAttributeFloat("Amount");
                            rom.Speed = reader.ReadAttributeFloat("Speed");
                            e.AddModifyer(rom);
                            break;
                    }

                }
            }

            reader.Close();
            return effect;
        }

        public void SaveToStream(FileStream stream)
        {
            CustomXmlWriter writer = CustomXmlWriter.Create(stream);
            writer.WriteStartDocument();

            writer.WriteStartElement("Effect");
            writer.WriteAttributeString("Name", Name);

            foreach(Emitter e in _emitters)
            {
                writer.WriteStartElement("Emitter");

                if (e is CircleEmitter)
                {
                    CircleEmitter ce = ((CircleEmitter)e);
                    writer.WriteAttributeString("Type", "Circle");
                    writer.WriteAttributeFloat("Radius", ce.Radius);
                    writer.WriteAttributeFloat("Speed_1", ce.ParticalSpeed.One);
                    writer.WriteAttributeFloat("Speed_2", ce.ParticalSpeed.Two);
                    writer.WriteAttributeEnum<EmmiterFunction>("Function", ce.Function);
                    writer.WriteAttributeVector2("Offset", ce.Offset);
                }
                if (e is ConeEmitter)
                {
                    ConeEmitter ce = ((ConeEmitter)e);
                    writer.WriteAttributeString("Type", "Cone");
                    writer.WriteAttributeFloat("Radius", ce.Radius);
                    writer.WriteAttributeFloat("Speed_1", ce.ParticalSpeed.One);
                    writer.WriteAttributeFloat("Speed_2", ce.ParticalSpeed.Two);
                    writer.WriteAttributeEnum<EmmiterFunction>("Function", ce.Function);
                    writer.WriteAttributeFloat("MinAngle", ce.MinRadious);
                    writer.WriteAttributeFloat("MaxAngle", ce.MaxRadious);
                    writer.WriteAttributeVector2("Offset", ce.Offset);
                }
                if (e is RectangleEmitter)
                    writer.WriteAttributeString("Type", "Rectangle");
                
                writer.WriteAttributeString("Name", e.Name);
                writer.WriteAttributeFloat("ReleaseAmount", e.ReleaseAmount);

                writer.WriteAttributeFloat("I_Angulor_Vel_1", e.Initial_Angulor_Velocity.One);
                writer.WriteAttributeFloat("I_Angulor_Vel_2", e.Initial_Angulor_Velocity.Two);
                writer.WriteAttributeColor("I_Colour_1", e.Initial_Colour.One);
                writer.WriteAttributeColor("I_Colour_2", e.Initial_Colour.Two);

                writer.WriteAttributeFloat("I_LifeTime_1", e.Initial_LifeTime.One);
                writer.WriteAttributeFloat("I_LifeTime_2", e.Initial_LifeTime.Two);

                writer.WriteAttributeFloat("I_Rotaiton_1", e.Initial_Rotaiton.One);
                writer.WriteAttributeFloat("I_Rotaiton_2", e.Initial_Rotaiton.Two);

                writer.WriteAttributeFloat("I_Scale_1", e.Initial_Scale.One);
                writer.WriteAttributeFloat("I_Scale_2", e.Initial_Scale.Two);

                writer.WriteAttributeVector2("I_Vel_1", e.Initial_Velocity.One);
                writer.WriteAttributeVector2("I_Vel_2", e.Initial_Velocity.Two);

                writer.WriteAttributeString("TexturePath", e.TexturePath);

                

                foreach (Modifyer mod in e.Modifyers)
                {
                    writer.WriteStartElement("Modifyer");
                    if (mod is LinearColourModifyer)
                    {
                        writer.WriteAttributeString("Type", "LinearColourModifyer");
                        LinearColourModifyer lcm = mod as LinearColourModifyer;
                        writer.WriteAttributeColor("Initial", lcm.InitialColor);
                        writer.WriteAttributeColor("End", lcm.EndColor);
                    }

                    if (mod is LinearFadeModifyer)
                    {
                        writer.WriteAttributeString("Type", "LinearFadeModifyer");
                        LinearFadeModifyer lfm = mod as LinearFadeModifyer;
                        writer.WriteAttributeFloat("Initial", lfm.InitialFade);
                        writer.WriteAttributeFloat("End", lfm.EndFade);
                    }

                    if (mod is LinearGravityModifyer)
                    {
                        writer.WriteAttributeString("Type", "LinearGravityModifyer");
                        LinearGravityModifyer lgm = mod as LinearGravityModifyer;
                        writer.WriteAttributeVector2("Gravity", lgm.Gravity);
                    }

                    if (mod is LinearScaleModifyer)
                    {
                        writer.WriteAttributeString("Type", "LinearScaleModifyer");
                        LinearScaleModifyer lsm = mod as LinearScaleModifyer;
                        writer.WriteAttributeFloat("End", lsm.EndScale);
                    }

                    if (mod is StateColourModifyer)
                    {
                        writer.WriteAttributeString("Type", "StateColourModifyer");
                        StateColourModifyer scm = mod as StateColourModifyer;
                        writer.WriteAttributeColor("Initial", scm.InitialColor);
                        writer.WriteAttributeColor("Middle", scm.MiddleColour);
                        writer.WriteAttributeColor("End", scm.EndColour);
                        writer.WriteAttributeFloat("MiddlePosition", scm.MiddleColourPosition);
                    }

                    if (mod is StateFadeModifyer)
                    {
                        writer.WriteAttributeString("Type", "StateFadeModifyer");
                        StateFadeModifyer sfm = mod as StateFadeModifyer;
                        writer.WriteAttributeFloat("Initial", sfm.InitialFade);
                        writer.WriteAttributeFloat("Middle", sfm.MiddleStateFade);
                        writer.WriteAttributeFloat("End", sfm.EndFade);
                        writer.WriteAttributeFloat("MiddlePosition", sfm.MiddleLifeTime);
                    }

                    if (mod is DampaningModifyer)
                    {
                        writer.WriteAttributeString("Type", "DampaningModifyer");
                        DampaningModifyer dm = mod as DampaningModifyer;
                        writer.WriteAttributeFloat("DampenAngulorAmount", dm.AngulorDampaningAmount);
                        writer.WriteAttributeFloat("DampenAmount", dm.DampaningAmount);
                        writer.WriteAttributeBool("EnableDamping", dm.DampenVelocity);
                        writer.WriteAttributeBool("EnableAnuglorDamping", dm.DampenAngulorVelocity);
                    }

                    if (mod is OsolatingModifyer)
                    {
                        writer.WriteAttributeString("Type", "OsolatingModifyer");
                        OsolatingModifyer om = mod as OsolatingModifyer;
                        writer.WriteAttributeFloat("Amount", om.Amount);
                        writer.WriteAttributeFloat("Speed", om.Speed);
                        writer.WriteAttributeBool("XAxis", om.XAxis);
                        writer.WriteAttributeBool("YAxis", om.YAxis);
                    }


                    if (mod is RotatingOsolatingModifyer)
                    {
                        writer.WriteAttributeString("Type", "RotatingOsolatingModifyer");
                        RotatingOsolatingModifyer rom = mod as RotatingOsolatingModifyer;
                        writer.WriteAttributeFloat("Amount", rom.Amount);
                        writer.WriteAttributeFloat("Speed", rom.Speed);
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndDocument();
            writer.Close();
        }

        public const int MaxImiters = 3;

        public List<Emitter> Emitters { get { return _emitters; } }
        public List<Emitter> _emitters { get; private set; }
        private SpriteBatch _spriteBatch;

        public string Name = "effect";

        public void Clear()
        {
            foreach (Emitter emitter in _emitters)
                emitter.Clear();
        }

        public void SetEmitterTexture(Texture2D texture, int emitter, string path)
        {
            _emitters[emitter].SetTexture(texture,path);
        }

        

        public ParticalEffect()
        {
            _emitters = new List<Emitter>();
        }

        public void Trigger(Point position, int amount)
        {
            if (amount == 0)
                return;
            foreach (Emitter em in _emitters)
                em.Trigger(position, amount);
        }

        public void Trigger(Vector2 position, int amount, int velocity)
        {
            if (amount == 0)
                return;
            foreach (Emitter em in _emitters)
            {
                ConeEmitter ce = em as ConeEmitter;
                if (ce != null)
                    ce.ParticalSpeed = velocity;
                em.Trigger(position, amount);
            }
        }

        public void Trigger(Vector2 position, int amount)
        {
            if (amount == 0)
                return;
            foreach (Emitter em in _emitters)
                em.Trigger(position, amount);
        }

        public bool AddEmitter(Emitter emitter)
        {
            if (_emitters.Count >= MaxImiters)
                return false;
            _emitters.Add(emitter);
            return true;
        }

        public void KillParticles(Modifyers.KillModifyer kill, float dt)
        {
            foreach (Emitter em in _emitters)
                foreach (Partical p in em.Particals)
                {
                    kill.Processes(p, dt);
                }
        }

        public void Update(float dt)
        {
            foreach (Emitter em in _emitters)
                em.Update(dt);
        }

        public void Render(Vector2 offset)
        {
            foreach (Emitter em in _emitters)
                em.Render(_spriteBatch, offset);
        }

        public void Render(SpriteBatch sb)
        {
            foreach (Emitter em in _emitters)
                em.Render(sb);
        }

        public void Render(SpriteBatch sb, Vector2 offset)
        {
            foreach (Emitter em in _emitters)
                em.Render(sb, offset);
        }


    }
}
