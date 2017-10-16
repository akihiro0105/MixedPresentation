using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MixedPresentation
{
    public enum JsonMessageType
    {
        Camera,
        Transform,
        Play
    }

    [Serializable]
    public class JsonVector3
    {
        public float x;
        public float y;
        public float z;
        public void Set(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public void Set(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public Vector3 Get() { return new Vector3(x, y, z); }
        public JsonVector3(float x, float y, float z) { Set(x, y, z); }
        public JsonVector3() { }
    }

    [Serializable]
    public class JsonQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public void Set(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public void Set(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }
        public Quaternion Get() { return new Quaternion(x, y, z, w); }
        public JsonQuaternion(float x, float y, float z, float w) { Set(x, y, z, w); }
        public JsonQuaternion() { }
    }

    [Serializable]
    public class JsonTransform
    {
        public JsonVector3 position;
        public JsonQuaternion rotation;
        public JsonVector3 scale;
        public void Set(Vector3 pos, Quaternion rot, Vector3 sca)
        {
            position.Set(pos);
            rotation.Set(rot);
            scale.Set(sca);
        }
        public JsonTransform()
        {
            position = new JsonVector3();
            rotation = new JsonQuaternion();
            scale = new JsonVector3();
        }
    }

    [Serializable]
    public class JsonGameObject
    {
        public string name;
        public JsonTransform transform;
        public JsonGameObject() { transform = new JsonTransform(); }
    }

    [Serializable]
    public class JsonGameObjectFlag
    {
        public string name;
        public bool flag;
        public JsonGameObjectFlag() { }
    }

    [Serializable]
    public class JsonMessage
    {
        public int type;
        public JsonMessage()
        {
            gameobject = new JsonGameObject();
            gameobjectflag = new JsonGameObjectFlag();
        }
        // Camera
        public int CamNum;
        // Transform
        public JsonGameObject gameobject;
        // Play
        public JsonGameObjectFlag gameobjectflag;
    }

    [Serializable]
    public class JsonSaveGameobject
    {
        public List<JsonGameObject> gameobject;
        public JsonSaveGameobject() { gameobject = new List<JsonGameObject>(); }
    }
}
