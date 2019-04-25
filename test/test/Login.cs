using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test.DAL;

namespace test
{
    public partial class Login : Form
    {
        public bool exit = true;
        public Login()
        {
            InitializeComponent();
        }

        private void login_btn_click(object sender, EventArgs e)
        {
            string username = this.user_name_txtbox.Text;
            string password = this.password_txtbox.Text;
            LoginDAL login = new LoginDAL();
            if (login.Login(username, password))
            {
                this.exit = false;
                this.Dispose();
            }
            else {

                this.display_message.Text = "登录失败";
                this.display_message.ForeColor = Color.Red;
            }
        }

        private void register_btn_click(object sender, EventArgs e)
        {
            var registerForm = new Register();
            registerForm.StartPosition = FormStartPosition.CenterScreen;
            registerForm.ShowDialog();
        }

        private void Login_Closed(object sender, FormClosedEventArgs e)
        {
            if (this.exit)
            {
                System.Environment.Exit(0);
            }
            
        }
    }
}
