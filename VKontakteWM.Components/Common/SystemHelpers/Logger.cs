using System;

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Galssoft.VKontakteWM.Components.SystemHelpers
{
    /// <summary>
    /// Класс для логирования
    /// </summary>
    public class Logger
    {
        private readonly string _logFileName;
        private readonly LogEntryType _maxErrorLevel;

        /// <summary>
        /// Максимальный уровень ошибки, который будет выводиться в лог
        /// </summary>
        public LogEntryType MaxErrorLevel { get { return _maxErrorLevel;} }

        public Logger(string logFileName, LogEntryType maxErrorLevel)
        {
            _logFileName = logFileName;
            _maxErrorLevel = maxErrorLevel;
            
            var sw = new StreamWriter(_logFileName, false);
            sw.WriteLine("Application log started at " + DateTime.Now.ToString("dd/MM/YYYY HH:mm:ss\r\n"));
            sw.Flush();
            sw.Close();
        }

        public void Write(LogEntryType entryType, string message)
        {
            if (entryType > _maxErrorLevel) return;

            //FIXME! если в один лог будут писать и приложение, и служба нотификаций,
            // возможна ситуация, когда файл занят и вылетит исключение.
            // (это не проблема, если лог будет отключён в продакшене)
            var sw = new StreamWriter(_logFileName, true);
            sw.Write(DateTime.Now.ToString("HH:mm:ss"));
            sw.WriteLine(" - " + message);
            sw.Flush();
            sw.Close();
        }
    }

    /// <summary>
    /// Тип сообщения в лог по важности: от ошибок до отладочных сообщений
    /// </summary>
    public enum LogEntryType {NoLogging = 0, Error = 1, Warning = 2, DebugInfo = 3}
}
