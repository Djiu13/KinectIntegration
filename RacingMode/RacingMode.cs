using System;
using Microsoft.Kinect;
using WindowsInput;

namespace IModes
{
    public class RacingMode : IMode
    {
        private bool isTurning;
        private SkeletonPoint oldRightPosition;
        private bool isBraking;
        private bool isDriving;

        public RacingMode()
            : base()
        {
            ModeName = "racing mode";
            isTurning = false;
            isBraking = false;
            isDriving = false;
            //keysPress.Add(VirtualKeyCode.VK_Z);
        }

        public override void recognizedWord(String word)
        {

        }


        public override void skeletChanged(Skeleton s, long timeStamp)
        {
          // remoteOperation.press_key_down(VirtualKeyCode.VK_Z);
          //  
            calculHandsPosition(s.Joints, timeStamp);
            calculFootPosition(s.Joints);
        }

        private void calculFootPosition(JointCollection joints)
        {
            Joint footLeft = joints[JointType.FootLeft];
            Joint footRight = joints[JointType.FootRight];

            Console.WriteLine((footLeft.Position.Z - footRight.Position.Z).ToString());
            if (footLeft.Position.Z - footRight.Position.Z > 0.2)
            {
                remoteOperation.press_key_down(VirtualKeyCode.UP);
                keysPress.Add(VirtualKeyCode.UP);
            }
            else
                if (footRight.Position.Z - footLeft.Position.Z > 0.2)
                {
                    remoteOperation.press_key_down(VirtualKeyCode.DOWN);
                    keysPress.Add(VirtualKeyCode.DOWN);
                }
                else
                {
                    remoteOperation.press_key_up(VirtualKeyCode.UP);
                    remoteOperation.press_key_up(VirtualKeyCode.DOWN);
                    keysPress.Remove(VirtualKeyCode.UP);
                    keysPress.Remove(VirtualKeyCode.DOWN);
                }
        }

        private void calculHandsPosition(JointCollection joints, long p)
        {
            Joint jointLeft = joints[JointType.HandLeft];
            Joint jointRight = joints[JointType.HandRight];

            float Xsquare = (jointLeft.Position.X - jointRight.Position.X);
            Xsquare *= Xsquare;

            float Ysquare = (jointLeft.Position.Y - jointRight.Position.Y);
            Ysquare *= Ysquare;

            double distance = Math.Sqrt(Xsquare + Ysquare);

            //bool handAligned = (jointLeft.Position.Z > jointRight.Position.Z - 0.1 && jointLeft.Position.Z < jointRight.Position.Z + 0.1);
            //handAligned = handAligned && (jointRight.Position.Z > jointLeft.Position.Z - 0.1 && jointRight.Position.Z < jointLeft.Position.Z + 0.1);


            if (distance <= 0.4)//&& handAligned)
            {

                isBraking = false;
                oldRightPosition = jointRight.Position;
                drivingWithTwoHands(jointLeft.Position, jointRight.Position);
            }
            else
            {
                remoteOperation.press_key_up(VirtualKeyCode.RIGHT);
                remoteOperation.press_key_up(VirtualKeyCode.LEFT);
            }
            /*else
            {
                remoteOperation.message("*********** ONE hands *******************");
                drivingWithOneHands(jointLeft.Position, oldRightPosition, distance);
                trackRightHand(jointRight.Position, joints[JointType.HipCenter].Position);
                remoteOperation.message("*****************************************");
            }*/
        }

        private void trackRightHand(SkeletonPoint vectorRight, SkeletonPoint vectorHip)
        {
            if (isBraking && vectorRight.Y > vectorHip.Y && vectorRight.X > vectorHip.X)
            {
                remoteOperation.press_key_down(VirtualKeyCode.DOWN);
                keysPress.Add(VirtualKeyCode.DOWN);
                isBraking = false;
            }
            if (vectorRight.Y < vectorHip.Y && vectorRight.X > vectorHip.X)
            {
                bool removeBrake = keysPress.Contains(VirtualKeyCode.DOWN);
                if (removeBrake)
                {
                    remoteOperation.press_key_up(VirtualKeyCode.DOWN);
                    keysPress.Remove(VirtualKeyCode.DOWN);
                }
                else
                    isBraking = true;
            }
        }

        public void drivingWithTwoHands(SkeletonPoint leftV, SkeletonPoint rightV)
        {
            double coef;

            if (leftV.X > rightV.X)
            {
                if (leftV.Y > rightV.Y)
                    coef = -1; // we must go to right
                else
                    coef = 1; //we must go to left
            }
            else
            {
                coef = (rightV.Y - leftV.Y);
                coef = coef / (rightV.X - leftV.X);
            }

            if (coef > .25 && !isTurning)
            {
                isTurning = true;
                remoteOperation.press_key_down(VirtualKeyCode.LEFT);
                //remoteOperation.message("gauche");
                keysPress.Add(VirtualKeyCode.LEFT);
            }
            if (coef < -0.25 && !isTurning)
            {
                isTurning = true;
                remoteOperation.press_key_down(VirtualKeyCode.RIGHT);
                //remoteOperation.message("droite");
                keysPress.Add(VirtualKeyCode.RIGHT);
            }
            if (coef > -0.25 && coef < .25)
            {
                isTurning = false;
                remoteOperation.press_key_up(VirtualKeyCode.LEFT);
                remoteOperation.press_key_up(VirtualKeyCode.RIGHT);

                keysPress.Remove(VirtualKeyCode.LEFT);
                keysPress.Remove(VirtualKeyCode.RIGHT);
            }
        }

        private void drivingWithOneHands(SkeletonPoint leftV, SkeletonPoint rightV, double dist)
        {
            const float tx_error = 0.05f;
            SkeletonPoint topCenterPoint = new SkeletonPoint();
            topCenterPoint.X = (leftV.X + rightV.X) / 2;
            topCenterPoint.Y = ((leftV.Y + rightV.Y) / 2);
            topCenterPoint.Y += ((float)dist / 2);

            
            //not : left hand is approximatively 20% in the topCenterPoint
            // => we can turn
            if (!(leftV.X <= (topCenterPoint.X * (1 + tx_error)) && leftV.X >= (topCenterPoint.X * (1 - tx_error))
                && leftV.Y <= (topCenterPoint.Y * (1 + tx_error)) && leftV.Y >= (topCenterPoint.Y * (1 - tx_error))))
            {
                String dir = "";
                dir = (leftV.X > topCenterPoint.X) ? ("droite") : ("gauche");
                if (leftV.X > topCenterPoint.X)
                    drivingWithTwoHands(topCenterPoint, leftV);
                else
                    drivingWithTwoHands(leftV, topCenterPoint);
                //remoteOperation.message("troune a " + dir);
            }
            //else
                //remoteOperation.message("En zone neutre");
        }
    }
}
