using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace test
{
    public partial class Main : Form
    {
   
        SynchronizationContext m_SyncContext = null;
    
        public Main()
        {
            InitializeComponent();
            m_SyncContext = SynchronizationContext.Current;
        }   

        private void button1_Click(object sender, EventArgs e)
        {
            SingletonProxyServer.ServerStart();
            SingletonProxyServer.Instance.OnReceiveResponse += Instance_OnReceiveResponse;

        }

        private void Instance_OnReceiveResponse(object sender, EventArgs e)
        {
            if (e is MyCustomEventArge) {
                m_SyncContext.Post(SetTextSafePost, (e as MyCustomEventArge).text);
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

        private void button3_Click(object sender, EventArgs e)
        {
            this.textboxDisplay.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SingletonProxyServer.OperationType = OperationType.Register;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            new Login().ShowDialog();
            listView.View = View.Details;//设置视图  

            //添加列  
            listView.Columns.Add("本地路径", 150, HorizontalAlignment.Left);
            listView.Columns.Add("远程路径", 150, HorizontalAlignment.Left);
            listView.Columns.Add("上传状态", 80, HorizontalAlignment.Left);
            listView.Columns.Add("耗时", 80, HorizontalAlignment.Left);

            //添加行  
            var item = new ListViewItem();
            item.ImageIndex = 1;
            item.Text = "jiangshangfeng"; //本地路径  
            item.SubItems.Add("stefanjiang"); //远程路径  
            item.SubItems.Add("ok"); //执行状态  
            item.SubItems.Add("0.5"); //耗时统计  

            listView.BeginUpdate();
            listView.Items.Add(item);
            listView.Items[listView.Items.Count - 1].EnsureVisible();//滚动到最后  
            listView.EndUpdate();
        }
    }
}
