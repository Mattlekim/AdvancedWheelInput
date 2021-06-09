using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using vJoy.Wrapper;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Riddlersoft.Core.Input;
using Riddlersoft.Core.Xml;

using AdvancedInput.UI;

namespace AdvancedInput
{
    /// <summary>
    /// this class does all the work with the second clutch emulations
    /// the plan is to add even more function to this in the future
    /// </summary>
    public class AdvanceWheel
    {
        /// <summary>
        /// the center value for an axis only has positive vlaue
        /// </summary>
        const int CenterAxisValue = 16000;

        /// <summary>
        /// the max value for a positive only axis
        /// </summary>
        const int MaxAxisValue = CenterAxisValue * 2;

        //the current state of the wheel
        private WheelState _currentState = WheelState.Config; //set to config by default
        //the type of config state
        private ConfigArea _configState = ConfigArea.DetectJoystickInput; //set to detect input by default

        //biting point for second cluthc
        internal float _secondClutchBitingPoint = .8f; 

        /// <summary>
        /// this is how much the clutch is depressed
        /// </summary>
        private float _secondClutchDepressedAmount;

        /// <summary>
        /// the speed that the clutch releases in seconds
        /// </summary>
        internal float _secondClutchRelaseTime = 1 / 1f;

        /// <summary>
        /// the button index to use for the second clutch
        /// </summary>
        internal int _secondClutchButtonIndex = -1;

        /// <summary>
        /// the wheel input
        /// </summary>
        internal JoystickState _inputWheel;
        /// <summary>
        ///  the wheel index
        /// </summary>
        private int _inputWheelIndex = -1;
        /// <summary>
        /// it id for the wheel so when we reload we can make sure we have the correct one
        /// we do this incase winodws loads in the gamepads in a different order
        /// </summary>
        private string _inputWheelIdentifyer = string.Empty;

        /// <summary>
        /// our virtual joystick
        /// we feed this information to emulate what we want
        /// </summary>
        private VirtualJoystick _virtualJoyStick;

        /// <summary>
        /// the font for displaying text
        /// </summary>
        internal SpriteFont _font;
        internal Texture2D _iconConfig;

        /// <summary>
        /// the 1x1 white texture we use
        /// </summary>
        private Texture2D _dot;

        private Game _game;

        /// <summary>
        /// a list of buttons
        /// THIS NOW NEED CHANGING TO UiEelements
        /// </summary>
        private List<Button> _buttons = new List<Button>();

        /// <summary>
        /// depress the second clutch
        /// </summary>
        /// <param name="full">if to depress the second clutch fully or just to the pre set buyting point</param>
        public void DepressSecondClutch(bool full = false)
        {
            if (full)
                _secondClutchDepressedAmount = 1;
            else
                _secondClutchDepressedAmount = _secondClutchBitingPoint;

        }

        
        

        public AdvanceWheel(Game game)
        {
            _game = game; //set the game
            _virtualJoyStick = new VirtualJoystick(1); //get the first virutal joystick
            try
            {
                _virtualJoyStick.Aquire(); //try to get the virutal joystick
            }
            catch
            {
                //====================CHANGE THIS TO DISPLAY AN ERROR MSG INSTEAD OF TERMINATING THE APP==============
                throw new Exception("A error msg needs to be displayed to the user");
            }

            LoadConfig(); //try to load the config

            _buttons.Add(new SecondClutchButton(this)); //add our clutch ui button
        }

        /// <summary>
        /// load all needed content
        /// </summary>
        /// <param name="content">the game content manager</param>
        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("Font"); //load font
            _iconConfig = content.Load<Texture2D>("Imgs\\config"); //load texture icon for config
            _dot = new Texture2D(_game.GraphicsDevice, 1, 1); //create 1x1 dot
            _dot.SetData<Color>(new Color[1] { Color.White }); //set color of dot to white

            UiEliment.LoadContent(_dot, _font); //load ui element content
        }

