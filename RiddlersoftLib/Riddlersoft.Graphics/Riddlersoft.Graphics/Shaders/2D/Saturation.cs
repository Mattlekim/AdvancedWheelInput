using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Shaders._2D
{
    public class SaturationEffect : Effect
    {
        public float Brightness
        {
            set
            {
                value = MathHelper.Clamp(value, 0f, 10f);
                Parameters["Brightness"].SetValue(value);
            }
        }

        public float Contrast
        {
            set
            {
                value = MathHelper.Clamp(value, 0f, 10f);
                Parameters["Contrast"].SetValue(value);
            }
        }

        EffectParameter MatrixTransform;

        public SaturationEffect(Effect source) :
            base(source)
        {
            MatrixTransform = Parameters["MatrixTransform"];
        }


    }
}
