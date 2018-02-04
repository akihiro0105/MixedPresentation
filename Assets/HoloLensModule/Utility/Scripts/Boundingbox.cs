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
        public bool isLineCalculate = true;
        public bool onlyCurrentObject = false;
        public bool isOutsideBound = false;
        [Space(14)]
        public bool ActiveMeshFilter = true;
        public bool ActiveCollider = false;

        private BoxCollider boxcollider;
        private LineRenderer linerenderer;
        private Vector3[] linepoint = new Vector3[16];

        private Bounds ParentBound = new Bounds();
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
            if (isLineCalculate == true)
            {
                ParentBound.center = Vector3.zero;
                ParentBound.size = Vector3.zero;
                if (ActiveMeshFilter==true)
                {
                    if (isOutsideBound == true)
                    {
                        BoundsOutsideMeshFilter(gameObject);
                    }
                    else
                    {
                        BoundsInsideMeshFilter(gameObject);
                    }
                }
                if (ActiveCollider==true)
                {
                    BoundsCollider(gameObject);
                }
                boxcollider.center = ParentBound.center;
                boxcollider.size = ParentBound.size;
            }

            if (isLineActive == true)
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

        private void BoundsCollider(GameObject obj, Matrix4x4? mat = null)
        {
            if (mat == null)
            {
                BoxCollider[] box = obj.GetComponents<BoxCollider>();
                for (int i = 0; i < box.Length; i++)
                {
                    if (box[i].GetInstanceID()!=boxcollider.GetInstanceID())
                    {
                        ParentBound.Encapsulate(new Bounds(box[i].center, box[i].size));
                    }
                }
                CapsuleCollider[] cap = obj.GetComponents<CapsuleCollider>();
                for (int i = 0; i < cap.Length; i++)
                {
                    Vector3 size = new Vector3(cap[i].radius * 2, cap[i].height, cap[i].radius*2);
                    ParentBound.Encapsulate(new Bounds(cap[i].center, size));
                }
                SphereCollider[] sph = obj.GetComponents<SphereCollider>();
                for (int i = 0; i < sph.Length; i++)
                {
                    Vector3 size = new Vector3(sph[i].radius * 2, sph[i].radius * 2, sph[i].radius * 2);
                    ParentBound.Encapsulate(new Bounds(sph[i].center, size));
                }
            }
            if (onlyCurrentObject == false)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    GameObject childobj = obj.transform.GetChild(i).gameObject;
                    Matrix4x4 bufmat = Matrix4x4.TRS(childobj.transform.localPosition, childobj.transform.localRotation, childobj.transform.localScale);
                    if (mat != null)
                    {
                        bufmat = mat.Value * bufmat;
                    }
                    BoxCollider[] box = childobj.GetComponents<BoxCollider>();
                    for (int j = 0; j < box.Length; j++)
                    {
                        Vector3 center = bufmat.MultiplyPoint3x4(box[j].center);
                        Vector3 size = GetSize(bufmat, childobj, box[j].size);
                        ParentBound.Encapsulate(new Bounds(center, size));
                    }
                    CapsuleCollider[] cap = childobj.GetComponents<CapsuleCollider>();
                    for (int j = 0; j < cap.Length; j++)
                    {
                        Vector3 center = bufmat.MultiplyPoint3x4(cap[j].center);
                        Vector3 size = GetSize(bufmat, childobj, new Vector3(cap[j].radius * 2, cap[j].height, cap[j].radius * 2));
                        ParentBound.Encapsulate(new Bounds(center, size));
                    }
                    SphereCollider[] sph = childobj.GetComponents<SphereCollider>();
                    for (int j = 0; j < sph.Length; j++)
                    {
                        Vector3 center = bufmat.MultiplyPoint3x4(sph[j].center);
                        Vector3 size = GetSize(bufmat, childobj, new Vector3(sph[j].radius * 2, sph[j].radius * 2, sph[j].radius * 2));
                        ParentBound.Encapsulate(new Bounds(center, size));
                    }
                    BoundsInsideMeshFilter(childobj, bufmat);
                }
            }
        }

        private void BoundsInsideMeshFilter(GameObject obj, Matrix4x4? mat=null)
        {
            if (mat==null)
            {
                MeshFilter mesh = obj.GetComponent<MeshFilter>();
                if (mesh != null)
                {
                    ParentBound.Encapsulate(new Bounds(mesh.mesh.bounds.center, mesh.mesh.bounds.size));
                }
            }
            if (onlyCurrentObject == false)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    GameObject childobj = obj.transform.GetChild(i).gameObject;
                    MeshFilter mesh = childobj.GetComponent<MeshFilter>();
                    Matrix4x4 bufmat = Matrix4x4.TRS(childobj.transform.localPosition, childobj.transform.localRotation, childobj.transform.localScale);
                    if (mat != null)
                    {
                        bufmat = mat.Value * bufmat;
                    }
                    if (mesh != null)
                    {
                        Vector3 center = bufmat.MultiplyPoint3x4(mesh.mesh.bounds.center);
                        Vector3 size = GetSize(bufmat,childobj, mesh.mesh.bounds.size);
                        ParentBound.Encapsulate(new Bounds(center, size));
                    }
                    BoundsInsideMeshFilter(childobj, bufmat);
                }
            }
        }

        private Bounds? BoundsOutsideMeshFilter(GameObject obj)
        {
            bool initflag = false;
            Bounds initbound = new Bounds(Vector3.zero, Vector3.zero);
            MeshFilter mesh = obj.GetComponent<MeshFilter>();
            if (mesh != null)
            {
                initbound.Encapsulate(new Bounds(mesh.mesh.bounds.center, mesh.mesh.bounds.size));
                initflag = true;
            }
            if (onlyCurrentObject == false)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    GameObject childobj = obj.transform.GetChild(i).gameObject;
                    Bounds? childbound = BoundsOutsideMeshFilter(childobj);
                    if (childbound != null)
                    {
                        Matrix4x4 mat = Matrix4x4.TRS(childobj.transform.localPosition, childobj.transform.localRotation, childobj.transform.localScale);
                        Vector3 center = mat.MultiplyPoint3x4(childbound.Value.center);
                        Vector3 size = GetSize(mat, childobj, childbound.Value.size);
                        initbound.Encapsulate(new Bounds(center, size));
                        initflag = true;
                    }
                }
            }
            ParentBound.center = initbound.center;
            ParentBound.size = initbound.size;
            return (initflag == true) ? (Bounds?)initbound : null;
        }

        private Vector3 GetSize(Matrix4x4 mat,GameObject obj, Vector3 setscale)
        {
            Vector3 scale = Vector3.Scale(setscale, obj.transform.localScale);
            Vector3 size1 = mat.MultiplyVector(scale);
            size1.Set(Mathf.Abs(size1.x), Mathf.Abs(size1.y), Mathf.Abs(size1.z));
            Vector3 size2 = mat.inverse.MultiplyVector(scale);
            size2.Set(Mathf.Abs(size2.x), Mathf.Abs(size2.y), Mathf.Abs(size2.z));
            Vector3 size = new Vector3(Mathf.Max(size1.x, size2.x), Mathf.Max(size1.y, size2.y), Mathf.Max(size1.z, size2.z));
            return size;
        }
    }
}
