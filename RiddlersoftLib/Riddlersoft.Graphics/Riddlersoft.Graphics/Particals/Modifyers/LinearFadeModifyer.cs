using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riddlersoft.Core.Xml;
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

        protected override void _prosses(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            amount = 1 - amount; //get invers
            input.Fade = (EndFade - InitialFade) * amount + InitialFade;
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
