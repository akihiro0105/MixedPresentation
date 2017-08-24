using UnityEngine;
using HoloLensModule;

namespace MixedPresentation
{
    public class DesktopCameraViewer : HoloLensModuleSingleton<DesktopCameraViewer>
    {
        public static int windowWeidth = 1920;
        public static int windowHeight = 1080;
        public Material alphaBlendPreviewMat;

        private bool setShaderTextures = false;
        private Camera _camera;
        [HideInInspector]
        public RenderTexture renderTexture;

        protected override void OnDestroy()
        {
            ResetCompositor();
            base.OnDestroy();
        }

        public void ResetCompositor()
        {
            setShaderTextures = false;
        }

        void LateUpdate()
        {
            if (_camera == null || renderTexture == null)
            {
                _camera = GetComponent<Camera>();
                if (_camera == null)
                {
                    renderTexture = null;
                    return;
                }
                _camera.enabled = true;
                renderTexture = new RenderTexture(windowWeidth, windowHeight, 24);
                renderTexture.antiAliasing = 8;
                renderTexture.anisoLevel = 0;
                renderTexture.filterMode = FilterMode.Trilinear;
                _camera.targetTexture = renderTexture;
            }

            if (!setShaderTextures)
            {
                if (renderTexture != null)
                {
                    setShaderTextures = true;
                    alphaBlendPreviewMat.SetTexture("_MainTex", renderTexture);
                }
            }
        }
    }
}
