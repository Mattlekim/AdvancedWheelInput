using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WinForms
{
    public class WinLable: Componate
    {
        private string _text;

        public bool CenterText { get; set; }
        public bool Shadow { get; set; }

        public Orentation TextOrentation = Orentation.Horizontal;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (Font != null && value != null) //if no nulls
                { //set the width and height
                    if (Units == MesurementUnit.Px)
                    {
                        _areaOriginal.Width = (int)Font.MeasureString(_text).X;
                        _areaOriginal.Height = (int)Font.MeasureString(_text).Y;

                        //also set for current as current width and height are not update for performace reason
                        _areaCurrent.Width = _areaOriginal.Width;
                        _areaCurrent.Height = _areaOriginal.Height;
                    }
                    _textCenter = new Vector2(_areaOriginal.Width * .5f, _areaOriginal.Height * .5f);
                }
            }
        }
            
        public float FontSacle = 1f;

        public WinLable(Rectangle area, Componate parentcomponate, string text = "") : base(area, parentcomponate)
        {
            _internalRender = false;
            Text = text;
            TextColourSecondry = Color.Black;
            TextColourPrimary = Color.White;
        }

        public WinLable(Vector2 pos, Componate parentcomponate, string text = "") : base(new Rectangle((int)pos.X, (int)pos.Y, 0, 0), parentcomponate)
        {
            _internalRender = false;
            Text = text;
            TextColourSecondry = Color.Black;
            TextColourPrimary = Color.White;
        }

        protected Vector2 _textCenter { get; private set; }
        protected override void Render(ref SpriteBatch sb)
        {
            float rotation = 0f;
            if (TextOrentation == Orentation.Veritical)
                rotation = MathHelper.PiOver2;
            if (CenterText)
            {
                if (Shadow)
                    sb.DrawString(Font, _text, new Vector2(_areaCurrent.X + 3, _areaCurrent.Y + 3), TextColourSecondry, rotation, _textCenter, FontSacle * MasterScale, SpriteEffects.None, 0f);
                sb.DrawString(Font, _text, new Vector2(_areaCurrent.X, _areaCurrent.Y), TextColourPrimary, rotation, _textCenter, FontSacle * MasterScale, SpriteEffects.None, 0f);
            }
            else
            {
                if (Shadow)
                    sb.DrawString(Font, _text, new Vector2(_areaCurrent.X + 3, _areaCurrent.Y + 3), TextColourSecondry, rotation, Vector2.Zero, FontSacle * MasterScale, SpriteEffects.None, 0f);
                if (Text != null)
                sb.DrawString(Font, _text, new Vector2(_areaCurrent.X, _areaCurrent.Y), TextColourPrimary, rotation, Vector2.Zero, FontSacle * MasterScale, SpriteEffects.None, 0f);
            }

            RenderDescription(ref sb);
            
        }
    }
}
