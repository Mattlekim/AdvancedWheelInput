using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Riddlersoft.Core.Input;

namespace Riddlersoft.Core
{
    public enum CameraMode { First_Person, RTS}

    /// <summary>
    /// represents a camera location and rotatial data
    /// </summary>
    public struct CameraLocation
    {
        /// <summary>
        /// camera position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// camera rotational data
        /// </summary>
        public float Yaw, Pitch, Roll;
    }


    public abstract class BasicCamera
    {
        public string Name;//nanme if canera

        public Matrix ViewMatrix { get; protected set; }
        public Matrix ProjectMatrix {get; protected set;}
        protected Rectangle _viewPort = new Rectangle(0, 0, 1280, 720);
        /// <summary>
        /// Gets the current viewport
        /// ====WARNING=====
        /// Do not set this variable unless you know exactly what you are doing
        /// This viewport will automaticly be updated on resoultions changes.
        /// </summary>
        public Rectangle CurrentViewPort {  get { return _viewPort; } set { _viewPort = value; } }


        public BasicEffect _effect;
#if ALPHAEFFECT
        public AlphaTestEffect _alpha_effect;
#endif
        public Vector3 Pos = new Vector3(0, 80, 0);
        public Vector3 Vel = Vector3.Zero;

        protected float AspectRatio = 1;

        /// <summary>
        /// the amount of zoom the camer will apply when zooming in or out
        /// </summary>
        protected float _zoomStep = .1f;

        public GraphicsDevice _device;

        public BoundingBox bouncingBox;

        public CameraMode Mode = CameraMode.RTS;

        public Point ScreenCenter = new Point(640, 360);

        public bool Lock = false;

       
        public BasicCamera(GraphicsDevice device, bool enabledlighting)
        {
            _device = device;

            _viewPort.Width = ScreenOldMethord.ActualResolution.Width;
            _viewPort.Height = ScreenOldMethord.ActualResolution.Height;
            ScreenCenter.X = Convert.ToInt32(ScreenOldMethord.Center.X);
            ScreenCenter.Y = Convert.ToInt32(ScreenOldMethord.Center.Y);

            bouncingBox = new BoundingBox(Vector3.Zero, Vector3.One);
            _effect = new BasicEffect(device);
            _effect.VertexColorEnabled = true;
            _effect.TextureEnabled = true;


#if ALPHAEFFECT
            _alpha_effect = new AlphaTestEffect(device);
            _alpha_effect.VertexColorEnabled = true;
#endif
            
            ViewMatrix = Matrix.CreateLookAt(Pos, Vector3.Zero, Vector3.Up);
            ProjectMatrix = Matrix.CreateOrthographicOffCenter(0, _viewPort.Width, _viewPort.Height,
            0, 1.0f, 1000.0f);
            RasterizerState ts = new RasterizerState();
            ts.CullMode = CullMode.CullCounterClockwiseFace;
            device.RasterizerState = ts;
            _effect.View = ViewMatrix;
            _effect.Projection = ProjectMatrix;
            _effect.World = Matrix.Identity;
            Mouse.SetPosition((int)ScreenCenter.X, (int)ScreenCenter.Y);
            _effect.FogColor = new Vector3((float)Color.CornflowerBlue.R / 255f, (float)Color.CornflowerBlue.G / 255f, (float)Color.CornflowerBlue.B / 255f);
            //_effect.FogEnd = 90;
            //_effect.FogStart = 70;

            _effect.FogEnd = 160;
            _effect.FogStart = 140;
            _effect.FogEnabled = true;
#if ALPHAEFFECT
            _alpha_effect.ReferenceAlpha = 0;
            _alpha_effect.AlphaFunction = CompareFunction.Equal;
#endif

            AspectRatio = (float)_viewPort.Width / (float)_viewPort.Height;

           // if (enabledlighting)
              //  _effect.EnableDefaultLighting();
         //   _effect.LightingEnabled = true;
           // _effect.AmbientLightColor = new Vector3(1, 1, 1);
            
        }

