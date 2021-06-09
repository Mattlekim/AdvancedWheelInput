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

        public List<float> ZeroToSixty { get; private set; } = new List<float>();

        public Action OnConnected;
        public Action OnDisconected;
        public IRacingTelemitry(Game game) : base(game)
        {
            iRacing.NewData += iRacing_NewData;
            iRacing.StartListening();
            
        }

        void iRacing_NewData(DataSample data)
        {
            
            SpeedMph = data.Telemetry.Speed * 2.25f;
            if (!IsConnected)
                if (OnConnected != null)
                    OnConnected();
            _timeSinceGotData = 5f;
            IsConnected = true;
        }


        public override void Initialize()
        {
            base.Initialize();
        }

        private bool _log0to60 = false;
        private float _0to60Time = 0;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timeSinceGotData -= dt;

            if (IsConnected)
                if (_timeSinceGotData <= 0)
                {
                    _log0to60 = false;
                    IsConnected = false;
                    if (OnDisconected != null)
                        OnDisconected();
                }

            _0to60Time += dt;
            if (_log0to60)
            {
                if (SpeedMph > 0.02f)
                {


                    if (SpeedMph >= 60)
                    {
                        _log0to60 = false;
                        ZeroToSixty.Add(_0to60Time);
                    }
                }
                else
                {
                    _0to60Time = 0;
                    _log0to60 = true;
                }
            }
            else
            if (SpeedMph < .02f)
            {
                _0to60Time = 0;
                _log0to60 = true;
            }

        }
    }
}
