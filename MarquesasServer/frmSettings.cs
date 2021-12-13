using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommonPluginHelper;
using MarquesasServer.Properties;
using System.Reflection;
//using SuperSocket.SocketBase;

namespace MarquesasServer
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();

            SetHelpText();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            txtPort.Text = PluginAppSettings.GetInt("Port") == 0 ? "80" : PluginAppSettings.GetInt("Port").ToString();
            txtSecurePort.Text = PluginAppSettings.GetInt("SecurePort") == 0 ? "443" : PluginAppSettings.GetInt("SecurePort").ToString();

            chkPortEnabled.Checked = PluginAppSettings.GetBoolean("PortEnabled");
            chkSecurePortEnabled.Checked = PluginAppSettings.GetBoolean("SecurePortEnabled");
            nudSecondsBetweenRefresh.Value = (PluginAppSettings.GetInt("SecondsBetweenRefresh") > 0 && PluginAppSettings.GetInt("SecondsBetweenRefresh") < 1000) ? PluginAppSettings.GetInt("SecondsBetweenRefresh") : 15;
            cmbAutomaticUpdates.SelectedIndex = cmbAutomaticUpdates.FindString(PluginAppSettings.GetBoolean("AutomaticUpdates") ? "On" : "Off");
            chkWriteEnabled.Checked = PluginAppSettings.GetBoolean("WriteEnabled");

            SetServerStatus();

            btnSave.Enabled = false;
            btnCheckForUpdates.Enabled = true;

            lblVersion.Text = PluginAppSettings.GetString("Version");
            
            // The version information in the config file overrides the assembly version.
            if (string.IsNullOrEmpty(lblVersion.Text)) lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
        }

        private void SetServerStatus()
        {
            if (MarquesasHttpServerInstance.RunningServer.IsRunning.Item1 || MarquesasHttpServerInstance.RunningServer.IsRunning.Item2)
            {
                lblStatus.Text = Resources.ServerStatus_Running;
                lblStatus.ForeColor = Color.DarkGreen;
                btnStartServer.Text = "Stop Server";
                lblLaunchPort.Enabled = chkPortEnabled.Checked;
                lblLaunchSecurePort.Enabled = chkSecurePortEnabled.Checked;
            }
            else
            {
                lblStatus.Text = "Stopped";
                lblStatus.ForeColor = Color.DarkRed;
                btnStartServer.Text = "Start Server";
                lblLaunchPort.Enabled = false;
                lblLaunchSecurePort.Enabled = false;
            }

            lblLaunchPort.Visible = lblLaunchPort.Enabled;
            lblLaunchSecurePort.Visible = lblLaunchSecurePort.Enabled;
        }

        private void SetHelpText()
        {
            toolTip1.SetToolTip(txtPort, AddNewLinesForTooltip(PluginAppSettings.GetString("Port_Help").Replace("  ", " ")));
            toolTip1.SetToolTip(chkPortEnabled, AddNewLinesForTooltip(PluginAppSettings.GetString("PortEnabled_Help").Replace("  ", " ")));

            toolTip1.SetToolTip(txtSecurePort, AddNewLinesForTooltip(PluginAppSettings.GetString("SecurePort_Help").Replace("  ", " ")));
            toolTip1.SetToolTip(chkSecurePortEnabled, AddNewLinesForTooltip(PluginAppSettings.GetString("SecurePortEnabled_Help").Replace("  ", " ")));
            toolTip1.SetToolTip(nudSecondsBetweenRefresh, AddNewLinesForTooltip(PluginAppSettings.GetString("SecondsBetweenRefresh_Help").Replace("  ", " ")));
            toolTip1.SetToolTip(cmbAutomaticUpdates, AddNewLinesForTooltip(PluginAppSettings.GetString("AutomaticUpdates_Help").Replace("  ", " ")));
            toolTip1.SetToolTip(chkWriteEnabled, AddNewLinesForTooltip(PluginAppSettings.GetString("WriteEnabled_Help").Replace("  ", " ")));

            toolTip1.SetToolTip(lblLaunchPort, AddNewLinesForTooltip("Browse index."));
            toolTip1.SetToolTip(lblLaunchSecurePort, AddNewLinesForTooltip("Browse index."));
        }

        private static string AddNewLinesForTooltip(string text)
        {
            if (text.Length < 20)
                return text;
            int lineLength = (int)Math.Sqrt((double)text.Length) * 2;
            StringBuilder sb = new StringBuilder();
            int currentLinePosition = 0;
            for (int textIndex = 0; textIndex < text.Length; textIndex++)
            {
                // If we have reached the target line length and the next
                // character is whitespace then begin a new line.
                if (currentLinePosition >= lineLength &&
                    char.IsWhiteSpace(text[textIndex]))
                {
                    sb.Append(Environment.NewLine);
                    currentLinePosition = 0;
                }
                // If we have just started a new line, skip all the whitespace.
                if (currentLinePosition == 0)
                    while (textIndex < text.Length && char.IsWhiteSpace(text[textIndex]))
                        textIndex++;
                // Append the next character.
                if (textIndex < text.Length)
                    sb.Append(text[textIndex]);

                currentLinePosition++;
            }
            return sb.ToString();
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            btnCheckForUpdates.Enabled = false;
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if (MarquesasHttpServerInstance.RunningServer.IsRunning.Item1 || MarquesasHttpServerInstance.RunningServer.IsRunning.Item2)
            {
                MarquesasHttpServerInstance.RunningServer.Stop();
            }
            else
            {
                StartHttpServices();
            }

            //if (SuperSocketAppServerInstance.RunningServer.State == ServerState.Running)
            //{
            //    SuperSocketAppServerInstance.RunningServer.Stop();
            //}
            //else
            //{
            //    StartSuperSockets();
            //}

            SetServerStatus();
        }

        public void StartHttpServices()
        {
            MarquesasHttpServerInstance.RunningServer.port = chkPortEnabled.Checked ? Convert.ToInt16(txtPort.Text) : -1;
            MarquesasHttpServerInstance.RunningServer.secure_port = chkSecurePortEnabled.Checked ? Convert.ToInt16(txtSecurePort.Text) : -1;
            MarquesasHttpServerInstance.RunningServer.Initialize();
            MarquesasHttpServerInstance.RunningServer.WarnIfPortsAreNotAvailable(true);
            MarquesasHttpServerInstance.RunningServer.Start();
        }

        //public void StartSuperSockets()
        //{
        //    //Setup the appServer
        //    if (!SuperSocketAppServerInstance.RunningServer.Setup(PluginAppSettings.GetInt("SuperSocketPort"))) //Setup with listening port
        //    {
        //        // Failed to setup!
        //        MessageBox.Show("Failed setup for Socket Server.", Resources.ApplicationName, MessageBoxButtons.OK);
        //        return;
        //    }

        //    //Try to start the appServer
        //    if (!SuperSocketAppServerInstance.RunningServer.Start())
        //    {
        //        // Failed to start!
        //        MessageBox.Show("Failed to start Socket Server on port " + PluginAppSettings.GetInt("SuperSocketPort") + ".", Resources.ApplicationName, MessageBoxButtons.OK);
        //        return;
        //    }
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateSettings())
            {
                StoreSettingsInAppStrings();

                PluginAppSettings.Save();

                btnSave.Enabled = false;
                btnCheckForUpdates.Enabled = true;
            }
        }

        private Boolean ValidateSettings()
        {
            Boolean bValid = true;
            string sErrorMessage = string.Empty;

            if (chkPortEnabled.Checked && chkSecurePortEnabled.Checked && txtPort.Text == txtSecurePort.Text)
            {
                bValid = false;
                sErrorMessage = "Secure HTTPS Port and HTTP Port cannot be the same.";
            }

            if (!bValid)
            {
                MessageBox.Show(sErrorMessage, "Settings could not be saved", MessageBoxButtons.OK);
            }

            return bValid;
        }

        private void StoreSettingsInAppStrings()
        {
            PluginAppSettings.SetString("Port", txtPort.Text);
            PluginAppSettings.SetString("PortEnabled", chkPortEnabled.Checked ? "True" : "False");

            PluginAppSettings.SetString("SecurePort", txtSecurePort.Text);
            PluginAppSettings.SetString("SecurePortEnabled", chkSecurePortEnabled.Checked ? "True" : "False");
            PluginAppSettings.SetString("SecondsBetweenRefresh", nudSecondsBetweenRefresh.Value.ToString());
            PluginAppSettings.SetString("AutomaticUpdates", cmbAutomaticUpdates.SelectedItem.ToString() == "On" ? "True" : "False");
            PluginAppSettings.SetString("WriteEnabled", chkWriteEnabled.Checked ? "True" : "False");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (btnSave.Enabled)
            {
                var oMessageBoxResults = MessageBox.Show("You have unsaved changes. Do you wish to save your changes?",
                    "Save Changes?", MessageBoxButtons.YesNoCancel);

                if (oMessageBoxResults == DialogResult.Cancel) return;

                if (oMessageBoxResults == DialogResult.Yes) btnSave_Click(sender, e);
                else PluginAppSettings.ReloadConfiguration();
            }

            this.Close();
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            PluginAppSettings.SetString("AutomaticUpdates", "False");

            PluginUpdate.Check();
        }

        private void chkPortEnabled_CheckedChanged(object sender, EventArgs e)
        {
            txtPort.Enabled = chkPortEnabled.Checked;
            ValueChanged(sender, e);
        }

        private void chkSecurePortEnabled_CheckedChanged(object sender, EventArgs e)
        {
            txtSecurePort.Enabled = chkSecurePortEnabled.Checked;
            ValueChanged(sender, e);
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {
            MarquesasHttpServerInstance.RunningServer.LaunchIndexPage();
        }

        private void lblLaunchPort_Click(object sender, EventArgs e)
        {
            MarquesasHttpServerInstance.RunningServer.LaunchIndexPage();
        }

        private void lblLaunchSecurePort_Click(object sender, EventArgs e)
        {
            MarquesasHttpServerInstance.RunningServer.LaunchIndexPage(true);
        }
    }
}