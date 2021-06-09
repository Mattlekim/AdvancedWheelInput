using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AdvancedInput.UI
{
    /// <summary>
    /// our custome button that will do all our second clutch stuff
    /// </summary>
    public class SecondClutchButton : Button
    {
        /// <summary>
        /// flag for waiting for user to press a button to assign to second clutch
        /// </summary>
        private bool _detectSecondClutchInput = false;

        //sliders for biting point and relase time
        private Slider _slBitingPoint;
        private Slider _slReleaseTime;

        //other ui buttons
        private Button _bntSaveConfig;
        private Button _bntSetInput;

        //timer for amout of fade.
        //we use this to put a display to tell the user is saved
        private float _showSaved = 0;


        public SecondClutchButton(AdvanceWheel wheel) : base(wheel, new Rectangle(100,100,300,300))
        {
            //some basic formatting
            PrimaryColour = Color.Red * .5f;
            SecondryColour = Color.OrangeRed * .5f;
            
            //----------------CREATE UI ELEMENTS---------------------------
            _slBitingPoint = new UI.Slider(new Rectangle(150, 180, 50, 200), 1, 0, .8f)
            {
                TextName = "Biting Point",
                TextScale = .5f,
                Current = wheel._secondClutchBitingPoint,
                OnChange = (Slider s) =>
                {
                    wheel._secondClutchBitingPoint = s.GetValue();
                },
            };

            _slReleaseTime = new UI.Slider(new Rectangle(250, 180, 50, 200), 3, 0, .5f)
            {
                TextName = "Release Time\n(Seconds)",
                TextScale = .3f,
                Current = 1f / (wheel._secondClutchRelaseTime * 3f),
                OnChange = (Slider s) =>
                {
                    wheel._secondClutchRelaseTime = 1f / s.GetValue();
                },
            };

            _bntSetInput = new UI.Button(wheel, new Rectangle(310, 180, 80, 80))
            {
                PrimaryColour = Color.Green,
                SecondryColour = Color.OrangeRed,
                ButtonText = "Set Input\nIn Iracing",
                TextScale = .3f,
                
                OnClick = (Button b) =>
                {
                    wheel.DepressSecondClutch(true);
                }
            };

            _bntSaveConfig = new UI.Button(wheel, new Rectangle(310, 330, 80, 50))
            {
                PrimaryColour = Color.Orange,
                SecondryColour = Color.OrangeRed,
                ButtonText = "Save",
                TextScale = .4f,
                OnClick = (Button b) =>
                {
                    wheel.SaveConfig();
                    _showSaved = 2f;
                }
            };

            //-----------------ADD UI ELEMENTS TO THIS -------------------------
            AddElement(_slBitingPoint);
            AddElement(_slReleaseTime);

            AddElement(_bntSetInput);
            AddElement(_bntSaveConfig);
        }

        float _flasher;

        public override void Update(float dt)
        {
            //showing save display
            _showSaved -= dt;
            _showSaved = MathHelper.Clamp(_showSaved, 0, 1);
           
            base.Update(dt); //base update for all ui elements

            _flasher += dt * 2; //flasher to display text to flas
            if (_flasher > 1)
                _flasher -= 2; //limit it to -1 to 1

            if (Wheel._inputWheel.Buttons == null)  //check that the current input wheel has buttons
                return;


            if (_detectSecondClutchInput) //if we are waiting to detect a button press for clutch
            {
                for (int i =0; i < Wheel._inputWheel.Buttons.Length; i++) //check all buttons on wheel
                {
                    if (Wheel._inputWheel.Buttons[i] == Microsoft.Xna.Framework.Input.ButtonState.Pressed) //look for press
                    {
                        _detectSecondClutchInput = false; //turn the flag off
                        Wheel._secondClutchButtonIndex = i; //set the button
                        Wheel.SaveConfig(); //save the config
                        break;
                    }
                }
            }

          

        }       

        public override void OnLClick(Vector2 pos)
        {
            if (Wheel._secondClutchButtonIndex == -1)
                _detectSecondClutchInput = true;
           
        }

        public override void Draw(SpriteBatch sb)
        {
            //draw save text when needed
            sb.Draw(Dot, new Rectangle(0, 450, 500, 50), Color.OrangeRed * _showSaved);
            sb.DrawString(Font, "Saved Setup", new Vector2(250, 475), Color.White * _showSaved, 0f, Font.MeasureString("Saved Setup") * .5f, .8f, SpriteEffects.None, 0f);


            if (Wheel._secondClutchButtonIndex == -1) //if we dont have a button set
            {

               

                if (_detectSecondClutchInput) //if wating for user to press a button display a promt
                    sb.DrawString(Wheel._font, "Press button for second clutch mapping", new Vector2(250, 250), Color.White * (_flasher * _flasher), 0f,
                        Wheel._font.MeasureString("Press button for second clutch mapping") * .5f, .45f, SpriteEffects.None, 0f);
                else
                {
                    //standard text layout
                    sb.DrawString(Wheel._font, "Clutch", new Vector2(110, 110), Color.White);
                    sb.DrawString(Wheel._font, "Click to set up", new Vector2(110, 165), Color.White * (.5f), 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
                    sb.Draw(Wheel._iconConfig, new Vector2(300, 105), null, Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);
                }
                return;
            }
            else
            {
                sb.DrawString(Wheel._font, "Clutch", new Vector2(110, 110), Color.White);
                base.Draw(sb);
            }
         


        }

        public override void OnLButtonDown(Vector2 pos)
        {
            throw new NotImplementedException();
        }
    }
}
