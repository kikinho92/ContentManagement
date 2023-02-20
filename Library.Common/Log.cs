using System;
using System.IO;

namespace Library.Common
{
    public static class Log
    {
        private static readonly string ROOT_PATH = $"{Directory.GetCurrentDirectory()}\\Logs";

        public enum LogLevel { Info = 1, Warning = 2, Error = 3 }

        public static void Write(string service, LogLevel logLevel, string message)
        {
            string path = $"{ROOT_PATH}\\{service}_{DateTime.Now.ToString("yyyyMMdd")}.log";

            using (StreamWriter w = File.AppendText(path))
            {
                string level = string.Empty;
                switch (logLevel)
                {
                    case LogLevel.Info:
                        level = "INFO";
                        break;
                    case LogLevel.Warning:
                        level = "WARNING";
                        break;
                    case LogLevel.Error:
                        level = "ERROR";
                        break;
                    default:
                        level = "INFO";
                        break;
                }
                w.WriteLineAsync($"{DateTime.Now.ToString()}  {level}  {service.ToUpper()} : {message}");
            }
        }
    }
}
