using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace test
{
    public class CustomEventArgs : EventArgs
    {
        public MessageBlock Data { get; set; }
    }

    public class SocketService
    {
        private static object consoleLock = new object();
        private const int sendChunkSize = 1024;
        private const int receiveChunkSize = 1024;
        private const bool verbose = true;
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000);

        private static SocketService instance;

        public event EventHandler<CustomEventArgs> OnMessage;

        // Constructor
        protected SocketService()
        {
            
        }

        // Methods
        public static SocketService Instance
        {
            get
            {
                // Uses "Lazy initialization"
                if (instance == null)
                {
                    instance = new SocketService();
                    Task.Run(() =>
                    {
                        instance.Init().Wait();
                    });
                }

                return instance;
            }
        }

        private async Task Init()
        {
            using (var ws = new ClientWebSocket())
            {
                await ws.ConnectAsync(new Uri(Constant.WebSocketEndpoint), cancellationToken: CancellationToken.None);
                await Task.WhenAll(Receive(ws), Send(ws));
            }

            //ClientWebSocket ws;
            //try
            //{

            //    ws = new ClientWebSocket();
            //    await ws.ConnectAsync(new Uri(Constant.WebSocketEndpoint), cancellationToken: CancellationToken.None);
            //    await Task.WhenAll(Receive(ws), Send(ws));
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Exception: {0}", ex);
            //}
            //finally
            //{
            //    if (ws != null)
            //        ws.Dispose();
            //    Console.WriteLine();

            //    //lock (consoleLock)
            //    //{
            //    //    Console.ForegroundColor = ConsoleColor.Red;
            //    //    Console.WriteLine("WebSocket closed.");
            //    //    Console.ResetColor();
            //    //}
            //}
        }

        static UTF8Encoding encoder = new UTF8Encoding();

        private static async Task Send(ClientWebSocket webSocket)
        {

            //byte[] buffer = encoder.GetBytes("{\"op\":\"blocks_sub\"}"); //"{\"op\":\"unconfirmed_sub\"}");
            byte[] buffer = encoder.GetBytes("{\"op\":\"unconfirmed_sub\"}");
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            while (webSocket.State == WebSocketState.Open)
            {
                //LogStatus(false, buffer, buffer.Length);
                await Task.Delay(delay);
            }
        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[receiveChunkSize];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    LogStatus(true, buffer, result.Count);
                    string jsonResult = UTF8Encoding.UTF8.GetString(buffer, 0, result.Count);

                    try
                    {
                        var block = JsonConvert.DeserializeObject<MessageBlock>(jsonResult);

                        // todo: handle received message blocks.
                        Console.WriteLine(block.data.message);

                        instance.OnMessage(instance, new CustomEventArgs { Data = block });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
        }

        private static void LogStatus(bool receiving, byte[] buffer, int length)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = receiving ? ConsoleColor.Green : ConsoleColor.Gray;
                //Console.WriteLine("{0} ", receiving ? "Received" : "Sent");

                if (verbose)
                    Console.WriteLine(encoder.GetString(buffer));

                Console.ResetColor();
            }
        }

        private void Ws_OnError(object sender, EventArgs e)
        {
            Console.WriteLine("Ws_OnError");
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Ws_OnOpen");
        }

        private void Ws_OnMessage(object sender, EventArgs e)
        {
            //Console.WriteLine("message: IsBinary:{0} Data:{1}");
        }

        private void Ws_OnClose(object sender, EventArgs e)
        {
            Console.WriteLine("Ws_OnClose");
        }

        public void Send(string e, string data)
        {
            //ws.Send(MakePacket(e, data));
        }

        private string MakePacket(string e, string data)
        {
            string body = JsonConvert.SerializeObject(new[] { e, data });
            return "42" + body;
        }
    }
}
