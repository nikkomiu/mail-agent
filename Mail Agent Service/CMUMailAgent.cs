using System;
using System.ServiceProcess;

namespace Mail_Agent_Service
{
    public partial class CMUMailAgent : ServiceBase
    {
        ServiceThread _svcThread;
        
        public CMUMailAgent()
        {
            // Create new Service Thread object
            _svcThread = new ServiceThread();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            // Start the Service Thread
            _svcThread.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();

            // Stop the Service Thread Gracefully
            _svcThread.Stop();
        }
    }
}
