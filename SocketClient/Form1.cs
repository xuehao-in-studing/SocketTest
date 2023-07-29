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
        // �����Ŀͻ��˵�����
        private static int counter = 0;
        byte[] receiveBuffer = new byte[4096];

        public Form1()
        {
            InitializeComponent();
            infochange += ShowMessBoxChange;
            this.Text = "ģ��ͻ���";
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
            // �ͻ������ӷ�����,����ʾ������Ϣ
            try
            {
                ClientStateBox.AppendText("�������������...");
                // ��һ������
                //ClientSocket.Connect(endPoint);
                try
                {
                    IAsyncResult isa =  ClientSocket.BeginConnect(endPoint, ConnectCallBack, ClientSocket);
                    // ������ǰ�̣߳�ֱ�����߳���ɻ��߳�ʱ
                    isa.AsyncWaitHandle.WaitOne(100);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                if (ClientSocket.Connected)
                {
                    ClientStateBox.AppendText("\n���ӳɹ�");
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
                ClientStateBox.AppendText($"{counter}���ͻ������ӵ�������");
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
                //����һ��������Ϣ��byte����
                //�ӻص������л�ȡ����Conntect�����е�socket����
                Socket socket = result.AsyncState as Socket;
                // ����������첽connect
                socket.EndConnect(result);
                //�ж��Ƿ�ͷ���˵�socket��������
                if (socket.Connected)
                {
                    //��ʼ �첽���շ���˴�������Ϣ��ͬ����socket����ص������Ĳ�����
                    socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);

                }
                else { ConnectCallBack(result); }
            }
            catch
            {
                Console.WriteLine("���ӳ���");
            }
        }

        private void ReceiveCallBack(IAsyncResult result)
        {
            Socket socket = result.AsyncState as Socket;
            //��ȡ�ӷ������˴��������ݣ�EndReceive�ǹر��첽���շ�����ͬʱ��ȡ����
            try
            {
                int bytesRead = socket.EndReceive(result);
                if (bytesRead > 0)
                {
                    try
                    {
                        //���������˵����ݺ���߼�
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(receiveBuffer, receivedData, bytesRead);
                        Invoke(infochange, "������Ϣ:" + Encoding.UTF8.GetString(receivedData));
                    }
                    catch (Exception e) { MessageBox.Show("Error" + e.ToString()); }
                }
                // �ݹ�������������Ƿ�����Ϣ��һ���������ٴη�����Ϣ���ͻ�����Ȼ���Խ��յ�
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
                    ClientStateBox.AppendText(socket.LocalEndPoint.ToString() + "�ѹر�\n");
                    // �رս��պͷ���
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
            //    // ����������������һʱ���Ȱ�֮ǰ�Ĺر�
            //    CloseSocket(ClientSocket);
            //}
            ConnectToServer();

        }

        private void SendMessClick(object sender, EventArgs e)
        {
            try
            {
                // ����һ��1024�ֽڵ��ֽڻ������������洢���͵���Ϣ
                sendBuffer = Encoding.UTF8.GetBytes(SendMessBox.Text);
                // ��ͬ��ģʽ��
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