using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public struct PairVector3
{
	Vector3 a;
	Vector3 b;
	public PairVector3(Vector3 a, Vector3 b)
	{
		this.a = a;
		this.b = b;
	}
	public Vector3 A { get { return a; } set { a = value; } }
	public Vector3 B { get { return b; } set { b = value; } }
}
public class ListPairVector3
{
	List<Vector3> listA;
	List<Vector3> listB;
	public ListPairVector3()
	{
		listA = new List<Vector3>();
		listB = new List<Vector3>();
	}
	public ListPairVector3(List<Vector3> listA, List<Vector3> listB)
	{
		this.listA = listA;
		this.listB = listB;
	}
	public List<Vector3> ListA { get { return listA; } set { listA = value; } }
	public List<Vector3> ListB { get { return listB; } set { listB = value; } }
	public void Add(Vector3 a, Vector3 b)
	{
		listA.Add(a);
		listB.Add(b);
	}
	public void Add(PairVector3 a)
	{
		listA.Add(a.A);
		listB.Add(a.B);
	}
	public void Clear()
	{
		listA.Clear();
		listB.Clear();
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
	public float initEaveColumnHeight = 11;
	public float initGoldColumnHeight = 11;

	public float initAllJijaHeight = 13f;
	public float initMainRidgeHeightOffset = -6;
	public float initAllJijaHeight_DownStair;
	public float initMainRidgeHeightOffset_DownStair;


    public Vector3 buildingCenter = Vector3.zero;

    public float floorScaleRatio=1.0f;
    public int buildingCount = 0;
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
            if ( Buildings.Count> 0 && selectFloor < Buildings.Count)
            { 
                DeleteFloor();
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
            LTing.GetComponent<BuildingObj>().InitFunction(LTing, LTingCenter, initPlatformLength, initPlatformWidth, initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);
            RTing.GetComponent<BuildingObj>().InitFunction(RTing, RTingCenter, initPlatformLength, initPlatformWidth, initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);

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
            building.GetComponent<BuildingObj>().InitFunction(building, buildingCenter , initPlatformLength, initPlatformWidth, initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);
            Buildings.Add(building.GetComponent<BuildingObj>());
        }

        GameObject flyRafer = new GameObject("flyRafer");
        MeshFilter meshFilter = flyRafer.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = flyRafer.AddComponent<MeshRenderer>();
        meshRenderer.material.color = Color.white;
        flyRafer.transform.parent = transform;
        List<Vector3> posList = new List<Vector3>();
        posList.Add(new Vector3(100, 0, 100));
        posList.Add(new Vector3(111, 0, 111));
        posList.Add(new Vector3(112, 0, 112));
        List<Vector3> upList = new List<Vector3>();
        upList.Add(Vector3.up);
        upList.Add(Vector3.up);
        upList.Add(Vector3.up);
        MeshCenter.Instance.CreateCurveCubeMesh(posList, upList, 0.5f, 0.5f, meshFilter);

    }

	
	/**
	 * 新增樓層
	 */
	public void CreateNewFloor()
	{
		if (selectFloor == (Buildings.Count - 1))//選的是最頂，蓋出新的一層，select那層重算成盝頂
		{
		
			GameObject newBuilding = new GameObject("building" + buildingCount++);
			newBuilding.AddComponent<BuildingObj>();


			newBuilding.GetComponent<BuildingObj>().InitFunction(newBuilding, Buildings[selectFloor].roofTopCenter - (Buildings[selectFloor].roofController.allJijaHeight - initAllJijaHeight_DownStair) * Vector3.up, initPlatformLength * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformWidth * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);

			Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);

			Buildings.Insert(selectFloor+1, newBuilding.GetComponent<BuildingObj>());

			selectFloor++;
		}
		else //select處插入一層盝頂
		{

			//UpdatePosition
			Vector3 offsetValue = (initPlatformHeight / 2.0f + initEaveColumnHeight + initAllJijaHeight_DownStair) * Vector3.up;
			for (int iIndex = selectFloor; iIndex < Buildings.Count; iIndex++)
			{
				Buildings[iIndex].BuildingMove(offsetValue);
			}

			//增加樓層
			GameObject newBuilding = new GameObject("building" + buildingCount++);
			newBuilding.AddComponent<BuildingObj>();
			newBuilding.GetComponent<BuildingObj>().InitFunction(newBuilding, (selectFloor > 0) ? (Buildings[selectFloor - 1].roofTopCenter) : (buildingCenter), initPlatformLength * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformWidth * Mathf.Pow(floorScaleRatio, selectFloor), initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);

			if (selectFloor > 0)
			{
				offsetValue=(Buildings[selectFloor - 1].roofController.allJijaHeight - initAllJijaHeight_DownStair) * Vector3.up;

				Buildings[selectFloor - 1].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);
				//如果原本上層的屋面有調整過舉架高度，要變回預設值時要補上一段offset
				for (int iIndex = selectFloor; iIndex < Buildings.Count; iIndex++)
				{
					Buildings[iIndex].BuildingMove(-offsetValue);
 				}
			}

            Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor+1].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);

            Buildings.Insert(selectFloor, newBuilding.GetComponent<BuildingObj>());
		}

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



    /**
	 * 刪除樓層
	 */
    public void DeleteFloor()
    {

        float buildingHeight = Buildings[selectFloor].buildingHeight - Buildings[selectFloor].platformController.platformHeight / 2.0f;

        for (int iIndex = selectFloor + 1; iIndex < Buildings.Count; iIndex++)
        {
            Buildings[iIndex].BuildingMove(buildingHeight * -Vector3.up);
        }

        if (selectFloor == (Buildings.Count - 1))
        {
            int roofType = (int)Buildings[selectFloor].roofController.roofType;
            float allJijaHeight = (int)Buildings[selectFloor].roofController.allJijaHeight;
            float mainRidgeHeightOffset = (int)Buildings[selectFloor].roofController.mainRidgeHeightOffset;


            Debug.Log("TopFloor");
            Vector3 offsetValue = (Buildings[selectFloor - 1].roofController.allJijaHeight - initAllJijaHeight_DownStair) * Vector3.up;
            Buildings[selectFloor - 1].ResetRoofFunction(mainRidgeHeightOffset, allJijaHeight, null, false, roofType);
			//如果原本上層的屋面有調整過舉架高度，要變回預設值時要補上一段offset
            for (int iIndex = selectFloor + 1; iIndex < Buildings.Count; iIndex++)
            {
                Buildings[iIndex].BuildingMove(-offsetValue);
            }

        }
        else
        {
            if (selectFloor != 0)
            {
                Vector3 offsetValue = (Buildings[selectFloor + 1].roofController.allJijaHeight - initAllJijaHeight_DownStair) * Vector3.up;
                Buildings[selectFloor - 1].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor + 1].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);
				//如果原本上層的屋面有調整過舉架高度，要變回預設值時要補上一段offset
                for (int iIndex = selectFloor + 1; iIndex < Buildings.Count; iIndex++)
                {
                    Buildings[iIndex].BuildingMove(-offsetValue);
                }

            }
        }

        Destroy(Buildings[selectFloor].gameObject);
        Buildings.RemoveAt(selectFloor);

        selectFloor = Mathf.Max(0, selectFloor - 1);
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
