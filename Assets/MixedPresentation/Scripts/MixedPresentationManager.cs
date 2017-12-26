using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using HoloLensModule.Network;
using HoloLensModule.Input;
#if UNITY_UWP
using System.Threading.Tasks;
using Windows.Storage;
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif

namespace MixedPresentation
{
    public class MixedPresentationManager : MonoBehaviour
    {
        public GameObject DesktopCamera;
        public GameObject[] PresentationCameras;// 1^9
        public GameObject MedioObjectPrefab;

        private string SaveDirectoryName = "PresentationContent";
        private Dictionary<string, GameObject> MediaObject = new Dictionary<string, GameObject>();
        private JsonSaveGameobject MediaJsonObject = new JsonSaveGameobject();
        private bool isLoading = false;
        private TransformImportExportManager importexpot;
        private JsonMessageControl jsonmessagecontrol;

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            isLoading = false;
            importexpot = GetComponent<TransformImportExportManager>();
            importexpot.SaveDirectoryName = SaveDirectoryName;
            importexpot.TransformImportEvent += SetJsonTransformGameObject;

            Dictionary<string, string> MediaName = new Dictionary<string, string>();
#if UNITY_UWP
            Task task= Task.Run(async()=> {
            var folder=await ApplicationData.Current.LocalFolder.CreateFolderAsync(SaveDirectoryName,CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFilesAsync();
                if(file!=null)foreach (var item in file) MediaName.Add(item.Name, ApplicationData.Current.LocalFolder.Path+"\\"+SaveDirectoryName+"\\"+item.Name);
            });
            task.Wait();
#elif UNITY_EDITOR || UNITY_STANDALONE
            string directorypath = Application.dataPath + "\\..\\" + SaveDirectoryName;
            Directory.CreateDirectory(directorypath);
            DirectoryInfo info = new DirectoryInfo(directorypath);
            FileInfo[] files = info.GetFiles();
            for (int i = 0; i < files.Length; i++) MediaName.Add(files[i].Name, files[i].FullName);
#endif
            StartCoroutine(LoadMediaCoroutine(MediaName));

            if (DesktopCamera != null) SetPresentationCamera(0);
            for (int i = 0; i < PresentationCameras.Length; i++) MediaObject.Add(PresentationCameras[i].name, PresentationCameras[i]);

            jsonmessagecontrol = GetComponent<JsonMessageControl>();
            if (jsonmessagecontrol==null) gameObject.AddComponent<JsonMessageControl>();
            jsonmessagecontrol.ReceiveCameraJsonMessage += ReceiveCameraJsonMessage;
            jsonmessagecontrol.ReceivePlayJsonMessage += ReceivePlayJsonMessage;
            jsonmessagecontrol.ReceiveTransformJsonMessage += ReceiveTransformJsonMessage;
        }

        private IEnumerator LoadMediaCoroutine(Dictionary<string, string> MediaName)
        {
            foreach (var item in MediaName)
            {
                string path = "file://" + item.Value;
                if (item.Key.IndexOf(".mp4") > 0) yield return CreateMedia(item.Key, path, MediaControl.MediaType.Video);
                else if (item.Key.IndexOf(".png") > 0) yield return CreateMedia(item.Key, path, MediaControl.MediaType.Image);
                else if (item.Key.IndexOf(".PNG") > 0) yield return CreateMedia(item.Key, path, MediaControl.MediaType.Image);
                else if (item.Key.IndexOf(".jpg") > 0) yield return CreateMedia(item.Key, path, MediaControl.MediaType.Image);
                else if (item.Key.IndexOf(".wav") > 0) yield return CreateMedia(item.Key, path, MediaControl.MediaType.Audio);
                else if (item.Key.IndexOf(".ogg") > 0) yield return CreateMedia(item.Key, path, MediaControl.MediaType.Audio);
                else if (item.Key.IndexOf(".assetbundle") > 0) yield return CreateMedia(item.Key, path, MediaControl.MediaType.Asset);
            }
            importexpot.ImportTransform();
            isLoading = true;
        }

        private IEnumerator CreateMedia(string name, string path, MediaControl.MediaType type)
        {
            GameObject obj = Instantiate(MedioObjectPrefab, transform);
            obj.name = name;
            yield return obj.GetComponent<MediaControl>().LoadMedia(path, type);
            MediaObject.Add(name, obj);
        }

        void OnDestroy()
        {
            importexpot.TransformImportEvent -= SetJsonTransformGameObject;
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
            jsonmessagecontrol.ReceiveCameraJsonMessage -= ReceiveCameraJsonMessage;
            jsonmessagecontrol.ReceivePlayJsonMessage -= ReceivePlayJsonMessage;
            jsonmessagecontrol.ReceiveTransformJsonMessage -= ReceiveTransformJsonMessage;
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state == HandsGestureManager.HandGestureState.ShiftHold) importexpot.ImportTransform();
            else if (state == HandsGestureManager.HandGestureState.ShiftDoubleTap)
            {
                transform.position = Camera.main.transform.position;
                transform.LookAt(Camera.main.transform.position + Camera.main.transform.forward);
            }
            else if (state == HandsGestureManager.HandGestureState.MultiDoubleTap) importexpot.ExportTransform();
        }