        public BasicCamera(GraphicsDevice device, bool enabledlighting, Point customsize)
        {
            _device = device;

            _viewPort.Width = customsize.X;
            _viewPort.Height = customsize.Y;
            ScreenCenter.X = Convert.ToInt32(customsize.X / 2f);
            ScreenCenter.Y = Convert.ToInt32(customsize.Y / 2f);

            bouncingBox = new BoundingBox(Vector3.Zero, Vector3.One);
            _effect = new BasicEffect(device);
            _effect.VertexColorEnabled = true;
            _effect.TextureEnabled = true;
#if ALPHAEFFECT
            _alpha_effect = new AlphaTestEffect(device);
            _alpha_effect.VertexColorEnabled = true;
#endif
            ViewMatrix = Matrix.CreateLookAt(Pos, Vector3.Zero, Vector3.Up);
            ProjectMatrix = Matrix.CreateOrthographicOffCenter(0, _viewPort.Width, _viewPort.Height,
            0, 1.0f, 1000.0f);
            RasterizerState ts = new RasterizerState();
            ts.CullMode = CullMode.CullCounterClockwiseFace;
            device.RasterizerState = ts;

            _effect.View = ViewMatrix;
            _effect.Projection = ProjectMatrix;
            _effect.World = Matrix.Identity;

            Mouse.SetPosition((int)ScreenCenter.X, (int)ScreenCenter.Y);
            _effect.FogColor = new Vector3((float)Color.CornflowerBlue.R / 255f, (float)Color.CornflowerBlue.G / 255f, (float)Color.CornflowerBlue.B / 255f);
            //_effect.FogEnd = 90;
            //_effect.FogStart = 70;

            _effect.FogEnd = 160;
            _effect.FogStart = 140;
            _effect.FogEnabled = true;
#if ALPHAEFFECT
            _alpha_effect.ReferenceAlpha = 0;
            _alpha_effect.AlphaFunction = CompareFunction.Equal;
#endif

            AspectRatio = (float)_viewPort.Width / (float)_viewPort.Height;

            // if (enabledlighting)
            //  _effect.EnableDefaultLighting();
            //   _effect.LightingEnabled = true;
            // _effect.AmbientLightColor = new Vector3(1, 1, 1);

        }

        protected bool _isLerping = false;
        public void Lerp(CameraLocation start, CameraLocation end, float amount)
        {
            _isLerping = true;
            Pos.X = start.Position.X + amount * (end.Position.X - start.Position.X);
            Pos.Y = start.Position.Y + amount * (end.Position.X - start.Position.Y);
            Pos.Z = start.Position.Z + amount * (end.Position.X - start.Position.Z);
        }

        public void Rotate(float xrot, float yrot)
        {
            pitch += yrot;
            pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + .0001f, MathHelper.PiOver2 - .0001f);
            yaw += xrot;
        }

        protected bool _rotateCamera = false;
        protected Vector3 _rotationPoint = Vector3.Zero; //used as a not set flag
        protected Vector2 _rotationData; //amount to rotate by
        protected Vector3 _rotationVectorToCenter;


        /// <summary>
        /// set the rotation point and start the camera rotating
        /// </summary>
        /// <param name="rotpoint">the point in the world to rotate around</param>
        public void SetRotationPoint(Vector3 rotpoint)
        {
            _rotateCamera = true;
            _rotationPoint = rotpoint;
            _rotationData = Vector2.Zero; //reset rotations angles to zero
            _rotationVectorToCenter = Pos - _rotationPoint; //get the look at vector
        }

        /// <summary>
        /// set the rotation point and start the camera rotating
        /// </summary>
        /// <param name="rotpoint">the point in the world to rotate around</param>
        public void SetRotationPoint(Vector3 rotpoint, bool relative)
        {
            if (relative)
                rotpoint = Vector3.Transform(rotpoint, lookmatrix);
            _rotateCamera = true;
            _rotationPoint = rotpoint;
            _rotationData = Vector2.Zero; //reset rotations angles to zero

            if (relative)
                _rotationVectorToCenter = Pos - _rotationPoint; //get the look at vector
        }

        /// <summary>
        /// clear the rotation point and stop camera rotating
        /// </summary>
        public void StopRotationPoint()
        {
            _rotateCamera = false;
            _rotationPoint = Vector3.Zero;
            _rotationData = Vector2.Zero;
        }


        /// <summary>
        /// will rotate the cameara around a given point
        /// </summary>
        /// <param name="amount">amount of rotation</param>
        public void RotateAroundPoint(float xamont, float yamount)
        {
#if DEBUG
            if (!_rotateCamera)
                throw new Exception("Not intialized Rotation Point");
#endif
            _rotationData.X += xamont;
            _rotationData.Y += yamount;
        }

        /// <summary>
        /// stop the camera rotating around a point
        /// </summary>
        public void StopRotateingAroundPoint()
        {
            _rotateCamera = false;
        }
        KeyboardState kb;

        protected MouseState mouse;
        //z = roll, y = yaw, x = pitch
        protected float roll = 0, pitch = .65f, yaw = .45f;

