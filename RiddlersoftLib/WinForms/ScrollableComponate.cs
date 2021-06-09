using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core.Input;


namespace WinForms
{
    public enum ScrollDirection { None = 0, Horizontal = 1, Vertical = 2 };

    public class ScrollableComponate : Componate
    {
        public ScrollDirection DirectionScroable = ScrollDirection.None;

        public ScrollableComponate(Rectangle area, Componate parentcomponate) : base(area, parentcomponate)
        {
        }

        protected override void Update(float dt)
        {
            DragEvent de = MouseTouch.Drag;
            switch (DirectionScroable)
            {
                case ScrollDirection.Vertical:
                    if (de.Active)
                        if (Math.Abs(de.Delta.X) < Math.Abs(de.Delta.Y))
                            ScrollOffset.X += de.Delta.X;
                    break;

                case ScrollDirection.Horizontal:
                    if (de.Active)
                        if (Math.Abs(de.Delta.X) > Math.Abs(de.Delta.Y))
                            ScrollOffset.X += de.Delta.X;
                    break;

                case ScrollDirection.None:
                    ScrollOffset = Vector2.Zero;
                    break;
            }
    
        }

        protected override void Render(ref SpriteBatch sb)
        {
            
            if (_internalRender)
            {
                sb.Draw(bg, _areaCurrent, BgColourPrimary);

                if (!Boarderless)
                {
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, 2), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, 2, _areaCurrent.Height), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + _areaCurrent.Height - 2, _areaCurrent.Width, 2), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X + _areaCurrent.Width - 2, _areaCurrent.Y, 2, _areaCurrent.Height), BgColourSecondry);
                }
            }

            RenderComponates(ref sb);
            RenderDescription(ref sb);
            
        }

    }
}
