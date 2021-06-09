using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Riddlersoft.Core
{
    public class ScreenOldMethord
    {
        /// <summary>
        /// the resualution of the screen
        /// </summary>
        private static Rectangle _actualResolution;
        public static Rectangle ActualResolution { get { return _actualResolution; } }

        public static Rectangle NativeResolution { get; private set; }

        public static Vector2 ScaleAmount { get; protected set; }

        private static GraphicsDeviceManager _device;
        public static GraphicsDeviceManager GraphicsDeviceManager { get { return _device; } }

        public delegate void _OnResChange(int width, int height);
        public static event _OnResChange OnResChange;

        private static Vector2 _center;
        public static Vector2 Center { get { return _center; } }

        private static GameWindow _window;

        public static bool IsIntalized { get; protected set; } = false;

        public static void SetGPU(GraphicsDeviceManager device, GameWindow window)
        {
            _device = device;
            _window = window;
            IsIntalized = true;

            _center.X = (float)device.PreferredBackBufferWidth * .5f;
            _center.Y = (float)device.PreferredBackBufferHeight * .5f;

            _actualResolution.X = 0;
            _actualResolution.Y = 0;
            _actualResolution.Width = device.PreferredBackBufferWidth;
            _actualResolution.Height = device.PreferredBackBufferHeight;
            NativeResolution = new Rectangle(0, 0, 1280, 720);
        }

        public static void SetGPU(GraphicsDeviceManager device, GameWindow window, Rectangle nativeResolution)
        {
            _device = device;
            _window = window;
            IsIntalized = true;

            _center.X = (float)device.PreferredBackBufferWidth * .5f;
            _center.Y = (float)device.PreferredBackBufferHeight * .5f;

            _actualResolution.X = 0;
            _actualResolution.Y = 0;
            _actualResolution.Width = device.PreferredBackBufferWidth;
            _actualResolution.Height = device.PreferredBackBufferHeight;

            NativeResolution = nativeResolution;
        }

        private static void CalculateScaling()
        {
            ScaleAmount = new Vector2((float)NativeResolution.Width / (float)_actualResolution.Width,
                (float)NativeResolution.Width / (float)_actualResolution.Width);
        }

        public static void SetResolution(int width, int height, int mode)
        {
#if SWITCH
//            _center.X = (float)width * .5f;
  //          _center.Y = (float)height * .5f;

    //        _actualResolution.X = 0;
      //      _actualResolution.Y = 0;
        //    _actualResolution.Width = width;
          //  _actualResolution.Height = height;

            //CalculateScaling();
            //return;
#endif
            _device.PreferredBackBufferWidth = width;
            _device.PreferredBackBufferHeight = height;

            if (mode > 0 )
            {
                _device.IsFullScreen = false;
#if !WINDOWS_UWP
                if (mode == 1)
                    _window.IsBorderless = false;
                else
                    _window.IsBorderless = true;
#endif
            }
            else
                _device.IsFullScreen = true;

            _device.ApplyChanges();

            if (OnResChange != null)
                OnResChange(width, height);
            _center.X = (float)width * .5f;
            _center.Y = (float)height * .5f;

            _actualResolution.X = 0;
            _actualResolution.Y = 0;
            _actualResolution.Width = width;
            _actualResolution.Height = height;

            CalculateScaling();
        }

     
    }
}
