using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riddlersoft.Graphics.Particals.Emitters;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class StateFadeModifyer : Modifyer
    {
        public float InitialFade { get; set; }
        public float MiddleLifeTime { get; set; }
        public float MiddleStateFade { get; set; }
        public float EndFade { get; set; }

        private Emitter _parent;
        
        public StateFadeModifyer(Emitter parent) : base()
        {
            _parent = parent;
            InitialFade = 1;
            MiddleStateFade = .5f;
            EndFade = 0;
            MiddleLifeTime = .5f;

        }

        public override void Processes(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            amount = 1 - amount; //get invers

            input.Fade = BasicMath.Lerp3(InitialFade, MiddleStateFade, MiddleLifeTime, EndFade, amount);
        }
    }
}
