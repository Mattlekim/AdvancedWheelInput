using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinForms
{
    public class ProgressBar : Componate
    {
        public float MaxValue = 100, MinValue = 0;
        public float CurrentValue = 0;
        public string BaseText = "";
        public bool DisplayAsPercentage = true;

        public Texture2D Img;

        public ProgressBar(Rectangle area, Componate parentcomponate) : base(area, parentcomponate)
        {
        }
        
        private string tmp;
        protected override void Render(ref SpriteBatch sb)
        {
            if (Img == null)
                _internalRender = true;
            else
            {
                _internalRender = false;
                sb.Draw(bg, _areaCurrent, Color.Black);
            }

            float per = 1;
            if (MaxValue != 0)
                per = ((CurrentValue - MinValue) / (MaxValue - MinValue));
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, (int)(_areaCurrent.Width * per), _areaCurrent.Height), BgColourSecondry);

            if (Img != null)
                sb.Draw(Img, _areaCurrent, BgColourPrimary);

            if (DisplayAsPercentage)
            {
                tmp = BaseText + Math.Round(per * 100, 2).ToString() + "%";
                sb.DrawString(Font, tmp, new Vector2(_areaCurrent.X + _areaCurrent.Width / 2, _areaCurrent.Y + _areaCurrent.Height / 2),
                    Color.White, 0f, Font.MeasureString(tmp) * .5f, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                tmp = BaseText + CurrentValue.ToString() + " / " + MaxValue.ToString();
                sb.DrawString(Font, tmp, new Vector2(_areaCurrent.X + _areaCurrent.Width / 2 + 2, _areaCurrent.Y + _areaCurrent.Height / 2 + 2),
                    Color.Black, 0f, Font.MeasureString(tmp) * .5f, 1f, SpriteEffects.None, 0f);
                sb.DrawString(Font, tmp, new Vector2(_areaCurrent.X + _areaCurrent.Width / 2, _areaCurrent.Y + _areaCurrent.Height / 2),
                    Color.White, 0f, Font.MeasureString(tmp) * .5f, 1f, SpriteEffects.None, 0f);

                
            }
        }
    }
}