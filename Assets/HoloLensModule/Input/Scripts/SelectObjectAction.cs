using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloLensModule.Input
{
    [Serializable]
    public class SelectActionEvent : UnityEvent<GameObject>{ }
    [Serializable]
    public class DragActionEvent : UnityEvent<GameObject, Vector3> { }

    public class SelectObjectAction : MonoBehaviour, IFocusInterface,ITapInterface, IDragInterface
    {
        public SelectActionEvent SelectEvent = new SelectActionEvent();
        public DragActionEvent DragStartEvent = new DragActionEvent();
        public DragActionEvent DragUpdateEvent = new DragActionEvent();
        public SelectActionEvent FocusEnterEvent = new SelectActionEvent();
        public SelectActionEvent FocusEndEvent = new SelectActionEvent();

        public void ObjectTap()
        {
            SelectEvent.Invoke(gameObject);
        }

        public void ObjectStartDrag(Vector3 pos)
        {
            DragStartEvent.Invoke(gameObject,pos);
        }

        public void ObjectUpdateDrag(Vector3 pos)
        {
            DragUpdateEvent.Invoke(gameObject,pos);
        }

        public void FocusEnter(RaycastHit hit)
        {
            FocusEnterEvent.Invoke(gameObject);
        }

        public void FocusEnd()
        {
            FocusEndEvent.Invoke(gameObject);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