        private void SetJsonTransformGameObject(JsonSaveGameobject jsonsave)
        {
            if (jsonsave != null && jsonsave.gameobject != null)
            {
                for (int i = 0; i < jsonsave.gameobject.Count; i++)
                {
                    GameObject obj;
                    if (MediaObject.TryGetValue(jsonsave.gameobject[i].name, out obj))
                    {
                        JsonTransform jsontransform = jsonsave.gameobject[i].transform;
                        obj.transform.localPosition = jsontransform.position.Get();
                        obj.transform.localRotation = jsontransform.rotation.Get();
                        obj.transform.localScale = jsontransform.scale.Get();
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isLoading)
            {
                for (int i = 0; i < PresentationCameras.Length; i++)
                {
                    PresentationCameraControl cameracontrol = PresentationCameras[i].GetComponent<PresentationCameraControl>();
                    if (Input.GetKeyUp((KeyCode)i + 49) || cameracontrol.GetTapFlag() == true)
                    {
                        SetPresentationCamera(i);
                        jsonmessagecontrol.SendCameraMessage(i);
                        Debug.Log("Camera:" + PresentationCameras[i].name);
                    }
                }

                foreach (var item in MediaObject)
                {
                    JsonGameObject jgo = null;
                    for (int i = 0; i < MediaJsonObject.gameobject.Count; i++)
                    {
                        if (MediaJsonObject.gameobject[i].name == item.Key)
                        {
                            jgo = MediaJsonObject.gameobject[i];
                            break;
                        }
                    }
                    if (jgo == null)
                    {
                        jgo = new JsonGameObject();
                        jgo.name = item.Key;
                        jgo.transform.Set(item.Value.transform.localPosition, item.Value.transform.localRotation, item.Value.transform.localScale);
                        MediaJsonObject.gameobject.Add(jgo);
                    }
                    else jgo.transform.Set(item.Value.transform.localPosition, item.Value.transform.localRotation, item.Value.transform.localScale);
                    
                    MediaControl media = item.Value.GetComponent<MediaControl>();
                    if (media != null)
                    {
                        bool flag;
                        if (media.GetTapFlag(out flag))
                        {
                            jsonmessagecontrol.SendPlayMessage(item.Key, flag);
                            Debug.Log("Play:" + item.Key);
                        }
                        if (media.GetTransform())
                        {
                            jsonmessagecontrol.SendTransformMessage(jgo);
                            Debug.Log("Send:" + item.Key);
                        }
                    }
                    else
                    {
                        if (item.Value.GetComponent<PresentationCameraControl>().GetTransform())
                        {
                            jsonmessagecontrol.SendTransformMessage(jgo);
                            Debug.Log("Send:" + item.Key);
                        }
                    }
                }
                importexpot.jsonobject = MediaJsonObject;
            }
        }

        private void SetPresentationCamera(int num)
        {
            DesktopCamera.transform.SetParent(PresentationCameras[num].transform);
            DesktopCamera.transform.localPosition = Vector3.zero;
            DesktopCamera.transform.localRotation = Quaternion.identity;
            PresentationCameras[num].GetComponent<PresentationCameraControl>().isActiveCamera(true);
            for (int i = 0; i < PresentationCameras.Length; i++) if (i != num) PresentationCameras[i].GetComponent<PresentationCameraControl>().isActiveCamera(false);
        }

        private void ReceiveCameraJsonMessage(JsonMessage jm)
        {
            SetPresentationCamera(jm.CamNum);
        }

        private void ReceivePlayJsonMessage(JsonMessage jm)
        {
            GameObject obj;
            if (MediaObject.TryGetValue(jm.gameobjectflag.name, out obj)) obj.GetComponent<MediaControl>().SetPlay(jm.gameobjectflag.flag);
        }

        private void ReceiveTransformJsonMessage(JsonMessage jm)
        {
            GameObject obj;
            if (MediaObject.TryGetValue(jm.gameobject.name, out obj))
            {
                obj.transform.localPosition = jm.gameobject.transform.position.Get();
                obj.transform.localRotation = jm.gameobject.transform.rotation.Get();
                obj.transform.localScale = jm.gameobject.transform.scale.Get();
                MediaControl mc = obj.GetComponent<MediaControl>();
                if (mc != null) mc.ResetTransform();
                else obj.GetComponent<PresentationCameraControl>().ResetTransform();
                Debug.Log("Set Receive Position");
            }
        }
    }
}
