using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core.Networking
{
    /// <summary>
    /// Select which kind of ranking to get by specifying a RankingMode for the parameter of the GetRanking
    /// </summary>
    public enum RankingType
    {
        /// <summary>
        /// Gets rankings in the range of the specified starting position and length.
        /// </summary>
        RANKING_MODE_RANGE,
        /// <summary>
        /// Gets rankings near the specified user.
        /// </summary>
        RANKING_MODE_NEAR,
        /// <summary>
        /// Gets the rankings of friends of the local host. If a player does not have any friends, this gets only that player's data.
        /// </summary>
        RANKING_MODE_FRIEND_RANGE,
        /// <summary>
        /// Gets rankings of friends that are near to the rankings of the local host.
        /// </summary>
        RANKING_MODE_FRIEND_NEAR,
        /// <summary>
        /// Gets the ranking data for the specified user.
        /// </summary>
        RANKING_MODE_USER
    }
}
