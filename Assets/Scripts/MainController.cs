using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public struct ModelStruct//模型旋轉、縮放
{
	public GameObject model;
	public Vector3 rotation;
	public Vector3 scale;
    
	public ModelStruct(GameObject model, Vector3 rotation, Vector3 scale)
	{
		this.model = model;
		this.rotation = rotation;
		this.scale = scale;
	}
}
public class MainController : Singleton<MainController>
{

    //public GameObject building;
	public List<BuildingObj> Buildings = new List<BuildingObj>();       //* 建築樓層列表
    public int selectFloor = 0;                                       //* 目前 選擇/建造 的樓層
    //FormFactor***********************************************************************
    public enum FormFactorSideType { ThreeSide = 3, FourSide = 4, FiveSide = 5, SixSide = 6, EightSide = 8 };
    public FormFactorSideType sides = FormFactorSideType.ThreeSide;

	public enum RoofType { Zan_Jian_Ding = 0, Wu_Dian_Ding = 1, Lu_Ding = 2, Juan_Peng = 3, Shya_Shan_Ding = 4 };//Zan_Jian_Ding攢尖頂, Wu_Dian_Ding廡殿頂,Lu_Ding盝頂,Juan_Peng卷棚
	public RoofType roofType = RoofType.Zan_Jian_Ding;
    //**********************************************************************************
	public float initPlatformWidth=40;
	public float initPlatformLength = 50;
	public float initPlatformHeight = 1;
	public float initEaveColumnHeight=11;
	public float initGoldColumnHeight=11;

	public float initAllJijaHeight = 15f;
	public float initMainRidgeHeightOffset = -6;
	public float initAllJijaHeight_DownStair;
	public float initMainRidgeHeightOffset_DownStair;

	public float floorScaleRatio=0.9f;
    // Use this for initialization

    private void Awake()
    {
        InitFunction();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            print("A key was pressed");
            CreateNewFloor();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            print("D key was pressed");
            if (selectFloor != 0 && selectFloor < Buildings.Count)
            { 
                DeleteFloor(selectFloor);
            }
        }
    }
    /**
     * 初始化
     */
    public void InitFunction()
    {
		initAllJijaHeight_DownStair = initAllJijaHeight/3.0f;
		//?????這著值不能亂取
		initMainRidgeHeightOffset_DownStair = initMainRidgeHeightOffset * (1.0f-floorScaleRatio);

       GameObject building = new GameObject("building");
	   building.gameObject.AddComponent<BuildingObj>();
	   building.GetComponent<BuildingObj>().initFunction(building, Vector3.zero, initPlatformLength, initPlatformWidth, initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false,(int)roofType);
	   Buildings.Add(building.GetComponent<BuildingObj>());
    }
    /**
     * 新增樓層
     */
    public void CreateNewFloor()
    {
		if (selectFloor == Buildings.Count-1)//選的是最頂，蓋出新的一層，select那層重算成盝頂
		{
			selectFloor++;
			GameObject newBuilding = new GameObject("building" + Buildings.Count);

			newBuilding.AddComponent<BuildingObj>();
			Buildings.Insert(selectFloor, newBuilding.GetComponent<BuildingObj>());
			//UpdatePosition
			for (int iIndex = selectFloor + 1; iIndex < Buildings.Count; iIndex++)
			{
				Buildings[iIndex].BuildingMove(Buildings[selectFloor - 1].buildingHeight * Vector3.up);
			}
			newBuilding.GetComponent<BuildingObj>().initFunction(newBuilding, Buildings[selectFloor - 1].roofTopCenter - (initAllJijaHeight-initAllJijaHeight_DownStair)*Vector3.up, initPlatformLength * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformWidth * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformHeight * Mathf.Pow(floorScaleRatio, selectFloor), initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);

			Buildings[selectFloor - 1].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, 2);
		}
		else //select處插入一層盝頂
		{
		//錯誤
			GameObject newBuilding = new GameObject("building" + Buildings.Count);
			newBuilding.AddComponent<BuildingObj>();

			newBuilding.GetComponent<BuildingObj>().initFunction(newBuilding, Buildings[selectFloor - 1].roofTopCenter, initPlatformLength * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformWidth * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformHeight * Mathf.Pow(floorScaleRatio, selectFloor), initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);
			//UpdatePosition
			for (int iIndex = selectFloor; iIndex < Buildings.Count; iIndex++)
			{
				Buildings[iIndex].BuildingMove(Buildings[selectFloor].buildingHeight * Vector3.up);
			}
			Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);

			Buildings.Insert(selectFloor, newBuilding.GetComponent<BuildingObj>());
		}


    }
    /**
     * 刪除樓層
     */
    public void DeleteFloor(int floor)
    {
		int roofType = (int)Buildings[selectFloor].roofController.roofType;
		int allJijaHeight = (int)Buildings[selectFloor].roofController.allJijaHeight;
		int mainRidgeHeightOffset = (int)Buildings[selectFloor].roofController.mainRidgeHeightOffset;

		Destroy(Buildings[selectFloor].gameObject);
        Buildings.RemoveAt(selectFloor);
        for (int iIndex = floor; iIndex < Buildings.Count; iIndex++)
        {
			Buildings[iIndex].BuildingMove(-Buildings[selectFloor].buildingHeight * Vector3.up);
        }
        selectFloor = floor - 1;

        if (selectFloor == Buildings.Count - 1)
        {
			Buildings[selectFloor].ResetRoofFunction(mainRidgeHeightOffset, allJijaHeight,null, false, roofType);
        }
    }

    /**
     * 顯示觀察用的點
     */
    static public void ShowPos(Vector3 pos, GameObject parent, Color color, float localScale = 0.2f)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.position = pos;
        obj.transform.parent = parent.transform;
        obj.transform.localScale = Vector3.one * localScale;
        obj.GetComponent<MeshRenderer>().material.color = color;
    }
}
