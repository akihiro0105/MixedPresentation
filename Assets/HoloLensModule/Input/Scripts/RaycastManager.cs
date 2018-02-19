using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif
#endif

namespace HoloLensModule.Input
{
    public class RaycastManager : MonoBehaviour
    {
        public GameObject RayObject;
        [SerializeField]
        private float maxDistance = 30.0f;
        public bool isActiveRenderViewportScale = true;

        public delegate void RaycastFocusEventHandler(RaycastHit? info);
        public RaycastFocusEventHandler RaycastFocusEvent;

        private GameObject hifobj = null;
        private RaycastHit hitinfo;
        private IFocusInterface[] fs;

        private float initRenderViewportScale;
        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
            initRenderViewportScale = XRSettings.renderViewportScale;
#else
            initRenderViewportScale = VRSettings.renderViewportScale;
#endif
#endif
            Debug.Log("renderViewportScale "+initRenderViewportScale);
        }

        // Update is called once per frame
        void Update()
        {
            float viewportscale = 1.0f;
            Vector3 position = Camera.main.transform.position;
            Vector3 forward = Camera.main.transform.forward;
            if (RayObject != null)
            {
                position = RayObject.transform.position;
                forward = RayObject.transform.forward;
            }
            if (Physics.Raycast(position, forward, out hitinfo, maxDistance))
            {
                SearchFocusObject(hitinfo.transform.gameObject);
                if (RaycastFocusEvent != null) RaycastFocusEvent(hitinfo);

                float distance = hitinfo.distance;

                if (distance>2.0f)
                {
                    viewportscale = 1.0f;
                }
                else if (distance>1.0f)
                {
                    viewportscale = 0.5f;
                }
                else
                {
                    viewportscale = 0.1f;
                }

            }
            else
            {
                SearchFocusObject(null);
                if (RaycastFocusEvent != null) RaycastFocusEvent(null);
                viewportscale = initRenderViewportScale;
            }

            if (isActiveRenderViewportScale==true)
            {
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
                XRSettings.renderViewportScale = viewportscale;
#else
                VRSettings.renderViewportScale = viewportscale;
#endif
#endif
            }
            else
            {
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
                XRSettings.renderViewportScale = initRenderViewportScale;
#else
                VRSettings.renderViewportScale = initRenderViewportScale;
#endif
#endif
            }
        }

        private void SearchFocusObject(GameObject obj)
        {
            if (obj == null)
            {
                if (hifobj != null)
                {
                    fs = hifobj.GetComponents<IFocusInterface>();
                    for (int i = 0; i < fs.Length; i++)
                    {
                        fs[i].FocusEnd();
                    }
                    hifobj = null;
                }
            }
            else
            {
                if (obj.GetComponent<IFocusInterface>() == null)
                {
                    if (obj.transform.parent != null)
                    {
                        SearchFocusObject(obj.transform.parent.gameObject);
                    }
                    else
                    {
                        SearchFocusObject(null);
                    }
                }
                else
                {
                    if (hifobj != null)
                    {
                        if (hifobj.Equals(obj) == false)
                        {
                            fs = hifobj.GetComponents<IFocusInterface>();
                            for (int i = 0; i < fs.Length; i++)
                            {
                                fs[i].FocusEnd();
                            }
                            fs = obj.GetComponents<IFocusInterface>();
                            for (int i = 0; i < fs.Length; i++)
                            {
                                fs[i].FocusEnter(hitinfo);
                            }
                        }
                    }
                    else
                    {
                        fs = obj.GetComponents<IFocusInterface>();
                        for (int i = 0; i < fs.Length; i++)
                        {
                            fs[i].FocusEnter(hitinfo);
                        }
                    }
                    hifobj = obj;

                }
            }
        }
    }

    public interface IFocusInterface
    {
        void FocusEnter(RaycastHit hit);
        void FocusEnd();
    }
}
