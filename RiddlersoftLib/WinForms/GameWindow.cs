using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Riddlersoft.Core.Input;
using WinForms;
using Microsoft.Xna.Framework.Input;

namespace WinForms
{
    /// <summary>
    /// A game window detects all clicks within its bounds. It does no drawing
    /// other than drawing any componates attached to it.
    /// the isvisible flag is ingnored and instead uses isacive to know 
    /// if to render componats or not
    /// </summary>
    public class GameScreen: Componate
    {
        /// <summary>
        /// returns true if the game window has had a click event within the last update
        /// </summary>
        private bool _haveClickEvent = false;
        public bool HaveClickEvent { get { return _haveClickEvent; } }

        public bool IsActiveComponate = false;

        public GameScreen(Rectangle area, Componate parent): base (area, parent)
        {
            _internalRender = false;
        }
        public override bool OnRightMouseClick(Point p)
        {
            _haveClickEvent = true;
            return base.OnRightMouseClick(p);
        }

        public override bool OnLeftMouseClick(Point p)
        {
            _haveClickEvent = true;
            return base.OnLeftMouseClick(p);
        }

        public override bool ComponatsCheckLeftMouseDown(Point p)
        {
            bool childPressed = base.ComponatsCheckLeftMouseDown(p);

            if (MouseTouch.LastMouseS.LeftButton == ButtonState.Released) //if intial press
            {
                if (childPressed)
                    IsActiveComponate = false;
                else
                    IsActiveComponate = true;
            }

            return childPressed;
        }

        protected override void Update(float dt)
        {
            _haveClickEvent = false;
        }

        /*
        public override bool RenderPrimitives(GraphicsDevice device, BasicCamera camera)
        {
         /   if (Active)
                
            {
                for (int i = 0; i < Componates.Count; i++)
                    Componates[i].RenderPrimitives(device, camera);
                return true;
            }
            return false;
        }
        */
        /*
        public override bool Render(ref SpriteBatch sb)
        {
            if (Active)
            {
                for (int i = 0; i < Componates.Count; i++)
                    Componates[i].Render(ref sb);
                return true;
            }
            return false;
        }
        */
    }
}
