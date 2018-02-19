using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#else
#endif
#endif


namespace HoloLensModule.Utility
{
    public class SkyboxControl : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
            Camera cam = gameObject.GetComponent<Camera>();
            if (cam != null)
            {
                if (HolographicSettings.IsDisplayOpaque)
                {
                    cam.clearFlags = CameraClearFlags.Skybox;
                }
                else
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                }
            }
#else
            
#endif
#endif
        }
    }
}
