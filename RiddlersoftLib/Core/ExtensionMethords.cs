using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Core.Extentions
{
    public static class IntExtension
    {
        /// <summary>
        /// Shrinks the rectangle by the given amount and returns a new rectangle
        /// </summary>
        /// <param name="source"></param>
        /// <param name="amountInPx">the amount of pixels to shrink on each side</param>
        /// <returns></returns>
        public static Rectangle Shrink(this Rectangle source, int amountInPx)
        {
            return new Rectangle(source.X + amountInPx, source.Y + amountInPx, source.Width - amountInPx * 2, source.Height - amountInPx * 2);
        }

        /// <summary>
        /// Grows the rectangle by the given amount and returns a new rectangle
        /// </summary>
        /// <param name="source"></param>
        /// <param name="amountInPx">the amount of pixels to grow the rectangle by on each side</param>
        /// <returns></returns>
        public static Rectangle Grow(this Rectangle source, int amountInPx)
        {
            return new Rectangle(source.X - amountInPx, source.Y - amountInPx, source.Width + amountInPx * 2, source.Height + amountInPx * 2);
        }

        /// <summary>
        /// return the center of the rectangle
        /// </summary>
        /// <param name="source">the source rectangle</param>
        /// <param name="IncludeCords">if we want the x and y of the rectangle to be included in the result</param>
        /// <returns></returns>
        public static Vector2 Center(this Rectangle source, bool IncludeCords)
        {
            if (IncludeCords)
                return new Vector2(source.Width * .5f + source.X, source.Height * .5f + source.Y);

            return new Vector2(source.Width * .5f, source.Height * .5f);
        }

        /// <summary>
        /// returns the x and y cords
        /// </summary>
        /// <param name="source">the source rectangle</param>
        /// <returns>returns the x, y of the rectangle</returns>
        public static Vector2 TopLeft(this Rectangle source)
        {
            return new Vector2(source.X, source.Y);
        }

        /// <summary>
        /// returns the x and y + height cords
        /// </summary>
        /// <param name="source">the source rectangle</param>
        /// <returns>returns the x, y + height of the rectangle</returns>
        public static Vector2 BotomLeft(this Rectangle source)
        {
            return new Vector2(source.X, source.Y + source.Height);
        }
    }
}


