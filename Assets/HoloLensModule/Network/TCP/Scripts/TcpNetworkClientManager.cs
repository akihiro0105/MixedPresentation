using System;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Threading;
using System.Net.Sockets;
#endif

namespace HoloLensModule.Network
{
    public class TcpNetworkClientManager
    {

        public delegate void ConnectMessageHandler();
        public delegate void ReceiveMessageHandler(Byte[] data);
        public ConnectMessageHandler ConnectMessage;
        public ConnectMessageHandler DisconnectMessage;
        public ReceiveMessageHandler ReceiveMessage;

#if UNITY_UWP
        private Stream streamOut = null;
        private string errorstring = "";
#elif UNITY_EDITOR || UNITY_STANDALONE
        private Thread thread = null;
        private NetworkStream stream = null;
        private TcpClient tcp;
#endif
        public TcpNetworkClientManager(string IP,int port)
        {
#if UNITY_UWP
            Task.Run(async () => {
                try
                {
                    StreamSocket socket = new StreamSocket();
                    Windows.Networking.HostName serverhost = new Windows.Networking.HostName(IP);
                    await socket.ConnectAsync(serverhost, port.ToString());

                    streamOut = socket.OutputStream.AsStreamForWrite();
                    Stream streamIn = socket.InputStream.AsStreamForRead();
                    if (ConnectMessage != null) ConnectMessage();

                    Byte[] bytes = new Byte[1024];
                    while (true)
                    {
                        int bytecount = await streamIn.ReadAsync(bytes, 0, bytes.Length);
                        byte[] data = new byte[bytecount];
                        Buffer.BlockCopy(bytes, 0, data, 0, bytecount);
                        if (ReceiveMessage != null) ReceiveMessage(data);
                    }
                }
                catch (Exception e)
                {
                    errorstring = e.ToString();
                    if (DisconnectMessage != null) DisconnectMessage();
                    if (streamOut != null) streamOut.Dispose();
                    streamOut = null;
                }
            });
#elif UNITY_EDITOR || UNITY_STANDALONE
            tcp = new TcpClient(IP, port);
            thread = new Thread(ThreadProcess);
            thread.Start();
#endif
        }

        public void DeleteManager()
        {
#if UNITY_UWP
            if (streamOut != null) streamOut.Dispose();
            streamOut = null;
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (thread != null)
            {
                if (stream != null) stream.Close();
                thread.Abort();
            }
            stream = null;
            thread = null;
#endif
        }

        public void SendMessage(Byte[] data)
        {
#if UNITY_UWP
            if (streamOut != null) Task.Run(async () =>
             {
                 await streamOut.WriteAsync(data, 0, data.Length);
                 await streamOut.FlushAsync();
             });
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (stream != null) stream.Write(data, 0, data.Length);
#endif
        }

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ThreadProcess()
        {
            try
            {
                stream = tcp.GetStream();
                if (ConnectMessage != null) ConnectMessage();
                while (tcp.Connected)
                {
                    byte[] bytes = new byte[tcp.ReceiveBufferSize];
                    //int bytecount = 
                    stream.Read(bytes, 0, bytes.Length);
                    //byte[] data = new byte[bytecount];
                    //Buffer.BlockCopy(bytes, 0, data, 0, bytecount);
                    if (ReceiveMessage != null) ReceiveMessage(bytes);
                }
                if (DisconnectMessage != null) DisconnectMessage();
                stream.Close();
                stream = null;
                tcp.Close();
            }
            catch (Exception)
            {
                if (DisconnectMessage != null) DisconnectMessage();
                if (stream != null) stream.Close();
                stream = null;
            }
        }
#endif
    }
}
