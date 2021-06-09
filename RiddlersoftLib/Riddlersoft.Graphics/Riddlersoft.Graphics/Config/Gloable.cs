using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics.Config
{
    public static class Gloable
    {
        public static TextureResolution TextureRes { get; private set; }

        public static Action<TextureResolution> OnTextureResolutionChange;

        public static void SetTextureResolution(TextureResolution res)
        {
            if (res == TextureRes)
                return;
            TextureRes = res;
            if (OnTextureResolutionChange != null)
                OnTextureResolutionChange(TextureRes);
        }
    }
}
