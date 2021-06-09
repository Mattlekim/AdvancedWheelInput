using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Riddlersoft.Graphics.Shaders._2D
{
    public class GaussianBlur : Effect
    {
        EffectParameter MatrixTransform;

        private const string _path = "Shaders\\GaussianBlur";


       

        public GaussianBlur(Effect source) :
            base(source)
        {
            MatrixTransform = Parameters["MatrixTransform"];
        }

        public static GaussianBlur Load(ContentManager content)
        {
            return new _2D.GaussianBlur(content.Load<Effect>(_path));
        }

    }
}
