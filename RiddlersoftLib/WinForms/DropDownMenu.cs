using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core;
using Riddlersoft.Core.Input;

namespace WinForms
{
    public class DropDownMenu: Componate
    {
        private List<Button> Items = new List<Button>();
        private bool Open;
        private int ButtonHeight;
        private int _openHeight; //the maximum amount of heigh that there is for the menu

        public DropDownMenu(Vector2 pos, List<string> buttons, Componate parent) : base(new Rectangle((int)pos.X,(int)pos.Y,0,0), parent)
        {
            Font = Fonts.VerySmall;
            ButtonHeight = Convert.ToInt32(Fonts.VerySmall.MeasureString("qwertyuiopasdfghjklzxcvbnm").Y); //the the maximum possible width of any text on the button
            ButtonHeight += 4; //add buffer

            int buttonWidth = 0;
            for (int i = 0; i < buttons.Count; i++)
                if (Font.MeasureString(buttons[i]).X > buttonWidth)
                    buttonWidth = Convert.ToInt32(Font.MeasureString(buttons[i]).X); //find the longes button text

            buttonWidth += 10; //add buffer to the lenght

            int buttonaddpos = 0;
            foreach (string b in buttons)
            {
                Items.Add(new Button(new Rectangle(0, buttonaddpos, buttonWidth, ButtonHeight), this)
                {
                    Text = b,
                    id = "_ddmo_"+b,
                    Font = Font,
                });
                buttonaddpos += ButtonHeight;
            }
            _areaOriginal = new Rectangle((int)pos.X, (int)pos.Y, buttonWidth, ButtonHeight);
            _openHeight = buttonaddpos;
            if (parent != null)
                _areaCurrent = new Rectangle(_areaOriginal.X + parent.Area.X, _areaOriginal.Y + parent.Area.Y, _areaOriginal.Width, _areaOriginal.Height);
            else
                _areaCurrent = _areaOriginal;
            Open = false;
        }

        public DropDownMenu(Rectangle area, List<Button> buttons, Componate parent) : base(area, parent)
        {
            Font = Fonts.VerySmall;
            ButtonHeight = Convert.ToInt32(Fonts.VerySmall.MeasureString("qwertyuiopasdfghjklzxcvbnm").Y); //the the maximum possible width of any text on the button
            ButtonHeight += 4; //add buffer

            int buttonWidth = 0;
            for (int i = 0; i < buttons.Count; i++)
                if (Font.MeasureString(buttons[i].Text).X > buttonWidth)
                    buttonWidth = Convert.ToInt32(Font.MeasureString(buttons[i].Text).X); //find the longes button text

            buttonWidth += 10; //add buffer to the lenght

            int buttonaddpos = 0;
            foreach (Button b in buttons)
            {
                Items.Add(new Button(new Rectangle(0, buttonaddpos, buttonWidth, ButtonHeight), this)
                {
                    Text = b.Text,
                    id = b.id,
                    Font = Font,
                });
                buttonaddpos += ButtonHeight;
            }
            _areaOriginal = new Rectangle(area.X, area.Y, buttonWidth, buttonaddpos);
            if (parent != null)
                _areaCurrent = new Rectangle(_areaOriginal.X + parent.Area.X, _areaOriginal.Y + parent.Area.Y, _areaOriginal.Width, _areaOriginal.Height);
            else
                _areaCurrent = _areaOriginal;
                    
        }

        public override bool OnLeftMouseClick(Point p)
        {
            if (Open)
            {
                for (int i = 1; i < Items.Count; i++)
                    if (Items[i].Area.Contains(p))
                    {
                        if (OnLeftClick != null)
                            OnLeftClick(Items[i], p);
                        Open = false;
                        _areaOriginal.Height = ButtonHeight;
                        break;
                    }


            }
            if (Items[0].Area.Contains(p))
            {
                Open = !Open;
                if (Open)
                    _areaOriginal.Height = _openHeight;
                else
                    _areaOriginal.Height = ButtonHeight;
            }
            else
                return true;

            return false;
        }

        protected override void Update(float dt)
        {
            
            if (Open)
            {
                //detect a mouse click
                if (MouseTouch.MouseS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && MouseTouch.LastMouseS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    Point p = new Point((int)MouseTouch.Position.X, (int)MouseTouch.Position.Y); //get the touch point
                    if (!Area.Contains(p)) //check if the mouse click is in the drop down menu
                    {
                        Open = false; //if not close the menu
                        _areaOriginal.Height = ButtonHeight;
                    }
                }
                for (int i = 0; i < Items.Count; i++)
                    Items[i].InternalUpdate(dt);
            }
            else
                Items[0].InternalUpdate(dt);
           
        }


        protected override void Render(ref SpriteBatch sb)
        {
            if (Open)
            {
                for (int i = 0; i < Items.Count; i++)
                    Items[i].InternalRender(ref sb);
            }
            else
                Items[0].InternalRender(ref sb);

        }
    }
}
