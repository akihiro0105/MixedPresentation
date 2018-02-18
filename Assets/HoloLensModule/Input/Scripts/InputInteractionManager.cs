using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
using UnityEngine.VR.WSA.Input;
#else
using UnityEngine.XR.WSA.Input;
#endif
#endif

namespace HoloLensModule.Input
{
    public class InputInteractionManager : MonoBehaviour
    {
        public delegate void InputInteractionEventHandler(uint id,HandPoint handpoint);
        public InputInteractionEventHandler onDetectEvent;
        public InputInteractionEventHandler onUpdateEvent;
        public delegate void InputInteractionLostEventHandler(uint id);
        public InputInteractionLostEventHandler onLostEvent;
        public delegate void InputInteractionButtonEventHandler(ButtonType type, uint id, HandPoint handpoint);
        public InputInteractionButtonEventHandler onPressEvent;
        public InputInteractionButtonEventHandler onReleaseEvent;

        [Serializable]
        public enum DeviceType
        {
           Right,
           Left,
           Unknown
        }

        [Serializable]
        public enum ButtonType
        {
            Press,
            Select,
            Grip,
            Menu,
            Touchpad,
            Stick
        }

        public class HandPoint
        {
            private DeviceType type;
            public DeviceType devicetype { get { return type; } }
            public Vector3 pos;
            public Quaternion rot;
            public bool select = false;
            public float select_amount = 0.0f;
            public bool grip = false;
            public bool menu = false;
            public Vector2? touchpad = null;
            public bool touchpad_press = false;
            public Vector2? stick = null;
            public bool stick_press = false;
            public float trackingstate = 0.0f;

            public HandPoint(Vector3 pos)
            {
                type = DeviceType.Unknown;
                this.pos = pos;
            }

            public HandPoint(Vector3 pos,Quaternion rot,bool isRight)
            {
                type = (isRight == true) ? DeviceType.Right : DeviceType.Left;
                this.pos = pos;
                this.rot = rot;
            }
        }
        private Dictionary<uint, HandPoint> HandList = new Dictionary<uint, HandPoint>();
        public Dictionary<uint,HandPoint> GetHandList() { return HandList; }

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
            InteractionManager.SourceDetected += SourceDetected;
            InteractionManager.SourceUpdated += SourceUpdated;
            InteractionManager.SourceLost += SourceLost;
            InteractionManager.SourcePressed += SourcePressed;
            InteractionManager.SourceReleased += SourceReleased;
#else
            InteractionManager.InteractionSourceDetected += SourceDetected;
            InteractionManager.InteractionSourceUpdated += SourceUpdated;
            InteractionManager.InteractionSourceLost += SourceLost;
            InteractionManager.InteractionSourcePressed += SourcePressed;
            InteractionManager.InteractionSourceReleased += SourceReleased;
#endif
#endif
        }

        void OnDestroy()
        {
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
            InteractionManager.SourceDetected -= SourceDetected;
            InteractionManager.SourceUpdated -= SourceUpdated;
            InteractionManager.SourceLost -= SourceLost;
            InteractionManager.SourcePressed -= SourcePressed;
            InteractionManager.SourceReleased -= SourceReleased;
#else
            InteractionManager.InteractionSourceDetected -= SourceDetected;
            InteractionManager.InteractionSourceUpdated -= SourceUpdated;
            InteractionManager.InteractionSourceLost -= SourceLost;
            InteractionManager.InteractionSourcePressed -= SourcePressed;
            InteractionManager.InteractionSourceReleased -= SourceReleased;
#endif
#endif
        }

#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
        void SourceDetected(InteractionSourceState state)
        {
         Vector3 v;
            if (state.properties.location.TryGetPosition(out v) == true)
            {
                    HandList.Add(state.source.id, new HandPoint(v));
                HandPoint handpoint;
                HandList.TryGetValue(state.source.id, out handpoint);
                if (onDetectEvent != null) onDetectEvent(state.source.id, handpoint);
            }
        }

        void SourceUpdated(InteractionSourceState state)
        {
        Vector3 v;
            if (state.properties.location.TryGetPosition(out v) == true)
            {
                HandPoint handpoint;
                if (HandList.TryGetValue(state.source.id,out handpoint))
                {
                    handpoint.pos = v;
                    handpoint.select = state.pressed;
                    if (onUpdateEvent != null) onUpdateEvent(state.source.id, handpoint);
                }else{
         HandList.Add(state.source.id, new HandPoint(v));
        HandList.TryGetValue(state.source.id, out handpoint);
                if (onDetectEvent != null) onDetectEvent(state.source.id, handpoint);
        }
            }
        }

        void SourceLost(InteractionSourceState state)
        {
            HandList.Remove(state.source.id);
            if (onLostEvent != null) onLostEvent(state.source.id);
        }

        void SourcePressed(InteractionSourceState state)
        {
        HandPoint handpoint;
                if (HandList.TryGetValue(state.source.id,out handpoint))
                {
        if (onPressEvent != null) onPressEvent(ButtonType.Press, state.source.id,handpoint);
        }
        }

