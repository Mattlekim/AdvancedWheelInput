using System;
using System.Collections.Generic;
using System.Text;

using MySqlDI.Database;

namespace ServerKernel
{
    public static class CustomHttpEncoders
    {


        private static int _matchOffset = 0;

        public static int AttackingId { get; set; }
        public static int OpponateId { get; set; } = -1;
        public static int OpponatesRank { get; set; }
        public static int OpponatesLevel { get; set; }
        public static int OpponatesAvalibleCredits { get; set; }
        public static int OpponatesAvalibleSpairParts { get; set; }
        public static float OpponatesTimeToBeat { get; set; }
        public static int LevelToRace { get; set; }



        public static RequestData FindMatch(string args)
        {
            _matchOffset = 0;
            return new RequestData($"?offset={_matchOffset}", null);
        }

        public static RequestData NextMatch(string args)
        {
            _matchOffset++;
            return new RequestData($"?offset={_matchOffset}", null);
        }

        public static RequestData Attack(string args)
        {
            if (OpponateId == -1)
                return null;//validation check
            
            Dictionary<string, string> post = new Dictionary<string, string>()
            {
                {"id", OpponateId.ToString() },
                {"lvl", OpponatesLevel.ToString() } ,
                {"rank", OpponatesRank.ToString() },
                {"ac", OpponatesAvalibleCredits.ToString() },
                {"asp", OpponatesAvalibleSpairParts.ToString() }

            };

            return new RequestData("", post);
        }

        public static RequestData AResult(string args)
        {
            
            if (OpponateId == -1)
                return null; //validation check

            Dictionary<string, string> post = new Dictionary<string, string>()
            {
                {"id", OpponateId.ToString() },
                {"r", args },
                {"lvl", OpponatesLevel.ToString() },
                {"rank", OpponatesRank.ToString() },
                {"ac", OpponatesAvalibleCredits.ToString() },
                {"asp", OpponatesAvalibleSpairParts.ToString() },
                {"aid", AttackingId.ToString() }
            };

            OpponateId = -1; //reset id so we cant resue command
            return new RequestData("", post);
        }

        public static RequestData ForceServerUpdate(string args)
        {
            return new RequestData("?id=x7dj30fkl48fj38djgflkgid83j4d8jd3", null);
        }

        public static RequestData Resources(string args)
        {
            if (args == null || !args.Contains("c=") && !args.Contains("s="))
                return null;

            int cs = args.IndexOf("c=");
            int ss = args.IndexOf("s=");
            int space = args.IndexOf(" ");

            string credits = "0";
            string spairParts = "0";
           
            if (space < ss)
            {
                if (cs != -1)
                    credits = args.Substring(cs + 2, space - 2);
                spairParts = args.Substring(ss + 2);
            }
            else
            {
                if (ss != -1)
                    spairParts = args.Substring(ss + 2, space - 2);
                credits = args.Substring(cs + 2);
            }

            Dictionary<string, string> post = new Dictionary<string, string>()
            {
                {"credits", credits },
                {"spairparts", spairParts },
            };

            return new RequestData("", post);
        }

        public static RequestData CreateUser(string args)
        {
            if (args == null)
            {
                return null;
            }

            int userIndex = args.IndexOf("user=");
            if (userIndex < 0)
            {
                return null;
            }

            string password = args.Substring(0, userIndex);
            string username = args.Substring(userIndex + 5);

            Dictionary<string, string> loginCreds = new Dictionary<string, string>()
                {
                    {"username", username },
                {"password", password}
                };
            return new RequestData(null, loginCreds);
        }
    }
}
