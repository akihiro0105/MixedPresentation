using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
#else
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
#endif
#endif

namespace HoloLensModule.Environment
{
    // オブジェクトのWorldAnchorコントロール
    public class WorldAnchorControl : MonoBehaviour
    {
        public bool RemoveWorldAnchorOnDestory = false;
#if UNITY_EDITOR || UNITY_UWP
        private WorldAnchorStore anchorstore = null;
#endif

        void Start()
        {
#if UNITY_EDITOR || UNITY_UWP
            WorldAnchorStore.GetAsync((store) =>
            {
                anchorstore = store;
                anchorstore.Load(gameObject.name, gameObject);
                RemoveWorldAnchor();
                SetWorldAnchor();
            });
#endif
        }

        void OnDestroy()
        {
            if (RemoveWorldAnchorOnDestory == true)
            {
                RemoveWorldAnchor();
            }
        }

        public bool isLocatedWorldAnchor()
        {
#if UNITY_EDITOR || UNITY_UWP
            if (anchorstore != null)
            {
                WorldAnchor worldanchor = gameObject.GetComponent<WorldAnchor>();
                if (worldanchor != null && worldanchor.isLocated == true)
                {
                    return true;
                }
            }
#endif
            return false;
        }

        public void RemoveWorldAnchor()
        {
#if UNITY_EDITOR || UNITY_UWP
            if (anchorstore != null)
            {
                WorldAnchor worldanchor = gameObject.GetComponent<WorldAnchor>();
                if (worldanchor != null)
                {
                    anchorstore.Delete(worldanchor.name);
                    DestroyImmediate(worldanchor);
                }
            }
#endif
        }

        public void SetWorldAnchor()
        {
#if UNITY_EDITOR || UNITY_UWP
            if (anchorstore != null)
            {
                WorldAnchor worldanchor = gameObject.GetComponent<WorldAnchor>();
                if (worldanchor == null)
                {
                    worldanchor = gameObject.AddComponent<WorldAnchor>();
                }
                worldanchor.name = gameObject.name;
                if (worldanchor.isLocated == true)
                {
                    anchorstore.Save(worldanchor.name, worldanchor);
                }
                else
                {
                    worldanchor.OnTrackingChanged += OnTrackingChanged;
                }
            }
#endif
        }

#if UNITY_EDITOR || UNITY_UWP
        private void OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located == true)
            {
                anchorstore.Save(self.name, self);
                self.OnTrackingChanged -= OnTrackingChanged;
            }
        }
#endif

        #region All WorldAnchor Function
        public void ClearAllWorldAnchor()
        {
#if UNITY_EDITOR || UNITY_UWP
            if (anchorstore != null)
            {
                anchorstore.Clear();
            }
#endif
        }
        #endregion
    }
}
