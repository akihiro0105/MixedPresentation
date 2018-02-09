using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class RaycastManager : MonoBehaviour
    {
        public GameObject RayObject;
        [SerializeField]
        private float maxDistance = 30.0f;

        public delegate void RaycastFocusEventHandler(RaycastHit? info);
        public RaycastFocusEventHandler RaycastFocusEvent;

        private GameObject hifobj = null;
        private RaycastHit hitinfo;
        private IFocusInterface[] fs;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
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
            }
            else
            {
                SearchFocusObject(null);
                if (RaycastFocusEvent != null) RaycastFocusEvent(null);
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
