using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Riddlersoft.Core.Extentions;

namespace AdvancedInput.UI
{
    /// <summary>
    /// standard ui button
    /// </summary>
    public class Button : UiEliment
    {
        internal const float TransitionSpeed = 3f; //speed on trancation

        /// <summary>
        /// the advanced wheel for this button
        /// </summary>
        private AdvanceWheel _wheel;
        protected AdvanceWheel Wheel {  get { return _wheel; } }
   
        /// <summary>
        /// used for animations
        /// </summary>
        internal float _tranaction = 0;
        protected float Tranaction {  get { return _tranaction; } }

        /// <summary>
        /// the text to display for the button
        /// </summary>
        public string ButtonText = string.Empty;

        /// <summary>
        /// creates a new button
        /// </summary>
        /// <param name="wheel">the advance wheel</param>
        /// <param name="area">the area for the button</param>
        public Button(AdvanceWheel wheel, Rectangle area): base()
        {
            _wheel = wheel;
            _currentArea = area;
            
        }

      

        public override void Update(float dt)
        {
           
            //transition suff for basic click color
            if (IsMouseDown)
                _tranaction += dt * TransitionSpeed;
            else
                _tranaction -= dt * TransitionSpeed;
            _tranaction = MathHelper.Clamp(_tranaction, 0, 1);

            base.Update(dt);
        }


        public override void Draw(SpriteBatch sb)
        {
            
            sb.Draw(Dot, _currentArea, Color.Lerp(PrimaryColour, SecondryColour, _tranaction)); //draw the button color
            base.Draw(sb);

            if (ButtonText != string.Empty) //draw test
                sb.DrawString(Font, ButtonText, _currentArea.Center(true), TextColour, 0f, Font.MeasureString(ButtonText) * .5f, TextScale, SpriteEffects.None, 0f);
        }

        public Action<Button> OnClick;

        public override void OnLClick(Vector2 pos)
        {
            if (OnClick != null)
                OnClick(this);
        }

        public override void OnLButtonDown(Vector2 pos)
        {
        }
    }
}
