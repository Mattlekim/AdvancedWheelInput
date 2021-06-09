using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Particals
{
    public static class BasicMath
    {
        public static float Lerp(float val1, float val2, float amount)
        {
            return (val2 - val1) * amount + val1;
        }

        public static float Lerp3(float val1, float val2, float val2Position, float val3, float amount)
        {
            if (amount < val2Position)
                return Lerp(val1, val2, amount / val2Position);
            else
            {
                float per = (amount - val2Position) / (1 - val2Position);
                if (per > 1f)
                { }
                return Lerp(val2, val3, per);
            }
            
        }



        public static Color Lerp3(Color val1, Color val2, float val2Position, Color val3, float amount)
        {
            if (amount < val2Position)
                return Color.Lerp(val1, val2, amount / val2Position);
            else
                return Color.Lerp(val2, val3, (amount - val2Position) / (1 - val2Position));

        }
    }
}
