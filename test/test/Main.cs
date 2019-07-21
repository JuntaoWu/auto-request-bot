using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using test.DAL;

namespace test
{
    public partial class Main : Form
    {

        private AddUserDAL user;
        public string base64Str;
        SynchronizationContext m_SyncContext = null;
        private Random random = new Random();

        private int lowerTimeSpan = 3000;
        private int higherTimeSpan = 5000;

        public bool Initialized { get; set; }

        public event EventHandler<CustomGridViewImageEventArgs> OnImageLoaded = (object sender, CustomGridViewImageEventArgs e) => { };

        public CheckInMode Mode { get; set; }

        public Main()
        {
            InitializeComponent();
            m_SyncContext = SynchronizationContext.Current;

            //开始启动监听服务，并注册事件响应函数
            SingletonProxyServer.OperationType = OperationType.Stopped;

            SingletonProxyServer.Instance.OnReceiveResponse += Instance_OnReceiveResponse;

            MemberCheckInSingletonService.Instance.OnReceiveCheckInResponse += Instance_OnReceiveCheckInResponse;

            SocketService.Instance.OnMessage += Instance_OnMessage;
            SocketService.Instance.OnError += Instance_OnError;

            FaceSyncService.Instance.OnSynced += Instance_OnSynced;

            this.OnImageLoaded += (object sender, CustomGridViewImageEventArgs e) =>
            {
                Console.WriteLine(DateTime.Now);
                this.m_SyncContext.Post((data) =>
                {
                    if (e.DataGridView != null)
                    {
                        e.DataGridView.Refresh();
                    }
                    else
                    {
                        this.member_list_grdaview.Refresh();
                        this.wait_checkin_datagrid.Refresh();
                        this.success_checkin_datagrid.Refresh();
                        this.error_checkin_datagrid.Refresh();
                    }
                }, null);
            };
        }

        private void Instance_OnError(object sender, CustomEventArgs e)
        {
            this.m_SyncContext.Post((context) =>
            {
                this.toolStripStatusLabel1.Text = e.Message;
            }, this);
        }

        private void Instance_OnSynced(object sender, CustomEventArgs e)
        {
            this.m_SyncContext.Post((data) =>
            {
                this.toolStripProgressBar1.Value = 100;
                this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                this.toolStripStatusLabel1.Text = "用户脸同步完成";

                this.button1.Enabled = true;
                this.button2.Enabled = true;
            }, e.Data);
        }

        //窗体加载事件
        private void Main_Load(object sender, EventArgs e)
        {
            Login loginform = new Login();
            loginform.StartPosition = FormStartPosition.CenterScreen;
            DialogResult dialogResult = loginform.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                this.Init();
            }
        }

        private async void Init()
        {
            this.checkin_type_combox.SelectedIndex = 0;
            user = new AddUserDAL();
            await this.BindCheckInAddressDataMember();
            await MemberCheckInSingletonService.getAllMemberCheckInOnToday(this.getCheckInType(), this.GetCheckInLocation());
            await FaceSyncService.Instance.Run();
            this.Initialized = true;
        }


        //监听打开信息更新事件
        private void Instance_OnReceiveCheckInResponse(object sender, EventArgs e)
        {
            if (e is CustomCheckInEventArge)
            {
                var CustomCheckIn = (e as CustomCheckInEventArge);
                m_SyncContext.Post((argument) =>
                {
                    UpdateCheckInDataGrid(argument);
                }, CustomCheckIn.currentdata);
            }
        }

        //监听服务事件响应函数
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

        //设置打开是否成功消息
        private void SetTextSafePost(object text)
        {
            //this.textboxDisplay.Text += text.ToString() + '\n';

        }

        //设置激活和获取OpenId
        private void SetReigsterAndOpenid(object text)
        {
            /*this.openId_txt.Text = text.ToString();
            this.register_btn.Enabled = false;
            this.confirm_btn.Enabled = true;
            this.active_label.Text = "激活成功,可以提交";*/
        }

        //根据用户地址获取经纬度
        private async Task<Location> getAddressLocation(string address)
        {
            Location currentLocation = new Location();
            string url = $"http://api.map.baidu.com/geocoder/v2/?address={address}&output=json&ak=oLrWGIzoPEusmDqQrmG37OuUC7UE60uo";
            string retString = await HttpUtil.Request(url);

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
            return currentLocation;
        }

        //图片上传
        private void upload_image_btn_Click(object sender, EventArgs e)
        {
            /*OpenFileDialog file = new OpenFileDialog();
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

            }*/
        }

