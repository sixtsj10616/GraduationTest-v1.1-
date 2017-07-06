using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//** 來源 : http://akingunityscript.blogspot.tw/2015/11/unity.html
public class TakePic : MonoBehaviour {

    private bool StartShot = false;
    public Texture2D ShotTexture;

    public void StartScreenshot()
    {
        //在擷取畫面之前請等到所有的Camera都Render完成
        ShotTexture = null;
        StartShot = true;
    }
    public void OnPostRender()
    {
        if (StartShot == true)
        {
            StartShot = false;
            Camera myCamera = Camera.main;
            ShotTexture = new Texture2D((int)myCamera.pixelWidth, (int)myCamera.pixelHeight);
            //擷取全部畫面的資訊
            ShotTexture.ReadPixels(new Rect(0, 0, (int)myCamera.pixelWidth, (int)myCamera.pixelHeight), 0, 0, false);
            ShotTexture.Apply();
        }
    }
}
