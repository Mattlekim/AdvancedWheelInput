using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Riddlersoft.Core;

namespace AdvancedInput.UI
{
    /// <summary>
    /// our custome button that will do all our second clutch stuff
    /// </summary>
    public class SecondClutchButton : Button
    {
        //sliders for biting point and relase time
        private Slider _slBitingPoint;
        private Slider _slReleaseTime;

        //other ui buttons
        private Button _bntSaveConfig;
        private Button _bntSetInput;

        public bool LockControles { get; private set; } = true;
        /// <summary>
        /// we need this to make sure we dont screw things up when we unlock stuff
        /// </summary>
        private bool _lockTillRelease = false;
        //timer for amout of fade.
        //we use this to put a display to tell the user is saved
        private float _showSaved = 0;

        private int _sliderToChange = 0;

        private Button _bntSlowRelease, _bntFastRelease;

        public SecondClutchButton(AdvanceWheel wheel) : base(wheel, new Rectangle(0, 0, 300, 500))
        {
            //some basic formatting
            PrimaryColour = Color.LightBlue * .3f;
            SecondryColour = Color.OrangeRed * .5f;
            HaveMouseOver = false;
            //----------------CREATE UI ELEMENTS---------------------------
            _slBitingPoint = new UI.Slider(new Rectangle(50, 80, 50, 200), 1, 0, .8f)
            {
                TextName = "Biting Point",
                TextScale = .5f,
                Current = wheel._secondClutchBitingPoint,
                OnChange = (Slider s) =>
                {
                    wheel._secondClutchBitingPoint = s.GetValue();
                },
            };


            
            _slReleaseTime = new UI.Slider(new Rectangle(150, 80, 50, 200), 3, 0, .5f)
            {
                TextName = "Release Time\n(Seconds)",
                TextScale = .3f,
                Current = 1f / (wheel._secondClutchRelaseTime * 3f),
                OnChange = (Slider s) =>
                {
                    wheel._secondClutchRelaseTime = 1f / s.GetValue();
                },
            };

            if (Wheel._useNewReleaseMethord)
            {
                _slReleaseTime.Deactive();
                _slReleaseTime.Locked = true;

                _bntSlowRelease = new Button(wheel, new Rectangle(185, 80, 70, 70))
                {
                    PrimaryColour = Color.LightBlue * .5f,
                    ButtonText = "  Slow\nRelease",
                    TextScale = .3f,
                    Sticky = true,
                    TextShadow = true,
                    OnClick = (Button b)=>
                    {
                        _bntFastRelease.ResetButtonState();
                        Wheel._secondClutchRelaseTime = 2f;
                    }
                };

                _bntFastRelease = new Button(wheel, new Rectangle(185, 210, 70, 70))
                {
                    PrimaryColour = Color.LightBlue * .5f,
                    ButtonText = "  Quick\nRelease",
                    TextScale = .3f,
                    Sticky = true,
                    TextShadow = true,
                    OnClick = (Button b) =>
                    {
                        _bntSlowRelease.ResetButtonState();
                        Wheel._secondClutchRelaseTime = 1f;
                    }
                }; 

                AddElement(_bntSlowRelease);
                AddElement(_bntFastRelease);
            }


            _bntSetInput = new UI.Button(wheel, new Rectangle(0, 410, 80, 70))
            {
                PrimaryColour = Color.Orange,
                SecondryColour = Color.OrangeRed,
                ButtonText = "Set Input\nIn Iracing",
                TextScale = .3f,
                TextShadow = true,
                OnClick = (Button b) =>
                {
                    wheel.DepressSecondClutch(true);
                }
            };

            _bntSaveConfig = new UI.Button(wheel, new Rectangle(110, 400, 80, 80))
            {
                PrimaryColour = Color.Transparent,
                SecondryColour = Color.Green,
                ButtonText = "Save",
                Icon = Content.Load<Texture2D>("Imgs\\saveicon"),
             
                TextScale = .3f,
                TextShadow = true,
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

        /// <summary>
        /// used to set the input state for the new release system
        /// any value 1 or lower is quick release
        /// anything else is slow relase
        /// </summary>
        /// <param name="inputValue"></param>
        public void SetReleaseState(float inputValue)
        {
            _bntFastRelease.ResetButtonState();
            _bntSlowRelease.ResetButtonState();
            if (inputValue <= 1)
                _bntFastRelease.SetPressed();
            else
                _bntSlowRelease.SetPressed();
                
        }

        float _flasher;

        /// <summary>
        /// the time to highlihgt
        /// </summary>
        int _selectedTime = -1;


        public void SetSelectedTime(int index)
        {
            if (index < 0)
                return;
            if (index >= Wheel._telemitry.TimeRecords.Count)
                return;
            _selectedTime = index;
        }

        private bool _haveLoadedContent = false;
        public override void Update(float dt)
        {
            base.Update(dt); //base update for all ui elements

            if (!_active)
                return;

            if (!_haveLoadedContent)
            {
               
                _haveLoadedContent = true;
            }
            //showing save display
            _showSaved -= dt * .75f;
            _showSaved = MathHelper.Clamp(_showSaved, 0, 1);

       

            _flasher += dt * 2; //flasher to display text to flas
            if (_flasher > 1)
                _flasher -= 2; //limit it to -1 to 1

          /*


            if (DetectSecondClutchInput) //if we are waiting to detect a button press for clutch
            {
                for (int i = 0; i < Wheel._inputWheel.Buttons.Length; i++) //check all buttons on wheel
                {
                    if (Wheel._inputWheel.Buttons[i] == Microsoft.Xna.Framework.Input.ButtonState.Pressed) //look for press
                    {
                        DetectSecondClutchInput = false; //turn the flag off
                        Wheel._secondClutchButtonIndex = i; //set the button
                        Wheel.SaveConfig(); //save the config
                        break;
                    }
                }
            }

            */

            //  if (Wheel._telemitry.IsConnected)
            if (SimpleMouse.IsLButtonClick)
                for (int i = 0; i < Wheel._telemitry.TimeRecords.Count; i++)
                {
                    if (new Rectangle(300, 80 + i * 20, 500, 20).Contains(SimpleMouse.Pos))
                    {
                        TimeRecord tr = Wheel._telemitry.TimeRecords[i];
                        Wheel._secondClutchBitingPoint = tr.ClutchBitingPoint;
                        Wheel._secondClutchRelaseTime = tr.ClutchReleaseTime;
                        _selectedTime = i;
                        UpdateSliders();
                        return;
                    }
                }

            if (_lockTillRelease)
            {
                bool l = Wheel.IsWheelInputPressed(Wheel._directionButtons[0]) > .5f || Wheel.IsWheelInputPressed(Wheel._directionButtons[1]) > .5f || Wheel.IsWheelInputPressed(Wheel._directionButtons[2]) > .5f || Wheel.IsWheelInputPressed(Wheel._directionButtons[3]) > .5f;
                if (!l)
                {
                    _lockTillRelease = false;
                    LockControles = false;
                }
                return;
            }

            if (LockControles)
            {
                if (Wheel.IsWheelInputPressed(Wheel._directionButtons[0]) > .5f && Wheel.IsWheelInputPressed(Wheel._secondClutchButtonIndex) > .5f)
                {
                    
                    _lockTillRelease = true;
                    _timeTillLock = 60;
                }
            }
            else
            {
                _timeTillLock -= dt;
                bool l = Wheel.IsWheelInputPressed(Wheel._directionButtons[0]) > .5f || Wheel.IsWheelInputPressed(Wheel._directionButtons[1]) > .5f || Wheel.IsWheelInputPressed(Wheel._directionButtons[2]) > .5f || Wheel.IsWheelInputPressed(Wheel._directionButtons[3]) > .5f;
                if (l)
                    _timeTillLock = 60;
                
                if (_timeTillLock <= 0)
                    LockControles = true;

                if (Wheel.IsWheelInputPressed(Wheel._directionButtons[(int)CardinalDirection.Left]) > .5f) //slow relase
                {
                    if (Wheel._useNewReleaseMethord)
                    {
                        Wheel._secondClutchRelaseTime = 2f;
                        _bntSlowRelease.ResetButtonState();
                        _bntFastRelease.ResetButtonState();
                        _bntSlowRelease.SetPressed();
                    }
                    _sliderToChange--;
                }

                if (Wheel.IsWheelInputPressed(Wheel._directionButtons[(int)CardinalDirection.Right]) > .5f) //fast relase
                {
                    if (Wheel._useNewReleaseMethord)
                    {
                        Wheel._secondClutchRelaseTime = .5f;

                        _bntSlowRelease.ResetButtonState();
                        _bntFastRelease.ResetButtonState();
                        _bntFastRelease.SetPressed();
                    }
                    _sliderToChange++;
                }
                _sliderToChange = MathHelper.Clamp(_sliderToChange,0,1);
                if (_sliderToChange == 0)
                {
                    if (Wheel.IsWheelInputPressed(Wheel._directionButtons[(int)CardinalDirection.Up]) > .5f)
                    {
                        
                        Wheel._secondClutchBitingPoint += .01f;
                        Wheel._secondClutchBitingPoint = MathHelper.Clamp(Wheel._secondClutchBitingPoint, 0, 1);
                        Wheel._secondClutchBitingPoint = (float)Math.Round(Wheel._secondClutchBitingPoint, 2);
                        
                        UpdateSliders();
                    }

                    if (Wheel.IsWheelInputPressed(Wheel._directionButtons[(int)CardinalDirection.Down]) > .5f)
                    {
                        Wheel._secondClutchBitingPoint -= .01f;
                        Wheel._secondClutchBitingPoint = MathHelper.Clamp(Wheel._secondClutchBitingPoint, 0, 1);
                        Wheel._secondClutchBitingPoint = (float)Math.Round(Wheel._secondClutchBitingPoint, 2);
                        UpdateSliders();
                    }
                }
                if (_sliderToChange == 1)
                {
                    if (Wheel.IsWheelInputPressed(Wheel._directionButtons[(int)CardinalDirection.Up]) > .5f)
                    {
                        _slReleaseTime.Current += 0.01f;
                        _slReleaseTime.Current = MathHelper.Clamp(_slReleaseTime.Current, 0, 1);
                        Wheel._secondClutchRelaseTime = 1f / _slReleaseTime.GetValue();
                        // Wheel._secondClutchRelaseTime += .01f;
                        //  Wheel._secondClutchRelaseTime = MathHelper.Clamp(Wheel._secondClutchRelaseTime, 0, 1);
                        //  Wheel._secondClutchRelaseTime = (float)Math.Round(Wheel._secondClutchRelaseTime, 2);
                        // UpdateSliders();
                    }

                    if (Wheel.IsWheelInputPressed(Wheel._directionButtons[(int)CardinalDirection.Down]) > .5f)
                    {
                        _slReleaseTime.Current -= 0.01f;
                        _slReleaseTime.Current = MathHelper.Clamp(_slReleaseTime.Current, 0, 1);
                        Wheel._secondClutchRelaseTime = 1f / _slReleaseTime.GetValue();
                        //  Wheel._secondClutchRelaseTime -= .01f;
                        //  Wheel._secondClutchRelaseTime = MathHelper.Clamp(Wheel._secondClutchRelaseTime, 0, 1);
                        //  Wheel._secondClutchRelaseTime = (float)Math.Round(Wheel._secondClutchRelaseTime, 2);
                        //  UpdateSliders();
                    }
                }
            }

        }

        public void UpdateSliders()
        {
            _slBitingPoint.Current = Wheel._secondClutchBitingPoint;
            _slReleaseTime.Current = 1f / (Wheel._secondClutchRelaseTime * 3f);
        }

        public override void OnLClick(Vector2 pos)
        {
            if (!_active)
                return;
            

        }

        float _timeTillLock = 0;
        public override void Draw(SpriteBatch sb)
        {
            if (!_active)
                return;

            //draw save text when needed
            sb.Draw(Dot, new Rectangle(0, 430, 800, 50), Color.Green * _showSaved);
            sb.DrawString(Font, "Saved Setup", new Vector2(400, 455), Color.White * _showSaved, 0f, Font.MeasureString("Saved Setup") * .5f, .8f, SpriteEffects.None, 0f);


            if (Wheel._secondClutchButtonIndex.Index == -1) //if we dont have a button set
            {



                base.Draw(sb);
                sb.Draw(Dot, new Rectangle(0, 0, 500, 400), Color.Black * .8f);
                sb.DrawString(Wheel._font, "Second Clutch Needs", new Vector2(10, 145), Color.White , 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
                sb.DrawString(Wheel._font, "Setting up", new Vector2(80, 175), Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
                

                return;
            }


            if (LockControles)
            {
                sb.DrawString(Font, "Settings Are Locked", new Vector2(25, 310), Color.Orange, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
                sb.DrawString(Font, "Press Up and Clutch Button\n At Same Time To Release", new Vector2(45, 340), Color.Orange, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
            }
            else
            {
                sb.DrawString(Font, $"Settings Will Lock In {(int)_timeTillLock}s", new Vector2(15, 310), Color.Orange, 0f, Vector2.Zero, .45f, SpriteEffects.None, 0f);
                sb.DrawString(Font, $"Of Inativity", new Vector2(95, 340), Color.Orange, 0f, Vector2.Zero, .45f, SpriteEffects.None, 0f);
            }

                sb.DrawString(Wheel._font, $"Clutch", new Vector2(10, 10), Color.White);
            base.Draw(sb);

            if (Wheel._telemitry.IsConnected || Wheel._telemitry.CurrentCar != null & Wheel._telemitry.CurrentCar != string.Empty)
            {
                sb.Draw(Dot, new Rectangle(300, 0, 500, 50), Color.LightBlue * .5f);
                string cont = "Connected";
                if (!Wheel._telemitry.IsConnected)
                    cont = "Not Connected!";
                sb.DrawString(Font, $"{cont} {Wheel._telemitry.CurrentCar}", new Vector2(350, 10), Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);

                sb.Draw(Dot, new Rectangle(300, 50, 500, 25), Color.LightBlue * .8f);

                sb.DrawString(Font, $"0 to 60", new Vector2(320, 52), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                sb.DrawString(Font, $"0 - 100 ", new Vector2(410, 52), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                sb.DrawString(Font, $"Biting Point", new Vector2(490, 52), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                sb.DrawString(Font, $"Release Time", new Vector2(600, 52), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                sb.DrawString(Font, $"Hold Time", new Vector2(715, 52), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);

                Color col = Color.LightBlue;
                float fade = .5f;
                for (int i = 0; i < Wheel._telemitry.TimeRecords.Count; i++)
                {
                    if (i % 2 == 0)
                        fade = .25f;
                    else
                        fade = .5f;

                    if (_selectedTime == i)
                    {
                        col = Color.Red;
                        fade = .5f;
                    }
                    else
                    {
                        col = Color.LightBlue;

                    }

                    sb.Draw(Dot, new Rectangle(300, 80 + i * 20, 500, 20), col * (.8f * fade));
                    TimeRecord tr = Wheel._telemitry.TimeRecords[i];
                    if (!tr.WasClutchStart)
                        sb.DrawString(Font, $"*", new Vector2(310, 85 + 20 * i), Color.Orange, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                    sb.DrawString(Font, $"{Math.Round(tr.ZeroToSixty, 2)}s", new Vector2(330, 80 + 20 * i), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                    if (Wheel._useNewReleaseMethord)
                    {
                        if (Wheel._telemitry.TimeRecords[i].ClutchReleaseTime <= 1) //fast
                            sb.DrawString(Font, $"Fast", new Vector2(630, 80 + 20 * i), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                        else
                            sb.DrawString(Font, $"Slow", new Vector2(630, 80 + 20 * i), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                    }
                    else
                        sb.DrawString(Font, $"{Math.Round(1 / tr.ClutchReleaseTime, 2)} ", new Vector2(630, 80 + 20 * i), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                    sb.DrawString(Font, $"{Math.Round(tr.ZeroToOnehundrand, 2)}s", new Vector2(420, 80 + 20 * i), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                    sb.DrawString(Font, $"{Math.Round(tr.ClutchBitingPoint, 2)}", new Vector2(510, 80 + 20 * i), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);

                    sb.DrawString(Font, $"{Math.Round(tr.HoldTime, 2)}s", new Vector2(730, 80 + 20 * i), Color.White, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
                }
            }

        }




        public override void OnLButtonDown(Vector2 pos)
        {
            throw new NotImplementedException();
        }
    }
}
