using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;

public class Network_Sample : MonoBehaviour {
    public GameObject sampleobject;

    private UDPListenerManager listener;
    private UDPSenderManager client;

    private TCPSenderManager tcpclient;
    private TcpNetworkServerManager server;

    private string mssting = "";
	// Use this for initialization
	void Start () {
        //listener = new UDPListenerManager(8008);
        //listener.ListenerMessageEvent += ListenerMessageEvent;

        //client = new UDPSenderManager("127.0.0.1", 8008);

        tcpclient = new TCPSenderManager("127.0.0.1", 8009);
        tcpclient.ReceiveMessageEvent += ListenerTCPMessageEvent;

        server = new TcpNetworkServerManager(8009);
    }

    void OnDestroy()
    {
        //listener.ListenerMessageEvent -= ListenerMessageEvent;
        //listener.DisConnectListener();

        //client.DisConnectSender();

        //tcpclient.ReceiveMessage -= ListenerTCPMessageEvent;
        tcpclient.ReceiveMessageEvent -= ListenerTCPMessageEvent;
        tcpclient.DisConnectSender();

        //tcpclient.DeleteManager();

        server.DeleteManager();
    }

    // Update is called once per frame
    void Update () {
        

        if (mssting!="")
        {
            sampleobject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Debug.Log(mssting);
            mssting = "";
        }
        else
        {
            sampleobject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }

        //client.SendMessage("Hello");
        tcpclient.SendMessage("Hello TCP");
    }

    private void ListenerMessageEvent(string ms,string address)
    {
        mssting = ms + address;
    }

    private void ListenerTCPMessageEvent(string ms)
    {
        mssting = ms;
    }
}
