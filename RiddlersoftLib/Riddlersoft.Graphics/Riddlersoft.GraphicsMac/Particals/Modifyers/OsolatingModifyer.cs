using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class OsolatingModifyer : Modifyer
    {
        public bool YAxis = true;
        public bool XAxis = true;
        public float Amount = 10;
        public float Speed = 2;
        public override void Processes(Partical input, float dt)
        {
            if (YAxis)
                input.Position.Y = input.Position.Y + (Amount * dt * (float)Math.Cos(input.LifeTime * Speed));

            if (XAxis)
                input.Position.X = input.Position.X + (Amount * dt * (float)Math.Sin(input.LifeTime * Speed));
        }
    }
}
