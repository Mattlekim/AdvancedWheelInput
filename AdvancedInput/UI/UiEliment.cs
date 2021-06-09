using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AdvancedInput.UI
{
    /// <summary>
    /// a basic ui eliment
    /// </summary>
    public abstract class UiEliment
    {
        
        /// <summary>
        /// the texture to draw anything rectanglual with
        /// </summary>
        internal static Texture2D Dot;

        //our font for outputting text
        internal static SpriteFont Font;

        //formatting collors
        public Color PrimaryColour = Color.DarkBlue;
        public Color SecondryColour = Color.LightBlue;

        //text suff
        public string TextName;
        public float TextScale = 1;
        public Color TextColour = Color.White;

        /// <summary>
        /// every ui eliment can contain other ui elements
        /// </summary>
        private List<UiEliment> Elements = new List<UiEliment>();

        //area
        protected Rectangle _currentArea;
        public Rectangle CurrentArea { get { return _currentArea; } }

     
        //visibility of the element
        public float Visiblity = 1f;

        /// <summary>
        /// construct the element
        /// </summary>
        /// <param name="haveLeftClick">if we have left click action</param>
        /// <param name="haveLeftHold">if we have left hold action</param>
        public UiEliment(bool haveLeftClick = true, bool haveLeftHold = false)
        {
            if (haveLeftClick)
                SimpleMouse.OnLeftClick += MouseLeftClick;
            if (haveLeftHold)
                SimpleMouse.OnLeftDown += MouseButtonDown;
        }

        /// <summary>
        /// add a new ui element to this
        /// </summary>
        /// <param name="el">the element to add</param>
        public void AddElement(UiEliment el)
        {
            if (el == null)
                throw new Exception("Can not be null");
            Elements.Add(el);
        }

        /// <summary>
        /// load the content
        /// </summary>
        /// <param name="dot">load in the 1x1 white texture</param>
        /// <param name="font">the font to use</param>
        public static void LoadContent(Texture2D dot, SpriteFont font)
        {
            Dot = dot;
            Font = font;
        }

        public virtual void Update(float dt)
        {
            if (!SimpleMouse.IsLButtonDown)
                _isLMouseDown = false; //reset the flag
            _isClicked = false; //reset is clicked flag
            foreach (UiEliment ui in Elements)
                ui.Update(dt);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            foreach (UiEliment ui in Elements) //draw all ui elements
                ui.Draw(sb);
        }

        /// <summary>
        /// flag for is there has been a mouse click this update
        /// </summary>
        private bool _isClicked = false;
        /// <summary>
        /// flag for is the l mouse button is down within this element
        /// </summary>
        private bool _isLMouseDown = false;

        public bool IsMouseDown { get { return _isLMouseDown; } }
        /// <summary>
        /// this funciton is called whenever there is a mouse click
        /// it check that the mouse click is withing the area of this button
        /// </summary>
        /// <param name="pos">mose postion</param>
        private void MouseLeftClick(Vector2 pos)
        {

            if (_currentArea.Contains(pos.ToPoint())) //validate the click to make sure its on the button
            {
                _isClicked = true;
                OnLClick(pos); //run on click
            }
        }

        /// <summary>
        /// this funciton is called whenever there is a mouse click
        /// it check that the mouse click is withing the area of this button
        /// </summary>
        /// <param name="pos">mose postion</param>
        private void MouseButtonDown(Vector2 pos)
        {
            if (_isLMouseDown)
            {
                OnLButtonDown(pos);
                return;
            }

            //first make sure this is the click
            if (!_isClicked)
                return;

            if (_currentArea.Contains(pos.ToPoint()))
                _isLMouseDown = true;

            OnLButtonDown(pos);
        }

        public abstract void OnLClick(Vector2 pos);

        public abstract void OnLButtonDown(Vector2 pos);

    }
}
