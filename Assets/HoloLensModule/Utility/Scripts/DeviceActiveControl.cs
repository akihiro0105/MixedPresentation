using UnityEngine;

namespace HoloLensModule.Utility
{
    // HoloLens or DeskTopのみでアクティブにする
    public class DeviceActiveControl : MonoBehaviour
    {
        [SerializeField]
        public enum ActiveDeviceModel
        {
            MRDevice,
            Other
        }
        
        public ActiveDeviceModel ActiveDevice = ActiveDeviceModel.MRDevice;

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget== UnityEditor.BuildTarget.WSAPlayer)
            {
                gameObject.SetActive((ActiveDevice == ActiveDeviceModel.MRDevice) ? true : false);
            }
            else
            {
                gameObject.SetActive((ActiveDevice == ActiveDeviceModel.Other) ? true : false);
            }
#else
            if (Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64)
            {
                gameObject.SetActive((ActiveDevice == ActiveDeviceModel.MRDevice) ? true : false);
            }
            else
            {
                gameObject.SetActive((ActiveDevice == ActiveDeviceModel.Other) ? true : false);
            }
#endif
        }
    }
}
