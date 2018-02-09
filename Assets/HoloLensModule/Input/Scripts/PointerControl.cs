using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class PointerControl : MonoBehaviour,IPointerInterface
    {
        public GameObject RightObject;
        public GameObject LeftObject;

        public void SetDeviceType(PointerManager.DeviceType type)
        {
            if (type==PointerManager.DeviceType.Right)
            {
                RightObject.SetActive(true);
            }
            else
            {
                LeftObject.SetActive(true);
            }
        }

        void Start()
        {
            RightObject.SetActive(false);
            LeftObject.SetActive(false);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
