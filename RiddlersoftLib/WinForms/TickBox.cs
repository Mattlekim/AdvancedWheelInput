using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core;

namespace WinForms
{
    public class TickBox: Componate
    {
        public bool Ticked = false;
        public string Text;
        public Vector2 TextOffset = Vector2.Zero;
        public InternalEvent OnChange;
        /// <summary>
        /// a basic text box
        /// </summary>
        /// <param name="area"></param>
        /// <param name="parent"></param>
        public TickBox(Rectangle area, Componate parent): base(area, parent)
        {
            Text = "";
        }

        /// <summary>
        /// a basic text box
        /// </summary>
        /// <param name="area"></param>
        /// <param name="parent"></param>
        public TickBox(Vector2 pos, Componate parent) : base(new Rectangle((int)pos.X, (int)pos.Y, 25,25), parent)
        {
            Text = "";
        }


        public override bool OnLeftMouseClick(Point p)
        {
            Ticked = !Ticked;
            if (OnChange != null)
                OnChange(this);
            return base.OnLeftMouseClick(p);
        }

        private float _flasher = 0;
        private float _textWidth = -1;
        protected override void Render(ref SpriteBatch sb)
        {
          
            sb.Draw(bg, _areaCurrent, Color.White * Controler.DefaultFade);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, 2), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, 2, _areaCurrent.Height), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + _areaCurrent.Height - 2, _areaCurrent.Width, 2), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X + _areaCurrent.Width - 2, _areaCurrent.Y, 2, _areaCurrent.Height), Controler.SecondryBG);
            
            if (Ticked)
                sb.Draw(bg, new Rectangle(_areaCurrent.X + 8, _areaCurrent.Y + 8, _areaCurrent.Width -16, _areaCurrent.Height-16), Color.Black * Controler.DefaultFade);

            if (_textWidth == -1)
                _textWidth = Font.MeasureString(Text).X;

            sb.DrawString(Font, Text, new Vector2(_areaCurrent.X - _textWidth - 5, _areaCurrent.Y - 7) + TextOffset, Color.White);

            RenderDescription(ref sb);
           
        }
    }
}
