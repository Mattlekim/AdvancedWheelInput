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
    public class SpecialAlphaMask : Effect
    {
        private Texture2D _alphaTexture;

        public Texture2D AlphaTexture
        {
            get { return _alphaTexture; }
            set
            {
                _alphaTexture = value;

              //  GraphicsDevice.Textures[1] = value;
                //return;

                EffectParameter tex = this.Parameters["AlphaTexture"];
                if (tex == null)
                    tex = this.Parameters["AlphaTextureSampler"];
                tex.SetValue(value);
             //   Parameters["Mask"].SetValue(value);
            }
        }

        EffectParameter MatrixTransform;

        private const string _path = "Shaders\\AlphaMask";


        private bool _enableMask = true;

        public bool EnabledMask
        {
            get { return _enableMask; }
            set
            {
                _enableMask = value;
             
                if (_enableMask)
                    Parameters["Enabled"].SetValue(1f);
                else
                    Parameters["Enabled"].SetValue(0f);
            }
        }

        private float _rotation = 0;

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;

                Parameters["Rotation"].SetValue(value);
            }
        }

        public SpecialAlphaMask(Effect source) :
            base(source)
        {
            MatrixTransform = Parameters["MatrixTransform"];
        }

        public static SpecialAlphaMask Load(ContentManager content)
        {
            return new _2D.SpecialAlphaMask(content.Load<Effect>(_path));
        }

    }
}
