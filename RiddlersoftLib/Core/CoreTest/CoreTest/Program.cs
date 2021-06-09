using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Riddlersoft.Core.Networking;



namespace CoreTest
{
    class Program
    {

        private static bool run = true;
        static List<string> chars = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j"};
        static void Main(string[] args)
        {
            Console.Clear();
            Leaderboards.Initalize(0x28BA8400, "ca527321");

            Leaderboards.Load();
            List<HighScore> scores = Leaderboards.GetLocalScores("one", SortOrder.Accending);
            Console.WriteLine("===============HIGHSCORE=============");
            Console.WriteLine("Leaderboard One");
            for (int i = 0; i < scores.Count; i++)
                Console.WriteLine($"{i}: {scores[i].Principal_ID}: {scores[i].Score}");

            Console.WriteLine("Leaderboard Two");
            scores = Leaderboards.GetLocalScores("two", SortOrder.Desending);
            for (int i = 0; i < scores.Count; i++)
                Console.WriteLine($"{i}: {scores[i].Principal_ID}: {scores[i].Score}");
            //       Leaderboards.ForceSave();
            //    Leaderboards.UploadScore(100, "sasdf", "asdf", "one", SortOrder.Accending);
            //  Leaderboards.UploadScore(190, "sasdf", "asdf", "one", SortOrder.Desending);
            CategoryHelper.AddCategory("one", 0, SortOrder.Accending);
            CategoryHelper.AddCategory("two", 1, SortOrder.Accending);
            Random rd = new Random();
            for (int i = 0; i < 2; i++)
            {
                string name = string.Empty;
                for (int c = 0; c < 10; c++)
                    name += chars[rd.Next(10)];
                Leaderboards.UploadScore((uint)rd.Next(1000, 100000), "two");
            }
          
            while (run)
            {
                Leaderboards.Update(.016f);
                System.Threading.Thread.Sleep(1000);
                if (Console.KeyAvailable)
                    run = false;
            }
        }
    }
}
