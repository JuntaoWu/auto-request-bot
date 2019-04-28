using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test.DAL;

namespace test
{
    public partial class AddUser : Form
    {
        private AddUserDAL user;
        public string base64Str;
        public string currentID;
        public AddUser(string ID) {
            this.currentID = ID;
            InitializeComponent();
            user = new AddUserDAL();
            this.BindDataMember();

        }
        public AddUser()
        {
            InitializeComponent();

        }

        
        private void BindDataMember() {

            this.checkin_address_combox.DataSource = user.getCheckInAddressList();
            this.checkin_address_combox.DisplayMember = "Name";
            this.checkin_address_combox.ValueMember = "Address";

            MemberCheckIn data = user.getOneMemberById(this.currentID);
            this.weixin_username_txt.Text = data.weixin_uername;
            this.weixin_number_txt.Text = data.weixin_number;
            this.contact_name_txt.Text = data.username;
            this.contact_telephone_txt.Text = data.telephone;
            this.checkin_address_combox.SelectedValue = data.checkin_addressId;
            this.image_picturebox.Image = this.LoadImage(data.avatarurl);            
        }

        // <summary>
        /// 根据URL生成Image对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Image LoadImage(string url)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream = response.GetResponseStream();
            Bitmap bmp = new Bitmap(responseStream);
            System.IO.MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            var SigBase64 = Convert.ToBase64String(byteImage); // Get Base64
            responseStream.Dispose();
            return bmp;
        }

        private void upload_image_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            file.FilterIndex = 1;
            if (file.ShowDialog() == DialogResult.OK)
            {
                if (file.FileName != string.Empty)
                {
                    try
                    {
                        string imagepath = file.FileName;   //获得文件的绝对路径
                        string fileExtension = Path.GetExtension(file.FileName).Substring(1);
                        using (FileStream filestream = new FileStream(imagepath, FileMode.Open))
                        {
                            byte[] bt = new byte[filestream.Length];
                            //调用read读取方法
                            filestream.Read(bt, 0, bt.Length);
                            this.base64Str = "data:image/" + fileExtension + ";base64," + Convert.ToBase64String(bt);
                            this.image_picturebox.Image = new Bitmap(filestream);
                            filestream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
        }

        private void commit_btn_Click(object sender, EventArgs e)
        {
            bool result = user.UpdateUser(this.currentID, this.weixin_username_txt.Text, this.weixin_number_txt.Text, this.contact_name_txt.Text, this.contact_telephone_txt.Text, this.checkin_address_combox.SelectedValue.ToString(), this.base64Str);
            if (result) {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
