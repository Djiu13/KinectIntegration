using System;
using WindowsInput;

namespace States
{
    public enum KeyStatut
    {
        UP,
        DOWN
    }


    public class StateChangeEventArgs : EventArgs
    {
        public State nextState;

        public StateChangeEventArgs(State next)
        { 
            this.nextState = next;
        }
       
    }

    public class KeyInputEventArgs : EventArgs
    {
        public KeyStatut statut;
        public KeyInputEventArgs(KeyStatut stat, VirtualKeyCode key)
        {
            this.statut = stat;
            this.key = key;
        }

        public VirtualKeyCode key { get; set; }
    }

}
