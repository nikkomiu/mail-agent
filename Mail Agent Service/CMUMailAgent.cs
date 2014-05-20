using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace Mail_Agent_Service
{
    public partial class CMUMailAgent : ServiceBase
    {
        private Thread mainThread;
        private Logging Log;
        private Settings settings;

        public CMUMailAgent()
        {
            
        }

        protected override void OnStart(string[] args)
        {
            // Create a new thread for the thread loop
            mainThread = new Thread(ThreadLoop);

            // Start the thread loop
            mainThread.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();

            // Write the end statement to the log
            Log.End();

            // Destroy the thread
            mainThread.Abort();
        }

        private void ThreadLoop()
        {
#if DEBUG
            // Make the thread sleep long enough to attach a debugger to the process
            Thread.Sleep(10000);
#endif
            // Parse the settings file
            this.settings = new Settings();
            settings.Parse();

            bool localLocation;
            bool.TryParse(settings.General["LogLocalLocation"], out localLocation);
            Logging.Level logLevel;
            Enum.TryParse(settings.General["LogLevel"], true, out logLevel);

            // Setup the logging file
            Log = new Logging(settings.General["LogLocation"], logLevel, localLocation);

            // Log the start of the thread with some settings information
            Log.Begin(settings.General);

            // Convert MailPolling variable into an int
            int threadSleep;
            int.TryParse(settings.General["MailPolling"], out threadSleep);

            // Loop "forever"
            while (true)
            {
                try
                {
                    ExchangeServer exchange = new ExchangeServer(settings.General);

                    exchange.SaveMail(settings.Profiles, Log);

                    Thread.Sleep(threadSleep);
                }

                // Catch exception for thread abort (OnStop)
                catch (ThreadAbortException)
                {
                    Log.WriteLine(Logging.Level.WARNING, "Thread exiting!");

                catch (Microsoft.Exchange.WebServices.Data.ServiceXmlDeserializationException ex)
                {
                    // Output the more generic error to the logs
                    Log.WriteError(ex);

                    // Output the slightly more specific error message to logs
                    Log.WriteLine(Logging.Level.CRITICAL, "Exchange Web Service Encountered an error!");
                    Log.WriteLine(Logging.Level.CRITICAL, "  This could be caused by a login failure.");
                    Log.WriteLine(Logging.Level.CRITICAL, "  Check your connection information in the settings and restart the service.");

                    // Quit the service because this error cannot be recovered from
                    return;
                }

                // Catch other general exceptions
                catch (Exception ex)
                {
                    // Write the exception to the log
                    Log.WriteError(ex);
                }

                // Garbage collection
                Log.WriteLine(Logging.Level.DEBUG, "Memory Allocated (Before): " + GC.GetTotalMemory(false));
                Log.WriteLine(Logging.Level.DEBUG, "Collecting Garbage...");
                GC.Collect();
                Log.WriteLine(Logging.Level.DEBUG, "Memory Allocated (After) : " + GC.GetTotalMemory(true));
            }
        }
    }
}
