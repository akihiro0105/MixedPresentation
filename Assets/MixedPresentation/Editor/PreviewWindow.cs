using UnityEngine;
using UnityEditor;

namespace MixedPresentation
{
    public class PreviewWindow : EditorWindow
    {
        private static PreviewWindow window = null;
        static bool killWindow = false;

        public static void ShowWindow()
        {
            killWindow = false;
            if (window == null)
            {
                window = ScriptableObject.CreateInstance<PreviewWindow>();
                window.position = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
                window.ShowPopup();
            }
            else
            {
                killWindow = true;
                window.Close();
            }
        }

        void Update()
        {
            if (window != null) window.Repaint();
            else if (!killWindow) ShowWindow();
        }

        void OnGUI()
        {
            if (Event.current != null && Event.current.isKey)
            {
                if (Event.current.keyCode != KeyCode.None)
                {
                    killWindow = true;
                    this.Close();
                    window = null;
                }
            }

            if (Event.current != null && Event.current.type == EventType.Repaint)
            {
                if (DesktopCameraViewer.Instance != null && DesktopCameraViewer.Instance.alphaBlendPreviewMat != null && DesktopCameraViewer.Instance.renderTexture != null)
                {
                    Graphics.DrawTexture(new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height), DesktopCameraViewer.Instance.renderTexture, DesktopCameraViewer.Instance.alphaBlendPreviewMat);
                }
            }
        }
    }
}
