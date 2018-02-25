using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;

public class Network_Sample : MonoBehaviour {
    public GameObject sampleobject;

    // udpでipaddress送信
    // 2秒間自分以外のデータが返ってこなかったら自分がtcpサーバーに移行
    // 2秒以内に自分以外のデータが返ってきたらtcpクライアントモードに移行

    private UDPListenerManager listener;

    private UDPSenderManager client;

    private string mssting = "";
	// Use this for initialization
	void Start () {
        listener = new UDPListenerManager(8008);
        listener.ListenerMessageEvent += ListenerMessageEvent;

        client = new UDPSenderManager("127.0.0.1", 8008);
    }

    void OnDestroy()
    {
        listener.ListenerMessageEvent -= ListenerMessageEvent;
        listener.DisConnectListener();

        client.DisConnectSender();
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

        client.SendMessage("Hello");
    }

    private void ListenerMessageEvent(string ms,string address)
    {
        mssting = ms + address;
    }
}
