using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedInput
{
    public enum InputType { Button, HatUp,HatDown,HatLeft,HatRight};


    public class Input
    {
        public InputType Type;
        public int Index;
    }
}
