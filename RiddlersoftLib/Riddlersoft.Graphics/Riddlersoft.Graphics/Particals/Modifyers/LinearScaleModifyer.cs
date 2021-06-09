using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Riddlersoft.Core.Xml;
using Riddlersoft.Graphics.Particals.Emitters;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class LinearScaleModifyer : Modifyer
    {
        public float EndScale { get; set; }

        private Emitter _parent;
        
        public LinearScaleModifyer(Emitter parent) : base()
        {
            _parent = parent;
            EndScale = 1;
        }

        protected override void _prosses(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            float end = input.InititalScale * EndScale;
            amount = 1 - amount; //get invers
          
            input.Scale =  MathHelper.Lerp(input.InititalScale, end, amount);
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
