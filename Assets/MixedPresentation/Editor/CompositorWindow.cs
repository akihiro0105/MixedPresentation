using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MixedPresentation
{
    public class CompositorWindow : EditorWindow
    {
        private static CompositorWindow window = null;

        static float padding = 10;
        static float frameWidth = 100;
        static float frameHeight = 100;
        static float aspect;

        void OnDestroy()
        {
            if (DesktopCameraViewer.Instance != null)
            {
                DesktopCameraViewer.Instance.ResetCompositor();
            }
        }

        [MenuItem("MixedPresentation/Compositor", false, 0)]
        public static void ShowWindow()
        {
            aspect = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
            window = (CompositorWindow)GetWindow(typeof(CompositorWindow), false, "Compositor", true);
            Vector2 minDimensons = new Vector2(315, 400);
            window.minSize = minDimensons;
            window.Show();
        }
        void OnGUI()
        {
            EditorGUILayout.BeginVertical("Box");
            {

                EditorGUILayout.BeginHorizontal("Box");
                {
                    if (GUILayout.Button("Toggle Full Screen")) PreviewWindow.ShowWindow();
                }
                EditorGUILayout.EndHorizontal();

                Rect spacing = GUILayoutUtility.GetRect(1, 2);
                float left = 0;
                UpdateFrameDimensions(spacing.y, ref left);

                Rect framesRect = GUILayoutUtility.GetRect(frameWidth * 2 + padding, frameHeight * 2 + padding);
                framesRect.x += left;

                if (Event.current != null && Event.current.type == EventType.Repaint)
                {
                    if (DesktopCameraViewer.Instance != null)
                    {
                        if (DesktopCameraViewer.Instance.renderTexture != null && DesktopCameraViewer.Instance.alphaBlendPreviewMat != null)
                        {
                            Graphics.DrawTexture(new Rect(framesRect.x, framesRect.y, frameWidth, frameHeight), DesktopCameraViewer.Instance.renderTexture, DesktopCameraViewer.Instance.alphaBlendPreviewMat);
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        void UpdateFrameDimensions(float currentTop, ref float left)
        {
            frameWidth = position.width;
            frameHeight = position.height - currentTop;

            if (frameWidth <= frameHeight * aspect)
            {
                frameHeight = frameWidth / aspect;
                left = 0;
            }
            else
            {
                frameWidth = frameHeight * aspect;
                left = (position.width / 2.0f) - (frameWidth / 2.0f);
            }
            frameWidth -= padding;
        }

        void Update()
        {
            if (window != null) window.Repaint();
            else ShowWindow();
        }
    }
}
