using MailAgent.Domain;
using MailAgent.Domain.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MailAgent.Service
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
            FileMan.LocalFile("_settings.active.xml").Delete();

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

            // Loop "forever"
            while (true)
            {
                _log.WriteLine(Logging.Level.DEBUG, "Reached Start of Thread Loop");

                try
                {
                    // Create a new ExchangeServer
                    ExchangeServer exchange = new ExchangeServer(_settings.General, _log);

                    // Save mail in Exchange
                    exchange.SaveMail(_settings.Profiles);
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
                    _log.WriteLine(Logging.Level.CRITICAL, "  This is probably caused by a login failure.");
                    _log.WriteLine(Logging.Level.CRITICAL, "  Check your connection information in the settings and restart the service.");
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
                    Monitor.Wait(_lockObject, new TimeSpan(0, 0, 0, _settings.General.Mail.Polling));

                    // If there was a request to cancel the thread
                    if (_cancelToken.Token.IsCancellationRequested)
                    {
                        // Exit the thread
                        return;
                    }
                }

                _log.WriteLine(Logging.Level.DEBUG, "Reached End of Thread Loop");

                // Recreate the Logging File
                _log = new Logging(_settings.General.Log);
            }
        }

        private void ThreadSetup()
        {
            // Parse the settings file
            FileMan fileManager = FileMan.LocalFile("Settings.xml");
            _settings = fileManager.Deserialize<Settings>();

            // Set defaults for profiles
            foreach (Profile profile in _settings.Profiles.Where(x => x.ID == null || x.ID == Guid.Empty))
            {
                profile.ID = Guid.NewGuid();
            }

            foreach (Profile profile in _settings.Profiles.Where(x => x.KeyDelimiter == null || (x.IsDefaultKeyDelimiter == true && x.KeyDelimiter != _settings.General.Export.KeyDelimiter)))
            {
                profile.KeyDelimiter = _settings.General.Export.KeyDelimiter;
                profile.IsDefaultKeyDelimiter = true;
            }

            foreach (Profile profile in _settings.Profiles.Where(x => x.SavePath == null || (x.IsDefaultSavePath == true && x.SavePath != _settings.General.DefaultSavePath)))
            {
                profile.SavePath = _settings.General.DefaultSavePath;
                profile.IsDefaultSavePath = true;
            }

            fileManager = FileMan.LocalFile("_settings.active.xml");
            fileManager.Serialize<Settings>(_settings);

            // Setup the logging file
            _log = new Logging(_settings.General.Log);

            // Log the start of the thread with some settings information
            _log.Begin(_settings.General);
        }
    }
}
