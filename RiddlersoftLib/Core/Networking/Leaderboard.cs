using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

/// <summary>
/// I HAVE THE FOLLOWING QUESTOINS
/// iv serched to docs but i cant find the limints on server requests so what are they?
/// if i was to send several scores in a batch does that count a 1 connection or one for each score.
/// Do i need to logout after each request or do i stay logged in for the duration of the playtime.
/// MATT'S ANSWERS: the limit is on APIs. Anytime you can batch, do it. You should not logout after every request (but that's my business)
/// Also, I'm doing the throttling myself (they test the timing down to the second, you don't want to mess it up)
/// </summary>


namespace Riddlersoft.Core.Networking
{

    public static partial class Leaderboards
    {

        //========================timeings====================================
        //putscore, getcommondata, putcommondata, deletecommondata 10 times per minute each
        //getranking, get categorysettings, get rankingchart, getestimatescoreank 20 times per minute each


        /// <summary>
        /// the amount of time to wait in seconds to send all score in a batch
        /// </summary>
        const float SendScoresInBatchIntervel = 30; //try every 30 seconds

        public static Action<NetworkError, string> OnNetworkError;

        private static float _batchTimer = 0;

        public static bool CanCommunicateWithRankingsServer { get; private set; } = false;

        public static void DisableSendScores()
        {
            SendingFailed = true;
        }

        public static void CheckIfCanConnectToRankingServer()
        {
#if SWITCH
            if (Core.Switch.NSwitch.IsNSAAccount())
                CanCommunicateWithRankingsServer = true;
            else
                CanCommunicateWithRankingsServer = false;
#endif
        }

        /// <summary>
        /// the server id for the leaderboards
        /// </summary>
        public static UInt32 Game_Server_Id { get; internal set; }

        /// <summary>
        /// the access key so there server can login to the leaderboards
        /// </summary>
        public static string NGS_ACCESS_KEY { get; internal set; }

        private static bool _isInitalized = false;

        /// <summary>
        /// if true the leaderboard will automaticly try to send scores that have not been sent
        /// </summary>
        public static bool AutomaticlyRetrySendScores { get; set; } = true;

        public static bool SendingFailed { get; private set; } = false;

        private static List<QueuededScore> _scoresToSend = new List<QueuededScore>();
        /// <summary>
        /// Initalize the server
        /// </summary>
        /// <param name="gameServerId">the login in server id</param>
        /// <param name="ngsAccessKey">the login access key</param>
        public static void Initalize(UInt32 gameServerId,string ngsAccessKey)
        {
            if (_isInitalized)
                throw new Exception("Leaderboards can only be initialized once.");
            _isInitalized = true;

            Game_Server_Id = gameServerId;
            NGS_ACCESS_KEY = ngsAccessKey;

            if (ngsAccessKey.Length != 8)
                throw new Exception("ngsAccessKey should be 8 characters but isn't. Check for mixups.");

#if SWITCH
            _native_init(gameServerId, ngsAccessKey);
#endif
            CheckIfCanConnectToRankingServer();
        }

      
        private static void TriggerErrorEvent(NetworkError error, string msg)
        {
            if (OnNetworkError != null)
                OnNetworkError(NetworkError.Login_Failed, "Error Could not login");
        }


        public static void Update(float dt, bool upload = true)
        {
            if (!_isInitalized)
                throw new Exception("Must initalizes leaderboards first!");
            if (CanCommunicateWithRankingsServer && upload)
            {
                _batchTimer += dt;
                if (_batchTimer > SendScoresInBatchIntervel)
                {
                    _batchTimer = 0;
                    UploadBatchedScores();
                }
            }

            UpdateStorage();
        }

        /// <summary>
        /// all scores to send in a batch
        /// </summary>
        private static void UploadBatchedScores()
        {

            if (!AutomaticlyRetrySendScores)
                return;

            if (_scoresToSend.Count <= 0)
                return;

            if (IsBussy)
            {
                _batchTimer = 0;
                return;
            }

            IsBussy = true;
            Thread t = new Thread(() =>
            {
                int retrycounter = 10;
               
               
                _batchTimer = -1000;//set very high so as not to send another score while uploading this one
                if (_uploadScoreToServer(_scoresToSend[0].Score, _scoresToSend[0].Leaderboard))
                {
                    _scoresToSend.RemoveAt(0); //delete score if successfuly uploaded
                    SendingFailed = false; //success
                }
                else
                    SendingFailed = true;
                _batchTimer = 0;
                IsBussy = false;
            });
            t.Start();
        }