        public void Update(float dt)
        {
            //see if we dont have a id for the input wheel
            //in this case we need to run the start up sequence
            if (_inputWheelIdentifyer == string.Empty) 
            {
                JoystickCapabilities jCapabilityes;
                JoystickState jState;
                for (int i = 0; i < 8; i++) //loop though all joystics
                {
                    jCapabilityes = Joystick.GetCapabilities(i); //get joystick capabilitys
                    if (jCapabilityes.IsConnected) //if is connectec
                        if (jCapabilityes.ButtonCount > 0) //make sure it has buttons
                        {
                            jState = Joystick.GetState(i); //get its state
                            for (int c = 0; c < jCapabilityes.ButtonCount; c++)
                                if (jState.Buttons[c] == ButtonState.Pressed) //at this point we want to log the button and the input device
                                {
                                    _inputWheelIdentifyer = jCapabilityes.Identifier; //set this as the wheel
                                    _inputWheelIndex = i; //set the index for this input
                                    _currentState = WheelState.Run; //change app to run mode
                                    SaveConfig(); //save the config
                                    return;
                                }
                        }
                }

                return;
            }

            switch (_currentState)
            {
                case WheelState.Run:
                    foreach (Button b in _buttons)
                        b.Update(dt);
                    break;
            }

            if (_inputWheelIndex == -1) //check to make sure we have an input device
                return;

            _inputWheel = Joystick.GetState(_inputWheelIndex); //get the input

            _secondClutchDepressedAmount -= dt * _secondClutchRelaseTime; //release the cutch by release amount

            //================DISPLAY AN ERROR MSG IF NO BUTTONS FOR USER TO KNOW THERE IS A PROBLEM=================

            if (_inputWheel.Buttons != null) //check that the current input has buttons
                if (_inputWheel.Buttons.Length > 0) //Make sure we have a button
                {
                    if (_inputWheelIndex > -1 && _secondClutchButtonIndex > -1) //check things are set up
                        if (_secondClutchButtonIndex < _inputWheel.Buttons.Length) //make sure index exist
                        if (_inputWheel.Buttons[_secondClutchButtonIndex] == ButtonState.Pressed) //if its pressed
                            DepressSecondClutch(); //depresse the clutch
                }

                    
            _secondClutchDepressedAmount = MathHelper.Clamp(_secondClutchDepressedAmount, 0, 1); //standard clamp

            //update the virtual joystick
            _virtualJoyStick.SetJoystickAxis(Convert.ToInt32(MaxAxisValue * _secondClutchDepressedAmount), Axis.HID_USAGE_Z);


          
        }


        public void Draw(SpriteBatch sb)
        {
            //just display needed info to the user
            switch (_currentState)
            {
                case WheelState.Config:
                    string text = "Press any button\non the wheel\nyou want to use";
                    sb.DrawString(_font, text, new Vector2(250,250), Color.White, 0f, _font.MeasureString(text)*.5f, 1f, SpriteEffects.None, 0f); 
                    break;

                case WheelState.Run:
                    foreach (Button b in _buttons)
                        b.Draw(sb);
                 /*   sb.Draw(_dot, new Rectangle(100, 100, 300, 100), Color.Red * .5f);
                    if (_secondClutchButtonIndex == -1)
                    {
                        sb.DrawString(_font, "Clutch", new Vector2(110, 110), Color.White);
                        sb.DrawString(_font, "Click to set up", new Vector2(110, 165), Color.White * .5f, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
                        sb.Draw(_iconConfig, new Vector2(300, 105), null, Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);
                    }*/
                    break;
            }
        }

        //=====================CONFIG FILE==============================
        private string _configFile = "config.txt";

        public void SaveConfig()
        {
            using (FileStream stream = File.Create(_configFile))
            {
                CustomXmlWriter writer = CustomXmlWriter.Create(stream);
                writer.WriteStartDocument();

                writer.WriteStartElement("Config");

                writer.WriteAttributeString("InputId", _inputWheelIdentifyer);
                writer.WriteAttributeInt("SecondClutch", _secondClutchButtonIndex);
                writer.WriteAttributeFloat("SecondClutchBite", _secondClutchBitingPoint);
                writer.WriteAttributeFloat("SecondClutchReleaseTime", _secondClutchRelaseTime);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }

        public void LoadConfig()
        {
            if (!File.Exists(_configFile))
                return;
            try
            {
                using (FileStream stream = File.OpenRead(_configFile))
                {
                    CustomXmlReader reader = CustomXmlReader.Create(stream);
                    while (reader.Read())
                    {
                        if (reader.Name == "Config")
                        {
                            _inputWheelIdentifyer = reader.ReadAttributeString("InputId");
                            _secondClutchButtonIndex = reader.ReadAttributeInt("SecondClutch");
                            _secondClutchBitingPoint = reader.ReadAttributeFloat("SecondClutchBite");
                            _secondClutchRelaseTime = reader.ReadAttributeFloat("SecondClutchReleaseTime");
                        }
                    }

                    reader.Close();
                }

                _currentState = WheelState.Run;
            }
            catch
            {
                _inputWheelIdentifyer = string.Empty;
                _inputWheelIndex = -1;
                _secondClutchButtonIndex = -1;
            }

            for (int i =0; i < 8; i++)
            {
                JoystickCapabilities jc = Joystick.GetCapabilities(i);
              //  if (jc == null)
                  //  continue;

                if (jc.Identifier == _inputWheelIdentifyer) //if we find the wheel we are using 
                {
                    _inputWheelIndex = i;
                    return;
                }
            }

        }
    }
}
