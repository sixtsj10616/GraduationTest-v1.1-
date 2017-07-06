using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour {

    public GameObject Menu;
    public bool isOpen = false;

    /*
     * 點擊 TabButton 時，更改狀態與通知 TabViewController做選單變更
     */
    public void OnButtonClick()
    {
        if (!isOpen)
        {
            setButton(true);
            this.GetComponentInParent<TabViewController>().UpdateTabBtnState(this.transform.name);
        } 
    }
    /**
     * 設定按鈕狀態與顏色
     */
    public void setButton(bool bOpen)
    {
        isOpen = bOpen;
        Menu.SetActive(isOpen);

        if (bOpen == true)
        {
            this.GetComponent<Image>().color = Color.white;
        }
        else
        {
            this.GetComponent<Image>().color = Color.gray;
        }
    }


}
