using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace RemotingClient.classes
{
    public class RacingMode : RemotingInterfaces.Mode
    {
        public override void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame allSkeletons = e.SkeletonFrame;

            //get the first tracked skeleton
            SkeletonData skeleton = (from s in allSkeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();

            if (skeleton != null)
            {
                calculHandsPosition(skeleton.Joints[JointID.HandLeft], skeleton.Joints[JointID.HandRight]);
            }
        }

        private void calculHandsPosition(Joint jointLeft, Joint jointRight)
        {

            var scaledJointLeft = jointLeft.ScaleTo(1366, 768, .2f, .2f);
            var scaledJointRight = jointRight.ScaleTo(1366, 768, .2f, .2f);

            float Xsquare = (scaledJointLeft.Position.X - scaledJointRight.Position.X);
            Xsquare *= Xsquare;

            float Ysquare = (scaledJointLeft.Position.Y - scaledJointRight.Position.Y);
            Ysquare *= Ysquare;

            double distance = Math.Sqrt(Xsquare + Ysquare);

        }
    }
}
