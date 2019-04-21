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
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void register_click(object sender, EventArgs e)
        {
            if (this.password_txtbox.Text != this.confirm_password_txtbox.Text)
            {
                this.display_error_message.Text = "两次输入的密码不一致,请重新输入";
                this.display_error_message.ForeColor = Color.Red;
            }
            else
            {
                RegisterDAL register = new RegisterDAL();
                if (register.Register(this.username_txtbox.Text, this.telephone_txtbox.Text, this.password_txtbox.Text)) {
                    this.Close();
                }
            }
        }
    }
}
