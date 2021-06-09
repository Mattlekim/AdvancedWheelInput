using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Riddlersoft.Core.Xml;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class RotatingOsolatingModifyer : Modifyer
    {
        public float Amount = 3;
        public float Speed = 2;

        protected override CustomModifyer _create()
        {
            throw new NotImplementedException();
        }

        public override void ReadFromFile(CustomXmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteToFile(CustomXmlWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void _prosses(Partical input, float dt)
        {
            input.AngulorRotation = (Amount * dt * (float)Math.Sin(input.LifeTime * Speed));
            
        }
    }
}
