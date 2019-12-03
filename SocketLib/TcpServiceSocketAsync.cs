using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WolfInv.Com.SocketLib
{
    public class AsyncUserToken
    {
        private Socket socket = null;
        public Socket Socket { get => socket; set => socket = value; }
    }
    public class TcpServiceSocketAsync
    {
        //接收数据事件
        public Action<string> recvMessageEvent = null;
        //发送结果事件
        public Action<int> sendResultEvent = null;

        //监听socket
        private Socket listenSocket = null;
        //允许连接到tcp服务器的tcp客户端数量
        private int numConnections = 0;
        //用于socket发送和接受的缓存区大小
        private int bufferSize = 0;
        //socket缓冲区管理对象
        private BufferManager bufferManager = null;
        //SocketAsyncEventArgs池
        private SocketAsyncEventArgsPool socketAsyncEventArgsPool = null;
        //当前连接的tcp客户端数量
        private int numberAcceptedClients = 0;
        //控制tcp客户端连接数量的信号量
        private Semaphore maxNumberAcceptedClients = null;
        //用于socket发送数据的SocketAsyncEventArgs集合
        private List<SocketAsyncEventArgs> sendAsyncEventArgs = null;
        //tcp服务器ip
        private string ip = "";
        //tcp服务器端口
        private int port = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="numConnections">允许连接到tcp服务器的tcp客户端数量</param>
        /// <param name="bufferSize">用于socket发送和接受的缓存区大小</param>
        public TcpServiceSocketAsync(int numConnections, int bufferSize)
        {
            if (numConnections <= 0 || numConnections > int.MaxValue)
                throw new ArgumentOutOfRangeException("_numConnections is out of range");
            if (bufferSize <= 0 || bufferSize > int.MaxValue)
                throw new ArgumentOutOfRangeException("_receiveBufferSize is out of range");

            this.numConnections = numConnections;
            this.bufferSize = bufferSize;
            bufferManager = new BufferManager(numConnections * bufferSize * 2, bufferSize);
            socketAsyncEventArgsPool = new SocketAsyncEventArgsPool(numConnections);
            maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
            sendAsyncEventArgs = new List<SocketAsyncEventArgs>();
        }

        public static int getRandomPort(int? defaultPort=80)
        {
            if(defaultPort!= null && !PortInUse(defaultPort.Value))
            {
                return defaultPort.Value;
            }
            Random rm = new Random();
            int val = rm.Next(1, 65535);
            while(PortInUse(val))
            {
                val = rm.Next(1, 65535);
            }
            return val;
        }

        public static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }


            return inUse;
        }

        /// <summary>
        /// 初始化数据（bufferManager，socketAsyncEventArgsPool）
        /// </summary>
        public void Init()
        {
            numberAcceptedClients = 0;
            bufferManager.InitBuffer();
            SocketAsyncEventArgs readWriteEventArg;
            for (int i = 0; i < numConnections * 2; i++)
            {
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = new AsyncUserToken();

                bufferManager.SetBuffer(readWriteEventArg);
                socketAsyncEventArgsPool.Push(readWriteEventArg);
            }
        }

        /// <summary>
        ///  开启tcp服务器，等待tcp客户端连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Start(string ip, int port)
        {
            //if (string.IsNullOrEmpty(ip))
            //    throw new ArgumentNullException("ip cannot be null");
            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException("port is out of range");

            this.ip = ip;
            this.port = port;

            try
            {
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress address = IPAddress.Any;
                if(!string.IsNullOrEmpty(ip))
                    address = IPAddress.Parse(ip);
                IPEndPoint endpoint = new IPEndPoint(address, port);
                listenSocket.Bind(endpoint);//绑定地址
                listenSocket.Listen(int.MaxValue);//开始监听

                StartAccept(null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 关闭tcp服务器
        /// </summary>
        public void CloseSocket()
        {
            if (listenSocket == null)
                return;

            try
            {
                foreach (var e in sendAsyncEventArgs)
                {
                    ((AsyncUserToken)e.UserToken).Socket.Shutdown(SocketShutdown.Both);
                }
                listenSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }

            try
            {
                foreach (var e in sendAsyncEventArgs)
                {
                    ((AsyncUserToken)e.UserToken).Socket.Close();
                }
                listenSocket.Close();
            }
            catch { }

            try
            {
                foreach (var e in sendAsyncEventArgs)
                {

                    e.Dispose();
                }
            }
            catch { }

            sendAsyncEventArgs.Clear();
            socketAsyncEventArgsPool.Clear();
            bufferManager.FreeAllBuffer();
            maxNumberAcceptedClients.Release(numberAcceptedClients);
        }

        /// <summary>
        /// 重新启动tcp服务器
        /// </summary>
        public void Restart()
        {
            CloseSocket();
            Init();
            Start(ip, port);
        }

        /// <summary>
        /// 开始等待tcp客户端连接
        /// </summary>
        /// <param name="acceptEventArg"></param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArg.AcceptSocket = null;
            }

            maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// Socket.AcceptAsync完成回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        /// 接受到tcp客户端连接，进行处理
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref numberAcceptedClients);
            //设置用于接收的SocketAsyncEventArgs的socket，可以接受数据
            SocketAsyncEventArgs recvEventArgs = socketAsyncEventArgsPool.Pop();
            ((AsyncUserToken)recvEventArgs.UserToken).Socket = e.AcceptSocket;
            //设置用于发送的SocketAsyncEventArgs的socket，可以发送数据
            SocketAsyncEventArgs sendEventArgs = socketAsyncEventArgsPool.Pop();
            ((AsyncUserToken)sendEventArgs.UserToken).Socket = e.AcceptSocket;
            sendAsyncEventArgs.Add(sendEventArgs);

            StartRecv(recvEventArgs);
            StartAccept(e);
        }

        /// <summary>
        /// tcp服务器开始接受tcp客户端发送的数据
        /// </summary>
        private void StartRecv(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent = ((AsyncUserToken)e.UserToken).Socket.ReceiveAsync(e);
            if (!willRaiseEvent)
            {
                ProcessReceive(e);
            }
        }

        /// <summary>
        /// socket.sendAsync和socket.recvAsync的完成回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
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

        /// <summary>
        /// 处理接受到的tcp客户端数据
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (recvMessageEvent != null)
                    //一定要指定GetString的长度
                    recvMessageEvent(Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred));

                StartRecv(e);
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        /// <summary>
        /// 处理tcp服务器发送的结果
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.SocketError == SocketError.Success)
            {
                if (sendResultEvent != null)
                    sendResultEvent(e.BytesTransferred);
            }
            else
            {
                if (sendResultEvent != null)
                    sendResultEvent(e.BytesTransferred);
                CloseClientSocket(e);
            }
        }

        /// <summary>
        /// 关闭一个与tcp客户端连接的socket
        /// </summary>
        /// <param name="e"></param>
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            try
            {
                //关闭socket时，单独使用socket.close()通常会造成资源提前被释放，应该在关闭socket之前，先使用shutdown进行接受或者发送的禁用，再使用socket进行释放
                token.Socket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            token.Socket.Close();

            Interlocked.Decrement(ref numberAcceptedClients);
            socketAsyncEventArgsPool.Push(e);
            maxNumberAcceptedClients.Release();

            if (e.LastOperation == SocketAsyncOperation.Send)
                sendAsyncEventArgs.Remove(e);
        }

        /// <summary>
        /// 给全部tcp客户端发送数据
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToAllClients(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message cannot be null");

            foreach (var e in sendAsyncEventArgs)
            {
                byte[] buff = Encoding.UTF8.GetBytes(message);
                if (buff.Length > bufferSize)
                    throw new ArgumentOutOfRangeException("message is out off range");

                buff.CopyTo(e.Buffer, e.Offset);
                e.SetBuffer(e.Offset, buff.Length);
                bool willRaiseEvent = ((AsyncUserToken)e.UserToken).Socket.SendAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessSend(e);
                }
            }
        }
    }

    // This class creates a single large buffer which can be divided up 
    // and assigned to SocketAsyncEventArgs objects for use with each 
    // socket I/O operation.  
    // This enables bufffers to be easily reused and guards against 
    // fragmenting heap memory.
    // 
    // The operations exposed on the BufferManager class are not thread safe.
    public class BufferManager
    {
        //buffer缓冲区大小
        private int m_numBytes;
        //缓冲区
        private byte[] m_buffer;
        private Stack<int> m_freeIndexPool;
        private int m_currentIndex;
        private int m_bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// 给buffer分配缓冲区
        /// </summary>
        public void InitBuffer()
        {
            m_buffer = new byte[m_numBytes];
        }

        /// <summary>
        ///  将buffer添加到args的IO缓冲区中，并设置offset
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize;
            }
            return true;
        }

        /// <summary>
        /// 将buffer从args的IO缓冲区中释放
        /// </summary>
        /// <param name="args"></param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

        /// <summary>
        /// 释放全部buffer缓存
        /// </summary>
        public void FreeAllBuffer()
        {
            m_freeIndexPool.Clear();
            m_currentIndex = 0;
            m_buffer = null;
        }
    }

    // Represents a collection of reusable SocketAsyncEventArgs objects.  
    public class SocketAsyncEventArgsPool
    {
        private Stack<SocketAsyncEventArgs> m_pool;

        // Initializes the object pool to the specified size
        //
        // The "capacity" parameter is the maximum number of 
        // SocketAsyncEventArgs objects the pool can hold
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        // Add a SocketAsyncEventArg instance to the pool
        //
        //The "item" parameter is the SocketAsyncEventArgs instance 
        // to add to the pool
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        // Removes a SocketAsyncEventArgs instance from the pool
        // and returns the object removed from the pool
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        /// <summary>
        /// 清空栈中元素
        /// </summary>
        public void Clear()
        {
            lock (m_pool)
            {
                m_pool.Clear();
            }
        }

        // The number of SocketAsyncEventArgs instances in the pool
        public int Count
        {
            get { return m_pool.Count; }
        }

    }
}
