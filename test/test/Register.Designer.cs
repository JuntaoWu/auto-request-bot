namespace test
{
    partial class Register
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
            this.password_label = new System.Windows.Forms.Label();
            this.confirm_password_label = new System.Windows.Forms.Label();
            this.username_txtbox = new System.Windows.Forms.TextBox();
            this.telephone_txtbox = new System.Windows.Forms.TextBox();
            this.password_txtbox = new System.Windows.Forms.TextBox();
            this.confirm_password_txtbox = new System.Windows.Forms.TextBox();
            this.display_error_message = new System.Windows.Forms.Label();
            this.register_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // user_name_label
            // 
            this.user_name_label.AutoSize = true;
            this.user_name_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.user_name_label.Location = new System.Drawing.Point(97, 49);
            this.user_name_label.Name = "user_name_label";
            this.user_name_label.Size = new System.Drawing.Size(85, 24);
            this.user_name_label.TabIndex = 0;
            this.user_name_label.Text = "用户名";
            // 
            // telephone_label
            // 
            this.telephone_label.AutoSize = true;
            this.telephone_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.telephone_label.Location = new System.Drawing.Point(122, 105);
            this.telephone_label.Name = "telephone_label";
            this.telephone_label.Size = new System.Drawing.Size(60, 24);
            this.telephone_label.TabIndex = 1;
            this.telephone_label.Text = "电话";
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.password_label.Location = new System.Drawing.Point(122, 168);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(60, 24);
            this.password_label.TabIndex = 2;
            this.password_label.Text = "密码";
            // 
            // confirm_password_label
            // 
            this.confirm_password_label.AutoSize = true;
            this.confirm_password_label.Font = new System.Drawing.Font("SimSun", 13.8F, System.Drawing.FontStyle.Bold);
            this.confirm_password_label.Location = new System.Drawing.Point(22, 229);
            this.confirm_password_label.Name = "confirm_password_label";
            this.confirm_password_label.Size = new System.Drawing.Size(160, 24);
            this.confirm_password_label.TabIndex = 3;
            this.confirm_password_label.Text = "再次确认密码";
            // 
            // username_txtbox
            // 
            this.username_txtbox.Location = new System.Drawing.Point(206, 47);
            this.username_txtbox.Name = "username_txtbox";
            this.username_txtbox.Size = new System.Drawing.Size(205, 25);
            this.username_txtbox.TabIndex = 4;
            // 
            // telephone_txtbox
            // 
            this.telephone_txtbox.Location = new System.Drawing.Point(206, 103);
            this.telephone_txtbox.Name = "telephone_txtbox";
            this.telephone_txtbox.Size = new System.Drawing.Size(205, 25);
            this.telephone_txtbox.TabIndex = 5;
            // 
            // password_txtbox
            // 
            this.password_txtbox.Location = new System.Drawing.Point(206, 166);
            this.password_txtbox.Name = "password_txtbox";
            this.password_txtbox.PasswordChar = '*';
            this.password_txtbox.Size = new System.Drawing.Size(205, 25);
            this.password_txtbox.TabIndex = 6;
            // 
            // confirm_password_txtbox
            // 
            this.confirm_password_txtbox.Location = new System.Drawing.Point(206, 227);
            this.confirm_password_txtbox.Name = "confirm_password_txtbox";
            this.confirm_password_txtbox.PasswordChar = '*';
            this.confirm_password_txtbox.Size = new System.Drawing.Size(205, 25);
            this.confirm_password_txtbox.TabIndex = 7;
            // 
            // display_error_message
            // 
            this.display_error_message.AutoSize = true;
            this.display_error_message.Location = new System.Drawing.Point(26, 293);
            this.display_error_message.Name = "display_error_message";
            this.display_error_message.Size = new System.Drawing.Size(0, 15);
            this.display_error_message.TabIndex = 8;
            // 
            // register_btn
            // 
            this.register_btn.Location = new System.Drawing.Point(206, 310);
            this.register_btn.Name = "register_btn";
            this.register_btn.Size = new System.Drawing.Size(205, 42);
            this.register_btn.TabIndex = 9;
            this.register_btn.Text = "确认注册";
            this.register_btn.UseVisualStyleBackColor = true;
            this.register_btn.Click += new System.EventHandler(this.register_click);
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 394);
            this.Controls.Add(this.register_btn);
            this.Controls.Add(this.display_error_message);
            this.Controls.Add(this.confirm_password_txtbox);
            this.Controls.Add(this.password_txtbox);
            this.Controls.Add(this.telephone_txtbox);
            this.Controls.Add(this.username_txtbox);
            this.Controls.Add(this.confirm_password_label);
            this.Controls.Add(this.password_label);
            this.Controls.Add(this.telephone_label);
            this.Controls.Add(this.user_name_label);
            this.Name = "Register";
            this.Text = "Register";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label user_name_label;
        private System.Windows.Forms.Label telephone_label;
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.Label confirm_password_label;
        private System.Windows.Forms.TextBox username_txtbox;
        private System.Windows.Forms.TextBox telephone_txtbox;
        private System.Windows.Forms.TextBox password_txtbox;
        private System.Windows.Forms.TextBox confirm_password_txtbox;
        private System.Windows.Forms.Label display_error_message;
        private System.Windows.Forms.Button register_btn;
    }
}