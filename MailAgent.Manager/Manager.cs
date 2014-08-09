using MailAgent.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailAgent.Manager
{
    public partial class Manager : Form
    {
        private ServiceController _serviceController;
        private BackgroundWorker _backgroundWorker;
        private Settings _settings;

        public Manager()
        {
            InitializeComponent();

            _serviceController = new ServiceController("Mail Agent");
            _backgroundWorker = new BackgroundWorker();
            _settings = new Settings();

            GetServiceInfo();
        }

        private void Manager_Load(object sender, EventArgs e)
        {
            _settings.Parse();

            profileList.Items.AddRange(_settings.Profiles.ToArray());
        }

        private void GetServiceInfo()
        {
            try
            {
                if (_serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    serviceStartStopButton.Enabled = true;
                    serviceStartStopButton.Text = "Start Service";
                }
                else if (_serviceController.Status == ServiceControllerStatus.Running)
                {
                    serviceStartStopButton.Enabled = true;
                    serviceStartStopButton.Text = "Stop Service";
                }
            }
            catch (Exception)
            {
                serviceRestartButton.Enabled = false;
                serviceRestartButton.Text = "Unknown Status";
                serviceStartStopButton.Enabled = false;
                serviceStartStopButton.Text = "Unknown Status";
            }
        }

        private void serviceButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;

            if (button.Name == "serviceRestartButton")
            {
                _serviceController.Stop();
                _serviceController.Start();
            }
            else if (button.Name == "serviceStartStopButton")
            {
                if (_serviceController.Status == ServiceControllerStatus.Running && button.Text.StartsWith("Stop"))
                {
                    _serviceController.Stop();
                }
                else if (_serviceController.Status == ServiceControllerStatus.Stopped && button.Text.StartsWith("Start"))
                {
                    _serviceController.Start();
                }

                _serviceController.Refresh();

                GetServiceInfo();
            }
        }

        private void profileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Profile profile = profileList.SelectedItem as Profile;

            profileID.Text = profile.Id;
            profileName.Text = profile.Name;
            profilePath.Text = profile.SavePath;
            profileAlias.Text = profile.Alias;
            profileSubjectMatch.Text = profile.EmailSubject;
            profileBodyMatch.Text = profile.EmailBody;

            profileSaveBody.Checked = profile.SaveEmailBody;
            profileSaveAttachments.Checked = profile.SaveAttachments;
            profileIsActive.Checked = profile.Active;
            profileDefaultPath.Checked = profile.IsDefaultPath;

            profileKeys.Items.Clear();
            profileKeys.Items.AddRange(profile.Keys.Select(x => x.Name).ToArray());
        }

        private void profileKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            string value = (string) profileKeys.SelectedItem;

            Key profileKey = (profileList.SelectedItem as Profile).Keys.Where(x => x.Name == value).SingleOrDefault();

            keyName.Text = profileKey.Name;
            keyValue.Text = profileKey.Value;
            keyType.SelectedIndex = keyType.Items.IndexOf(profileKey.Type);
        }
    }
}
