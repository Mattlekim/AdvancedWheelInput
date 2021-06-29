using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using iRacingSDK;
using Riddlersoft.Core.Xml;

namespace AdvancedInput
{

    /// <summary>
    /// the telemitry class is what does all the work with getting all the relervent iracing data
    /// </summary>
    class IRacingTelemitry : GameComponent
    {

        /// <summary>
        /// the speed in miles per hour
        /// </summary>
        public float SpeedMph { get; private set; }

        /// <summary>
        /// if we are connect to telemitory or not
        /// </summary>
        public bool IsConnected { get; private set; } = false;
        private float _timeSinceGotData = 5f;

        /// <summary>
        /// name of current car
        /// </summary>
        public string CurrentCar { get; private set; }

        public List<TimeRecord> TimeRecords = new List<TimeRecord>();
        /// <summary>
        /// runs on connection to iracing
        /// </summary>
        public Action OnConnected;
        /// <summary>
        /// runs on disconneced from iracing
        /// </summary>
        public Action OnDisconected;

        /// <summary>
        /// the advance wheel
        /// </summary>
        private AdvanceWheel _wheel;
        public IRacingTelemitry(Game game) : base(game)
        {
            iRacing.NewData += iRacing_NewData; //set get new data function
            iRacing.StartListening(); //start listing to iracing
            OnConnected += OnConnect;
        }

        /// <summary>
        /// when 0 to 60 is reachead
        /// </summary>
        public Action<TimeRecord> On0To60;

        /// <summary>
        /// reset the times
        /// </summary>
        public void ClearCurrentCarTimes()
        {
            TimeRecords.Clear();
        }

        private void SaveSoon()
        {
            _timeTillSave = 5f; //5 seconds to save
        }
        public void UpdateTopSpeed()
        {
            if (!_logTopSpeed)
                return;

            if (SpeedMph > _previousTopSpeed)
            {
                _previousTopSpeed = SpeedMph;
                TimeRecords[TimeRecords.Count - 1].TopSpeed = SpeedMph;
                SaveSoon();
            }
        }
        /// <summary>
        /// add new record to the time list
        /// </summary>
        public void AddTimeRecord0To60()
        {
            //make sure we can add
            if (CurrentCar == null || CurrentCar == string.Empty)
                return;

            _logTopSpeed = true;
            if (TimeRecords.Count >= 19) //if we where over the limint of what can be listed
            {
                //find the worst score and delete it
                float lowert = float.MinValue;
                int index = -1;

                for (int i = 0; i < TimeRecords.Count; i++)
                    if (TimeRecords[i].ZeroToSixty > lowert)
                    {
                        index = i;
                        lowert = TimeRecords[i].ZeroToSixty;
                    }

                if (index != -1)
                    TimeRecords.RemoveAt(index);
            }

            //create the record
            TimeRecord tr = new TimeRecord(_0to60Time, _wheel._secondClutchBitingPoint, _wheel._secondClutchRelaseTime, _holdTime)
            {
                WasClutchStart = _usedSecondClutch,
            };

            //add the record
            TimeRecords.Add(tr);

            //trigger the 0 to 60
            if (On0To60 != null)
                On0To60(tr);

            //save telimtory data
            SaveTelimtory();
        }

        /// <summary>
        /// add the time for 0 to 100
        /// </summary>
        public void AddTimeRecord0To100()
        {
            TimeRecords[TimeRecords.Count - 1].ZeroToOnehundrand = _0to60Time;
            SaveTelimtory();
        }

        /// <summary>
        /// add the time for 0 to 100
        /// </summary>
        public void AddTimeRecord0To150()
        {
            TimeRecords[TimeRecords.Count - 1].ZeroToOneFifty = _0to60Time;
            SaveTelimtory();
        }

        /// <summary>
        /// set the advanced wheel
        /// </summary>
        /// <param name="wheel"></param>
        public void SetWheel(AdvanceWheel wheel)
        {
            _wheel = wheel;
            //AddTimeRecord0To60();
        }

        private int _playerCar = -1;

        public string testdata;
        void iRacing_NewData(DataSample data)
        {
            if (data.Telemetry.CarDetails.Length > 1)
                CurrentCar = data.Telemetry.CarDetails[1].Driver.CarScreenName;
            else
                CurrentCar = data.Telemetry.CarDetails[0].Driver.CarScreenName;

            SpeedMph = data.Telemetry.Speed * 2.25f;
            if (!IsConnected)
            {
                if (OnConnected != null)
                    OnConnected();
            }
            _timeSinceGotData = 5f;
            IsConnected = true;

            //testdata = $"{data.Telemetry.RRwearM} / {data.Telemetry.RRtempCM}";
            testdata = data.Telemetry.LatAccel.ToString();
        }

        public void OnConnect()
        {
            LoadTelimtory();
        }
        public override void Initialize()
        {
            base.Initialize();
        }

        private bool _log0to60 = false;
        private bool _log0to100 = false;
        private bool _log0to150 = false;
        private float _0to60Time = 0;
        public bool _usedSecondClutch = false;
        private float _holdTime = 0;


        private bool _logTopSpeed = false;
        private float _previousTopSpeed = 0;

        private float _timeTillSave = -1f;

