using HoloLensModule.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using HoloLensModule.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MixedPresentation
{
    public class MediaControl : MonoBehaviour, FocusInterface
    {
        public GameObject MediaPanel;
        public Texture defaultTexture = null;
        public enum MediaType
        {
            None,
            Image,
            Audio,
            Video,
            Asset
        }
        private MediaType type = MediaType.None;
        private bool isPlay = false;
        private AudioSource audiosource;
        private VideoPlayer videoplayer;

        private Boundingbox boundingbox = null;
        private bool focusflag = false;
        private bool tapflag = false;
        private Vector3 bufpos;
        private Quaternion bufrot;
        private Vector3 bufscale;

#if UNITY_EDITOR
        [CustomEditor(typeof(MediaControl))]
        public class CustomInspector : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (GUILayout.Button("Play"))
                {
                    Debug.Log("Export Transform");
                    ((MediaControl)target).SetPlayButton();
                }
            }
        }
#endif

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            boundingbox = GetComponent<Boundingbox>();
            audiosource = MediaPanel.GetComponent<AudioSource>();
            videoplayer = MediaPanel.GetComponent<VideoPlayer>();
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
        }

        public IEnumerator LoadMedia(string path, MediaType type)
        {
            this.type = type;
            Renderer render = MediaPanel.GetComponent<Renderer>();
            using (WWW www = new WWW(path))
            {
                yield return www;
                switch (type)
                {
                    case MediaType.Image:
                        Texture tex = www.texture;
                        render.material.mainTexture = tex;
                        Vector3 scale = MediaPanel.transform.localScale;
                        scale.x = (float)tex.width / 1000;
                        scale.y = (float)tex.height / 1000;
                        MediaPanel.transform.localScale = scale;
                        break;
                    case MediaType.Audio:
                        if (defaultTexture != null) render.material.mainTexture = defaultTexture;
                        audiosource.clip = www.GetAudioClip(true, true);
                        break;
                    case MediaType.Video:
                        videoplayer.url = www.url;
                        videoplayer.aspectRatio = VideoAspectRatio.FitInside;
                        break;
                    case MediaType.Asset:
                        MediaPanel.SetActive(false);
                        AssetBundleRequest bundlerequest = www.assetBundle.LoadAllAssetsAsync();
                        yield return bundlerequest;
                        object[] obj = bundlerequest.allAssets;
                        for (int i = 0; i < obj.Length; i++) Instantiate((GameObject)obj[i], transform);
                        www.assetBundle.Unload(false);
                        www.Dispose();
                        break;
                    default:
                        break;
                }
            }
            ResetTransform();
            //boundingbox.isActive(false,true);
        }

#if UNITY_EDITOR
        public void SetPlayButton()
        {
            tapflag = true;
            isPlay = !isPlay;
            SetPlay(isPlay);
        }
#endif

        public void SetPlay(bool flag)
        {
            if (type == MediaType.Audio)
            {
                if (flag) audiosource.Play();
                else audiosource.Stop();
            }
            else if (type == MediaType.Video)
            {
                if (flag) videoplayer.Play();
                else videoplayer.Stop();
            }
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state == HandsGestureManager.HandGestureState.DoubleTap && focusflag == true)
            {
                tapflag = true;
                isPlay = !isPlay;
                SetPlay(isPlay);
            }
        }

        public bool GetTapFlag(out bool play)
        {
            play = isPlay;
            if (tapflag == false) return false;
            tapflag = false;
            return true;
        }

        public void FocusEnter()
        {
            focusflag = true;
            //if (boundingbox) boundingbox.isActive(focusflag);
        }

        public void FocusEnd()
        {
            focusflag = false;
            //if (boundingbox) boundingbox.isActive(focusflag);
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
