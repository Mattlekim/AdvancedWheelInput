using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

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

        public override void Processes(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            amount = 1 - amount; //get invers
            input.Colour = Color.Lerp(InitialColor, EndColor, amount);
        }
    }
}