        private void SetUpStartCheck()
        {
            _0to60Time = 0;

            _previousTopSpeed = 0;
            _logTopSpeed = false;

            _log0to60 = true;
            _log0to100 = false;
            _log0to150 = false;
            
            _holdTime = 0;
            if (_wheel._secondClutchDepressedAmount > 0)
                _usedSecondClutch = true;
            else
                _usedSecondClutch = false;

         
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timeSinceGotData -= dt;

            UpdateTopSpeed();

            _timeTillSave -= dt;
            _timeTillSave = MathHelper.Clamp(_timeTillSave, -1, 1000000f);
            
            if (_timeTillSave <= 0 && _timeTillSave + dt > 0f)
                SaveTelimtory();

            if (IsConnected)
                if (_timeSinceGotData <= 0)
                {
                    _log0to60 = false;
                    IsConnected = false;
                    if (OnDisconected != null)
                        OnDisconected();
                }

            _0to60Time += dt;

            if (_log0to100 | _log0to150)
            {
                if (SpeedMph >= 100)
                {
                    if (_log0to100)
                    {
                        AddTimeRecord0To100();
                        _log0to100 = false;
                        _log0to150 = true;
                    }
                    else
                    if (SpeedMph >= 150)
                    {
                        AddTimeRecord0To150();
                        _log0to150 = false;
                    }
                }
                else
                    if (SpeedMph < 60)
                {
                    _log0to100 = false;
                    _log0to150 = false;
                }
                return;
            }

            if (_log0to60)
            {
                if (SpeedMph > 0.02f)
                {
                    if (_wheel.IsWheelInputPressed(_wheel._secondClutchButtonIndex) > .5f)
                        _holdTime += dt;
                    if (SpeedMph >= 60)
                    {
                        _log0to60 = false;
                        _log0to100 = true;
                        AddTimeRecord0To60();
                    }
                }
                else
                {
                    SetUpStartCheck();
                }
            }
            else
            if (SpeedMph < .02f)
            {
                SetUpStartCheck();
            }

        }

        public string path;

        public void DeleteTelimtoryFile()
        {
            string tmp = CurrentCar.Replace("-", "");
            tmp = tmp.Replace(" ", "");
            path = $"{Dir}\\{tmp}.tel";
            if (File.Exists(path))
                File.Delete(path);
        }
        public void SaveTelimtory()
        {
            if (!Directory.Exists($"{Dir}"))
                Directory.CreateDirectory($"{Dir}");

            if (TimeRecords == null || TimeRecords.Count <= 0)
                return;


            string tmp = CurrentCar.Replace("-", "");
            tmp = tmp.Replace(" ", "");
            path  = $"{Dir}\\{tmp}.tel";
            using (FileStream fs = File.Create(path))
            {
                CustomXmlWriter writer = CustomXmlWriter.Create(fs);
                writer.WriteStartDocument();
                writer.WriteStartElement("Car");

                foreach (TimeRecord tr in TimeRecords)
                    if (tr.WasClutchStart)
                    {
                        writer.WriteStartElement("Time");

                        writer.WriteAttributeFloat("ztosix", tr.ZeroToSixty);
                        writer.WriteAttributeFloat("ztoone", tr.ZeroToOnehundrand);
                        writer.WriteAttributeFloat("ztoonefifty", tr.ZeroToOneFifty);
                        writer.WriteAttributeFloat("Biting", tr.ClutchBitingPoint);
                        writer.WriteAttributeFloat("Release", tr.ClutchReleaseTime);
                        writer.WriteAttributeFloat("HoldTime", tr.HoldTime);
                        writer.WriteAttributeFloat("TopSpeed", tr.TopSpeed);
                        writer.WriteEndElement();
                    }



                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }

        }

        private const string Dir = "Tel";
        public void LoadTelimtory()
        {
            
            string tmp = CurrentCar.Replace("-", "");
            tmp = tmp.Replace(" ", "");
            path = $"{Dir}\\{tmp}.tel";
            //path = $"{tmp}.tel";
            TimeRecords.Clear();

            if (File.Exists(path))
            {



                try
                {
                    using (FileStream fs = File.OpenRead(path))
                    {
                        CustomXmlReader reader = CustomXmlReader.Create(fs);

                        while (reader.Read())
                        {
                            if (reader.Name == "Time")
                            {
                                if (TimeRecords.Count < 19)
                                    TimeRecords.Add(new TimeRecord()
                                    {
                                        ClutchBitingPoint = reader.ReadAttributeFloat("Biting"),
                                        ClutchReleaseTime = reader.ReadAttributeFloat("Release"),
                                        ZeroToSixty = reader.ReadAttributeFloat("ztosix"),
                                        ZeroToOnehundrand = reader.ReadAttributeFloat("ztoone"),
                                        ZeroToOneFifty = reader.ReadAttributeFloat("ztoonefifty"),
                                        HoldTime = reader.ReadAttributeFloat("HoldTime"),
                                        TopSpeed = reader.ReadAttributeFloat("TopSpeed"),
                                        WasClutchStart = true,
                                    });
                            }
                        }

                        reader.Close();
                    }


                    if (_wheel.AutoLoadFastestSetup)
                    {
                        int index = -1;
                        float time = float.PositiveInfinity;
                        for (int i = 0; i < TimeRecords.Count; i++)
                            if (time > TimeRecords[i].ZeroToSixty)
                            {
                                time = TimeRecords[i].ZeroToSixty;
                                index = i;
                            }

                        if (index >= 0)
                        {
                            _wheel._secondClutchBitingPoint = TimeRecords[index].ClutchBitingPoint;
                            _wheel._secondClutchRelaseTime = TimeRecords[index].ClutchReleaseTime;
                            _wheel._secondClutchButton.SetSelectedTime(index);
                            
                        }
                    }
                }
                catch { }
            }
        }

    }
}
