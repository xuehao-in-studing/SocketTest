using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SocketClient
{
    public delegate void InfoChangeDele(string info);
    public delegate void Mydel();

    public partial class Form1 : Form
    {

        private Socket ClientSocket;
        private IPAddress address;
        private int port;
        private IPEndPoint endPoint;
        private byte[] sendBuffer = new byte[1024];

        InfoChangeDele infochange;
        // 创建的客户端的数量
        private static int counter = 0;
        byte[] receiveBuffer = new byte[4096];

        public Form1()
        {
            InitializeComponent();
            infochange += ShowMessBoxChange;
            this.Text = "模拟客户端";
        }

        private void ShowMessBoxChange(string info)
        {
            RecvBox.AppendText(info + "\n");
        }

        public void ConnectToServer()
        {
            address = IPAddress.Parse(ServerIPAddress.Text.Trim());
            port = Convert.ToInt32(ServerPortText.Text.Trim());
            endPoint = new IPEndPoint(address, port);


            //if (ClientSocket == null)
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 客户端连接服务器,并显示连接信息
            try
            {
                ClientStateBox.AppendText("与服务器连接中...");
                // 第一次握手
                //ClientSocket.Connect(endPoint);
                try
                {
                    IAsyncResult isa =  ClientSocket.BeginConnect(endPoint, ConnectCallBack, ClientSocket);
                    // 阻塞当前线程，直到该线程完成或者超时
                    isa.AsyncWaitHandle.WaitOne(100);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                if (ClientSocket.Connected)
                {
                    ClientStateBox.AppendText("\n连接成功");
                }
                else if (ClientSocket.Poll(0, SelectMode.SelectWrite))
                {
                    ClientStateBox.AppendText("\nThis Socket is writable.");
                }
                else if (ClientSocket.Poll(0, SelectMode.SelectRead))
                {
                    ClientStateBox.AppendText("\nThis Socket is readable.");
                }
                else if (ClientSocket.Poll(0, SelectMode.SelectError))
                {
                    ClientStateBox.AppendText("\nThis Socket has an error.");
                }
                counter++;
                ClientStateBox.AppendText($"{counter}个客户端连接到服务器");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error" + e.ToString());
                ClientSocket.Close();
            }

        }

        public void ConnectCallBack(IAsyncResult result)
        {
            try
            {
                //建立一个接受信息的byte数组
                //从回调参数中获取上面Conntect方法中的socket对象
                Socket socket = result.AsyncState as Socket;
                // 结束挂起的异步connect
                socket.EndConnect(result);
                //判断是否和服务端的socket建立连接
                if (socket.Connected)
                {
                    //开始 异步接收服务端传来的信息，同样将socket传入回调方法的参数中
                    socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);

                }
                else { ConnectCallBack(result); }
            }
            catch
            {
                Console.WriteLine("连接出错");
            }
        }

        private void ReceiveCallBack(IAsyncResult result)
        {
            Socket socket = result.AsyncState as Socket;
            //读取从服务器端传来的数据，EndReceive是关闭异步接收方法，同时读取数据
            try
            {
                int bytesRead = socket.EndReceive(result);
                if (bytesRead > 0)
                {
                    try
                    {
                        //接受完服务端的数据后的逻辑
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(receiveBuffer, receivedData, bytesRead);
                        Invoke(infochange, "接收消息:" + Encoding.UTF8.GetString(receivedData));
                    }
                    catch (Exception e) { MessageBox.Show("Error" + e.ToString()); }
                }
                // 递归监听服务器端是否发来信息，一旦服务器再次发送信息，客户端仍然可以接收到
                socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallBack, socket);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }



        private void CloseSocket(Socket socket)
        {
            try
            {
                if (socket.Connected)
                {
                    ClientStateBox.AppendText(socket.LocalEndPoint.ToString() + "已关闭\n");
                    // 关闭接收和发送
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    MessageBox.Show("Client Socket closed");
                }
            }
            catch (SocketException e)
            {
                MessageBox.Show("Error" + e.ToString());
            }
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void SendIPClick(object sender, EventArgs e)
        {

        }

        private void ConnectToServerClick(object sender, EventArgs e)
        {
            //if (counter >= 1)
            //{
            //    // 当创建的数量超过一时，先把之前的关闭
            //    CloseSocket(ClientSocket);
            //}
            ConnectToServer();

        }

        private void SendMessClick(object sender, EventArgs e)
        {
            try
            {
                // 定义一个1024字节的字节缓冲区，用来存储发送的信息
                sendBuffer = Encoding.UTF8.GetBytes(SendMessBox.Text);
                // 是同步模式吗
                if(sendBuffer.Length > 0)
                {
                    ClientSocket.SendAsync(sendBuffer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.ToString());
            }
        }

        private void SocketCloseClick(object sender, EventArgs e)
        {
            CloseSocket(ClientSocket);
        }

        private void RecvBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}