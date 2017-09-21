using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MixedPresentation
{
    public enum MessageType
    {
        SendIP,
        ObjectTransform
    }
    [Serializable]
    public class JsonPosition
    {
        public float x;
        public float y;
        public float z;

        public void Set(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    }
    [Serializable]
    public class JsonRotation
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public void Set(Quaternion v) { x = v.x; y = v.y; z = v.z; w = v.w; }
    }
    [Serializable]
    public class JsonScale
    {
        public float x;
        public float y;
        public float z;

        public void Set(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    }
    [Serializable]
    public class JsonTransform
    {
        public JsonTransform()
        {
            position = new JsonPosition();
            rotation = new JsonRotation();
            scale = new JsonScale();
        }
        public JsonPosition position;
        public JsonRotation rotation;
        public JsonScale scale;

        public void Set(Vector3 pos,Quaternion rot, Vector3 sca)
        {
            position.Set(pos);
            rotation.Set(rot);
            scale.Set(sca);
        }
    }
    [Serializable]
    public class JsonObject
    {
        public JsonObject() { transform = new JsonTransform(); }
        public int num;
        public bool play;
        public JsonTransform transform;

        public void Set(int num, Vector3 position, Quaternion rotation, Vector3 scale,bool play=false) { this.num = num; transform.Set(position, rotation, scale);this.play = play; }
    }
    [Serializable]
    public class JsonMessage
    {
        public JsonMessage() { jobject = new JsonObject(); }
        public int type;
        public string ip;
        public JsonObject jobject;
    }
}
