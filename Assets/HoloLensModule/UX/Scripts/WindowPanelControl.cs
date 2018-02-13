using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using HoloLensModule.Utility;

namespace HoloLensModule.UX
{
    public class WindowPanelControl : MonoBehaviour
    {
        [SerializeField]
        private bool isActiveAdjust = true;
        [SerializeField]
        private bool isActiveResize = true;
        [SerializeField]
        private bool isActiveRemove = true;
        [SerializeField]
        private bool isActiveBack = true;
        public string WindowPanelName
        {
            set{ WindowPanelNameText.text = value; }
            get { return WindowPanelNameText.text; }
        }
        public bool isHidePanel = false;
        [Space(14)]
        [SerializeField]
        private Text WindowPanelNameText;

        
        [SerializeField]
        private GameObject HidePanel;
        public float SetDistance = 1.5f;


        // Use this for initialization
        void Start()
        {
            WindowPanelNameText.text = Application.productName;
        }

        void OnDestroy()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetWindowPanel()
        {
            transform.LookAt(Camera.main.transform.position);
            Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * SetDistance;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2.0f);
        }
    }
}
