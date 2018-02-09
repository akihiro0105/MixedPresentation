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
    public class GestureManager : MonoBehaviour
    {
        public float DragDistance = 0.02f;
        public float HoldTriggerTime = 1.0f;

        public delegate void GestureEventHandler(GestureState state, Vector3? pos1 = null, Vector3? pos2 = null,float holdtime = 0.0f);
        public GestureEventHandler GestureEvent;
        public GestureEventHandler GestureUpdateEvent;

        [Serializable]
        public enum GestureState
        {
            Tap,
            Drag,
            Hold,
            ShiftTap,
            ShiftDrag,
            ShiftHold,
            MultiTap,
            MultiDrag
        }

        private InputInteractionManager interactionmanager;
        public class GesturePoint
        {
            public float t;
            public Vector3 pos;
            public float distance;
            public GesturePoint(Vector3 pos)
            {
                this.pos = pos;
                distance = 0.0f;
                t = Time.time;
            }
        }
        private Dictionary<uint, GesturePoint> GestureList = new Dictionary<uint, GesturePoint>();
        private bool DragFlag = false;

        // Use this for initialization
        void Start()
        {
            interactionmanager = GetComponent<InputInteractionManager>();
#if UNITY_EDITOR || UNITY_UWP
#if UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque == false)
            {
#endif
                interactionmanager.onLostEvent += onLostEvent;
                interactionmanager.onPressEvent += onPressEvent;
                interactionmanager.onUpdateEvent += onUpdateEvent;
                interactionmanager.onReleaseEvent += onReleaseEvent;
#if UNITY_2017_2_OR_NEWER
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
                interactionmanager.onLostEvent -= onLostEvent;
                interactionmanager.onPressEvent -= onPressEvent;
                interactionmanager.onUpdateEvent -= onUpdateEvent;
                interactionmanager.onReleaseEvent -= onReleaseEvent;
#if UNITY_2017_2_OR_NEWER
            }
#endif
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void onLostEvent(uint id)
        {
            onReleaseEvent(InputInteractionManager.ButtonType.Press, id, null);
        }

        private void onPressEvent(InputInteractionManager.ButtonType type, uint id, InputInteractionManager.HandPoint handpoint)
        {
            GestureList.Add(id, new GesturePoint(handpoint.pos));
        }

        private void onUpdateEvent(uint id, InputInteractionManager.HandPoint handpoint)
        {
            GesturePoint gesturepoint;
            if (GestureList.TryGetValue(id, out gesturepoint))
            {
                gesturepoint.distance += Vector3.Distance(gesturepoint.pos, handpoint.pos);
                gesturepoint.pos = handpoint.pos;
                if (gesturepoint.distance > DragDistance)
                {
                    if (GestureList.Count == 1)
                    {
                        if (interactionmanager.GetHandList().Count == 1)
                        {
                            if (DragFlag == false)
                            {
                                Debug.Log("Gesture Drag");
                                if (GestureEvent != null) GestureEvent(GestureState.Drag, gesturepoint.pos);
                            }
                            else
                            {
                                if (GestureUpdateEvent != null) GestureUpdateEvent(GestureState.Drag, gesturepoint.pos);
                            }
                        }
                        else
                        {
                            if (DragFlag == false)
                            {
                                Debug.Log("Gesture ShiftDrag");
                                if (GestureEvent != null) GestureEvent(GestureState.ShiftDrag, gesturepoint.pos);
                            }
                            else
                            {
                                if (GestureUpdateEvent != null) GestureUpdateEvent(GestureState.ShiftDrag, gesturepoint.pos);
                            }
                        }
                    }
                    else
                    {
                        GesturePoint[] points = new GesturePoint[GestureList.Count];
                        GestureList.Values.CopyTo(points, 0);
                        if (DragFlag == false)
                        {
                            Debug.Log("Gesture MultiDrag");
                            if (GestureEvent != null) GestureEvent(GestureState.MultiDrag, points[0].pos, points[1].pos);
                        }
                        else
                        {
                            if (GestureUpdateEvent != null) GestureUpdateEvent(GestureState.MultiDrag, points[0].pos, points[1].pos);
                        }
                    }
                    DragFlag = true;
                }
                else
                {
                    if (GestureList.Count == 1)
                    {
                        if (interactionmanager.GetHandList().Count == 1)
                        {
                            if (GestureUpdateEvent != null) GestureUpdateEvent(GestureState.Hold, gesturepoint.pos, null, Time.time - gesturepoint.t);
                        }
                        else
                        {
                            if (GestureUpdateEvent != null) GestureUpdateEvent(GestureState.ShiftHold, gesturepoint.pos, null, Time.time - gesturepoint.t);
                        }
                    }
                }
            }
        }

        private void onReleaseEvent(InputInteractionManager.ButtonType type, uint id, InputInteractionManager.HandPoint handpoint)
        {
            GesturePoint gesturepoint;
            if (GestureList.TryGetValue(id, out gesturepoint))
            {
                if (DragFlag == false)
                {
                    if (GestureList.Count == 1)
                    {
                        if (Time.time - gesturepoint.t < HoldTriggerTime)
                        {
                            if (interactionmanager.GetHandList().Count == 1)
                            {
                                Debug.Log("Gesture Tap");
                                if (GestureEvent != null) GestureEvent(GestureState.Tap);
                            }
                            else
                            {
                                Debug.Log("Gesture ShiftTap");
                                if (GestureEvent != null) GestureEvent(GestureState.ShiftTap);
                            }
                        }
                        else
                        {
                            if (interactionmanager.GetHandList().Count == 1)
                            {
                                Debug.Log("Gesture Hold");
                                if (GestureEvent != null) GestureEvent(GestureState.Hold);
                            }
                            else
                            {
                                Debug.Log("Gesture ShiftHold");
                                if (GestureEvent != null) GestureEvent(GestureState.ShiftHold);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Gesture MultiTap");
                        if (GestureEvent != null) GestureEvent(GestureState.MultiTap);
                    }
                }
                DragFlag = false;
                GestureList.Clear();
            }
        }
    }
}
