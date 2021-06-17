using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace AdvancedInput
{
    /// <summary>
    /// input types
    /// </summary>
    public enum InputType { Button, HatUp,HatDown,HatLeft,HatRight, Anolog};

    /// <summary>
    /// an input device
    /// </summary>
    public class Input
    {
        /// <summary>
        /// the input type
        /// </summary>
        public InputType Type;

        /// <summary>
        /// the index for the input ie button number anolog stick
        /// </summary>
        public int Index;
        /// <summary>
        /// the id string from the input device
        /// we get this as input device order can change in windows on reboot
        /// therefore if we catch the actual id of the device we can keep track of it
        /// </summary>
        public string InputID;

        /// <summary>
        /// the input index
        /// this is not saved but found when the input is created
        /// </summary>
        public int InputDeviceIndex;
        public Input(InputType t, int i, string inputid)
        {
            Type = t;
            Index = i;
            InputID = inputid;
            CatchInputIndex();
        }

        /// <summary>
        /// get the inputs index by using the id of the device
        /// </summary>
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
