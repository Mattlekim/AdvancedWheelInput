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
using Riddlersoft.Core;
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

        /// <summary>
        /// weahter to use the new release methord or not
        /// </summary>
        internal bool _useNewReleaseMethord = true;

        //the current state of the wheel
        private WheelState _currentState = WheelState.Config; //set to config by default
        //the type of config state
        private ConfigArea _configState = ConfigArea.DetectJoystickInput; //set to detect input by default

        //biting point for second cluthc
        internal float _secondClutchBitingPoint = .8f; 

        /// <summary>
        /// this is how much the clutch is depressed
        /// </summary>
        internal float _secondClutchDepressedAmount;

        /// <summary>
        /// the speed that the clutch releases in seconds
        /// </summary>
        internal float _secondClutchRelaseTime = 1 / 1f;

        /// <summary>
        /// the button index to use for the second clutch
        /// </summary>
        internal int _secondClutchButtonIndex = -1;


        /// <summary>
        /// the inputs for making ajustments with the wheel
        /// </summary>
        internal Input[] _directionButtons = new Input[4] { -1, -1, -1, -1 };

        private CardinalDirection _inputDirection = CardinalDirection.None;
        /// <summary>
        /// the wheel input
        /// </summary>
        internal JoystickState _inputWheel;
        /// <summary>
        ///  the wheel index
        /// </summary>
        private Input _inputWheelIndex = -1;
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
        private List<UiEliment> _uiElements = new List<UiEliment>();

        private bool _VJoyConnected = false;

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


        /// <summary>
        /// a surface that contains the settings
        /// </summary>
        Surface _surfaceSettings;

        /// <summary>
        /// button for the second clutch 
        /// </summary>
        SecondClutchButton _secondClutchButton;

        /// <summary>
        /// button for the setttings
        /// </summary>
        Button _bntSettings;
        /// <summary>
        /// used for loging 0 to 60
        /// </summary>
        internal IRacingTelemitry _telemitry;
        public AdvanceWheel(Game game)
        {
            _game = game; //set the game
            _telemitry = _game.Components[2] as IRacingTelemitry;
            _virtualJoyStick = new VirtualJoystick(1); //get the first virutal joystick
            try
            {
                _VJoyConnected = true;
                _virtualJoyStick.Aquire(); //try to get the virutal joystick
            }
            catch
            {
                _VJoyConnected = false;
                SimpleMouse.Enabled = false;
            }

            LoadConfig(); //try to load the config

            _secondClutchButton = new SecondClutchButton(this);
            _uiElements.Add(_secondClutchButton); //add our clutch ui button

            _surfaceSettings = new Surface()
            {
                TextName = "Settings",
                TextColour = Color.Black,
                PrimaryColour = Color.LightBlue * .4f,
            };
           

            _surfaceSettings.AddElement(new Button(this, new Rectangle(75, 75, 150, 150))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "  Up Button\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .4f,
                OnClick = (Button b) =>
                {
                    _currentState = WheelState.Config;
                    _configState = ConfigArea.SetDirectionInput;
                    _inputDirection = CardinalDirection.Up;
                    _surfaceSettings.Deactive();
                }
            });

            _surfaceSettings.AddElement(new Button(this, new Rectangle(275, 75, 150, 150))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Down Button\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .4f,
                OnClick = (Button b) =>
                {
                    _currentState = WheelState.Config;
                    _configState = ConfigArea.SetDirectionInput;
                    _inputDirection = CardinalDirection.Down;
                    _surfaceSettings.Deactive();
                }
            });

            _surfaceSettings.AddElement(new Button(this, new Rectangle(75, 275, 150, 150))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Left Button\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .4f,
                OnClick = (Button b) =>
                {
                    _currentState = WheelState.Config;
                    _configState = ConfigArea.SetDirectionInput;
                    _inputDirection = CardinalDirection.Left;
                    _surfaceSettings.Deactive();
                }
            });

            _surfaceSettings.AddElement(new Button(this, new Rectangle(275, 275, 150, 150))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Right Button\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .4f,
                OnClick = (Button b) =>
                {
                    _currentState = WheelState.Config;
                    _configState = ConfigArea.SetDirectionInput;
                    _inputDirection = CardinalDirection.Right;
                    _surfaceSettings.Deactive();
                }
            });

            _surfaceSettings.AddElement(new Button(this, new Rectangle(420, 440, 80, 60))
            {
                PrimaryColour = Color.Black * .2f,
                ButtonText = "Back",
                TextScale = .6f,
                OnClick = (Button b) =>
                {
                    SimpleMouse.Reset();
                    _surfaceSettings.Deactive();
                    _secondClutchButton.Activate();
                    _bntSettings.Activate();
                }
            
            });

            _surfaceSettings.Deactive();
            _uiElements.Add(_surfaceSettings);
            UpdateSuraceButtons();
        }

        public void UpdateSuraceButtons()
        {
            foreach (UiEliment ui in _surfaceSettings.Elements)
            {
                Button b = ui as Button;
                if (b!= null)
                {
                    if (b.ButtonText.Contains("Up"))
                    {
                        if (_directionButtons[(int)CardinalDirection.Up].Index == -1)
                        {
                            b.ButtonText = "  Up Button\n\nNot Assigned";
                            b.TextColour = Color.DarkRed;
                        }
                        else
                        {
                            b.ButtonText = "  Up Button";
                            b.TextColour = Color.Black;
                        }
                    }

                    if (b.ButtonText.Contains("Down"))
                    {
                        if (_directionButtons[(int)CardinalDirection.Down].Index == -1)
                        {
                            b.ButtonText = "  Down Button\n\nNot Assigned";
                            b.TextColour = Color.DarkRed;
                        }
                        else
                        {
                            b.ButtonText = "  Down Button";
                            b.TextColour = Color.Black;
                        }
                    }

                    if (b.ButtonText.Contains("Left"))
                    {
                        if (_directionButtons[(int)CardinalDirection.Left].Index == -1)
                        {
                            b.ButtonText = "  Left Button\n\nNot Assigned";
                            b.TextColour = Color.DarkRed;
                        }
                        else
                        {
                            b.ButtonText = "  Left Button";
                            b.TextColour = Color.Black;
                        }
                    }

                    if (b.ButtonText.Contains("Right"))
                    {
                        if (_directionButtons[(int)CardinalDirection.Right].Index == -1)
                        {
                            b.ButtonText = "  Right Button\n\nNot Assigned";
                            b.TextColour = Color.DarkRed;
                        }
                        else
                        {
                            b.ButtonText = "  Right Button";
                            b.TextColour = Color.Black;
                        }
                    }
                }
            }

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

            UiEliment.LoadContent(_dot, _font, content); //load ui element content

            _bntSettings = new Button(this, new Rectangle(220, 410, 80, 80))
            {
                Icon = _iconConfig,
                PrimaryColour = Color.LightBlue * 0,
                SecondryColour = Color.LightBlue,
                OnClick = (Button ui) =>
                {
                    _surfaceSettings.Activate();
                    _secondClutchButton.Deactive();
                    _bntSettings.Deactive();
                }
            };

            _uiElements.Add(_bntSettings);
        }

        public bool IsWheelInputPressed(Input i)
        {
            if (i.Index == -1)
                return false;

            if (i.Type == InputType.Button)
            {
                if (_inputWheel.Buttons != null)
                    if (_inputWheel.Buttons.Length > 0 && i.Index >= 0 && i.Index < _inputWheel.Buttons.Length)
                        if (_inputWheel.Buttons[i.Index] == ButtonState.Pressed)
                            return true;

            }
            else
            {
                if (_inputWheel.Hats != null)
                    if (_inputWheel.Hats.Length > 0)
                        if (i.Index >= 0 && i.Index < _inputWheel.Hats.Length)
                        {
                            if (i.Type == InputType.HatUp)
                                if (_inputWheel.Hats[i.Index].Up == ButtonState.Pressed)
                                    return true;

                            if (i.Type == InputType.HatDown)
                                if (_inputWheel.Hats[i.Index].Down == ButtonState.Pressed)
                                    return true;

                            if (i.Type == InputType.HatLeft)
                                if (_inputWheel.Hats[i.Index].Left == ButtonState.Pressed)
                                    return true;

                            if (i.Type == InputType.HatRight)
                                if (_inputWheel.Hats[i.Index].Right == ButtonState.Pressed)
                                    return true;
                        }
            }

                return false;
            }

        public Input GetInputButton()
        {
            JoystickCapabilities jCapabilityes;
            JoystickState jState;
            for (int i = 0; i < 8; i++) //loop though all joystics
            {
                jCapabilityes = Joystick.GetCapabilities(i); //get joystick capabilitys
                if (jCapabilityes.IsConnected) //if is connectec
                {
                    jState = Joystick.GetState(i); //get its state
                    if (jCapabilityes.ButtonCount > 0) //make sure it has buttons
                    {
                        
                        for (int c = 0; c < jCapabilityes.ButtonCount; c++)
                            if (jState.Buttons[c] == ButtonState.Pressed) //at this point we want to log the button and the input device
                            {
                                return i;
                            }
                    }

                    for (int c = 0; c < jCapabilityes.HatCount; c++)
                    {
                        if (jState.Hats[c].Up == ButtonState.Pressed)
                            return new Input(InputType.HatUp, c);

                        if (jState.Hats[c].Down == ButtonState.Pressed)
                            return new Input(InputType.HatDown, c);

                        if (jState.Hats[c].Left == ButtonState.Pressed)
                            return new Input(InputType.HatLeft, c);

                        if (jState.Hats[c].Right == ButtonState.Pressed)
                            return new Input(InputType.HatRight, c);
                    }
                }
            }
            return -1;
        }

        public void Update(float dt)
        {

            if (!_VJoyConnected)
            {

                return;
            }
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

            if (_inputWheelIndex.Index == -1) //check to make sure we have an input device
            {


                return;
            }
            _inputWheel = Joystick.GetState(_inputWheelIndex.Index); //get the input

            switch (_currentState)
            {
                case WheelState.Run:
                    foreach (UiEliment b in _uiElements)
                        b.Update(dt);

                 
                    break;

                case WheelState.Config:
                    switch (_configState)
                    {
                        case ConfigArea.SetDirectionInput:
                            /*case ConfigArea.SetDirectionInput:
                                foreach (UiEliment b in _uiElements)
                                    b.Update(dt);
                                return;*/
                            Input bnt = GetInputButton();
                            if (bnt.Index != -1) //if a button is press
                            {
                                _directionButtons[(int)_inputDirection] = bnt;
                                _currentState = WheelState.Run;
                                _surfaceSettings.Activate();
                                UpdateSuraceButtons();
                                SaveConfig();
                            }
                            return;
                    }
                    break;
            }

            

           

            _secondClutchDepressedAmount -= dt * _secondClutchRelaseTime; //release the cutch by release amount

            //================DISPLAY AN ERROR MSG IF NO BUTTONS FOR USER TO KNOW THERE IS A PROBLEM=================

            if (_inputWheel.Buttons != null) //check that the current input has buttons
                if (_inputWheel.Buttons.Length > 0) //Make sure we have a button
                {
                    if (_inputWheelIndex.Index > -1 && _secondClutchButtonIndex > -1) //check things are set up
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
                    string text;
                    if (_configState == ConfigArea.SetDirectionInput)
                    {
                        text = "   Press the button\n      on the wheel\nthat you want to use";
                        sb.DrawString(_font, text, new Vector2(250, 250), Color.White, 0f, _font.MeasureString(text) * .5f, .9f, SpriteEffects.None, 0f);
                        return;
                    }

                    text = "Press any button\non the wheel\nyou want to use";
                    sb.DrawString(_font, text, new Vector2(250,250), Color.White, 0f, _font.MeasureString(text)*.5f, 1f, SpriteEffects.None, 0f); 
                    break;

                case WheelState.Run:

                    if (!_VJoyConnected)
                    {

                        foreach (UiEliment b in _uiElements)
                            b.Draw(sb);
                        sb.Draw(_dot, new Rectangle(0, 0, 300, 500), Color.Black * .8f);
                        sb.DrawString(_font, "No Vjoy Device", new Vector2(10, 10), Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);

                        sb.DrawString(_font, "To you the software clutch", new Vector2(10, 60), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                        sb.DrawString(_font, "you must install vJoy.", new Vector2(10, 80), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                        sb.DrawString(_font, "Without Vjoy installed you", new Vector2(10, 120), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                        sb.DrawString(_font, "can only use the timing", new Vector2(10, 140), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                        sb.DrawString(_font, "part of this app", new Vector2(10, 160), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                        sb.DrawString(_font, "To use the timings", new Vector2(10, 200), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                        sb.DrawString(_font, "simply start up Iracing", new Vector2(10, 220), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                        sb.DrawString(_font, "and pratice your starts.", new Vector2(10, 240), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                        return;
                    }

                    if (_inputWheelIndex.Index == -1) //check to make sure we have an input device
                    {
                        sb.DrawString(_font, "Your Input device cannot be found", new Vector2(400, 220), Color.White, 0f, _font.MeasureString("Your Input device cannot be found") * .5f, .9f, SpriteEffects.None, 0f);
                        sb.DrawString(_font, "Pleae check your Connection and restart this app.", new Vector2(400, 270), Color.White, 0f, _font.MeasureString("Pleae check your Connection and restart this app.") * .5f, .6f, SpriteEffects.None, 0f);
                        return;
                    }

                    foreach (UiEliment b in _uiElements)
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
                
                    writer.WriteAttributeEnum<InputType>("InputUpType", _directionButtons[(int)CardinalDirection.Up].Type);
                    writer.WriteAttributeInt("InputUpIndex", _directionButtons[(int)CardinalDirection.Up].Index);
                    writer.WriteAttributeEnum<InputType>("InputDownType", _directionButtons[(int)CardinalDirection.Down].Type);
                    writer.WriteAttributeInt("InputDownIndex", _directionButtons[(int)CardinalDirection.Down].Index);
                    writer.WriteAttributeEnum<InputType>("InputLeftType", _directionButtons[(int)CardinalDirection.Left].Type);
                    writer.WriteAttributeInt("InputLeftIndex", _directionButtons[(int)CardinalDirection.Left].Index);
                    writer.WriteAttributeEnum<InputType>("InputRightType", _directionButtons[(int)CardinalDirection.Right].Type);
                    writer.WriteAttributeInt("InputRightIndex", _directionButtons[(int)CardinalDirection.Right].Index);
                
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }

        public void LoadConfig()
        {
            if (!File.Exists(_configFile))
            {
                SaveConfig();
                return;
            }
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


                            InputType t = reader.ReadAttributeEnum<InputType>("InputUpType");
                            int i = reader.ReadAttributeInt("InputUpIndex");
                            _directionButtons[(int)CardinalDirection.Up] = new Input(t, i);

                            t = reader.ReadAttributeEnum<InputType>("InputDownType");
                            i = reader.ReadAttributeInt("InputDownIndex");
                            _directionButtons[(int)CardinalDirection.Down] = new Input(t, i);

                            t = reader.ReadAttributeEnum<InputType>("InputLeftType");
                            i = reader.ReadAttributeInt("InputLeftIndex");
                            _directionButtons[(int)CardinalDirection.Left] = new Input(t, i);

                            t = reader.ReadAttributeEnum<InputType>("InputRightType");
                            i = reader.ReadAttributeInt("InputRightIndex");
                            _directionButtons[(int)CardinalDirection.Right] = new Input(t, i);
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
