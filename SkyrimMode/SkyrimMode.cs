using System;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using SkyrimMode;
using States;


namespace IModes
{
    class SkyrimMode : IMode, IStateChangable
    {
        private State _currentState
        {
            get;
            set;
        }

        public SkyrimMode()
            : base()
        {
            ModeName = "skyrim mode";
            _currentState = new FightState();
            this._currentState.KeyEvent += new KeyEventHandler(KeyEventHandler);
            this._currentState.StateEvent += new StateChangeEventHandler(StateChangeEventHandler);
        }

        public override GrammarBuilder getGrammar()
        {
            if (this.grammar == null)
            {
                Choices words = new Choices();

                words.Add("cry");
                words.Add("use");
                words.Add("menu");
                words.Add("leave");

                this.grammar = new GrammarBuilder();
                this.grammar.Append(words);
            }
            return this.grammar;
        }

        public void KeyEventHandler(object sender, KeyInputEventArgs e)
        {
            if (e.statut == KeyStatut.DOWN)
            {
                this.remoteOperation.press_key_down(e.key);
                this.keysPress.Add(e.key);
            }
            else
            {
                this.remoteOperation.press_key_up(e.key);
                this.keysPress.Remove(e.key);
            }
        }
        public void StateChangeEventHandler(object sender, StateChangeEventArgs e)
        {
            this._currentState.KeyEvent -= new KeyEventHandler(KeyEventHandler);
            this._currentState.StateEvent -= new StateChangeEventHandler(StateChangeEventHandler);
            this._currentState = e.nextState;
            this._currentState.KeyEvent += new KeyEventHandler(KeyEventHandler);
            this._currentState.StateEvent += new StateChangeEventHandler(StateChangeEventHandler);
            this.remoteOperation.play("Entering " + this._currentState.Name + " state.");
        }
        
        public override void skeletChanged(Skeleton s, long timeStamp)
        {
            _currentState.skeletChanged(s, timeStamp);
        }

        public override void recognizedWord(String word)
        {
             _currentState.recognizedWord(word);
        }
    }
}
