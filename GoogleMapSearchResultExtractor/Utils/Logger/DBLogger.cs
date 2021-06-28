using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapSearchResultExtractor.Utils.Logger
{
    public class DBLogger : LoggerBase
    {
        string connectionString = string.Empty;
        public override void Log(string message)
        {
            lock (lockObj)
            {
                //Code to log data to the database
            }
        }
    }
}
