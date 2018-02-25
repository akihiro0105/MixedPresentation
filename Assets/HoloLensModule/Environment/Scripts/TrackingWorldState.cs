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

namespace HoloLensModule.Environment
{
    public class TrackingWorldState : MonoBehaviour
    {
        public delegate void TrackingWorldStateEventHandler(bool isActive);
        public TrackingWorldStateEventHandler TrackingWorldStateEvent;

#if UNITY_EDITOR || UNITY_UWP
        [HideInInspector]
        public PositionalLocatorState state;
#endif

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR || UNITY_UWP
            WorldManager.OnPositionalLocatorStateChanged += OnPositionalLocatorStateChanged;
#endif
        }

        void OnDestroy()
        {
#if UNITY_EDITOR || UNITY_UWP
            WorldManager.OnPositionalLocatorStateChanged -= OnPositionalLocatorStateChanged;
#endif
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR || UNITY_UWP
            state = WorldManager.state;
            switch (state)
            {
                case PositionalLocatorState.Active:
                    // tracking active
                    break;
                case PositionalLocatorState.Unavailable:
                case PositionalLocatorState.OrientationOnly:
                case PositionalLocatorState.Activating:
                case PositionalLocatorState.Inhibited:
                default:
                    // tracking lost
                    break;
            }
#endif
        }

#if UNITY_EDITOR || UNITY_UWP
        private void OnPositionalLocatorStateChanged(PositionalLocatorState oldstate, PositionalLocatorState newstate)
        {
            if (newstate == PositionalLocatorState.Active)
            {
                // becomimg tracking active
                if (TrackingWorldStateEvent != null) TrackingWorldStateEvent(true);
            }
            else
            {
                // becomimg tracking lost
                if (TrackingWorldStateEvent != null) TrackingWorldStateEvent(false);
            }
        }
#endif
    }
}
