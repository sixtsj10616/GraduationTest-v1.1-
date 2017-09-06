using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TypeButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    public void OnButtonClick()
    {
        MainMenuController.Instance.SelectRoofType(transform.GetSiblingIndex());
    }
	public void OnButtonClick2SelectFloor()
	{
		MainMenuController.Instance.SelectFloor(transform.GetSiblingIndex());
	}
	public void OnButtonClick2AddFloor()
	{
		MainMenuController.Instance.AddFloor();
	}
	public void OnButtonClick2DeleteFloor()
	{
		MainMenuController.Instance.DeleteFloor();
	}
}
