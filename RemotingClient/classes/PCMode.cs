using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using System.Drawing;
using System.Windows.Forms;

namespace RemotingClient.classes
{
    public class PCMode : RemotingInterfaces.Mode
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
                //set position
                setCursorPosition(skeleton.Joints[JointID.HandRight]);
            }
        }

        private void setCursorPosition(Joint joint)
        {

            var scaledJoint = joint.ScaleTo(1366, 768, .2f, .2f);

            Cursor.Position = new Point(Convert.ToInt32(scaledJoint.Position.X), Convert.ToInt32(scaledJoint.Position.Y));
        }
    }
}
