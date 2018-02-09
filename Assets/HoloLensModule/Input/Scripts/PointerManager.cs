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
    public class PointerManager : MonoBehaviour
    {
        public GameObject ControlerPrefab;

        [Serializable]
        public enum DeviceType
        {
            Right,
            Left
        }
        [Serializable]
        public enum DeviceButton
        {
            Select, // trigger
            Menu,
            Grip,
            Touchpad, // trigger
            Stick // trigger
        }

        private InputInteractionManager interactionmanager;

        public class ControlerObject
        {
            public bool gripflag = false;
            public GameObject model;
            public ControlerObject(GameObject obj)
            {
                gripflag = false;
                model = obj;
            }
        }
        private Dictionary<uint, ControlerObject> ControlerList = new Dictionary<uint, ControlerObject>();

        private GameObject gripobject = null;
        // Use this for initialization
        void Start()
        {
            interactionmanager = GetComponent<InputInteractionManager>();
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque == true)
            {
                interactionmanager.onDetectEvent += onDetectEvent;
                interactionmanager.onLostEvent += onLostEvent;
                interactionmanager.onPressEvent += onPressEvent;
                interactionmanager.onUpdateEvent += onUpdateEvent;
                interactionmanager.onReleaseEvent += onReleaseEvent;
            }
#endif
#endif
        }

        void OnDestroy()
        {
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque == true)
            {
                interactionmanager.onDetectEvent -= onDetectEvent;
                interactionmanager.onLostEvent -= onLostEvent;
                interactionmanager.onPressEvent -= onPressEvent;
                interactionmanager.onUpdateEvent -= onUpdateEvent;
                interactionmanager.onReleaseEvent -= onReleaseEvent;
            }
#endif
#endif
        }

        // Update is called once per frame
        void Update()
        {
            var list = interactionmanager.GetHandList();
        }

        private void onDetectEvent(uint id, InputInteractionManager.HandPoint handpoint)
        {
            GameObject obj = Instantiate(ControlerPrefab, transform);
            obj.transform.position = handpoint.pos;
            obj.transform.rotation = handpoint.rot;
            IPointerInterface pi = obj.GetComponent<IPointerInterface>();
            if (pi!=null)
            {
                if (handpoint.devicetype==InputInteractionManager.DeviceType.Left)
                {
                    pi.SetDeviceType(DeviceType.Left);
                }
                else
                {
                    pi.SetDeviceType(DeviceType.Right);
                }
            }
            ControlerList.Add(id,new ControlerObject(obj));
        }

        private void onLostEvent(uint id)
        {
            ControlerList.Remove(id);
        }

        private void onPressEvent(InputInteractionManager.ButtonType type, uint id, InputInteractionManager.HandPoint handpoint)
        {
            ControlerObject obj;
            if (ControlerList.TryGetValue(id, out obj))
            {
                switch (type)
                {
                    case InputInteractionManager.ButtonType.Grip:
                        obj.gripflag = true;
                        gripobject = MinDistanceObject(handpoint.pos, 0.1f);
                        IMoveInterface[] mi = SearchInterfaceObject<IMoveInterface>(gripobject);
                        if (mi != null)
                        {
                            for (int i = 0; i < mi.Length; i++)
                            {
                                mi[i].ObjectStartMove(handpoint.pos, handpoint.rot);
                            }
                        }
                        break;
                }
            }
        }

        private void onUpdateEvent(uint id, InputInteractionManager.HandPoint handpoint)
        {
            ControlerObject obj;
            if (ControlerList.TryGetValue(id, out obj))
            {
                obj.model.transform.position = handpoint.pos;
                obj.model.transform.rotation = handpoint.rot;
                if (obj.gripflag==true)
                {
                    // grip update
                    IMoveInterface[] mi = SearchInterfaceObject<IMoveInterface>(gripobject);
                    if (mi != null)
                    {
                        for (int i = 0; i < mi.Length; i++)
                        {
                            mi[i].ObjectUpdateMove(handpoint.pos, handpoint.rot);
                        }
                    }
                }
            }
        }

        private void onReleaseEvent(InputInteractionManager.ButtonType type, uint id, InputInteractionManager.HandPoint handpoint)
        {
            ControlerObject obj;
            if (ControlerList.TryGetValue(id, out obj))
            {
                switch (type)
                {
                    case InputInteractionManager.ButtonType.Select:
                        GameObject selectobj = MinDistanceObject(handpoint.pos, 0.1f);
                        ISelectInterface[] si = SearchInterfaceObject<ISelectInterface>(selectobj);
                        if (si != null)
                        {
                            for (int i = 0; i < si.Length; i++)
                            {
                                si[i].ObjectSelect();
                            }
                        }
                        break;
                    case InputInteractionManager.ButtonType.Grip:
                        obj.gripflag = false;
                        break;
                }
            }
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

        private GameObject MinDistanceObject(Vector3 pos,float maxdistance)
        {
            var collider = Physics.OverlapSphere(pos, maxdistance);
            if (collider != null)
            {
                float distance = 0.0f;
                int num = 0;
                for (int i = 0; i < collider.Length; i++)
                {
                    float buf = Vector3.Distance(pos, collider[i].gameObject.transform.position);
                    if (i == 0)
                    {
                        distance = buf;
                    }
                    else
                    {
                        if (distance > buf)
                        {
                            distance = buf;
                            num = i;
                        }
                    }
                }
                return collider[num].gameObject;
            }
            else
            {
                return null;
            }
        }
    }

    public interface ISelectInterface
    {
        void ObjectSelect();
    }

    public interface IMoveInterface
    {
        void ObjectStartMove(Vector3 pos,Quaternion rot);
        void ObjectUpdateMove(Vector3 pos,Quaternion rot);
    }

    public interface IPointerInterface
    {
        void SetDeviceType(PointerManager.DeviceType type);
    }
}
