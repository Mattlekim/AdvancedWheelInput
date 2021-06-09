using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
namespace Riddlersoft.Core.Screen
{
    public class ScreenManiger: GameComponent
    {
        public static ScreenFormatter Format { get; private set; }
        public static Action<ScreenFormatter> OnResolutionChange;

        public static bool IsInitalized { get; private set; } = false;

        internal int VirtualResolutionWidth = -1, VirtualResolutionHeight = -1;

        internal int ScreenResolutionWidth = -1, ScreenResolutionHeight = -1;

        private bool _resolutionSet = false;


        public ScreenManiger(Game game) : base(game)
        {
            if (IsInitalized)
                throw new Exception("You can only implement on screen maniger!");
            IsInitalized = true;

         
            game.Window.ClientSizeChanged += Window_ClientSizeChanged;
            Format = new Core.Screen.ScreenFormatter(this);
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Screen Resolution Changed");
        }
      

        public void SetResolution(int screenWidth, int screenHight,
            int virtualWidth, int virtualHeight)
        {
            ScreenResolutionWidth = screenWidth;
            ScreenResolutionHeight = screenHight;

            VirtualResolutionWidth = virtualWidth;
            VirtualResolutionHeight = virtualHeight;

            Format = new Core.Screen.ScreenFormatter(this);
            if (OnResolutionChange != null)
                OnResolutionChange(Format);
            _resolutionSet = true;

#if DEBUG
            Console.WriteLine($"Reslutoin set: {Format}");
#endif
        }

      

       
       

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
#if DEBUG
            if (!_resolutionSet)
                throw new Exception("Resolution must be set");

#endif
            base.Update(gameTime);
        }

        
    }
}
