using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics.Text.Modifyers
{
    public class ColorModifyer : TextModifyer
    {
        public Color StartColour;
        public Color EndColour;

        public float StartTime;
        public float EndTime;

        private static float _tmp;

        public ColorModifyer(Color startColour, float startTime, Color endColour, float endTime)
        {
            StartColour = startColour;
            StartTime = startTime;

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

            _tmp = c.LifeTime - StartTime;
            c.Colour = Color.Lerp(StartColour, EndColour, _tmp / (EndTime - StartTime));

        }
    }
}
