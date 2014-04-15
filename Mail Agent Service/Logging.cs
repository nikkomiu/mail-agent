using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail_Agent_Service
{
    public class Logging
    {
        // Log Levels
        public enum Level { DEBUG, INFO, WARNING, ERROR, CRITICAL }

        private FileMan fManager;
        private Level logLevel;

        public Logging() : this("debug.log", Level.DEBUG)
        {
        }

        public Logging(string logName, Level logLevel, bool isLocalFile = true)
        {
            // Set the log level to output
            this.logLevel = logLevel;

            // If it is a local file use a local path otherwise use an absolute path
            if (isLocalFile)
                fManager = FileMan.LocalFile(logName);
            else
                fManager = new FileMan(logName);
        }

        public void WriteLine(Level logLevel, string logText)
        {
            logText += "\r\n";
            this.Write(logLevel, logText);
        }

        public void Write(Level logLevel, string logText)
        {
            // TODO: Add some other awesome info for logging

            fManager.Read();
            fManager.Append(LineText(logLevel) + logText);
            fManager.Save();
        }

        public void Begin(Dictionary<string, string> settings)
        {
            this.WriteLine(Level.INFO, "-----------------------------------------------------------");
            this.WriteLine(Level.INFO, "Process has started @ " + DateTime.Now.ToString());

            // Output Settings Information
            this.WriteLine(Logging.Level.INFO, "Log Location: " + settings["LogLocation"]);
            this.WriteLine(Logging.Level.INFO, "Log Level:    " + settings["LogLevel"]);
            this.WriteLine(Logging.Level.INFO, "Mail Polling: " + settings["MailPolling"]);
            
            this.WriteLine(Level.INFO, "-----------------------------------------------------------");
        }

        public void End()
        {
            this.WriteLine(Level.INFO, "-----------------------------------------------------------");
            this.WriteLine(Level.INFO, "Process has ended @ " + DateTime.Now.ToString());
            this.WriteLine(Level.INFO, "-----------------------------------------------------------");
        }

        public void WriteError(Exception ex)
        {
            this.WriteLine(Logging.Level.CRITICAL, "====================================================================");
            this.WriteLine(Logging.Level.CRITICAL, "There was an error that the program could not handle on its own:");
            this.WriteLine(Logging.Level.CRITICAL, ex.Message);
            this.WriteLine(Logging.Level.CRITICAL, "Was Caused By:");
            this.WriteLine(Logging.Level.CRITICAL, ex.Source);
            this.WriteLine(Logging.Level.CRITICAL, "====================================================================");
        }

        private static string LineText(Level logLevel)
        {
            return "(" + DateTime.Now.ToString() + ") " + "[" + logLevel.ToString() + "]: ";
        }
    }
}
