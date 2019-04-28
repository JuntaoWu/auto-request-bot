using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace test
{
    public enum OperationType
    {
        Stopped,
        Register,
        Checkin
        
    }

    public class MyCustomEventArge : EventArgs {
        public string text { get; set; }
        public OperationType type { get; set; }
    }

    public class SingletonProxyServer
    {
        // Fields
        private static SingletonProxyServer instance;
        private ProxyServer proxyServer;
        private ExplicitProxyEndPoint explicitEndPoint;

        public static OperationType OperationType = OperationType.Checkin;
        public event EventHandler OnReceiveResponse;
       
        // Constructor
        protected SingletonProxyServer()
        {
        }

        // Methods
        public static SingletonProxyServer Instance
        {
            get
            {
                // Uses "Lazy initialization"
                if (instance == null)
                {
                    instance = new SingletonProxyServer();
                    InitServer();
                }

                return instance;
            }
        }

        public static void InitServer()
        {
            Instance.proxyServer = new ProxyServer();

            //locally trust root certificate used by this proxy 
            Instance.proxyServer.CertificateManager.TrustRootCertificate(true);

            Instance.explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8765, true)
            {
            };

            Instance.proxyServer.AddEndPoint(Instance.explicitEndPoint);
            var transparentEndPoint = new TransparentProxyEndPoint(IPAddress.Any, 8766, true)
            {
                GenericCertificateName = "google.com"
            };
            Instance.proxyServer.AddEndPoint(transparentEndPoint);
            foreach (var endPoint in Instance.proxyServer.ProxyEndPoints)
            {

            }
        }

        public static void ServerStart()
        {

            Instance.proxyServer.Start();

            //Fired when a CONNECT request is received
            Instance.explicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;

            //Only explicit proxies can be set as system proxy!
            Instance.proxyServer.SetAsSystemHttpProxy(Instance.explicitEndPoint);
            Instance.proxyServer.SetAsSystemHttpsProxy(Instance.explicitEndPoint);

            Instance.proxyServer.BeforeRequest += OnRequest;
            Instance.proxyServer.BeforeResponse += OnResponse;
            Instance.proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            Instance.proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;
        }

        public static void ServerStop()
        {
            Instance.explicitEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
            Instance.proxyServer.BeforeRequest -= OnRequest;
            Instance.proxyServer.BeforeResponse -= OnResponse;
            Instance.proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
            Instance.proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;

            Instance.proxyServer.Stop();
        }

        private async static Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;

            if (hostname.Contains("dropbox.com"))
            {
                //Exclude Https addresses you don't want to proxy
                //Useful for clients that use certificate pinning
                //for example dropbox.com
                e.DecryptSsl = false;
            }
        }

        public async static Task OnRequest(object sender, SessionEventArgs e)
        {
            //Console.WriteLine(e.HttpClient.Request.Url);
            if (e.HttpClient.Request.Url.StartsWith("http://tms.ihxlife.com/tms/html/1_kqlr/sign.html"))
            {
                if (OperationType == OperationType.Checkin)
                {
                    Dictionary<string, string> my_dictionay = new Dictionary<string, string>();
                    var requsturl = new System.Uri(e.HttpClient.Request.Url);
                    var requestquery = requsturl.Query;
                    List<String> res = requestquery.Substring(1).Split('&').ToList();
                    res.ForEach((item) =>
                    {
                        my_dictionay[item.Split('=')[0]] = item.Split('=')[1];
                    });

                    Instance.sendRequst(my_dictionay, "104.07", "30.67");
                }
                else if(OperationType == OperationType.Register) {
                    Dictionary<string, string> my_dictionay = new Dictionary<string, string>();
                    var requsturl = new System.Uri(e.HttpClient.Request.Url);
                    var requestquery = requsturl.Query;
                    List<String> res = requestquery.Substring(1).Split('&').ToList();
                    res.ForEach((item) =>
                    {
                        my_dictionay[item.Split('=')[0]] = item.Split('=')[1];
                    });
                    string openId;
                    my_dictionay.TryGetValue("openid", out openId);
                    Instance.OnReceiveResponse(Instance, new MyCustomEventArge { text = openId, type = OperationType.Register });
                }
               
            }

            ////read request headers
            var requestHeaders = e.HttpClient.Request.Headers;

            var method = e.HttpClient.Request.Method.ToUpper();
            if ((method == "POST" || method == "PUT" || method == "PATCH"))
            {
                //Get/Set request body bytes
                byte[] bodyBytes = await e.GetRequestBody();
                e.SetRequestBody(bodyBytes);

                //Get/Set request body as string
                string bodyString = await e.GetRequestBodyAsString();
                e.SetRequestBodyString(bodyString);

                //store request 
                //so that you can find it from response handler 
                e.UserData = e.HttpClient.Request;
            }

            //To cancel a request with a custom HTML content
            //Filter URL
            if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("google.com"))
            {
                e.Ok("<!DOCTYPE html>" +
                      "<html><body><h1>" +
                      "Website Blocked" +
                      "</h1>" +
                      "<p>Blocked by titanium web proxy.</p>" +
                      "</body>" +
                      "</html>");
            }
            //Redirect example
            if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("wikipedia.org"))
            {
                e.Redirect("https://www.paypal.com");
            }
        }

        //Modify response
        public async static Task OnResponse(object sender, SessionEventArgs e)
        {
            //read response headers
            var responseHeaders = e.HttpClient.Response.Headers;

            //if (!e.ProxySession.Request.Host.Equals("medeczane.sgk.gov.tr")) return;
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            {
                if (e.HttpClient.Response.StatusCode == 200)
                {
                    if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        byte[] bodyBytes = await e.GetResponseBody();
                        e.SetResponseBody(bodyBytes);

                        string body = await e.GetResponseBodyAsString();
                        e.SetResponseBodyString(body);
                    }
                }
            }

            if (e.UserData != null)
            {
                //access request from UserData property where we stored it in RequestHandler
                var request = (Request)e.UserData;
            }

        }

        /// Allows overriding default certificate validation logic
        public static Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            //set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;

            return Task.FromResult(0);
        }

        /// Allows overriding default client certificate selection logic during mutual authentication
        public static Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            //set e.clientCertificate to override
            return Task.FromResult(0);
        }

        private void sendRequst(Dictionary<string, string> template, string longitude, string latitude)
        {
            string ur = $"{string.Format("0")}\"\",\"\"";
            string openId, userId, timestamp, nonce, trade_source, signature, qrcodeid;
            template.TryGetValue("openid", out openId);
            template.TryGetValue("userid", out userId);
            template.TryGetValue("timestamp", out timestamp);
            template.TryGetValue("nonce", out nonce);
            template.TryGetValue("trade_source", out trade_source);
            template.TryGetValue("signature", out signature);
            template.TryGetValue("attach", out qrcodeid);

            string customParams = $"{{\"openid\":\"{openId}\",\"userid\":\"{userId}\",\"timestamp\":\"{timestamp}\",\"nonce\":\"{nonce}\",\"trade_source\":\"{trade_source}\",\"signature\":\"{signature}\",\"qrcodeid\":\"{qrcodeid}\",\"attentype\":\"morning\",\"longitude\":{longitude},\"latitude\":{latitude},\"cacheflag\":\"0\"}}";
            //"openid":"","userid":"510129760","timestamp":"1555230703033","nonce":"b422fca3-6745-45fb-942e-3277e4c2872f","trade_source":"HXQYH","signature":"C5B39A04405C819AB045BD54A3376D59","qrcodeid":"00000000000000105723","attentype":"morning","longitude":104.07,"latitude":30.67,"cacheflag":"0"
            string url = $"http://kqapi.hxlife.com/tms/api/QRcodeSign?callbackparam=success_jsonpCallback&params={customParams}&_={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            //OnReceiveResponse(this, new MyCustomEventArge { text = retString,type = OperationType.Checkin });
            MemberCheckInSingletonService.updateMemberCheckInInformation(openId, CheckInStatus.Success);
        }
    }
}
