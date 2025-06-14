using System;
using System.Text;

namespace gMKVToolNix.Log
{
    public delegate void LogLineAddedEventHandler(string lineAdded, DateTime actionDate);

    public static class gMKVLogger
    {
        private static readonly StringBuilder _Log = new StringBuilder();

        public static string LogText { get { return _Log.ToString(); } }

        public static event LogLineAddedEventHandler LogLineAdded;

        public static void Clear()
        {
            _Log.Length = 0;
        }

        public static void Log(string message)
        {
            DateTime actionDate = DateTime.Now;
            string logMessage = $"{actionDate:[yyyy-MM-dd][HH:mm:ss]} {message}";
            _Log.AppendLine(logMessage);
            OnLogLineAdded(logMessage, actionDate);            
        }

        public static void OnLogLineAdded(string lineAdded, DateTime actionDate)
        {
            LogLineAdded?.Invoke(lineAdded, actionDate);
        }
    }
}
