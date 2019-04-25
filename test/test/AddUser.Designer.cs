namespace test
{
    partial class AddUser
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
            this.user_name_label = new System.Windows.Forms.Label();
            this.telephone_label = new System.Windows.Forms.Label();
            this.checkin_address_label = new System.Windows.Forms.Label();
            this.user_name_txt = new System.Windows.Forms.TextBox();
            this.telephone_txt = new System.Windows.Forms.TextBox();
            this.checkin_address_combox = new System.Windows.Forms.ComboBox();
            this.confirm_btn = new System.Windows.Forms.Button();
            this.upload_image_btn = new System.Windows.Forms.Button();
            this.image_picturebox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.image_picturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // user_name_label
            // 
            this.user_name_label.AutoSize = true;
            this.user_name_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.user_name_label.Location = new System.Drawing.Point(80, 65);
            this.user_name_label.Name = "user_name_label";
            this.user_name_label.Size = new System.Drawing.Size(110, 24);
            this.user_name_label.TabIndex = 0;
            this.user_name_label.Text = "微信昵称";
            // 
            // telephone_label
            // 
            this.telephone_label.AutoSize = true;
            this.telephone_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.telephone_label.Location = new System.Drawing.Point(80, 126);
            this.telephone_label.Name = "telephone_label";
            this.telephone_label.Size = new System.Drawing.Size(110, 24);
            this.telephone_label.TabIndex = 1;
            this.telephone_label.Text = "电话号码";
            // 
            // checkin_address_label
            // 
            this.checkin_address_label.AutoSize = true;
            this.checkin_address_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.checkin_address_label.Location = new System.Drawing.Point(83, 193);
            this.checkin_address_label.Name = "checkin_address_label";
            this.checkin_address_label.Size = new System.Drawing.Size(110, 24);
            this.checkin_address_label.TabIndex = 2;
            this.checkin_address_label.Text = "打卡地址";
            // 
            // user_name_txt
            // 
            this.user_name_txt.Location = new System.Drawing.Point(222, 64);
            this.user_name_txt.Name = "user_name_txt";
            this.user_name_txt.Size = new System.Drawing.Size(255, 25);
            this.user_name_txt.TabIndex = 4;
            // 
            // telephone_txt
            // 
            this.telephone_txt.Location = new System.Drawing.Point(222, 124);
            this.telephone_txt.Name = "telephone_txt";
            this.telephone_txt.Size = new System.Drawing.Size(255, 25);
            this.telephone_txt.TabIndex = 5;
            // 
            // checkin_address_combox
            // 
            this.checkin_address_combox.FormattingEnabled = true;
            this.checkin_address_combox.ItemHeight = 15;
            this.checkin_address_combox.Location = new System.Drawing.Point(222, 193);
            this.checkin_address_combox.Name = "checkin_address_combox";
            this.checkin_address_combox.Size = new System.Drawing.Size(441, 23);
            this.checkin_address_combox.TabIndex = 7;
            // 
            // confirm_btn
            // 
            this.confirm_btn.Location = new System.Drawing.Point(222, 455);
            this.confirm_btn.Name = "confirm_btn";
            this.confirm_btn.Size = new System.Drawing.Size(124, 33);
            this.confirm_btn.TabIndex = 8;
            this.confirm_btn.Text = "添加用户";
            this.confirm_btn.UseVisualStyleBackColor = true;
            this.confirm_btn.Click += new System.EventHandler(this.confirm_btn_Click);
            // 
            // upload_image_btn
            // 
            this.upload_image_btn.Location = new System.Drawing.Point(87, 257);
            this.upload_image_btn.Name = "upload_image_btn";
            this.upload_image_btn.Size = new System.Drawing.Size(124, 33);
            this.upload_image_btn.TabIndex = 9;
            this.upload_image_btn.Text = "点击上传头像";
            this.upload_image_btn.UseVisualStyleBackColor = true;
            this.upload_image_btn.Click += new System.EventHandler(this.upload_image_btn_Click);
            // 
            // image_picturebox
            // 
            this.image_picturebox.Location = new System.Drawing.Point(222, 257);
            this.image_picturebox.Name = "image_picturebox";
            this.image_picturebox.Size = new System.Drawing.Size(441, 167);
            this.image_picturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.image_picturebox.TabIndex = 10;
            this.image_picturebox.TabStop = false;
            // 
            // AddUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 500);
            this.Controls.Add(this.image_picturebox);
            this.Controls.Add(this.upload_image_btn);
            this.Controls.Add(this.confirm_btn);
            this.Controls.Add(this.checkin_address_combox);
            this.Controls.Add(this.telephone_txt);
            this.Controls.Add(this.user_name_txt);
            this.Controls.Add(this.checkin_address_label);
            this.Controls.Add(this.telephone_label);
            this.Controls.Add(this.user_name_label);
            this.Name = "AddUser";
            this.Text = "AddUser";
            ((System.ComponentModel.ISupportInitialize)(this.image_picturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label user_name_label;
        private System.Windows.Forms.Label telephone_label;
        private System.Windows.Forms.Label checkin_address_label;
        private System.Windows.Forms.TextBox user_name_txt;
        private System.Windows.Forms.TextBox telephone_txt;
        private System.Windows.Forms.ComboBox checkin_address_combox;
        private System.Windows.Forms.Button confirm_btn;
        private System.Windows.Forms.Button upload_image_btn;
        private System.Windows.Forms.PictureBox image_picturebox;
    }
}