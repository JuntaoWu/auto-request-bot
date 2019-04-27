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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using test.DAL;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace test
{
    public partial class Main : Form
    {

        private AddUserDAL user;
        public string imagepath;
        public string base64Str;
        SynchronizationContext m_SyncContext = null;

        public Main()
        {
            InitializeComponent();
            m_SyncContext = SynchronizationContext.Current;
            user = new AddUserDAL();
            this.checkin_address_combox.DataSource = user.getCheckInAddressList();
            this.checkin_address_combox.DisplayMember = "Name";
            this.checkin_address_combox.ValueMember = "Address";

            SingletonProxyServer.OperationType = OperationType.Stopped;
            SingletonProxyServer.ServerStart();
            SingletonProxyServer.Instance.OnReceiveResponse += Instance_OnReceiveResponse;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SingletonProxyServer.ServerStart();
            //SingletonProxyServer.Instance.OnReceiveResponse += Instance_OnReceiveResponse;

        }

        private void Instance_OnReceiveResponse(object sender, EventArgs e)
        {
            if (e is MyCustomEventArge)
            {
                var myevent = (e as MyCustomEventArge);
                if (myevent.type == OperationType.Checkin)
                {
                    m_SyncContext.Post(SetTextSafePost, (e as MyCustomEventArge).text);
                }
                else if (myevent.type == OperationType.Register)
                {
                    m_SyncContext.Post(SetReigsterAndOpenid, myevent.text);
                }
            }

        }



        private void button2_Click(object sender, EventArgs e)
        {
            SingletonProxyServer.ServerStop();
        }

        private void SetTextSafePost(object text)
        {
            this.textboxDisplay.Text += text.ToString() + '\n';

        }

        private void SetReigsterAndOpenid(object text)
        {
            this.openId_txt.Text = text.ToString();
            this.register_btn.Enabled = false;
            this.confirm_btn.Enabled = true;
            this.active_label.Text = "激活成功,可以提交";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textboxDisplay.Text = "";
        }

        private void register_btn_Click(object sender, EventArgs e)
        {
            SingletonProxyServer.OperationType = OperationType.Register;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Login loginform = new Login();
            loginform.StartPosition = FormStartPosition.CenterScreen;
            loginform.ShowDialog();

        }

        private void add_user_btn_Click(object sender, EventArgs e)
        {
            /*AddUser addUser = new AddUser();
            addUser.StartPosition = FormStartPosition.CenterScreen;
            addUser.ShowDialog();
            if (addUser.DialogResult == DialogResult.OK) {
            }*/
        }

        private void tabControl1_DrawItem_1(object sender, DrawItemEventArgs e)
        {
            SolidBrush _Brush = new SolidBrush(Color.Black);//单色画刷
            RectangleF _TabTextArea = (RectangleF)tabControl1.GetTabRect(e.Index);//绘制区域
            StringFormat _sf = new StringFormat();//封装文本布局格式信息
            _sf.LineAlignment = StringAlignment.Center;
            _sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(tabControl1.Controls[e.Index].Text, SystemInformation.MenuFont, _Brush, _TabTextArea, _sf);
        }

        private void confirm_btn_Click(object sender, EventArgs e)
        {
            string selecteaddress = this.checkin_address_combox.SelectedValue.ToString();
            Location userlocation = getAddressLocation(selecteaddress);
            bool result = user.AddUser(userlocation, this.weixin_username_txt.Text, this.weixin_number_txt.Text, this.contact_name_txt.Text, this.contact_telephone_txt.Text, this.base64Str);
            if (result)
            {
                this.weixin_username_txt.Text = String.Empty;
                this.weixin_number_txt.Text = String.Empty;
                this.contact_name_txt.Text = String.Empty;
                this.contact_telephone_txt.Text = String.Empty;
                this.checkin_address_combox.SelectedValue = String.Empty;
                this.openId_txt.Text = String.Empty;
                this.register_btn.Enabled = true;
                this.confirm_btn.Enabled = false;
                this.active_label.Text = "未激活";
                this.image_picturebox.Image = null;
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
                        using (FileStream filestream = new FileStream(this.imagepath, FileMode.Open))
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

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindData()
        {
            List<Member> memberlist = new List<Member>();
            memberlist.Add(new Member
            {
                ID = "000001",
                avatar = LoadImage("https://cn.bing.com/sa/simg/SharedSpriteDesktopRewards_022118.png"),
                weixin_uername = "stefnjiang",
                username = "jiangshangfeng",
                telephone = "12234234234",
                weixin_number = "23e234",
                status = "激活成功",
                registertiem = "2019-4-27"
            });
            this.member_list_grdaview.DataSource = memberlist;
        }

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

        private void member_list_grdaview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.member_list_grdaview.Columns[e.ColumnIndex].Name == "update")
            {
                Member currentmember = this.member_list_grdaview.Rows[e.RowIndex].DataBoundItem as Member;
            }
            else if (this.member_list_grdaview.Columns[e.ColumnIndex].Name == "delete") {
                Member currentmember = this.member_list_grdaview.Rows[e.RowIndex].DataBoundItem as Member;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Text == "会员列表") {
                this.BindData();
            }
        }
    }
}