        protected ButtonState MousePressed = ButtonState.Released;
        public ButtonState LeftButtonPress { get { return MousePressed; } }
        protected ButtonState MousePressedR = ButtonState.Released;
        public Vector2 mousePos;
        protected Vector2 lastmousepos;
        protected Vector3 LookVector = new Vector3(.33f, -.53f, -.33f);

        //the internal direction the camera is poiting at
        public Vector3 LookAt;

        /// <summary>
        /// the direction the camera is pointing
        /// </summary>
        public Vector3 Direction { get { return LookAt; } }
        public Vector3 StrafAt;
        protected bool resetmouse = true;
        public float FOV = MathHelper.PiOver2;
        protected Matrix lookmatrix;
        KeyboardState lkb;
        

        public Ray GetMouseRay()
        {

            Vector2 pos = MouseTouch.Position;
            if (Mode == CameraMode.First_Person)
                pos = new Vector2(ScreenCenter.X, ScreenCenter.Y);
            Vector3 nearPoint = new Vector3(pos, 0);
            Vector3 farPoint = new Vector3(pos, 1);

            nearPoint = _device.Viewport.Unproject(nearPoint, ProjectMatrix, ViewMatrix, Matrix.Identity);
            farPoint = _device.Viewport.Unproject(farPoint, ProjectMatrix, ViewMatrix, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        public Ray GetMouseRay(Vector2 mouseoveride)
        {

            Vector2 pos = mouseoveride;
            if (Mode == CameraMode.First_Person)
                pos = new Vector2(ScreenCenter.X, ScreenCenter.Y);
            Vector3 nearPoint = new Vector3(pos, 0);
            Vector3 farPoint = new Vector3(pos, 1);

            nearPoint = _device.Viewport.Unproject(nearPoint, ProjectMatrix, ViewMatrix, Matrix.Identity);
            farPoint = _device.Viewport.Unproject(farPoint, ProjectMatrix, ViewMatrix, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        public Ray OldRay;
        public Vector3 RayCollisionPoint;
        public float speed = 0.4f;

        /// <summary>
        /// set the exact camera rotation
        /// </summary>
        /// <param name="rots">x = yaw, y = pitch, z = roll</param>
        public void SetRotation(Rotation3 rots)
        {
            pitch = rots.Pitch;
            yaw = rots.Yaw;
            roll = rots.Roll;
        }

        public void OnResolutionChange(int width, int height)
        {
            _viewPort.Width = width;
            _viewPort.Height = height;
            ScreenCenter = new Point(width / 2, height / 2);
            AspectRatio = (float)width / (float)height;
        }

        protected void UpdateRTSCam(float dt)
        {
            Vector2 moveAmount = Vector2.Zero;

            //rotation around point code
            if (_rotateCamera)
            {
                //we need to get the anlge to the look vector
                //first get the vector

                 Vector3 rotationLookVector = Vector3.Transform(_rotationVectorToCenter, Matrix.CreateRotationY(_rotationData.X));

              //  Vector3 rotationLookVector = Vector3.Transform(_rotationVectorToCenter, Matrix.CreateRotationZ(_rotationData.Y));
                //_rotationData.X = 0;

                //now we need to calculate angle of look on pitch axis

                yaw = (float)Math.Atan2(-rotationLookVector.X, -rotationLookVector.Z);
             //   pitch = (float)Math.Atan2(-rotationLookVector.Z, -rotationLookVector.Y);
                Pos = _rotationPoint + rotationLookVector;
            }

            if (_currentZoomTime > 0)
            {
                Pos = Vector3.Lerp(Pos, _targetZoom, .2f - _currentZoomTime);
                float per = (float)(Pos.Y - MaxZoom) / (float)(MinZoom - MaxZoom);
                pitch = MathHelper.Lerp(ZoomedInAngle , ZoomedOutAngle, per);
                _currentZoomTime -= dt;
                if (_currentZoomTime < 0)
                    _currentZoomTime = 0;
            }
            Pos += Vel;
        }

        public virtual void Update(float dt)
        {
            _effect.World = Matrix.Identity;
#if ALPHAEFFECT
            _alpha_effect.World = Matrix.Identity;
#endif
            if (_isLerping)
            {
                lookmatrix = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);
                LookAt = Vector3.Transform(LookVector, lookmatrix);
                StrafAt = Vector3.Transform(LookVector, Matrix.CreateRotationY(yaw - MathHelper.PiOver2));
                ViewMatrix = Matrix.CreateLookAt(Pos, Pos + LookAt, Vector3.Up);
                ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(FOV, AspectRatio, .1f, 10000);
                _effect.View = ViewMatrix;
                _effect.Projection = ProjectMatrix;
#if ALPHAEFFECT
                _alpha_effect.View = ViewMatrix;
                _alpha_effect.Projection = ProjectMatrix;
#endif
                
                _isLerping = false;
                return;
            }



            lkb = kb;
            kb = Keyboard.GetState();

            if (resetmouse)
            {

                mouse = Mouse.GetState();
                mousePos = new Vector2(mouse.X, mouse.Y);

                LookVector = Vector3.Zero;
                LookVector.Z = 1;
                lookmatrix = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);
                LookAt = Vector3.Transform(LookVector, lookmatrix);
                StrafAt = Vector3.Transform(LookVector, Matrix.CreateRotationY(yaw - MathHelper.PiOver2));
                ViewMatrix = Matrix.CreateLookAt(Pos, Pos + LookAt, Vector3.Up);
                //_projectMatrix = Matrix.CreateOrthographicOffCenter(0, ViewPort.Width, ViewPort.Height, 0, 1.0f, 1000.0f);
                ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(FOV, AspectRatio, .1f, 10000);
                _effect.View = ViewMatrix;
                _effect.Projection = ProjectMatrix;
#if ALPHAEFFECT
                _alpha_effect.View = ViewMatrix;
                _alpha_effect.Projection = ProjectMatrix;
#endif
                // if (_type == CameraType.First_Person)
                //   Mouse.SetPosition((int)ScreenCenter.X,(int)ScreenCenter.Y);

                if (!Lock)
                {
                    if (mouse.LeftButton == ButtonState.Pressed && MousePressed == ButtonState.Released)
                    {
                        LeftClick();
                        //int tmp = GetClickedSector(true);
                    }

                    if (mouse.RightButton == ButtonState.Pressed && MousePressedR == ButtonState.Released)
                    {
                        OnRightClick();
                        //  int tmp = GetClickedSector(false);
                        //if (tmp != -1)
                        // {
                        //   GetClickedBlock(Engine.Engine.Sectors[tmp]);
                        //Engine.Sectors[tmp].HaveCalculatedGeomotry = false;
                        //}
                    }
                    MousePressed = mouse.LeftButton;
                    MousePressedR = mouse.RightButton;
                }
            }

            if (!Lock)
                UpdateRTSCam(dt);


        }

        protected virtual void LeftClick()
        {
#if DEBUG
            OldRay = GetMouseRay();
#endif
        }

        protected virtual void OnRightClick()
        {
        }

        public void Move(Vector3 amount)
        {
            Pos += amount;
        }

        public void MoveInPerspective(Vector3 amount)
        {
            Pos += amount.X * StrafAt;
            Pos.Y += amount.Y;
        }
  
        /// <summary>
        /// retruns the camera location and rotation values
        /// </summary>
        /// <returns>a camera location structure</returns>
        public CameraLocation GetLookAt()
        {
            CameraLocation loc = new CameraLocation();
            loc.Pitch = this.pitch;
            loc.Roll = this.roll;
            loc.Yaw = this.yaw;
            loc.Position = this.Pos;
            return loc;
        }

        protected const float TimeToCompleateZoom = .2f;
        protected Vector3 _targetZoom;
        protected float _currentZoomTime;
        
        protected const int MaxZoom = 70, MinZoom = 90;
        protected const float ZoomedOutAngle = 1.2f, ZoomedInAngle = .5f;

        public void Zoom(float amount)
        {
            Vector3 newzoom = Vector3.Zero;

            if (_currentZoomTime <= 0) //if we are not zooming
                newzoom = Pos;

            newzoom += LookAt * amount * Pos.Y * _zoomStep * .005f;
            if (newzoom.Y > MaxZoom && newzoom.Y < MinZoom)
            {
                _targetZoom = newzoom;

                _currentZoomTime = TimeToCompleateZoom;
            }
        }

        //=========================LIGHTING CODE==================================================

        public void EnableLighting()
        {
            _effect.AmbientLightColor = new Vector3(20,0,0);
            _effect.EnableDefaultLighting();
            _effect.VertexColorEnabled = true;
        //    _effect.PreferPerPixelLighting = true;
          //  _effect.LightingEnabled = true; // turn on the lighting subsystem.
            //_effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 0, 0); // a red light
           // _effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);  // coming along the x-axis
           // _effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights
            //_effect.DirectionalLight0.Enabled = true;
            //_effect.
        }

        public void DisableLighting()
        {

        }
    
    }
}
