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
    public class Slider : Componate
    {
        public enum DisplayTextType { None, Value, Percentage }



        public int CurrentValue {get; protected set;}
        public int MaxValue = 2;
        public int MinValue = 0;

        private Orentation _orentation;
        /// <summary>
        /// the current value as a percentage base on the min and max values
        /// </summary>
        public float CurrentPercentage { get; private set; }

        private bool _adustValue = false;
        private bool _initalPress = true;

        public string CustomText = "";
        public DisplayTextType TextDisplayType = DisplayTextType.None;

        /// <summary>
        /// int is current value
        /// float in current percentage
        /// </summary>
        public Action<int, float> OnValueChange;

        public void OnMouseDownEvent(Vector2 location)
        {
            if (!_initalPress)
                return;

            _initalPress = false;
            if (Area.Contains(location.ToPoint()))
            {
                _adustValue = true;
            }
        }

        public void SetValue(int newValue)
        {
            
            CurrentValue = newValue;
            CurrentPercentage = (CurrentValue - MinValue) / (float)(MaxValue - MinValue);
        }

        public void OnMouseReleaseEvent(Vector2 location)
        {
            _adustValue = false;
            _initalPress = true;
        }

        public Slider(Rectangle area, int maxValue = 100, int minValue = 0,
            Componate parentcomponate = null,
            Orentation orentation = Orentation.Horizontal) : base(area, parentcomponate)
        {
            MaxValue = maxValue;
            MinValue = minValue;

            MouseTouch.OnLeftMouseDown += OnMouseDownEvent;
            MouseTouch.OnLeftMouseRelease += OnMouseReleaseEvent;

            CurrentPercentage = 0;
            CurrentValue = 0;
            _orentation = orentation;
        }

        private void UpdateSliderValue()
        {
            Vector2 pos = MouseTouch.Position;
            if (_adustValue)
                switch (_orentation)
                {
                    case Orentation.Veritical:
                        CurrentPercentage = pos.Y - Area.Y;
                        CurrentPercentage = Area.Height - CurrentPercentage;
                        CurrentPercentage /= Area.Height;
                        break;

                    case Orentation.Horizontal:
                        CurrentPercentage = pos.X - Area.X;
                        CurrentPercentage /= Area.Width;
                        break;
                }
            CurrentPercentage = MathHelper.Clamp(CurrentPercentage, 0, 1);
            int lastValue = CurrentValue;
            CurrentValue = Convert.ToInt32(CurrentPercentage * (MaxValue - MinValue) + MinValue);
            if (lastValue != CurrentValue)
                if (OnValueChange != null)
                    OnValueChange(CurrentValue, CurrentPercentage);
        }

        protected override void Update(float dt)
        {
            base.Update(dt);
            UpdateSliderValue();
          //  CurrentPercentage = (CurrentValue - MinValue) / (float)(MaxValue - MinValue);


        }

        protected override void Render(ref SpriteBatch sb)
        {
            base.Render(ref sb);
            int pos;

            string displayText = $"{CustomText} ";
            switch (TextDisplayType)
            {
                case DisplayTextType.Percentage:
                    displayText = $"{displayText}{CurrentPercentage}$";
                    break;
                case DisplayTextType.Value:
                    displayText = $"{displayText}{CurrentValue}$";
                    break;
            }

            switch (_orentation)
            {
                case Orentation.Horizontal:
                    pos = Convert.ToInt32(CurrentPercentage * Area.Width);
                    sb.Draw(bg, new Rectangle(Area.X, Area.Y, pos, Area.Height), BgColourSecondry);

                    if (TextDisplayType != DisplayTextType.None)
                        sb.DrawString(Font, displayText, new Vector2(Area.X + Area.Width / 2f, Area.Y + Area.Height / 2f)
                            , TextColourPrimary, 0f, Font.MeasureString(displayText) * .5f, Scale, SpriteEffects.None, 0f);
                    break;

                case Orentation.Veritical:
                    if (TextDisplayType != DisplayTextType.None)
                        sb.DrawString(Font, displayText, new Vector2(Area.X + Area.Width / 2f, Area.Y + Area.Height / 2f)
                            , TextColourPrimary, MathHelper.PiOver2, Font.MeasureString(displayText) * .5f, Scale, SpriteEffects.None, 0f);
                    pos = Area.Height - Convert.ToInt32(CurrentPercentage * Area.Height);
                    sb.Draw(bg, new Rectangle(Area.X, Area.Y + pos, Area.Width, Area.Height - pos), BgColourSecondry);
                    break;
            }
        }

    }
}
