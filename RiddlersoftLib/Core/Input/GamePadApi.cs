using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Riddlersoft.Core.Debug;

namespace Riddlersoft.Core.Input
{
    public enum DPadDirection { Up, Down, Left, Right }

    public class GamePadApi: GameComponent
    {
        private static GamePadState _currentState, _oldState;

        public static GamePadState State { get { return _currentState; } }
        public static GamePadState LastState { get { return _oldState; } }

        public static bool IsNintendoSwitchControler = false;
        
        private static int _playerOne = 0;

        private static bool _initalized = false;

        private static float _masterFeedbackLevel = 1;

        private static bool _connected = false;
        public static bool IsConnected { get { return _connected; } }

        public static Action<int> OnControlerDisconect;

        private static bool _joyConConnected = true;
        public static Action<int> OnJoyConsAttached;

        public static float MasterFeedbackLevel
        {
            get { return _masterFeedbackLevel; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;
                _masterFeedbackLevel = value;
            }
        }

        public static void SetControlerIndex(int index)
        {
            _playerOne = index;
        }

        public GamePadApi(Game game) : base(game)
        {
            if (_initalized)
                throw new Exception("Already Added. You cannot have more than one instance of this class");
            _initalized = true;

            _joyConConnected = GamePad.GetState(8).IsConnected;

        }

        private static Buttons GetInputForNintendoSwitch(Buttons input)
        {
            switch (input)
            {
                case Buttons.A:
                    return Buttons.B;
                case Buttons.B:
                    return Buttons.A;
                case Buttons.X:
                    return Buttons.Y;
                case Buttons.Y:
                    return Buttons.X;
                default:
                    return input;
            }
        }

        public static bool GetGamePadIndex = true;

        private static int _numberOfConnectedControlers = 0;
        private static bool CheckAllInputs(GamePadState _new)
        {
            if (_new.Buttons.A == ButtonState.Pressed || _new.Buttons.B == ButtonState.Pressed ||
                _new.Buttons.X == ButtonState.Pressed || _new.Buttons.Y == ButtonState.Pressed)
                return true;

            if (_new.Buttons.Back == ButtonState.Pressed || _new.Buttons.Back == ButtonState.Pressed)
                return true;

            if (_new.DPad.Up == ButtonState.Pressed || _new.DPad.Down == ButtonState.Pressed ||
                _new.DPad.Left == ButtonState.Pressed || _new.DPad.Right == ButtonState.Pressed)
                return true;

            if (Math.Abs(_new.ThumbSticks.Left.X) > .2f || Math.Abs(_new.ThumbSticks.Left.Y) > .2f)
                return true;

            if (Math.Abs(_new.ThumbSticks.Right.X) > .2f || Math.Abs(_new.ThumbSticks.Right.Y) > .2f)
                return true;

            if (_new.Triggers.Left > .2f || _new.Triggers.Right > .2f)
                return true;

            if (_new.Buttons.LeftShoulder == ButtonState.Pressed || _new.Buttons.RightShoulder == ButtonState.Pressed)
                return true;

            return false;
        }

        public static bool Active { get; private set; }

        private static bool GamePadInputChanged()
        {
           
            return _currentState.PacketNumber != _oldState.PacketNumber;
        }

        public static void Update(float dt)
        {
            Active = false;

#if SWITCH
            

            GamePadState joyStat = GamePad.GetState(8);
            if (!_joyConConnected)
            {
                if (joyStat.IsConnected)
                {
                    Debugger.WriteLine($"Joycons Connected");
                    if (OnJoyConsAttached != null)
                        OnJoyConsAttached(8);
                }
            }

            _joyConConnected = joyStat.IsConnected;
#endif

            _oldState = _currentState;

            int tmp = 0;
            for (int i = 8; i >= 0; i--)
                if (GamePad.GetState(i).IsConnected)
                    tmp++;
            if (tmp != _numberOfConnectedControlers)
                GetGamePadIndex = true;
            _numberOfConnectedControlers = tmp;

            if (GetGamePadIndex)
            {
                for (int i = 8; i >= 0; i--)
                    if (CheckAllInputs(GamePad.GetState(i)))
                    {
                        _playerOne = i;
                        _connected = true;
                        GetGamePadIndex = false;
                        Debugger.WriteLine($"Controler {i} is now in controle");
                        return;
                    }
                _currentState = GamePad.GetState(0);
                return;
            }

            _currentState = GamePad.GetState(_playerOne);

            if (_connected)
            {
                if (GamePadInputChanged())
                    Active = true;
                if (!_currentState.IsConnected)
                {
                    _connected = false;
                    Debugger.WriteLine($"Controler {_playerOne} disconected");
                    if (OnControlerDisconect != null)
                        OnControlerDisconect(_playerOne);
                }
            }

            _rumbleDuration -= dt;

            if (_rumbleDuration < 0)
                GamePad.SetVibration(_playerOne, 0, 0);

        }

