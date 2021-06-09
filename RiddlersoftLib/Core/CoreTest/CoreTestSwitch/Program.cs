using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Riddlersoft.Core.Networking;
using Riddlersoft.Core.Input;
using Riddlersoft.Core.Switch;

namespace CoreTest
{
    class Program
    {
 

        private static bool run = true;
        static List<string> chars = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
        static void Main(string[] args)
        {

            Riddlersoft.Core.Switch.NSwitch.Init();
         //   Console.Clear();
            Leaderboards.Initalize(0x28BA8400, "ca527321");
            
            if (Riddlersoft.Core.Switch.NSwitch.IsNSAAccount())
            {
                Console.WriteLine("Yey you can play");
            }
            else
                Console.WriteLine("Bhoo go away!");

            //  Leaderboards.Load();

            CategoryHelper.AddCategory("test", 0, SortOrder.Accending);

            Leaderboards.UploadScore(NSwitch.GetNickName(),  15000, "test", 0);
            //return;
            List<HighScore> scores = Leaderboards.GetScores("test", SortOrder.Accending, LeaderboardFilter.RANGE_RANKING, 0, 100);
            Console.WriteLine("===============HIGHSCORE=============");
            Console.WriteLine("Leaderboard One");
            for (int i = 0; i < scores.Count; i++)
                Console.WriteLine($"{i}: {scores[i].CommonDataUserName}: {scores[i].Score}");

            return;
     
        }
    }
}

