using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Riddlersoft.Core.Xml;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class OsolatingModifyer : Modifyer
    {
        public bool YAxis = true;
        public bool XAxis = true;
        public float Amount = 10;
        public float Speed = 2;
        protected override void _prosses(Partical input, float dt)
        {
            if (YAxis)
                input.Position.Y = input.Position.Y + (Amount * dt * (float)Math.Cos(input.LifeTime * Speed));

            if (XAxis)
                input.Position.X = input.Position.X + (Amount * dt * (float)Math.Sin(input.LifeTime * Speed));
        }

        public override void WriteToFile(CustomXmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void ReadFromFile(CustomXmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override CustomModifyer _create()
        {
            throw new NotImplementedException();
        }
    }
}
