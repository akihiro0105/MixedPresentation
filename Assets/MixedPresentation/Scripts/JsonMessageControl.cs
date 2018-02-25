using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;

namespace MixedPresentation
{
    public class JsonMessageControl : MonoBehaviour
    {
        public string BroadcastAddress = "";
        public int UDPport = 8000;

        //private UdpNetworkClientManager udpclient;
        //private UdpNetworkListenManager udpserver;
        private JsonMessage SendJsonMessage = new JsonMessage();
        private Queue<JsonMessage> ReceiveJsonMessageQueue = new Queue<JsonMessage>();

        public delegate void ReceiveJsonMessageEventHandler(JsonMessage obj);
        public ReceiveJsonMessageEventHandler ReceiveCameraJsonMessage;
        public ReceiveJsonMessageEventHandler ReceivePlayJsonMessage;
        public ReceiveJsonMessageEventHandler ReceiveTransformJsonMessage;

        // Use this for initialization
        void Start()
        {
            //if (BroadcastAddress == null || BroadcastAddress == "") udpclient = new UdpNetworkClientManager(UDPport);
            //else udpclient = new UdpNetworkClientManager(UDPport, BroadcastAddress);
            //udpserver = new UdpNetworkListenManager(UDPport);
            //udpserver.UdpNetworkListenEvent += UdpNetworkListenEvent;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < ReceiveJsonMessageQueue.Count; i++)
            {
                JsonMessage jm = ReceiveJsonMessageQueue.Dequeue();
                Debug.Log("Receive data");
                switch ((JsonMessageType)jm.type)
                {
                    case JsonMessageType.Camera:
                        if (ReceiveCameraJsonMessage != null) ReceiveCameraJsonMessage(jm);
                        break;
                    case JsonMessageType.Transform:
                        if (ReceiveTransformJsonMessage != null) ReceiveTransformJsonMessage(jm);
                        break;
                    case JsonMessageType.Play:
                        if (ReceivePlayJsonMessage != null) ReceivePlayJsonMessage(jm);
                        break;
                    default:
                        break;
                }
            }
        }

        void OnDestroy()
        {
            //udpclient.DeleteManager();
            //udpserver.DeleteManager();
            //udpserver.UdpNetworkListenEvent -= UdpNetworkListenEvent;
        }

        public void SendCameraMessage(int CamNum)
        {
            SendJsonMessage.type = (int)JsonMessageType.Camera;
            SendJsonMessage.CamNum = CamNum;
            //udpclient.SendMessage(JsonUtility.ToJson(SendJsonMessage));
        }

        public void SendPlayMessage(string name,bool flag)
        {
            SendJsonMessage.type = (int)JsonMessageType.Play;
            SendJsonMessage.gameobjectflag.name = name;
            SendJsonMessage.gameobjectflag.flag = flag;
            //udpclient.SendMessage(JsonUtility.ToJson(SendJsonMessage));
        }

        public void SendTransformMessage(JsonGameObject obj)
        {
            SendJsonMessage.type = (int)JsonMessageType.Transform;
            SendJsonMessage.gameobject = obj;
            //udpclient.SendMessage(JsonUtility.ToJson(SendJsonMessage));
        }

        private void UdpNetworkListenEvent(string data, string address) { ReceiveJsonMessageQueue.Enqueue(JsonUtility.FromJson<JsonMessage>(data)); }
    }
}
