using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riddlersoft.Core.Xml;
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

        protected override void _prosses(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            amount = 1 - amount; //get invers

            input.Fade = BasicMath.Lerp3(InitialFade, MiddleStateFade, MiddleLifeTime, EndFade, amount);
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
