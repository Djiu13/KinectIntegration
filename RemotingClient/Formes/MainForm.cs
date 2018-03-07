using System;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using EventLogger;

namespace RemotingClient.Formes
{
    public partial class MainForm : Form
    {
        private const int port = 2345;
        private Program _program = null;
        private readonly object padlock = new object();


        public MainForm()
        {
            InitializeComponent();
            this.rtbConsole.ReadOnly = true;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                // Création d'un nouveau canal d'écoute sur le port 1069
                TcpChannel channel = new TcpChannel(port);

                // Enregistrement du canal
                ChannelServices.RegisterChannel(channel, true);
                // Démarrage de l'écoute en exposant l'objet en Singleton
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteOperation), "RemoteOperation",
                    WellKnownObjectMode.Singleton);
                RemoteOperation.mainForm = this;

                //demarre le service
                //this.bckWrkStartService.RunWorkerAsync();
                _program = new Program();
                _program.DMain();
            }
            catch (Exception e)
            {
                this.rtbConsole.AppendText(e.ToString());
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //stoppe le service
            if (_program != null)
                _program.Stop();
            /*
             if (this.kinect_ok)
             {
                 this.WriteLine("Fermeture...\n");
                 System.Diagnostics.Process proc = new System.Diagnostics.Process();
                 proc = System.Diagnostics.Process.Start("C:\\Windows\\System32\\Net.exe", " STOP ServiceKinect");
                 proc.WaitForExit();
                 proc.Close();
             }
              * */
        }

        public void WriteLine(string line)
        {
            
            if (!this.bckWrkRtbText.IsBusy)
                this.bckWrkRtbText.RunWorkerAsync(line);
        }

        //modifie la richtextbox
        private void bckWrkRtbText_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;
        }

        private void bckWrkRtbText_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (this.rtbConsole)
            {
                this.rtbConsole.Invoke((MethodInvoker)delegate
                {
                    if (this.rtbConsole.Lines.Length > 499)
                        this.rtbConsole.Clear();
                    this.rtbConsole.AppendText(e.Result.ToString() + Environment.NewLine);
                    this.rtbConsole.Focus();
                    this.rtbConsole.ScrollToCaret();
                });
            }
        }

        //Demarre le service

        private void bckWrkStartService_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc = System.Diagnostics.Process.Start("C:\\Windows\\System32\\Net.exe", " START ServiceKinect");
            proc.WaitForExit();
            e.Result = proc.ExitCode;
            proc.Close();
        }
        private void bckWrkStartService_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((int)e.Result == 0)
            {
                // this.kinect_ok = true;
            }
            else
            {
                this.WriteLine("Une erreur est survenue... Code erreur : " + e.Result.ToString());
            }
        }



    }
}
