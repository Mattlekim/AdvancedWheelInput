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

        /// <summary>
        /// used for the new input system
        /// the old one had a bug too ;)
        /// </summary>
        private float _clutchReleaseTimer = 0;

        private float _clutchReleaseTimeStart = 0;

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
        internal Input _secondClutchButtonIndex = -1;

        /// <summary>
        /// if true we will search through the times and load the fastest detales in
        /// </summary>
        public bool AutoLoadFastestSetup { get; private set; } = true;

        /// <summary>
        /// work in progress
        /// </summary>
        public bool TimesOnlyMode { get; private set; } = false;

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

        /// <summary>
        /// if vjoy is connect or not
        /// </summary>
        private bool _vJoyConnected = false;

        private bool _validVJoyConnection = true;

        private float _newReleaseClutchBitingPointCatch;
        private Random _rd = new Random();
        /// <summary>
        /// depress the second clutch
        /// </summary>
        /// <param name="full">if to depress the second clutch fully or just to the pre set buyting point</param>
        public void DepressSecondClutch(bool full = false)
        {
            if (full)
            {
                _secondClutchDepressedAmount = 1;
                _newReleaseClutchBitingPointCatch = 1f;
                if (_useNewReleaseMethord)
                {
                    _clutchReleaseTimer = .5f;
                    _clutchReleaseTimeStart = _clutchReleaseTimer;
                }
            }
            else
            {
                _secondClutchDepressedAmount = _secondClutchBitingPoint;
                _newReleaseClutchBitingPointCatch = _secondClutchBitingPoint;
                if (_useNewReleaseMethord)
                {
                    if (_secondClutchRelaseTime <= 1) //set the release times we have a bit of random to emulate human error
                        _clutchReleaseTimer = (float)_rd.NextDouble() * .2f + .1f;
                    else
                        _clutchReleaseTimer = (float)_rd.NextDouble() * .3f + .6f;
                    _clutchReleaseTimeStart = _clutchReleaseTimer;
                }
            }

        }


        /// <summary>
        /// a surface that contains the settings
        /// </summary>
        Surface _surfaceSettings;

        /// <summary>
        /// button for the second clutch 
        /// </summary>
        internal SecondClutchButton _secondClutchButton;

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
                _vJoyConnected = true;
                _virtualJoyStick.Aquire(); //try to get the virutal joystick
                if (!_virtualJoyStick.Valid())
                {
                    _validVJoyConnection = false;
                }
            }
            catch
            {
                _vJoyConnected = false;
                SimpleMouse.Enabled = false;
            }

            LoadConfig(); //try to load the config
          
            _surfaceSettings = new Surface()
            {
                TextName = "Settings",
                TextColour = Color.Black,
                PrimaryColour = Color.LightBlue * .4f,
                OnActive = (UiEliment sender) =>
                {
                    Surface s = sender as Surface;

                    Button b = s.Elements[5] as Button; //get auto load button
                    b.ResetButtonState();
                    if (AutoLoadFastestSetup)
                        b.SetPressed();

                    b = s.Elements[6] as Button; //get the times only mode button
                    if (TimesOnlyMode)
                        b.SetPressed();
                }
            };
           

            _surfaceSettings.AddElement(new Button(this, new Rectangle(75, 75, 150, 150))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Incress Biting\n      Point\n\nNot Assigned",
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
                ButtonText = "Decress Biting\n      Point\n\nNot Assigned",
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
            Button but = new Button(this, new Rectangle(75, 275, 150, 150))
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
            };

            _surfaceSettings.AddElement(but);

            if (_useNewReleaseMethord)
                but.ButtonText = "Slow Release\n\nNot Assigned";

            but = new Button(this, new Rectangle(275, 275, 150, 150))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Slow Release\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .4f,
                OnClick = (Button b) =>
                {
                    _currentState = WheelState.Config;
                    _configState = ConfigArea.SetDirectionInput;
                    _inputDirection = CardinalDirection.Right;
                    _surfaceSettings.Deactive();
                }
            };

            if (_useNewReleaseMethord)
                but.ButtonText = "Fast Release\n\nNot Assigned";

            _surfaceSettings.AddElement(but);

            but = new Button(this, new Rectangle(720, 430, 80, 60))
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
                    SaveConfig();
                }

            };

            _surfaceSettings.AddElement(but);

            _surfaceSettings.AddElement(new Button(this, new Rectangle(600, 75, 200, 100))
            {
                PrimaryColour = Color.Orange * .2f,
                SecondryColour = Color.Orange,
                ButtonText = " Auto Load\nBest Setup",
                TextScale = .6f,
                Sticky = true,
                OnClick = (Button b) =>
                {
                    AutoLoadFastestSetup = b.Depressed;
                }
            });

            _surfaceSettings.AddElement(new Button(this, new Rectangle(600, 200, 200, 100))
            {
                PrimaryColour = Color.Orange * .2f,
                SecondryColour = Color.Orange,
                ButtonText = "Timing Mode\n       Only",
                TextScale = .6f,
                Sticky = true,

                OnClick = (Button b) =>
                {
                    TimesOnlyMode = b.Depressed;
                }
            });

            _surfaceSettings.AddElement(new Button(this, new Rectangle(600, 325, 200, 100))
            {
                PrimaryColour = Color.Red * .2f,
                SecondryColour = Color.Orange,
                ButtonText = "Clear Current\n  Car Times",
                TextScale = .6f,

                OnClick = (Button b) =>
                {
                    if (_telemitry.IsConnected)
                        if (_telemitry.CurrentCar != null && _telemitry.CurrentCar != string.Empty)
<<<<<<< HEAD
                        {
                            _telemitry.ClearCurrentCarTimes();
                            _telemitry.DeleteTelimtoryFile();
                        }
=======
                            _telemitry.ClearCurrentCarTimes();
>>>>>>> main
                }
            });


