﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Riddlersoft.Core.Extentions;

namespace AdvancedInput.UI
{
    public class Surface : UiEliment
    {
        
        public override void OnLButtonDown(Vector2 pos)
        {
        }

        public override void OnLClick(Vector2 pos)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (TextName != null)
                sb.DrawString(Font, TextName, _currentArea.TopLeft(), TextColour, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
        }
    }
}
