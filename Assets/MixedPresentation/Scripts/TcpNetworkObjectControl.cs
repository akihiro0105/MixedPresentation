using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloLensModule.Network;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
#endif

namespace MixedPresentation
{
    public class TcpNetworkObjectControl : MonoBehaviour
    {
        public int UDPPort=8000;
        public int TCPPort = 1234;
        public GameObject PresentationCamera;
        private TcpNetworkClientManager tcpclient;
        private TcpNetworkServerManager tcpserver;
        private UdpNetworkClientManager udpclient;
        private UdpNetworkListenManager udpserver;
        private MediaObjectManager mediaobject;

        public enum MessageType
        {
            Connected,
            ObjectTransform
        }
        private List<GameObject> objectlist = new List<GameObject>();
        private bool ConnectFlag = false;
        public class CurrentTransform
        {
            public CurrentTransform(Vector3 pos,Quaternion rot,Vector3 scale)
            {
                Position = pos;
                Rotation = rot;
                Scale = scale;
            }
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
        }
        private List<CurrentTransform> objecttransformlist = new List<CurrentTransform>();
        private bool ConnectedFlag = false;
        private int ObjectTransformFlag = -1;
        private string ServerIP = "";

        // Use this for initialization
        void Start()
        {
            mediaobject = GetComponent<MediaObjectManager>();
            mediaobject.MediaLoadCompleteEvent += MediaLoadCompleteEvent;
            mediaobject.LoadMediaObjects();
            udpserver = new UdpNetworkListenManager(UDPPort);
            udpserver.UdpNetworkListenEvent += UdpNetworkListenEvent;
            udpclient = new UdpNetworkClientManager(UDPPort);
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            IPAddress[] address = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var item in address)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ServerIP = item.ToString();
                    Debug.Log(ServerIP);
                    break;
                }
            }
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if (ConnectFlag == false) return;
            if (ObjectTransformFlag!=-1)
            {
                objectlist[ObjectTransformFlag].transform.localPosition = objecttransformlist[ObjectTransformFlag].Position;
                objectlist[ObjectTransformFlag].transform.localRotation = objecttransformlist[ObjectTransformFlag].Rotation;
                objectlist[ObjectTransformFlag].transform.localScale = objecttransformlist[ObjectTransformFlag].Scale;
                ObjectTransformFlag = -1;
            }
            for (int i = 0; i < objectlist.Count; i++)
            {
                if (objectlist[i].transform.localPosition!= objecttransformlist[i].Position || objectlist[i].transform.localRotation != objecttransformlist[i].Rotation|| objectlist[i].transform.localScale != objecttransformlist[i].Scale)
                {
                    JsonMessage ms = new JsonMessage();
                    ms.type = 1;
                    ms.jobject.Set(i,objectlist[i]);
                    string mss = JsonUtility.ToJson(ms);
                    tcpclient.SendMessage(mss);
                    objecttransformlist[i].Position = objectlist[i].transform.localPosition;
                    objecttransformlist[i].Rotation = objectlist[i].transform.localRotation;
                    objecttransformlist[i].Scale = objectlist[i].transform.localScale;
                }
            }
            if (ConnectedFlag)
            {
                for (int i = 0; i < objectlist.Count; i++)
                {
                    JsonMessage ms = new JsonMessage();
                    ms.type = 1;
                    ms.jobject.Set(i, objectlist[i]);
                    string mss = JsonUtility.ToJson(ms);
                    tcpserver.SendMessage(mss);
                }
                ConnectedFlag = false;
            }
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            udpclient.SendMessage(ServerIP);
#endif
        }

        void OnDestroy()
        {
            udpclient.DeleteManager();
            udpserver.DeleteManager();
            tcpclient.DeleteManager();
            tcpserver.DeleteManager();
        }

        private void MediaLoadCompleteEvent(List<GameObject> obj)
        {
            objectlist.Add(PresentationCamera);
            objecttransformlist.Add(new CurrentTransform(PresentationCamera.transform.localPosition, PresentationCamera.transform.localRotation, PresentationCamera.transform.localScale));

            for (int i = 0; i < obj.Count; i++)
            {
                objectlist.Add(obj[i]);
                objecttransformlist.Add(new CurrentTransform(obj[i].transform.localPosition,obj[i].transform.localRotation, obj[i].transform.localScale));
            }
            StartCoroutine(TCPConnect());
        }

        private IEnumerator TCPConnect()
        {
            while (ServerIP == "") yield return null;
            Debug.Log("Connect Server : "+ServerIP);
            tcpserver = new TcpNetworkServerManager(TCPPort);
            tcpclient = new TcpNetworkClientManager(ServerIP, TCPPort);
            tcpclient.ConnectMessage += ConnectMessage;
            tcpclient.ReceiveMessage += ReceiveMessage;
        }

        private void ConnectMessage()
        {
            JsonMessage ms = new JsonMessage();
            ms.type = 0;
            string mss = JsonUtility.ToJson(ms);
            tcpclient.SendMessage(mss);
            ConnectFlag = true;
        }

        private void ReceiveMessage(string data)
        {
            JsonMessage ms = new JsonMessage();
            ms = JsonUtility.FromJson<JsonMessage>(data);
            MessageType type = (MessageType)ms.type;
            switch (type)
            {
                case MessageType.Connected:
                    ConnectedFlag = true;
                    break;
                case MessageType.ObjectTransform:
                    int id = ms.jobject.num;
                    ObjectTransformFlag = id;
                    if (id >= 0 && id < objectlist.Count)
                    {
                        objecttransformlist[id].Position = new Vector3(ms.jobject.transform.position.x, ms.jobject.transform.position.y, ms.jobject.transform.position.z);
                        objecttransformlist[id].Rotation = new Quaternion(ms.jobject.transform.rotation.x, ms.jobject.transform.rotation.y, ms.jobject.transform.rotation.z, ms.jobject.transform.rotation.w);
                        objecttransformlist[id].Scale = new Vector3(ms.jobject.transform.scale.x, ms.jobject.transform.scale.y, ms.jobject.transform.scale.z);
                    }
                    break;
                default:
                    break;
            }
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
        }

        private void UdpNetworkListenEvent(string data,string address)
        {
            ServerIP = data;
        }

        [Serializable]
        public class JsonPosition
        {
            public float x;
            public float y;
            public float z;
            public void Set(Vector3 v) { x = v.x; y = v.y; z = v.z; }
        }
        [Serializable]
        public class JsonRotation
        {
            public float x;
            public float y;
            public float z;
            public float w;
            public void Set(Quaternion v) { x = v.x; y = v.y; z = v.z; w = v.w; }
        }
        [Serializable]
        public class JsonScale
        {
            public float x;
            public float y;
            public float z;
            public void Set(Vector3 v) { x = v.x; y = v.y; z = v.z; }
        }
        [Serializable]
        public class JsonTransform
        {
            public JsonTransform()
            {
                position = new JsonPosition();
                rotation = new JsonRotation();
                scale = new JsonScale();
            }
            public JsonPosition position;
            public JsonRotation rotation;
            public JsonScale scale;
            public void Set(GameObject t)
            {
                position.Set(t.transform.localPosition);
                rotation.Set(t.transform.localRotation);
                scale.Set(t.transform.localScale);
            }
        }
        [Serializable]
        public class JsonObject
        {
            public JsonObject() { transform = new JsonTransform(); }
            public int num;
            public JsonTransform transform;
            public void Set(int num,GameObject obj) { this.num = num; transform.Set(obj); }
        }
        [Serializable]
        public class JsonMessage
        {
            public JsonMessage() { jobject = new JsonObject(); }
            public int type;
            public JsonObject jobject;
        }
    }
}