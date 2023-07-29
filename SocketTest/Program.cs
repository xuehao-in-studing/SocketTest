﻿using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using MyLib1;

#region SocketTest
public class Sample
{
    public static string DoSocketGet(string server)
    {
        //Set up variables and String to write to the server.
        Encoding ASCII = Encoding.ASCII;
        string Get = "GET / HTTP/1.1\r\nHost: " + server +
                     "\r\nConnection: Close\r\n\r\n";
        Byte[] ByteGet = ASCII.GetBytes(Get);
        Byte[] RecvBytes = new Byte[256];
        String strRetPage = null;

        // IPAddress and IPEndPoint represent the endpoint that will
        //   receive the request.
        // Get first IPAddress in list return by DNS.

        try
        {

            // Define those variables to be evaluated in the next for loop and
            // then used to connect to the server. These variables are defined
            // outside the for loop to make them accessible there after.
            Socket s = null;
            IPEndPoint hostEndPoint;
            IPAddress hostAddress = null;
            int conPort = 80;

            // Get DNS host information.
            IPHostEntry hostInfo = Dns.GetHostEntry(server);
            // Get the DNS IP addresses associated with the host.
            IPAddress[] IPaddresses = hostInfo.AddressList;

            // Evaluate the socket and receiving host IPAddress and IPEndPoint.
            for (int index = 0; index < IPaddresses.Length; index++)
            {
                hostAddress = IPaddresses[index];
                hostEndPoint = new IPEndPoint(hostAddress, conPort);


                // Creates the Socket to send data over a TCP connection.
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


                // Connect to the host using its IPEndPoint.
                s.Connect(hostEndPoint);

                if (!s.Connected)
                {
                    // Connection failed, try next IPaddress.
                    strRetPage = "Unable to connect to host";
                    s = null;
                    continue;
                }

                // Sent the GET request to the host.
                s.Send(ByteGet, ByteGet.Length, 0);

            } // End of the for loop.


            // Receive the host home page content and loop until all the data is received.
            Int32 bytes = s.Receive(RecvBytes, RecvBytes.Length, 0);
            strRetPage = "Default HTML page on " + server + ":\r\n";
            strRetPage = strRetPage + ASCII.GetString(RecvBytes, 0, bytes);

            while (bytes > 0)
            {
                bytes = s.Receive(RecvBytes, RecvBytes.Length, 0);
                strRetPage = strRetPage + ASCII.GetString(RecvBytes, 0, bytes);
            }

        } // End of the try block.

        catch (SocketException e)
        {
            Console.WriteLine("SocketException caught!!!");
            Console.WriteLine("Source : " + e.Source);
            Console.WriteLine("Message : " + e.Message);
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException caught!!!");
            Console.WriteLine("Source : " + e.Source);
            Console.WriteLine("Message : " + e.Message);
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine("NullReferenceException caught!!!");
            Console.WriteLine("Source : " + e.Source);
            Console.WriteLine("Message : " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception caught!!!");
            Console.WriteLine("Source : " + e.Source);
            Console.WriteLine("Message : " + e.Message);
        }

        return strRetPage;
    }
    //public static void Main()
    //{
    //    Console.WriteLine(DoSocketGet("localhost"));
    //}
}
#endregion


// Implements the connection logic for the socket server.
// After accepting a connection, all data read from the client
// is sent back to the client. The read and echo back to the client pattern
// is continued until the client disconnects.
class Server
{
    private int m_numConnections;   // the maximum number of connections the sample is designed to handle simultaneously
    private int m_receiveBufferSize;// buffer size to use for each socket I/O operation
    BufferManager m_bufferManager;  // represents a large reusable set of buffers for all socket operations
    const int opsToPreAlloc = 2;    // read, write (don't alloc buffer space for accepts)
    Socket listenSocket;            // the socket used to listen for incoming connection requests
    // pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
    SocketAsyncEventArgsPool m_readWritePool;
    int m_totalBytesRead;           // counter of the total # bytes received by the server
    int m_numConnectedSockets;      // the total number of clients connected to the server
    Semaphore m_maxNumberAcceptedClients;

    // Create an uninitialized server instance.
    // To start the server listening for connection requests
    // call the Init method followed by Start method
    //
    // <param name="numConnections">the maximum number of connections the sample is designed to handle simultaneously</param>
    // <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param>
    public Server(int numConnections, int receiveBufferSize)
    {
        m_totalBytesRead = 0;
        m_numConnectedSockets = 0;
        m_numConnections = numConnections;
        m_receiveBufferSize = receiveBufferSize;
        // allocate buffers such that the maximum number of sockets can have one outstanding read and
        //write posted to the socket simultaneously
        m_bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,
            receiveBufferSize);

        m_readWritePool = new SocketAsyncEventArgsPool(numConnections);
        m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
    }

