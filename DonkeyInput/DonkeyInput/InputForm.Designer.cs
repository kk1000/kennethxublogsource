namespace DonkeyInput
{
    partial class InputForm
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
            this.labelServer = new System.Windows.Forms.Label();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.textBoxLinks = new System.Windows.Forms.TextBox();
            this.panelAuthentication = new System.Windows.Forms.Panel();
            this.panelServer = new System.Windows.Forms.Panel();
            this.tableLayoutPanelBottom = new System.Windows.Forms.TableLayoutPanel();
            this.panelAction = new System.Windows.Forms.Panel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.panelPort = new System.Windows.Forms.Panel();
            this.labelPort = new System.Windows.Forms.Label();
            this.panelMiddle = new System.Windows.Forms.Panel();
            this.panelAuthentication.SuspendLayout();
            this.panelServer.SuspendLayout();
            this.tableLayoutPanelBottom.SuspendLayout();
            this.panelAction.SuspendLayout();
            this.tableLayoutPanelTop.SuspendLayout();
            this.panelPort.SuspendLayout();
            this.panelMiddle.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelServer
            // 
            this.labelServer.AutoSize = true;
            this.labelServer.Location = new System.Drawing.Point(0, 3);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(38, 13);
            this.labelServer.TabIndex = 0;
            this.labelServer.Text = "Server";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxServer.Location = new System.Drawing.Point(56, 6);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(287, 20);
            this.textBoxServer.TabIndex = 0;
            this.textBoxServer.Text = "127.0.0.1";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(393, 6);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(44, 20);
            this.textBoxPort.TabIndex = 1;
            this.textBoxPort.Text = "4080";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(195, 3);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(62, 20);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(140, 6);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(53, 13);
            this.labelPassword.TabIndex = 2;
            this.labelPassword.Text = "Password";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(60, 3);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(70, 20);
            this.textBoxUserName.TabIndex = 0;
            this.textBoxUserName.Text = "admin";
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(0, 6);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(60, 13);
            this.labelUserName.TabIndex = 0;
            this.labelUserName.Text = "User Name";
            // 
            // textBoxLinks
            // 
            this.textBoxLinks.AcceptsReturn = true;
            this.textBoxLinks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLinks.Location = new System.Drawing.Point(0, 0);
            this.textBoxLinks.Multiline = true;
            this.textBoxLinks.Name = "textBoxLinks";
            this.textBoxLinks.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLinks.Size = new System.Drawing.Size(446, 78);
            this.textBoxLinks.TabIndex = 0;
            this.textBoxLinks.WordWrap = false;
            // 
            // panelAuthentication
            // 
            this.panelAuthentication.AutoSize = true;
            this.panelAuthentication.Controls.Add(this.textBoxPassword);
            this.panelAuthentication.Controls.Add(this.labelUserName);
            this.panelAuthentication.Controls.Add(this.labelPassword);
            this.panelAuthentication.Controls.Add(this.textBoxUserName);
            this.panelAuthentication.Location = new System.Drawing.Point(9, 6);
            this.panelAuthentication.Name = "panelAuthentication";
            this.panelAuthentication.Size = new System.Drawing.Size(260, 26);
            this.panelAuthentication.TabIndex = 0;
            // 
            // panelServer
            // 
            this.panelServer.AutoSize = true;
            this.panelServer.Controls.Add(this.labelServer);
            this.panelServer.Location = new System.Drawing.Point(9, 6);
            this.panelServer.Name = "panelServer";
            this.panelServer.Size = new System.Drawing.Size(41, 16);
            this.panelServer.TabIndex = 0;
            // 
            // tableLayoutPanelBottom
            // 
            this.tableLayoutPanelBottom.AutoSize = true;
            this.tableLayoutPanelBottom.ColumnCount = 2;
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelBottom.Controls.Add(this.panelAction, 1, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.panelAuthentication, 0, 0);
            this.tableLayoutPanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelBottom.Location = new System.Drawing.Point(0, 110);
            this.tableLayoutPanelBottom.Name = "tableLayoutPanelBottom";
            this.tableLayoutPanelBottom.Padding = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.tableLayoutPanelBottom.RowCount = 1;
            this.tableLayoutPanelBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelBottom.Size = new System.Drawing.Size(446, 39);
            this.tableLayoutPanelBottom.TabIndex = 3;
            // 
            // panelAction
            // 
            this.panelAction.AutoSize = true;
            this.panelAction.Controls.Add(this.buttonCancel);
            this.panelAction.Controls.Add(this.buttonOK);
            this.panelAction.Location = new System.Drawing.Point(279, 6);
            this.panelAction.Name = "panelAction";
            this.panelAction.Size = new System.Drawing.Size(158, 27);
            this.panelAction.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(80, 1);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.HandleButtonCancelClick);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(-1, 1);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.HandleButtonOkClick);
            // 
            // tableLayoutPanelTop
            // 
            this.tableLayoutPanelTop.AutoSize = true;
            this.tableLayoutPanelTop.ColumnCount = 4;
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTop.Controls.Add(this.panelPort, 2, 0);
            this.tableLayoutPanelTop.Controls.Add(this.textBoxServer, 1, 0);
            this.tableLayoutPanelTop.Controls.Add(this.panelServer, 0, 0);
            this.tableLayoutPanelTop.Controls.Add(this.textBoxPort, 3, 0);
            this.tableLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelTop.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            this.tableLayoutPanelTop.Padding = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.tableLayoutPanelTop.RowCount = 1;
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(446, 32);
            this.tableLayoutPanelTop.TabIndex = 1;
            // 
            // panelPort
            // 
            this.panelPort.AutoSize = true;
            this.panelPort.Controls.Add(this.labelPort);
            this.panelPort.Location = new System.Drawing.Point(349, 6);
            this.panelPort.Name = "panelPort";
            this.panelPort.Size = new System.Drawing.Size(38, 16);
            this.panelPort.TabIndex = 2;
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(9, 3);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(26, 13);
            this.labelPort.TabIndex = 0;
            this.labelPort.Text = "Port";
            // 
            // panelMiddle
            // 
            this.panelMiddle.Controls.Add(this.textBoxLinks);
            this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMiddle.Location = new System.Drawing.Point(0, 32);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(446, 78);
            this.panelMiddle.TabIndex = 2;
            // 
            // InputForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(446, 149);
            this.Controls.Add(this.panelMiddle);
            this.Controls.Add(this.tableLayoutPanelTop);
            this.Controls.Add(this.tableLayoutPanelBottom);
            this.MinimumSize = new System.Drawing.Size(462, 153);
            this.Name = "InputForm";
            this.Text = "MLDonkey Download Input";
            this.Load += new System.EventHandler(this.HandleInputFormLoad);
            this.Shown += new System.EventHandler(this.HandleInputFormShown);
            this.panelAuthentication.ResumeLayout(false);
            this.panelAuthentication.PerformLayout();
            this.panelServer.ResumeLayout(false);
            this.panelServer.PerformLayout();
            this.tableLayoutPanelBottom.ResumeLayout(false);
            this.tableLayoutPanelBottom.PerformLayout();
            this.panelAction.ResumeLayout(false);
            this.tableLayoutPanelTop.ResumeLayout(false);
            this.tableLayoutPanelTop.PerformLayout();
            this.panelPort.ResumeLayout(false);
            this.panelPort.PerformLayout();
            this.panelMiddle.ResumeLayout(false);
            this.panelMiddle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.TextBox textBoxLinks;
        private System.Windows.Forms.Panel panelAuthentication;
        private System.Windows.Forms.Panel panelServer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBottom;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panelAction;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Panel panelPort;
        private System.Windows.Forms.Panel panelMiddle;
    }
}

