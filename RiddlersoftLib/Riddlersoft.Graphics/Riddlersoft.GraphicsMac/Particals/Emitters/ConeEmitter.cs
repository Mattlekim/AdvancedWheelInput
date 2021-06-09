using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals.Emitters
{
    public class ConeEmitter : CircleEmitter
    {
        public float MinRadious
        {
            get { return _minRadious; }
            set { _minRadious = value; }
        }

        public float MaxRadious
        {
            get { return _maxRadious; }
            set { _maxRadious = value; }
        }

        public ConeEmitter(float radius): base(radius)
        {
            Name = "Cone";
        }


       
    }
}
