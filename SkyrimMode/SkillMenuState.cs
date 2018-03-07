using System;
using Microsoft.Kinect;
using RemotingInterfaces;
using States;
using WindowsInput;
using System.Threading;

namespace SkyrimMode
{
    class SkillMenuState : State
    {
        private IRemoteOperation _remote;

        private long previousTimeStamp;
        private SkeletonPoint previousPositionR; //right
        private SkeletonPoint previousPositionL; //left
        private bool rightPressed;
        private bool leftPressed;

        public SkillMenuState()
        {
            this._remote = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
                "tcp://localhost:2345/RemoteOperation");
            this.previousTimeStamp = 0;
            this.rightPressed = false;
            this.leftPressed = false;
            this.Name = "skill menu";
        }

        public override void skeletChanged(Skeleton s, long timeStamp)
        {
            handleRightMove(s.Joints, timeStamp);
            handleLeftMove(s.Joints, timeStamp);

            this.previousPositionR = s.Joints[JointType.HandRight].Position;
            this.previousPositionL = s.Joints[JointType.HandLeft].Position;
            this.previousTimeStamp = timeStamp;
        }

        private void handleRightMove(JointCollection joints, long timeStamp)
        {
            float dx = joints[JointType.HandRight].Position.X - previousPositionR.X;
            long dt = timeStamp - previousTimeStamp;
            float speed = 1000 * dx / dt;

            //if (speed < -2.5) //swap right
            //{
            //    CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_D));
            //    CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_D));
            //    Thread.Sleep(600);
            //    return;
            //}

            if (Math.Floor(speed) == 0 &&
               joints[JointType.ElbowRight].Position.Y > joints[JointType.ShoulderCenter].Position.Y)
            {
                if(!this.rightPressed )
                {
                    CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_D));
                    //Thread.Sleep(600);
                    this.rightPressed = true;
                }
            }
            else if (this.rightPressed)
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_D));
                this.rightPressed = false;
            }

        }

        private void handleLeftMove(JointCollection joints, long timeStamp)
        {
            float dx = joints[JointType.HandLeft].Position.X - previousPositionL.X;
            long dt = timeStamp - previousTimeStamp;
            float speed = 1000 * dx / dt;

            //if (speed > 2.5) //swap left
            //{
            //    CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Q));
            //    CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Q));
            //    Thread.Sleep(600);
            //    return;
            //}

            //Rester appuyé sur la gauche
            if (Math.Floor(speed) == 0 &&
               joints[JointType.ElbowLeft].Position.Y > joints[JointType.ShoulderCenter].Position.Y)
            {
                if (!this.leftPressed)
                {
                    CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Q));
                    //Thread.Sleep(600);
                    this.leftPressed = true;
                }
            }
            else if (this.leftPressed)
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Q));
                this.leftPressed = false;
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
        }
    }
}
