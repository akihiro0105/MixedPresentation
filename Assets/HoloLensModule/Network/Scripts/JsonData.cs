using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule
{
    [Serializable]
    public class JsonVector3
    {
        public float x;
        public float y;
        public float z;

        public JsonVector3() { }

        public JsonVector3(Vector3 v)
        {
            SetVector3(v);
        }

        public void SetVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 GetVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class JsonQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public JsonQuaternion() { }

        public JsonQuaternion(Quaternion q)
        {
            SetQuaternion(q);
        }

        public void SetQuaternion(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public Quaternion GetQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }

    [Serializable]
    public class JsonTransform
    {
        public JsonVector3 position;
        public JsonQuaternion rotation;
        public JsonVector3 scale;

        public JsonTransform()
        {
            position = new JsonVector3();
            rotation = new JsonQuaternion();
            scale = new JsonVector3();
        }

        public JsonTransform(Vector3 p, Quaternion r, Vector3 s)
        {
            position = new JsonVector3(p);
            rotation = new JsonQuaternion(r);
            scale = new JsonVector3(s);
        }
    }
}
