using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace AdvancedInput
{
    /// <summary>
    /// a very simple mouse
    /// </summary>
    public class SimpleMouse : GameComponent //inheriate from game componate so we can add it as a componet and have update 
    {
        //if this has been set up to be used
        private static bool _HaveIntialized = false;

        //the mouse states
        private static MouseState _MouseState, _LMouseState;

        //our actions
        public static Action<Vector2> OnLeftClick;
        public static Action<Vector2> OnLeftDown;

        public static bool IsLButtonDown { get { return _MouseState.LeftButton == ButtonState.Pressed; } }


        public SimpleMouse(Game game) : base(game)
        {
            if (_HaveIntialized) //initalize the stuff we neeed
                throw new Exception("You can only have one simple mouse in a project");

            _HaveIntialized = true;
        }

        public override void Update(GameTime gameTime)
        {
            //get current state and update last state
            _LMouseState = _MouseState;
            _MouseState = Mouse.GetState();

            if (_MouseState.LeftButton == ButtonState.Pressed) //left mouse down
            {
                if (_LMouseState.LeftButton == ButtonState.Released) //click if last mouse state is released
                    if (OnLeftClick != null) //on click
                        OnLeftClick(new Vector2(_MouseState.X, _MouseState.Y));

                if (OnLeftDown != null) //on hold
                    OnLeftDown(new Vector2(_MouseState.X, _MouseState.Y));
            }


            base.Update(gameTime);
        }
    }
}
