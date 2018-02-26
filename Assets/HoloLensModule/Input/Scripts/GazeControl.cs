using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class GazeControl : MonoBehaviour,IGazeInterface
    {
        public bool isView = true;
        public GameObject GazeModel;

        private Vector3 InitScale;
        private Vector3 NextScale;
        private float scale = 0.5f;
        private float speed = 0.4f;

        public void SetGazeState(GazeManager.GazeState state)
        {
            switch (state)
            {
                case GazeManager.GazeState.Normal:
                    NextScale = InitScale * scale;
                    break;
                case GazeManager.GazeState.Hand:
                    NextScale = InitScale;
                    break;
                case GazeManager.GazeState.Press:
                    NextScale = new Vector3(InitScale.x, InitScale.y, InitScale.z *scale);
                    break;
                default:
                    break;
            }
        }

        // Use this for initialization
        void Start()
        {
            InitScale = GazeModel.transform.localScale;
            NextScale = InitScale * scale;
        }

        // Update is called once per frame
        void Update()
        {
            GazeModel.SetActive(isView);
            if (isView==true)
            {
                GazeModel.transform.localScale = Vector3.Lerp(GazeModel.transform.localScale, NextScale, speed);
            }
        }
    }
}
