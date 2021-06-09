using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics.Text.Modifyers
{
    public class ThreeWayScaleModifyer : TextModifyer
    {
        public float StartScale;
        public float MiddleScale;
        public float EndScale;

        public float StartTime;
        public float MiddleTimer;
        public float EndTime;



        private static float _tmp, _tmp1, _tmp2;

        public ThreeWayScaleModifyer(float startScale, float startTime, float middleScale, float middleTimer, float endScale, float endTime)
        {
            if (startTime >= middleTimer || middleTimer >= endTime)
                throw new Exception("timers are not correct");

            StartScale = startScale;
            StartTime = startTime;

            MiddleScale = middleScale;
            MiddleTimer = middleTimer;

            EndScale = endScale;
            EndTime = endTime;
        }

        protected override void ParentApply(TextChar c, StringEffect _effect, int index)
        {
            if (c.LifeTime <= StartTime)
            {
                _tmp = MathHelper.Clamp(c.LifeTime - StartTime, 0, 1) / (MiddleTimer - StartTime);
                _tmp1 = MathHelper.Clamp(c.LifeTime - MiddleTimer, 0, 1) / (EndTime - MiddleTimer);
                _tmp2 = (c.LifeTime - StartTime) / (EndTime - StartTime);
                c.Scale = MathHelper.Lerp(MathHelper.Lerp(StartScale, MiddleScale, _tmp), MathHelper.Lerp(MiddleScale, EndScale, _tmp1), _tmp2);
            }
        }

        protected override void ParentUpdate(TextChar c,int index)
        {
            if (c.LifeTime <= StartTime)
            {
             
                return;
            }

            if (c.LifeTime >= EndTime)
            {

              
                return;
            }

            _tmp = MathHelper.Clamp(c.LifeTime - StartTime,0,1) / (MiddleTimer - StartTime);
            _tmp1 = MathHelper.Clamp(c.LifeTime - MiddleTimer,0,1) / (EndTime - MiddleTimer);
            _tmp2 = (c.LifeTime - StartTime) / (EndTime - StartTime);
            c.Scale = MathHelper.Lerp(MathHelper.Lerp(StartScale, MiddleScale, _tmp), MathHelper.Lerp(MiddleScale, EndScale, _tmp1), _tmp2);
            if (c.Scale < 0)
                c.Scale = 0;

        }
    }
}
