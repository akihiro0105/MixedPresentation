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
        public GameObject PresentationCamera;
        private TcpNetworkClientManager client;
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
            client = GetComponent<TcpNetworkClientManager>();
            client.ConnectMessage += ConnectMessage;
            client.ReceiveMessage += ReceiveMessage;
            udpserver = GetComponent<UdpNetworkListenManager>();
            udpserver.UdpNetworkListenEvent += UdpNetworkListenEvent;
            udpclient = GetComponent<UdpNetworkClientManager>();
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            IPAddress[] address = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var item in address)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ServerIP = item.ToString();
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
                    client.SendMessage(SetTransformByteList(i, objectlist[i].transform.localPosition, objectlist[i].transform.localRotation, objectlist[i].transform.localScale));
                    objecttransformlist[i].Position = objectlist[i].transform.localPosition;
                    objecttransformlist[i].Rotation = objectlist[i].transform.localRotation;
                    objecttransformlist[i].Scale = objectlist[i].transform.localScale;
                }
            }
            if (ConnectedFlag)
            {
                for (int i = 0; i < objectlist.Count; i++)
                {
                    TcpNetworkServerManager.Instance.SendMessage(SetTransformByteList(i, objectlist[i].transform.localPosition, objectlist[i].transform.localRotation, objectlist[i].transform.localScale));
                }
                ConnectedFlag = false;
            }

            udpclient.UDPSendMessage(ServerIP);
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
            client.IP = ServerIP;
            client.Connect();
        }

        private void ConnectMessage()
        {
            client.SendMessage(BitConverter.GetBytes((Int32)MessageType.Connected));
            ConnectFlag = true;
        }

        private void ReceiveMessage(byte[] data)
        {
            int bytecount = 0;
            MessageType type = (MessageType)GetToInt32(data, bytecount, out bytecount);
            switch (type)
            {
                case MessageType.Connected:
                    ConnectedFlag = true;
                    break;
                case MessageType.ObjectTransform:
                    int id = GetToInt32(data, bytecount, out bytecount);
                    ObjectTransformFlag = id;
                    if (id >= 0 && id < objectlist.Count)
                    {
                        objecttransformlist[id].Position = GetToVector3(data, bytecount, out bytecount);
                        objecttransformlist[id].Rotation = GetToQuaternion(data, bytecount, out bytecount);
                        objecttransformlist[id].Scale = GetToVector3(data, bytecount, out bytecount);
                    }
                    break;
                default:
                    break;
            }
        }

        private void UdpNetworkListenEvent(string data)
        {
            ServerIP = data;
        }

        private byte[] SetTransformByteList(int id, Vector3 position, Quaternion rotation,Vector3 scale)
        {
            List<byte> listbyte = new List<byte>();
            listbyte.AddRange(BitConverter.GetBytes((Int32)MessageType.ObjectTransform));
            listbyte.AddRange(BitConverter.GetBytes((Int32)id));
            listbyte.AddRange(BitConverter.GetBytes(position.x));
            listbyte.AddRange(BitConverter.GetBytes(position.y));
            listbyte.AddRange(BitConverter.GetBytes(position.z));
            listbyte.AddRange(BitConverter.GetBytes(rotation.x));
            listbyte.AddRange(BitConverter.GetBytes(rotation.y));
            listbyte.AddRange(BitConverter.GetBytes(rotation.z));
            listbyte.AddRange(BitConverter.GetBytes(rotation.w));
            listbyte.AddRange(BitConverter.GetBytes(scale.x));
            listbyte.AddRange(BitConverter.GetBytes(scale.y));
            listbyte.AddRange(BitConverter.GetBytes(scale.z));
            return listbyte.ToArray();
        }

        private Vector3 GetToVector3(byte[] data, int startcount, out int endcount)
        {
            Vector3 v = new Vector3();
            v.x = GetToSingle(data, startcount, out startcount);
            v.y = GetToSingle(data, startcount, out startcount);
            v.z = GetToSingle(data, startcount, out startcount);
            endcount = startcount;
            return v;
        }

        private Quaternion GetToQuaternion(byte[] data, int startcount, out int endcount)
        {
            Quaternion q = new Quaternion();
            q.x = GetToSingle(data, startcount, out startcount);
            q.y = GetToSingle(data, startcount, out startcount);
            q.z = GetToSingle(data, startcount, out startcount);
            q.w = GetToSingle(data, startcount, out startcount);
            endcount = startcount;
            return q;
        }

        private int GetToInt32(byte[] data, int startcount, out int endcount)
        {
            int i = BitConverter.ToInt32(data, startcount);
            endcount = startcount + sizeof(Int32);
            return i;
        }

        private float GetToSingle(byte[] data, int startcount, out int endcount)
        {
            float f = BitConverter.ToSingle(data, startcount);
            endcount = startcount + sizeof(float);
            return f;
        }
    }
}