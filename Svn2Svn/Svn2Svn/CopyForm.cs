using System;
#region License

/*
 * Copyright (C) 2012 the original author or authors.
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
using System.Threading;
using System.Windows.Forms;

namespace Svn2Svn
{
    /// <summary>
    /// The main form
    /// </summary>
    /// <author>Kenneth Xu</author>
    public partial class CopyForm : Form
    {
        private TextBoxLogger _logger;
        private volatile bool _isCopyInProgress;
        private Copier _copier;

        public CopyForm()
        {
            InitializeComponent();
        }

        private void ControlCopyButtonEnabled()
        {
            buttonCopy.Enabled = textBoxSource.Text.Length > 0
                                 && textBoxTarget.Text.Length > 0
                                 && textBoxWorkdingDir.Text.Length > 0;
        }

        private void HandleCopyClick(object sender, EventArgs e)
        {
            if (_isCopyInProgress)
            {
                if (_copier!= null && MessageBox.Show("Are you sure you want to stop?", "Svn2Svn", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _copier.Stop();
                }
                return;
            }
            textBoxLog.Text = String.Empty;
            textBoxSourceRevision.Text = string.Empty;
            textBoxDestinationRevision.Text = string.Empty;
            buttonCopy.Text = "Stop";
            var t = new Thread(DoCopy);
            t.Start();
        }

        private void DoCopy()
        {
            Log(DateTime.Now);
            try
            {
                _isCopyInProgress = true;
                _copier = new Copier(new Uri(textBoxSource.Text), new Uri(textBoxTarget.Text),
                                        textBoxWorkdingDir.Text)
                                 {
                                     Logger = _logger,
                                     CopyAuthor = checkBoxCopyAuthor.Checked,
                                     CopyDateTime = checkBoxCopyDateTime.Checked,
                                     CopySourceRevision = checkBoxCopySourceRevision.Checked,
                                 };
                _copier.Copy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            finally
            {
                _isCopyInProgress = false;
                DoWindowUpdate(()=>buttonCopy.Text = "Copy");
            }
            Log(DateTime.Now);
        }

        private void HandleRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonError.Checked) _logger.Level = LogLevel.Error;
            else if (radioButtonInfo.Checked) _logger.Level = LogLevel.Info;
            else if (radioButtonTrace.Checked) _logger.Level = LogLevel.Trace;
        }

        private void HandleTextBoxTextChanged(object sender, EventArgs e)
        {
            ControlCopyButtonEnabled();
        }

        private void HandleCopyFormLoad(object sender, EventArgs e)
        {
            ControlCopyButtonEnabled();
            _logger = new TextBoxLogger { Parent = this, Level = LogLevel.Error };
        }

        private void Log(object o)
        {
            DoWindowUpdate(() => textBoxLog.Text += o + Environment.NewLine);
        }

        private void UpdateProgress(long sourceRevision, long destinationRevision)
        {
            textBoxSourceRevision.Text = sourceRevision.ToString("#0");
            textBoxDestinationRevision.Text = destinationRevision.ToString("#0");
        }

        private void DoWindowUpdate(Action action)
        {
            if (InvokeRequired) BeginInvoke(action);
            else action();
        }

        private class TextBoxLogger : AbstractLogger
        {
            public CopyForm Parent;
            protected override void Log(LogLevel level, string value)
            {
                Parent.Log(value);
            }

            public override void UpdateProgress(long sourceRevision, long destinationReivison)
            {
                Parent.DoWindowUpdate(()=>Parent.UpdateProgress(sourceRevision, destinationReivison));
            }
        }

        private void HandleCheckBoxCopyReversionPropertyCheckedChanged(object sender, EventArgs e)
        {
            var isChecked = checkBoxCopyReversionProperty.Checked;
            checkBoxCopyAuthor.Checked = isChecked;
            checkBoxCopyDateTime.Checked = isChecked;
            checkBoxCopySourceRevision.Checked = isChecked;
            checkBoxCopyAuthor.Enabled = isChecked;
            checkBoxCopyDateTime.Enabled = isChecked;
            checkBoxCopySourceRevision.Enabled = isChecked;
        }
    }
} ;
