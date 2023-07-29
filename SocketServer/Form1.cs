using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Net.NetworkInformation;

namespace SocketServer
{
    // 信息显示委托
    public delegate void InfoChangeDele(string info);
    // 服务器监听委托
    public delegate void SocketDele(Socket socket);

    public partial class Form1 : Form
    {
        // server运行标志位
        private bool serverOpen = false;
        // form更改委托
        InfoChangeDele formWindowChange;
        // 客户端消息缓存区
        byte[] revBuffer = new byte[1024];
        // 服务器主机socket，用来监听客户端连接
        public Socket? ListenSocket;
        // 服务器向客户端发送消息的缓冲区
        private byte[] sendBuffer = new byte[1024];
        private Socket ClientSocket;
        public Form1()
        {
            InitializeComponent();
            ServerIPAddress.Text = "127.0.0.1";
            ServerPortText.Text = "123";
            formWindowChange += MessBoxChange;
            this.Text = "模拟服务器";
        }

        /// <summary>
        /// 构建server
        /// </summary>
        public void BuildServer()
        {
            // 创建socket对象
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 配置socket bind时的IP地址和端口
            IPAddress address = IPAddress.Parse(ServerIPAddress.Text.Trim());
            int port = Convert.ToInt32(ServerPortText.Text.Trim());
            // 本机服务器地址
            IPEndPoint localendPoint = new IPEndPoint(address, port);

            try
            {
                ListenSocket.Bind(localendPoint);

                MessageBox.Show("bind ok");
                // 服务端进入监听状态，最大监听数为10
                ListenSocket.Listen(10);
                serverOpen = true;

                // 开启监听线程
                ThreadStart ThreadFun = ListenFun;
                Thread NewThread = new Thread(ThreadFun);

                NewThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 处理客户端请求
        /// </summary>
        public async void HandleClient(Socket ClientSocket)
        {
            Invoke(formWindowChange, $"检测到从{ClientSocket.RemoteEndPoint}连接的客户端");
            // 开启一次写进程
            #region 写测试
            SendMessToClient("你好", ClientSocket);
            #endregion

            // 这里需要循环监听客户端发送的消息，不能只监听一次
            ReceMess(revBuffer, ClientSocket);

            MessageBox.Show("我读完了");
        }

        /// <summary>
        /// 开启新线程向客户端写一次数据，
        /// </summary>
        /// <param name="mess">发送的信息,string</param>
        /// <param name="socket">客户端socket</param>
        /// <returns>写成功与否</returns>
        public bool SendMessToClient(string mess, Socket socket)
        {
            bool SendState = false;
            try
            {
                byte[] messByte = Encoding.UTF8.GetBytes(mess);
                if (socket.Connected)
                {
                    Thread writethread = new Thread(async () =>
                    {
                        int len = await ClientSocket.SendAsync(messByte);
                        Thread.Sleep(10);
                        //break;
                        string showmess = $"向客户端写入了{mess},长度为{len}";
                        Invoke(formWindowChange, showmess);
                        MessageBox.Show("写进程退出");
                    });
                    writethread.Start();
                    SendState = true;
                }
                else if (!SendState)
                {
                    string showmess = $"写入失败，重新写入";
                    Invoke(formWindowChange, showmess);
                    // 没有送达继续发送，直到送达为止
                    SendMessToClient(mess, socket);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return SendState;
        }

        /// <summary>
        /// 循环接收客户端消息
        /// </summary>
        /// <param name="revBuffer"></param>
        /// <param name="socket"></param>
        public async void ReceMess(byte[] revBuffer, Socket socket)
        {
            while (socket.Connected)
            {
                try
                {
                    #region 从客户端读数据进程 
                    Array.Clear(revBuffer, 0, revBuffer.Length);
                    socket.ReceiveTimeout = 1000;
                    int recebyte = await socket.ReceiveAsync(revBuffer);
                    if (recebyte == 0)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                        socket = null;
                        break;
                    }
                    //ClientSocket.ReceiveAsync(revBuffer);
                    string show = Encoding.UTF8.GetString(revBuffer).Replace("\0", "");
                    show = $"从远程连接{socket.RemoteEndPoint}接收,接收内容:\n" + show;
                    // 不能在非UI线程上对UI的对象进行修改
                    // 只能通过调用invoke方法
                    Invoke(formWindowChange, show);
                    Thread.Sleep(100);
                    #endregion
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        /// <summary>
        /// 监听客户端连接
        /// </summary>
        public async void ListenFun()
        {

            // 当服务器运行时，监听客户端连接
            // 为每一个接收到的socket对象创建一个新线程
            while (serverOpen)
            {
                try
                {
                    // 非阻塞Accept，接受client的socket
                    ClientSocket = await ListenSocket.AcceptAsync();
                    if (ClientSocket != null)
                    {
                        // 每接受到一个客户端则开启新的线程来处理
                        SocketDele mydele = new SocketDele(HandleClient);
                        // 线程处理函数是上文定义的委托
                        Thread thread = new Thread(() => mydele(ClientSocket));

                        thread.Start();
                        //Thread.Sleep(100);
                        // 等待新创建的线程处理完成,
                        // 这里不能加join，因为创建了客户端之后一直在循环监听，如果加join那么服务器就会只访问那一个客户端
                        //thread.Join();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 框内显示消息
        /// </summary>
        /// <param name="info"></param>
        public void MessBoxChange(string info)
        {
            RecvBox.AppendText(info + '\n');
            // 只现实客户端发送的消息
            //ClientMessShowBox.Text = info;
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void ServerOpenClick(object sender, EventArgs e)
        {
            BuildServer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void CloseServerClick(object sender, EventArgs e)
        {

            CloseSocket(ListenSocket);

        }

        private void CloseSocket(Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    serverOpen = false;
                    RecvBox.AppendText(socket.LocalEndPoint.ToString() + "已关闭\n");
                    socket.Close();
                    MessageBox.Show("服务器已关闭");
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            }
        }

        private void SendMessButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 定义一个1024字节的字节缓冲区，用来存储发送的信息
                SendMessToClient(SendMessBox.Text.ToString(), ClientSocket);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.ToString());
            }
        }
    }
}