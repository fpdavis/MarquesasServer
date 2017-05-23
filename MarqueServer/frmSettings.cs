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
        private List<MarquesasHttpServer> oaMarquesasHttpServers = new List<MarquesasHttpServer>();
        private GameObject oGameObject = new GameObject();

        public frmSettings()
        {
            InitializeComponent();
            SetHelpText();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            cmbIPAddress.Items.Add("All");

            // Available IP Addresses
            foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            foreach (var ua in i.GetIPProperties().UnicastAddresses)
            {
                if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    cmbIPAddress.Items.Add(ua.Address);
                }
            }

            cmbIPAddress.SelectedItem = string.IsNullOrWhiteSpace(oPluginAppSettings.GetString("IPAddress")) ? "All" : oPluginAppSettings.GetString("IPAddress");

            txtPort.Text = oPluginAppSettings.GetInt("Port") == 0 ? "80" : oPluginAppSettings.GetInt("Port").ToString();
            cmbAutomaticUpdates.SelectedItem = oPluginAppSettings.GetBoolean("AutomaticUpdates") ? "On" : "Off";

            btnSave.Enabled = false;
            btnCheckForUpdates.Enabled = true;
        }

        private void SetHelpText()
        {

            helpProvider1.SetHelpString(cmbIPAddress, oPluginAppSettings.GetString("IPAddress_Help").Replace("  ", " "));
            helpProvider1.SetHelpString(txtPort, oPluginAppSettings.GetString("Port_Help").Replace("  ", " "));
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

            new Spinup(oaMarquesasHttpServers, oPluginAppSettings, oGameObject);
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
            oPluginAppSettings.SetString("IPAddress", cmbIPAddress.SelectedItem.ToString() == "All" ? "" : cmbIPAddress.SelectedItem.ToString());
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
    }
}
