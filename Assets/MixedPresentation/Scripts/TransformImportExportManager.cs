using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_UWP
using System;
using Windows.Storage;
using System.Threading.Tasks;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Threading;
#endif

// json形式で保存に変更

namespace MixedPresentation
{
    // オブジェクトのTransform情報を保存，再生する
    public class TransformImportExportManager : MonoBehaviour
    {
        [SerializeField]
        private KeyCode exportcode = KeyCode.E;
        [SerializeField]
        private KeyCode importcode = KeyCode.I;
        public string filename = "transform.json";

        [HideInInspector]
        public string SaveDirectoryName = "";
        [HideInInspector]
        public JsonSaveGameobject jsonobject = new JsonSaveGameobject();

        public delegate void TransformImportEventHandler(JsonSaveGameobject jsonsave);
        public TransformImportEventHandler TransformImportEvent;

#if UNITY_EDITOR
        [CustomEditor(typeof(TransformImportExportManager))]
        public class CustomInspector:Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                TransformImportExportManager obj = (TransformImportExportManager)target;
                if (GUILayout.Button("Export Transform")) obj.ExportTransform();
                if (GUILayout.Button("Import Transform")) obj.ImportTransform();
            }
        }
#endif

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyUp(exportcode)) ExportTransform();
            if (UnityEngine.Input.GetKeyUp(importcode)) ImportTransform();
        }

        public void ExportTransform() { StartCoroutine(Export()); }

        private IEnumerator Export()
        {
            bool isWriteFlag = false;
#if UNITY_UWP
            Task.Run(async () =>
            {
                var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(SaveDirectoryName);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(file, JsonUtility.ToJson(jsonobject));
                isWriteFlag = true;
            });
#elif UNITY_EDITOR || UNITY_STANDALONE
            string writepath = Application.dataPath + "\\..\\" + SaveDirectoryName;
            Thread thread = new Thread(()=> {
                FileInfo info = new FileInfo(writepath + "\\" + filename);
                StreamWriter sw = info.CreateText();
                sw.Write(JsonUtility.ToJson(jsonobject));
                sw.Flush();
                sw.Close();
                isWriteFlag = true;
            });
            thread.Start();
#endif
            yield return new WaitUntil(() => isWriteFlag);
            Debug.Log("Export Transform");
        }

        public void ImportTransform() { StartCoroutine(Import()); }

        private IEnumerator Import()
        {
            string path = "";
#if UNITY_UWP
            path = ApplicationData.Current.LocalFolder.Path + "\\" + SaveDirectoryName;
#elif UNITY_EDITOR || UNITY_STANDALONE
            path = Application.dataPath + "\\..\\" + SaveDirectoryName;
#endif
            using (WWW www = new WWW("file://"+path+"\\"+ filename))
            {
                yield return www;
                JsonSaveGameobject JsonMediaObject = new JsonSaveGameobject();
                JsonMediaObject = JsonUtility.FromJson<JsonSaveGameobject>(www.text);
                if (TransformImportEvent != null) TransformImportEvent(JsonMediaObject);
            }
            Debug.Log("Import Transform");
        }
    }
}
