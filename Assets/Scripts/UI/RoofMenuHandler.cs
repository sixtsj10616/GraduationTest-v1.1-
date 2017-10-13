using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoofMenuHandler : MonoBehaviour {

	//名字必須唯一 不能與Body和Roof的icon名字重複
    public const string JijaHeightSlider_NAME = "JijaHeightSlider";
    public const string SurfaceSlider_NAME = "SurfaceSlider";
    public const string EaveSlider_NAME = "EaveSlider";
    public const string RidgeSlider_NAME = "RidgeSlider";
    public const string WingAngleSlider_NAME = "WingAngleSlider";

    // Use this for initialization
    void Start () {

        
}
	
	// Update is called once per frame
	void Update () {
		
	}

	//某一層樓的資訊
	public void UpdateMenuInfo(Dictionary<string, List<DataInfo>> Data)
	{
		List<DataInfo> data = Data[Define.RoofDataList];
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
