using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Effects
{
    public class PoligonEffect: Effect
    {
        public EffectParameter matrixParam;

        EffectParameter MatrixTransform;

        public Matrix World, View, Projection;

        public bool TextureEnabled;

        public Texture2D Texture { set
            {
                EffectParameter tex = this.Parameters["Texture"];
                if (tex == null)
                    tex = this.Parameters["TextureSampler"];
                tex.SetValue(value);
            } }

        public PoligonEffect(Effect source) :
            base(source)
        {
            MatrixTransform = Parameters["MatrixTransform"];
        }

        private void CalculateMatrix()
        {
            Parameters["MatrixTransform"].SetValue(World * View * Projection);
        }

        protected override void OnApply()
        {
            CalculateMatrix();
            base.OnApply();
        }
    }
}
