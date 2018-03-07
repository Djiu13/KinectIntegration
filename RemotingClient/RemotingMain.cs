
using System;
using RemotingClient.Formes;


namespace RemotingClient
{
    class RemotingMain
    {
        [MTAThread]
        static void Main(string[] args)
        {
            MainForm form = new MainForm();
            form.ShowDialog();

        }
    }
}
