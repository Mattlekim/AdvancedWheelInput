using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riddlersoft.Graphics.Particals.Emitters;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public class LinearGravityModifyer : Modifyer
    {
        public Vector2 Gravity { get; set; }
        
        private Emitter _parent;
        
        public LinearGravityModifyer(Emitter parent) : base()
        {
            _parent = parent;
            Gravity = new Vector2(0, 9.81f);
        }

        public override void Processes(Partical input, float dt)
        {
            input.Velocity += Gravity * dt;
        }
    }
}
