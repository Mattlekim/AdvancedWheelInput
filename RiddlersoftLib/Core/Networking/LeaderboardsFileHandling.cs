using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Riddlersoft.Core.Storage;
using Riddlersoft.Core.Xml;

namespace Riddlersoft.Core.Networking
{
    /// <summary>
    /// the file handling for the leaderboards
    /// </summary>
    public static partial class Leaderboards
    {
        private static string _savePath = "test.tst";

        /// <summary>
        /// wehere we need to save soon or not
        /// </summary>
        private static bool _saveSoon = false;
        private static bool AllowAutoSave = false;
        private static void UpdateStorage()
        {
            if (_saveSoon && AllowAutoSave)
            {
                Save();
                _saveSoon = false;
            }

        }
        
        public static void ForceSave()
        {
            _saveSoon = false;
            Save();
        }

        private static void Save()
        {
            PrintLine("Atempting Save of leaderboards");
            FileStream fs = UFile.Create(_savePath);

            CustomXmlWriter cxw = CustomXmlWriter.Create(fs);
            cxw.WriteStartDocument();
            cxw.WriteStartElement("LocalScores");
            foreach(QueuededScore qs in _scoresToSend)
            {
                cxw.WriteStartElement("ScoresToSend");

                cxw.WriteAttributeString("Leaderboard", qs.Leaderboard);
                cxw.WriteAttributeULong("Id", qs.Score.Principal_ID);
                cxw.WriteAttributeULong("Nex_Id", qs.Score.Nex_Unique_ID);
                cxw.WriteAttributeULong("Misc", qs.Score.Misc);
                cxw.WriteAttributeUInt("Score", qs.Score.Score);
                cxw.WriteAttributeArray<byte>("Cud", qs.Score.CommonDataBinary);
                cxw.WriteAttributeString("UserName", qs.Score.CommonDataUserName);
                cxw.WriteAttributeInt("Type", (int)qs.Type);
                cxw.WriteEndElement();
            }

            foreach(KeyValuePair<string, List<HighScore>> kvp in _localLeaderboards)
            {
                cxw.WriteStartElement("Leaderboard");
                cxw.WriteAttributeString("Name", kvp.Key);

                foreach(HighScore hs in kvp.Value)
                {
                    cxw.WriteStartElement("Score");
                    cxw.WriteAttributeULong("Id", hs.Principal_ID);
                    cxw.WriteAttributeULong("Nex_Id", hs.Nex_Unique_ID);
                    cxw.WriteAttributeUInt("Score", hs.Score);
                    cxw.WriteAttributeULong("Misc", hs.Misc);
                    cxw.WriteAttributeString("UserName", hs.CommonDataUserName);
                    cxw.WriteAttributeArray<byte>("Cud", hs.CommonDataBinary);
                    cxw.WriteEndElement();
                }
                cxw.WriteEndElement();
            }
            cxw.WriteEndElement();

            cxw.WriteEndDocument();
            cxw.Close();
            fs.Close();
            UFile.Commit();
            PrintLine("Save Compleate");
        }

        public static void Load()
        {
            PrintLine("Atempting Load of leadeboards");
            FileStream fs = null;
            if (!UFile.Exits(_savePath))
            {
                PrintLine("no file found");
                return;
            }
            try
            {
                fs = UFile.Open(_savePath, FileMode.Open);
                PrintLine("Loading Scores");
                CustomXmlReader reader = CustomXmlReader.Create(fs);
                _scoresToSend.Clear();
                while (reader.Read())
                {
                    if (reader.Name == "ScoresToSend")
                        if (reader.IsStartElement())
                        {
                                HighScore hs = new Networking.HighScore()
                                {
                                    Principal_ID = reader.ReadAttributeULong("Id"),
                                    Nex_Unique_ID = reader.ReadAttributeULong("Nex_Id"),
                                    Misc = reader.ReadAttributeULong("Misc"),
                                    Score = reader.ReadAttributeUInt("Score"),
                                    CommonDataBinary = reader.ReadAttributeArrayOfByte("Cud"),
                                    CommonDataUserName = reader.ReadAttributeString("UserName"),
                                };

                                QueuededScore qhs = new QueuededScore(hs, reader.ReadAttributeString("Leaderboard"), 
                                    (HighScoreType)reader.ReadAttributeInt("Type"));
                                _scoresToSend.Add(qhs);
                        }
                    string currentLeaderboard;
                    if (reader.Name == "Leaderboard")
                        if (reader.IsStartElement())
                        {
                            currentLeaderboard = reader.ReadAttributeString("Name");

                            if (!_localLeaderboards.ContainsKey(currentLeaderboard))
                                _localLeaderboards.Add(currentLeaderboard, new List<HighScore>());
                            while (reader.Read())
                            {
                                if (reader.Name == "Leaderboard")
                                    if (!reader.IsStartElement())
                                        break;

                                if (reader.Name == "Score")
                                    if (reader.IsStartElement())
                                    {
                                        HighScore hs = new HighScore()
                                        {
                                            Principal_ID = reader.ReadAttributeULong("Id"),
                                            Nex_Unique_ID = reader.ReadAttributeULong("Nex_Id"),
                                            Misc = reader.ReadAttributeULong("Misc"),
                                            Score = reader.ReadAttributeUInt("Score"),
                                            CommonDataUserName = reader.ReadAttributeString("UserName"),
                                            CommonDataBinary = reader.ReadAttributeArrayOfByte("Cud"),
                                        };


                                        _localLeaderboards[currentLeaderboard].Add(hs);
                                    }
                            }
                        }

                }

                foreach (QueuededScore qs in _scoresToSend)
                    PrintLine($"{qs.Leaderboard} > {qs.Score.Principal_ID}, {qs.Score.Score}");
                reader.Close();
                PrintLine("Loading Compleate");
            }
            catch (System.Exception x)
            {
                PrintLine($"Error Saving: {x.Message}");
//
            }
            fs.Close();

        }
    }
}
