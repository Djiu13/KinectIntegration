using System;
using System.Linq;
using EventLogger.classes;
using RemotingInterfaces;
using IModes;
using Microsoft.Kinect;

namespace EventLogger
{
    public class SkeletTracking
    {
        #region Member fields
        // Private fields
        private IRemoteOperation _remoteOperation;
        static public IMode mode = null;

        #endregion Member fields
        private Skeleton[] skeletons;
        public SkeletTracking()
        {
            _remoteOperation = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
            "tcp://localhost:1069/RemoteOperation");
            
        }
        public void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //on récupère tous les skelette
            SkeletonFrame allSkeletons = e.OpenSkeletonFrame();
            skeletons = new Skeleton[allSkeletons.SkeletonArrayLength];
            allSkeletons.CopySkeletonDataTo(skeletons);
            //get the first tracked skeleton
            Skeleton skeleton = (from s in skeletons
                                 where s.TrackingState == SkeletonTrackingState.Tracked
                                 select s).FirstOrDefault();

            if (skeleton != null && mode != null)
            {
                //on indique au mode en cours que le skelette à changé de postion
                mode.skeletChanged(skeleton, allSkeletons.Timestamp);
            }
        }
    }
}
