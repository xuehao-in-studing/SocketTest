using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Net.NetworkInformation;

namespace SocketServer
{
    // ��Ϣ��ʾί��
    public delegate void InfoChangeDele(string info);
    // ����������ί��
    public delegate void SocketDele(Socket socket);

    public partial class Form1 : Form
    {
        // server���б�־λ
        private bool serverOpen = false;
        // form����ί��
        InfoChangeDele formWindowChange;
        // �ͻ�����Ϣ������
        byte[] revBuffer = new byte[1024];
        // ����������socket�����������ͻ�������
        public Socket? ListenSocket;
        // ��������ͻ��˷�����Ϣ�Ļ�����
        private byte[] sendBuffer = new byte[1024];
        private Socket ClientSocket;
        public Form1()
        {
            InitializeComponent();
            ServerIPAddress.Text = "127.0.0.1";
            ServerPortText.Text = "123";
            formWindowChange += MessBoxChange;
            this.Text = "ģ�������";
        }

        /// <summary>
        /// ����server
        /// </summary>
        public void BuildServer()
        {
            // ����socket����
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // ����socket bindʱ��IP��ַ�Ͷ˿�
            IPAddress address = IPAddress.Parse(ServerIPAddress.Text.Trim());
            int port = Convert.ToInt32(ServerPortText.Text.Trim());
            // ������������ַ
            IPEndPoint localendPoint = new IPEndPoint(address, port);

            try
            {
                ListenSocket.Bind(localendPoint);

                MessageBox.Show("bind ok");
                // ����˽������״̬����������Ϊ10
                ListenSocket.Listen(10);
                serverOpen = true;

                // ���������߳�
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
        /// ����ͻ�������
        /// </summary>
        public async void HandleClient(Socket ClientSocket)
        {
            Invoke(formWindowChange, $"��⵽��{ClientSocket.RemoteEndPoint}���ӵĿͻ���");
            // ����һ��д����
            #region д����
            SendMessToClient("���", ClientSocket);
            #endregion

            // ������Ҫѭ�������ͻ��˷��͵���Ϣ������ֻ����һ��
            ReceMess(revBuffer, ClientSocket);

            MessageBox.Show("�Ҷ�����");
        }

        /// <summary>
        /// �������߳���ͻ���дһ�����ݣ�
        /// </summary>
        /// <param name="mess">���͵���Ϣ,string</param>
        /// <param name="socket">�ͻ���socket</param>
        /// <returns>д�ɹ����</returns>
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
                        string showmess = $"��ͻ���д����{mess},����Ϊ{len}";
                        Invoke(formWindowChange, showmess);
                        MessageBox.Show("д�����˳�");
                    });
                    writethread.Start();
                    SendState = true;
                }
                else if (!SendState)
                {
                    string showmess = $"д��ʧ�ܣ�����д��";
                    Invoke(formWindowChange, showmess);
                    // û���ʹ�������ͣ�ֱ���ʹ�Ϊֹ
                    SendMessToClient(mess, socket);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return SendState;
        }

        /// <summary>
        /// ѭ�����տͻ�����Ϣ
        /// </summary>
        /// <param name="revBuffer"></param>
        /// <param name="socket"></param>
        public async void ReceMess(byte[] revBuffer, Socket socket)
        {
            while (socket.Connected)
            {
                try
                {
                    #region �ӿͻ��˶����ݽ��� 
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
                    show = $"��Զ������{socket.RemoteEndPoint}����,��������:\n" + show;
                    // �����ڷ�UI�߳��϶�UI�Ķ�������޸�
                    // ֻ��ͨ������invoke����
                    Invoke(formWindowChange, show);
                    Thread.Sleep(100);
                    #endregion
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        /// <summary>
        /// �����ͻ�������
        /// </summary>
        public async void ListenFun()
        {

            // ������������ʱ�������ͻ�������
            // Ϊÿһ�����յ���socket���󴴽�һ�����߳�
            while (serverOpen)
            {
                try
                {
                    // ������Accept������client��socket
                    ClientSocket = await ListenSocket.AcceptAsync();
                    if (ClientSocket != null)
                    {
                        // ÿ���ܵ�һ���ͻ��������µ��߳�������
                        SocketDele mydele = new SocketDele(HandleClient);
                        // �̴߳����������Ķ����ί��
                        Thread thread = new Thread(() => mydele(ClientSocket));

                        thread.Start();
                        //Thread.Sleep(100);
                        // �ȴ��´������̴߳������,
                        // ���ﲻ�ܼ�join����Ϊ�����˿ͻ���֮��һֱ��ѭ�������������join��ô�������ͻ�ֻ������һ���ͻ���
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
        /// ������ʾ��Ϣ
        /// </summary>
        /// <param name="info"></param>
        public void MessBoxChange(string info)
        {
            RecvBox.AppendText(info + '\n');
            // ֻ��ʵ�ͻ��˷��͵���Ϣ
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
                    RecvBox.AppendText(socket.LocalEndPoint.ToString() + "�ѹر�\n");
                    socket.Close();
                    MessageBox.Show("�������ѹر�");
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            }
        }

        private void SendMessButton_Click(object sender, EventArgs e)
        {
            try
            {
                // ����һ��1024�ֽڵ��ֽڻ������������洢���͵���Ϣ
                SendMessToClient(SendMessBox.Text.ToString(), ClientSocket);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.ToString());
            }
        }
    }
}