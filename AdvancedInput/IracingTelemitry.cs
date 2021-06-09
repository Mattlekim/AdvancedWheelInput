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

        public float SpeedMph { get; private set; }

        public bool IsConnected { get; private set; } = false;
        private float _timeSinceGotData = 5f;

        public Action OnConnected;
        public Action OnDisconected;
        public IRacingTelemitry(Game game) : base(game)
        {
            iRacing.NewData += iRacing_NewData;
            iRacing.StartListening();
            _timeSinceGotData = 5f;
        }

        void iRacing_NewData(DataSample data)
        {
            
            SpeedMph = data.Telemetry.Speed * 2.25f;
            if (!IsConnected)
                if (OnConnected != null)
                    OnConnected();

            IsConnected = true;
        }


        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _timeSinceGotData -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsConnected)
                if (_timeSinceGotData <= 0)
                {
                    IsConnected = false;
                    if (OnDisconected != null)
                        OnDisconected();
                }
        }
    }
}
