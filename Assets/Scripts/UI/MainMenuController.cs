using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct RoofTypeInfo
{
    public string name;
    public int Index;
    public Color bgColor;
    public MainController.RoofType type;
    public RoofTypeInfo(string textName , int index , Color color , MainController.RoofType roofType)
    {
        name = textName;
        Index = index;
        bgColor = color;
        type = roofType;
    }
}

public class MainMenuController : Singleton<MainMenuController>
{

    public GameObject typeMenu;
	public GameObject floorMenu;
	public GameObject CTMenu;
	public GameObject ViewMenu;
    public int TypeMenuCount = 0;
    public Button StateBtn;
	public GameObject FloorBtn;
    List<RoofTypeInfo> typeInfoList;
    public int nowRoofTypeIndex = 0;
    // Use this for initialization
    void Start()
    {
        initTypeMenuData();
        StateBtn.GetComponentInChildren<Text>().text = typeInfoList[0].name;
    }

    // Update is called once per frame
    void Update()
    {

    }
    /**
     * 點擊目前屋頂型態按鈕
     */
    public void ClickRoofTypeBtn()
    {
		typeMenu.SetActive(!typeMenu.activeInHierarchy);
		floorMenu.SetActive(false);
    }
	/**
	* 點擊目前樓層按鈕
	*/
	public void ClickFloorTypeBtn()
	{
		floorMenu.SetActive(!floorMenu.activeInHierarchy);
		typeMenu.SetActive(false);
	}
    /**
     * 設定屋頂型態
     * 點擊選單中屋頂型態按鈕時呼叫
     */
    public void SelectRoofType(int index)
    {
        if (nowRoofTypeIndex != index)
        { 
            MainController.Instance.OnRoofTypeBtnClick(typeInfoList[index].type);
            StateBtn.GetComponentInChildren<Text>().text = typeInfoList[index].name;
            nowRoofTypeIndex = index;
        }
        typeMenu.SetActive(true);
    }
	/**
   * 選擇樓層
   */
	public void SelectFloor(int index)
	{
		if (MainController.Instance.selectFloor != index)
		{
			MainController.Instance.selectFloor=index;
		}
		floorMenu.SetActive(true);
	}
	/**
	* 創建樓層
	*/
	public void AddFloor()
	{
		GameObject newBtn = Instantiate(FloorBtn) as GameObject;
		newBtn.transform.parent=FloorBtn.transform.parent;
		newBtn.transform.SetSiblingIndex(newBtn.transform.parent.GetChildCount() - 3);
		newBtn.GetComponentInChildren<Text>().text = (newBtn.transform.parent.GetChildCount()-3).ToString();

		MainController.Instance.CreateNewFloor();
	}
	/**
	* 刪除樓層
	*/
	public void DeleteFloor()
	{
		if (FloorBtn.transform.parent.GetChildCount()>3)
		{
			Destroy(FloorBtn.transform.parent.GetChild(FloorBtn.transform.parent.GetChildCount() - 3).gameObject);

			MainController.Instance.DeleteFloor();
		}
	}
	/**
	* 設定組合亭型態
	*/
	public void ClickCombinedTingTypeBtn()
	{
		CTMenu.SetActive(!CTMenu.activeInHierarchy);
		ViewMenu.SetActive(!ViewMenu.activeInHierarchy);

		typeMenu.SetActive(false);
		floorMenu.SetActive(false);
	}
    /**
     * 初始化數據，名稱，索引，按鈕被景色，屋頂型態
     * 目前只會用到名稱與屋頂型態
     */
    void initTypeMenuData()
    {
        typeInfoList = new List<RoofTypeInfo>();
        typeInfoList.Add(new RoofTypeInfo("攢尖", 0, Color.blue,MainController.RoofType.Zan_Jian_Ding));
        typeInfoList.Add(new RoofTypeInfo("廡殿", 1, Color.yellow, MainController.RoofType.Wu_Dian_Ding));
        typeInfoList.Add(new RoofTypeInfo("盝頂", 2, Color.red, MainController.RoofType.Lu_Ding));
        typeInfoList.Add(new RoofTypeInfo("歇山", 3, Color.white, MainController.RoofType.Shya_Shan_Ding));
    }
    /**
     * 屋頂選單初始化，沒用到
     */
    public void initTypeMenu()
    {
        typeMenu.transform.FindChild("ScrollBlock").FindChild("Content");
        for (int iIndex = 0; iIndex < TypeMenuCount; iIndex++)
        {
        }
    }
}
