using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class KillModifyer : Modifyer
    {
        public enum KillDirectoin { All, Left, Right, Top, Bottom}

        public KillDirectoin KillCheck = KillDirectoin.All;
        public Rectangle LiveArea;

        public override void Processes(Partical input, float dt)
        {
            if (KillCheck == KillDirectoin.All)
            {
                if (!LiveArea.Contains(input.Position))
                    input.LifeTime = 0;
            }
            else
                if (KillCheck == KillDirectoin.Left)
            {
                if (input.Position.X < LiveArea.X)
                    input.LifeTime = 0;
            }
            else
                if (KillCheck == KillDirectoin.Right)
            {
                if (input.Position.X > LiveArea.X)
                    input.LifeTime = 0;
            }
            else
                if (KillCheck == KillDirectoin.Top)
            {
                if (input.Position.Y < LiveArea.Y)
                    input.LifeTime = 0;
            }
            else
                if (KillCheck == KillDirectoin.Bottom)
            {
                if (input.Position.Y > LiveArea.Y)
                    input.LifeTime = 0;
            }
        }
    }
}
