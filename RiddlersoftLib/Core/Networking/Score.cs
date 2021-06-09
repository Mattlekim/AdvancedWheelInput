using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core.Networking
{
    /// <summary>
    /// the type of highscore
    /// </summary>
    public enum HighScoreType { Score, CommonData }

    public struct HighScore
    {
        public ulong Principal_ID;
        public ulong Misc;
        public ulong Nex_Unique_ID;

        /// <summary>
        /// the rank in the leaderboard
        /// </summary>
        public uint Rank;

        /// <summary>
        /// the score of the player
        /// </summary>
        public uint Score;

        public byte[] CommonDataBinary;

        public string CommonDataUserName;
    }
}
