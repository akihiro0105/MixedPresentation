using System;
using System.Collections.Generic;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
using System.Threading;
using System.Net.Sockets;
#endif

namespace HoloLensModule.Network
{
    public class TCPListenerManager
    {
#if UNITY_UWP
        private StreamSocketListener streamsocketlistener;
        private List<StreamWriter> writer = new List<StreamWriter>();
#elif UNITY_EDITOR || UNITY_STANDALONE
        TcpListener tcpserver = null;
#endif

        public TCPListenerManager() { }

        public TCPListenerManager(int port)
        {
            ConnectListener(port);
        }

        public void ConnectListener(int port)
        {
#if UNITY_UWP
            if (task==null)
            {
                task = Task.Run(async () => {
                    socket = new DatagramSocket();
                    socket.MessageReceived += MessageReceived;
                    await socket.BindServiceNameAsync(port.ToString());
                });
            }
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (tcpserver==null)
            {
                tcpserver = new TcpListener(IPAddress.Any, port);
                tcpserver.Start();
            }
#endif
        }

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ReceiveCallback(IAsyncResult result)
        {

        }
#endif

        public void DisConnectListener()
        {
#if UNITY_UWP
            if (socket != null)
            {
                socket.MessageReceived -= MessageReceived;
                socket.Dispose();
                socket = null;
                task = null;
            }
#elif UNITY_EDITOR || UNITY_STANDALONE
            tcpserver.Stop();
#endif
        }
    }
}
