using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertView : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Show()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Image")
            {
                child.GetComponent<RotateObj>().startRotate();
            }
        }
    }
    public void Close()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Image")
            {
                child.GetComponent<RotateObj>().stopRotate();
            }
        }
        
    }
}
