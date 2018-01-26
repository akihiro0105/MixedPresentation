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
    public class HandPressManager : HoloLensModuleSingleton<HandPressManager>
    {
        public GameObject GazeModel;

        public delegate void HandsEventHandler();
        public static event HandsEventHandler onReleased;

        private GazeControl gazecontrol;
        // Use this for initialization
        void Start()
        {
            if (GazeModel != null)
            {
                gazecontrol = GazeModel.GetComponent<GazeControl>();
            }
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
            InteractionManager.SourceDetected += SourceDetected;
            InteractionManager.SourceLost += SourceLost;
            InteractionManager.SourcePressed += SourcePressed;
            InteractionManager.SourceReleased += SourceReleased;
#else
            InteractionManager.InteractionSourceDetected += SourceDetected;
            InteractionManager.InteractionSourceLost += SourceLost;
            InteractionManager.InteractionSourcePressed += SourcePressed;
            InteractionManager.InteractionSourceReleased += SourceReleased;
#endif
#endif
        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
            InteractionManager.SourceDetected -= SourceDetected;
            InteractionManager.SourceLost -= SourceLost;
            InteractionManager.SourcePressed -= SourcePressed;
            InteractionManager.SourceReleased -= SourceReleased;
#else
            InteractionManager.InteractionSourceDetected -= SourceDetected;
            InteractionManager.InteractionSourceLost -= SourceLost;
            InteractionManager.InteractionSourcePressed -= SourcePressed;
            InteractionManager.InteractionSourceReleased -= SourceReleased;
#endif
#endif
            base.OnDestroy();
        }


#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
        void SourceDetected(InteractionSourceState state)
        {
            if (GazeModel != null)
            {
                gazecontrol.Hand();
            }
        }

        void SourceLost(InteractionSourceState state)
        {
            if (GazeModel != null)
            {
                gazecontrol.Normal();
            }
        }

        void SourcePressed(InteractionSourceState state)
        {
         if (GazeModel != null)
            {
                gazecontrol.Press();
            }
        }

        void SourceReleased(InteractionSourceState state)
        {
        if (GazeModel != null)
            {
                gazecontrol.Hand();
            }
            if (onReleased != null) onReleased();
        }
#else
        void SourceDetected(InteractionSourceDetectedEventArgs state)
        {
            if (GazeModel != null)
            {
                gazecontrol.Hand();
            }
        }

        void SourceLost(InteractionSourceLostEventArgs state)
        {
            if (GazeModel != null)
            {
                gazecontrol.Normal();
            }
        }

        void SourcePressed(InteractionSourcePressedEventArgs state)
        {
            if (GazeModel != null)
            {
                gazecontrol.Press();
            }
        }

        void SourceReleased(InteractionSourceReleasedEventArgs state)
        {
            if (GazeModel != null)
            {
                gazecontrol.Hand();
            }
            if (onReleased != null) onReleased();
        }
#endif
#endif
    }
}
