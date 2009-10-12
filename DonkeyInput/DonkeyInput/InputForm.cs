#region License
/*
* Copyright (C) 2002-2009 the original author or authors.
* 
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
*      http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#endregion
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System;

namespace DonkeyInput
{
    /// <summary>
    /// The main form of the application.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public partial class InputForm : Form
    {
        private ConfigOption _option;
        private OptionStore _registrStore;
        private readonly List<string> _links = new List<string>();

        public InputForm()
        {
            InitializeComponent();
        }

        public InputForm(IEnumerable<string> links) : this()
        {
            _links.AddRange(links);
        }

        private void HandleButtonCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void HandleButtonOkClick(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            var req = ViewToModel();
            ConfigOption configOption = req.Option;
            if (!configOption.Equals(_option))
            {
                _registrStore.Value = configOption;
                _option = configOption;
            }
            try
            {
                req.Send();
                Close();
            }
            catch(WebException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(textBoxServer.Text)) return false;
            int port;
            if (!int.TryParse(textBoxPort.Text, out port))
            {
                textBoxPort.Text = ConfigOption.DefaultPort.ToString();
            }
            return true;
        }

        private void HandleInputFormLoad(object sender, EventArgs e)
        {
            _registrStore = new OptionStore();
            _registrStore.RegisterProtocolHandler();
            _option = _registrStore.Value;
            ModelToView();
        }

        private void ModelToView()
        {
            textBoxServer.Text = _option.Server;
            textBoxPort.Text = _option.Port.ToString();
            textBoxUserName.Text = _option.UserName;
            textBoxPassword.Text = _option.Password;
            checkBoxFixIE.Checked = _option.FixIE;
            checkBoxSavePassword.Checked = _option.SavePassword;
            ComputeTextForLinks();
        }

        private DownloadRequest ViewToModel()
        {
            var option = new ConfigOption
            {

                UserName = textBoxUserName.Text,
                Password = textBoxPassword.Text,
                Server = textBoxServer.Text,
                Port = int.Parse(textBoxPort.Text),
                SavePassword = checkBoxSavePassword.Checked,
                FixIE = checkBoxFixIE.Checked,
            };
            var req = new DownloadRequest
            {
                Option = option,
                Links = DownloadRequest.ParseLinks(textBoxLinks.Text)
            };
            return req;
        }

        private void HandleInputFormShown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLinks.Text))
                textBoxLinks.Focus();
            else
                textBoxPassword.Focus();
        }

        public void AddLink(ICollection<string> links)
        {
            _links.AddRange(links);
            ComputeTextForLinks();
        }

        private static string JoinLinksToString(ICollection<string> links, bool fixIE)
        {
            if (links == null || links.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var link in links)
            {
                if(fixIE)
                {
                    foreach (char c in link)
                    {
                        int i = c & 0xFF;
                        if (i < 128) sb.Append(c);
                        else sb.Append('%').Append(String.Format("{0:X2}", i));
                    }
                }
                else
                {
                    sb.Append(link);
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private void HandleCheckBoxIEFixCheckedChanged(object sender, EventArgs e)
        {
            _option.FixIE = checkBoxFixIE.Checked;
            ComputeTextForLinks();
        }

        private void ComputeTextForLinks()
        {
            textBoxLinks.Text = JoinLinksToString(_links, _option.FixIE);
        }
    }
}
