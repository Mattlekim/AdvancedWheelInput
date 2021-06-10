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
    
        public float ZeroToSixty, ZeroToOnehundrand;

        public float ClutchBitingPoint;

        public bool WasClutchStart = false;
    
        public float ClutchReleaseTime;

        public TimeRecord() { }
        public TimeRecord(float zeroto60, float cbp, float crt)
        {
            ZeroToSixty = zeroto60;
            ClutchBitingPoint = cbp;
            ClutchReleaseTime = crt;
        }

    }
}
