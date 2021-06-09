using System;
using System.Collections.Generic;
using System.Text;

namespace MySqlDI.Database
{
    public struct DatabaseRequest
    {
        public RequestType Type { get; set; }
        public DbAction ActionType;
        public string URL;
        public string Name, Description;

        public readonly Func<string, RequestData> Encoder;
        public readonly Action<object> Decoder;
        public string GetData;

        
        public DatabaseRequest(DbAction action, string url, Func<string, RequestData> encoder, Action<object> decoder)
        {
            GetData = null;
            Type = MySqlDI.RequestType.Normal;
            ActionType = action;
            URL = url;
            Name = "";
            Description = "";
            Decoder = decoder;
            Encoder = encoder;
        }

        public static DatabaseRequest Null = new DatabaseRequest(DbAction.None, "", HttpEncoders.None , (object o) => { });
    }
}
