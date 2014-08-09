namespace MailAgent.Manager
{
    partial class Manager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manager));
            this.serviceStartStopButton = new System.Windows.Forms.Button();
            this.serviceRestartButton = new System.Windows.Forms.Button();
            this.serviceGroupBox = new System.Windows.Forms.GroupBox();
            this.profileList = new System.Windows.Forms.ListBox();
            this.profileCreateButton = new System.Windows.Forms.Button();
            this.profileGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.profileBodyMatch = new System.Windows.Forms.TextBox();
            this.profileSubjectMatch = new System.Windows.Forms.TextBox();
            this.profileSaveAttachments = new System.Windows.Forms.CheckBox();
            this.profileSaveBody = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.profileAlias = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.profilePath = new System.Windows.Forms.TextBox();
            this.profileDefaultPath = new System.Windows.Forms.CheckBox();
            this.profileIsActive = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.profileKeys = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.profileName = new System.Windows.Forms.TextBox();
            this.profileID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.keyName = new System.Windows.Forms.TextBox();
            this.keyValue = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.keyType = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.serviceGroupBox.SuspendLayout();
            this.profileGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // serviceStartStopButton
            // 
            this.serviceStartStopButton.ForeColor = System.Drawing.Color.Black;
            this.serviceStartStopButton.Location = new System.Drawing.Point(6, 19);
            this.serviceStartStopButton.Name = "serviceStartStopButton";
            this.serviceStartStopButton.Size = new System.Drawing.Size(106, 23);
            this.serviceStartStopButton.TabIndex = 0;
            this.serviceStartStopButton.Text = "Unknown Status";
            this.serviceStartStopButton.UseVisualStyleBackColor = true;
            this.serviceStartStopButton.Click += new System.EventHandler(this.serviceButton_Click);
            // 
            // serviceRestartButton
            // 
            this.serviceRestartButton.ForeColor = System.Drawing.Color.Black;
            this.serviceRestartButton.Location = new System.Drawing.Point(6, 48);
            this.serviceRestartButton.Name = "serviceRestartButton";
            this.serviceRestartButton.Size = new System.Drawing.Size(106, 23);
            this.serviceRestartButton.TabIndex = 1;
            this.serviceRestartButton.Text = "Restart Service";
            this.serviceRestartButton.UseVisualStyleBackColor = true;
            this.serviceRestartButton.Click += new System.EventHandler(this.serviceButton_Click);
            // 
            // serviceGroupBox
            // 
            this.serviceGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.serviceGroupBox.Controls.Add(this.serviceStartStopButton);
            this.serviceGroupBox.Controls.Add(this.serviceRestartButton);
            this.serviceGroupBox.ForeColor = System.Drawing.Color.White;
            this.serviceGroupBox.Location = new System.Drawing.Point(12, 12);
            this.serviceGroupBox.Name = "serviceGroupBox";
            this.serviceGroupBox.Size = new System.Drawing.Size(118, 78);
            this.serviceGroupBox.TabIndex = 2;
            this.serviceGroupBox.TabStop = false;
            this.serviceGroupBox.Text = "Service Controls";
            // 
            // profileList
            // 
            this.profileList.ForeColor = System.Drawing.Color.Black;
            this.profileList.FormattingEnabled = true;
            this.profileList.Location = new System.Drawing.Point(6, 48);
            this.profileList.Name = "profileList";
            this.profileList.Size = new System.Drawing.Size(106, 212);
            this.profileList.TabIndex = 3;
            this.profileList.SelectedIndexChanged += new System.EventHandler(this.profileList_SelectedIndexChanged);
            // 
            // profileCreateButton
            // 
            this.profileCreateButton.ForeColor = System.Drawing.Color.Black;
            this.profileCreateButton.Location = new System.Drawing.Point(6, 19);
            this.profileCreateButton.Name = "profileCreateButton";
            this.profileCreateButton.Size = new System.Drawing.Size(106, 23);
            this.profileCreateButton.TabIndex = 4;
            this.profileCreateButton.Text = "New Profile";
            this.profileCreateButton.UseVisualStyleBackColor = true;
            // 
            // profileGroupBox
            // 
            this.profileGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.profileGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.profileGroupBox.Controls.Add(this.profileList);
            this.profileGroupBox.Controls.Add(this.profileCreateButton);
            this.profileGroupBox.ForeColor = System.Drawing.Color.White;
            this.profileGroupBox.Location = new System.Drawing.Point(12, 96);
            this.profileGroupBox.Name = "profileGroupBox";
            this.profileGroupBox.Size = new System.Drawing.Size(118, 277);
            this.profileGroupBox.TabIndex = 5;
            this.profileGroupBox.TabStop = false;
            this.profileGroupBox.Text = "Profiles";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.profileBodyMatch);
            this.groupBox1.Controls.Add(this.profileSubjectMatch);
            this.groupBox1.Controls.Add(this.profileSaveAttachments);
            this.groupBox1.Controls.Add(this.profileSaveBody);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.profileAlias);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.profilePath);
            this.groupBox1.Controls.Add(this.profileDefaultPath);
            this.groupBox1.Controls.Add(this.profileIsActive);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.profileName);
            this.groupBox1.Controls.Add(this.profileID);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(136, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(407, 361);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit Profile";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(227, 150);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Delimiter:";
            // 
            // textBox1
            // 
            this.textBox1.ForeColor = System.Drawing.Color.Black;
            this.textBox1.Location = new System.Drawing.Point(283, 147);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(115, 20);
            this.textBox1.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(19, 150);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Body Match:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(7, 124);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Subject Match:";
            // 
            // profileBodyMatch
            // 
            this.profileBodyMatch.ForeColor = System.Drawing.Color.Black;
            this.profileBodyMatch.Location = new System.Drawing.Point(92, 147);
            this.profileBodyMatch.Name = "profileBodyMatch";
            this.profileBodyMatch.Size = new System.Drawing.Size(111, 20);
            this.profileBodyMatch.TabIndex = 16;
            // 
            // profileSubjectMatch
            // 
            this.profileSubjectMatch.ForeColor = System.Drawing.Color.Black;
            this.profileSubjectMatch.Location = new System.Drawing.Point(92, 121);
            this.profileSubjectMatch.Name = "profileSubjectMatch";
            this.profileSubjectMatch.Size = new System.Drawing.Size(111, 20);
            this.profileSubjectMatch.TabIndex = 15;
            // 
            // profileSaveAttachments
            // 
            this.profileSaveAttachments.AutoSize = true;
            this.profileSaveAttachments.ForeColor = System.Drawing.Color.White;
            this.profileSaveAttachments.Location = new System.Drawing.Point(230, 97);
            this.profileSaveAttachments.Name = "profileSaveAttachments";
            this.profileSaveAttachments.Size = new System.Drawing.Size(119, 17);
            this.profileSaveAttachments.TabIndex = 14;
            this.profileSaveAttachments.Text = "Save Attachments?";
            this.profileSaveAttachments.UseVisualStyleBackColor = true;
            // 
            // profileSaveBody
            // 
            this.profileSaveBody.AutoSize = true;
            this.profileSaveBody.ForeColor = System.Drawing.Color.White;
            this.profileSaveBody.Location = new System.Drawing.Point(230, 71);
            this.profileSaveBody.Name = "profileSaveBody";
            this.profileSaveBody.Size = new System.Drawing.Size(112, 17);
            this.profileSaveBody.TabIndex = 13;
            this.profileSaveBody.Text = "Save Email Body?";
            this.profileSaveBody.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(28, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Alias:";
            // 
            // profileAlias
            // 
            this.profileAlias.ForeColor = System.Drawing.Color.Black;
            this.profileAlias.Location = new System.Drawing.Point(66, 69);
            this.profileAlias.Name = "profileAlias";
            this.profileAlias.Size = new System.Drawing.Size(137, 20);
            this.profileAlias.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(7, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Save Path:";
            // 
            // profilePath
            // 
            this.profilePath.ForeColor = System.Drawing.Color.Black;
            this.profilePath.Location = new System.Drawing.Point(73, 95);
            this.profilePath.Name = "profilePath";
            this.profilePath.Size = new System.Drawing.Size(130, 20);
            this.profilePath.TabIndex = 9;
            // 
            // profileDefaultPath
            // 
            this.profileDefaultPath.AutoSize = true;
            this.profileDefaultPath.ForeColor = System.Drawing.Color.White;
            this.profileDefaultPath.Location = new System.Drawing.Point(230, 46);
            this.profileDefaultPath.Name = "profileDefaultPath";
            this.profileDefaultPath.Size = new System.Drawing.Size(91, 17);
            this.profileDefaultPath.TabIndex = 8;
            this.profileDefaultPath.Text = "Default Path?";
            this.profileDefaultPath.UseVisualStyleBackColor = true;
            // 
            // profileIsActive
            // 
            this.profileIsActive.AutoSize = true;
            this.profileIsActive.ForeColor = System.Drawing.Color.White;
            this.profileIsActive.Location = new System.Drawing.Point(230, 20);
            this.profileIsActive.Name = "profileIsActive";
            this.profileIsActive.Size = new System.Drawing.Size(62, 17);
            this.profileIsActive.TabIndex = 7;
            this.profileIsActive.Text = "Active?";
            this.profileIsActive.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(12, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Keys:";
            // 
            // profileKeys
            // 
            this.profileKeys.ForeColor = System.Drawing.Color.Black;
            this.profileKeys.FormattingEnabled = true;
            this.profileKeys.Location = new System.Drawing.Point(51, 19);
            this.profileKeys.Name = "profileKeys";
            this.profileKeys.Size = new System.Drawing.Size(120, 121);
            this.profileKeys.TabIndex = 5;
            this.profileKeys.SelectedIndexChanged += new System.EventHandler(this.profileKeys_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(22, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name:";
            // 
            // profileName
            // 
            this.profileName.ForeColor = System.Drawing.Color.Black;
            this.profileName.Location = new System.Drawing.Point(66, 43);
            this.profileName.Name = "profileName";
            this.profileName.Size = new System.Drawing.Size(137, 20);
            this.profileName.TabIndex = 3;
            // 
            // profileID
            // 
            this.profileID.ForeColor = System.Drawing.Color.Black;
            this.profileID.Location = new System.Drawing.Point(66, 17);
            this.profileID.Name = "profileID";
            this.profileID.Size = new System.Drawing.Size(137, 20);
            this.profileID.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Profile ID:";
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(323, 332);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Save Profile";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // keyName
            // 
            this.keyName.ForeColor = System.Drawing.Color.Black;
            this.keyName.Location = new System.Drawing.Point(233, 19);
            this.keyName.Name = "keyName";
            this.keyName.Size = new System.Drawing.Size(143, 20);
            this.keyName.TabIndex = 21;
            // 
            // keyValue
            // 
            this.keyValue.ForeColor = System.Drawing.Color.Black;
            this.keyValue.Location = new System.Drawing.Point(233, 72);
            this.keyValue.Name = "keyValue";
            this.keyValue.Size = new System.Drawing.Size(143, 20);
            this.keyValue.TabIndex = 22;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(189, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "Name:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(190, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Value:";
            // 
            // keyType
            // 
            this.keyType.ForeColor = System.Drawing.Color.Black;
            this.keyType.FormattingEnabled = true;
            this.keyType.Items.AddRange(new object[] {
            "Dynamic",
            "Static",
            "Search"});
            this.keyType.Location = new System.Drawing.Point(233, 45);
            this.keyType.Name = "keyType";
            this.keyType.Size = new System.Drawing.Size(143, 21);
            this.keyType.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(193, 48);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Type:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.keyType);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.profileKeys);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.keyName);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.keyValue);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(10, 173);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(388, 153);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Link Keys";
            // 
            // button2
            // 
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(301, 117);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 28;
            this.button2.Text = "Save Key";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Manager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(555, 385);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.profileGroupBox);
            this.Controls.Add(this.serviceGroupBox);
            this.Name = "Manager";
            this.Text = "Mail Agent Manager";
            this.Load += new System.EventHandler(this.Manager_Load);
            this.serviceGroupBox.ResumeLayout(false);
            this.profileGroupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button serviceStartStopButton;
        private System.Windows.Forms.Button serviceRestartButton;
        private System.Windows.Forms.GroupBox serviceGroupBox;
        private System.Windows.Forms.ListBox profileList;
        private System.Windows.Forms.Button profileCreateButton;
        private System.Windows.Forms.GroupBox profileGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox profileName;
        private System.Windows.Forms.TextBox profileID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox profileKeys;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox profileIsActive;
        private System.Windows.Forms.CheckBox profileDefaultPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox profilePath;
        private System.Windows.Forms.TextBox profileAlias;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox profileBodyMatch;
        private System.Windows.Forms.TextBox profileSubjectMatch;
        private System.Windows.Forms.CheckBox profileSaveAttachments;
        private System.Windows.Forms.CheckBox profileSaveBody;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox keyType;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox keyValue;
        private System.Windows.Forms.TextBox keyName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button2;
    }
}

