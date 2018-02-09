using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
using UnityEngine.VR.WSA;
#else
using UnityEngine.XR.WSA;
#endif
#endif

namespace HoloLensModule.Input
{
    public class GazeManager : MonoBehaviour
    {
        public GameObject GazeObject;
        [SerializeField]
        private float GazeSpeed = 0.15f;
        public float GazeDistance = 2.0f;

        [Serializable]
        public enum GazeState
        {
            Normal,
            Hand,
            Press
        }

        private GestureManager gesturemanager;
        private InputInteractionManager interactionmanager;
        private RaycastManager raycastmanager;

        private GameObject rayhitobject = null;
        private GameObject focusobject = null;
        private GameObject gestureobject = null;
        // Use this for initialization
        void Start()
        {
            gesturemanager = GetComponent<GestureManager>();
            interactionmanager = GetComponent<InputInteractionManager>();
            raycastmanager = GetComponent<RaycastManager>();
            GazeObject.SetActive(false);
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque == false)
            {
#endif
                GazeObject.SetActive(true);
                gesturemanager.GestureEvent += GestureEvent;
                gesturemanager.GestureUpdateEvent += GestureUpdateEvent;
                raycastmanager.RaycastFocusEvent += RaycastFocusEvent;
#if UNITY_2017_2_OR_NEWER
            }
            else
            {
                GazeObject.SetActive(false);
            }
#endif
#endif
        }

        void OnDestroy()
        {
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque == false)
            {
#endif
                gesturemanager.GestureEvent -= GestureEvent;
                gesturemanager.GestureUpdateEvent -= GestureUpdateEvent;
                raycastmanager.RaycastFocusEvent -= RaycastFocusEvent;
#if UNITY_2017_2_OR_NEWER
            }
#endif
#endif
        }

        // Update is called once per frame
        void Update()
        {
            IGazeInterface gi = GazeObject.GetComponent<IGazeInterface>();
            if (gi != null)
            {
                var list = interactionmanager.GetHandList();
                if (list.Count > 0)
                {
                    bool pressflag = false;
                    foreach (var item in list.Values)
                    {
                        if (item.select == true)
                        {
                            pressflag = true;
                            break;
                        }
                    }
                    if (pressflag == true)
                    {
                        gi.SetGazeState(GazeState.Press);
                    }
                    else
                    {
                        gi.SetGazeState(GazeState.Hand);
                    }
                }
                else
                {
                    gi.SetGazeState(GazeState.Normal);
                }
            }
        }

        private void GestureEvent(GestureManager.GestureState state, Vector3? pos1, Vector3? pos2, float updatetime)
        {
            switch (state)
            {
                case GestureManager.GestureState.Tap:
                    gestureobject = rayhitobject;
                    ITapInterface[] tf = SearchInterfaceObject<ITapInterface>(gestureobject);
                    if (tf!=null)
                    {
                        for (int i = 0; i < tf.Length; i++)
                        {
                            tf[i].ObjectTap();
                        }
                    }
                    break;
                case GestureManager.GestureState.Drag:
                    gestureobject = rayhitobject;
                    IDragInterface[] df = SearchInterfaceObject<IDragInterface>(gestureobject);
                    if (df!=null)
                    {
                        for (int i = 0; i < df.Length; i++)
                        {
                            df[i].ObjectStartDrag(pos1.Value);
                        }
                    }
                    break;
            }
        }

        private void GestureUpdateEvent(GestureManager.GestureState state, Vector3? pos1, Vector3? pos2, float holdtime)
        {
            switch (state)
            {
                case GestureManager.GestureState.Drag:
                    IDragInterface[] df=SearchInterfaceObject<IDragInterface>(gestureobject);
                    if (df!=null)
                    {
                        for (int i = 0; i < df.Length; i++)
                        {
                            df[i].ObjectUpdateDrag(pos1.Value);
                        }
                    }
                    break;
            }
        }

        private void RaycastFocusEvent(RaycastHit? hit)
        {
            Vector3 pos = GazeObject.transform.position;
            if (hit!=null)
            {
                GazeObject.transform.position = Vector3.Lerp(pos, hit.Value.point, GazeSpeed);
                GazeObject.transform.forward = -hit.Value.normal;
                rayhitobject = hit.Value.transform.gameObject;
                focusobject = rayhitobject;
            }
            else
            {
                GazeObject.transform.position = Vector3.Lerp(pos, Camera.main.transform.position + Camera.main.transform.forward * GazeDistance, GazeSpeed);
                GazeObject.transform.forward = Camera.main.transform.forward;
                rayhitobject = null;
            }

#if UNITY_EDITOR || UNITY_UWP
            if (focusobject != null)
            {
                Vector3 normal = -Camera.main.transform.forward;
                Vector3 position = focusobject.transform.position;
                HolographicSettings.SetFocusPointForFrame(position, normal);
            }
#endif
        }

        private T[] SearchInterfaceObject<T>(GameObject obj)
        {
            if (obj != null)
            {
                if (obj.GetComponent<T>() != null)
                {
                    return obj.GetComponents<T>();
                }
                if (obj.transform.parent != null)
                {
                    return SearchInterfaceObject<T>(obj.transform.parent.gameObject);
                }
            }
            return null;
        }
    }

    public interface ITapInterface
    {
        void ObjectTap();
    }

    public interface IDragInterface
    {
        void ObjectStartDrag(Vector3 pos);
        void ObjectUpdateDrag(Vector3 pos);
    }

    public interface IGazeInterface
    {
        void SetGazeState(GazeManager.GazeState state);
    }
}
