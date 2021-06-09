using System;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Text.Modifyers
{
    public class FadeTransition : TextModifyer
    {
        public float FadeInDuration;
        public float FadeInTime;
        public float FadeOutDuration;
        public float FadeOutTime;

        private static float _tmp;

        public FadeTransition(float fadeInDuration, float fadeInTime, float fadeOutDuration, float fadeOutTime)
        {
            FadeInDuration = fadeInDuration;
            FadeInTime = fadeInTime;
            FadeOutDuration = fadeOutDuration;
            FadeOutTime = fadeOutTime;
        }

        protected override void ParentApply(TextChar c, StringEffect _effect, int index)
        {
            c.Opacity = 0;
            
        }

        protected override void ParentUpdate(TextChar c, int index)
        {
            if (c.LifeTime < FadeInTime && c.LifeTime >= FadeInTime - FadeInDuration)
            {
                _tmp = c.LifeTime - (FadeInTime - FadeInDuration);
                c.Opacity = _tmp / FadeInDuration;
                return;
            }

            if (c.LifeTime >= FadeOutTime)
            {
                _tmp = c.LifeTime - FadeOutTime;
                c.Opacity = 1 - MathHelper.Clamp(_tmp / FadeOutDuration, 0, 1);
                return;
            }


        }

       
    }
}
