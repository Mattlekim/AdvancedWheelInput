using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
namespace Riddlersoft.Core.Screen
{
    public static class ScreenMath
    {
        public static Rectangle Resize(Rectangle input)
        {
            ScreenFormatter fm = ScreenManiger.Format;
            return new Rectangle(Convert.ToInt32(input.X * fm.ScaleX), Convert.ToInt32(input.Y * fm.ScaleY),
                Convert.ToInt32(input.Width * fm.ScaleX), Convert.ToInt32(input.Height * fm.ScaleY));
        }
    }
}
