using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnPlamformSliderChange(Slider slider, float value);



public class PlamformHandler : MonoBehaviour {

    public const string WidthSlider_NAME = "WidthSlider";
    public const string DepthSlider_NAME = "DepthSlider";
    public const string HeightSlider_NAME = "HeightSlider";
    public const string StairNumSlider_NAME = "StairNumSlider";
    public const string StairLengthSlider_NAME = "StairLenSlider";
    public const string StairWidthSlider_NAME = "StairWidSlider";
    public const string StairToggle_NAME = "StairToggle";
    public const string BorderToggle_NAME = "BorderToggle";
   

    public Toggle BorderToggle;
    public Toggle StairToggle;
    public GameObject StairInfo;

    // Use this for initialization
    void Start () {
        BorderToggle.onValueChanged.AddListener((bool value) => OnToggleClick(BorderToggle, value));
        StairToggle.onValueChanged.AddListener((bool value) => OnToggleClick(StairToggle, value));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnToggleClick(Toggle toggle ,  bool isOn)
    {
        switch (toggle.name)
        {
            case StairToggle_NAME:
                StairInfo.SetActive(isOn);
                break;
            case BorderToggle_NAME:
                break;
            default:
                print("!!! Can't Found Toggle Name");
                break;
        }
    }
    
    
}
