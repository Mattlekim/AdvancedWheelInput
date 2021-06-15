using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace AdvancedInput
{
    public enum InputType { Button, HatUp,HatDown,HatLeft,HatRight, Anolog};


    public class Input
    {
        public InputType Type;
        public int Index;
        public string InputID;
        public int InputDeviceIndex;
        public Input(InputType t, int i, string inputid)
        {
            Type = t;
            Index = i;
            InputID = inputid;
            CatchInputIndex();
        }

        private void CatchInputIndex()
        {
            if (InputID == null|| InputID == string.Empty)
            {
                InputDeviceIndex = -1;
                return;
            }

            for (int i =0; i < 8; i++)
                if (Joystick.GetCapabilities(i).Identifier == InputID)
                {
                    InputDeviceIndex = i;
                    return;
                }

        }

        public static implicit operator Input(int i)
        {
            return new Input(InputType.Button, i, string.Empty);
        }

   
    }
}
