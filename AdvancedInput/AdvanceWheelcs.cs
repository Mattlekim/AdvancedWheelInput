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
using Microsoft.Xna.Framework.Audio;

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
        const int HalfValue = 16000;

        /// <summary>
        /// the max value for a positive only axis
        /// </summary>
        const int MaxAxisValue = HalfValue * 2;


        private Slider _sldVoiceVolume;

        internal float _voiceVolume = 1f;
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
        private WheelState _currentState = WheelState.Run; //set to config by default
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
        internal float _secondClutchRelaseTime = 1f;

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
        /// if to say out loud the time or not
        /// </summary>
        public bool SayTimingOutloud = false;
        /// <summary>
        /// the inputs for making ajustments with the wheel
        /// </summary>
        internal Input[] _directionButtons = new Input[4] { -1, -1, -1, -1 };

        /// <summary>
        /// used for settings page knowing which input we are currently working with for the direciotn buttons
        /// </summary>
        private CardinalDirection _inputDirection = CardinalDirection.None;
        
    

        /// <summary>
        /// our virtual joystick
        /// we feed this information to emulate what we want
        /// </summary>
        private VirtualJoystick _virtualJoyStick;

        /// <summary>
        /// the font for displaying text
        /// </summary>
        internal SpriteFont _font;

        /// <summary>
        /// simple texture of the config button
        /// </summary>
        internal Texture2D _iconConfig;

        /// <summary>
        /// the 1x1 white texture we use
        /// </summary>
        private Texture2D _dot;

        /// <summary>
        /// the parent device
        /// </summary>
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

        public bool ValidVJoyConnection { get; private set; } = true;

        private float _newReleaseClutchBitingPointCatch;

        /// <summary>
        /// if we have a real second clutch we can use that input instead of trying to emulate the release of a fake one
        /// </summary>
        internal float _realSecondClutchInputAmount;

        /// <summary>
        /// if to use a real clutch or not
        /// </summary>
        internal bool _useRealSecondClutch;
        private Random _rd = new Random();

        public CustomAudioMsg _customAudio;
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
                        _clutchReleaseTimer = (float)_rd.NextDouble() * .3f + .1f;
                    else
                        _clutchReleaseTimer = (float)_rd.NextDouble() * .3f + .7f;
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

        public void SetTelemitory()
        {
            _telemitry = _game.Components[2] as IRacingTelemitry;

            _telemitry.On0To60 = (TimeRecord tr) =>
            {
                if (SayTimingOutloud)
                    VoiceTime.Speak(tr.ZeroToSixty);
            };
        }
        public AdvanceWheel(Game game)
        {
            _game = game; //set the game

            _customAudio = new CustomAudioMsg(this);
            _virtualJoyStick = new VirtualJoystick(1); //get the first virutal joystick
            try
            {
                _vJoyConnected = true;
                _virtualJoyStick.Aquire(); //try to get the virutal joystick
                if (!_virtualJoyStick.Valid())
                {
                    ValidVJoyConnection = false;
                 //   TimesOnlyMode = true;
                }
            }
            catch
            {
                _vJoyConnected = false;
                //TimesOnlyMode = true;
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

                    Button b = s.Elements[6] as Button; //get auto load button
                    b.ResetButtonState();
                    if (AutoLoadFastestSetup)
                        b.SetPressed();

                    /*if (TimesOnlyMode)
                        b.Deactive();
                    else
                        b.Activate()*/

                    b = s.Elements[7] as Button; //get the times only mode button
                    b.ResetButtonState();
                    if (TimesOnlyMode)
                        b.SetPressed();

                    b = s.Elements[4] as Button; //get the map clutch button
                    if (_secondClutchButtonIndex.Index == -1)
                    {
                        b.ButtonText = " Map Second\n     Clutch\n\nNot Assigned";
                        b.TextColour = Color.Red;
                    }
                    else
                    {
                        b.ButtonText = "Map Second\n     Clutch";
                        b.TextColour = Color.Black;
                    }

                    b = s.Elements[9] as Button; //get the speak button
                    b.Depressed = SayTimingOutloud;

                    b = s.Elements[11] as Button;
                    b.Depressed = _useRealSecondClutch;

                    if (_secondClutchButtonIndex.Type == InputType.Anolog)
                        b.Activate();
                    else
                    {
                        b.Deactive();
                        _useRealSecondClutch = false;
                    }

                    _sldVoiceVolume.Current = _voiceVolume;
                   
                }
            };
           

            _surfaceSettings.AddElement(new Button(this, new Rectangle(50, 75, 125, 125))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Incress Biting\n      Point\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .35f,
                OnClick = (Button b) =>
                {
                    _currentState = WheelState.Config;
                    _configState = ConfigArea.SetDirectionInput;
                    _inputDirection = CardinalDirection.Up;
                    _surfaceSettings.Deactive();
                }
            });

            _surfaceSettings.AddElement(new Button(this, new Rectangle(200, 75, 125, 125))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Decress Biting\n      Point\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .35f,
                OnClick = (Button b) =>
                {
                    _currentState = WheelState.Config;
                    _configState = ConfigArea.SetDirectionInput;
                    _inputDirection = CardinalDirection.Down;
                    _surfaceSettings.Deactive();
                }
            });
            Button but = new Button(this, new Rectangle(50, 300, 125, 125))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Left Button\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .35f,
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

            but = new Button(this, new Rectangle(200, 300, 125, 125))
            {
                PrimaryColour = Color.LightBlue * .8f,
                ButtonText = "Slow Release\n\nNot Assigned",
                TextColour = Color.DarkRed,
                TextScale = .35f,
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

            _surfaceSettings.AddElement(new Button(this, new Rectangle(350, 75, 125, 125))
            {
                PrimaryColour = Color.LightBlue * .8f,
                TextScale = .35f,
                TextColour = Color.Black,
                ButtonText = "Map Clutch",
                OnClick = (Button b) =>
                {
                    _surfaceSettings.Deactive();
                    _currentState = WheelState.Config;
                    _configState = ConfigArea.DetectClutch;
                }
            });

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
                    _bntSettings.Activate(true);
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

                    //use this to refresh the settings
                    _surfaceSettings.Deactive();
                    _surfaceSettings.Activate();
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
                        {
                            _telemitry.ClearCurrentCarTimes();
                            _telemitry.DeleteTelimtoryFile();
                        }
                }
            });

            _surfaceSettings.AddElement(new Button(this, new Rectangle(350, 300, 125, 125))
            {
                PrimaryColour = Color.Orange * .2f,
                SecondryColour = Color.Orange,
                ButtonText = " Speak\nTimings",
                TextColour = Color.White,
                TextScale = .5f,
                Sticky = true,
                OnClick = (Button b) =>
                {

                    SayTimingOutloud = b.Depressed;
                    //
                }
            });

            _sldVoiceVolume = new Slider(new Rectangle(490, 300, 55, 125), 1, 0, _voiceVolume)
            {
                PrimaryColour = Color.Orange,
                SecondryColour = Color.LightBlue,
                TextColour = Color.White,
                TextScale = .5f,
                TextName = "Volume",
                OnChange = (Slider sld) =>
                {
                    _voiceVolume = MathHelper.Clamp(sld.Current, 0, 1);
                    SoundEffect.MasterVolume = sld.Current;
                },
            };
            _surfaceSettings.AddElement(_sldVoiceVolume);

            _surfaceSettings.AddElement(new Button(this, new Rectangle(350, 210, 125, 75))
            {
                PrimaryColour = Color.Orange * .2f,
                SecondryColour = Color.Orange,
                ButtonText = " Use Real\n   Clutch",
                TextColour = Color.White,
                TextScale = .5f,
                Sticky = true,
                OnClick = (Button b) =>
                {
                   _useRealSecondClutch = b.Depressed;
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
                if (b != null)
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

                    if (b.ButtonText.Contains("Map Clutch"))
                        if (_secondClutchButtonIndex.Index == -1)
                        {
                            b.ButtonText = " Map Clutch\n\nNot Assigned";
                            b.TextColour = Color.Red;
                        }
                        else
                        {
                            b.ButtonText = "Map Clutch";
                            b.TextColour = Color.Black;
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
            VoiceTime.LoadContent(content);
            _customAudio.LoadContent(content);
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

            ///we deactivate and activate to tring on activate event on startup
            _secondClutchButton.Deactive();
            _secondClutchButton.Activate(true);

            _secondClutchButton.UpdateReleaseTimeButtons();
        }

        /// <summary>
        /// there is no point in checking every joystick input every update frame
        /// so we will catch the ones we need to update
        /// </summary>
        private List<int> _usedInputDevices = new List<int>();

        public void LoadAllInputDevices()
        {
            _usedInputDevices.Clear();

            JoystickCapabilities jCap;



            if (!_usedInputDevices.Contains(_secondClutchButtonIndex.InputDeviceIndex) && _secondClutchButtonIndex.InputDeviceIndex >= 0)
                _usedInputDevices.Add(_secondClutchButtonIndex.InputDeviceIndex); //add this to the list of inputs to check against


            foreach (Input input in _directionButtons)
            {
                if (!_usedInputDevices.Contains(input.InputDeviceIndex) && input.InputDeviceIndex >= 0)
                    _usedInputDevices.Add(input.InputDeviceIndex); //add this to the list of inputs to check against

            }
        }

        /// <summary>
        /// checks the input
        /// </summary>
        /// <param name="i">the input to check</param>
        /// <returns>returns 0 to 1 0 .5> is true</returns>
        public float IsWheelInputPressed(Input i)
        {
            if (i.InputDeviceIndex <= 0) //if no input device
                return 0;
            if (i.Index == -1) //if the input button / axis is null
                return 0;

            JoystickState _inputWheel = _currentJoystickStates[i.InputDeviceIndex];
            
            switch (i.Type)
            {
                case InputType.Button:
                    if (_inputWheel.Buttons != null)
                        if (_inputWheel.Buttons.Length > 0 && i.Index >= 0 && i.Index < _inputWheel.Buttons.Length)
                            if (_inputWheel.Buttons[i.Index] == ButtonState.Pressed)
                                return 1;
                    break;

                case InputType.HatDown:
                case InputType.HatLeft:
                case InputType.HatRight:
                case InputType.HatUp:
                    if (_inputWheel.Hats != null)
                        if (_inputWheel.Hats.Length > 0)
                            if (i.Index >= 0 && i.Index < _inputWheel.Hats.Length)
                            {
                                if (i.Type == InputType.HatUp)
                                    if (_inputWheel.Hats[i.Index].Up == ButtonState.Pressed)
                                        return 1;

                                if (i.Type == InputType.HatDown)
                                    if (_inputWheel.Hats[i.Index].Down == ButtonState.Pressed)
                                        return 1;

                                if (i.Type == InputType.HatLeft)
                                    if (_inputWheel.Hats[i.Index].Left == ButtonState.Pressed)
                                        return 1;

                                if (i.Type == InputType.HatRight)
                                    if (_inputWheel.Hats[i.Index].Right == ButtonState.Pressed)
                                        return 1;
                            }
                    break;
                case InputType.Anolog:
                    if (_inputWheel.Axes != null)
                        if (i.Index < _inputWheel.Axes.Length)
                            return (float)_inputWheel.Axes[i.Index] / MaxAxisValue;
                    break;
            }

            return 0;
        }

        
        /// <summary>
        /// the old Joystick states
        /// </summary>
        JoystickState[] _oldJoystickStates = new JoystickState[8];
        /// <summary>
        /// the new joystick states
        /// </summary>
        JoystickState[] _currentJoystickStates = new JoystickState[8];
        public Input GetInputButton()
        {
            if (KeyboardAPI.IsKeyPressed(Keys.Escape))
                return null;

            JoystickCapabilities jCapabilityes;
            JoystickState jState;
            for (int i = 0; i < 8; i++) //loop though all joystics
            {
                jCapabilityes = Joystick.GetCapabilities(i); //get joystick capabilitys
                if (jCapabilityes.IsConnected) //if is connectec
                {
                    jState = Joystick.GetState(i); //get its state

                    for (int c = 0; c < jCapabilityes.ButtonCount; c++)
                        if (jState.Buttons[c] == ButtonState.Pressed) //at this point we want to log the button and the input device
                        {
                            _oldJoystickStates[i] = jState;
                            return new Input(InputType.Button, c, jCapabilityes.Identifier);
                        }

                    for (int c = 0; c < jCapabilityes.HatCount; c++)
                    {
                        if (jState.Hats[c].Up == ButtonState.Pressed)
                        {
                            _oldJoystickStates[i] = jState;
                            return new Input(InputType.HatUp, c, jCapabilityes.Identifier);
                        }

                        if (jState.Hats[c].Down == ButtonState.Pressed)
                        {
                            _oldJoystickStates[i] = jState;
                            return new Input(InputType.HatDown, c, jCapabilityes.Identifier);
                        }

                        if (jState.Hats[c].Left == ButtonState.Pressed)
                        {
                            _oldJoystickStates[i] = jState;
                            return new Input(InputType.HatLeft, c, jCapabilityes.Identifier);
                        }

                        if (jState.Hats[c].Right == ButtonState.Pressed)
                        {
                            _oldJoystickStates[i] = jState;
                            return new Input(InputType.HatRight, c, jCapabilityes.Identifier);
                        }
                    }


                    for (int a = 0; a < jCapabilityes.AxisCount; a++)
                    {
                        if (_oldJoystickStates[i].Axes != null && _oldJoystickStates[i].Axes.Length > 0)
                            if (MathHelper.Distance(jState.Axes[a], _oldJoystickStates[i].Axes[a]) > 1000) //if more than half pressed
                            {
                                _oldJoystickStates[i] = jState;
                                return new Input(InputType.Anolog, a, jCapabilityes.Identifier);
                            }
                    }

                   
                    
                }
            }
            return -1;
        }

        private int _numberOfConnectedDevices = 0;
        private int _lastNumberOfConnectedDevices = 0;
        private int _maxClutchPointDetected = 0;
        public void Update(float dt)
        {
            VoiceTime.Update(dt);
            _customAudio.Update(dt);
            _game.Window.Title = _telemitry.testdata;

            if (!TimesOnlyMode)
            {
                if (!_vJoyConnected || !ValidVJoyConnection)
                {
                    _surfaceSettings.Update(dt);
                    return;
                }
             
            }
            //set old state
            _numberOfConnectedDevices = 0;
            for (int i = 0; i < 8; i++)
            {
                _oldJoystickStates[i] = _currentJoystickStates[i];
                _currentJoystickStates[i] = Joystick.GetState(i);
                if (_currentJoystickStates[i].IsConnected)
                    _numberOfConnectedDevices++;
            }

            if (_lastNumberOfConnectedDevices != _numberOfConnectedDevices)
                this.Reconnect();
            _lastNumberOfConnectedDevices = _numberOfConnectedDevices;
            //get all reaquid inputs
//            for (int i = 0; i < _usedInputDevices.Count; i++)
  //              _currentJoystickStates[_usedInputDevices[i]] = Joystick.GetState(_usedInputDevices[i]);

            switch (_currentState)
            {
                case WheelState.Run:
                    foreach (UiEliment b in _uiElements)
                        b.Update(dt);


                    break;

                case WheelState.Config:
                    Input bnt;
                    switch (_configState)
                    {
                        case ConfigArea.SetDirectionInput:
                            /*case ConfigArea.SetDirectionInput:
                                foreach (UiEliment b in _uiElements)
                                    b.Update(dt);
                                return;*/
                            bnt = GetInputButton();
                            if (bnt == null)
                            {
                                _currentState = WheelState.Run;
                                _surfaceSettings.Activate();
                                UpdateSuraceButtons();
                            }
                            else
                            if (bnt.Index != -1) //if a button is press
                            {
                                _directionButtons[(int)_inputDirection] = bnt;
                                _currentState = WheelState.Run;
                                _surfaceSettings.Activate();
                                UpdateSuraceButtons();
                                SaveConfig();
                            }
                            return;
                        case ConfigArea.DetectClutch:
                            bnt = GetInputButton();
                            if (bnt == null)
                            {
                                _currentState = WheelState.Run;
                                _surfaceSettings.Activate();
                                UpdateSuraceButtons();
                            }
                            else
                            if (bnt.Index != -1) //if a button is press
                            {
                                _secondClutchButtonIndex = bnt;
                                _currentState = WheelState.Run;
                                _surfaceSettings.Activate();
                                UpdateSuraceButtons();
                                SaveConfig();
                            }
                            break;
                    }
                    break;
            }



            if (_useRealSecondClutch)
            {
                float clutchOutput = (IsWheelInputPressed(_secondClutchButtonIndex) + 1) / 2f;
                _realSecondClutchInputAmount = clutchOutput;
                _secondClutchDepressedAmount = MathHelper.Clamp(_secondClutchBitingPoint * clutchOutput, 0, 1); //standard clamp
            }
            else
            {
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


                if (IsWheelInputPressed(_secondClutchButtonIndex) > .5f)
                    DepressSecondClutch(); //depresse the clutch

                _secondClutchDepressedAmount = MathHelper.Clamp(_secondClutchDepressedAmount, 0, 1); //standard clamp
            }


           

            //update the virtual joystick
            //now there is a really annoying thing about vjoy axis
            //when vjoy has no api setting data the axis default to half value
            //so if we run Iracing without runnig this app the clutch defaults to half pressed
            //therefore we have to componsate by not allowing the axis to go below half value
            //we loses half resolution but its worth the traid off
            if (ValidVJoyConnection && _vJoyConnected)
                _virtualJoyStick.SetJoystickAxis(Convert.ToInt32(HalfValue * _secondClutchDepressedAmount + HalfValue), Axis.HID_USAGE_Z);


            if (KeyboardAPI.IsKeyPressed(Keys.T))
                _telemitry.NextDriver();
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

                    if (_configState == ConfigArea.DetectClutch) //if wating for user to press a button display a promt
                    {
                        sb.Draw(_dot, new Rectangle(0, 0, 800, 500), Color.Black * .8f);
                        sb.DrawString(_font, "Press button for second clutch mapping", new Vector2(250, 250), Color.White, 0f,
                            _font.MeasureString("Press button for second clutch mapping") * .5f, .45f, SpriteEffects.None, 0f);
                        return;
                    }

                    text = "Press any button\non the wheel\nyou want to use";
                    sb.DrawString(_font, text, new Vector2(250,250), Color.White, 0f, _font.MeasureString(text)*.5f, 1f, SpriteEffects.None, 0f); 
                    break;

                case WheelState.Run:
                    if (!TimesOnlyMode)
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
                        if (!ValidVJoyConnection)
                        {
                            foreach (UiEliment b in _uiElements)
                                b.Draw(sb);

                            if (_surfaceSettings.IsActive)
                                return;

                            sb.Draw(_dot, new Rectangle(0, 0, 300, 500), Color.Black);
                            _uiElements[2].Draw(sb);
                            sb.DrawString(_font, "VJoy Error", new Vector2(10, 10), Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "To use the software clutch", new Vector2(10, 60), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "you must install vJoy.", new Vector2(10, 80), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                            sb.DrawString(_font, "If Vjoy is installed", new Vector2(10, 120), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "run configure vJoy", new Vector2(10, 140), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "make sure controler 1", new Vector2(10, 160), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "is enabled.", new Vector2(10, 180), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                            sb.DrawString(_font, "Also please make sure", new Vector2(10, 220), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "the the vJoy controler", new Vector2(10, 240), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "has its Z axis enabled.", new Vector2(10, 260), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);

                            sb.DrawString(_font, "You can turn on.", new Vector2(10, 300), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "Timings Mode Only", new Vector2(10, 320), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            sb.DrawString(_font, "in the settings", new Vector2(10, 340), Color.White, 0f, Vector2.Zero, .4f, SpriteEffects.None, 0f);
                            return;
                        }
                        
                    }
                   

                    foreach (UiEliment b in _uiElements)
                        b.Draw(sb);

                  //  if (TimesOnlyMode)
                    //    if (!_surfaceSettings.IsActive)
                      //      sb.Draw(_dot, new Rectangle(0, 0, 300, 400), new Color(51, 64, 69));

                    if (!_surfaceSettings.IsActive)
                    {
                        _secondClutchButton.DrawTelemitory(sb);
                    }

                    if (!TimesOnlyMode)
                        if (_useRealSecondClutch) //if using real clutch hide the release settings
                            if (_uiElements[1].IsActive)
                                sb.Draw(_dot, new Rectangle(180, 80, 100, 220), new Color(51,64,69));

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

                writer.WriteAttributeEnum<InputType>("SecondClutchInputType", _secondClutchButtonIndex.Type);
                writer.WriteAttributeInt("SecondClutchInputIndex", _secondClutchButtonIndex.Index);
                writer.WriteAttributeString("SecondClutchInputDeviceId", _secondClutchButtonIndex.InputID);

                writer.WriteAttributeFloat("SecondClutchBite", _secondClutchBitingPoint);
                writer.WriteAttributeFloat("SecondClutchReleaseTime", _secondClutchRelaseTime);

                writer.WriteAttributeEnum<InputType>("InputUpType", _directionButtons[(int)CardinalDirection.Up].Type);
                writer.WriteAttributeInt("InputUpIndex", _directionButtons[(int)CardinalDirection.Up].Index);
                writer.WriteAttributeString("InputUpIndexDeviceId", _directionButtons[(int)CardinalDirection.Up].InputID);

                writer.WriteAttributeEnum<InputType>("InputDownType", _directionButtons[(int)CardinalDirection.Down].Type);
                writer.WriteAttributeInt("InputDownIndex", _directionButtons[(int)CardinalDirection.Down].Index);
                writer.WriteAttributeString("InputDownIndexInputDeviceId", _directionButtons[(int)CardinalDirection.Down].InputID);

                writer.WriteAttributeEnum<InputType>("InputLeftType", _directionButtons[(int)CardinalDirection.Left].Type);
                writer.WriteAttributeInt("InputLeftIndex", _directionButtons[(int)CardinalDirection.Left].Index);
                writer.WriteAttributeString("InputLeftTypeInputDeviceId", _directionButtons[(int)CardinalDirection.Left].InputID);

                writer.WriteAttributeEnum<InputType>("InputRightType", _directionButtons[(int)CardinalDirection.Right].Type);
                writer.WriteAttributeInt("InputRightIndex", _directionButtons[(int)CardinalDirection.Right].Index);
                writer.WriteAttributeString("InputRightTypeInputDeviceId", _directionButtons[(int)CardinalDirection.Right].InputID);

                writer.WriteAttributeBool("AutoLoadBestSetup", AutoLoadFastestSetup);
                writer.WriteAttributeBool("TimingsOnlyMode", TimesOnlyMode);
                writer.WriteAttributeBool("SpeakTimings", SayTimingOutloud);
                writer.WriteAttributeBool("UseRealClutch", _useRealSecondClutch);
                writer.WriteAttributeFloat("VoiceVolume", _voiceVolume);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }


        private bool _isConnected;
        public void Reconnect()
        {
          

            _secondClutchButtonIndex = new Input(_secondClutchButtonIndex.Type, _secondClutchButtonIndex.Index, _secondClutchButtonIndex.InputID);
            _directionButtons[(int)CardinalDirection.Up] = new Input(_directionButtons[(int)CardinalDirection.Up].Type,  _directionButtons[(int)CardinalDirection.Up].Index,  _directionButtons[(int)CardinalDirection.Up].InputID);
            _directionButtons[(int)CardinalDirection.Down] = new Input(_directionButtons[(int)CardinalDirection.Down].Type, _directionButtons[(int)CardinalDirection.Down].Index, _directionButtons[(int)CardinalDirection.Down].InputID);
            _directionButtons[(int)CardinalDirection.Left] = new Input(_directionButtons[(int)CardinalDirection.Left].Type, _directionButtons[(int)CardinalDirection.Left].Index, _directionButtons[(int)CardinalDirection.Left].InputID);
            _directionButtons[(int)CardinalDirection.Right] = new Input(_directionButtons[(int)CardinalDirection.Right].Type, _directionButtons[(int)CardinalDirection.Right].Index, _directionButtons[(int)CardinalDirection.Right].InputID);
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
                        

                            ///this is for a temperary fix untill i move compleately over to the new system
                            string tmp = reader.ReadAttributeString("SecondClutchInputDeviceId");
                            _secondClutchButtonIndex = new Input(reader.ReadAttributeEnum<InputType>("SecondClutchInputType"), reader.ReadAttributeInt("SecondClutchInputIndex"), tmp);

                            _secondClutchBitingPoint = reader.ReadAttributeFloat("SecondClutchBite");
                            _secondClutchRelaseTime = reader.ReadAttributeFloat("SecondClutchReleaseTime");


                            InputType t = reader.ReadAttributeEnum<InputType>("InputUpType");
                            int i = reader.ReadAttributeInt("InputUpIndex");
                            _directionButtons[(int)CardinalDirection.Up] = new Input(t, i, reader.ReadAttributeString("InputUpIndexDeviceId"));

                            t = reader.ReadAttributeEnum<InputType>("InputDownType");
                            i = reader.ReadAttributeInt("InputDownIndex");
                            _directionButtons[(int)CardinalDirection.Down] = new Input(t, i, reader.ReadAttributeString("InputDownIndexInputDeviceId"));

                            t = reader.ReadAttributeEnum<InputType>("InputLeftType");
                            i = reader.ReadAttributeInt("InputLeftIndex");
                            _directionButtons[(int)CardinalDirection.Left] = new Input(t, i, reader.ReadAttributeString("InputLeftIndexInputDeviceId"));

                            t = reader.ReadAttributeEnum<InputType>("InputRightType");
                            i = reader.ReadAttributeInt("InputRightIndex");
                            _directionButtons[(int)CardinalDirection.Right] = new Input(t, i, reader.ReadAttributeString("InputRightIndexInputDeviceId"));

                            AutoLoadFastestSetup = reader.ReadAttributeBool("AutoLoadBestSetup");
                            TimesOnlyMode = reader.ReadAttributeBool("TimingsOnlyMode");

                            SayTimingOutloud = reader.ReadAttributeBool("SpeakTimings");

                            _useRealSecondClutch = reader.ReadAttributeBool("UseRealClutch");
                            _voiceVolume = reader.ReadAttributeFloat("VoiceVolume");
                        }
                    }

                    reader.Close();
                }

                _currentState = WheelState.Run;
            }
            catch
            {
                _voiceVolume = 1f;   
                _secondClutchButtonIndex = -1;
            }

            SoundEffect.MasterVolume = MathHelper.Clamp(_voiceVolume, 0, 1);

            LoadAllInputDevices();
          

            ///update the buttons to the correct state
            

        }
    }
}
