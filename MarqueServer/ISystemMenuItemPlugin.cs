using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins;

namespace MarqueServer
{
    public class MarqueServer_ISystemMenuItemPlugin : ISystemMenuItemPlugin
    {
        public void OnSelected()
        {
            frmSettings ofrmSettings = new frmSettings();
            ofrmSettings.Show();
        }

        public string Caption
        {
            get { return "Marquesas Server Settings"; }
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
