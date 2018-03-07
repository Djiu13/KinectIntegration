using System;
using System.Threading;
using Microsoft.Kinect;
using RemotingInterfaces;
using States;
using WindowsInput;


namespace SkyrimMode
{
    public class FightState : State
    {
        private Boolean handRaised;
        private long timeStampHand = 0;
        private float shoulderCenter;
        private Boolean crouched;

        private IRemoteOperation _remote;

        public FightState()
        {
            this.handRaised = false;
            this._remote = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
                "tcp://localhost:2345/RemoteOperation");
            this.shoulderCenter = -42;
            this.crouched = false;
            this.Name = "fight";
        }

        public override void skeletChanged(Skeleton s, long timeStamp)
        {
            Joint leftHand = s.Joints[JointType.HandLeft];
            Joint footRight = s.Joints[JointType.FootRight];
            Joint footLeft = s.Joints[JointType.FootLeft];
            Joint shoulderRight = s.Joints[JointType.ShoulderRight];
            Joint shoulderLeft = s.Joints[JointType.ShoulderLeft];

            handleCrouch(s.Joints, timeStamp);
            handleSword(s.Joints, timeStamp);

            //avancer / reculer
            #region avancer_reculer
            {
                if (footLeft.Position.Z - footRight.Position.Z > 0.3)
                {
                    CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Z));
                }
                else
                    if (footRight.Position.Z - footLeft.Position.Z > 0.3)
                    {
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_S));
                    }
                    else
                    {
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Z));
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_S));
                    }
            }
            #endregion

            //bouclier
            #region bouclier
            {
                if (leftHand.Position.Y > shoulderLeft.Position.Y)
                    this._remote.mouseRightDown();
                else
                    this._remote.mouseRightUp();
            }
            #endregion

            //regarder à gauche
            #region look_gauche

            if (Math.Round(shoulderLeft.Position.Z, 2) - Math.Round(shoulderRight.Position.Z, 2) >= 0.14)
            {
                this._remote.moveCursorToLeft();
            }
            else
                if (Math.Round(shoulderRight.Position.Z, 2) - Math.Round(shoulderLeft.Position.Z, 2) >= 0.14)
                {
                    this._remote.moveCursorToRight();
                }
            #endregion

        }

        private void handleCrouch(JointCollection joints, long timeStamp)
        {
            Joint head = joints[JointType.Head];

            if (this.shoulderCenter == -42)
                this.shoulderCenter = joints[JointType.ShoulderCenter].Position.Y;

            if (head.Position.Y < this.shoulderCenter && !crouched)
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.LCONTROL));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.LCONTROL));
                this.crouched = true;
            }
            else if (crouched && head.Position.Y > this.shoulderCenter)
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.LCONTROL));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.LCONTROL));
                this.crouched = false;
            }
        }

        private void handleSword(JointCollection joints, long timeStamp)
        {
            Joint rightHand = joints[JointType.HandRight];
            Joint shoulderRight = joints[JointType.ShoulderRight];

            if (rightHand.Position.Y > shoulderRight.Position.Y)
            {
                if (!this.handRaised)
                {
                    this.handRaised = true;
                    this.timeStampHand = timeStamp;
                }
            }
            else if (this.handRaised)
            {
                this._remote.mouseLeftDown();
                if (timeStamp - this.timeStampHand > 800)
                {
                    Thread.Sleep(2000);
                }
                else
                    Thread.Sleep(100);
                this._remote.mouseLeftUp();
                this.handRaised = false;
            }
        }

        public override void recognizedWord(String word)
        {
            if (word.Equals("cry"))
            {

                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_W));
                Thread.Sleep(1000);
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_W));
            }
            if (word.Equals("use"))
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_E));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_E));
            }
            if (word.Equals("menu"))
            {
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.TAB));
                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.TAB));
                CallStateEvent(new StateChangeEventArgs(new MenuTabState()));
            }
        }
    }

}