        /// <summary>
        /// gets highscores for a given leaderboard
        /// </summary>
        /// <param name="leaderboard">the leaderbaord to get the scores for</param>
        /// <param name="sortby">the sort order for the leaderboard</param>
        /// <param name="fillter">the range of scores to get</param>
        /// <param name="offset">score offset, the rank to start getting scors from</param>
        /// <param name="scoresToGet">total number of scores to get</param>
        /// <returns>returns the socres for the leaderboard. returns null when failes</returns>
        public static List<HighScore> GetScores(string leaderboard, SortOrder sortby, LeaderboardFilter fillter, uint offset = 0, int scoresToGet = 100)
        {
            NEX_Category? category = CategoryHelper.GetCategory(leaderboard);
            if (category == null)   
                throw new Exception("Category does not exits!");

            List<HighScore> results = new List<HighScore>();
            IsBussy = true;
            SendingFailed = false; //this has to be called by the user therefore we want to reset it

#if SWITCH
            _native_getScores((uint)category.Value.Code, fillter, offset, results);
            IsBussy = false;
            if (!asyncComm.result) //if failed
            {
                SendingFailed = true;
                if (results.Count == 0)
                {
                    results = null;

                }
            }
            else
                SendingFailed = false;
            return results;
#else

            IsBussy = false;
            return null;
#endif

        }

        /// <summary>
        /// gets highscores for a given leaderboard
        /// </summary>
        /// <param name="leaderboard">the leaderbaord to get the scores for</param>
        /// <param name="sortby">the sort order for the leaderboard</param>
        /// <param name="fillter">the range of scores to get</param>
        /// <param name="offset">score offset, the rank to start getting scors from</param>
        /// <param name="scoresToGet">total number of scores to get</param>
        /// <returns>returns the socres for the leaderboard. returns null when failes</returns>
        public static List<HighScore> GetCatchedScores(string leaderboard, SortOrder sortby, LeaderboardFilter fillter, int offset = 0, int scoresToGet = 100)
        {
            NEX_Category? category = CategoryHelper.GetCategory(leaderboard);
            if (category == null)
                throw new Exception("Category does not exits!");
            throw new NotImplementedException();
        }

        public static void ClearLocal(string leaderboard)
        {
            NEX_Category? category = CategoryHelper.GetCategory(leaderboard);
            if (category == null)
                throw new Exception("Category does not exits!");

            _localLeaderboards[leaderboard].Clear();
        }

        public static bool DeleteScore(string leaderboard, string principleId, string nexId)
        {
            NEX_Category? category = CategoryHelper.GetCategory(leaderboard);
            if (category == null)
                throw new Exception("Category does not exits!");
            throw new NotImplementedException();
        }


        private static void UploadFailed(HighScore score, string leaderboard, HighScoreType type)
        {
#if DEBUG
            Console.WriteLine($"Uploading Failed. Adding to Scores to send");
#endif
            //make sure that we dont have the score in the list already

            NEX_Category? category = CategoryHelper.GetCategory(leaderboard);
            if (category == null)
                throw new Exception("Category does not exits!");

            int index = -1;
            if (type == HighScoreType.Score)
                index = _scoresToSend.FindIndex(x => x.Leaderboard == leaderboard && x.Score.Principal_ID == score.Principal_ID &&
                    x.Score.Nex_Unique_ID == score.Nex_Unique_ID && x.Score.Score == score.Score);
            else
                index = _scoresToSend.FindIndex(x => x.Leaderboard == leaderboard && x.Score.Principal_ID == score.Principal_ID &&
                    x.Score.Nex_Unique_ID == score.Nex_Unique_ID); // && x.Score.CommonUserData == score.CommonUserData); <-- nonsense

            if (index != -1) //if the score already exits
                return;

            //here we dont have one of theses scores add it
            int scoreIndex = _scoresToSend.FindIndex(x => x.Leaderboard == leaderboard &&
            x.Score.Principal_ID == score.Principal_ID && x.Score.Nex_Unique_ID == score.Nex_Unique_ID);
            if (scoreIndex == -1)
            {
                _scoresToSend.Add(new QueuededScore(score, leaderboard, type));
                _saveSoon = true;
            }
            else
            {
                bool isBetter = false;
                //check to see if score is better or not
                if (category.Value.SortBy == SortOrder.Accending)
                {
                    if (_scoresToSend[scoreIndex].Score.Score > score.Score)
                        isBetter = true;
                }
                else
                    if (_scoresToSend[scoreIndex].Score.Score > score.Score)
                    isBetter = true;

                if (isBetter)
                {
                    QueuededScore qs = new QueuededScore(score, _scoresToSend[scoreIndex].Leaderboard, type);
                    _scoresToSend[scoreIndex] = qs;
                }
            }
        }


