using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour {

    public GameObject Menu;
    public bool isOpen = false;

    public void OnButtonClick()
    {
        if (!isOpen)
        {
            setButton(true);
            this.GetComponentInParent<TabViewController>().UpdateTabBtnState(this.transform.name);
        } 
    }
    
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
