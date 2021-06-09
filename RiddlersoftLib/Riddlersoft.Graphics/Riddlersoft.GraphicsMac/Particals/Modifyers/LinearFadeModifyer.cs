using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riddlersoft.Graphics.Particals.Emitters;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class LinearFadeModifyer : Modifyer
    {
        public float InitialFade { get; set; }
        public float EndFade { get; set; }

        private Emitter _parent;
        
        public LinearFadeModifyer(Emitter parent) : base()
        {
            _parent = parent;
            InitialFade = 1;
            EndFade = 0;
        }

        public override void Processes(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            amount = 1 - amount; //get invers
            input.Fade = (EndFade - InitialFade) * amount + InitialFade;
        }
    }
}
