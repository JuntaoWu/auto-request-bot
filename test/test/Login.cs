using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
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
            this.ProcessLogin();
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

        private void Password_txtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ProcessLogin();
            }
        }

        private async void ProcessLogin()
        {
            string username = this.user_name_txtbox.Text;
            string password = this.password_txtbox.Text;
            LoginDAL login = new LoginDAL();
            try
            {
                ResponseResult<TokenResponse> result = await login.Login(this.user_name_txtbox.Text, this.password_txtbox.Text);
                if (result.code == 0)
                {
                    AuthStore.Token = result.data.token;
                    this.DialogResult = DialogResult.OK;
                    this.exit = false;
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show(result.message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //this.display_message.Text = "登录失败";
                    //this.display_message.ForeColor = Color.Red;
                }
            }
            catch(HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (AggregateException ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
