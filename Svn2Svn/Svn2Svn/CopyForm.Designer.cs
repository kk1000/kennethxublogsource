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
namespace Svn2Svn
{
    partial class CopyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTarget = new System.Windows.Forms.TextBox();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.textBoxWorkdingDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxCopyRevisionProperties = new System.Windows.Forms.GroupBox();
            this.checkBoxCopySourceRevision = new System.Windows.Forms.CheckBox();
            this.checkBoxCopyDateTime = new System.Windows.Forms.CheckBox();
            this.checkBoxCopyAuthor = new System.Windows.Forms.CheckBox();
            this.checkBoxCopyReversionProperty = new System.Windows.Forms.CheckBox();
            this.textBoxDestinationRevision = new System.Windows.Forms.TextBox();
            this.textBoxSourceRevision = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonTrace = new System.Windows.Forms.RadioButton();
            this.radioButtonInfo = new System.Windows.Forms.RadioButton();
            this.radioButtonError = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.groupBoxCopyRevisionProperties.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSource
            // 
            this.textBoxSource.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxSource.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this.textBoxSource.Location = new System.Drawing.Point(103, 10);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(277, 20);
            this.textBoxSource.TabIndex = 1;
            this.textBoxSource.TextChanged += new System.EventHandler(this.HandleTextBoxTextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Source URI:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Destination URI:";
            // 
            // textBoxTarget
            // 
            this.textBoxTarget.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxTarget.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this.textBoxTarget.Location = new System.Drawing.Point(103, 36);
            this.textBoxTarget.Name = "textBoxTarget";
            this.textBoxTarget.Size = new System.Drawing.Size(277, 20);
            this.textBoxTarget.TabIndex = 3;
            this.textBoxTarget.TextChanged += new System.EventHandler(this.HandleTextBoxTextChanged);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(555, 8);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(75, 23);
            this.buttonCopy.TabIndex = 7;
            this.buttonCopy.Text = "C&opy";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.HandleCopyClick);
            // 
            // textBoxWorkdingDir
            // 
            this.textBoxWorkdingDir.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxWorkdingDir.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBoxWorkdingDir.Location = new System.Drawing.Point(103, 62);
            this.textBoxWorkdingDir.Name = "textBoxWorkdingDir";
            this.textBoxWorkdingDir.Size = new System.Drawing.Size(277, 20);
            this.textBoxWorkdingDir.TabIndex = 5;
            this.textBoxWorkdingDir.TextChanged += new System.EventHandler(this.HandleTextBoxTextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "&Working Dir:";
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Location = new System.Drawing.Point(0, 135);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(643, 153);
            this.textBoxLog.TabIndex = 1;
            this.textBoxLog.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxCopyRevisionProperties);
            this.panel1.Controls.Add(this.textBoxDestinationRevision);
            this.panel1.Controls.Add(this.textBoxSourceRevision);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.textBoxTarget);
            this.panel1.Controls.Add(this.textBoxSource);
            this.panel1.Controls.Add(this.buttonCopy);
            this.panel1.Controls.Add(this.textBoxWorkdingDir);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(643, 135);
            this.panel1.TabIndex = 0;
            // 
            // groupBoxCopyRevisionProperties
            // 
            this.groupBoxCopyRevisionProperties.Controls.Add(this.checkBoxCopySourceRevision);
            this.groupBoxCopyRevisionProperties.Controls.Add(this.checkBoxCopyDateTime);
            this.groupBoxCopyRevisionProperties.Controls.Add(this.checkBoxCopyAuthor);
            this.groupBoxCopyRevisionProperties.Controls.Add(this.checkBoxCopyReversionProperty);
            this.groupBoxCopyRevisionProperties.Location = new System.Drawing.Point(386, 13);
            this.groupBoxCopyRevisionProperties.Name = "groupBoxCopyRevisionProperties";
            this.groupBoxCopyRevisionProperties.Size = new System.Drawing.Size(163, 69);
            this.groupBoxCopyRevisionProperties.TabIndex = 6;
            this.groupBoxCopyRevisionProperties.TabStop = false;
            // 
            // checkBoxCopySourceRevision
            // 
            this.checkBoxCopySourceRevision.AutoSize = true;
            this.checkBoxCopySourceRevision.Checked = true;
            this.checkBoxCopySourceRevision.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCopySourceRevision.Location = new System.Drawing.Point(6, 43);
            this.checkBoxCopySourceRevision.Name = "checkBoxCopySourceRevision";
            this.checkBoxCopySourceRevision.Size = new System.Drawing.Size(104, 17);
            this.checkBoxCopySourceRevision.TabIndex = 3;
            this.checkBoxCopySourceRevision.Text = "Source &Revision";
            this.checkBoxCopySourceRevision.UseVisualStyleBackColor = true;
            // 
            // checkBoxCopyDateTime
            // 
            this.checkBoxCopyDateTime.AutoSize = true;
            this.checkBoxCopyDateTime.Checked = true;
            this.checkBoxCopyDateTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCopyDateTime.Location = new System.Drawing.Point(74, 20);
            this.checkBoxCopyDateTime.Name = "checkBoxCopyDateTime";
            this.checkBoxCopyDateTime.Size = new System.Drawing.Size(77, 17);
            this.checkBoxCopyDateTime.TabIndex = 2;
            this.checkBoxCopyDateTime.Text = "Date/&Time";
            this.checkBoxCopyDateTime.UseVisualStyleBackColor = true;
            // 
            // checkBoxCopyAuthor
            // 
            this.checkBoxCopyAuthor.AutoSize = true;
            this.checkBoxCopyAuthor.Checked = true;
            this.checkBoxCopyAuthor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCopyAuthor.Location = new System.Drawing.Point(7, 20);
            this.checkBoxCopyAuthor.Name = "checkBoxCopyAuthor";
            this.checkBoxCopyAuthor.Size = new System.Drawing.Size(57, 17);
            this.checkBoxCopyAuthor.TabIndex = 1;
            this.checkBoxCopyAuthor.Text = "&Author";
            this.checkBoxCopyAuthor.UseVisualStyleBackColor = true;
            // 
            // checkBoxCopyReversionProperty
            // 
            this.checkBoxCopyReversionProperty.AutoSize = true;
            this.checkBoxCopyReversionProperty.Checked = true;
            this.checkBoxCopyReversionProperty.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCopyReversionProperty.Location = new System.Drawing.Point(7, -1);
            this.checkBoxCopyReversionProperty.Name = "checkBoxCopyReversionProperty";
            this.checkBoxCopyReversionProperty.Size = new System.Drawing.Size(144, 17);
            this.checkBoxCopyReversionProperty.TabIndex = 0;
            this.checkBoxCopyReversionProperty.Text = "Copy Revision &Properties";
            this.checkBoxCopyReversionProperty.UseVisualStyleBackColor = true;
            this.checkBoxCopyReversionProperty.CheckedChanged += new System.EventHandler(this.HandleCheckBoxCopyReversionPropertyCheckedChanged);
            // 
            // textBoxDestinationRevision
            // 
            this.textBoxDestinationRevision.Location = new System.Drawing.Point(510, 102);
            this.textBoxDestinationRevision.Name = "textBoxDestinationRevision";
            this.textBoxDestinationRevision.ReadOnly = true;
            this.textBoxDestinationRevision.Size = new System.Drawing.Size(60, 20);
            this.textBoxDestinationRevision.TabIndex = 12;
            this.textBoxDestinationRevision.TabStop = false;
            // 
            // textBoxSourceRevision
            // 
            this.textBoxSourceRevision.Location = new System.Drawing.Point(341, 102);
            this.textBoxSourceRevision.Name = "textBoxSourceRevision";
            this.textBoxSourceRevision.ReadOnly = true;
            this.textBoxSourceRevision.Size = new System.Drawing.Size(59, 20);
            this.textBoxSourceRevision.TabIndex = 10;
            this.textBoxSourceRevision.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(406, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Destination Revision:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(256, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Source Revision:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonTrace);
            this.groupBox1.Controls.Add(this.radioButtonInfo);
            this.groupBox1.Controls.Add(this.radioButtonError);
            this.groupBox1.Location = new System.Drawing.Point(12, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(218, 38);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log level:";
            // 
            // radioButtonTrace
            // 
            this.radioButtonTrace.AutoSize = true;
            this.radioButtonTrace.Location = new System.Drawing.Point(153, 15);
            this.radioButtonTrace.Name = "radioButtonTrace";
            this.radioButtonTrace.Size = new System.Drawing.Size(52, 17);
            this.radioButtonTrace.TabIndex = 2;
            this.radioButtonTrace.Text = "D&etail";
            this.radioButtonTrace.UseVisualStyleBackColor = true;
            this.radioButtonTrace.CheckedChanged += new System.EventHandler(this.HandleRadioButtonCheckedChanged);
            // 
            // radioButtonInfo
            // 
            this.radioButtonInfo.AutoSize = true;
            this.radioButtonInfo.Location = new System.Drawing.Point(81, 15);
            this.radioButtonInfo.Name = "radioButtonInfo";
            this.radioButtonInfo.Size = new System.Drawing.Size(66, 17);
            this.radioButtonInfo.TabIndex = 1;
            this.radioButtonInfo.Text = "R&evision";
            this.radioButtonInfo.UseVisualStyleBackColor = true;
            this.radioButtonInfo.CheckedChanged += new System.EventHandler(this.HandleRadioButtonCheckedChanged);
            // 
            // radioButtonError
            // 
            this.radioButtonError.AutoSize = true;
            this.radioButtonError.Checked = true;
            this.radioButtonError.Location = new System.Drawing.Point(16, 15);
            this.radioButtonError.Name = "radioButtonError";
            this.radioButtonError.Size = new System.Drawing.Size(47, 17);
            this.radioButtonError.TabIndex = 0;
            this.radioButtonError.TabStop = true;
            this.radioButtonError.Text = "&Error";
            this.radioButtonError.UseVisualStyleBackColor = true;
            this.radioButtonError.CheckedChanged += new System.EventHandler(this.HandleRadioButtonCheckedChanged);
            // 
            // CopyForm
            // 
            this.AcceptButton = this.buttonCopy;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 288);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(659, 171);
            this.Name = "CopyForm";
            this.Text = "Svn2Svn";
            this.Load += new System.EventHandler(this.HandleCopyFormLoad);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBoxCopyRevisionProperties.ResumeLayout(false);
            this.groupBoxCopyRevisionProperties.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTarget;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.TextBox textBoxWorkdingDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonTrace;
        private System.Windows.Forms.RadioButton radioButtonInfo;
        private System.Windows.Forms.RadioButton radioButtonError;
        private System.Windows.Forms.CheckBox checkBoxCopyReversionProperty;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxDestinationRevision;
        private System.Windows.Forms.TextBox textBoxSourceRevision;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBoxCopyRevisionProperties;
        private System.Windows.Forms.CheckBox checkBoxCopySourceRevision;
        private System.Windows.Forms.CheckBox checkBoxCopyDateTime;
        private System.Windows.Forms.CheckBox checkBoxCopyAuthor;
    }
}

