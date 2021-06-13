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

   
    class IRacingTelemitry : GameComponent
    {

        public float SpeedMph { get; private set; }

        public bool IsConnected { get; private set; } = false;
        private float _timeSinceGotData = 5f;


        public string CurrentCar { get; private set; }

        public List<TimeRecord> TimeRecords = new List<TimeRecord>();
        public Action OnConnected;
        public Action OnDisconected;

        private AdvanceWheel _wheel;
        public IRacingTelemitry(Game game) : base(game)
        {
            iRacing.NewData += iRacing_NewData;
            iRacing.StartListening();
            OnConnected += OnConnect;
        }

        public void AddTimeRecord0To60()
        {
            if (CurrentCar == null || CurrentCar == string.Empty)
                return;

            if (TimeRecords.Count >= 21)
            {
                float lowert = float.MinValue;
                int index = -1;

                for (int i =0; i < TimeRecords.Count; i++)
                    if (TimeRecords[i].ZeroToSixty > lowert)
                    {
                        index = i;
                        lowert = TimeRecords[i].ZeroToSixty;
                    }

                if (index != -1)
                    TimeRecords.RemoveAt(index);
            }

            TimeRecords.Add(new TimeRecord(_0to60Time, _wheel._secondClutchBitingPoint, _wheel._secondClutchRelaseTime)
            {
                WasClutchStart = _usedSecondClutch,
            });

            SaveTelimtory();
        }

        public void AddTimeRecord0To100()
        {
            TimeRecords[TimeRecords.Count - 1].ZeroToOnehundrand = _0to60Time;
            SaveTelimtory();
        }
        public void SetWheel(AdvanceWheel wheel)
        {
            _wheel = wheel;
            //AddTimeRecord0To60();
        }

        private int _playerCar = -1;
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
        private float _0to60Time = 0;
        public bool _usedSecondClutch = false;
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

            if (_log0to100)
            {
                if (SpeedMph >= 100)
                {
                    AddTimeRecord0To100();
                    _log0to100 = false;
                }
                else
                    if (SpeedMph < 60)
                        _log0to100 = false;
                return;
            }

            if (_log0to60)
            {
                if (SpeedMph > 0.02f)
                {
                    if (SpeedMph >= 60)
                    {
                        _log0to60 = false;
                        _log0to100 = true;
                        AddTimeRecord0To60();
                    }
                }
                else
                {
                    _0to60Time = 0;
                    _log0to60 = true;
                    if (_wheel._secondClutchDepressedAmount > 0)
                        _usedSecondClutch = true;
                    else
                        _usedSecondClutch = false;
                }
            }
            else
            if (SpeedMph < .02f)
            {
                _0to60Time = 0;
                _log0to60 = true;
                if (_wheel._secondClutchDepressedAmount > 0)
                    _usedSecondClutch = true;
                else
                    _usedSecondClutch = false;
            }

        }

        public string path;
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
                        writer.WriteAttributeFloat("Biting", tr.ClutchBitingPoint);
                        writer.WriteAttributeFloat("Release", tr.ClutchReleaseTime);

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
                                TimeRecords.Add(new TimeRecord()
                                {
                                    ClutchBitingPoint = reader.ReadAttributeFloat("Biting"),
                                    ClutchReleaseTime = reader.ReadAttributeFloat("Release"),
                                    ZeroToSixty = reader.ReadAttributeFloat("ztosix"),
                                    ZeroToOnehundrand = reader.ReadAttributeFloat("ztoone"),
                                    WasClutchStart = true
                                }); ;
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
