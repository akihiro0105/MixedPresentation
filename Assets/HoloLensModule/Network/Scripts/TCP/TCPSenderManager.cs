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
        private Thread thread = null;
        private TcpClient tcpclient = null;
        private NetworkStream stream = null;
        private MemoryStream allBytes = null;
        private byte[] readBytes = new byte[1024];
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
                thread = new Thread(() => {
                    tcpclient = new TcpClient(ipaddress, port);
                    tcpclient.ReceiveTimeout = 100;
                    stream = tcpclient.GetStream();
                    if (stream!=null)
                    {
                        if (allBytes != null)
                        {
                            allBytes.Close();
                        }
                        allBytes = new MemoryStream();
                        if (stream.CanRead)
                        {
                            stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReceiveCallback), null);
                        }
                    }
                });
                thread.Start();
            }
#endif
        }

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ReceiveCallback(IAsyncResult result)
        {
            int count = tcpclient.ReceiveBufferSize;
            int readbyte = stream.EndRead(result);
            if (readbyte>0)
            {
                allBytes.Write(readBytes, 0, readbyte);
            }
            else
            {
                if (ReceiveByteEvent != null) ReceiveByteEvent(allBytes.ToArray());
                if (ReceiveMessageEvent != null) ReceiveMessageEvent(allBytes.ToString());
                allBytes.Close();
            }
            stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReceiveCallback), null);
        }
#endif

        public bool SendMessage(string ms)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (stream != null)
            {
                if (thread == null || thread.ThreadState != ThreadState.Running)
                {
                    thread = new Thread(() =>
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(ms);
                        stream.Write(bytes, 0, bytes.Length);
                    });
                    thread.Start();
                    return true;
                }
            }
#endif
            return false;
        }

        public bool SendMessage(byte[] data)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (stream != null)
            {
                if (thread == null || thread.ThreadState != ThreadState.Running)
                {
                    thread = new Thread(() =>
                    {
                        stream.Write(data, 0, data.Length);
                    });
                    thread.Start();
                    return true;
                }
            }
#endif
            return false;
        }

        public void DisConnectSender()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (tcpclient!=null)
            {
                stream.Close();
                stream = null;
                tcpclient.Close();
                tcpclient = null;
            }
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
#endif
        }
    }
}
