using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedInput
{
    public enum InputType { Button, HatUp,HatDown,HatLeft,HatRight, Anolog};


    public class Input
    {
        public InputType Type;
        public int Index;
        public string InputId;
        public int InputDeviceIndex;
        public Input(InputType t, int i, string inputid, int inputIndex)
        {
            Type = t;
            Index = i;
            InputId = inputid;
            InputDeviceIndex = inputIndex;
        }

        public static implicit operator Input(int i)
        {
            return new Input(InputType.Button, i, string.Empty, -1);
        }

   
    }
}
