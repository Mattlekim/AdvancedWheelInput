using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core;
using Riddlersoft.Core.Input;

namespace WinForms
{
    public class Form: ScrollableComponate
    {
        public static float TitleTextOffset = 0;

        public static int TitleBarHeight = 30;

        public bool Movable = true;
        public bool Docable = true;
        public bool HaveCloseButton = true;
        public string Title;

        public Vector2 TopConer { get { return new Vector2(Position.X, Position.Y + TitleBarHeight); } }

        public Texture2D BackGroundImg;

        private bool _haveTitleBar = true;

        public delegate void _customRender(ref SpriteBatch sb);
        public _customRender CustomRender;
        /// <summary>
        /// if the form has a title bar or not
        /// </summary>
        public bool HaveTitleBar
        {
            get
            { return _haveTitleBar; }
            set
            {
                _haveTitleBar = value;
                if (!value)
                {
                    HaveCloseButton = false;
                    startMove = false;
                }
            }
        }

        bool startMove = false;
        Vector2 moveoffset = Vector2.Zero;

        private Button CloseButton;


        public Form(Rectangle area, Componate parentcomponate) : base(area, parentcomponate)
        {
            //title bar is always 30 pixels hight
            //so we need to calculate what percenentage that is
            if (HaveCloseButton)
                CloseButton = new Button(new Rectangle(96, 0, 30, TitleBarHeight), this)
                {
                    Img = Controler.XIcon,
                    CloseParent = true,
                    Boarderless = true,
                    _internalRender = false,
                    _Anchor = Anchor.Right,
                    Text = "X",
                    TextColourPrimary = Color.Red,
                    OnLeftClick = (object sender, Point p) =>
                    {
                        if (HaveCloseButton)
                            DeactivateNextUpdateCycle = true;
                    }
                };
        }

        public override bool ComponatsCheckLeftClick(Point p)
        {
            bool clicked = base.ComponatsCheckLeftClick(p);
            if (clicked)
                return true;

            if (CloseButton.Area.Contains(p))
            {
                CloseButton.OnLeftMouseClick(p);
                return true;
            }
                

            return false;
        }

        protected override void Update(float dt)
        {
            base.Update(dt);

            if (HaveCloseButton)
                CloseButton.InternalUpdate(dt);

            if (MouseTouch.MouseS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                MouseTouch.LastMouseS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                Point p = new Point((int)MouseTouch.Position.X, (int)MouseTouch.Position.Y);
                if (CloseButton.Area.Contains(p))
                {
                    
                }
                else
                if (Movable)
                    if (new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, TitleBarHeight).Contains(p)) //if title bar clicked
                    {
                        startMove = true;
                        moveoffset = new Vector2(p.X - _areaOriginal.X, p.Y - _areaOriginal.Y);
                    }

            }

            if (Movable) //if movable
                if (startMove) //if we have started moving
                    if (MouseTouch.LeftButtonHold)
                    {
                        _areaOriginal.X = Convert.ToInt32(MouseTouch.Position.X - moveoffset.X);
                        _areaOriginal.Y = Convert.ToInt32(MouseTouch.Position.Y - moveoffset.Y);
                    }
                    else
                        if (MouseTouch.MouseS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released) //if no longer holding down mouse
                    {
                        startMove = false;
                        moveoffset = Vector2.Zero;
                    }

        }

        protected override void Render(ref SpriteBatch sb)
        {
            if (BackGroundImg != null)
            {
                sb.Draw(BackGroundImg, _areaCurrent, BgColourPrimary);
                _internalRender = false;
            }
           

            if (CustomRender != null) //if we have a custom render
                CustomRender(ref sb); //render custom stuff

            //sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, TitleBarHeight), Color.Black * .4f);
            if (HaveTitleBar)
            {
                sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, TitleBarHeight), Controler.SecondryBG);
                if (Title != null)
                    sb.DrawString(FontLarge, Title, new Vector2(_areaCurrent.X + 5, _areaCurrent.Y + TitleTextOffset), Controler.PrimaryText);

                // sb.Draw(bg, new Rectangle(_areaCurrent.X + _areaCurrent.Width - 20, _areaCurrent.Y, 20, TitleBarHeight), Color.Black * .4f);
                // sb.DrawString(Fonts.Small, "X", new Vector2(_areaCurrent.X + _areaCurrent.Width, _areaCurrent.Y), Color.Black, 0f, new Vector2(Fonts.Small.MeasureString("X").X, 0), 1, SpriteEffects.None, 0f);

                if (HaveCloseButton)
                    CloseButton.InternalRender(ref sb);
            }
            RenderComponates(ref sb);
        }

    }
}
