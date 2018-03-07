using System;
using Microsoft.Kinect;
using RemotingInterfaces;
using States;
using WindowsInput;
using System.Threading;

namespace SkyrimMode
{
    class MagicMenuState : State
    {

        private IRemoteOperation _remote;

        private long previousTimeStamp;
        private long previousTimeStampHand;
        private SkeletonPoint previousPositionR;
        private bool atBegin;

        public MagicMenuState()
        {
            _remote = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
                "tcp://localhost:2345/RemoteOperation");
            atBegin = true;
            this.Name = "magic";
        }

        public override void skeletChanged(Skeleton s, long timeStamp)
        {
            handleUpDown(s.Joints, timeStamp);
            handleSwap(s.Joints, timeStamp);

            previousPositionR = s.Joints[JointType.HandRight].Position;
            previousTimeStamp = timeStamp;
        }

        private void handleSwap(JointCollection jointCollection, long timeStamp)
        {
            Joint rightHand = jointCollection[JointType.HandRight];

            float d1 = rightHand.Position.X - this.previousPositionR.X;
            long dt = timeStamp - this.previousTimeStamp;
            float speed1 = 1000 * d1 / dt;

            if (speed1 > 2.5) //swap left menu
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Q));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Q));
                Thread.Sleep(600);
                atBegin = false;
            }

            if (speed1 < -2.5) //swap right menu
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_D));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_D));
                if (atBegin)
                    CallStateEvent(new StateChangeEventArgs(new MenuTabState()));
                atBegin = true;
                Thread.Sleep(600);
            }
        }

        private void handleUpDown(JointCollection jointCollection, long timeStamp)
        {
            Joint rightHand = jointCollection[JointType.HandRight];
            Joint shoulderRight = jointCollection[JointType.ShoulderRight];
            Joint hipRight = jointCollection[JointType.HipRight];

            if (rightHand.Position.X > hipRight.Position.X)
            {
                if (rightHand.Position.Y > shoulderRight.Position.Y) //main positionnée en haut
                {
                    if (timeStamp - this.previousTimeStampHand >= 1000) //toutes les 1000ms
                    {
                        this.previousTimeStampHand = timeStamp;
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Z));
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Z));
                    }
                }
                else if (rightHand.Position.Y < hipRight.Position.Y)//main positionnée en bas
                {
                    if (timeStamp - this.previousTimeStampHand >= 1000)
                    {
                        this.previousTimeStampHand = timeStamp;
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_S));
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_S));
                    }
                }
            }
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
               this._remote.mouseLeftDown();
               this._remote.mouseLeftUp();
            }
        }
    }
}