        public class Ranking2ScoreUploadRecord
        {
            public uint Category;
            public uint Score;
            public ulong Misc;
        }


        static Ranking2ScoreUploadRecord[] scoreBatch = new Ranking2ScoreUploadRecord[20];
        private static bool _uploadScoreToServer(HighScore hs, string leaderboard, ulong misc = 0)
        {
            NEX_Category? category = CategoryHelper.GetCategory(leaderboard);
            if (category == null)
                throw new Exception("Category does not exits!");

            //this API only supports one score
            scoreBatch[0] = new Ranking2ScoreUploadRecord();
            scoreBatch[0].Category = (uint)category.Value.Code;
            scoreBatch[0].Score = hs.Score;
            scoreBatch[0].Misc = misc;

            #if SWITCH
            //now this should work
            if (!SendingFailed)
            {
                _native_uploadCommonData(hs.CommonDataUserName, hs.CommonDataBinary);

                List<HighScore> results = new List<HighScore>();
                PrintLine("Uploading Score------");
                asyncComm.signal = false;
                _native_uploadScore(scoreBatch, 0, 1);
            }
            else
            {
                PrintLine("Not uploading score as we may not be connected.");
                return false;
            }
        
#endif
            return asyncComm.result; //return the result. true if success false if failor
           
        }
        public static bool IsBussy { get; private set; }

        public static void UploadScores(string userName, List<uint> scores, List<string> leaderboards, ulong misc = 0, byte[] commonData = null, Action<bool> onCompleate = null)
        {
            foreach (string s in leaderboards)
            {
                NEX_Category? category = CategoryHelper.GetCategory(s);
                if (category == null)
                    throw new Exception("Category does not exits!");
            }

            List<HighScore> hscore = new List<HighScore>();
            foreach (uint u in scores)
            {
#if DEBUG
                Console.WriteLine($"Uploading Score {userName}: {u}");
#endif

                hscore.Add(new HighScore()
                {
                    Score = u,
                    Misc = misc,
                    CommonDataUserName = userName,
                    CommonDataBinary = commonData,
                });
            }
            if (!CanCommunicateWithRankingsServer)
            {
                PrintLine("Not Allowed to connect to rankings server");
                for (int i = 0; i < leaderboards.Count; i++)
                {
                    UploadFailed(hscore[i], leaderboards[i], HighScoreType.Score); //on failed to upload score
                    PrintLine("Adding Score Localy");
                    AddScoreLocaly(hscore[0], leaderboards[i]);
                    PrintLine("Done");
                }
                return;
            }

            Thread t = new System.Threading.Thread(() =>
            {
                int checkCout = 10;
                while (IsBussy)
                {
                    System.Threading.Thread.Sleep(1000);
                    checkCout--;
                    if (checkCout <= 0)
                    {
                        PrintLine("Sending failed");
                        for (int i = 0; i < leaderboards.Count; i++)
                            UploadFailed(hscore[i], leaderboards[i], HighScoreType.Score); //on failed to upload score
                        return;
                    }
                }
                IsBussy = true;
                //I guess you meant to upload the common data here too but never did
                //using hscore.CommonDataBinary....
                for (int i = 0; i < leaderboards.Count; i++)
                {
                    bool success = _uploadScoreToServer(hscore[i], leaderboards[i], misc);
                    if (!success) //try to upload the scores to the server
                    {
                        PrintLine("************************");
                        UploadFailed(hscore[i], leaderboards[i], HighScoreType.Score); //on failed to upload score
                        SendingFailed = true; // dont allow to try to connect again
                    }
                    if (onCompleate != null)
                        onCompleate(success);
                }
                IsBussy = false;
            });
            t.Start();

            for (int i = 0; i < leaderboards.Count; i++)
            {
                PrintLine("Adding Score Localy");
                AddScoreLocaly(hscore[i], leaderboards[i]);
                PrintLine("Done");

            }
        }

