using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics.Text.Modifyers
{
    public class ThreeWayColourModifyer : TextModifyer
    {
        public Color StartColour;
        public Color MiddleColour;
        public Color EndColour;

        public float StartTime;
        public float MiddleTimer;
        public float EndTime;

        private static float _tmp, _tmp1, _tmp2;

        public ThreeWayColourModifyer(Color startColour, float startTime, Color middleColor, float middleTimer, Color endColour, float endTime)
        {
            if (startTime >= middleTimer || middleTimer >= endTime)
                throw new Exception("timers are not correct");

            StartColour = startColour;
            StartTime = startTime;

            MiddleColour = middleColor;
            MiddleTimer = middleTimer;

            EndColour = endColour;
            EndTime = endTime;
        }

        protected override void ParentApply(TextChar c, StringEffect _effect, int index)
        {
            if (c.LifeTime <= StartTime)
                c.Colour = StartColour;
        }

        protected override void ParentUpdate(TextChar c, int index)
        {
            if (c.LifeTime <= StartTime)
            {
                c.Colour = StartColour;
                return;
            }

            if (c.LifeTime >= EndTime)
            {
                c.Colour = EndColour;
                return;
            }

            _tmp = MathHelper.Clamp(c.LifeTime - StartTime,0,1) / (MiddleTimer - StartTime);
            _tmp1 = MathHelper.Clamp(c.LifeTime - MiddleTimer,0,1) / (EndTime - MiddleTimer);
            _tmp2 = (c.LifeTime - StartTime) / (EndTime - StartTime);
            c.Colour = Color.Lerp(Color.Lerp(StartColour, MiddleColour, _tmp), Color.Lerp(MiddleColour, EndColour, _tmp1), _tmp2);
            

        }
    }
}
