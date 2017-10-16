using HoloLensModule.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MixedPresentation
{
    public class PresentationCameraControl : MonoBehaviour, FocusInterface
    {
        public Material ActiveMaterial;
        public Material UnActiveMaterial;
        public GameObject CameraObject;

        private Boundingbox boundingbox;
        private bool focusflag = false;
        private bool tapflag = false;
        private Vector3 bufpos;
        private Quaternion bufrot;
        private Vector3 bufscale;

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            boundingbox = GetComponent<Boundingbox>();
            ResetTransform();
            CameraObject.GetComponent<Renderer>().material = UnActiveMaterial;
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
        }

        public void isActiveCamera(bool flag)
        {
            if (flag) CameraObject.GetComponent<Renderer>().material = ActiveMaterial;
            else CameraObject.GetComponent<Renderer>().material = UnActiveMaterial;
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state == HandsGestureManager.HandGestureState.DoubleTap && focusflag == true) tapflag = true;
        }

        public bool GetTapFlag()
        {
            if (tapflag == false) return false;
            tapflag = false;
            return true;
        }

        public void FocusEnd()
        {
            focusflag = false;
            if (boundingbox) boundingbox.isActive(focusflag);
        }

        public void FocusEnter()
        {
            focusflag = true;
            if (boundingbox) boundingbox.isActive(focusflag);
        }

        public bool GetTransform()
        {
            if (transform.localPosition != bufpos || transform.localRotation != bufrot || transform.localScale != bufscale)
            {
                ResetTransform();
                return true;
            }
            else return false;
        }

        public void ResetTransform()
        {
            bufpos = transform.localPosition;
            bufrot = transform.localRotation;
            bufscale = transform.localScale;
        }
    }
}
