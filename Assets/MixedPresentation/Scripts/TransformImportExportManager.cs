using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MixedPresentation
{
    // オブジェクトのTransform情報を保存，再生する
    public class TransformImportExportManager : MonoBehaviour
    {
        [SerializeField]
        private KeyCode exportcode = KeyCode.E;
        [SerializeField]
        private KeyCode importcode = KeyCode.I;
        public string filename = "transform.position";
        public GameObject ImportExportParentObject = null;

#if UNITY_EDITOR
        [CustomEditor(typeof(TransformImportExportManager))]
        public class CustomInspector:Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                TransformImportExportManager obj = (TransformImportExportManager)target;
                if (GUILayout.Button("Export Transform"))
                {
                    Debug.Log("Export Transform");
                    obj.ExportTransform();
                }
                if (GUILayout.Button("Import Transform"))
                {
                    Debug.Log("Import Transform");
                    obj.ImportTransform();
                }
            }
        }
#endif
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Input.GetKeyUp(exportcode)) ExportTransform();
            if (UnityEngine.Input.GetKeyUp(importcode)) ImportTransform();
        }

        public void ExportTransform()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            StartCoroutine(Export());
        }

        private IEnumerator Export()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            FileInfo info = new FileInfo(Application.streamingAssetsPath+"/"+filename);
            StreamWriter sw = info.CreateText();
            GameObject obj = gameObject;
            int childcount = gameObject.transform.childCount;
            if (ImportExportParentObject != null)
            {
                obj = ImportExportParentObject;
                childcount = ImportExportParentObject.transform.childCount;
            }
            sw.WriteLine(childcount);
            for (int i = 0; i < childcount; i++)
            {
                GameObject go = obj.transform.GetChild(i).gameObject;
                sw.WriteLine(go.name);
                sw.WriteLine(go.transform.localPosition.x);
                sw.WriteLine(go.transform.localPosition.y);
                sw.WriteLine(go.transform.localPosition.z);
                sw.WriteLine(go.transform.localRotation.x);
                sw.WriteLine(go.transform.localRotation.y);
                sw.WriteLine(go.transform.localRotation.z);
                sw.WriteLine(go.transform.localRotation.w);
                sw.WriteLine(go.transform.localScale.x);
                sw.WriteLine(go.transform.localScale.y);
                sw.WriteLine(go.transform.localScale.z);
                yield return null;
            }
            sw.Flush();
            sw.Close();
#endif
            yield return null;
        }

        public void ImportTransform()
        {
            StartCoroutine(Import());
        }

        private IEnumerator Import()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            FileInfo info = new FileInfo(Application.streamingAssetsPath + "/" + filename);
            StreamReader sr = info.OpenText();
            int srcount = int.Parse(sr.ReadLine());
            GameObject parent= gameObject;
            if (ImportExportParentObject != null) parent = ImportExportParentObject;
            for (int i = 0; i < srcount; i++)
            {
                string name = sr.ReadLine();
                Vector3 pos = new Vector3();
                Quaternion rot = new Quaternion();
                Vector3 sca = new Vector3();
                pos.x = float.Parse(sr.ReadLine());
                pos.y = float.Parse(sr.ReadLine());
                pos.z = float.Parse(sr.ReadLine());
                rot.x = float.Parse(sr.ReadLine());
                rot.y = float.Parse(sr.ReadLine());
                rot.z = float.Parse(sr.ReadLine());
                rot.w = float.Parse(sr.ReadLine());
                sca.x = float.Parse(sr.ReadLine());
                sca.y = float.Parse(sr.ReadLine());
                sca.z = float.Parse(sr.ReadLine());
                for (int j = 0; j < parent.transform.childCount; j++)
                {
                    GameObject go = parent.transform.GetChild(j).gameObject;
                    if (go.name == name)
                    {
                        go.transform.localPosition = pos;
                        go.transform.localRotation = rot;
                        go.transform.localScale = sca;
                        break;
                    }
                }
                yield return null;
            }
            sr.Close();
#endif
            yield return null;
        }
    }
}
