using System;
using Microsoft.Kinect;
using RemotingInterfaces;
using States;
using WindowsInput;
using System.Threading;

namespace SkyrimMode
{
    class ObjectMenuState : State
    {

        private IRemoteOperation _remote;

        private long previousTimeStamp;
        private long previousTimeStampHand;
        private SkeletonPoint previousPositionR;
        private bool atBegin;

        public ObjectMenuState()
        {
            _remote = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
                "tcp://localhost:2345/RemoteOperation");
            atBegin = true;
            previousTimeStampHand = 0;
            this.Name = "object menu";
        }

        public override void skeletChanged(Skeleton s, long timeStamp)
        {
            handleUpDown(s.Joints, timeStamp);

            this.previousPositionR = s.Joints[JointType.HandRight].Position;
            this.previousTimeStamp = timeStamp;
        }

        private void handleSwap(JointCollection jointCollection, long timeStamp)
        {
            Joint rightHand = jointCollection[JointType.HandRight];

            float d2 = rightHand.Position.X - this.previousPositionR.X;
            long dt = timeStamp - this.previousTimeStamp;
            float speed2 = 1000 * d2 / dt;


            if (speed2 < -2.5) //swap right menu
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_D));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_D));
                this.atBegin = false;
            }

            if (speed2 > 2.5) //swap right menu
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Q));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Q));
                if (this.atBegin)
                    CallStateEvent(new StateChangeEventArgs(new MenuTabState()));
                this.atBegin = true;
            }
        }

        private void handleUpDown(JointCollection jointCollection, long timeStamp)
        {
            Joint leftHand = jointCollection[JointType.HandLeft];
            Joint shoulderLeft = jointCollection[JointType.ShoulderLeft];
            Joint hipLeft = jointCollection[JointType.HipLeft];

            if (leftHand.Position.X < hipLeft.Position.X)
            {
                if (leftHand.Position.Y  > shoulderLeft.Position.Y) //main positionnée en haut
                {
                    if (timeStamp - this.previousTimeStampHand >= 1000) //toutes les 1000ms
                    {
                       //this._remote.message("up");
                        this.previousTimeStampHand = timeStamp;
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Z));
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Z));
                    }
                }
                else if (leftHand.Position.Y < hipLeft.Position.Y)//main positionnée en bas
                {
                    if (timeStamp - this.previousTimeStampHand >= 1000)
                    {
                        //this._remote.message("down");
                        this.previousTimeStampHand = timeStamp;
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_S));
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_S));
                    }
                }
            }
            else
                handleSwap(jointCollection,  timeStamp);
        }

        public override void recognizedWord(String word)
        {
            if (word.Equals("leave"))
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.TAB));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.TAB));

                CallStateEvent(new StateChangeEventArgs(new FightState()));
            }
            if (word.Equals("use"))
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_E));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_E));
            }
        }
    }
}
