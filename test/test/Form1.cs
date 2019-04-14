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
    public partial class Form1 : Form
    {
   
        SynchronizationContext m_SyncContext = null;
    
        public Form1()
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
    }
}
