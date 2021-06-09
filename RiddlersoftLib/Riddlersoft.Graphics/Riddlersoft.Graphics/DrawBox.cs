using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Graphics
{
    public static class SpriteBatchShapeRender
    {
        static Texture2D dot;

        public static void Intitalize(GraphicsDevice device)
        {
            dot = new Texture2D(device, 1, 1); //create texture
            dot.SetData<Color>(new Color[1] { Color.White }); //set pixel color
        }

        public static void RenderVLine(SpriteBatch sb, Point p, int width, int thinkness, Color color)
        {
            sb.Draw(dot, new Rectangle(p.X, p.Y, width, thinkness), color);
        }

        public static void RenderVLine(SpriteBatch sb, Vector2 p, int width, int thinkness, Color color)
        {
            RenderVLine(sb, p.ToPoint(), width, thinkness, color);
        }

        public static void RenderHLine(SpriteBatch sb, Point p, int height, int thinkness, Color color)
        {
            sb.Draw(dot, new Rectangle(p.X, p.Y, thinkness, height), color);
        }

        public static void RenderHLine(SpriteBatch sb, Vector2 p, int height, int thinkness, Color color)
        {
            RenderHLine(sb, p, height, thinkness, color);
        }

        public static void RenderRectangle(SpriteBatch sb, Rectangle area, Color col)
        {
            int thinkness = 10;
            RenderVLine(sb, new Point(area.X, area.Y), area.Width, thinkness, col);

            RenderHLine(sb, new Point(area.X, area.Y), area.Height, thinkness, col);
            RenderHLine(sb, new Point(area.X + area.Width - thinkness, area.Y), area.Height, thinkness, col);

            RenderVLine(sb, new Point(area.X, area.Y + area.Height - thinkness), area.Width, thinkness, col);
        }
    }
}
