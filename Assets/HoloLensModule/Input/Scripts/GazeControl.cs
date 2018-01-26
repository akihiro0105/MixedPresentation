using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{

    public class GazeControl : MonoBehaviour
    {
        [SerializeField]
        private GameObject gaze_d;
        [SerializeField]
        private GameObject gaze_r;
        [SerializeField]
        private GameObject gaze_u;
        [SerializeField]
        private GameObject gaze_l;

        private Vector3[] pos = new Vector3[4];
        private Vector3[] scale = new Vector3[4];
        private Vector3[] nextpos = new Vector3[4];
        private Vector3[] nextscale = new Vector3[4];

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < 4; i++)
            {
                pos[i] = new Vector3();
                scale[i] = new Vector3();
            }
            pos[0] = gaze_d.transform.localPosition;
            pos[1] = gaze_r.transform.localPosition;
            pos[2] = gaze_u.transform.localPosition;
            pos[3] = gaze_l.transform.localPosition;
            scale[0] = gaze_d.transform.localScale;
            scale[1] = gaze_r.transform.localScale;
            scale[2] = gaze_u.transform.localScale;
            scale[3] = gaze_l.transform.localScale;
            nextpos[0] = gaze_d.transform.localPosition;
            nextpos[1] = gaze_r.transform.localPosition;
            nextpos[2] = gaze_u.transform.localPosition;
            nextpos[3] = gaze_l.transform.localPosition;
            nextscale[0] = gaze_d.transform.localScale;
            nextscale[1] = gaze_r.transform.localScale;
            nextscale[2] = gaze_u.transform.localScale;
            nextscale[3] = gaze_l.transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            gaze_d.transform.localPosition = Vector3.Lerp(gaze_d.transform.localPosition, nextpos[0], 0.1f);
            gaze_r.transform.localPosition = Vector3.Lerp(gaze_r.transform.localPosition, nextpos[1], 0.1f);
            gaze_u.transform.localPosition = Vector3.Lerp(gaze_u.transform.localPosition, nextpos[2], 0.1f);
            gaze_l.transform.localPosition = Vector3.Lerp(gaze_l.transform.localPosition, nextpos[3], 0.1f);
            gaze_d.transform.localScale = Vector3.Lerp(gaze_d.transform.localScale, nextscale[0], 0.1f);
            gaze_r.transform.localScale = Vector3.Lerp(gaze_r.transform.localScale, nextscale[1], 0.1f);
            gaze_u.transform.localScale = Vector3.Lerp(gaze_u.transform.localScale, nextscale[2], 0.1f);
            gaze_l.transform.localScale = Vector3.Lerp(gaze_l.transform.localScale, nextscale[3], 0.1f);
        }

        public void Normal()
        {
            for (int i = 0; i < 4; i++)
            {
                nextpos[i] = pos[i];
                nextscale[i] = scale[i];
            }
        }

        public void Hand()
        {
            for (int i = 0; i < 4; i++)
            {
                nextpos[i] = pos[i] * 2;
            }
        }

        public void Press()
        {
            for (int i = 0; i < 4; i++)
            {
                nextpos[i].y = -pos[i].y;
            }
        }
    }
}
