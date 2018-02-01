using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Utility
{
    public class Boundingbox : MonoBehaviour
    {
        public Material LineMaterial;
        public float LineWidth = 0.001f;
        public bool isLineActive = true;
        [Space(14)]
        public bool isCollider = true;
        public bool isMeshFilter = true;

        private BoxCollider boxcollider;
        private LineRenderer linerenderer;
        private Vector3[] linepoint = new Vector3[16];

        // Use this for initialization
        void Start()
        {
            boxcollider = gameObject.AddComponent<BoxCollider>();

            linerenderer = gameObject.AddComponent<LineRenderer>();
            linerenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            linerenderer.receiveShadows = false;
            linerenderer.widthMultiplier = LineWidth;
            linerenderer.material = LineMaterial;
            linerenderer.useWorldSpace = false;
            linerenderer.positionCount = 0;
        }

        void Update()
        {
            Bounds? bounds = BoundsMeshFilter(gameObject);
            if (bounds != null)
            {
                boxcollider.center = bounds.Value.center;
                boxcollider.size = bounds.Value.size;
            }

            if (isLineActive==true)
            {
                linerenderer.widthMultiplier = LineWidth;
                linerenderer.material = LineMaterial;
                linepoint[0].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[1].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[2].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[3].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[4].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[5].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[6].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[7].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[8].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[9].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[10].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[11].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[12].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[13].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[14].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[15].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linerenderer.positionCount = linepoint.Length;
                linerenderer.SetPositions(linepoint);
            }
            else
            {
                linerenderer.positionCount = 0;
            }
        }

        private Bounds? BoundsMeshFilter(GameObject obj)
        {
            bool initflag = false;
            Bounds initbound = new Bounds(Vector3.zero, Vector3.zero);
            MeshFilter mesh = obj.GetComponent<MeshFilter>();
            if (mesh != null)
            {
                initbound.Encapsulate(new Bounds(mesh.mesh.bounds.center, mesh.mesh.bounds.size));
                initflag = true;
            }
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject childobj = obj.transform.GetChild(i).gameObject;
                Bounds? childbound = BoundsMeshFilter(childobj);
                if (childbound != null)
                {
                    Matrix4x4 mat = Matrix4x4.TRS(childobj.transform.localPosition, childobj.transform.localRotation, childobj.transform.localScale);
                    Vector3 center = mat.MultiplyPoint3x4(childbound.Value.center);
                    Vector3 size = mat.MultiplyVector(childbound.Value.size);
                    size.Set(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
                    initbound.Encapsulate(new Bounds(center, size));
                    initflag = true;
                }
            }
            return (initflag == true) ? (Bounds?)initbound : null;
        }


        public void isActive(bool flag,bool isBoxCal = false)
        {
            if (isBoxCal)
            {
                Bounds bounds;
                //MeshFilterBounds(gameObject, out bounds);
                //boxcollider.center = bounds.center;
                //boxcollider.size = bounds.size;
            }
        }
    }
}
