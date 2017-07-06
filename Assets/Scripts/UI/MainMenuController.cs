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
    public int TypeMenuCount = 0;
    public Button StateBtn;

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

    public void ClickRoofTypeBtn()
    {
        typeMenu.SetActive(!typeMenu.activeInHierarchy);
    }

    public void SelectRoofType(int index)
    {
        if (nowRoofTypeIndex != index)
        { 
            MainController.Instance.OnRoofTypeBtnClick(typeInfoList[index].type);
            StateBtn.GetComponentInChildren<Text>().text = typeInfoList[index].name;
            nowRoofTypeIndex = index;
        }
        typeMenu.SetActive(!typeMenu.activeInHierarchy);

    }

    void initTypeMenuData()
    {
        typeInfoList = new List<RoofTypeInfo>();
        typeInfoList.Add(new RoofTypeInfo("攢尖", 0, Color.blue,MainController.RoofType.Zan_Jian_Ding4));
        typeInfoList.Add(new RoofTypeInfo("廡殿", 1, Color.yellow, MainController.RoofType.Wu_Dian_Ding));
        typeInfoList.Add(new RoofTypeInfo("露頂", 2, Color.red, MainController.RoofType.Lu_Ding));
        typeInfoList.Add(new RoofTypeInfo("歇山", 3, Color.white, MainController.RoofType.Shya_Shan_Ding));
    }

    public void initTypeMenu()
    {
        typeMenu.transform.FindChild("ScrollBlock").FindChild("Content");
        for (int iIndex = 0; iIndex < TypeMenuCount; iIndex++)
        {
        }
    }
}
