using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonPluginHelper;

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
            cmbAutomaticUpdates.SelectedText = PluginAppSettings.GetBoolean("AutomaticUpdates") ? "On" : "Off";
            
            SetServerStatus();

            btnSave.Enabled = false;
            btnCheckForUpdates.Enabled = true;
        }

        private void SetServerStatus()
        {
            if (MarquesasHttpServerInstance.RunningServer.IsRunning)
            {
                lblStatus.Text = "Running";
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
            helpProvider1.SetHelpString(txtPort, PluginAppSettings.GetString("Port_Help").Replace("  ", " "));
            helpProvider1.SetHelpString(chkPortEnabled, PluginAppSettings.GetString("PortEnabled_Help").Replace("  ", " "));

            helpProvider1.SetHelpString(txtSecurePort, PluginAppSettings.GetString("SecurePort_Help").Replace("  ", " "));
            helpProvider1.SetHelpString(chkSecurePortEnabled, PluginAppSettings.GetString("SecurePortEnabled_Help").Replace("  ", " "));
            helpProvider1.SetHelpString(nudSecondsBetweenRefresh, PluginAppSettings.GetString("SecondsBetweenRefresh_Help").Replace("  ", " "));
            helpProvider1.SetHelpString(cmbAutomaticUpdates, PluginAppSettings.GetString("AutomaticUpdates_Help").Replace("  ", " "));          
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            btnCheckForUpdates.Enabled = false;
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if (MarquesasHttpServerInstance.RunningServer.IsRunning)
            {
                MarquesasHttpServerInstance.RunningServer.Stop();
            }
            else
            {
                MarquesasHttpServerInstance.RunningServer.port = chkPortEnabled.Checked ? Convert.ToInt16(txtPort.Text) : -1;
                MarquesasHttpServerInstance.RunningServer.secure_port = chkSecurePortEnabled.Checked ? Convert.ToInt16(txtSecurePort.Text) : -1;
                MarquesasHttpServerInstance.RunningServer.Initialize();
                MarquesasHttpServerInstance.RunningServer.Start();
            }
            
            SetServerStatus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StoreSettingsInAppStrings();

            PluginAppSettings.Save();

            btnSave.Enabled = false;
            btnCheckForUpdates.Enabled = true;
        }

        private void StoreSettingsInAppStrings()
        {
            PluginAppSettings.SetString("Port", txtPort.Text);
            PluginAppSettings.SetString("PortEnabled", chkPortEnabled.Checked ? "True" : "False");

            PluginAppSettings.SetString("SecurePort", txtSecurePort.Text);
            PluginAppSettings.SetString("SecurePortEnabled", chkSecurePortEnabled.Checked ? "True" : "False");
            PluginAppSettings.SetString("SecondsBetweenRefresh", nudSecondsBetweenRefresh.Value.ToString());
            PluginAppSettings.SetString("AutomaticUpdates", cmbAutomaticUpdates.SelectedItem.ToString() == "On" ? "True" : "False");
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

        private void lblLaunchPort_Click(object sender, EventArgs e)
        {
            Process.Start("http://" + string.Join(".", MarquesasHttpServerInstance.RunningServer.localIPv4Addresses[0]) + (MarquesasHttpServerInstance.RunningServer.port != 80 ? ":" + MarquesasHttpServerInstance.RunningServer.port : ""));
        }

        private void lblLaunchSecurePort_Click(object sender, EventArgs e)
        {
            Process.Start("https://" + string.Join(".", MarquesasHttpServerInstance.RunningServer.localIPv4Addresses[0]) + (MarquesasHttpServerInstance.RunningServer.secure_port != 443 ? ":" + MarquesasHttpServerInstance.RunningServer.port : ""));
        }
    }
}
