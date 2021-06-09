using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Input;

using Riddlersoft.Core;
using Riddlersoft.Core.Input;

namespace WinForms
{
    public class Button : Componate
    {
        public float SelectedScale = 1.04f;

        public string Text;
        protected bool Highlighted { get; private set; }
        public bool CloseParent = false;

        public float ImgScale = 1f;

        public bool HaveFrame = true;
        public Texture2D Img;
        public Texture2D backgroundImage;
        public Texture2D TodaledImg;

        private float _targetScale = 1;
        protected float _currentSacle { get; private set; } = 1;
        private const float AnimationLenght = .1f;
        private float _animationTimer;
        private bool _isAnimating = false;

        public bool TodalState = false;
        public bool Depressed { get; protected set; } = false;

        public Button(Rectangle area, Componate parentcomponate, string text = "") : base(area, parentcomponate)
        {
            Text = text;
        }

        public void ForceSelected()
        {
            Highlighted = true;
            OnMouseEnter();
        }

        public void ClearState()
        {
            OnMouseExit();
        }

        public void SetState(bool depressed)
        {
            Depressed = depressed;
            if (depressed)
                OnMouseEnter();
            else
                OnMouseExit();
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
            _targetScale = SelectedScale;
        }

        public override void OnMouseExit()
        {
            _targetScale = Scale;
            base.OnMouseExit();
        }

        public override bool OnLeftMouseClick(Point p)
        {
            if (CloseParent) //if this button is set to close the parent
                _parent.DeactivateNextUpdateCycle = true;

            if (TodalState) //swap stated if todal opption is enabled
                Depressed = !Depressed;

            return base.OnLeftMouseClick(p);
        }

        protected override void Update(float dt)
        {
            if (_isAnimating)
            {
                _animationTimer += dt;
                if (_animationTimer >= AnimationLenght)
                {
                    _animationTimer = AnimationLenght;
                    _isAnimating = false;
                }

                _currentSacle = MathHelper.Lerp(_currentSacle, _targetScale, _animationTimer / AnimationLenght);
            }
            else
               if (_targetScale != _currentSacle)
            {
                _animationTimer = 0;
                _isAnimating = true;
            }

            Highlighted = false;
            Point p = new Point((int)MouseTouch.Position.X, (int)MouseTouch.Position.Y);
            if (_areaCurrent.Contains(p))
            {
                ScaleTimer += dt * Controler.ScaleSpeed;
                if (ScaleTimer > 1)
                    ScaleTimer -= 2;
                Highlighted = true;
            }

            if (TodalState && Depressed)
            {
                Highlighted = true;
                
            }
            return;
        }

        protected Rectangle buttonArea;
        protected override void Render(ref SpriteBatch sb)
        {
            _internalRender = HaveFrame;
          
            float per = (_currentSacle - Scale) / (SelectedScale - Scale);
            Color renderColour;

            buttonArea = new Rectangle(_areaCurrent.X - Controler.BoarderSize, _areaCurrent.Y - Controler.BoarderSize,
                   _areaCurrent.Width + Controler.BoarderSize * 2, _areaCurrent.Height + Controler.BoarderSize * 2);

            float diffx = (float)_areaCurrent.Width * _currentSacle - _areaCurrent.Width;
            float diffy = (float)_areaCurrent.Height * _currentSacle - _areaCurrent.Height;
            diffx = diffx * .5f;
            diffy = diffy * .5f;

            buttonArea.X -= Convert.ToInt32(diffx);
            buttonArea.Y -= Convert.ToInt32(diffx);
            buttonArea.Height += Convert.ToInt32(diffx * 2);
            buttonArea.Width += Convert.ToInt32(diffx * 2);
             
            if (backgroundImage != null) //if we have a background image
            {
                renderColour = Color.Lerp(BgColourPrimary, BgColourPrimary * 2f, per);
                if (!Active)
                    renderColour = Color.Lerp(renderColour, Color.Black, .5f);
                sb.Draw(backgroundImage, buttonArea, renderColour);
                Boarderless = true;
            }

            if (Img != null) //if image button
            {
                renderColour = Color.Lerp(TextColourPrimary, TextColourPrimary * 2f, per);
                if (!Active)
                    renderColour = Color.Lerp(renderColour, Color.Black, .5f);

                if (TodalState && Depressed)
                    if (Depressed)
                        if (TodaledImg != null)
                        {
                            sb.Draw(TodaledImg, ScaleRectangle(buttonArea, ImgScale), renderColour);
                            return;
                        }

                sb.Draw(Img, ScaleRectangle(buttonArea, ImgScale), renderColour);
                
                return;
            }

            if (Highlighted)
            {
                if (!Boarderless)
                    sb.Draw(bg, new Rectangle(_areaCurrent.X - Controler.BoarderSize, _areaCurrent.Y - Controler.BoarderSize,
                      _areaCurrent.Width + Controler.BoarderSize * 2, _areaCurrent.Height + Controler.BoarderSize * 2), BgColourSecondry);

            }
            else
            {
                
                if (!Boarderless)
                {
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, _areaCurrent.Width, 2), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y, 2, _areaCurrent.Height), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X, _areaCurrent.Y + _areaCurrent.Height - 2, _areaCurrent.Width, 2), BgColourSecondry);
                    sb.Draw(bg, new Rectangle(_areaCurrent.X + _areaCurrent.Width - 2, _areaCurrent.Y, 2, _areaCurrent.Height), BgColourSecondry);
                }
            }
            Color col = TextColourPrimary;
            if (!Active)
                col = Color.Gray;

            if (Text != null)
                sb.DrawString(Font, Text, new Vector2(_areaCurrent.X + _areaCurrent.Width / 2, _areaCurrent.Y + _areaCurrent.Height / 2), col,
                        0f, Font.MeasureString(Text) * .5f, MasterScale * _currentSacle  , SpriteEffects.None, 0f);
            
         
        }
    }
}
