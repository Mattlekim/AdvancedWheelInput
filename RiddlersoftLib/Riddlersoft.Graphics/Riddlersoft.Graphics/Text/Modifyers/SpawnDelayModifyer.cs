using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Graphics.Text.Modifyers
{
    public class SpawnDelayModifyer : TextModifyer
    {
        private float _delay;

        public SpawnDelayModifyer(float delay)
        {
            _delay = delay;
        }

        protected override void ParentApply(TextChar c, StringEffect _effect, int index)
        {
            c.LifeTime -= index * _delay;
        }

        protected override void ParentUpdate(TextChar c, int index)
        {
            
        }
    }
}