        //设置监听状态为Register
        private void register_btn_Click(object sender, EventArgs e)
        {
            SingletonProxyServer.OperationType = OperationType.Register;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C checkin";
            process.StartInfo = startInfo;
            process.Start();
        }

        //提交用户信息
        private void confirm_btn_Click(object sender, EventArgs e)
        {
            /*string selecteaddress = this.checkin_address_combox.SelectedValue.ToString();
            // Location userlocation = getAddressLocation(selecteaddress);
            ResponseResult<MemberCheckIn> result = await user.AddUser(selecteaddress, this.weixin_username_txt.Text, this.weixin_number_txt.Text, this.contact_name_txt.Text, this.contact_telephone_txt.Text, this.base64Str, this.openId_txt.Text);
            if (result.code == 0)
            {
                this.weixin_username_txt.Text = String.Empty;
                this.weixin_number_txt.Text = String.Empty;
                this.contact_name_txt.Text = String.Empty;
                this.contact_telephone_txt.Text = String.Empty;
                this.checkin_address_combox.SelectedIndex = 0;
                this.openId_txt.Text = String.Empty;
                this.register_btn.Enabled = true;
                this.confirm_btn.Enabled = false;
                this.active_label.Text = "未激活";
                this.image_picturebox.Image = null;
                SingletonProxyServer.OperationType = OperationType.Checkin;
            }
            else
            {
                MessageBox.Show(result.message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        //左导航栏样式更改
        private void tabControl1_DrawItem_1(object sender, DrawItemEventArgs e)
        {
            SolidBrush _Brush = new SolidBrush(Color.Black);//单色画刷
            RectangleF _TabTextArea = (RectangleF)tabControl1.GetTabRect(e.Index);//绘制区域
            StringFormat _sf = new StringFormat();//封装文本布局格式信息
            _sf.LineAlignment = StringAlignment.Center;
            _sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(tabControl1.Controls[e.Index].Text, SystemInformation.MenuFont, _Brush, _TabTextArea, _sf);
        }

        //左导航栏选择
        private async void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }
            if (e.TabPage.Text == "会员列表")
            {
                await this.BindDataMemberList();
            }
            /*else if (e.TabPage.Text == "会员注册")
            {
                this.checkin_address_combox.DataSource = await user.getCheckInAddressList();
                this.checkin_address_combox.DisplayMember = "text";
                this.checkin_address_combox.ValueMember = "value";
                this.checkin_address_combox.SelectedIndex = 0;
            }*/
            else if (e.TabPage.Text == "打卡管理")
            {
                await MemberCheckInSingletonService.getAllMemberCheckInOnToday(this.getCheckInType(), this.GetCheckInLocation());
            }
        }

        private async Task BindCheckInAddressDataMember()
        {
            this.checkin_address_combox.DataSource = await user.getCheckInAddressList();
            this.checkin_address_combox.DisplayMember = "text";
            this.checkin_address_combox.ValueMember = "value";

            this.checkin_address_combox.SelectedIndex = 0;
        }



        /// <summary>
        /// 绑定会员列表数据数据
        /// </summary>
        private async Task BindDataMemberList()
        {
            var defaultImage = await ImageLoader.LoadImage("");

            var result = await user.getAllMemberList();

            List<Member> memberlist = result.AsParallel().Select((m) =>
            {
                return new Member
                {
                    ID = m._id,
                    avatarUrl = m.avatarUrl,
                    avatar = defaultImage,
                    weixin_uername = m.nickName,
                    username = m.contactName,
                    telephone = m.telephone,
                    weixin_number = m.wechatId,
                    status = MappingStatus(m.status),
                    registertime = m.registerTime,
                };
            }).ToList();

            var allTasks = memberlist.AsParallel().Select(async member =>
            {
                member.avatar = await ImageLoader.LoadImage(member.avatarUrl);
                return member.avatar;
            });

            this.member_list_grdaview.DataSource = memberlist;

            await Task.WhenAll(allTasks);
            OnImageLoaded(this, new CustomGridViewImageEventArgs() { DataGridView = this.member_list_grdaview });
        }

