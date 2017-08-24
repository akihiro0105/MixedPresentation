using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_UWP
using System;
using System.Threading.Tasks;
using Windows.Storage;
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif

namespace MixedPresentation
{
    // 指定フォルダ内のメディアデータを読み込み
    // Desktop : Streaming
    // HoloLens : Streaming,Local
    public class MediaObjectManager : MonoBehaviour
    {
        public GameObject MediaObjectPrefab;

        public delegate void MediaLoadCompleteEventHandler(List<GameObject> objs);
        public MediaLoadCompleteEventHandler MediaLoadCompleteEvent;

        private List<GameObject> MediaObjects = new List<GameObject>();
        private Dictionary<string, string> MediaName = new Dictionary<string, string>();// name,path
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadMediaObjects()
        {
            DirectoryInfo info = new DirectoryInfo(Application.streamingAssetsPath);
            if (info != null)
            {
                FileInfo[] filesname = info.GetFiles();
                foreach (var item in filesname)
                {
                    MediaName.Add(item.Name, item.FullName);
                }
            }
#if UNITY_UWP
            Task.Run(async()=> {
                var file = await ApplicationData.Current.LocalFolder.GetFilesAsync();
                if(file!=null)
                {
                    foreach (var item in file)
                    {
                        MediaName.Add(item.Name, ApplicationData.Current.LocalFolder.Path+"\\"+item.Name);
                    }
                }
                UnityEngine.WSA.Application.InvokeOnAppThread(() => { StartCoroutine(LoadMedia()); }, true);
            });
#elif UNITY_EDITOR || UNITY_STANDALONE
            StartCoroutine(LoadMedia());
#endif
        }

        private IEnumerator LoadMedia()
        {
            foreach (var item in MediaName)
            {
                string path = "file://" + item.Value;
                if (item.Key.IndexOf(".meta") > 0) { }
                else if (item.Key.IndexOf(".mp4") > 0) CreateMediaObject(item.Key, path, MediaObjectControl.MediaType.Video);
                else if (item.Key.IndexOf(".png") > 0) CreateMediaObject(item.Key, path, MediaObjectControl.MediaType.Image);
                else if (item.Key.IndexOf(".jpg") > 0) CreateMediaObject(item.Key, path, MediaObjectControl.MediaType.Image);
                else if (item.Key.IndexOf(".wav") > 0) CreateMediaObject(item.Key, path, MediaObjectControl.MediaType.Audio);
                else if (item.Key.IndexOf(".ogg") > 0) CreateMediaObject(item.Key, path, MediaObjectControl.MediaType.Audio);
                else if (item.Key.IndexOf(".manifest") > 0) { }
                else if (item.Key.IndexOf(".position") > 0) { }
                else if (item.Key.IndexOf(".dat") > 0) { }
                else if (item.Key.IndexOf(".bak") > 0) { }
                else if (item.Key.IndexOf(".dummy") > 0) { }
                else CreateAssetBundle(item.Key, path);// assetbundle
                yield return null;
            }
            if (MediaLoadCompleteEvent != null) MediaLoadCompleteEvent(MediaObjects);
        }

        private void CreateMediaObject(string name,string path,MediaObjectControl.MediaType type)
        {
            GameObject obj = Instantiate(MediaObjectPrefab);
            obj.name = name;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(0,0,1);
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localRotation = Quaternion.Euler(0, 180, 0);
            MediaObjectControl moc = obj.GetComponent<MediaObjectControl>();
            moc.LoadMedia(path, type);
            MediaObjects.Add(obj);
        }

        private IEnumerator CreateAssetBundle(string name,string path)
        {
            using (WWW www = new WWW(path))
            {
                yield return www;
                AssetBundle bundle = www.assetBundle;
                AssetBundleRequest bundlerequest = bundle.LoadAllAssetsAsync();
                yield return bundlerequest;
                object[] obj = bundlerequest.allAssets;
                for (int i = 0; i < obj.Length; i++)
                {
                    GameObject gobj = Instantiate((GameObject)obj[i]);
                    gobj.transform.SetParent(transform);
                    gobj.name = name;
                    MediaObjects.Add(gobj);
                }
                bundle.Unload(false);
                www.Dispose();
            }
        }
    }
}
