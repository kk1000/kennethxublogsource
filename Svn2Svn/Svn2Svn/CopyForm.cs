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
        private Interaction _interaction;
        private volatile bool _isCopyInProgress;
        private Copier _copier;
        private long _fromRevision = 0;
        private long _toRevision = -1;

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
                if (_copier != null && ConfirmStop() == DialogResult.Yes) _copier.Stop();
                return;
            }
            textBoxLog.Text = String.Empty;
            textBoxSourceRevision.Text = string.Empty;
            textBoxDestinationRevision.Text = string.Empty;
            buttonCopy.Text = "St&op";
            var t = new Thread(DoCopy);
            t.Start();
        }

        private static DialogResult ConfirmStop()
        {
            return MessageBox.Show("Are you sure you want to stop?", "Svn2Svn", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
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
                                     Interaction = _interaction,
                                     CopyAuthor = checkBoxCopyAuthor.Checked,
                                     CopyDateTime = checkBoxCopyDateTime.Checked,
                                     CopySourceRevision = checkBoxCopySourceRevision.Checked,
                                     StartRevision = _fromRevision,
                                     EndRevision = _toRevision,
                                 };
                _copier.Copy();
            }
            catch (Exception ex)
            {
                _interaction.Error(ex.ToString());
            }
            finally
            {
                _isCopyInProgress = false;
                DoWindowUpdate(()=>buttonCopy.Text = "C&opy");
            }
            Log(DateTime.Now);
        }

        private void HandleRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonError.Checked) _interaction.Level = LogLevel.Error;
            else if (radioButtonInfo.Checked) _interaction.Level = LogLevel.Info;
            else if (radioButtonTrace.Checked) _interaction.Level = LogLevel.Trace;
        }

        private void HandleTextBoxTextChanged(object sender, EventArgs e)
        {
            ControlCopyButtonEnabled();
        }

        private void HandleCopyFormLoad(object sender, EventArgs e)
        {
            ControlCopyButtonEnabled();
            _interaction = new Interaction { Parent = this, Level = LogLevel.Error };
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

        private void HandleTextBoxToRevisionTextChanged(object sender, EventArgs e)
        {
            RegulateRevisionTextBox((TextBox)sender, ref _toRevision, -1);
        }

        private void HandleTextBoxFromRevisionTextChanged(object sender, EventArgs e)
        {
            RegulateRevisionTextBox((TextBox)sender, ref _fromRevision, 0);
        }

        private void RegulateRevisionTextBox(TextBox textBox, ref long saveRevision, long x)
        {
            var selectionStart = textBox.SelectionStart;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                saveRevision = x;
                return;
            }
            long revision;
            if (!long.TryParse(textBox.Text, out revision) || revision < x)
            {
                textBox.Text = saveRevision == x ? string.Empty : saveRevision.ToString("#0");
                textBox.Select(selectionStart - 1, 0);
                return;
            }
            saveRevision = revision;
            if (revision == x) textBox.Text = string.Empty;
        }

        private class Interaction : AbstractInteraction
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

            public override ErrorDisposition Ask(string title, string message)
            {
                if (Parent.InvokeRequired)
                {
                    return (ErrorDisposition)Parent.Invoke(
                        new Func<ErrorDisposition>(() => Ask(title, message)));
                }
                var result = MessageBox.Show(message, title,
                                             MessageBoxButtons.AbortRetryIgnore,
                                             MessageBoxIcon.Error,
                                             MessageBoxDefaultButton.Button2);
                switch (result)
                {
                        case DialogResult.Retry: return ErrorDisposition.Retry;
                        case DialogResult.Ignore:
                        var ignoreAll = MessageBox.Show(
                            "Do you want ignore all subsequent errors of same type?",
                            title,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2);
                        return ignoreAll == DialogResult.Yes ? ErrorDisposition.IgnoreAll : ErrorDisposition.Ignore;
                }
                return ErrorDisposition.Fail;
            }
        }
    }
} ;
