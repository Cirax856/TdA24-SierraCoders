using System.Text;

namespace aspnetapp
{
    public static class Log
    {
        private static List<LogMessage> messages = new List<LogMessage>();
        private static StringBuilder builder = new StringBuilder();

        private static void log(string message, LogLevel level)
        {
            LogMessage log = new LogMessage(level, message);
            messages.Add(log);
            builder.Append($"[{log.Level}]".PadRight(12, ' '))
                .Append($"[{log.Time.ToString("G")}] ") // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#GeneralDateLongTime
                .AppendLine(message);
        }
        public static void Debug(string message)
            => log(message, LogLevel.Debug);
        public static void Info(string message)
            => log(message, LogLevel.Info);
        public static void Error(string message)
            => log(message, LogLevel.Error);
        public static void Exception(Exception ex)
            => log(ex.ToString(), LogLevel.Error);

        internal static void Clear()
        {
            messages.Clear();
            builder.Clear();
        }

        public static new string ToString()
            => builder.ToString();
    }

    public struct LogMessage
    {
        public LogLevel Level;
        public DateTime Time;
        public string Message;

        public LogMessage(LogLevel _level, string _message)
        {
            Level = _level;
            Time = DateTime.UtcNow;
            Message = _message;
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Error,
        Exception
    }
}