    // Initializes the server by preallocating reusable buffers and
    // context objects.  These objects do not need to be preallocated
    // or reused, but it is done this way to illustrate how the API can
    // easily be used to create reusable objects to increase server performance.
    //
    public void Init()
    {
        // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds
        // against memory fragmentation
        m_bufferManager.InitBuffer();

        // preallocate pool of SocketAsyncEventArgs objects
        SocketAsyncEventArgs readWriteEventArg;

        for (int i = 0; i < m_numConnections; i++)
        {
            //Pre-allocate a set of reusable SocketAsyncEventArgs
            readWriteEventArg = new SocketAsyncEventArgs();
            readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

            // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
            m_bufferManager.SetBuffer(readWriteEventArg);

            // add SocketAsyncEventArg to the pool
            m_readWritePool.Push(readWriteEventArg);
        }
    }

    // Starts the server such that it is listening for
    // incoming connection requests.
    //
    // <param name="localEndPoint">The endpoint which the server will listening
    // for connection requests on</param>
    public void Start(IPEndPoint localEndPoint)
    {
        // create the socket which listens for incoming connections
        listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listenSocket.Bind(localEndPoint);
        // start the server with a listen backlog of 100 connections
        listenSocket.Listen(100);

        // post accepts on the listening socket
        SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
        acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
        StartAccept(acceptEventArg);

        //Console.WriteLine("{0} connected sockets with one outstanding receive posted to each....press any key", m_outstandingReadCount);
        Console.WriteLine("Press any key to terminate the server process....");
        Console.ReadKey();
    }

    // Begins an operation to accept a connection request from the client
    //
    // <param name="acceptEventArg">The context object to use when issuing
    // the accept operation on the server's listening socket</param>
    public void StartAccept(SocketAsyncEventArgs acceptEventArg)
    {
        // loop while the method completes synchronously
        bool willRaiseEvent = false;
        while (!willRaiseEvent)
        {
            m_maxNumberAcceptedClients.WaitOne();

            // socket must be cleared since the context object is being reused
            acceptEventArg.AcceptSocket = null;
            willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }
    }

    // This method is the callback method associated with Socket.AcceptAsync
    // operations and is invoked when an accept operation is complete
    //
    void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
    {
        ProcessAccept(e);

        // Accept the next connection request
        StartAccept(e);
    }

    private void ProcessAccept(SocketAsyncEventArgs e)
    {
        Interlocked.Increment(ref m_numConnectedSockets);
        Console.WriteLine("Client connection accepted. There are {0} clients connected to the server",
            m_numConnectedSockets);

        // Get the socket for the accepted client connection and put it into the
        //ReadEventArg object user token
        SocketAsyncEventArgs readEventArgs = m_readWritePool.Pop();
        readEventArgs.UserToken = e.AcceptSocket;

        // As soon as the client is connected, post a receive to the connection
        bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
        if (!willRaiseEvent)
        {
            ProcessReceive(readEventArgs);
        }
    }

    // This method is called whenever a receive or send operation is completed on a socket
    //
    // <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
    void IO_Completed(object sender, SocketAsyncEventArgs e)
    {
        // determine which type of operation just completed and call the associated handler
        switch (e.LastOperation)
        {
            case SocketAsyncOperation.Receive:
                ProcessReceive(e);
                break;
            case SocketAsyncOperation.Send:
                ProcessSend(e);
                break;
            default:
                throw new ArgumentException("The last operation completed on the socket was not a receive or send");
        }
    }

    // This method is invoked when an asynchronous receive operation completes.
    // If the remote host closed the connection, then the socket is closed.
    // If data was received then the data is echoed back to the client.
    //
    private void ProcessReceive(SocketAsyncEventArgs e)
    {
        // check if the remote host closed the connection
        if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
        {
            //increment the count of the total bytes receive by the server
            Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);
            Console.WriteLine("The server has read a total of {0} bytes", m_totalBytesRead);

            //echo the data received back to the client
            e.SetBuffer(e.Offset, e.BytesTransferred);
            Socket socket = (Socket)e.UserToken;
            bool willRaiseEvent = socket.SendAsync(e);
            if (!willRaiseEvent)
            {
                ProcessSend(e);
            }
        }
        else
        {
            CloseClientSocket(e);
        }
    }

    // This method is invoked when an asynchronous send operation completes.
    // The method issues another receive on the socket to read any additional
    // data sent from the client
    //
    // <param name="e"></param>
    private void ProcessSend(SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            // done echoing data back to the client
            Socket socket = (Socket)e.UserToken;
            // read the next block of data send from the client
            bool willRaiseEvent = socket.ReceiveAsync(e);
            if (!willRaiseEvent)
            {
                ProcessReceive(e);
            }
        }
        else
        {
            CloseClientSocket(e);
        }
    }

    private void CloseClientSocket(SocketAsyncEventArgs e)
    {
        Socket socket = (Socket)e.UserToken;

        // close the socket associated with the client
        try
        {
            socket.Shutdown(SocketShutdown.Send);
        }
        // throws if client process has already closed
        catch (Exception) { }
        socket.Close();

        // decrement the counter keeping track of the total number of clients connected to the server
        Interlocked.Decrement(ref m_numConnectedSockets);

        // Free the SocketAsyncEventArg so they can be reused by another client
        m_readWritePool.Push(e);

        m_maxNumberAcceptedClients.Release();
        Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", m_numConnectedSockets);
    }
}
