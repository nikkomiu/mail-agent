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
        private Logging Debug;
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
            Debug.End();

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
            Debug = new Logging(settings.General["LogLocation"], logLevel, localLocation);

            // Log the start of the thread with some settings information
            Debug.Begin(settings.General);

            // Convert MailPolling variable into an int
            int threadSleep;
            int.TryParse(settings.General["MailPolling"], out threadSleep);

            // Loop "forever"
            while (true)
            {
                try
                {
                    ExchangeServer exchange = new ExchangeServer(settings.General);

                    exchange.SaveMail(settings.Profiles, Debug);

#region TestingCode
                    Debug.WriteLine(Logging.Level.DEBUG, "Number of Profiles: " + settings.Profiles.Count.ToString());
                    foreach (Profile p in settings.Profiles)
                    {
                        Debug.WriteLine(Logging.Level.DEBUG, "::::::::::::::::::::::::::::::::::::::::::::::");
                        Debug.WriteLine(Logging.Level.DEBUG, p.Id);
                        Debug.WriteLine(Logging.Level.DEBUG, p.Name);
                        Debug.WriteLine(Logging.Level.DEBUG, p.EmailSubject);
                        Debug.WriteLine(Logging.Level.DEBUG, p.EmailBody);
                        Debug.WriteLine(Logging.Level.DEBUG, "::::::::::::::::::::::::::::::::::::::::::::::");
                    }

                    Debug.WriteLine(Logging.Level.DEBUG, "Thread is running...");
#endregion

                    Thread.Sleep(threadSleep);
                }
                // Exception for thread abort (OnStop)
                catch (ThreadAbortException ex)
                {
                    Debug.WriteLine(Logging.Level.WARNING, "Thread exiting!");
                }
                catch (Exception ex)
                {
                    // Write the exception to the log
                    Debug.WriteError(ex);
                }

                // Garbage collection
                Debug.WriteLine(Logging.Level.DEBUG, "Memory Free: " + GC.GetTotalMemory(false));
                Debug.WriteLine(Logging.Level.DEBUG, "Collecting Garbage...");
                GC.Collect();
                Debug.WriteLine(Logging.Level.DEBUG, "Memory Free: " + GC.GetTotalMemory(true));
            }
        }
    }
}
