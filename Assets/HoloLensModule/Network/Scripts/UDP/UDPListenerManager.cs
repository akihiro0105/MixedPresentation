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
    public class UDPListenerManager
    {
        public delegate void ListenerStateEventHandler();
        public ListenerStateEventHandler ListenerStateEvent;

        public delegate void ListenerMessageEventHandler(string ms);
        public ListenerMessageEventHandler ListenerMessageEvent;

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif

        public UDPListenerManager()
        {

        }

        public UDPListenerManager(string ipaddress,int port)
        {

        }

        public void ConnectListener(string ipaddress, int port)
        {

        }

        public void DisConnectListener()
        {
            
        }
    }
}
