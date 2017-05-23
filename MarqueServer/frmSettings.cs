using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarquesasServer
{
    public partial class frmSettings : Form
    {
        private PluginAppSettings oPluginAppSettings = new PluginAppSettings();
        private GameObject oGameObject = new GameObject();

        public frmSettings()
        {
            InitializeComponent();
            SetHelpText();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            // Available IP Addresses
            foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            foreach (var ua in i.GetIPProperties().UnicastAddresses)
            {
                if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    //cmbIPAddress.Items.Add(ua.Address);
                }
            }
            
            txtPort.Text = oPluginAppSettings.GetInt("Port") == 0 ? "80" : oPluginAppSettings.GetInt("Port").ToString();
            txtSecurePort.Text = oPluginAppSettings.GetInt("SecurePort") == 0 ? "443" : oPluginAppSettings.GetInt("SecurePort").ToString();

            chkPortEnabled.Checked = oPluginAppSettings.GetBoolean("PortEnabled");
            chkSecurePortEnabled.Checked = oPluginAppSettings.GetBoolean("SecurePortEnabled");

            cmbAutomaticUpdates.SelectedItem = oPluginAppSettings.GetBoolean("AutomaticUpdates") ? "On" : "Off";
            
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
            }
            else
            {
                lblStatus.Text = "Stopped";
                lblStatus.ForeColor = Color.DarkRed;
                btnStartServer.Text = "Start Server";
            }
        }

        private void SetHelpText()
        {
            helpProvider1.SetHelpString(txtPort, oPluginAppSettings.GetString("Port_Help").Replace("  ", " "));
            helpProvider1.SetHelpString(chkPortEnabled, oPluginAppSettings.GetString("PortEnabled_Help").Replace("  ", " "));

            helpProvider1.SetHelpString(txtSecurePort, oPluginAppSettings.GetString("SecurePort_Help").Replace("  ", " "));
            helpProvider1.SetHelpString(chkSecurePortEnabled, oPluginAppSettings.GetString("SecurePortEnabled_Help").Replace("  ", " "));

            helpProvider1.SetHelpString(cmbAutomaticUpdates, oPluginAppSettings.GetString("AutomaticUpdates_Help").Replace("  ", " "));          
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            btnCheckForUpdates.Enabled = false;
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            oGameObject.Title = "SelectedGame";
            oGameObject.Marque = @"C:\OneDrive\Data\Source\MarqueServer\ScreenShots\WindowsSecurityAlert.PNG";

            if (MarquesasHttpServerInstance.RunningServer.IsRunning)
            {
                MarquesasHttpServerInstance.RunningServer.Stop();
            }
            else
            {
                MarquesasHttpServerInstance.RunningServer.oGameObject = oGameObject;
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

            oPluginAppSettings.Save();

            btnSave.Enabled = false;
            btnCheckForUpdates.Enabled = true;
        }

        private void StoreSettingsInAppStrings()
        {
            oPluginAppSettings.SetString("Port", txtPort.Text);
            oPluginAppSettings.SetString("PortEnabled", chkPortEnabled.Checked ? "True" : "False");

            oPluginAppSettings.SetString("SecurePort", txtSecurePort.Text);
            oPluginAppSettings.SetString("SecurePortEnabled", chkSecurePortEnabled.Checked ? "True" : "False");

            oPluginAppSettings.SetString("AutomaticUpdates", cmbAutomaticUpdates.SelectedItem.ToString() == "On" ? "True" : "False");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (btnSave.Enabled)
            {
                var oMessageBoxResults = MessageBox.Show("You have unsaved changes. Do you wish to save your changes?",
                    "Save Changes?", MessageBoxButtons.YesNoCancel);

                if (oMessageBoxResults == DialogResult.Cancel) return;

                if (oMessageBoxResults == DialogResult.Yes) btnSave_Click(sender, e);
                else oPluginAppSettings.ReloadConfiguration();
            }
            
            this.Close();
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            oPluginAppSettings.SetString("AutomaticUpdates", "False");

            new Update(oPluginAppSettings);
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
    }
}
