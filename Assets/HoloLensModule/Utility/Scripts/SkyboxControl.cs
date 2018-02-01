using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

namespace HoloLensModule.Utility
{
    public class SkyboxControl : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
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
#endif
        }
    }
}
