namespace MarquesasServer
{
    partial class frmSettings
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
            oPluginAppSettings.Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.MaskedTextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbAutomaticUpdates = new System.Windows.Forms.ComboBox();
            this.btnCheckForUpdates = new System.Windows.Forms.Button();
            this.linkLabelExampleLink = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.txtSecurePort = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkPortEnabled = new System.Windows.Forms.CheckBox();
            this.chkSecurePortEnabled = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnStartServer
            // 
            this.btnStartServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStartServer.Location = new System.Drawing.Point(12, 191);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(75, 23);
            this.btnStartServer.TabIndex = 0;
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(205, 191);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(124, 191);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(128, 63);
            this.txtPort.Mask = "00000";
            this.txtPort.Name = "txtPort";
            this.txtPort.PromptChar = ' ';
            this.txtPort.Size = new System.Drawing.Size(43, 20);
            this.txtPort.TabIndex = 4;
            this.txtPort.ValidatingType = typeof(int);
            this.txtPort.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.SystemColors.Info;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(3, 11);
            this.lblStatus.MinimumSize = new System.Drawing.Size(286, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(286, 37);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Running";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Automatic Updates:";
            // 
            // cmbAutomaticUpdates
            // 
            this.cmbAutomaticUpdates.FormattingEnabled = true;
            this.cmbAutomaticUpdates.Items.AddRange(new object[] {
            "On",
            "Off"});
            this.cmbAutomaticUpdates.Location = new System.Drawing.Point(128, 120);
            this.cmbAutomaticUpdates.Name = "cmbAutomaticUpdates";
            this.cmbAutomaticUpdates.Size = new System.Drawing.Size(43, 21);
            this.cmbAutomaticUpdates.TabIndex = 10;
            this.cmbAutomaticUpdates.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCheckForUpdates.Location = new System.Drawing.Point(177, 119);
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(75, 23);
            this.btnCheckForUpdates.TabIndex = 11;
            this.btnCheckForUpdates.Text = "Update...";
            this.btnCheckForUpdates.UseVisualStyleBackColor = true;
            this.btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
            // 
            // linkLabelExampleLink
            // 
            this.linkLabelExampleLink.AutoSize = true;
            this.linkLabelExampleLink.Location = new System.Drawing.Point(128, 153);
            this.linkLabelExampleLink.Name = "linkLabelExampleLink";
            this.linkLabelExampleLink.Size = new System.Drawing.Size(55, 13);
            this.linkLabelExampleLink.TabIndex = 12;
            this.linkLabelExampleLink.TabStop = true;
            this.linkLabelExampleLink.Text = "linkLabel1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(55, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Marque Link:";
            // 
            // txtSecurePort
            // 
            this.txtSecurePort.Location = new System.Drawing.Point(128, 91);
            this.txtSecurePort.Mask = "00000";
            this.txtSecurePort.Name = "txtSecurePort";
            this.txtSecurePort.PromptChar = ' ';
            this.txtSecurePort.Size = new System.Drawing.Size(43, 20);
            this.txtSecurePort.TabIndex = 15;
            this.txtSecurePort.ValidatingType = typeof(int);
            this.txtSecurePort.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Secure Port:";
            // 
            // chkPortEnabled
            // 
            this.chkPortEnabled.AutoSize = true;
            this.chkPortEnabled.Location = new System.Drawing.Point(187, 65);
            this.chkPortEnabled.Name = "chkPortEnabled";
            this.chkPortEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkPortEnabled.TabIndex = 16;
            this.chkPortEnabled.Text = "Enabled";
            this.chkPortEnabled.UseVisualStyleBackColor = true;
            this.chkPortEnabled.CheckedChanged += new System.EventHandler(this.chkPortEnabled_CheckedChanged);
            // 
            // chkSecurePortEnabled
            // 
            this.chkSecurePortEnabled.AutoSize = true;
            this.chkSecurePortEnabled.Location = new System.Drawing.Point(187, 93);
            this.chkSecurePortEnabled.Name = "chkSecurePortEnabled";
            this.chkSecurePortEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkSecurePortEnabled.TabIndex = 17;
            this.chkSecurePortEnabled.Text = "Enabled";
            this.chkSecurePortEnabled.UseVisualStyleBackColor = true;
            this.chkSecurePortEnabled.CheckedChanged += new System.EventHandler(this.chkSecurePortEnabled_CheckedChanged);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 226);
            this.Controls.Add(this.chkSecurePortEnabled);
            this.Controls.Add(this.chkPortEnabled);
            this.Controls.Add(this.txtSecurePort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linkLabelExampleLink);
            this.Controls.Add(this.btnCheckForUpdates);
            this.Controls.Add(this.cmbAutomaticUpdates);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnStartServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.Text = "Marquesas Server Administrator";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox txtPort;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbAutomaticUpdates;
        private System.Windows.Forms.Button btnCheckForUpdates;
        private System.Windows.Forms.LinkLabel linkLabelExampleLink;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.MaskedTextBox txtSecurePort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkPortEnabled;
        private System.Windows.Forms.CheckBox chkSecurePortEnabled;
    }
}