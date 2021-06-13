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
    public class Slider : UiEliment
    {
        public float Max, Min, Current;

        public Action<Slider> OnChange;

        public Slider(Rectangle area, float max, float min, float cur) : base(true, true)
        {
            Max = max;
            Min = min;
            Current = cur;
            _currentArea = area;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

        }

        private static string _Output;
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (!_active)
                return;

            sb.Draw(Dot, _currentArea, SecondryColour * Visiblity);

            Rectangle secondArea = _currentArea.Shrink(2);
            sb.Draw(Dot, new Rectangle(secondArea.X, secondArea.Y + Convert.ToInt32(secondArea.Height * (1 - Current)), secondArea.Width, Convert.ToInt32(secondArea.Height * (Current))), PrimaryColour * Visiblity);


//            sb.Draw(Dot, new Rectangle(_currentArea.Left - 5, _currentArea.Y, 5, 2), SecondryColour * Visiblity);
            //sb.Draw(Dot, new Rectangle(_currentArea.Left - 5, _currentArea.Y + _currentArea.Height - 2, 5, 2), SecondryColour);
            //sb.Draw(Dot, new Rectangle(_currentArea.Left - 5, Convert.ToInt32(_currentArea.Y  + _currentArea.Height * (1 - Current) + 1), 8, 2), PrimaryColour * Visiblity);
            sb.DrawString(Font, $"{Max}", _currentArea.TopLeft() + new Vector2(32,2), Color.Black * Visiblity, 0f, new Vector2(Font.MeasureString($"{Max}").X, Font.MeasureString($"{Max}").Y * .5f), .5f, SpriteEffects.None, 0f);
            sb.DrawString(Font, $"{Min}", _currentArea.BotomLeft() + new Vector2(34, 2), Color.Black * Visiblity, 0f, new Vector2(Font.MeasureString($"{Min}").X, Font.MeasureString($"{Min}").Y * .5f), .5f, SpriteEffects.None, 0f);
            

            sb.DrawString(Font, $"{Max}", _currentArea.TopLeft() + new Vector2(30, 0), TextColour * Visiblity, 0f, new Vector2(Font.MeasureString($"{Max}").X, Font.MeasureString($"{Max}").Y * .5f), .5f, SpriteEffects.None, 0f);
            sb.DrawString(Font, $"{Min}", _currentArea.BotomLeft() + new Vector2(32, 0), TextColour * Visiblity, 0f, new Vector2(Font.MeasureString($"{Min}").X, Font.MeasureString($"{Min}").Y * .5f), .5f, SpriteEffects.None, 0f);
            _Output = $"{Math.Round(Current * (Max - Min) + Min, 2)}";
            sb.DrawString(Font, _Output, _currentArea.TopLeft() + new Vector2(49, 2 + secondArea.Height * (1 - Current)),  Color.Black * Visiblity, 0f, new Vector2(Font.MeasureString(_Output).X, Font.MeasureString(_Output).Y * .5f), .4f, SpriteEffects.None, 0f);
            sb.DrawString(Font, _Output, _currentArea.TopLeft() + new Vector2(47, secondArea.Height * (1 - Current)), TextColour * Visiblity, 0f, new Vector2(Font.MeasureString(_Output).X, Font.MeasureString(_Output).Y * .5f), .4f, SpriteEffects.None, 0f);

            if (TextName == null || TextName == string.Empty)
                return;

            sb.DrawString(Font, TextName, _currentArea.Center(true) + new Vector2(42, 2), Color.Black * Visiblity, -MathHelper.PiOver2, Font.MeasureString(TextName) * .5f, TextScale, SpriteEffects.None, 0f);
            sb.DrawString(Font, TextName, _currentArea.Center(true) + new Vector2(40, 0), TextColour * Visiblity, -MathHelper.PiOver2, Font.MeasureString(TextName) * .5f, TextScale, SpriteEffects.None, 0f);
        }

        public override void OnLClick(Vector2 pos)
        {
            
        }

        public override void OnLButtonDown(Vector2 pos)
        {
            float per = MathHelper.Clamp(1 - ((pos.Y - _currentArea.Y) / (float)_currentArea.Height),0,1);
            Current = per;
            Current = (float)Math.Round(Current, 2);

            if (OnChange != null)
                OnChange(this);
        }

        public float GetValue()
        {
            return Current * (Max - Min) + Min;
        }

        
    }
}
