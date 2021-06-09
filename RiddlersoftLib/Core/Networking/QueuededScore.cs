using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core.Networking
{
    public struct QueuededScore
    {
        public HighScore Score { get; private set; }

        public string Leaderboard { get; private set; }
        public HighScoreType Type;

        public QueuededScore(HighScore score, string leaderboard, HighScoreType type)
        {
            Score = score;
            Leaderboard = leaderboard;
            Type = type;
        }

    }
}
