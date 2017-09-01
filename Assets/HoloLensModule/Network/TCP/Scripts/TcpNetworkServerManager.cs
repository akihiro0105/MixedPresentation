using UnityEngine;
using System;
#if UNITY_UWP
        
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
#endif

namespace HoloLensModule.Network
{
    public class TcpNetworkServerManager
    {

#if UNITY_UWP
        
#elif UNITY_EDITOR || UNITY_STANDALONE
        private TcpListener tcpserver;
        public class NetThread
        {
            public Thread thread;
            public NetworkStream stream;
        }
        private List<NetThread> threads = new List<NetThread>();
        private bool ListenFlag = false;
#endif

        public TcpNetworkServerManager(int port)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Debug.Log("Server Start");
            tcpserver = new TcpListener(IPAddress.Any, port);
            tcpserver.Start();
            ListenFlag = true;
            NetThread nt = new NetThread();
            nt.thread = new Thread(new ParameterizedThreadStart(ThreadProcess));
            nt.thread.Start(nt.stream);
            threads.Add(nt);
#endif
        }

        public void DeleteManager()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Debug.Log("Server Stop");
            ListenFlag = false;
            for (int i = 0; i < threads.Count; i++) threads[i].thread.Abort();
            threads.Clear();
            tcpserver.Stop();
#endif
        }

        public void SendMessage(byte[] bytes)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            //byte[] bytes = Encoding.UTF8.GetBytes(data);
            for (int i = 0; i < threads.Count; i++) threads[i].stream.Write(bytes, 0, bytes.Length);
#endif
        }

#if UNITY_EDITOR || UNITY_STANDALONE

        private void ThreadProcess(object obj)
        {
            NetworkStream stream = (NetworkStream)obj;
            TcpClient tcpclient = tcpserver.AcceptTcpClient();
            IPEndPoint remote = (IPEndPoint)tcpclient.Client.RemoteEndPoint;
            string ip = remote.Address.ToString();
            Debug.Log("Connected " + ip);
            tcpclient.ReceiveTimeout = 100;
            stream = tcpclient.GetStream();
            NetThread nt = new NetThread();
            nt.thread = new Thread(new ParameterizedThreadStart(ThreadProcess));
            nt.thread.Start(nt.stream);
            threads.Add(nt);
            while (ListenFlag)
            {
                try
                {
                    byte[] bytes = new byte[tcpclient.ReceiveBufferSize];
                    stream.Read(bytes, 0, (int)tcpclient.ReceiveBufferSize);
                    //string data= Encoding.UTF8.GetString(bytes);
                    //if (ReceiveMessage != null) ReceiveMessage(data,ip);
                    SendMessage(bytes);
                }
                catch (Exception) { }
                if (tcpclient.Client.Poll(1000, SelectMode.SelectRead) && tcpclient.Client.Available == 0) break;
            }
            stream.Close();
            tcpclient.Close();
            Debug.Log("Disconnected " + ip);
        }
#endif
    }
}
