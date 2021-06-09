using System;
using System.Collections.Generic;
using System.Text;

namespace MySqlDI.Database
{
    /// <summary>
    /// contals Post and Get data for a server request
    /// </summary>
    public class RequestData
    {
        /// <summary>
        /// the html get data
        /// </summary>
        public string Get { get; private set; }
        /// <summary>
        /// the html post data
        /// </summary>
        public Dictionary<string, string> Post { get; private set; }

        /// <summary>
        /// sets the data for the request
        /// </summary>
        /// <param name="get">sets the get data</param>
        /// <param name="post">sets the post data</param>
        public RequestData(string get, Dictionary<string, string> post)
        {
            Get = get;
            Post = post;
        }
    }
}
