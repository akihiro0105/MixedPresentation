using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloLensModule;
using HoloLensModule.Input;
using HoloLensModule.Network;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
#endif

namespace MixedPresentation
{
    public class TcpNetworkObjectControl : HoloLensModuleSingleton<TcpNetworkObjectControl>
    {
        public int UDPPort=8000;
        public int TCPPort = 1234;
        public GameObject PresentationCamera;
        private TcpNetworkClientManager tcpclient;
        private TcpNetworkServerManager tcpserver;
        private UdpNetworkClientManager udpclient;
        private UdpNetworkListenManager udpserver;
        private MediaObjectManager mediaobject;

        private List<GameObject> objectlist = new List<GameObject>();
        private bool LoadCompletedFlag = false;
        public class CurrentTransform
        {
            public CurrentTransform(Vector3 pos,Quaternion rot,Vector3 scale,bool flag)
            {
                Position = pos;
                Rotation = rot;
                Scale = scale;
                playflag = flag;
            }
            public bool playflag;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
        }
        private List<CurrentTransform> objecttransformlist = new List<CurrentTransform>();
        private int ObjectTransformFlag = -1;
        private string ServerIP = "";
        JsonMessage sendjson = new JsonMessage();
        JsonMessage receivejson = new JsonMessage();

        private bool selectnetworkflag = false;

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;

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
            StartCoroutine(TCPConnect());
        }

        private void MediaLoadCompleteEvent(List<GameObject> obj)
        {
            objectlist.Add(PresentationCamera);
            objecttransformlist.Add(new CurrentTransform(PresentationCamera.transform.localPosition, PresentationCamera.transform.localRotation, PresentationCamera.transform.localScale,false));

            for (int i = 0; i < obj.Count; i++)
            {
                objectlist.Add(obj[i]);
                objecttransformlist.Add(new CurrentTransform(obj[i].transform.localPosition, obj[i].transform.localRotation, obj[i].transform.localScale,false));
            }
            LoadCompletedFlag = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (LoadCompletedFlag == false) return;
            if (ObjectTransformFlag != -1)
            {
                objectlist[ObjectTransformFlag].transform.localPosition = objecttransformlist[ObjectTransformFlag].Position;
                objectlist[ObjectTransformFlag].transform.localRotation = objecttransformlist[ObjectTransformFlag].Rotation;
                objectlist[ObjectTransformFlag].transform.localScale = objecttransformlist[ObjectTransformFlag].Scale;
                MediaTapControl tap = objectlist[ObjectTransformFlag].GetComponent<MediaTapControl>();
                if(tap!=null)tap.SetPlay(objecttransformlist[ObjectTransformFlag].playflag);
                ObjectTransformFlag = -1;
            }
            else
            {
                for (int i = 0; i < objectlist.Count; i++)
                {
                    MediaTapControl tap = objectlist[i].GetComponent<MediaTapControl>();
                    bool flag = false;
                    if (tap != null) flag = tap.GetPlay();
                    if (objectlist[i].transform.localPosition != objecttransformlist[i].Position || objectlist[i].transform.localRotation != objecttransformlist[i].Rotation || objectlist[i].transform.localScale != objecttransformlist[i].Scale || objecttransformlist[i].playflag != flag)
                    {
                        sendjson.type = (int)MessageType.ObjectTransform;
                        sendjson.jobject.Set(i, objectlist[i].transform.localPosition, objectlist[i].transform.localRotation, objectlist[i].transform.localScale, flag);
                        SendJsonMessage(JsonUtility.ToJson(sendjson), selectnetworkflag);
                    }
                    objecttransformlist[i].Position = objectlist[i].transform.localPosition;
                    objecttransformlist[i].Rotation = objectlist[i].transform.localRotation;
                    objecttransformlist[i].Scale = objectlist[i].transform.localScale;
                    objecttransformlist[i].playflag = flag;
                }
            }
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            sendjson.type = (int)MessageType.SendIP;
            sendjson.ip = ServerIP;
            SendJsonMessage(JsonUtility.ToJson(sendjson), true);
#endif
        }

        protected override void OnDestroy()
        {
            udpclient.DeleteManager();
            udpserver.DeleteManager();
            tcpclient.DeleteManager();
            tcpserver.DeleteManager();
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
        }

        private IEnumerator TCPConnect()
        {
            while (ServerIP == "") yield return null;
            Debug.Log("Connect Server : "+ServerIP);
            tcpserver = new TcpNetworkServerManager(TCPPort);
            tcpclient = new TcpNetworkClientManager(ServerIP, TCPPort);
            tcpclient.ReceiveMessage += ReceiveMessage;
        }

        private void ReceiveMessage(string data) { ReceiveJsonMessage(data); }

        private void UdpNetworkListenEvent(string data, string address) { ReceiveJsonMessage(data); }

        private void ReceiveJsonMessage(string data)
        {
            receivejson = JsonUtility.FromJson<JsonMessage>(data);
            MessageType type = (MessageType)receivejson.type;
            switch (type)
            {
                case MessageType.SendIP:
                    ServerIP = receivejson.ip;
                    break;
                case MessageType.ObjectTransform:
                    int id = receivejson.jobject.num;
                    ObjectTransformFlag = id;
                    if (id >= 0 && id < objectlist.Count)
                    {
                        objecttransformlist[id].Position = new Vector3(receivejson.jobject.transform.position.x, receivejson.jobject.transform.position.y, receivejson.jobject.transform.position.z);
                        objecttransformlist[id].Rotation = new Quaternion(receivejson.jobject.transform.rotation.x, receivejson.jobject.transform.rotation.y, receivejson.jobject.transform.rotation.z, receivejson.jobject.transform.rotation.w);
                        objecttransformlist[id].Scale = new Vector3(receivejson.jobject.transform.scale.x, receivejson.jobject.transform.scale.y, receivejson.jobject.transform.scale.z);
                        objecttransformlist[id].playflag = receivejson.jobject.play;
                    }
                    break;
                default:
                    break;
            }
        }

        public void SendJsonMessage(string data,bool udpflag=false)
        {
            if (udpflag) udpclient.SendMessage(data);
            else tcpclient.SendMessage(data);
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state == HandsGestureManager.HandGestureState.MultiDoubleTap)
            {
                Debug.Log("Select Network");
                selectnetworkflag = !selectnetworkflag;
            }
        }
    }
}