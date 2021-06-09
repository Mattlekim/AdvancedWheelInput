using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics.Shaders._2D
{
    public class NormalMapLighting : Effect
    {
        private const string Path = "Shaders\\NormalMapLighting";

        private NormalMapLighting(Effect cloneSource) : base(cloneSource)
        {
        }

        private Color _ambientColor;

        public Color AmbientColor
        {
            get
            {
                return _ambientColor;
            }

            set
            {
                _ambientColor = value;
                Parameters["AmbientColor"].SetValue(value.ToVector4());
            }
        }

        private float _minimumLightPower = .1f;

        public float MinimumLightPower
        {
            get
            {
                return _minimumLightPower;
            }

            set
            {
                _minimumLightPower = value;
                Parameters["MinimumLightPower"].SetValue(value);
            }
        }

        private float _normalLightingMultiplyer = 3f;

        public float NormalLightingMultiplyer
        {
            get
            {
                return _normalLightingMultiplyer;
            }

            set
            {
                _normalLightingMultiplyer = value;
                Parameters["LightingMultiplyer"].SetValue(value);
            }
        }

        private float _ambientLightMultiplyer = 3f;

        public float AmbientLightMultiplyer
        {
            get
            {
                return _ambientLightMultiplyer;
            }

            set
            {
                _ambientLightMultiplyer = value;
                Parameters["AmbientLightMultiplyer"].SetValue(value);
            }
        }

        

        private bool _renderNormalLightingOnly = false;

        public bool RenderNormalLightingOnly
        {
            get
            {
                if (_renderNormalLightingOnly)
                return true;
                return false;
            }

            set
            {
                 _renderNormalLightingOnly = value;

                if (_renderNormalLightingOnly)
                    Parameters["RenderNormalLightingOnly"].SetValue(1);
                else
                    Parameters["RenderNormalLightingOnly"].SetValue(0);
            }
        }
        private Texture2D _lightMap;

        public Texture2D LightMap
        {
            get { return _lightMap; }
            set
            {
                _texture = value;

                //   GraphicsDevice.Textures[1] = value;
                // return;
                EffectParameter tex = this.Parameters["LightMap"];
                if (tex == null)
                    tex = this.Parameters["LightMapSampler"];
                tex.SetValue(value);

                //Parameters["TextureLight"].SetValue(value);
            }
        }

        private Texture2D _emmisionsMap;

        public Texture2D EmmisionsMap
        {
            get { return _emmisionsMap; }
            set
            {
                _texture = value;

                //   GraphicsDevice.Textures[1] = value;
                // return;
                EffectParameter tex = this.Parameters["EmmisionsMap"];
                if (tex == null)
                    tex = this.Parameters["EmmisionsMapSampler"];
                tex.SetValue(value);

                //Parameters["TextureLight"].SetValue(value);
            }
        }

        private Texture2D _lightDirectionalMap;

        public Texture2D LightDirectionalMap
        {
            get { return _lightDirectionalMap; }
            set
            {
                _texture = value;

                //   GraphicsDevice.Textures[1] = value;
                // return;
                EffectParameter tex = this.Parameters["LightDirectionMap"];
                if (tex == null)
                    tex = this.Parameters["LightDirectionMapSampler"];
                tex.SetValue(value);

                //Parameters["TextureLight"].SetValue(value);
            }
        }

        private Texture2D _texture;

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;

                //   GraphicsDevice.Textures[1] = value;
                // return;
                EffectParameter tex = this.Parameters["Texture"];
                if (tex == null)
                    tex = this.Parameters["TextureSampler"];
                tex.SetValue(value);

                //Parameters["TextureLight"].SetValue(value);
            }
        }

        private Texture2D _normalMap;

        public Texture2D NormalMap
        {
            get { return _normalMap; }
            set
            {
                _normalMap = value;

                //   GraphicsDevice.Textures[1] = value;
                // return;
                EffectParameter tex = this.Parameters["NormalMap"];
                if (tex == null)
                    tex = this.Parameters["NormalMapSampler"];
                tex.SetValue(value);

                //Parameters["TextureLight"].SetValue(value);
            }
        }


        public static NormalMapLighting Load(ContentManager content)
        {
            return new NormalMapLighting(content.Load<Effect>(Path));
        }
    }
}
