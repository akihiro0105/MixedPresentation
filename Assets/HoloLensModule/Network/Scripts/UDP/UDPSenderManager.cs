using System;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text;
#endif

namespace HoloLensModule.Network
{
    public class UDPSenderManager
    {
        public delegate void SenderStateEventHandler();
        public SenderStateEventHandler SenderStateEvent;

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif

        public UDPSenderManager()
        {

        }

        public UDPSenderManager(string ipaddress,int port)
        {

        }

        public void ConnectSender(string ipaddress,int port)
        {

        }

        public void SendMessage(string ms)
        {

        }

        public void DisConnectSender()
        {

        }
    }
}
