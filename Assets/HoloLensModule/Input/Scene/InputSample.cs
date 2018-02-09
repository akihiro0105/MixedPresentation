using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSample : MonoBehaviour {

    private Vector3 startpos;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FocusEnd(GameObject obj)
    {
        obj.GetComponent<Renderer>().material.color = Color.blue;
    }

    public void FocusEnter(GameObject obj)
    {
        obj.GetComponent<Renderer>().material.color = Color.red;
    }

    public void ObjectTap(GameObject obj)
    {
        Destroy(obj);
    }

    public void ObjectStartDrag(GameObject obj, Vector3 pos)
    {
        startpos = pos;
    }

    public void ObjectUpdateDrag(GameObject obj, Vector3 pos)
    {
        obj.transform.position += pos - startpos;
        startpos = pos;
    }
}
