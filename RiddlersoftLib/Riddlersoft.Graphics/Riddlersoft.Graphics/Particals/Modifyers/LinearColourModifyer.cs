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
    public class LinearColourModifyer : Modifyer
    {
        public Color InitialColor { get; set; }
        public Color EndColor { get; set; }

        private Emitter _parent;
        
        public LinearColourModifyer(Emitter parent) : base()
        {
            _parent = parent;
            InitialColor = Color.White;
            EndColor = Color.Red;
        }

        protected override void _prosses(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            amount = 1 - amount; //get invers
            input.Colour = Color.Lerp(InitialColor, EndColor, amount);
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
