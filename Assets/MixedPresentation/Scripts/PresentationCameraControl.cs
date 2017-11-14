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
        private bool tapflag = false;
        private Vector3 bufpos;
        private Quaternion bufrot;
        private Vector3 bufscale;

        // Use this for initialization
        void Start()
        {
            boundingbox = GetComponent<Boundingbox>();
            ResetTransform();
            CameraObject.GetComponent<Renderer>().material = UnActiveMaterial;
        }

        public void isActiveCamera(bool flag)
        {
            if (flag) CameraObject.GetComponent<Renderer>().material = ActiveMaterial;
            else CameraObject.GetComponent<Renderer>().material = UnActiveMaterial;
        }

        public void SetTapAction()
        {
            tapflag = true;
        }

        public bool GetTapFlag()
        {
            if (tapflag == false) return false;
            tapflag = false;
            return true;
        }

        public void FocusEnd()
        {
            if (boundingbox) boundingbox.isActive(false);
        }

        public void FocusEnter()
        {
            if (boundingbox) boundingbox.isActive(true);
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
