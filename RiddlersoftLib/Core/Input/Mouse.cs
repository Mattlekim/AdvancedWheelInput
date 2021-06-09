using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Riddlersoft.Core.Input
{


    public class MouseTouch
    {
        public enum DragStatus { None, DetectedHold, Active }



        private const float ClickDetectionInterval = .2f;

        private static float _lClickTimer;
        /// <summary>
        /// the world postion of the mouse
        /// </summary>
        public static Vector2 RealGamePos { get; protected set; }

        public static Vector2 DragSpeed { get; protected set; }
        /// <summary>
        /// the minimum amout of distance the mouse needs to move in order to detect a mouse drag function
        /// </summary>
        private const int DragMinimumMoveThreshold = 5;

        /// <summary>
        /// the mouse states
        /// </summary>
        static MouseState _mouse, _lmouse;

        public static Vector2 MouseMoveAmount { get; private set; }

        public static bool SWheelDown
        {
            get
            {
                if (_scrollAmount > 0)
                    return true;
                return false;
            }
        }

        public static bool SWheelUp
        {
            get
            {
                if (_scrollAmount < 0)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// the current mouse state
        /// </summary>
        public static MouseState MouseS { get { return _mouse; } }

        /// <summary>
        /// the last mouses state
        /// </summary>
        public static MouseState LastMouseS { get { return _lmouse; } }

        public delegate void _MouseClickEvent(Vector2 Location, Ray mouseray);
        /// <summary>
        /// first when the left mouse is clicked
        /// _LeftMouseClick(Vector2 Location, Ray mouseray);
        /// </summary>
        public static event _MouseClickEvent OnLeftMouseClick;

        public static Action<Vector2> OnLeftMouseDown;

        public static Action<Vector2> OnLeftMouseRelease;

        private static bool _leftButtonHold = false;
        public static bool LeftButtonHold { get { return _leftButtonHold; } }

        public static bool LeftButtonReleased { get; private set; }
        private static bool _rightButtonHold = false;
        public static bool RightButtonHold { get { return _rightButtonHold; } }

        //public delegate void _RightMouseClickR(Vector2 Location, Ray r);
        public static event _MouseClickEvent OnRightMouseClick;

        public delegate void _MouseScroll(int Amount);
        public static event _MouseScroll OnMouseScroll;

        private static int _scrollAmount;

        /// <summary>
        /// the amount the mouse wheel has scrolled this turn
        /// </summary>
        public static int ScrollAmount { get { return _scrollAmount; } }

        /// <summary>
        /// this event fires when the player starts draging the mouse
        /// </summary>
        /// <param name="start"></param>
        public delegate void _OnDragStart(Vector2 start, Ray r);
        public static event _OnDragStart OnDragStart;

        /// <summary>
        /// this even will fire when the player ends draging the mouse
        /// </summary>
        /// <param name="end"></param>
        public delegate void _OnDragEnd(Vector2 end, Ray r);
        public static event _OnDragEnd OnDragEnd;

        public static Action<Rectangle> OnDrag;

        public static Vector2 LastPosition { get; protected set; }

        protected static Vector2 _position;
        public static Vector2 Position { get { return _position; } }

        public static Vector2 LastLiteralPosition { get; private set; }
        public static Vector2 LiteralPosition { get; private set; }
        public static Vector2 ScaledMousePosition { get; protected set; }

        private static Ray _mouseRay; //the mouse ray
        public static Ray MouseRay { get { return _mouseRay; } }

        private static bool lastRotateCamState = false;
        private static bool RotateCam = false; //if to rotate the camera or not
        public static bool AllowRotationOnZAxis = false;
        public static bool CameraRotaingMode { get { return RotateCam; } }

        private static bool _lButtonClick;
        /// <summary>
        /// is true if the left mouse button has been clicked
        /// </summary>
        public static bool LButtonClick { get { return _lButtonClick; } }

        public static bool LbuttonDown { get; protected set; }
        private static bool _rButtonClick;
        /// <summary>
        /// is true if the left mouse button has been clicked
        /// </summary>
        public static bool RButtonClick { get { return _rButtonClick; } }

        public static bool RbuttonDown { get; protected set; }

        public static bool RButtonReleased { get; protected set; }


        public static bool MButtonDown { get; protected set; }
        public static bool MButtonClick { get; protected set; }
        public static bool MButtonRelesed { get; protected set; }

        private static Vector2 _endDragPos = Vector2.Zero;

        private static Vector2 _intialDragPos = Vector2.Zero;
        /// <summary>
        /// The start position of the mouse drag
        /// </summary>
        public static Vector2 IntialDragPos { get { return _intialDragPos; } }

        public static Vector2 IntialClickPostion { get; private set; }

        private static Vector2 _currentDragPos = Vector2.Zero;
        /// <summary>
        /// the current position of the mouse drag
        /// </summary>
        private static Vector2 CurrentDragPos { get { return _currentDragPos; } }

        public static bool TouchEnabled { get; private set; } = false;



        public static Texture2D CorssHair;
        public static Vector2 CrossHairCenter;
        public static Texture2D Dot;

        public static TouchCollection TouchPoints { get; private set; }

        private static DragEvent _drag;
        public static DragEvent Drag { get { return _drag; } }
        private static DragStatus _dragStatus;

        public static bool FlickLeft { get; private set; }
        public static bool FlickRight { get; private set; }
        public static bool FlickUp { get; private set; }
        public static bool FlickDown { get; private set; }

        public static bool Active = false;

        public static bool DoubleTap { get; private set; }
        /// <summary>
        /// this function enables touch controles
        /// </summary>
        public static void EnableTouch()
        {
            TouchEnabled = true;
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag | GestureType.Flick | GestureType.DoubleTap;
        }

        public static void Reset()
        {
            _scrollAmount = 0; //reset the wheel value
            LastLiteralPosition = LiteralPosition;
            LastPosition = _position;
            _lmouse = _mouse; //get the old mouse state
            _mouse = Mouse.GetState(); //get the mouse state
            _position = new Vector2(_mouse.X, _mouse.Y); //get postion of mouse
            LiteralPosition = _position;
            _position = _position / _scaleBy;
            if (!Active)
                _position = new Vector2(-1, -1);
            _lButtonClick = false;
            _rButtonClick = false;
            _leftButtonHold = false;
            _rightButtonHold = false;
            _rButtonClick = false;

            LbuttonDown = MouseS.LeftButton == ButtonState.Pressed;
            RbuttonDown = MouseS.RightButton == ButtonState.Pressed;
            FlickDown = false;
            FlickUp = false;
            FlickLeft = false;
            FlickRight = false;
            DoubleTap = false;
            _drag = DragEvent.Empty;
        }

        private static Vector2 _initalClickPosition;
        private static Vector2 _lastMousePostion;

        public static Vector2 Camera = Vector2.Zero;

        public static bool CanDrag = true;

        public static void ClearDrag()
        {
            _dragStatus = DragStatus.None;
            
        }

        private static void PostUpdate(float dt)
        {
           
            Reset();

            if (MathHelper.Distance(MouseS.Position.X, LastMouseS.X) > 4 || MathHelper.Distance(MouseS.Y, LastMouseS.Y) > 4)
                Active = true;

            if (TouchEnabled) //if touch screen is enabled
            {
                TouchPoints = TouchPanel.GetState();
                
                GestureSample gs; //tmp varialbe for the gestors
                while (TouchPanel.IsGestureAvailable) //if the is a gesture to process
                {
                    gs = TouchPanel.ReadGesture(); //get the gesture
                    switch (gs.GestureType) //check the type
                    {
                        case GestureType.DoubleTap:
                            DoubleTap = true;
                            break;

                        case GestureType.Tap:
                            _lButtonClick = true; //set clicked flag to true
                            _position = gs.Position; //set the mouse pos
                            LiteralPosition = _position;
                            _position = _position / _scaleBy;
                            LbuttonDown = true;
                            break;

                        case GestureType.FreeDrag:
                            _drag = new DragEvent(gs.Delta, gs.Position, true);
                            break;

                        case GestureType.Flick:
                            if (Math.Abs(gs.Delta.X) > Math.Abs(gs.Delta.Y)) //if horizontal flick
                            {
                                if (gs.Delta.X > 0)//if right
                                    FlickRight = true;
                                else
                                    FlickLeft = true;
                            }
                            else //if vertical flick
                            {
                                if (gs.Delta.Y > 0) //if up
                                    FlickUp = true;
                                else
                                    FlickDown = true;
                            }
                            break;
                    }
                }
            }

            if (LbuttonDown)
                if (OnLeftMouseDown != null)
                    OnLeftMouseDown(Position);

            if (MouseS.LeftButton == ButtonState.Released && LastMouseS.LeftButton == ButtonState.Pressed)
            {
                if (OnLeftMouseRelease != null)
                    OnLeftMouseRelease(Position);
               
            }
            if (CanDrag)
            {
                if (MouseS.LeftButton == ButtonState.Released || _lmouse.LeftButton == ButtonState.Released)
                {
                    if (_dragStatus == DragStatus.DetectedHold) //if active drag
                    {
                        if (Vector2.Distance(_intialDragPos, _endDragPos) > DragMinimumMoveThreshold)
                            if (OnDrag != null)
                                OnDrag(new Rectangle((int)_intialDragPos.X, (int)_intialDragPos.Y, (int)(_endDragPos.X - _intialDragPos.X), (int)(_endDragPos.Y - _intialDragPos.Y)));
                    }

                    _dragStatus = DragStatus.None; //reset the drag as there is no drag
                }

                if (_dragStatus == DragStatus.DetectedHold)
                {
                    if (Vector2.Distance(_intialDragPos, LastPosition) > DragMinimumMoveThreshold)
                        _drag = new DragEvent((_mouse.Position - _lmouse.Position).ToVector2(), _lmouse.Position.ToVector2(), true);

                    _endDragPos = Position + Camera;
                }
            }
            else
                _dragStatus = DragStatus.None;

            //first start left click detection
            if (_mouse.LeftButton == ButtonState.Pressed && _lmouse.LeftButton == ButtonState.Released)
            {
                _lClickTimer = 0;
                _initalClickPosition = _position;
                IntialClickPostion = Position;
            }

                if (_mouse.LeftButton == ButtonState.Pressed && LastMouseS.LeftButton == ButtonState.Pressed)
            {
                
                _lClickTimer += dt;
                _leftButtonHold = true;
            }

            //on release
            if (_mouse.LeftButton == ButtonState.Released && _lmouse.LeftButton == ButtonState.Pressed)
            {
                if (_lClickTimer < ClickDetectionInterval && !_rightButtonHold && !_drag.Active)
                    if (Vector2.Distance(_position, _initalClickPosition) < 8)
                        _lButtonClick = true;
            }

          

            if (_mouse.RightButton == ButtonState.Pressed && _lmouse.RightButton == ButtonState.Released && _mouse.LeftButton == ButtonState.Released)
            {
                _rButtonClick = true;

            }

            LeftButtonReleased = _mouse.LeftButton == ButtonState.Released && _lmouse.LeftButton == ButtonState.Pressed;

            if (_mouse.LeftButton == ButtonState.Pressed && _lmouse.LeftButton == ButtonState.Pressed)
            {
                _leftButtonHold = true;
                if (_dragStatus == DragStatus.None) //if start of drag
                {
                    _dragStatus = DragStatus.DetectedHold;
                    _intialDragPos = Position + Camera;
                }
            }

            if (_mouse.RightButton == ButtonState.Pressed && _lmouse.RightButton == ButtonState.Pressed)
                _rightButtonHold = true;


            RButtonReleased = _mouse.RightButton == ButtonState.Released && _lmouse.RightButton == ButtonState.Pressed;

            MButtonClick = _mouse.MiddleButton == ButtonState.Pressed && _lmouse.MiddleButton == ButtonState.Released;
            MButtonDown = _mouse.MiddleButton == ButtonState.Pressed && _lmouse.MiddleButton == ButtonState.Pressed;
            MButtonRelesed = _mouse.MiddleButton == ButtonState.Released && _lmouse.MiddleButton == ButtonState.Pressed;

            if (_mouse.ScrollWheelValue != _lmouse.ScrollWheelValue)
            {
                _scrollAmount = (_mouse.ScrollWheelValue - _lmouse.ScrollWheelValue) / 120;
                if (OnMouseScroll != null)
                {
                    OnMouseScroll(_mouse.ScrollWheelValue - _lmouse.ScrollWheelValue);
                }
            }




           
         
        }

        private static Vector2 _scaleBy = new Vector2(1,1);
        public static void SetScale(Vector2 scale)
        {
            _scaleBy = scale;
        }

        public static void Update()
        {
            if (!IsActivated) //if the mouse is not activate exit
                return;

            PostUpdate(.016f);
            if (_lButtonClick)
            {
                if (OnLeftMouseClick != null)
                    OnLeftMouseClick(_position, new Ray());
            }

            if (_rButtonClick)
            {
                if (OnRightMouseClick != null)
                    OnRightMouseClick(_position, new Ray());
            }

            MouseMoveAmount = Position - _lastMousePostion;
            _lastMousePostion = Position;
        }

        public static void Update(Vector2 pos)
        {
            if (!IsActivated) //if the mouse is not activate exit
                return;

            PostUpdate(.016f);
            if (_lButtonClick)
            {
                if (OnLeftMouseClick != null)
                    OnLeftMouseClick(_position, new Ray());
            }

            if (_rButtonClick)
            {
                if (OnRightMouseClick != null)
                    OnRightMouseClick(_position, new Ray());
            }

        }


        /// <summary>
        /// main update for the mouse
        /// </summary>
        /// <param name="gameCamera">needs a 3d camera for relative positions</param>
        public static void Update(BasicCamera gameCamera)
        {
            if (gameCamera == null)
                throw new Exception("Game camera cannot be null");

            if (!IsActivated) //if the mouse is not activate exit
                return;

           
            PostUpdate(.016f);

            lastRotateCamState = RotateCam; //set last rote
            if (_lButtonClick)
            {
                _mouseRay = gameCamera.GetMouseRay();
                IntialClickPostion = _position;

                if (OnLeftMouseClick != null)
                    OnLeftMouseClick(_position, _mouseRay);
            }

            if (_rButtonClick)
            {
                _mouseRay = gameCamera.GetMouseRay();
                if (OnRightMouseClick != null)
                    OnRightMouseClick(_position, _mouseRay);
            }

            if (gameCamera != null) //if no camear is pressent we can not rotate cam
            {
                //code for rotating mouse
                if (KeyboardAPI.IsKeyDown(Keys.LeftControl) & KeyboardAPI.IsKeyDown(Keys.LeftShift) || //for use on laptops
                //where there is no middle butotn
                _mouse.MiddleButton == ButtonState.Pressed && _lmouse.MiddleButton == ButtonState.Pressed) //otherwise use middle button
                    RotateCam = true;
                else
                    RotateCam = false;
            }
        }

        private static bool IsActivated = true;
        public static void Activate()
        {
            IsActivated = true;
        }

        public static bool IsLeftClick()
        {
            if (MouseS.LeftButton == ButtonState.Pressed && LastMouseS.LeftButton == ButtonState.Released)
                return true;
            else
                return false;
        }


        public static bool IsRightClick()
        {
            if (MouseS.RightButton == ButtonState.Pressed && LastMouseS.RightButton == ButtonState.Released)
                return true;
            else
                return false;
        }

        public static void Deactivate()
        {
         //   IsActivated = false;
        }


        private static bool DrawMouse = false;


        private static void DrawRectangle(SpriteBatch sb, Vector2 start, Vector2 end, int boarder, Color col)
        {
            float tmp;
            if (start.X > end.X)
            {
                tmp = start.X;
                start.X = end.X;
                end.X = tmp;
            }

            if (start.Y > end.Y)
            {
                tmp = start.Y;
                start.Y = end.Y;
                end.Y = tmp;
            }

            sb.Draw(Dot, new Rectangle((int)start.X, (int)start.Y, (int)(end.X - start.X), boarder), col);
            sb.Draw(Dot, new Rectangle((int)start.X, (int)end.Y - boarder, (int)(end.X - start.X), boarder), col);

            sb.Draw(Dot, new Rectangle((int)start.X, (int)start.Y, (int)boarder, (int)(end.Y - start.Y)), col);
            sb.Draw(Dot, new Rectangle((int)end.X - boarder, (int)start.Y, boarder, (int)(end.Y - start.Y)), col);
        }

        public static void Render(ref SpriteBatch sb)
        {
            if (DrawMouse)
                sb.Draw(CorssHair, _position - CrossHairCenter, Color.White);

            if (Drag.Active)
            {
                DrawRectangle(sb, _intialDragPos - Camera, _endDragPos - Camera, 2, Color.Black);
            }
        }

        public static void SetPos(int x, int y)
        {
            if (IsActivated)
            {
                _position.X = x;
                _position.Y = y;
                
                Mouse.SetPosition(x, y);
            }
        }

        public static void LoadContent(ContentManager content)
        {
            CorssHair = content.Load<Texture2D>("crosshair");
            CrossHairCenter = new Vector2((float)CorssHair.Width * .5f,
                (float)CorssHair.Height * .5f);


        }
    }
}
