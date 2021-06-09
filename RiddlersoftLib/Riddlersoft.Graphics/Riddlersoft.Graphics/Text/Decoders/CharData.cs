using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics.Text.Decoders
{
    public struct CharData
    {
        public int StartIndex;
        public int Count;
        public Color Colour;

        public Texture2D Sprite;
    }
}