        void SourceReleased(InteractionSourceState state)
        {
        HandPoint handpoint;
                if (HandList.TryGetValue(state.source.id,out handpoint))
                {
        if (onReleaseEvent != null) onReleaseEvent(ButtonType.Press, state.source.id,handpoint);
        }
        }

#else
        void SourceDetected(InteractionSourceDetectedEventArgs state)
        {
            Debug.Log("Detect Hand");
            Vector3 v;
            if (state.state.sourcePose.TryGetPosition(out v) == true)
            {
                Quaternion r;
                state.state.sourcePose.TryGetRotation(out r);
                if (state.state.source.handedness == InteractionSourceHandedness.Right)
                {
                    HandList.Add(state.state.source.id, new HandPoint(v, r, true));
                }
                else if (state.state.source.handedness == InteractionSourceHandedness.Left)
                {
                    HandList.Add(state.state.source.id, new HandPoint(v, r, false));
                }
                else
                {
                    HandList.Add(state.state.source.id, new HandPoint(v));
                }
                HandPoint handpoint;
                HandList.TryGetValue(state.state.source.id, out handpoint);
                if (onDetectEvent != null) onDetectEvent(state.state.source.id, handpoint);
            }
        }

        void SourceUpdated(InteractionSourceUpdatedEventArgs state)
        {
            Vector3 v;
            Quaternion r;
            if (state.state.sourcePose.TryGetPosition(out v) == true)
            {
                state.state.sourcePose.TryGetRotation(out r);
                HandPoint handpoint;
                if (HandList.TryGetValue(state.state.source.id,out handpoint))
                {
                    handpoint.pos = v;
                    handpoint.rot = r;
                    handpoint.select_amount = state.state.selectPressedAmount;
                    handpoint.menu = state.state.menuPressed;
                    handpoint.grip = state.state.grasped;
                    handpoint.stick_press = state.state.thumbstickPressed;
                    handpoint.stick = state.state.thumbstickPosition;
                    handpoint.touchpad_press = state.state.touchpadPressed;
                    if (state.state.sourcePose.positionAccuracy==InteractionSourcePositionAccuracy.High)
                    {
                        if (state.state.properties.sourceLossRisk<1.0f)
                        {
                            handpoint.trackingstate = 1.0f;
                        }
                        else
                        {
                            handpoint.trackingstate = 0.5f;
                        }
                    }
                    else
                    {
                        handpoint.trackingstate = 0.0f;
                    }
                    if (state.state.touchpadTouched==true)
                    {
                        handpoint.touchpad = state.state.touchpadPosition;
                    }
                    else
                    {
                        handpoint.touchpad = null;
                    }
                    if (onUpdateEvent != null) onUpdateEvent(state.state.source.id, handpoint);
                }
                else
                {
                    if (state.state.source.handedness == InteractionSourceHandedness.Right)
                    {
                        HandList.Add(state.state.source.id, new HandPoint(v, r, true));
                    }
                    else if (state.state.source.handedness == InteractionSourceHandedness.Left)
                    {
                        HandList.Add(state.state.source.id, new HandPoint(v, r, false));
                    }
                    else
                    {
                        HandList.Add(state.state.source.id, new HandPoint(v));
                    }
                    HandList.TryGetValue(state.state.source.id, out handpoint);
                    if (onDetectEvent != null) onDetectEvent(state.state.source.id, handpoint);
                }
            }
        }

        void SourceLost(InteractionSourceLostEventArgs state)
        {
            Debug.Log("Lost Hand");
            HandList.Remove(state.state.source.id);
            if (onLostEvent != null) onLostEvent(state.state.source.id);
        }

        void SourcePressed(InteractionSourcePressedEventArgs state)
        {
            Debug.Log("Press Hand : "+ state.pressType);
            HandPoint handpoint;
            if (HandList.TryGetValue(state.state.source.id, out handpoint))
            {
                switch (state.pressType)
                {
                    case InteractionSourcePressType.None:
                        handpoint.select = true;
                        if (onPressEvent != null) onPressEvent(ButtonType.Press, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Select:
                        handpoint.select = true;
                        if (onPressEvent != null) onPressEvent(ButtonType.Select, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Menu:
                        if (onPressEvent != null) onPressEvent(ButtonType.Menu, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Grasp:
                        if (onPressEvent != null) onPressEvent(ButtonType.Grip, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Touchpad:
                        if (onPressEvent != null) onPressEvent(ButtonType.Touchpad, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Thumbstick:
                        if (onPressEvent != null) onPressEvent(ButtonType.Stick, state.state.source.id, handpoint);
                        break;
                    default:
                        break;
                }
            }
        }

        void SourceReleased(InteractionSourceReleasedEventArgs state)
        {
            Debug.Log("Release Hand");
            HandPoint handpoint;
            if (HandList.TryGetValue(state.state.source.id, out handpoint))
            {
                switch (state.pressType)
                {
                    case InteractionSourcePressType.None:
                        handpoint.select = false;
                        if (onReleaseEvent != null) onReleaseEvent(ButtonType.Press, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Select:
                        handpoint.select = false;
                        if (onReleaseEvent != null) onReleaseEvent(ButtonType.Select, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Menu:
                        if (onReleaseEvent != null) onReleaseEvent(ButtonType.Menu, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Grasp:
                        if (onReleaseEvent != null) onReleaseEvent(ButtonType.Grip, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Touchpad:
                        if (onReleaseEvent != null) onReleaseEvent(ButtonType.Touchpad, state.state.source.id, handpoint);
                        break;
                    case InteractionSourcePressType.Thumbstick:
                        if (onReleaseEvent != null) onReleaseEvent(ButtonType.Stick, state.state.source.id, handpoint);
                        break;
                    default:
                        break;
                }
            }
        }
#endif
#endif
    }
}
