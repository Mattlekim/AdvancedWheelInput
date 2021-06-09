using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Riddlersoft.Core.Screen
{
    /// <summary>
    /// gives basic informatoin about the game screen including resoultoin
    /// </summary>
    public struct ScreenFormatter
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int VirtualWidth { get; private set; }
        public int VirtualHeight { get; private set; }

        public Matrix ScaleMatrix { get; private set; }

        public float ScaleX { get; private set; }
        public float ScaleY { get; private set; }

        internal ScreenFormatter(ScreenManiger _maniger)
        {
            Width = _maniger.ScreenResolutionWidth;
            Height = _maniger.ScreenResolutionHeight;

            VirtualWidth = _maniger.VirtualResolutionWidth;
            VirtualHeight = _maniger.VirtualResolutionHeight;

            ScaleX = (float)Width / (float)_maniger.VirtualResolutionWidth;
            ScaleY = (float)Height / (float)_maniger.VirtualResolutionHeight;

            ScaleMatrix = Matrix.CreateScale(new Vector3(ScaleX, ScaleY, 0));
        }

        public override string ToString()
        {
            return $"Screen > {Width}, {Height}, Scale > {ScaleX}, {ScaleY}";
        }

    }
}
