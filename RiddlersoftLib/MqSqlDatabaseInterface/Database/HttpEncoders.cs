using System;
using System.Collections.Generic;
using System.Text;

namespace MySqlDI.Database
{
    public static class HttpEncoders
    {
        /// <summary>
        /// a blank encoder
        /// </summary>
        /// <param name="o"></param>
        public static RequestData None(string args) { return new RequestData("", null); }

    }
}
