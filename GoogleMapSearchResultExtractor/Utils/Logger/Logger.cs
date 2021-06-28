using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapSearchResultExtractor.Utils.Logger
{
    public static class Logger
    {
        private static LoggerBase logger = null;

        public enum LoggerType
        {
            File, Database, EventLog
        }
        public static void Log(string message, LoggerType type)
        {
            switch (type)
            {
                case LoggerType.File:
                    logger = new FileLogger();
                    logger.Log(message);
                    break;
                case LoggerType.Database:
                    logger = new DBLogger();
                    logger.Log(message);
                    break;
                case LoggerType.EventLog:
                    logger = new EventLogger();
                    logger.Log(message);
                    break;
                default:
                    return;
            }
        }

        public static void Info(string message)
        {
            logger = new FileLogger();

            logger.Log($"<div class='info'>[info] {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {message}</div>");
        }

        public static void Error(string message)
        {
            logger = new FileLogger();

            logger.Log($"<div class='error'>[error] {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {message}</div>");
        }

        public static void Warning(string message)
        {
            logger = new FileLogger();

            logger.Log($"<div class='warning'>[warning] {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {message}</div>");
        }


    }
}
