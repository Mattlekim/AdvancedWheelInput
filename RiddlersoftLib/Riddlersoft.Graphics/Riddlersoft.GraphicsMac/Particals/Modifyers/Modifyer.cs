using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Graphics.Particals.Modifyers
{
    public abstract class Modifyer
    {
        public abstract void Processes(Partical input, float dt);
    }
}
