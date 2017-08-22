using System.Drawing;
using System.Windows.Forms;
using Unbroken.LaunchBox.Plugins;

namespace MarquesasServer
{
    public class MarquesasServer_ISystemMenuItemPlugin : ISystemMenuItemPlugin
    {
        public void OnSelected()
        {
            frmSettings ofrmSettings = new frmSettings();
            ofrmSettings.Show();
        }

        public string Caption
        {
            get { return "Marquesas Server Admin"; }
        }

        public Image IconImage
        {
            get { return Properties.Resources.icon_original; }
        }

        public bool ShowInLaunchBox
        {
            get { return true; }
        }

        public bool ShowInBigBox
        {
            get { return false; }
        }

        public bool AllowInBigBoxWhenLocked
        {
            get { return false; }
        }
    }
}