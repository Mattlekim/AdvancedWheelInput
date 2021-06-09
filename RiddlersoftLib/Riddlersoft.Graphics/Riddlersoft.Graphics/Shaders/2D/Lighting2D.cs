using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Graphics.Shaders._2D
{
    public class Lighting2D : Effect
    {
        private const string Path = "Shaders\\BasicLighting";

        private float _lightStrenth = 1;

        /// <summary>
        /// the general strenth of the light
        /// </summary>
        public float Brightness
        {
            get
            {
                return _lightStrenth;
            }
            set
            {
                _lightStrenth = value;
                Parameters["Brightness"].SetValue(_lightStrenth);
            }
        }

        private float _ambientBrightness;

        public float AmbientBrightness
        {
            get { return _ambientBrightness; }
            set
            {
                _ambientBrightness = value;
                Parameters["AmbientBrigthness"].SetValue(value);
            }
        }

        private float _contrast = 1;
        /// <summary>
        /// the general strenth of the light
        /// </summary>
        public float Contrast
        {
            get
            {
                return _contrast;
            }
            set
            {
                _contrast = value;
                Parameters["Contrast"].SetValue(_contrast);
            }
        }

  
        

        public bool _enableLightMap = false;

        /// <summary>
        /// weather to enable normal map or not
        /// </summary>
        public bool EnableLightMap
        {
            get { return _enableLightMap; }
            set
            {
                _enableLightMap = value;
                //Parameters["EnableLightMap"].SetValue(0f);
                //return;

                if (_enableLightMap)
                    Parameters["EnableLightMap"].SetValue(1f);
                else
                    Parameters["EnableLightMap"].SetValue(0f);
            }
        }

        private Texture2D _lightMap;

        public Texture2D LightMap
        {
            get { return _lightMap; }
            set
            {
                _lightMap = value;
               
             //   GraphicsDevice.Textures[1] = value;
               // return;
                EffectParameter tex = this.Parameters["TextureLight"];
                if (tex == null)
                    tex = this.Parameters["TextureLightSampler"];
                tex.SetValue(value);

                //Parameters["TextureLight"].SetValue(value);
            }
        }

        public Texture2D _normalMap;

        public Texture2D NormalMap
        {
            get { return _normalMap; }
            set
            {
                _normalMap = value;
                Parameters["NormalMap"].SetValue(value);
            }
        }

        private Color _ambientColour;
        public Color AmbientColour
        {
            get { return _ambientColour; }
            set
            {
                _ambientColour = value;
                Parameters["AmbientColor"].SetValue(value.ToVector3());
            }
        }

  

        private Vector3 _lightDirection;
        public Vector3 LightDirection
        {
            get { return _lightDirection; }
            set
            {
                _lightDirection = value;
                Parameters["LightDirection"].SetValue(value);
            }
        }

        private Lighting2D(Effect cloneSource) : base(cloneSource)
        {
        }


        public static Lighting2D Load(ContentManager content)
        {
            return new _2D.Lighting2D(content.Load<Effect>(Path));
        }


    }
}
