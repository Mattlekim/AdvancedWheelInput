using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class RotatingOsolatingModifyer : Modifyer
    {
        public float Amount = 3;
        public float Speed = 2;
        public override void Processes(Partical input, float dt)
        {
            input.AngulorRotation = (Amount * dt * (float)Math.Sin(input.LifeTime * Speed));
            
        }
    }
}
