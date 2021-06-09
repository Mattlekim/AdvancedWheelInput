using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinForms
{
    /// <summary>
    /// a divider is a basic line
    /// </summary>
    public class Divider : Componate
    {
        public Divider(Rectangle area, Componate parent): base(area, parent)
        {

        }

        public Divider(int y, Componate parent) : base(new Rectangle(10, y, parent.Area.Width - 20, 4), parent)
        {
            Units = parent.Units;
        }

        protected override void Render(ref SpriteBatch sb)
        {
            sb.Draw(bg, _areaCurrent, Color.LightBlue);
        }
    }
}
