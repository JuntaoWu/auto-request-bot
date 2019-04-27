namespace test
{
    partial class Main
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
            this.button1 = new System.Windows.Forms.Button();
            this.textboxDisplay = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.register_btn = new System.Windows.Forms.Button();
            this.add_user_btn = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.active_label = new System.Windows.Forms.Label();
            this.openId_txt = new System.Windows.Forms.TextBox();
            this.openid_label = new System.Windows.Forms.Label();
            this.confirm_btn = new System.Windows.Forms.Button();
            this.image_picturebox = new System.Windows.Forms.PictureBox();
            this.checkin_address_combox = new System.Windows.Forms.ComboBox();
            this.contact_telephone_txt = new System.Windows.Forms.TextBox();
            this.contact_name_txt = new System.Windows.Forms.TextBox();
            this.weixin_number_txt = new System.Windows.Forms.TextBox();
            this.weixin_username_txt = new System.Windows.Forms.TextBox();
            this.upload_image_btn = new System.Windows.Forms.Button();
            this.checkin_address_label = new System.Windows.Forms.Label();
            this.contact_telephone_label = new System.Windows.Forms.Label();
            this.contact_name_label = new System.Windows.Forms.Label();
            this.weixin_number_label = new System.Windows.Forms.Label();
            this.weixin_username_label = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image_picturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(35, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textboxDisplay
            // 
            this.textboxDisplay.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textboxDisplay.Location = new System.Drawing.Point(0, 634);
            this.textboxDisplay.Multiline = true;
            this.textboxDisplay.Name = "textboxDisplay";
            this.textboxDisplay.Size = new System.Drawing.Size(1056, 75);
            this.textboxDisplay.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(188, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(326, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Clear";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // register_btn
            // 
            this.register_btn.Location = new System.Drawing.Point(167, 494);
            this.register_btn.Name = "register_btn";
            this.register_btn.Size = new System.Drawing.Size(112, 32);
            this.register_btn.TabIndex = 4;
            this.register_btn.Text = "激活";
            this.register_btn.UseVisualStyleBackColor = true;
            this.register_btn.Click += new System.EventHandler(this.register_btn_Click);
            // 
            // add_user_btn
            // 
            this.add_user_btn.Location = new System.Drawing.Point(601, 11);
            this.add_user_btn.Name = "add_user_btn";
            this.add_user_btn.Size = new System.Drawing.Size(116, 23);
            this.add_user_btn.TabIndex = 6;
            this.add_user_btn.Text = "新增用户";
            this.add_user_btn.UseVisualStyleBackColor = true;
            this.add_user_btn.Click += new System.EventHandler(this.add_user_btn_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.ItemSize = new System.Drawing.Size(60, 120);
            this.tabControl1.Location = new System.Drawing.Point(35, 53);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(983, 553);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 7;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem_1);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(124, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(855, 545);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "会员管理";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.active_label);
            this.tabPage2.Controls.Add(this.openId_txt);
            this.tabPage2.Controls.Add(this.openid_label);
            this.tabPage2.Controls.Add(this.register_btn);
            this.tabPage2.Controls.Add(this.confirm_btn);
            this.tabPage2.Controls.Add(this.image_picturebox);
            this.tabPage2.Controls.Add(this.checkin_address_combox);
            this.tabPage2.Controls.Add(this.contact_telephone_txt);
            this.tabPage2.Controls.Add(this.contact_name_txt);
            this.tabPage2.Controls.Add(this.weixin_number_txt);
            this.tabPage2.Controls.Add(this.weixin_username_txt);
            this.tabPage2.Controls.Add(this.upload_image_btn);
            this.tabPage2.Controls.Add(this.checkin_address_label);
            this.tabPage2.Controls.Add(this.contact_telephone_label);
            this.tabPage2.Controls.Add(this.contact_name_label);
            this.tabPage2.Controls.Add(this.weixin_number_label);
            this.tabPage2.Controls.Add(this.weixin_username_label);
            this.tabPage2.Location = new System.Drawing.Point(124, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(855, 545);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "会员注册";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // active_label
            // 
            this.active_label.AutoSize = true;
            this.active_label.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.active_label.Location = new System.Drawing.Point(26, 503);
            this.active_label.Name = "active_label";
            this.active_label.Size = new System.Drawing.Size(55, 15);
            this.active_label.TabIndex = 15;
            this.active_label.Text = "未激活";
            // 
            // openId_txt
            // 
            this.openId_txt.Enabled = false;
            this.openId_txt.Location = new System.Drawing.Point(167, 446);
            this.openId_txt.Name = "openId_txt";
            this.openId_txt.Size = new System.Drawing.Size(324, 25);
            this.openId_txt.TabIndex = 14;
            // 
            // openid_label
            // 
            this.openid_label.AutoSize = true;
            this.openid_label.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.openid_label.Location = new System.Drawing.Point(61, 446);
            this.openid_label.Name = "openid_label";
            this.openid_label.Size = new System.Drawing.Size(70, 15);
            this.openid_label.TabIndex = 13;
            this.openid_label.Text = "OpenId:";
            // 
            // confirm_btn
            // 
            this.confirm_btn.Enabled = false;
            this.confirm_btn.Location = new System.Drawing.Point(386, 494);
            this.confirm_btn.Name = "confirm_btn";
            this.confirm_btn.Size = new System.Drawing.Size(105, 32);
            this.confirm_btn.TabIndex = 12;
            this.confirm_btn.Text = "提交";
            this.confirm_btn.UseVisualStyleBackColor = true;
            this.confirm_btn.Click += new System.EventHandler(this.confirm_btn_Click);
            // 
            // image_picturebox
            // 
            this.image_picturebox.Location = new System.Drawing.Point(167, 272);
            this.image_picturebox.Name = "image_picturebox";
            this.image_picturebox.Size = new System.Drawing.Size(324, 145);
            this.image_picturebox.TabIndex = 11;
            this.image_picturebox.TabStop = false;
            // 
            // checkin_address_combox
            // 
            this.checkin_address_combox.FormattingEnabled = true;
            this.checkin_address_combox.Location = new System.Drawing.Point(167, 209);
            this.checkin_address_combox.Name = "checkin_address_combox";
            this.checkin_address_combox.Size = new System.Drawing.Size(324, 23);
            this.checkin_address_combox.TabIndex = 10;
            // 
            // contact_telephone_txt
            // 
            this.contact_telephone_txt.Location = new System.Drawing.Point(167, 157);
            this.contact_telephone_txt.Name = "contact_telephone_txt";
            this.contact_telephone_txt.Size = new System.Drawing.Size(324, 25);
            this.contact_telephone_txt.TabIndex = 9;
            // 
            // contact_name_txt
            // 
            this.contact_name_txt.Location = new System.Drawing.Point(167, 114);
            this.contact_name_txt.Name = "contact_name_txt";
            this.contact_name_txt.Size = new System.Drawing.Size(324, 25);
            this.contact_name_txt.TabIndex = 8;
            // 
            // weixin_number_txt
            // 
            this.weixin_number_txt.Location = new System.Drawing.Point(167, 67);
            this.weixin_number_txt.Name = "weixin_number_txt";
            this.weixin_number_txt.Size = new System.Drawing.Size(324, 25);
            this.weixin_number_txt.TabIndex = 7;
            // 
            // weixin_username_txt
            // 
            this.weixin_username_txt.Location = new System.Drawing.Point(167, 19);
            this.weixin_username_txt.Name = "weixin_username_txt";
            this.weixin_username_txt.Size = new System.Drawing.Size(324, 25);
            this.weixin_username_txt.TabIndex = 6;
            // 
            // upload_image_btn
            // 
            this.upload_image_btn.Location = new System.Drawing.Point(37, 272);
            this.upload_image_btn.Name = "upload_image_btn";
            this.upload_image_btn.Size = new System.Drawing.Size(88, 29);
            this.upload_image_btn.TabIndex = 5;
            this.upload_image_btn.Text = "上传头像";
            this.upload_image_btn.UseVisualStyleBackColor = true;
            this.upload_image_btn.Click += new System.EventHandler(this.upload_image_btn_Click);
            // 
            // checkin_address_label
            // 
            this.checkin_address_label.AutoSize = true;
            this.checkin_address_label.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.checkin_address_label.Location = new System.Drawing.Point(42, 217);
            this.checkin_address_label.Name = "checkin_address_label";
            this.checkin_address_label.Size = new System.Drawing.Size(80, 15);
            this.checkin_address_label.TabIndex = 4;
            this.checkin_address_label.Text = "打卡地址:";
            // 
            // contact_telephone_label
            // 
            this.contact_telephone_label.AutoSize = true;
            this.contact_telephone_label.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.contact_telephone_label.Location = new System.Drawing.Point(29, 167);
            this.contact_telephone_label.Name = "contact_telephone_label";
            this.contact_telephone_label.Size = new System.Drawing.Size(96, 15);
            this.contact_telephone_label.TabIndex = 3;
            this.contact_telephone_label.Text = "联系人电话:";
            // 
            // contact_name_label
            // 
            this.contact_name_label.AutoSize = true;
            this.contact_name_label.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.contact_name_label.Location = new System.Drawing.Point(26, 114);
            this.contact_name_label.Name = "contact_name_label";
            this.contact_name_label.Size = new System.Drawing.Size(103, 15);
            this.contact_name_label.TabIndex = 2;
            this.contact_name_label.Text = "联系人姓名：";
            // 
            // weixin_number_label
            // 
            this.weixin_number_label.AutoSize = true;
            this.weixin_number_label.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.weixin_number_label.Location = new System.Drawing.Point(58, 67);
            this.weixin_number_label.Name = "weixin_number_label";
            this.weixin_number_label.Size = new System.Drawing.Size(64, 15);
            this.weixin_number_label.TabIndex = 1;
            this.weixin_number_label.Text = "微信号:";
            // 
            // weixin_username_label
            // 
            this.weixin_username_label.AutoSize = true;
            this.weixin_username_label.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.weixin_username_label.Location = new System.Drawing.Point(29, 19);
            this.weixin_username_label.Name = "weixin_username_label";
            this.weixin_username_label.Size = new System.Drawing.Size(96, 15);
            this.weixin_username_label.TabIndex = 0;
            this.weixin_username_label.Text = "微信用户名:";
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(124, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(855, 545);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "会员列表";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 709);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.add_user_btn);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textboxDisplay);
            this.Controls.Add(this.button1);
            this.Name = "Main";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image_picturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textboxDisplay;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button register_btn;
        private System.Windows.Forms.Button add_user_btn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label contact_telephone_label;
        private System.Windows.Forms.Label contact_name_label;
        private System.Windows.Forms.Label weixin_number_label;
        private System.Windows.Forms.Label weixin_username_label;
        private System.Windows.Forms.Button upload_image_btn;
        private System.Windows.Forms.Label checkin_address_label;
        private System.Windows.Forms.TextBox weixin_username_txt;
        private System.Windows.Forms.TextBox contact_telephone_txt;
        private System.Windows.Forms.TextBox contact_name_txt;
        private System.Windows.Forms.TextBox weixin_number_txt;
        private System.Windows.Forms.ComboBox checkin_address_combox;
        private System.Windows.Forms.PictureBox image_picturebox;
        private System.Windows.Forms.Button confirm_btn;
        private System.Windows.Forms.Label openid_label;
        private System.Windows.Forms.TextBox openId_txt;
        private System.Windows.Forms.Label active_label;
    }
}

