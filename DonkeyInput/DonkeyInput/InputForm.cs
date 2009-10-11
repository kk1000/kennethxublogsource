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
        private ServerOption _option;
        private OptionStore _registrStore;
        private ICollection<string> _links;

        public InputForm()
        {
            InitializeComponent();
        }

        public InputForm(ICollection<string> links) : this()
        {
            _links = links;
        }

        private void HandleButtonCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void HandleButtonOkClick(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            var req = ViewToModel();
            ServerOption serverOption = req.Option;
            if (!serverOption.Equals(_option))
            {
                _registrStore.Value = serverOption;
                _option = serverOption;
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
                textBoxPort.Text = ServerOption.DefaultPort.ToString();
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
            textBoxLinks.Text = JoinLinksToString(_links, string.Empty);
        }

        private DownloadRequest ViewToModel()
        {
            var option = new ServerOption
            {

                UserName = textBoxUserName.Text,
                Password = textBoxPassword.Text,
                Server = textBoxServer.Text,
                Port = int.Parse(textBoxPort.Text),
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
            textBoxLinks.Text = JoinLinksToString(links, textBoxLinks.Text);
        }

        private static string JoinLinksToString(ICollection<string> links, string initialString)
        {
            if (links == null || links.Count == 0) return initialString;
            StringBuilder sb = new StringBuilder(initialString ?? string.Empty);
            if (!string.IsNullOrEmpty(initialString) &&
                !initialString.EndsWith(Environment.NewLine)) sb.Append(Environment.NewLine);
            foreach (var link in links)
            {
                sb.Append(link).Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
