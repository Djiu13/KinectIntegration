using System;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using EventLogger.classes;
using Microsoft.Kinect;
using RemotingInterfaces;

namespace EventLogger
{
    public class EventLoggerApp
    {
        #region Member fields
        // Private fields
        private Thread _thread;
        private IRemoteOperation _remoteOperation;

        #endregion Member fields

        private const string port = "2345";

        #region kinect
            private SkeletTracking _tracker = null;
            private SpeechRecognition _speech = null;
            //Kinect Runtime
            private KinectSensor _nui = null;
        #endregion kinect

        #region Public methods

        [MTAThread]
        public void Start()
        {
            //Creation du channel de communication
            TcpChannel channel = new TcpChannel();
            _remoteOperation = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
                "tcp://localhost:" + port + "/RemoteOperation");

            //Demarre la configuration de la kinect
            if (SetupKinect())
            {
                _remoteOperation.message("Le client a démarré avec succés");
                Execute();
            }
            else
            {
                // Environment.Exit(1);
            }
        }

        public void Stop()
        {
            //On arrête toutes les captures:
            //la voix + les gestes
            if (_speech != null)
                _speech.Stop();
            if (_nui != null && _nui.IsRunning)
                _nui.Stop();

            if (_thread != null)
            {
                _thread.Abort();
                // _thread.Join();
            }
        }

        #endregion Public methods

        #region Private methods
        //Thread principal
        [MTAThread]
        private void Execute()
        {
            // Loop until the thread gets aborted  
            _speech.Ecoute(); // on ecoute le flux audio
            try
            {
                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }
        private bool SetupKinect()
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                _remoteOperation.message("Aucun peripherique Kinect detecté");
                return false;
            }
            //use first Kinect
            _nui = KinectSensor.KinectSensors[0];
            //Initialize to do skeletal tracking
            {
                //to experiment, toggle TransformSmooth between true & false
                // parameters used to smooth the skeleton data
                TransformSmoothParameters parameters = new TransformSmoothParameters()
                {
                    Smoothing = 0.7f,
                    Correction = 0.3f,
                    Prediction = 0.4f,
                    JitterRadius = 1.0f,
                    MaxDeviationRadius = 0.5f
                };

                //enable skeleton
                _nui.SkeletonStream.Enable(parameters);

                //add event to receive skeleton data
                _tracker = new SkeletTracking();
                _nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(_tracker.nui_SkeletonFrameReady);
            }
            //Audio
            {
                _speech = new SpeechRecognition();
                _speech.configureAudio(_nui);
            }
            _nui.Start();

            return true;
        }
        #endregion Private methods
    }
}
