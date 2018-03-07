namespace RemotingClient.Formes
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.bckWrkRtbText = new System.ComponentModel.BackgroundWorker();
            this.bckWrkStartService = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // rtbConsole
            // 
            this.rtbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbConsole.BackColor = System.Drawing.SystemColors.WindowText;
            this.rtbConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbConsole.ForeColor = System.Drawing.Color.ForestGreen;
            this.rtbConsole.Location = new System.Drawing.Point(12, 12);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.Size = new System.Drawing.Size(318, 286);
            this.rtbConsole.TabIndex = 0;
            this.rtbConsole.Text = "";
            // 
            // bckWrkRtbText
            // 
            this.bckWrkRtbText.WorkerSupportsCancellation = true;
            this.bckWrkRtbText.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bckWrkRtbText_DoWork);
            this.bckWrkRtbText.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bckWrkRtbText_RunWorkerCompleted);
            // 
            // bckWrkStartService
            // 
            this.bckWrkStartService.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bckWrkStartService_DoWork);
            this.bckWrkStartService.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bckWrkStartService_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 310);
            this.Controls.Add(this.rtbConsole);
            this.Name = "MainForm";
            this.Text = "KinectOooo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbConsole;
        private System.ComponentModel.BackgroundWorker bckWrkRtbText;
        private System.ComponentModel.BackgroundWorker bckWrkStartService;
    }
}