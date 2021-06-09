using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core.Disk
{
    /// <summary>
    /// the current disk state
    /// </summary>
    public enum DiskState { None = 0, Read = 1, Write = 2, Bussy = 3}
}
