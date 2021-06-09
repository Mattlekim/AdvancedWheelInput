using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Riddlersoft.Graphics.Particals.Emitters;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class StateColourModifyer : Modifyer
    {
        public Color InitialColor { get; set; }
        public Color MiddleColour { get; set; }
        public float MiddleColourPosition { get; set; }
        public Color EndColour { get; set; }

        private Emitter _parent;
        
        public StateColourModifyer(Emitter parent) : base()
        {
            _parent = parent;
            InitialColor = Color.White;
            EndColour = Color.Red;
        }

        public override void Processes(Partical input, float dt)
        {
            float amount = input.LifeTime / (input.Age + input.LifeTime);
            amount = 1 - amount; //get invers
            input.Colour = BasicMath.Lerp3(InitialColor, MiddleColour, MiddleColourPosition, EndColour, amount);
        }
    }
}
