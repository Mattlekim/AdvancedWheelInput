using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

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

        public override void Processes(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            float end = input.InititalScale * EndScale;
            amount = 1 - amount; //get invers
          
            input.Scale =  MathHelper.Lerp(input.InititalScale, end, amount);
        }
    }
}
