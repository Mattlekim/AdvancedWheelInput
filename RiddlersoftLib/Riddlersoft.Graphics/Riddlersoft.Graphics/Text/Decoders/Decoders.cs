using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Graphics.Text.Decoders
{
    public interface TextDecoder
    {
        List<CharData> Decode(string input, out string trimed);
    }
}
