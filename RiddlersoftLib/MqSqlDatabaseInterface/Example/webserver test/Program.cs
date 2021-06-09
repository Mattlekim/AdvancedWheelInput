using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MySqlDI;

namespace webserver_test
{
    class Program
    {
        static LeaderboardServer server;
        static void Main(string[] args)
        {
            server = new LeaderboardServer("http://riddlersoftgames.co.uk/database/osrh2h/login.php", "Leaderboard Server");
            server.GetLeaderboardUrl = "http://riddlersoftgames.co.uk/database/osrh2h/getleaderboard.php";
            server.SendScoreUrl = "http://riddlersoftgames.co.uk/database/osrh2h/submitscore.php";
            server.WriteLogToOutputWindow = true;
            server.OnHTTPSuccesses = OnGetLeaderBoard;
            server.OnHTTPFailure = OnServerError;
            var r = server.Login("any name").Result;
            server.SendScore("lb0", 938261, LeaderboardServer.SortOrder.Desending);

            Console.Write("Sending Score: ");
            while (server.WatingForRequest) //just wait until server is avalible
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
            server.GetLeaderBoard("lb0", LeaderboardServer.SortOrder.Desending);
            Console.ReadKey();
        }

        static void OnGetLeaderBoard(object o)
        {
            if (o == null)
                return;
            List<HighScore> scores = (List<HighScore>)o;
            Console.WriteLine("=========Leaderboards");
            for (int i=0; i < scores.Count; i++)
            {
                Console.Write((i + 1).ToString());
                Console.CursorLeft = 4;
                Console.Write(scores[i].Name);
                Console.CursorLeft = 24;
                Console.WriteLine(scores[i].Score);
            }
        }

        static void OnServerError(string log)
        {
            Console.Clear();
            Console.Write(log);
        }
    }
}
