using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Shaders._2D
{
    public class PixalateEffect : Effect
    {
        public EffectParameter matrixParam;

        EffectParameter MatrixTransform;

        public Matrix World, View, Projection;

        public float PixalateAmount
        {
            set
            {
                value = MathHelper.Clamp(value, .001f, 1f);
                Parameters["Amount"].SetValue(value);
            }
        }

        public PixalateEffect(Effect source) :
            base(source)
        {
            MatrixTransform = Parameters["MatrixTransform"];
        }

       

       

      

    }
}
