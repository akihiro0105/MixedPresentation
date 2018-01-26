using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Input;

public class WindowButtonControl : MonoBehaviour ,FocusInterface{
    [SerializeField]
    private GameObject ButtonObject;
    [SerializeField]
    private Color UnFocusButtonColor = Color.white;
    [SerializeField]
    private Color FocusButtonColor = Color.white;
    [SerializeField]
    private float FocusScale = 1.25f;

    private bool FocusFlag = false;
    private Vector3 scale;
    private Renderer materialrenderer;

    public void FocusEnd()
    {
        FocusFlag = false;
    }

    public void FocusEnter()
    {
        FocusFlag = true;
    }

    // Use this for initialization
    void Start () {
        scale = transform.localScale;
        materialrenderer = ButtonObject.GetComponent<Renderer>();
        materialrenderer.material.color = UnFocusButtonColor;
    }
	
	// Update is called once per frame
	void Update () {
        if (FocusFlag==true)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale * FocusScale, Time.deltaTime * 10.0f);
            materialrenderer.material.color = FocusButtonColor;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale, Time.deltaTime * 10.0f);
            materialrenderer.material.color = UnFocusButtonColor;
        }
	}
}
