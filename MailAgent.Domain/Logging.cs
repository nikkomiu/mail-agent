using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAgent.Domain
{
    public class Logging
    {
        // Log Levels
        public enum Level { DEBUG = 5, INFO = 4, WARNING = 3, ERROR = 2, CRITICAL = 1, NONE = 0 }

        private FileMan _fileManager;
        private Level _logLevel;

        public Logging() : this("debug.log", Level.DEBUG)
        {
        }

        public Logging(string logName, Level logLevel, bool isLocalFile = true)
        {
            // Set the log level to output
            this._logLevel = logLevel;

            // Add datestamp to filename
            string[] splitLogName = logName.Split('.');
            splitLogName[splitLogName.Length - 2] += "_" + CurrentDate(DateTime.Now) + ".";
            logName = string.Empty;

            foreach (string stringPart in splitLogName)
                logName += stringPart;

            // If it is a local file use a local path otherwise use an absolute path
            if (isLocalFile)
                _fileManager = FileMan.LocalFile("logs\\" + logName);
            else
                _fileManager = new FileMan(logName);
        }

        public void Write(Level logLevel, string logText)
        {
            if (this.IsLogLevelHighEnough(logLevel))
            {
                _fileManager.Append(LineText(logLevel) + logText);
            }
        }

        public void WriteLine(Level logLevel, string logText)
        {
            logText += "\r\n";
            this.Write(logLevel, logText);
        }

        public void WriteError(Exception ex)
        {
            this.WriteLine(Logging.Level.CRITICAL, "====================================================================");
            this.WriteLine(Logging.Level.CRITICAL, "There was an error that the program could not handle on its own:");
            this.WriteLine(Logging.Level.CRITICAL, ex.Message);
            this.WriteLine(Logging.Level.CRITICAL, "with an error type: " + ex.GetType().ToString());
            this.WriteLine(Logging.Level.CRITICAL, "====================================================================");
        }

        private bool IsLogLevelHighEnough(Level outputLevel)
        {
            return (_logLevel >= outputLevel) ? true : false;
        }

        public void Begin(Dictionary<string, string> settings)
        {
            this.WriteLine(Level.INFO, "-----------------------------------------------------------");
            this.WriteLine(Level.INFO, "Process has started @ " + DateTime.Now.ToString());

            // Output Settings Information
            this.WriteLine(Logging.Level.DEBUG, "Log Location:          " + settings["LogLocation"]);
            this.WriteLine(Logging.Level.DEBUG, "Log Level:             " + settings["LogLevel"]);
            this.WriteLine(Logging.Level.DEBUG, "Mail Polling Interval: " + settings["MailPolling"]);

            this.WriteLine(Level.INFO, "-----------------------------------------------------------");
        }

        public void End()
        {
            this.WriteLine(Level.INFO, "-----------------------------------------------------------");
            this.WriteLine(Level.INFO, "Process has ended @ " + DateTime.Now.ToString());
            this.WriteLine(Level.INFO, "-----------------------------------------------------------");
        }

        private static string LineText(Level logLevel)
        {
            return "(" + CurrentTimestamp() + ") " + "[" + PaddedLogLevel(logLevel) + "]: ";
        }

        private static string PaddedLogLevel(Level logLevel)
        {
            string levelString = logLevel.ToString();
            int halfLength = (4 - (int)(levelString.Length/2)) + levelString.Length;
            levelString = levelString.PadRight(halfLength, ' ');
            levelString = levelString.PadLeft(8, ' ');

            return levelString;
        }

        private static string CurrentDate(DateTime time)
        {
            return time.Year.ToString("D4")  + "-" +
                   time.Month.ToString("D2") + "-" +
                   time.Day.ToString("D2");
        }

        private static string CurrentTimestamp()
        {
            DateTime time = DateTime.Now;

            return CurrentDate(time) + " " +
                time.Hour.ToString("D2") + ":" +
                time.Minute.ToString("D2") + ":" +
                time.Second.ToString("D2") + ":" +
                time.Millisecond.ToString("D3");
        }
    }
}
