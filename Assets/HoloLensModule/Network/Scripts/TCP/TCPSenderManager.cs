using System;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
#endif

namespace HoloLensModule.Network
{
    public class TCPSenderManager
    {
        public delegate void ReceiveMessageEventHandler(string ms);
        public ReceiveMessageEventHandler ReceiveMessageEvent;

        public delegate void ReceiveByteEventHandler(byte[] data);
        public ReceiveByteEventHandler ReceiveByteEvent;

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private TcpClient tcpclient = null;
        private NetworkStream stream = null;
        private MemoryStream allBytes = null;
        private byte[] readBytes = null;
#endif

        public TCPSenderManager() { }

        public TCPSenderManager(string ipaddress, int port)
        {
            ConnectSender(ipaddress, port);
        }

        public void ConnectSender(string ipaddress, int port)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (tcpclient==null)
            {
                tcpclient = new TcpClient();
                tcpclient.BeginConnect(ipaddress, port, new AsyncCallback(ConnectedCallback), tcpclient);
            }
#endif
        }

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ConnectedCallback(IAsyncResult result)
        {
            TcpClient tcp = (TcpClient)result.AsyncState;
            stream = tcp.GetStream();
            readBytes = new byte[1024];
            if (allBytes!=null)
            {
                allBytes.Close();
            }
            allBytes = new MemoryStream();
            stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReceiveCallback), stream);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            NetworkStream s = (NetworkStream)result.AsyncState;
            int readbyte = s.EndRead(result);
            if (readbyte>0)
            {
                allBytes.Write(readBytes, 0, readBytes.Length);
            }
            else
            {
                if (ReceiveByteEvent != null) ReceiveByteEvent(allBytes.ToArray());
                if (ReceiveMessageEvent != null) ReceiveMessageEvent(allBytes.ToString());
                allBytes.Close();
            }
            s.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReceiveCallback), stream);
        }
#endif

        public bool SendMessage(string ms)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            return false;
        }

        public bool SendMessage(byte[] data)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            return false;
        }

        public void DisConnectSender()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (tcpclient!=null)
            {
                tcpclient.EndConnect(null);
            }
#endif
        }
    }
}