        /// <summary>
        /// 会员列表编辑和删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void member_list_grdaview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                if (this.member_list_grdaview.Columns[e.ColumnIndex].Name == "update")
                {
                    Member currentmember = this.member_list_grdaview.Rows[e.RowIndex].DataBoundItem as Member;
                    AddUser updateform = new AddUser(currentmember.ID);
                    updateform.StartPosition = FormStartPosition.CenterScreen;
                    updateform.ShowDialog();
                    if (updateform.DialogResult == DialogResult.OK)
                    {
                        await this.BindDataMemberList();
                    }
                }
                else if (this.member_list_grdaview.Columns[e.ColumnIndex].Name == "delete")
                {
                    Member currentmember = this.member_list_grdaview.Rows[e.RowIndex].DataBoundItem as Member;

                    if (MessageBox.Show("确定删除用戶: " + currentmember.weixin_uername + " ?", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        await this.user.DeleteUser(currentmember.ID);
                        await this.BindDataMemberList();
                    }
                }
            }
        }

        /// <summary>
        /// 当CheckIn完成之后更新打卡管理全部打卡列表
        /// </summary>
        /// <param name="data"></param>
        private async void UpdateCheckInDataGrid(object data)
        {
            List<MemberCheckIn> list = data as List<MemberCheckIn>;

            this.wait_checkin_datagrid.DataSource = await ConstructMember(list, CheckInStatus.Waiting);

            this.success_checkin_datagrid.DataSource = await ConstructMember(list, CheckInStatus.Success);

            this.error_checkin_datagrid.DataSource = await ConstructMember(list, CheckInStatus.Error);

            OnImageLoaded(this, new CustomGridViewImageEventArgs());
        }

        private async Task<List<Member>> ConstructMember(List<MemberCheckIn> list, CheckInStatus status)
        {
            var defaultImage = await ImageLoader.LoadImage("");
            var waiting = list.Where((a) => { return a.status == status; }).ToList().Select((m) =>
            {
                return new Member
                {
                    ID = m._id,
                    avatarUrl = m.avatarUrl,
                    weixin_uername = m.nickName,
                    username = m.contactName,
                    telephone = m.telephone,
                    weixin_number = m.wechatId,
                    status = MappingStatus(m.status),
                    checkintime = m.checkInTime?.ToString(),
                    message = m.message,
                    IsChecked = m.needChecked != NeeChecked.NoNeed,
                    avatar = defaultImage,
                };
            }).ToList();

            var parallel = waiting.AsParallel().Select(async member =>
            {
                member.avatar = await ImageLoader.LoadImage(member.avatarUrl);
                //OnImageLoaded(this, new EventArgs());
                return member.avatar;
            });

            await Task.WhenAll(parallel);

            return waiting;
        }

        private string MappingStatus(CheckInStatus status)
        {
            switch (status)
            {
                case CheckInStatus.Waiting:
                    return "等待打卡";
                case CheckInStatus.Success:
                    return "已完成";
                case CheckInStatus.Error:
                    return "未完成";
                case CheckInStatus.UnActive:
                    return "未激活";
                case CheckInStatus.Actived:
                    return "已激活";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 启动打卡程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_chekcin_btn_Click(object sender, EventArgs e)
        {
            SingletonProxyServer.OperationType = OperationType.Checkin;
            SingletonProxyServer.ServerStart();
            //this.start_chekcin_btn.Enabled = false;
            //this.stop_checkin_btn.Enabled = true;
        }

        /// <summary>
        /// 停止打卡程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stop_checkin_btn_Click(object sender, EventArgs e)
        {
            SingletonProxyServer.OperationType = OperationType.Stopped;
            SingletonProxyServer.ServerStop();
            //this.start_chekcin_btn.Enabled = true;
            //this.stop_checkin_btn.Enabled = false;
        }

        /// <summary>
        /// 窗体关闭时停止打卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            SingletonProxyServer.ServerStop();
        }

        /// <summary>
        /// 获取打卡类型
        /// </summary>
        /// <returns></returns>
        private CheckInType getCheckInType()
        {
            switch (this.checkin_type_combox.Text)
            {
                case "上班打卡":
                    return CheckInType.CheckIn;
                case "下班打卡":
                    return CheckInType.CheckOut;
                default:
                    return CheckInType.CheckIn;
            }
        }

        private CheckInAddress GetCheckInLocation()
        {
            if (this.checkin_address_combox.SelectedValue is CheckInAddress)
            {
                var checkInAddress = this.checkin_address_combox.SelectedValue as CheckInAddress;
                return checkInAddress;
            }
            else if (this.checkin_address_combox.SelectedValue is string)
            {
                return AddUserDAL.AddressList.FirstOrDefault(item => item.value == this.checkin_address_combox.SelectedValue.ToString());
            }
            return null;
        }

        /// <summary>
        /// 打卡类型变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void checkin_type_combox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }

            await MemberCheckInSingletonService.getAllMemberCheckInOnToday(this.getCheckInType(), this.GetCheckInLocation());
        }

        /// <summary>
        /// 批量打卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            int lowerTimeSpanSeconds;
            int higherTimeSpanSeconds;
            try
            {
                lowerTimeSpanSeconds = Convert.ToInt32(this.textBox1.Text);
                higherTimeSpanSeconds = Convert.ToInt32(this.textBox2.Text);
                if (lowerTimeSpan != 0 && higherTimeSpan != 0)
                {
                    this.lowerTimeSpan = lowerTimeSpanSeconds * 1000;
                    this.higherTimeSpan = higherTimeSpanSeconds * 1000;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Format error");
            }

            this.lowerTimeSpan = Convert.ToInt32(this.textBox1.Text);
            var todaySeparator = DateTime.Today;
            todaySeparator = todaySeparator.AddHours(10);
            todaySeparator = todaySeparator.AddMinutes(30);

            var systemCheckInType = DateTime.Now >= todaySeparator ? CheckInType.CheckOut : CheckInType.CheckIn;

            if (getCheckInType() != systemCheckInType)
            {
                MessageBox.Show($"请选择正确的打卡类型, 当前时间: {DateTime.Now}");
                return;
            }

            var checkInUrl = GetCheckInLocation()?.url;

            Mode = CheckInMode.Batch;
            List<string> checkitems = new List<string>();
            MemberCheckInSingletonService.Instance.checkedInIds = new HashSet<string>();
            MemberCheckInSingletonService.Instance.needFaceIds = new HashSet<string>();

            for (int i = 0; i < this.wait_checkin_datagrid.RowCount; i++)
            {
                if ((bool)this.wait_checkin_datagrid.Rows[i].Cells[0].Value)
                {
                    Member member = (Member)this.wait_checkin_datagrid.Rows[i].DataBoundItem;
                    checkitems.Add(member.ID);
                }
            }
            if (checkitems.Count > 0)
            {
                Task.Run(async () =>
                {
                    this.m_SyncContext.Post((context) =>
                    {
                        this.toolStripStatusLabel1.Text = "正在更新需要打卡人员...";
                        this.toolStripProgressBar1.Value = 0;
                    }, this);

                    bool result = await MemberCheckInSingletonService.updateNeedCheckIn(checkitems);

                    this.m_SyncContext.Post((context) =>
                    {
                        this.toolStripProgressBar1.Value = 100;
                    }, this);

                    if (!result)
                    {
                        this.m_SyncContext.Post((context) =>
                        {
                            this.toolStripStatusLabel1.Text = "更新需要打卡人员失败";
                        }, this);

                        return;
                    }

                    MemberCheckInSingletonService.Instance.membercheckinlist.ForEach((item) =>
                    {
                        if (checkitems.FirstOrDefault(a => a == item._id) != null)
                        {
                            item.needChecked = NeeChecked.Need;
                        }
                        else
                        {
                            item.needChecked = NeeChecked.NoNeed;
                        }
                    });

                    this.m_SyncContext.Post((context) =>
                    {
                        List<MemberCheckIn> data = context as List<MemberCheckIn>;
                        var total = data.Count(item => item.status == CheckInStatus.Waiting);
                        this.toolStripProgressBar1.Step = 100 / total;
                        this.toolStripProgressBar1.Value = 0;
                        this.toolStripStatusLabel1.Text = $"[{total}条记录]准备中...";
                    }, MemberCheckInSingletonService.Instance.membercheckinlist);

                    CheckInSingleMember(checkInUrl);
                });
            }
            else
            {
                MemberCheckInSingletonService.Instance.membercheckinlist.ForEach((item) =>
                {
                    item.needChecked = NeeChecked.NoNeed;
                });
            }
        }

        private static void SwitchCheckInMember()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "switch.exe";

            process.StartInfo = startInfo;
            process.Start();
        }

        private void CheckInSingleMember(string checkInUrl)
        {
            if (string.IsNullOrWhiteSpace(checkInUrl))
            {
                MessageBox.Show("请选择打卡地址");
                return;
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "checkin.exe";
            startInfo.Arguments = $"{checkInUrl} 1";

            process.StartInfo = startInfo;
            process.Start();
        }

        private void Instance_OnMessage(object sender, CustomEventArgs e)
        {
            Console.WriteLine("Handling SocketService.OnMessage................................");
            Console.WriteLine(JsonConvert.SerializeObject(e.Data));
            //MessageBox.Show(e.Data.op.ToString());
            //this.m_SyncContext.Post((data) =>
            //{
            //    MessageBox.Show(e.Data.op.ToString());
            //}, null);

            this.m_SyncContext.Post((data) =>
            {
                var checkInUrl = GetCheckInLocation()?.url;

                switch (e.Data.op)
                {
                    case SocketOp.ACK:
                    case SocketOp.PLAIN:
                        this.toolStripProgressBar1.Value = 0;
                        this.toolStripStatusLabelPercent.Text = "";
                        this.toolStripStatusLabel1.Text = $"[{e.Data.data.message}]";
                        break;
                    case SocketOp.CHECK_IN_CREATED:
                        MemberCheckInSingletonService.createNewMemberCheckInformation(e.Data.data);
                        this.toolStripProgressBar1.Value = 100;
                        this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                        this.toolStripStatusLabel1.Text = $"[{e.Data.data.name}]绑定完成";
                        break;
                    case SocketOp.CHECK_IN_STARTED:
                        this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                        this.toolStripStatusLabel1.Text = $"[{e.Data.data.name}]开始打卡";
                        break;
                    case SocketOp.CHECK_IN_SKIP:
                        if (Mode == CheckInMode.Batch && IsSomeoneWaiting(e.Data.data.id))
                        {
                            this.toolStripProgressBar1.ProgressBar.PerformStep();
                            this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                            this.toolStripStatusLabel1.Text = $"[{e.Data.data.name}]跳过";
                            SwitchCheckInMember();
                            var sleepMS = random.Next(lowerTimeSpan, higherTimeSpan);
                            Thread.Sleep(sleepMS);
                            CheckInSingleMember(checkInUrl);
                        }
                        else if (Mode == CheckInMode.Register)
                        {
                            this.toolStripProgressBar1.Value = 100;
                            this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                            this.toolStripStatusLabel1.Text = $"[{e.Data.data.name}]重复绑定";
                        }
                        else
                        {
                            this.toolStripProgressBar1.Value = 100;
                            this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                            this.toolStripStatusLabel1.Text = "当前打卡完成";
                        }
                        break;
                    case SocketOp.CHECK_IN_UPDATED:
                        Task.Run(async () =>
                        {
                            await MemberCheckInSingletonService.updateMemberCheckInInformation(e.Data.data);
                            if (Mode == CheckInMode.Register)
                            {
                                this.m_SyncContext.Post(context =>
                                {
                                    this.toolStripProgressBar1.Value = 100;
                                    this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                                    this.toolStripStatusLabel1.Text = $"[{e.Data.data.name}]完成注册";
                                }, this);
                            }
                            else if (Mode == CheckInMode.Batch && IsSomeoneWaiting(e.Data.data.id))
                            {
                                this.m_SyncContext.Post(context =>
                                {
                                    this.toolStripProgressBar1.PerformStep();
                                    this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                                    this.toolStripStatusLabel1.Text = $"[{e.Data.data.name}]完成打卡";
                                }, this);
                                SwitchCheckInMember();
                                var sleepMS = random.Next(lowerTimeSpan, higherTimeSpan);
                                Thread.Sleep(sleepMS);
                                CheckInSingleMember(checkInUrl);
                            }
                            else if (NeedFace(e.Data.data.id))
                            {
                                this.m_SyncContext.Post(context =>
                                {
                                    this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                                    this.toolStripStatusLabel1.Text = $"[{e.Data.data.name}]开始打脸";
                                }, this);
                                if (MemberCheckInSingletonService.Instance.checkedInIds.Contains(e.Data.data.id))
                                {
                                    MemberCheckInSingletonService.Instance.checkedInIds.Remove(e.Data.data.id);
                                }
                                MemberCheckIn memberCheckIn = MemberCheckInSingletonService.Instance.membercheckinlist.SingleOrDefault(m => m._id == e.Data.data.id);
                                MonitorFaceDialog(memberCheckIn.faceList);
                            }
                            else
                            {
                                this.m_SyncContext.Post(context =>
                                {
                                    this.toolStripProgressBar1.Value = 100;
                                    this.toolStripStatusLabelPercent.Text = $"{this.toolStripProgressBar1.Value}%";
                                    this.toolStripStatusLabel1.Text = $"当前打卡完成";
                                }, this);
                            }
                        });
                        break;
                }
            }, this);
        }

        private bool IsCheckInAgain(string id)
        {
            if (MemberCheckInSingletonService.Instance.checkedInIds.Contains(id))
            {
                MemberCheckInSingletonService.Instance.checkedInIds.Clear();
                return true;
            }
            else
            {
                MemberCheckInSingletonService.Instance.checkedInIds.Add(id);
                return false;
            }
        }

        private bool IsNeedFaceAgain(string id)
        {
            if (MemberCheckInSingletonService.Instance.needFaceIds.Contains(id))
            {
                MemberCheckInSingletonService.Instance.needFaceIds.Clear();
                return true;
            }
            else
            {
                MemberCheckInSingletonService.Instance.needFaceIds.Add(id);
                return false;
            }
        }

        private bool IsSomeoneWaiting(string id)
        {
            if (IsCheckInAgain(id))
            {
                return false;
            }
            MemberCheckIn memberCheckIn = MemberCheckInSingletonService.Instance.membercheckinlist.SingleOrDefault(m => m._id == id);

            return (memberCheckIn == null || memberCheckIn.result != "needface") && MemberCheckInSingletonService.Instance.membercheckinlist.Count(m => m.status == CheckInStatus.Waiting && m.needChecked == NeeChecked.Need) > 0;
        }

        private bool NeedFace(string id)
        {
            if (IsNeedFaceAgain(id))
            {
                return false;
            }
            MemberCheckIn memberCheckIn = MemberCheckInSingletonService.Instance.membercheckinlist.SingleOrDefault(m => m._id == id);
            return memberCheckIn.result == "needface";
        }

        private void MonitorFaceDialog(List<string> uris)
        {
            var path = "";
            if (uris == null || uris.Count == 0)
            {
                this.m_SyncContext.Post(context =>
                {
                    this.toolStripStatusLabel1.Text = "人脸列表为空, 请关闭打开文件对话框或自行选择文件";
                }, this);
                path = $"{Settings1.Default.RootFolder}";
            }
            else
            {
                string uri = uris[random.Next(0, uris.Count - 1)];
                uri = Regex.Replace(uri, "/", "\\");
                path = $"{Settings1.Default.RootFolder}{uri}";
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "file.exe";
            startInfo.Arguments = path.ToLower();

            process.StartInfo = startInfo;
            process.Start();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Mode = CheckInMode.Single;
            var checkInUrl = this.GetCheckInLocation()?.url;
            CheckInSingleMember(checkInUrl);
        }

        private void wait_checkin_datagrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)//如果单击列表头，全选．
            {
                int i;
                for (i = 0; i < this.wait_checkin_datagrid.RowCount; i++)
                {
                    this.wait_checkin_datagrid.Rows[i].Cells[0].Value = !(bool)this.wait_checkin_datagrid.Rows[i].Cells[0].Value;
                }
            }
        }

        // 重置打卡
        private void button3_Click(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }

            List<string> resetCheckInItems = new List<string>();
            for (int i = 0; i < this.error_checkin_datagrid.RowCount; i++)
            {
                if ((bool)this.error_checkin_datagrid.Rows[i].Cells[0].Value)
                {
                    Member member = (Member)this.error_checkin_datagrid.Rows[i].DataBoundItem;
                    resetCheckInItems.Add(member.ID);

                }
            }

            if (resetCheckInItems.Count > 0)
            {
                var currentCheckInType = this.getCheckInType();
                var checkInAddress = this.GetCheckInLocation();

                Task.Run(async () =>
                {
                    bool result = await MemberCheckInSingletonService.resetErrorCheckIn(resetCheckInItems);

                    if (!result)
                    {
                        MessageBox.Show("重置打卡失败.");
                        return;
                    }

                    try
                    {
                        await MemberCheckInSingletonService.getAllMemberCheckInOnToday(currentCheckInType, checkInAddress);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw ex;
                    }

                });
            }
            else
            {
                MessageBox.Show("请选择所需要重置打卡的人员");
            }
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4_Click(object sender, EventArgs e)
        {
            Mode = CheckInMode.Register;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "checkin.exe";
            startInfo.Arguments = $"{Constant.Host}/api/member/authorize?state=admin 1";

            process.StartInfo = startInfo;
            process.Start();
        }

        private async void Checkin_address_combox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!this.Initialized)
            {
                return;
            }

            await MemberCheckInSingletonService.getAllMemberCheckInOnToday(this.getCheckInType(), this.GetCheckInLocation());
        }
    }

}
