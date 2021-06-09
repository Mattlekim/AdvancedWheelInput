using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Graphics.Text
{
    public class LifeTimeTrigger
    {
        public float Time;
        public Action Trigger;
        public bool Used = false;
    }
}
