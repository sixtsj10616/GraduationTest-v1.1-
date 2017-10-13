using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnPlamformSliderChange(Slider slider, float value);



public class PlamformMenuHandler : MonoBehaviour {

	//名字必須唯一 不能與Body和Roof的icon名字重複
    public const string WidthSlider_NAME = "PlatWidthSlider";
    public const string DepthSlider_NAME = "PlatDepthSlider";
    public const string HeightSlider_NAME = "PlatHeightSlider";
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
	//某一層樓的資訊
	public void UpdateMenuInfo(Dictionary<string, List<DataInfo>> Data)
	{
		List<DataInfo> data = Data[Define.PlatformDataList];
		for (int i = 0; i < data.Count; i++)
		{
			foreach (Transform uiTarget in transform.GetComponentsInChildren<Transform>())
			{
				if (uiTarget.name == (data[i].UIName))
				{
					if (uiTarget.GetComponent<Slider>())
					{
						uiTarget.GetComponent<Slider>().value = data[i].Value.y;
					}
					if (uiTarget.GetComponent<Toggle>())
					{
						uiTarget.GetComponent<Toggle>().isOn = (data[i].Value.y != 0);
					}
					break;
				}
			}
		}
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