        public override void Update(GameTime gameTime)
        {
            Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        public static bool IsButtonDown(Buttons button)
        {
            if (IsNintendoSwitchControler)
                button = GetInputForNintendoSwitch(button);
            return _currentState.IsButtonDown(button);
        }

        public static bool DPadDirectionDown(DPadDirection direciton)
        {
            switch (direciton)
            {
                case Input.DPadDirection.Up:
                    {
                        Active = true;
                        return _currentState.DPad.Up == ButtonState.Pressed;
                    }
                    
                case Input.DPadDirection.Down:
                    {
                        Active = true;
                        return _currentState.DPad.Down == ButtonState.Pressed;
                    }
                    
                case Input.DPadDirection.Left:
                    {
                        Active = true;
                        return _currentState.DPad.Left == ButtonState.Pressed;
                    }

                case Input.DPadDirection.Right:
                    {
                        Active = true;
                        return _currentState.DPad.Right == ButtonState.Pressed;
                    }
            }
            return false;
        }

        public static bool DPadDirectionClicked(DPadDirection direciton)
        {
            switch (direciton)
            {
                case Input.DPadDirection.Up:
                    {
                        Active = true;
                        return _currentState.DPad.Up == ButtonState.Pressed && _oldState.DPad.Up == ButtonState.Released;
                    }

                case Input.DPadDirection.Down:
                    {
                        Active = true;
                        return _currentState.DPad.Down == ButtonState.Pressed && _oldState.DPad.Down == ButtonState.Released;
                    }

                case Input.DPadDirection.Left:
                    {
                        Active = true;
                        return _currentState.DPad.Left == ButtonState.Pressed && _oldState.DPad.Left == ButtonState.Released;
                    }

                case Input.DPadDirection.Right:
                    {
                        Active = true;
                        return _currentState.DPad.Right == ButtonState.Pressed && _oldState.DPad.Right == ButtonState.Released;
                    }
            }
            return false;
        }

        /// <summary>
        /// returns true if button is pressed or clicked
        /// </summary>
        /// <param name="button">the button to check</param>
        /// <param name="Clicked">wheter to look for click or press</param>
        /// <returns></returns>
        public static bool IsButton(Buttons button, bool Clicked)
        {
            if (Clicked)
                return IsButtonClick(button);
            return IsButtonDown(button);
        }

        public static bool IsButtonClick(Buttons button)
        {
            if (IsNintendoSwitchControler)
                button = GetInputForNintendoSwitch(button);
            return _currentState.IsButtonDown(button) && _oldState.IsButtonUp(button);
        }

        public static Vector2 ThumbStickLeft { get { return _currentState.ThumbSticks.Left; } }
        public static Vector2 ThumbStickRight { get { return _currentState.ThumbSticks.Left; } }

        private const float FlickThreshold = .2f;

        private static bool tmp;
        public static bool LeftThumbStickFlickLeft
        {
            get
            {
                tmp = _currentState.ThumbSticks.Left.X < -FlickThreshold && _oldState.ThumbSticks.Left.X >= -FlickThreshold;
                if (tmp)
                    Active = true;
                return tmp;
            }
        }

        public static bool LeftThumbStickFlickDown
        {
            get
            {
                tmp = _currentState.ThumbSticks.Left.Y < -FlickThreshold && _oldState.ThumbSticks.Left.Y >= -FlickThreshold;
                if (tmp)
                    Active = true;
                return tmp;
            }
        }

        public static bool LeftThumbStickFlickRight
        {
            get
            {
                tmp =  _currentState.ThumbSticks.Left.X > FlickThreshold && _oldState.ThumbSticks.Left.X <= FlickThreshold;
                if (tmp)
                    Active = true;
                return tmp;
            }
        }

        public static bool LeftThumbStickFlickUp
        {
            get
            {
                tmp =  _currentState.ThumbSticks.Left.Y > FlickThreshold && _oldState.ThumbSticks.Left.Y <= FlickThreshold;
                if (tmp)
                    Active = true;
                return tmp;
            }
        }

        private static float _rumbleDuration = 0;
        public static void SetRumble(float amountLeft, float amountRight, float duration)
        {
            amountLeft *= _masterFeedbackLevel;
            amountRight *= _masterFeedbackLevel;
            GamePad.SetVibration(_playerOne, amountLeft, amountRight);
            _rumbleDuration = duration;
        }
    }
}
