using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Riddlersoft.Core.Xml;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class KillModifyer : Modifyer
    {
        public enum KillDirectoin { All, Left, Right, Top, Bottom}

        public KillDirectoin KillCheck = KillDirectoin.All;
        public Rectangle LiveArea;

        protected override void _prosses(Partical input, float dt)
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
