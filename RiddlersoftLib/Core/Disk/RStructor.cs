using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core.Disk
{
    public abstract class RStructor
    {
        protected static string Valadate(string path)
        {
            if (path.Contains(".\\"))
                path = path.Replace(".\\", "");
            return path;
        }
    }
}
