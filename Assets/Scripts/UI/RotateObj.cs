using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObj : MonoBehaviour {

    public Vector3 RotateDir = new Vector3(0, 0, -1);
    public int Speed = 2;
    public bool isRotate = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (isRotate)
        {
            transform.Rotate(Speed * RotateDir);
        }
	}
    /**
     * 開始轉啊
     */
    public void startRotate()
    {
        isRotate = true;
    }
    public void stopRotate()
    {
        isRotate = false;
    }
}
