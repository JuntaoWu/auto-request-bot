namespace test
{
    partial class Login
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
            this.user_name_txtbox = new System.Windows.Forms.TextBox();
            this.password_label = new System.Windows.Forms.Label();
            this.password_txtbox = new System.Windows.Forms.TextBox();
            this.login_btn = new System.Windows.Forms.Button();
            this.register_btn = new System.Windows.Forms.Button();
            this.display_message = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // user_name_label
            // 
            this.user_name_label.AutoSize = true;
            this.user_name_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.user_name_label.Location = new System.Drawing.Point(98, 187);
            this.user_name_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.user_name_label.Name = "user_name_label";
            this.user_name_label.Size = new System.Drawing.Size(131, 37);
            this.user_name_label.TabIndex = 0;
            this.user_name_label.Text = "用户名";
            // 
            // user_name_txtbox
            // 
            this.user_name_txtbox.Location = new System.Drawing.Point(255, 192);
            this.user_name_txtbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.user_name_txtbox.Name = "user_name_txtbox";
            this.user_name_txtbox.Size = new System.Drawing.Size(312, 31);
            this.user_name_txtbox.TabIndex = 1;
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.password_label.Location = new System.Drawing.Point(135, 263);
            this.password_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(93, 37);
            this.password_label.TabIndex = 2;
            this.password_label.Text = "密码";
            // 
            // password_txtbox
            // 
            this.password_txtbox.Location = new System.Drawing.Point(255, 270);
            this.password_txtbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.password_txtbox.Name = "password_txtbox";
            this.password_txtbox.PasswordChar = '*';
            this.password_txtbox.Size = new System.Drawing.Size(312, 31);
            this.password_txtbox.TabIndex = 3;
            // 
            // login_btn
            // 
            this.login_btn.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.login_btn.Location = new System.Drawing.Point(141, 418);
            this.login_btn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.login_btn.Name = "login_btn";
            this.login_btn.Size = new System.Drawing.Size(156, 68);
            this.login_btn.TabIndex = 4;
            this.login_btn.Text = "登录";
            this.login_btn.UseVisualStyleBackColor = true;
            this.login_btn.Click += new System.EventHandler(this.login_btn_click);
            // 
            // register_btn
            // 
            this.register_btn.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.register_btn.Location = new System.Drawing.Point(424, 418);
            this.register_btn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.register_btn.Name = "register_btn";
            this.register_btn.Size = new System.Drawing.Size(144, 68);
            this.register_btn.TabIndex = 5;
            this.register_btn.Text = "注册";
            this.register_btn.UseVisualStyleBackColor = true;
            this.register_btn.Click += new System.EventHandler(this.register_btn_click);
            // 
            // display_message
            // 
            this.display_message.AutoSize = true;
            this.display_message.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.display_message.Location = new System.Drawing.Point(141, 355);
            this.display_message.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.display_message.Name = "display_message";
            this.display_message.Size = new System.Drawing.Size(0, 25);
            this.display_message.TabIndex = 6;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 630);
            this.Controls.Add(this.display_message);
            this.Controls.Add(this.register_btn);
            this.Controls.Add(this.login_btn);
            this.Controls.Add(this.password_txtbox);
            this.Controls.Add(this.password_label);
            this.Controls.Add(this.user_name_txtbox);
            this.Controls.Add(this.user_name_label);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Login";
            this.Text = "登录";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Login_Closed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label user_name_label;
        private System.Windows.Forms.TextBox user_name_txtbox;
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.TextBox password_txtbox;
        private System.Windows.Forms.Button login_btn;
        private System.Windows.Forms.Button register_btn;
        private System.Windows.Forms.Label display_message;
    }
}