<<<<<<< HEAD
                            
=======
                            _telemitry.DeleteTelimtoryFile();
>>>>>>> main
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
                    if (b.ButtonText.Contains("Incress"))
                    {
                        if (_directionButtons[(int)CardinalDirection.Up].Index == -1)
                        {
                            b.ButtonText = "Incress Biting\n      Point\n\nNot Assigned";
                            b.TextColour = Color.DarkRed;
                        }
                        else
                        {
                            b.ButtonText = "Incress Biting\n      Point";
                            b.TextColour = Color.Black;
                        }
                    }

                    if (b.ButtonText.Contains("Decress"))
                    {
                        if (_directionButtons[(int)CardinalDirection.Down].Index == -1)
                        {
                            b.ButtonText = "Decress Biting\n      Point\n\nNot Assigned";
                            b.TextColour = Color.DarkRed;
                        }
                        else
                        {
                            b.ButtonText = "Decress Biting\n      Point";
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

                    if (b.ButtonText.Contains("Slow"))
                    {
                        if (_directionButtons[(int)CardinalDirection.Left].Index == -1)
                        {
                            b.ButtonText = "Slow Release\n\nNot Assigned";
                            b.TextColour = Color.DarkRed;
                        }
                        else
                        {
                            b.ButtonText = "Slow Release";
                            b.TextColour = Color.Black;
                        }
                    }

                    if (b.ButtonText.Contains("Fast"))
                    {
                        if (_directionButtons[(int)CardinalDirection.Right].Index == -1)
                        {
                            b.ButtonText = "Fast Release\n\nNot Assigned";
                            b.TextColour = Color.DarkRed;
                        }
                        else
                        {
                            b.ButtonText = "Fast Release";
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

            UiEliment.LoadContent(_dot, _font, _game.Content); //load ui element content
            _secondClutchButton = new SecondClutchButton(this);
            _uiElements.Add(_secondClutchButton); //add our clutch ui button
                                                  //  UiEliment.LoadContent(_dot, _font, content); //load ui element content

            _bntSettings = new Button(this, new Rectangle(220, 410, 80, 80))
            {
                Icon = _iconConfig,
                PrimaryColour = Color.LightBlue * 0,
                SecondryColour = Color.OrangeRed,
                TextScale = .4f,
                OnClick = (Button ui) =>
                {
                    _surfaceSettings.Activate();
                    _secondClutchButton.Deactive();
                    _bntSettings.Deactive();
                }
            };

            _uiElements.Add(_bntSettings);

            _secondClutchButton.SetReleaseState(_secondClutchRelaseTime);
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

            if (!_vJoyConnected || !_validVJoyConnection)
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





            if (_useNewReleaseMethord)
            {
                _clutchReleaseTimer -= dt;//release the cutch by time
                if (_clutchReleaseTimeStart > 0)
                    _secondClutchDepressedAmount = _clutchReleaseTimer / _clutchReleaseTimeStart * _newReleaseClutchBitingPointCatch;  //set the clutch point
                else
                    _secondClutchDepressedAmount = 0;
            }
            else
                _secondClutchDepressedAmount -= dt * _secondClutchRelaseTime; //release the cutch by release amount

            //================DISPLAY AN ERROR MSG IF NO BUTTONS FOR USER TO KNOW THERE IS A PROBLEM=================

            if (_inputWheel.Buttons != null) //check that the current input has buttons
                if (_inputWheel.Buttons.Length > 0) //Make sure we have a button
                {
                   if (IsWheelInputPressed(_secondClutchButtonIndex))
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

                    if (!_vJoyConnected)
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
                    else
                    {
                        if (!_validVJoyConnection)
                        {
                            foreach (UiEliment b in _uiElements)
                                b.Draw(sb);

                            sb.Draw(_dot, new Rectangle(0, 0, 300, 500), Color.Black * .8f);
                            sb.DrawString(_font, "VJoy Error", new Vector2(10, 10), Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "To you the software clutch", new Vector2(10, 60), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "you must install vJoy.", new Vector2(10, 80), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                            sb.DrawString(_font, "If Vjoy is installed", new Vector2(10, 120), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "run configure vJoy", new Vector2(10, 140), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "make sure controler 1", new Vector2(10, 160), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "is enabled.", new Vector2(10, 180), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                            sb.DrawString(_font, "Also please make sure", new Vector2(10, 220), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "the the vJoy controler", new Vector2(10, 240), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "has its Z axis enabled.", new Vector2(10, 260), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                            sb.DrawString(_font, "Timings only mode enabled.", new Vector2(10, 300), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "To use the timings", new Vector2(10, 320), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "simply start up Iracing", new Vector2(10, 340), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "and pratice your starts.", new Vector2(10, 360), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            return;
                        }
                        
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
                writer.WriteAttributeEnum<InputType>("SecondClutchInputType", _secondClutchButtonIndex.Type);
                writer.WriteAttributeInt("SecondClutchInputIndex", _secondClutchButtonIndex.Index);
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

                writer.WriteAttributeBool("AutoLoadBestSetup", AutoLoadFastestSetup);
                writer.WriteAttributeBool("TimingsOnlyMode", TimesOnlyMode);

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

                            _secondClutchButtonIndex = new Input(reader.ReadAttributeEnum<InputType>("SecondClutchInputType"), reader.ReadAttributeInt("SecondClutchInputIndex"));

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

                            AutoLoadFastestSetup = reader.ReadAttributeBool("AutoLoadBestSetup");
                            TimesOnlyMode = reader.ReadAttributeBool("TimingsOnlyMode");
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
                    break;
                }
            }

            ///update the buttons to the correct state
            

        }
    }
}
