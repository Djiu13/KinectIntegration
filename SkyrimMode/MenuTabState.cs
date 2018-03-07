using System;
using Microsoft.Kinect;
using RemotingInterfaces;
using States;
using WindowsInput;
using System.Threading;

namespace SkyrimMode
{
    public class MenuTabState : State
    {
        private enum Pos : int
        {
            DOWN = 0, LEFT = 1, RIGHT = 2, UP = 3
        }
        private IRemoteOperation _remote;

        private long previousTimeStamp;
        private SkeletonPoint previousPosition;
        private bool[] selected;

        public MenuTabState()
        {
            _remote = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
                "tcp://localhost:2345/RemoteOperation");
            selected = new bool[4] { false, false, false, false };
            this.Name = "menu";
        }


        public override void skeletChanged(Skeleton s, long timeStamp)
        {
            Joint rightHand = s.Joints[JointType.HandRight];
            Joint shoulderCenter = s.Joints[JointType.ShoulderCenter];
            Joint HipCenter = s.Joints[JointType.HipCenter];
            Joint hipLeft = s.Joints[JointType.HipLeft];
            Joint hipRight = s.Joints[JointType.HipRight];

            float dx = rightHand.Position.X - previousPosition.X;
            float dy = rightHand.Position.Y - previousPosition.Y;
            long dt = timeStamp - previousTimeStamp;
            float speedX = 1000 * dx / dt;
            float speedY = 1000 * dy / dt;

            previousPosition = rightHand.Position;
            previousTimeStamp = timeStamp;

            if (rightHand.Position.Y < HipCenter.Position.Y)
            {   
                selected[(int)Pos.LEFT] = false;
                selected[(int)Pos.RIGHT] = false;
                selected[(int)Pos.UP] = false;
                if (!selected[(int)Pos.DOWN])
                {
                    selected[(int)Pos.DOWN] = true;
                    CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_S));
                    CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_S));
                    return;
                }
                //    else if (speedY < -2.5)
                //    {
                //        //CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.DOWN));
                //        //CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.DOWN));
                //        selected[(int)Pos.DOWN] = false;
                //    }
            }
            else
            {
                selected[(int)Pos.DOWN] = false;
                if (rightHand.Position.Y > shoulderCenter.Position.Y)
                {
                    selected[(int)Pos.LEFT] = false;
                    selected[(int)Pos.RIGHT] = false;
                    if (!selected[(int)Pos.UP])
                    {
                        selected[(int)Pos.UP] = true;
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Z));
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Z));
                        return;
                    }
                    else if (speedY < -2.5)
                    {
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Z));
                        CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Z));
                        CallStateEvent(new StateChangeEventArgs(new SkillMenuState()));
                        Thread.Sleep(600);
                        return;
                    }
                }
                else
                    if (rightHand.Position.X < hipLeft.Position.X)
                    {
                        selected[(int)Pos.UP] = false;
                        selected[(int)Pos.RIGHT] = false;
                        if (!selected[(int)Pos.LEFT])
                        {
                            selected[(int)Pos.LEFT] = true;
                            CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Q));
                            CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Q));
                            return;
                        }
                        else if (speedX > 2.5) //swap left
                        {
                            CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_Q));
                            CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_Q));
                            CallStateEvent(new StateChangeEventArgs(new MagicMenuState()));
                            Thread.Sleep(600);
                            return;
                        }
                    }
                    else
                        if (rightHand.Position.X > hipRight.Position.X)
                        {
                            selected[(int)Pos.LEFT] = false;
                            selected[(int)Pos.UP] = false;
                            if (!selected[(int)Pos.RIGHT])
                            {
                                selected[(int)Pos.RIGHT] = true;
                                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_D));
                                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_D));
                                return;
                            }
                            else if (speedX < -2.5) //swap right
                            {
                                _remote.message(speedX.ToString());
                                CallKeyEvent(new KeyInputEventArgs(KeyStatut.DOWN, VirtualKeyCode.VK_D));
                                CallKeyEvent(new KeyInputEventArgs(KeyStatut.UP, VirtualKeyCode.VK_D));
                                CallStateEvent(new StateChangeEventArgs(new ObjectMenuState()));
                                Thread.Sleep(600);
                                return;
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
        }
    }
}
