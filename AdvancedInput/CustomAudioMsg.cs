using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using iRacingSDK;
namespace AdvancedInput
{
    public class CustomAudioMsg
    {
        //audio files are in content/Voice
        //to load file its content.Load<SoundEffect>(path);

        //iv added files coldtires1,coldtires2,coldtires3,coldtires4,coldtires5
        //and warmtires1,  warmtires2,  warmtires3,  warmtires4
        AdvanceWheel _parent;

        SoundEffect coldTire;
        public CustomAudioMsg(AdvanceWheel parent)
        {
            _parent = parent;
        }

        public void LoadContent(ContentManager content)
        {
            coldTire = content.Load<SoundEffect>("Voice\\coldtires1");
           // coldTire.Play();
        }
        public void Update(float dt)
        {
            if (!_parent._telemitry.IsConnected)
                return;

            if (_parent._telemitry.CurrentData == null)
                return;
            //logic to get current car
            int mycarIndex = 0;
            if (_parent._telemitry.CurrentData.Telemetry.CarDetails.Length > 1)
                mycarIndex = 1;

            if (_parent._telemitry.CurrentData.Telemetry.CarDetails.Length <= 0)
            {
                return;
            }

            Car myCar = _parent._telemitry.CurrentData.Telemetry.Cars[mycarIndex]; //get my car

        }
    }
}
