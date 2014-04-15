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

#region TestingCode
                    Log.WriteLine(Logging.Level.DEBUG, "Number of Profiles: " + settings.Profiles.Count.ToString());
                    foreach (Profile p in settings.Profiles)
                    {
                        Log.WriteLine(Logging.Level.DEBUG, "::::::::::::::::::::::::::::::::::::");
                        Log.WriteLine(Logging.Level.DEBUG, p.Id);
                        Log.WriteLine(Logging.Level.DEBUG, p.Name);
                        Log.WriteLine(Logging.Level.DEBUG, p.EmailSubject);
                        Log.WriteLine(Logging.Level.DEBUG, p.EmailBody);
                        Log.WriteLine(Logging.Level.DEBUG, "::::::::::::::::::::::::::::::::::::");
                    }

                    Log.WriteLine(Logging.Level.DEBUG, "Thread is running...");
#endregion

                    Thread.Sleep(threadSleep);
                }
                // Exception for thread abort (OnStop)
                catch (ThreadAbortException ex)
                {
                    Log.WriteLine(Logging.Level.WARNING, "Thread exiting!");
                }
                catch (Exception ex)
                {
                    // Write the exception to the log
                    Log.WriteError(ex);
                }

                // Garbage collection
                Log.WriteLine(Logging.Level.DEBUG, "Memory Free: " + GC.GetTotalMemory(false));
                Log.WriteLine(Logging.Level.DEBUG, "Collecting Garbage...");
                GC.Collect();
                Log.WriteLine(Logging.Level.DEBUG, "Memory Free: " + GC.GetTotalMemory(true));
            }
        }
    }
}
