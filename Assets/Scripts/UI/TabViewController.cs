using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TabViewController : MonoBehaviour {

    public int ButtonCount = 0;
    public Button[] AryTabBtn;

	// Use this for initialization
	void Start () {
        if (AryTabBtn.Length > 0)
        {
            AryTabBtn[0].GetComponent<TabButton>().setButton(true);
            UpdateTabBtnState(AryTabBtn[0].name);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void UpdateTabBtnState(string strBtnName)
    {
        foreach (Button btn in AryTabBtn)
        {
            if (btn.name == strBtnName)
            {
                print(btn.name);
            }
            else
            {
                btn.GetComponent<TabButton>().setButton(false);
            }
        }
    }
}
