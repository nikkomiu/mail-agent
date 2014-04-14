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

            // Parse the settings file
            this.settings = new Settings();
            settings.Parse();

            // Setup the logging file
            Debug = new Logging();

            // Log the start of the thread with some settings information
            Debug.Begin(settings.General);

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

            // Convert MailPolling variable into an int
            int threadSleep;
            int.TryParse(settings.General["MailPolling"], out threadSleep);

            ExchangeServer exchange = new ExchangeServer(settings.General);
            
            // Loop "forever"
            while (true)
            {
                try
                {
                    List<Microsoft.Exchange.WebServices.Data.Item> mailItems = exchange.GetMail();

                    foreach (Microsoft.Exchange.WebServices.Data.Item item in mailItems)
                    {
                        Debug.WriteLine(Logging.Level.INFO, item.Subject);
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
