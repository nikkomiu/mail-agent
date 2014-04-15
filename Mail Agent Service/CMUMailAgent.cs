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

            // Setup the logging file
            Debug = new Logging(settings.General["LogLocation"], Logging.Level.DEBUG, false);

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

                    exchange.GetMail(settings.Profiles, Debug);

                    Debug.WriteLine(Logging.Level.INFO, settings.Profiles.Count.ToString());
                    foreach (Profile p in settings.Profiles)
                    {
                        Debug.WriteLine(Logging.Level.INFO, p.Id);
                        Debug.WriteLine(Logging.Level.INFO, p.Name);
                        Debug.WriteLine(Logging.Level.INFO, p.EmailSubject);
                        Debug.WriteLine(Logging.Level.INFO, p.EmailBody);
                    }

                    Debug.WriteLine(Logging.Level.INFO, "Thread is running...");

                    Thread.Sleep(threadSleep);
                }
                // Exception for thread abort (OnStop)
                catch (ThreadAbortException ex)
                {
                    Debug.WriteLine(Logging.Level.INFO, "Thread exiting!");
                }
                catch (Exception ex)
                {
                    // Write the exception to the log
                    Debug.WriteError(ex);
                }
            }
        }
    }
}
