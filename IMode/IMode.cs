using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using RemotingInterfaces;
using WindowsInput;

namespace IModes
{
    public abstract class IMode
    {

        protected IRemoteOperation remoteOperation;
        //Grammaire
        protected GrammarBuilder grammar = null;
        //Nom du mode
        protected String ModeName;
        //Touches appuyees
        protected List<VirtualKeyCode> keysPress;


        public abstract void skeletChanged(Skeleton s, long timeStamp);
        public abstract void recognizedWord(String word);


        public virtual GrammarBuilder getGrammar()
        {
            return this.grammar;
        }

        public IMode()
        {
            remoteOperation = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
                "tcp://localhost:2345/RemoteOperation");
            keysPress = new List<VirtualKeyCode>();
        }

        public String getModeName()
        {
            return (ModeName);
        }

        public virtual void unloadAction()
        {
            foreach (VirtualKeyCode key in keysPress)
            {
                remoteOperation.press_key_up(key);
            }
        }
    }
}