        public static void UploadScore(string userName, uint score, string leaderboard, ulong misc = 0, byte[] commonData = null, Action<bool> onCompleate = null)
        {
            NEX_Category? category = CategoryHelper.GetCategory(leaderboard);
            if (category == null)
                throw new Exception("Category does not exits!");
#if DEBUG
            Console.WriteLine($"Uploading Score {userName}: {score}");
#endif
            HighScore hscore = new HighScore()
            {
                Score = score,
                Misc = misc,
                CommonDataUserName = userName,
                CommonDataBinary = commonData,
            };

            if (!CanCommunicateWithRankingsServer)
            {
                PrintLine("Not Allowed to connect to rankings server");
                UploadFailed(hscore, leaderboard, HighScoreType.Score); //on failed to upload score
                PrintLine("Adding Score Localy");
                AddScoreLocaly(hscore, leaderboard);
                PrintLine("Done");
                return;
            }

            Thread t = new System.Threading.Thread(() =>
            {
                int checkCout = 10;
                while (IsBussy) //make sure i dont send a score while we are current uploading a score
                {
                    System.Threading.Thread.Sleep(1000);
                    checkCout--;
                    if (checkCout <= 0)
                    {
                        PrintLine("Sending failed");
                        UploadFailed(hscore, leaderboard, HighScoreType.Score); //on failed to upload score
                        return;
                    }
                }
                IsBussy = true;
                //I guess you meant to upload the common data here too but never did
                //using hscore.CommonDataBinary....
                bool success = _uploadScoreToServer(hscore, leaderboard, misc);
                if (!success) //try to upload the scores to the server
                {
                    PrintLine("************************");
                    UploadFailed(hscore, leaderboard, HighScoreType.Score); //on failed to upload score
                    SendingFailed = true; // dont allow to try to connect again
                }
                if (onCompleate != null)
                    onCompleate(success);
                IsBussy = false;
            });
            t.Start();
            
            PrintLine("Adding Score Localy");
            AddScoreLocaly(hscore, leaderboard);
            PrintLine("Done");
          
        }

        public static bool UploadCommonData(byte[] data, string username)
        {

            //if common data failes to upload to server
            UploadFailed(new HighScore() { CommonDataBinary = data, CommonDataUserName = username}, string.Empty,
                 HighScoreType.CommonData);

            throw new NotImplementedException();
        }

        public static  byte[] GetCommonData(string principleId, string nexId)
        {
            throw new NotImplementedException();
        }

        public static void Finilize()
        { }

        private static void PrintLine(string str)
        {
#if DEBUG
            Console.WriteLine(str);
#endif
        }

        //local storage
        /// <summary>
        /// all leaderboards that we have saved localy
        /// </summary>
        private static Dictionary<string, List<HighScore>> _localLeaderboards = new Dictionary<string, List<HighScore>>();

        private static int Find(HighScore score, string leaderboard, bool matchNexId)
        {
            if (!_localLeaderboards.ContainsKey(leaderboard))
                return -1;

            if (matchNexId)
            {
                return _localLeaderboards[leaderboard].FindIndex(x => x.Principal_ID == score.Principal_ID && x.Nex_Unique_ID == score.Nex_Unique_ID);
            }
            else
            {
                return _localLeaderboards[leaderboard].FindIndex(x => x.Principal_ID == score.Principal_ID);
            }
        }

