using System;
using System.Threading;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using WindowsInput;
using System.Windows.Forms;

namespace IModes
{
    public class PCMode : IMode
    {
        private long previousTimeStamp;
        private SkeletonPoint previousPosition;
        private bool isInactive = false;
        private bool isJoin = false;
        private bool inFullScreen = false;

        public PCMode()
            : base()
        {
            //Nom du mode
            ModeName = "pc mode";
        }

        //Ajout des mots dans la grammaire
        public override GrammarBuilder getGrammar()
        {
            if (this.grammar == null)
            {
                Choices words = new Choices();
                //On definit les mots utilises dans le mode
                words.Add("push");
                words.Add("drag");
                words.Add("drop");
                words.Add("open");
                words.Add("click");

                this.grammar = new GrammarBuilder();
                this.grammar.Append(words);
            }
            return this.grammar;
        }

        //Methode appelee lorsqu'un mouvement du squelette est detecte
        public override void skeletChanged(Skeleton s, long timeStamp)
        {
            //Met a jour la position de la souris
            setCursorPosition(s.Joints[JointType.HandRight]);
            //Verifie si on passe en fullcreen
            passFullscreen(s.Joints[JointType.HandLeft], s.Joints[JointType.HandRight]);
            //Faire bouger les slides
            manageSlide(s.Joints[JointType.HandRight].Position, timeStamp);
        }

        //Methode utilisee pour deplacer le pointeur de la souris
        private void setCursorPosition(Joint joint)
        {
            var scaledJoint = joint.ScaleTo(SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height, .2f, .2f);
            //on effectue le deplacement du point X au point Y
            remoteOperation.moveCursorToPosition(scaledJoint.Position.X, scaledJoint.Position.Y);
        }

        //%ethode utilisee pour passer un powerpoint en plein ecran
        private void passFullscreen(Joint jointLeft, Joint jointRight)
        {
            //calcule la distance entre les mains
            float distX = (jointRight.Position.X - jointLeft.Position.X);
            float distY = (jointRight.Position.Y - jointLeft.Position.Y);

            if (distX < -0.05 && distY > -0.05 && distY < 0.05)
            {
                //Console.WriteLine("DistY : " + distX);
                //Console.WriteLine("DistY : " + distY);
                if (isJoin == false)
                    isJoin = true;
            }
            else
            {
                if (isJoin)
                {
                    isJoin = false;
                    //si on a les mains jointe et qu'on est en plein ecran, on quitte (touche echap)
                    if (inFullScreen)
                    {
                        inFullScreen = false;
                        //remoteOperation.message("not in fullScreen");
                        remoteOperation.press_key_down(VirtualKeyCode.ESCAPE);
                        remoteOperation.press_key_up(VirtualKeyCode.ESCAPE);
                    }
                    //si on a les main jointe on passe en plein ecran (touche F5)
                    else
                    {
                        inFullScreen = true;
                        //remoteOperation.message("in fullScreen");
                        remoteOperation.press_key_down(VirtualKeyCode.F5);
                        remoteOperation.press_key_up(VirtualKeyCode.F5);
                    }
                }
            }
        }

        //Methode utilisee pour faire defiler les slides
        private void manageSlide(SkeletonPoint vector, long p)
        {
            //Definition de la vitesse de la main droite
            var dx = vector.X - previousPosition.X;
            var dt = p - previousTimeStamp;
            var speed = 1000 * dx / dt;

            previousPosition = vector;
            previousTimeStamp = p;

            if (!isInactive)
            {
                //si le mouvement droite vers gauche est assez rapide, on passe au slide suivant
                if (speed < -2.5)
                {
                    isInactive = true;
                    //remoteOperation.message("right");
                    remoteOperation.press_key_down(VirtualKeyCode.DOWN);
                    remoteOperation.press_key_up(VirtualKeyCode.DOWN);
                    Thread.Sleep(600);
                    isInactive = false;
                }
                //si le mouvement gauche vers droite est assez rapide, on passe au slide précédent
                else if (speed > 2.5)
                {
                    isInactive = true;
                    //remoteOperation.message("left");
                    remoteOperation.press_key_down(VirtualKeyCode.UP);
                    remoteOperation.press_key_up(VirtualKeyCode.UP);
                    Thread.Sleep(600);
                    isInactive = false;
                }
            }
        }
        
        //Association des actions au mots clé defini plus haut
        public override void recognizedWord(String word)
        {
            //Maintenir le clique gauche de la souris
            if (word.Equals("push") || word.Equals("drag"))
                remoteOperation.mouseLeftDown();
            //Relacher le clique gauche de la souris
            if (word.Equals("drop"))
                remoteOperation.mouseLeftUp();
            //Effectuer un clique
            if (word.Equals("click"))
            {
                remoteOperation.mouseLeftDown();
                remoteOperation.mouseLeftUp();
            }
            //Faire un double clique
            if (word.Equals("open"))
            {
                remoteOperation.doubleClick();
            }
        }
    }
}
