using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Modules.Chat
{
    public enum ConversationType
    {
        /// <summary>
        /// an active converstation takes over input from the game
        /// </summary>
        Active,
        /// <summary>
        /// a passive conversation does not take over game controle, as such a passive conversation will carry on by its self and cannot have responces.
        /// </summary>
        Passive,
    }
}
