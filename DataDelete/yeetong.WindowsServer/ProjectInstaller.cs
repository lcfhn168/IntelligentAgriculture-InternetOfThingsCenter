using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using yeetong_WindowsServer;
using System.Windows.Forms;


namespace yeetong_WindowsServer
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            using (SettingHelper settingHelper = new SettingHelper())
            {
                serviceInstaller1.ServiceName = settingHelper.ServiceName;
                serviceInstaller1.DisplayName = settingHelper.DisplayName;
                serviceInstaller1.Description = settingHelper.Description;
            }
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
