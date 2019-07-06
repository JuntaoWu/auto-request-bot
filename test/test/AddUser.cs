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
        public string openId;

        public AddUser(string ID)
        {
            InitializeComponent();

            this.currentID = ID;
            this.user = new AddUserDAL();
            this.InitializeData();
        }

        public AddUser()
        {
            InitializeComponent();
        }

        public async void InitializeData()
        {
            await this.BindDataMember();
        }

        private async Task BindDataMember()
        {
            this.checkin_address_combox.DataSource = await user.getCheckInAddressList();
            this.checkin_address_combox.DisplayMember = "text";
            this.checkin_address_combox.ValueMember = "value";

            MemberCheckIn data = await user.getOneMemberById(this.currentID);
            this.weixin_username_txt.Text = data.nickName;
            this.weixin_number_txt.Text = data.wechatId;
            this.contact_name_txt.Text = data.contactName;
            this.contact_telephone_txt.Text = data.telephone;
            this.checkin_address_combox.SelectedValue = data.locationId;
            this.openId = data.openId;

            this.image_picturebox.Image = await ImageLoader.LoadImage(data.avatarUrl);
            this.base64Str = GetImageBase64(this.image_picturebox.Image);
        }

        private string GetImageBase64(Image bImage)
        {
            System.IO.MemoryStream ms = new MemoryStream();
            bImage.Save(ms, ImageFormat.Jpeg);
            byte[] byteImage = ms.ToArray();
            var SigBase64 = Convert.ToBase64String(byteImage); // Get Base64
            return "data:image/" + "jpeg" + ";base64," + SigBase64;
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

        private async void commit_btn_Click(object sender, EventArgs e)
        {
            bool result = await user.UpdateUser(this.currentID, locationId: this.checkin_address_combox.SelectedValue.ToString(), nickName: this.weixin_username_txt.Text, wechatId: this.weixin_number_txt.Text, contactName: this.contact_name_txt.Text, telephone: this.contact_telephone_txt.Text, imagedata: this.base64Str, openId: this.openId);
            if (result)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
