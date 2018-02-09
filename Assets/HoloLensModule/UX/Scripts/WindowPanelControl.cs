using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloLensModule.Input;
using HoloLensModule.Utility;

public class WindowPanelControl : MonoBehaviour
{
    [SerializeField]
    private Text WindowName;
    [SerializeField]
    private GameObject HidePanel;
    public float InitSetTime = 2.0f;
    public float SetDistance = 1.5f;

    private bool WindowInitSetFlag = true;
    private bool WindowSetFlag = false;
    private float t = 0.0f;
    private Boundingbox boundingbox;
    private BoxCollider boundingboxcollider = null;
    private AudioSource audiosource;
    private float starttime = 0.0f;

    // Use this for initialization
    void Start()
    {
        WindowName.text = Application.productName;
        //HandPressManager.onReleased += onReleased;
        boundingbox = GetComponent<Boundingbox>();
        audiosource = GetComponent<AudioSource>();

        HidePanel.SetActive(true);
    }

    void OnDestroy()
    {
        //HandPressManager.onReleased -= onReleased;
    }

    // Update is called once per frame
    void Update()
    {
        if (WindowInitSetFlag == true)
        {
            SetWindowPanel();
            t += Time.deltaTime;
            if (t > InitSetTime)
            {
                AdjustActive(false);
                audiosource.Play();
                WindowInitSetFlag = false;
            }
        }
        else
        {
            if (WindowSetFlag == true)
            {
                SetWindowPanel();
            }
        }
    }

    private void SetWindowPanel()
    {
        transform.LookAt(Camera.main.transform.position);
        Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * SetDistance;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2.0f);
    }

    private void onReleased()
    {
        float delta = Time.time - starttime;
        if (WindowSetFlag == true && delta > 0.5f)
        {
            AdjustActive(false);
            audiosource.Play();
        }
    }

    private void AdjustActive(bool flag)
    {
        HidePanel.SetActive(flag);
        //boundingbox.isActive(flag);
        WindowSetFlag = flag;
        if (boundingboxcollider == null)
        {
            boundingboxcollider = GetComponent<BoxCollider>();
        }
        boundingboxcollider.enabled = flag;
    }

    public void SetAdjust()
    {
        AdjustActive(true);
        starttime = Time.time;
    }
}
