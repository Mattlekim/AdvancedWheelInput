using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core.Input;

namespace WinForms
{
    public enum TextBoxInputType { Full, Int, Float}

    public class TextBox: Componate
    {
        private bool _selected = false;

        public bool MultiLine = false;
        public string Text;

        public Action<object> OnLeave;

        public InternalEvent OnKeyPressed;
        /// <summary>
        /// a basic text box
        /// </summary>
        /// <param name="area"></param>
        /// <param name="parent"></param>
        public TextBox(Rectangle area, Componate parent): base(area, parent)
        {
            Text = "";
            TextColourPrimary = Color.Black;
        }

        public override void ClickReset()
        {
            _selected = false;
            if (Active)
                if (OnLeave != null)
                    OnLeave(this);
            base.ClickReset();
        }

        public void ForceTextUpdate()
        {
            if (OnKeyPressed != null)
                OnKeyPressed(this);
        }

        public TextBoxInputType InputType = TextBoxInputType.Full;

        protected override void Update(float dt)
        {
            if (_selected)
            {
                char c;
                while (KeyboardAPI.IsKeyAvalible)
                {
                    c = KeyboardAPI.ReadKey();
                    if (c == '\b')
                    {
                        if (Text.Length > 0)
                            Text = Text.Substring(0, Text.Length - 1);
                        continue;
                    }
                    if (c != '#') // detect when we need to do more work with the key press
                    {
                        if (c == '\n' && !MultiLine) //if we are not multiline make the return key exit the textbox
                            _selected = false;
                        else
                            if (InputType != TextBoxInputType.Full)
                        {
                            if (Text.Length == 0)
                                if (c == '-')
                                    Text += c;

                            for (int i =0;i <=9; i++)
                                if (c == i.ToString()[0])
                                {
                                    Text += c;
                                    break;
                                }
                            if (InputType == TextBoxInputType.Float)
                            if (c == '.')
                                Text += c;

                        }
                        else
                            Text += c;
                    }
                    else
                    {
                        if (KeyboardAPI.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back)) //if back key is press then
                            if (Text.Length > 0)
                                Text = Text.Substring(0, Text.Length - 1);

                        if (KeyboardAPI.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) //if back key is press then
                            _selected = false;
                    }
                    if (OnKeyPressed != null)
                        OnKeyPressed(this);
                }
                _flasher += dt * 4;
                if (_flasher > 1)
                    _flasher -= 2;

                if (!_selected)
                    if (OnLeave != null)
                        OnLeave(this);

                KeyboardAPI.DisableXnaInputThisUpdate(); //disable xna style input
            }

        }

        public override bool OnLeftMouseClick(Point p)
        {
            _selected = true;
            return base.OnLeftMouseClick(p);
        }

        private float _flasher = 0;
        protected override void Render(ref SpriteBatch sb)
        {
                sb.Draw(bg, _areaCurrent, Color.White * Controler.DefaultFade);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, 2), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, 2, _areaCurrent.Height), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + _areaCurrent.Height - 2, _areaCurrent.Width, 2), Controler.SecondryBG);
            sb.Draw(bg, new Rectangle(_areaCurrent.X + _areaCurrent.Width - 2, _areaCurrent.Y, 2, _areaCurrent.Height), Controler.SecondryBG);

            if (Text != null) //if text is not null
            {
                bool valid = true;
                foreach (char c in Text)
                {
                    if (!Font.Characters.Contains(c) && c != '\n')
                        valid = false;
                }

                if (valid)
                {
                   
                    sb.DrawString(Font, Text, new Vector2(_areaCurrent.X + 2, _areaCurrent.Y), TextColourPrimary, 0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                    if (_selected)
                        sb.DrawString(Font, ":", new Vector2(_areaCurrent.X + 2 + Font.MeasureString(Text).X, _areaCurrent.Y), TextColourPrimary * (_flasher * _flasher),
                           0f, Vector2.Zero, MasterScale, SpriteEffects.None, 0f);
                }
            }

            RenderDescription(ref sb);
            
        }
    }
}
