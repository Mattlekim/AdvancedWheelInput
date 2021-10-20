using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


using iRacingSDK;

using Riddlersoft.Core.Input;

namespace AdvancedInput.RaceControle
{
    public class Main
    {
        private IRacingTelemitry _telemitry;
        private SpriteFont _font;
        private Texture2D _dot;
        public Main(Game g)
        {
            _telemitry = g.Components[2] as IRacingTelemitry;
            _font = g.Content.Load<SpriteFont>("Font"); //load font
            _dot = new Texture2D(g.GraphicsDevice, 1, 1);
            _dot.SetData<Color>(new Color[1] { Color.White });
        }

        public Queue<DataSample> _samples = new Queue<DataSample>();
        public void Update(float dt)
        {
            if (SimpleMouse.IsLButtonClick)
            {
                int ypos = 0;
                for (int i =0; i < _telemitry.CurrentData.Telemetry.CarDetails.Length; i++)
                {
                    if (new Rectangle(0, ypos, 1280, 30).Contains(SimpleMouse.Pos))
                    {
                        CarDetails cd = _telemitry.CurrentData.Telemetry.CarDetails[i];
                        iRacing.Replay.CameraOnDriver(cd.CarNumberRaw, 0, 0);
                    }
                    ypos += 30;
                }
            }
        }

       
        public void Draw(SpriteBatch sb)
        {
            int ypos = 0;
            if (!_telemitry.IsConnected)
                return;
            if (_telemitry.CurrentData == null)
                return;
            Color c = Color.DarkBlue;
            int counter = 0;

            iRacingDirector.Plugin.LeaderBoard lb = new iRacingDirector.Plugin.LeaderBoard();
            //_telemitry.CurrentData.SessionData.SessionInfo.Sessions[0].ResultsPositions
            int sn = _telemitry.CurrentData.Telemetry.SessionNum;
            SessionData._SessionInfo._Sessions._ResultsPositions[] sr = _telemitry.CurrentData.Telemetry.SessionData.SessionInfo.Sessions[sn].ResultsPositions;
            if (sr == null)
                return;

            foreach (SessionData._SessionInfo._Sessions._ResultsPositions rp in sr)
            {
                if (counter % 2 == 0)
                    c = Color.DarkBlue;
                else
                    c = Color.Black;
                sb.Draw(_dot, new Rectangle(0, ypos, 1280, 30), c);
                sb.DrawString(_font, $"{rp.CarIdx}, {rp.Incidents}, {rp.LapsComplete}", new Vector2(0, ypos), Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
                if (rp.Incidents > 0)
                    {

                }
               // iRacingSDK.
                //DataSampleExtensions.RaceIncidents2(_telemitry.CurrentData, 0, 1000);
                ypos += 30;
                counter++;
            }

        }
    }
}
