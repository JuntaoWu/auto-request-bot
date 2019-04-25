using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public string imagepath;
        public string base64Str;
        public AddUser()
        {
            InitializeComponent();
            user = new AddUserDAL();
            this.checkin_address_combox.DataSource = user.getCheckInAddressList();
            this.checkin_address_combox.DisplayMember = "Name";
            this.checkin_address_combox.ValueMember = "Address";
        }

        private void confirm_btn_Click(object sender, EventArgs e)
        {
            string selecteaddress = this.checkin_address_combox.SelectedValue.ToString();
            Location userlocation = getAddressLocation(selecteaddress);
            bool result = user.AddUser(userlocation, this.user_name_txt.Text, this.telephone_txt.Text, this.base64Str);
            if (result)
            {
                
                this.Close();
            }
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
                        this.imagepath = file.FileName;   //获得文件的绝对路径
                        string fileExtension = Path.GetExtension(file.FileName).Substring(1);
                        FileStream filestream = new FileStream(this.imagepath, FileMode.Open);
                        byte[] bt = new byte[filestream.Length];
                        //调用read读取方法
                        filestream.Read(bt, 0, bt.Length);
                        this.base64Str = "data:image/" + fileExtension + ";base64,"+ Convert.ToBase64String(bt);
                        filestream.Close();
                        this.image_picturebox.Load(this.imagepath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                
            }
        }

        private Location getAddressLocation(string address)
        {
            Location currentLocation = new Location();
            string url = $"http://api.map.baidu.com/geocoder/v2/?address={address}&output=json&ak=oLrWGIzoPEusmDqQrmG37OuUC7UE60uo";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            AddressUnity addressUnity = JsonConvert.DeserializeObject<AddressUnity>(retString);
            if (addressUnity.status == 0)
            {
                //string lng = float.Parse(jo.result.location.lng.ToString()).ToString("F2");
                //string lat = float.Parse(jo.result.location.lat.ToString()).ToString("F2");
                currentLocation.lng = addressUnity.result.location.lng;
                currentLocation.lat = addressUnity.result.location.lat;
            }
            else
            {
                currentLocation.lng = 0;
                currentLocation.lat = 0;
            }
            myStreamReader.Close();
            myResponseStream.Close();
            return currentLocation;
        }
    }
}
