using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Input;
using System;

namespace MixedPresentation
{
    public class MediaTapControl : MonoBehaviour, FocusInterface
    {
        private bool focusflag = false;
        private MediaObjectControl media;

        public void FocusEnd() { focusflag = false; }

        public void FocusEnter() { focusflag = true; }

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            media = this.transform.GetChild(0).gameObject.GetComponent<MediaObjectControl>();
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state == HandsGestureManager.HandGestureState.DoubleTap && focusflag == true)
            {
                if (media.isPlay()) media.Stop();
                else media.Play();
            }
        }

        public void SetPlay(bool flag)
        {
            if (flag)
            {
                if (!media.isPlay()) media.Play();
            }
            else if (media.isPlay()) media.Stop();
        }

        public bool GetPlay()
        {
            if (media.isPlay()) return true;
            else return false;
        }
    }
}
