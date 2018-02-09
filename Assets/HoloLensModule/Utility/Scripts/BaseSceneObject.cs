using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Utility
{
    // Scene遷移時にGameObjectを残しておく
    public class BaseSceneObject : MonoBehaviour
    {
        private static bool CreateFlag = false;

        void Awake()
        {
            if (CreateFlag==false)
            {
                DontDestroyOnLoad(gameObject);
                CreateFlag = true;
            }
            else
            {
                Destroy(gameObject);
            } 
        }

        // アプリ終了
        public void ExitApp()
        {
            Application.Quit();
#if UNITY_UWP
            Windows.ApplicationModel.Core.CoreApplication.Exit();
#endif
        }
    }
}
