using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using RemotingInterfaces;
using System.Threading;
using IModes;

namespace EventLogger.classes
{
    class SpeechRecognition
    {
        //Id pour la référence de la reco vocale
        private const string RecognizerId = "SR_MS_en-US_TELE_10.0";

        #region Member fields
        // Private fields
        // pour la communication avec le client
        private IRemoteOperation _remoteOperation;
        // pour référencer la prononciation au plugin
        private Dictionary<String, IMode> _profileWords;
        // la culture de l'id
        private CultureInfo _culture;
        // la grammaire chargé
        private Grammar _currentGrammar = null;
        // le moteur de reco vocale
        private SpeechRecognitionEngine _sre = null;
        // la source de l'audio : Kinect
        private KinectAudioSource _source;
        // le flux de l'audio
        private Stream _stream;
        // le blocage de toutes reco
        private Boolean isLocked = false; 
        #endregion Member fields

        #region Public methods
        public SpeechRecognition()
        {
            //on prend le client
            _remoteOperation = (IRemoteOperation)Activator.GetObject(typeof(IRemoteOperation),
                "tcp://localhost:2345/RemoteOperation");

            _profileWords = new Dictionary<String, IMode>();

            //Load plugins
            ModeLoader modeLoader = new ModeLoader(Properties.Resources.pluginFolder);
            foreach (IMode m in modeLoader.getListMode())
            {
                //on prend le nom du plugin + son instance
                _profileWords.Add(m.getModeName(), m);
            }
        }
        public void configureAudio(KinectSensor nui)
        {
            //on configure la source de l'audio
            _source = nui.AudioSource;
            _source.EchoCancellationMode = EchoCancellationMode.CancellationOnly;
            _source.AutomaticGainControlEnabled = false;
            _source.BeamAngleMode = BeamAngleMode.Adaptive;
            
            //on récupère le dictionnaire de la langue choisi
            RecognizerInfo ri = SpeechRecognitionEngine.InstalledRecognizers().Where(r => r.Id == RecognizerId).FirstOrDefault();

            if (ri == null)
            {
                _remoteOperation.message("Impossible de trouver le fichier de langue: reconnaissance vocale désactivée");
                return;
            }

            _remoteOperation.message("Configuration de la reconnaissance vocale....");

            _sre = new SpeechRecognitionEngine(ri.Id);

            //on charge les mots de profils et ajoute le verrouillage de la voix 
            Choices words = new Choices();
            _profileWords.Add("Lock my voice.",null);
            _profileWords.Add("Unlock my voice.", null);
            foreach (String s in _profileWords.Keys)
                words.Add(s);

            //on contruit la grammaire autour de ces mots
            GrammarBuilder gb = new GrammarBuilder { Culture = ri.Culture }; 
            _culture = ri.Culture;
            gb.Append(words);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            Grammar g = new Grammar(gb);
            _sre.LoadGrammar(g);
            Thread.Sleep(1000);
            //on s'abonne à la reconnaissance des mots
            _sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SreSpeechRecognized);
        }
        public void Ecoute()
        {
            //on ecoute le flux audio et on demande à l'analyser
            if (_sre == null) return;
            _stream = _source.Start();
            
                _remoteOperation.message("start recognition....");
                _sre.SetInputToAudioStream(_stream,
                    new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                _sre.RecognizeAsync(RecognizeMode.Multiple);
            
        }
        public void Stop()
        {
            //on arrête toute la reco vocale
            if (_sre != null)
            {
                _sre.RecognizeAsyncCancel();
                _sre.RecognizeAsyncStop();
                _source.Stop();
            }
        }
        #endregion Public methods

        #region Private methods
        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String s = "\rSpeech Recognized: \t" + e.Result.Text + " at : " + e.Result.Confidence;

            //on affiche le mot reconnu
            _remoteOperation.message(s);
            if (e.Result.Confidence < 0.5)
                return;

            String result = e.Result.Text;
            //Ignoring cases
            #region lockVoice
            if (result.Equals("Lock my voice.") && !isLocked)
            {
                isLocked = true;
                this._remoteOperation.play("OK, i'm ignoring you!");
            }
            if (result.Equals("Unlock my voice.") && isLocked)
            {
                isLocked = false;
                this._remoteOperation.play("Helloooooooo!");
            }
            if (isLocked)
                return;
            #endregion

            //on regarde si cela correspond au nom de profile
            if (_profileWords.ContainsKey(result))
            {
                //si c'est le cas, on va charger le profile
                if (_profileWords[result] != null)
                {
                    if (SkeletTracking.mode != null)
                        SkeletTracking.mode.unloadAction(); // on decharge toute les actions de l'ancien mode
                    SkeletTracking.mode = (IMode)Activator.CreateInstance(_profileWords[result].GetType()); // on charge le nouveau mode
                    reloadNewGrammar(); // on recharge la grammaire
                    _remoteOperation.play("The " + result + " profile is enabled."); // on indique que le mode est actif
                }
            }
            else
                SkeletTracking.mode.recognizedWord(result); // on dit au mode en cours que l'on a reconnu son mot
        }
        private void traitmentSpeech(string message)
        {
        }
        private void reloadNewGrammar()
        {
            //unload the grammar
            if (_currentGrammar != null)
                _sre.UnloadGrammar(_currentGrammar);

            //load new grammar
            _currentGrammar = null;
            
            //get current grammar for the mode
            GrammarBuilder gb = SkeletTracking.mode.getGrammar();
           
            if (gb != null)
            {
                gb.Culture = _culture;

                // Create the actual Grammar instance, and then load it into the speech recognizer.
                _currentGrammar = new Grammar(gb);
                _sre.LoadGrammar(_currentGrammar);
            }
        }
        //Useless
        private void DumpRecordedAudio(RecognizedAudio audio)
        {
            if (audio == null)
                return;

            int fileId = 0;
            string filename;
            while (File.Exists((filename = "RetainedAudio_" + fileId + ".wav")))
                fileId++;

            _remoteOperation.message("\nWriting file: " + filename);
            using (var file = new FileStream(filename, System.IO.FileMode.CreateNew))
                audio.WriteToWaveStream(file);
        }
        #endregion Private methods
    }
}
