using System;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Text.Modifyers
{
    public class ScaleTransition : TextModifyer
    {

        public float StartScale, EndScale;
        public float EndScaleTime;

        private static float _tmp;

        public ScaleTransition(float startScale, float endScale, float endTime)
        {
            if (endTime < startScale)
                throw new Exception("End time cannot be before start time");
            if (endTime <= 0)
                throw new Exception("End timer must be greater than 0");
            StartScale = startScale;
          
            EndScale = endScale;
       
            EndScaleTime = endTime;
        }

        protected override void ParentApply(TextChar c, StringEffect _effect, int index)
        {
            c.Scale = StartScale;
            
        }

       

        protected override void ParentUpdate(TextChar c, int index)
        {
            if (c.LifeTime < 0)
                return;

           

            if (c.LifeTime <= EndScaleTime)
            {
                _tmp = c.LifeTime / (EndScaleTime);
                c.Scale = MathHelper.Lerp(StartScale, EndScale, _tmp);
                return;
            }
          

        }

       
    }
}
