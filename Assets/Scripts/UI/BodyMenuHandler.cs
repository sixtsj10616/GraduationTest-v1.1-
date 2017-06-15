using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyMenuHandler : MonoBehaviour {

    public const string ColumeHeightSlider_NAME = "HeightSlider";
    public const string BodyWidthSlider_NAME = "WidthSlider";
    public const string BodyLengthSlider_NAME = "LengthSlider";
    public const string GoldColNumSlider_NAME = "ColNumSlider";
    public const string WindowNumSlider_NAME = "WindowNumSlider";
    public const string DoorNumSlider_NAME = "DoorNumSlider";
    public const string GoldColToggle_NAME = "GoldColToggle";
    public const string FriezeToggle_NAME = "FriezeToggle";
    public const string BalustradeToggle_NAME = "BalustradeToggle";
    
    public Toggle GoldColToggle;
    public GameObject GoldColInfo;


    // Use this for initialization
    void Start () {

        GoldColToggle.onValueChanged.AddListener((bool value) => OnToggleClick(GoldColToggle, value));


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnToggleClick(Toggle toggle, bool isOn)
    {
        switch (toggle.name)
        {
            case GoldColToggle_NAME:
                GoldColInfo.SetActive(isOn);
                break;

            default:
                print("!!! Can't Found Toggle Name");
                break;
        }
    }



}
