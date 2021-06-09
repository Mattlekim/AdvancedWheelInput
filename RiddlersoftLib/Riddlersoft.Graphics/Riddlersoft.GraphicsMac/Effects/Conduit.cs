using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Effects
{
    public class Conduit
    {
        public bool Active;

        public bool Powered;
        public Vector2 Position;
        public bool HaveRecivedPower;
        public bool ForceAirSparks;

        public float PowerReciveAmount = 0;
        public float AmountOfPower = 1;

        public float NumberOfAirSparks = EletricityEffect.NumberOfAirSparks;
        public float MaxAirSparkLenght = EletricityEffect.MaxAirSparkLenght;

        public float SparkGap;
    }
}