        /// <summary>
        /// the maximum number of local scores that we want to save to file
        /// </summary>
        public static int MaxNumberOfLocalScores = 150;
        /// <summary>
        /// adds a score to the local leaderboards
        /// </summary>
        /// <param name="score">the score to add</param>
        /// <param name="leaderboard">the leaderboard to add it to</param>
        public static void AddScoreLocaly(HighScore score, string leaderboard)
        {
            int index = Find(score, leaderboard, true);

            NEX_Category? category = CategoryHelper.GetCategory(leaderboard);
            if (category == null)
                throw new Exception("Category does not exits!");

            if (index == -1)
            {
                if (!_localLeaderboards.ContainsKey(leaderboard)) //create the leaderboard if it doesnt exits
                    _localLeaderboards.Add(leaderboard, new List<Networking.HighScore>());

                _localLeaderboards[leaderboard].Add(score); //add the highscore;
                PrintLine("Score Added");
            }
            else
            { //now we know the score exits we need to know if we need to replace the old score or not
                PrintLine("Score Found");
                if (category.Value.SortBy == SortOrder.Accending)
                {
                    if (score.Score < _localLeaderboards[leaderboard][index].Score)
                    {
                        _localLeaderboards[leaderboard][index] = score;
                        PrintLine("Score Replaced");
                        return;
                    }
                    PrintLine("Score Not Replaced");
                }
                else
                {
                    if (score.Score > _localLeaderboards[leaderboard][index].Score)
                    {
                        _localLeaderboards[leaderboard][index] = score;
                        PrintLine("Score Replaced");
                        return;
                    }
                    PrintLine("Score Not Replaced");
                }
            }
                
        }


        /// <summary>
        /// trim the leaderboard so that we dont save to may scores
        /// </summary>
        /// <param name="leaderboard">the leaderboard to trim</param>
        private static void Trim(List<HighScore> scores)
        {
            if (scores.Count > MaxNumberOfLocalScores)
            {
                scores.RemoveRange(MaxNumberOfLocalScores - 1, scores.Count - MaxNumberOfLocalScores);
            }
        }
        /// <summary>
        /// sort a list of highscore using a simple bubble sort
        /// </summary>
        /// <param name="input">input list</param>
        /// <param name="sortBy">sort order</param>
        /// <returns>sorted list as output</returns>
        private static List<HighScore> Sort(List<HighScore> input, SortOrder sortBy)
        {
            
            //basic bubble sort
            for (int i = 0; i < input.Count - 1; i++)
                for (int c = 0; c < input.Count - 1; c++)
                {
                    if (input[c].Score > input[c + 1].Score & sortBy == SortOrder.Accending ||
                        input[c].Score < input[c+1].Score & sortBy == SortOrder.Desending)
                    {
                        //now flip
                        HighScore hs = input[c];
                        input[c] = input[c + 1];
                        input[c + 1] = hs;
                    }
                }
            Trim(input);

            for (int i = 0; i < input.Count; i++)
            {
                HighScore hs = input[i];
                hs.Rank = (uint)i + 1;
                input[i] = hs;
            }

            return input;
        }


        public static List<HighScore> GetLocalScores(string scoreBoard, SortOrder sortBy)
        {
            if (!_localLeaderboards.ContainsKey(scoreBoard))
                return new List<Networking.HighScore>(); //return an empty scoreboard

            return Sort(_localLeaderboards[scoreBoard], sortBy);
        }

        struct NativeAsyncComm
        {
            /// <summary>
            /// this is set whenever the c++ asynchronous operations are complete
            /// </summary>
            public volatile bool signal;

            /// <summary>
            /// result of the last operation
            /// </summary>
            public volatile bool result;
        }

        /// <summary>
        /// In order to run the leaderboard APIs asynchronously, put it in a thread yourself.
        /// Set 'signal' to false, start the thread, and inside it call the synchronous function
        /// Poll 'signal' for true in your game to stop animating and display the results
        /// </summary>
        static NativeAsyncComm asyncComm;

        //native implementation parts

        /// <summary>
        /// IMMEDIATE
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void _native_init(UInt32 gameServerId, string ngsAccessKey);

        /// <summary>
        /// even though up to 20 can go in one API call, this will take more and batch them up to the maximum allowed size
        /// LONG-RUNNING SYNCHRONOUS
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void _native_uploadScore(Ranking2ScoreUploadRecord[] batch, int offset, int count);

        /// <summary>
        /// LONG-RUNNING SYNCHRONOUS
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void _native_uploadCommonData(string username, byte[] binaryData);

        /// <summary>
        /// Returns as many scores as possible (up to 100, or maybe 300 for friends mode). Puts them into the list, which should be empty.
        /// LONG-RUNNING SYNCHRONOUS
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void _native_getScores(uint category, LeaderboardFilter filter, uint offset, List<HighScore> result);

        /// <summary>
        /// returns the principal ID last used to connect successfully.
        /// If no operation has succeeded, then this is unset
        /// If the last operation failed, then this may be unset
        /// IMMEDIATE
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern ulong _native_getLastPrincipalId();

    }
}
