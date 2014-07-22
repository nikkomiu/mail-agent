using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mail_Agent_Service
{
    class ServiceThread
    {
        private Logging _log;
        private Settings _settings;

        private CancellationTokenSource _cancelToken;
        private ManualResetEvent _resetEvent;

        private readonly object _lockObject;

        public ServiceThread()
        {
            _lockObject = new object();
        }

        public void Start()
        {
            // Create the cancel token object
            _cancelToken = new CancellationTokenSource();

            // Create the reset event handler
            _resetEvent = new ManualResetEvent(false);

            // Create and start the thread
            Task.Factory.StartNew(() => ThreadLoop(), _cancelToken.Token);
        }

        public void Stop()
        {
            // Create and start new thread to kill process thread
            new Thread(() =>
            {
                lock (_lockObject)
                {
                    Monitor.Pulse(_lockObject);
                }

                // Cancel the reset delay
                _resetEvent.Set();

                // Cancel the thread
                _cancelToken.Cancel();

                // Write the end statement to the log
                _log.End();
            }).Start();
        }

        private void ThreadLoop()
        {
#if DEBUG
            // Make the thread sleep long enough to attach a debugger to the process
            Thread.Sleep(10000);
#endif
            this.ThreadSetup();

            // Convert MailPolling variable into an int
            int threadSleep;
            int.TryParse(_settings.General["MailPolling"], out threadSleep);

            // Loop "forever"
            while (true)
            {
                // Convert LogLocalLocation setting from string to bool
                bool localLocation;
                bool.TryParse(_settings.General["LogLocalLocation"], out localLocation);

                // Convert LogLevel from string to enum
                Logging.Level logLevel;
                Enum.TryParse(_settings.General["LogLevel"], true, out logLevel);

                // Setup the logging file
                _log = new Logging(_settings.General["LogLocation"], logLevel, localLocation);

                _log.WriteLine(Logging.Level.DEBUG, "Reached Start of Thread Loop");

                try
                {
                    // Create a new ExchangeServer
#if DEBUG
                    ExchangeServer exchange = new ExchangeServer(_settings.General, _log);
#else
                    ExchangeServer exchange = new ExchangeServer(_settings.General);
#endif

                    // Save mail in Exchange
                    exchange.SaveMail(_settings.Profiles, _log);
                }

                // Catch exception for thread abort (OnStop)
                catch (ThreadAbortException)
                {
                    // Should never happen...
                    _log.WriteLine(Logging.Level.ERROR, "Thread aborting!");
                }

                // Catch exception for Exchange XML Error
                catch (Microsoft.Exchange.WebServices.Data.ServiceXmlDeserializationException ex)
                {
                    // Output the more generic error to the logs
                    _log.WriteError(ex);

                    // Output the probable cause to logs
                    _log.WriteLine(Logging.Level.CRITICAL, "Exchange Web Service Encountered an error!");
                    _log.WriteLine(Logging.Level.CRITICAL, "  This could be caused by a login failure.");
                    _log.WriteLine(Logging.Level.CRITICAL, "  Check your connection information in the settings and restart the service.");

                    // Quit the service because this error cannot be recovered from
                    return;
                }

                // Catch other general exceptions
                catch (Exception ex)
                {
                    // Write the exception to the log
                    _log.WriteError(ex);
                }

                // Collect the garbage
                _log.WriteLine(Logging.Level.DEBUG, "Memory Allocated (Before): " + GC.GetTotalMemory(false));
                _log.WriteLine(Logging.Level.DEBUG, "Collecting Garbage...");
                GC.Collect();
                _log.WriteLine(Logging.Level.DEBUG, "Memory Allocated (After) : " + GC.GetTotalMemory(true));

                lock (_lockObject)
                {
                    // Sleep for the set time in the settings
                    Monitor.Wait(_lockObject, new TimeSpan(0, 0, 0, threadSleep));

                    // If there was a request to cancel the thread
                    if (_cancelToken.Token.IsCancellationRequested)
                    {
                        // Exit the thread
                        return;
                    }
                }

                _log.WriteLine(Logging.Level.DEBUG, "Reached End of Thread Loop");
            }
        }

        private void ThreadSetup()
        {
            // Parse the settings file
            this._settings = new Settings();
            _settings.Parse();

            // Convert LogLocalLocation setting from string to bool
            bool localLocation;
            bool.TryParse(_settings.General["LogLocalLocation"], out localLocation);

            // Convert LogLevel from string to enum
            Logging.Level logLevel;
            Enum.TryParse(_settings.General["LogLevel"], true, out logLevel);

            // Setup the logging file
            _log = new Logging(_settings.General["LogLocation"], logLevel, localLocation);

            // Log the start of the thread with some settings information
            _log.Begin(_settings.General);
        }
    }
}
