using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
namespace AdvancedInput
{
    public class TimeRecord
    {
    
        public float ZeroToSixty, ZeroToOnehundrand, ZeroToOneFifty;

        public float ClutchBitingPoint;

        public bool WasClutchStart = false;
        public float TopSpeed = 0;
        public float ClutchReleaseTime;

        public float HoldTime;
        public TimeRecord() { }
        public TimeRecord(float zeroto60, float cbp, float crt, float ht = 0)
        {
            ZeroToSixty = zeroto60;
            ClutchBitingPoint = cbp;
            ClutchReleaseTime = crt;
            HoldTime = ht;
        }


    }
}
