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
    public FormFactorSideType sides = FormFactorSideType.FourSide;

	public enum RoofType { Zan_Jian_Ding = 0, Wu_Dian_Ding = 1, Lu_Ding = 2, Juan_Peng = 3, Shya_Shan_Ding = 4 , Zan_Jian_Ding2 = 5};//Zan_Jian_Ding攢尖頂, Wu_Dian_Ding廡殿頂,Lu_Ding盝頂,Juan_Peng卷棚
    public enum BuildingType {  CombinTing = 0, Normal = 1 };
    public BuildingType buildingType = BuildingType.Normal;
    public RoofType roofType = RoofType.Zan_Jian_Ding2;
    //**********************************************************************************
	public float initPlatformWidth=30;
	public float initPlatformLength = 30;
	public float initPlatformHeight = 1;
	public float initEaveColumnHeight=11;
	public float initGoldColumnHeight=11;

	public float initAllJijaHeight = 13f;
	public float initMainRidgeHeightOffset = -6;
	public float initAllJijaHeight_DownStair;
	public float initMainRidgeHeightOffset_DownStair;

	public float floorScaleRatio=1.0f;
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
        
        if (buildingType == BuildingType.CombinTing)
        {
            Vector3 LTingCenter = Vector3.zero - 10 * Vector3.right;
            Vector3 RTingCenter = Vector3.zero + 10 * Vector3.right;

            GameObject LTing = new GameObject("building1");
            GameObject RTing = new GameObject("building2");
            LTing.gameObject.AddComponent<BuildingObj>();
            RTing.gameObject.AddComponent<BuildingObj>();
            LTing.GetComponent<BuildingObj>().initFunction(LTing, LTingCenter, initPlatformLength, initPlatformWidth, initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);
            RTing.GetComponent<BuildingObj>().initFunction(RTing, RTingCenter, initPlatformLength, initPlatformWidth, initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);

            //Buildings.Add(LTing.GetComponent<BuildingObj>());
            //Buildings.Add(RTing.GetComponent<BuildingObj>());
            if (isNeedCombine(LTing,RTing))
            {
                GameObject combinTing = new GameObject("CombinTing");
                CombineTing combineTingCtrl = combinTing.AddComponent<CombineTing>();
                //BodyController bodyController = combinTing.AddComponent<BodyController>();
                LTing.transform.parent = combinTing.transform;
                RTing.transform.parent = combinTing.transform;

                //** 調整組合亭中的柱子列表，再創造出柱子
                combineTingCtrl.adjustColPos(LTing.GetComponent<PlatformController>().platFormStruct.topPointPosList, RTing.GetComponent<PlatformController>().platFormStruct.topPointPosList , LTingCenter , RTingCenter);
                combineTingCtrl.CreateRingColumn(combinTing, 1, 1, LTing.GetComponent<BodyController>().eaveColumnHeight, "TingCol");
                //** 摧毀原先兩亭的body
                Destroy(LTing.GetComponent<BuildingObj>().body);
                Destroy(RTing.GetComponent<BuildingObj>().body);
                //** 建立欄杆
                combineTingCtrl.CreateRingFrieze(ModelController.Instance, 1.2f, 0.1f * LTing.GetComponent<BodyController>().eaveColumnHeight, combinTing);

                //***開始屋頂 (檢查主脊，屋面，meshCombine)
                combineTingCtrl.checkMainRidge(LTing.GetComponent<BuildingObj>(), 1, LTingCenter, RTingCenter);
                combineTingCtrl.checkMainRidge(RTing.GetComponent<BuildingObj>(), 3, LTingCenter, RTingCenter);
                combineTingCtrl.CheckSurface(LTing.GetComponent<BuildingObj>(), 0, LTingCenter, RTingCenter);
                combineTingCtrl.CheckSurface(LTing.GetComponent<BuildingObj>(), 1, LTingCenter, RTingCenter);
                combineTingCtrl.CheckSurface(RTing.GetComponent<BuildingObj>(), 2, LTingCenter, RTingCenter);
                combineTingCtrl.CheckSurface(RTing.GetComponent<BuildingObj>(), 3, LTingCenter, RTingCenter);
                for (int iIndex = 0; iIndex < LTing.GetComponent<RoofController>().SurfaceList.Count; iIndex++)
                {
                    LTing.GetComponent<RoofController>().CombineTileBySurfaceList(LTing.GetComponent<RoofController>().SurfaceList[iIndex]);
                }
                //for (int iIndex = 0; iIndex < RTing.GetComponent<RoofController>().SurfaceList.Count; iIndex++)
                //{
                //    RTing.GetComponent<RoofController>().CombineTileBySurfaceList(RTing.GetComponent<RoofController>().SurfaceList[iIndex]);
                //}
            }
        }
        else
        {
            GameObject building = new GameObject("building");
            building.gameObject.AddComponent<BuildingObj>();
            building.GetComponent<BuildingObj>().initFunction(building, Vector3.zero, initPlatformLength, initPlatformWidth, initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);
            Buildings.Add(building.GetComponent<BuildingObj>());
        }

       
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
			//for (int iIndex = selectFloor + 1; iIndex < Buildings.Count; iIndex++)
			//{
			//	Buildings[iIndex].BuildingMove(Buildings[selectFloor - 1].buildingHeight * Vector3.up);
			//}
			newBuilding.GetComponent<BuildingObj>().initFunction(newBuilding, Buildings[selectFloor - 1].roofTopCenter - (initAllJijaHeight-initAllJijaHeight_DownStair)*Vector3.up, initPlatformLength * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformWidth * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformHeight * Mathf.Pow(floorScaleRatio, selectFloor), initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);
			Buildings[selectFloor - 1].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, 2);
		}
		else //select處插入一層盝頂
		{
		//錯誤
			GameObject newBuilding = new GameObject("building" + Buildings.Count);
			newBuilding.AddComponent<BuildingObj>();
			//newBuilding.GetComponent<BuildingObj>().initFunction(newBuilding, Buildings[selectFloor - 1].roofTopCenter, initPlatformLength * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformWidth * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformHeight * Mathf.Pow(floorScaleRatio, selectFloor), initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);
			//UpdatePosition
			for (int iIndex = selectFloor; iIndex < Buildings.Count; iIndex++)
			{
				Buildings[iIndex].BuildingMove(Buildings[selectFloor].buildingHeight * Vector3.up);
			}
            newBuilding.GetComponent<BuildingObj>().initFunction(newBuilding, Buildings[selectFloor - 1].roofTopCenter, initPlatformLength * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformWidth * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformHeight * Mathf.Pow(floorScaleRatio, selectFloor), initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);

            //Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);
            Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor+1].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);

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
			Buildings[iIndex].BuildingMove(-Buildings[floor].buildingHeight * Vector3.up);
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
    /**
     * 檢查兩個亭是否需要合併(目前只有檢查兩亭中心距離)
     */
    public bool isNeedCombine(GameObject LTing , GameObject RTing)
    {
        Vector3 LTingCenter = LTing.GetComponent<BuildingObj>().platformCenter;
        Vector3 RTingCenter = RTing.GetComponent<BuildingObj>().platformCenter;
        float LTingWidth = LTing.GetComponent<PlatformController>().platformFrontWidth;
        float LTingLength = LTing.GetComponent<PlatformController>().platformFrontLength;
        float TingDiagDis = Mathf.Sqrt(LTingWidth* LTingWidth + LTingLength* LTingLength);

        if (Vector3.Distance( LTingCenter , RTingCenter) < TingDiagDis)
        {
            return true;
        }
        return false;
    }
}
