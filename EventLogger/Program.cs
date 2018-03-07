using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace EventLogger
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();

        private Thread _thread;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // Initialize and run the service
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new EventLoggerService() };
            ServiceBase.Run(ServicesToRun); 
        }

        public void DMain()
        {
            EventLoggerApp evenApp = new EventLoggerApp();

            _thread = new Thread(new ThreadStart(evenApp.Start));
            _thread.SetApartmentState(ApartmentState.MTA);
            _thread.Start();
            
        }

        public void Stop()
        {
            _thread.Abort();

        }
    }
}