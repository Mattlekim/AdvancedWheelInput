using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics
{
    public static class SpriteBatchExtender
    {
        public static Color ShadowColor = Color.Black;
        public static int ShadowAmount = 2;

       

        public static void DrawStringCentered(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color color,
            float rotation = 0, float scale = 1, SpriteEffects se = SpriteEffects.None, float depth = 0, bool shadow = false)
        {
            float al = (float)color.A / 255f;
            Vector2 center = font.MeasureString(text) * .5f;
            if (shadow)
                sb.DrawString(font, text, pos + new Vector2(ShadowAmount), ShadowColor * al, rotation, center, scale, se, depth);
            sb.DrawString(font, text, pos, color, rotation, center, scale, se, depth);
        }

        public static void DrawString(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color color, bool shadow = false,
            float rotation = 0, Vector2 origin = new Vector2(), float scale = 1, SpriteEffects se = SpriteEffects.None, float depth = 0)
        {
            float al = (float)color.A / 255f;
            if (shadow)
                sb.DrawString(font, text, pos + new Vector2(ShadowAmount), ShadowColor * al, rotation, origin, scale, se, depth);

            sb.DrawString(font, text, pos, color, rotation, origin, scale, se, depth);
        }

}
}
