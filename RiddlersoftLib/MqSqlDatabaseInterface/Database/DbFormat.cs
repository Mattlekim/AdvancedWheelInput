using System;
using System.Collections.Generic;
using System.Text;

namespace MySqlDI.Database
{

    /// <summary>
    /// a database format
    /// </summary>
    public class DbFormat
    {
        /// <summary>
        /// the database name
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
            

        public Dictionary<string, object> Records
        {
            get;
            private set;
        }

        public List<Type> RecordTypes
        {
            get;
            private set;
        }
        /// <summary>
        /// basic construtor
        /// </summary>
        /// <param name="databaseName"></param>
        public DbFormat(string databaseName)
        {
            Name = databaseName;
        }

        public DbFormat(string databaseName, Dictionary<string, object> records, List<Type> types)
        {
            Name = databaseName;
            Records = records;
            RecordTypes = types;
        }
    }
}
