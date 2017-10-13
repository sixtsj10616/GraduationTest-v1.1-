using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyMenuHandler : MonoBehaviour {

	//名字必須唯一 不能與platform和Roof的icon名字重複
    public const string ColumeHeightSlider_NAME = "ColHeightSlider";
    public const string BodyWidthSlider_NAME = "BodyWidthSlider";
    public const string BodyLengthSlider_NAME = "BodyLengthSlider";
    public const string GoldColNumSlider_NAME = "ColNumSlider";
	public const string UnitInBay_NAME = "UnitInBaySlider";
    public const string DoorNumSlider_NAME = "DoorNumSlider";
    public const string GoldColToggle_NAME = "GoldColToggle";
    public const string FriezeToggle_NAME = "FriezeToggle";
    public const string BalustradeToggle_NAME = "BalustradeToggle";
	//風格化參數
	public const string EaveColRadInflate_NAME = "EaveColRadInflateSlider";
	public const string EaveColOffset_NAME = "EaveColOffsetSlider";

    public Toggle GoldColToggle;
    public GameObject GoldColInfo;

    // Use this for initialization
    void Start () 
	{
        GoldColToggle.onValueChanged.AddListener((bool value) => OnToggleClick(GoldColToggle, value));
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
	//某一層樓的資訊
	public void UpdateMenuInfo(Dictionary<string, List<DataInfo>> Data)
	{
		List<DataInfo> data = Data[Define.BodyDataList];
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


}
