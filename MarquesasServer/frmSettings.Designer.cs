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
            this.components = new System.ComponentModel.Container();
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
            this.txtSecurePort = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkPortEnabled = new System.Windows.Forms.CheckBox();
            this.chkSecurePortEnabled = new System.Windows.Forms.CheckBox();
            this.lblLaunchPort = new System.Windows.Forms.Label();
            this.lblLaunchSecurePort = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nudSecondsBetweenRefresh = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.chkWriteEnabled = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudSecondsBetweenRefresh)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartServer
            // 
            this.btnStartServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStartServer.Location = new System.Drawing.Point(12, 240);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(75, 23);
            this.btnStartServer.TabIndex = 12;
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(206, 240);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(125, 240);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "HTTP Port:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(145, 63);
            this.txtPort.Mask = "00000";
            this.txtPort.Name = "txtPort";
            this.txtPort.PromptChar = ' ';
            this.txtPort.Size = new System.Drawing.Size(43, 20);
            this.txtPort.TabIndex = 1;
            this.txtPort.ValidatingType = typeof(int);
            this.txtPort.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.SystemColors.Info;
            this.lblStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(3, 11);
            this.lblStatus.MinimumSize = new System.Drawing.Size(286, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(286, 37);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Running";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 186);
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
            this.cmbAutomaticUpdates.Location = new System.Drawing.Point(145, 184);
            this.cmbAutomaticUpdates.Name = "cmbAutomaticUpdates";
            this.cmbAutomaticUpdates.Size = new System.Drawing.Size(43, 21);
            this.cmbAutomaticUpdates.TabIndex = 10;
            this.cmbAutomaticUpdates.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Location = new System.Drawing.Point(194, 183);
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(75, 24);
            this.btnCheckForUpdates.TabIndex = 11;
            this.btnCheckForUpdates.Text = "Update...";
            this.btnCheckForUpdates.UseVisualStyleBackColor = true;
            this.btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
            // 
            // txtSecurePort
            // 
            this.txtSecurePort.Location = new System.Drawing.Point(145, 91);
            this.txtSecurePort.Mask = "00000";
            this.txtSecurePort.Name = "txtSecurePort";
            this.txtSecurePort.PromptChar = ' ';
            this.txtSecurePort.Size = new System.Drawing.Size(43, 20);
            this.txtSecurePort.TabIndex = 4;
            this.txtSecurePort.ValidatingType = typeof(int);
            this.txtSecurePort.TextChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 18);
            this.label2.TabIndex = 14;
            this.label2.Text = "Secure HTTPS Port:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // chkPortEnabled
            // 
            this.chkPortEnabled.AutoSize = true;
            this.chkPortEnabled.Location = new System.Drawing.Point(194, 65);
            this.chkPortEnabled.Name = "chkPortEnabled";
            this.chkPortEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkPortEnabled.TabIndex = 2;
            this.chkPortEnabled.Text = "Enabled";
            this.chkPortEnabled.UseVisualStyleBackColor = true;
            this.chkPortEnabled.CheckedChanged += new System.EventHandler(this.chkPortEnabled_CheckedChanged);
            // 
            // chkSecurePortEnabled
            // 
            this.chkSecurePortEnabled.AutoSize = true;
            this.chkSecurePortEnabled.Location = new System.Drawing.Point(194, 93);
            this.chkSecurePortEnabled.Name = "chkSecurePortEnabled";
            this.chkSecurePortEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkSecurePortEnabled.TabIndex = 5;
            this.chkSecurePortEnabled.Text = "Enabled";
            this.chkSecurePortEnabled.UseVisualStyleBackColor = true;
            this.chkSecurePortEnabled.CheckedChanged += new System.EventHandler(this.chkSecurePortEnabled_CheckedChanged);
            // 
            // lblLaunchPort
            // 
            this.lblLaunchPort.AutoSize = true;
            this.lblLaunchPort.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblLaunchPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLaunchPort.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblLaunchPort.Location = new System.Drawing.Point(259, 66);
            this.lblLaunchPort.Name = "lblLaunchPort";
            this.lblLaunchPort.Size = new System.Drawing.Size(18, 15);
            this.lblLaunchPort.TabIndex = 3;
            this.lblLaunchPort.Text = "...";
            this.lblLaunchPort.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblLaunchPort.Click += new System.EventHandler(this.lblLaunchPort_Click);
            // 
            // lblLaunchSecurePort
            // 
            this.lblLaunchSecurePort.AutoSize = true;
            this.lblLaunchSecurePort.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblLaunchSecurePort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLaunchSecurePort.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblLaunchSecurePort.Location = new System.Drawing.Point(259, 94);
            this.lblLaunchSecurePort.Name = "lblLaunchSecurePort";
            this.lblLaunchSecurePort.Size = new System.Drawing.Size(18, 15);
            this.lblLaunchSecurePort.TabIndex = 6;
            this.lblLaunchSecurePort.Text = "...";
            this.lblLaunchSecurePort.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblLaunchSecurePort.Click += new System.EventHandler(this.lblLaunchSecurePort_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(5, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 20);
            this.label4.TabIndex = 20;
            this.label4.Text = "Seconds Between Refresh:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // nudSecondsBetweenRefresh
            // 
            this.nudSecondsBetweenRefresh.Location = new System.Drawing.Point(145, 117);
            this.nudSecondsBetweenRefresh.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudSecondsBetweenRefresh.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSecondsBetweenRefresh.Name = "nudSecondsBetweenRefresh";
            this.nudSecondsBetweenRefresh.Size = new System.Drawing.Size(43, 20);
            this.nudSecondsBetweenRefresh.TabIndex = 7;
            this.nudSecondsBetweenRefresh.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudSecondsBetweenRefresh.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 400;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(98, 211);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Version:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(142, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 13);
            this.label6.TabIndex = 23;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(142, 211);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(13, 13);
            this.lblVersion.TabIndex = 24;
            this.lblVersion.Text = "0";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(13, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(129, 19);
            this.label7.TabIndex = 25;
            this.label7.Text = "Write Enabled:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // chkWriteEnabled
            // 
            this.chkWriteEnabled.AutoSize = true;
            this.chkWriteEnabled.Location = new System.Drawing.Point(145, 144);
            this.chkWriteEnabled.Name = "chkWriteEnabled";
            this.chkWriteEnabled.Size = new System.Drawing.Size(15, 14);
            this.chkWriteEnabled.TabIndex = 8;
            this.chkWriteEnabled.UseVisualStyleBackColor = true;
            this.chkWriteEnabled.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(293, 275);
            this.Controls.Add(this.chkWriteEnabled);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudSecondsBetweenRefresh);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblLaunchSecurePort);
            this.Controls.Add(this.lblLaunchPort);
            this.Controls.Add(this.chkSecurePortEnabled);
            this.Controls.Add(this.chkPortEnabled);
            this.Controls.Add(this.txtSecurePort);
            this.Controls.Add(this.label2);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.Text = "Marquesas Server Administrator";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudSecondsBetweenRefresh)).EndInit();
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
        private System.Windows.Forms.MaskedTextBox txtSecurePort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkPortEnabled;
        private System.Windows.Forms.CheckBox chkSecurePortEnabled;
        private System.Windows.Forms.Label lblLaunchPort;
        private System.Windows.Forms.Label lblLaunchSecurePort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudSecondsBetweenRefresh;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkWriteEnabled;
    }
}