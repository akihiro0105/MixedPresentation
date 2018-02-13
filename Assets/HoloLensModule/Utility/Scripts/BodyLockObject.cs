using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Utility
{
    public class BodyLockObject : MonoBehaviour
    {
        public float MoveSpeed = 2.0f;
        public float MoveDistance = 1.0f;

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(Camera.main.transform.position);
            Vector3 target = Camera.main.transform.position + Camera.main.transform.forward * MoveDistance;
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * MoveSpeed);
        }
    }
}
