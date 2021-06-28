using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoogleMapSearchResultExtractor.Utils.Logger
{
    public class FileLogger : LoggerBase
    {
        private static string _filename;

        public override void Log(string message)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(_filename))
                {
                    ResolveFileName();

                    //<script language='javascript'> setInterval(function(){ window.location.reload(1); }, 5000); </script>
                    Log("<style> div.info { font-family: 'Consolas'; font-size: 13px; color: gray; } div.error { font-family: 'Consolas'; font-size: 13px; color: red; } div.warning { font-family: 'Consolas'; font-size: 13px; color: orange; } </style>");
                }

                using (StreamWriter w = File.AppendText(_filename))
                {
                    w.WriteLine(message);
                }
            }
        }

        private static void ResolveFileName()
        {
            string finalFilename = @"log.html";
            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(finalFilename);
            string extension = Path.GetExtension(finalFilename);
            string path = Path.GetDirectoryName(finalFilename);
            string newFullPath = finalFilename;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0} ({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            _filename = newFullPath;
        }
    }
}
