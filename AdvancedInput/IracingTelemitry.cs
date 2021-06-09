using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using iRacingSDK;

namespace AdvancedInput
{
    class IRacingTelemitry : GameComponent
    {
        private bool _wasConnected = false;
        public IRacingTelemitry(Game game) : base(game)
        {
        }

        private void OnConnectedToIracing()
        {

           
           
            foreach (var data in iRacing.GetDataFeed().TakeWhile(d => !d.IsConnected))
            {
                
            }

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!iRacing.IsConnected)
            {

                _wasConnected = false;
                return;
            }

            if (!_wasConnected)
            {
                OnConnectedToIracing();
            }

            _wasConnected = true;
        }
    }
}
