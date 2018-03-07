using System;
using Microsoft.Kinect;

namespace States
{
    public interface IStateChangable
    {
        void KeyEventHandler(object sender, KeyInputEventArgs e);
        void StateChangeEventHandler(object sender, StateChangeEventArgs e);
    }

    public delegate void KeyEventHandler(object sender, KeyInputEventArgs fe);
    public delegate void StateChangeEventHandler(object sender, StateChangeEventArgs fe);
    
    public abstract class State
    {
        private String name;
        public string Name { get {return name;}
             set {name = Name;}
        }
        
        public abstract void skeletChanged(Skeleton s, long timeStamp);
        public abstract void recognizedWord(String word);

        public event KeyEventHandler KeyEvent;
        public event StateChangeEventHandler StateEvent;

        protected void CallKeyEvent(KeyInputEventArgs fe)
        {
            KeyEvent(this, fe);
        }

        protected void CallStateEvent(StateChangeEventArgs fe)
        {
            StateEvent(this, fe);

        }


    }
}
