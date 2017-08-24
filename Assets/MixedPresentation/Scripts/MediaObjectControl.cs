using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace MixedPresentation
{
    // pathからメディアデータを取得
    // HoloLens : StreamingFolder,LocalFolder
    // Desktop : StreamingFolder
    public class MediaObjectControl : MonoBehaviour
    {
        public enum MediaType
        {
            None,
            Image,
            Audio,
            Video
        }
        [SerializeField]
        private MediaType type = MediaType.None;
        [SerializeField]
        private string path = "";
        [SerializeField]
        private bool onAwake = false;
        public bool playOnAwake = false;
        public Texture defaultTexture = null;
        [Header("Image")]
        public float ImageScale = 1000;
        [Header("Audio&Video")]
        public bool isLoop = false;

        private AudioSource audiosource;
        private VideoPlayer videoplayer;

        // Use this for initialization
        void Start()
        {
            audiosource = GetComponent<AudioSource>();
            videoplayer = GetComponent<VideoPlayer>();
            if (onAwake) LoadMedia();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadMedia(string path,MediaType type,bool playOnAwake = false)
        {
            this.type = type;
            this.path = path;
            this.playOnAwake = playOnAwake;
            LoadMedia();
        }

        public void LoadMedia()
        {
            StartCoroutine(Load());
        }

        IEnumerator Load()
        {
            Renderer rend = GetComponent<Renderer>();
            using (WWW www=new WWW(path))
            {
                yield return www;
                switch (type)
                {
                    case MediaType.Image:
                        Texture tex = www.texture;
                        rend.material.mainTexture = tex;
                        Vector3 scale = gameObject.transform.localScale;
                        scale.x = (float)tex.width / ImageScale;
                        scale.y = (float)tex.height / ImageScale;
                        gameObject.transform.localScale = scale;
                        break;
                    case MediaType.Audio:
                        if (defaultTexture != null) rend.material.mainTexture = defaultTexture;
                        audiosource.clip = www.GetAudioClip(true, true);
                        audiosource.loop = isLoop;
                        if (playOnAwake) audiosource.Play();
                        break;
                    case MediaType.Video:
                        videoplayer.url = www.url;
                        videoplayer.isLooping = isLoop;
                        videoplayer.aspectRatio = VideoAspectRatio.FitInside;
                        if (playOnAwake) videoplayer.Play();
                        break;
                    default:
                        if (defaultTexture != null) rend.material.mainTexture = defaultTexture;
                        break;
                }
            }
        }

        public void Play()
        {
            if(type==MediaType.Audio) audiosource.Play();
            else if(type==MediaType.Video) videoplayer.Play();
        }

        public void Stop()
        {
            if (type == MediaType.Audio) audiosource.Stop();
            else if (type == MediaType.Video) videoplayer.Stop();
        }
    }
}
