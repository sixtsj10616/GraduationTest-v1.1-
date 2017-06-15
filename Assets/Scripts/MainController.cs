using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

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
    public int selectFloor = 0;                                         //* 目前 選擇/建造 的樓層
    //FormFactor***********************************************************************
    public enum FormFactorSideType { ThreeSide = 3, FourSide = 4, FiveSide = 5, SixSide = 6, EightSide = 8 };
    public FormFactorSideType sides = FormFactorSideType.FourSide;

	public enum RoofType { Zan_Jian_Ding = 0, Wu_Dian_Ding = 1, Lu_Ding = 2, Juan_Peng = 3, Shya_Shan_Ding = 4 , Zan_Jian_Ding2 = 5};//Zan_Jian_Ding攢尖頂, Wu_Dian_Ding廡殿頂,Lu_Ding盝頂,Juan_Peng卷棚
    public enum BuildingType {  CombinTing = 0, Normal = 1 };
    public BuildingType buildingType = BuildingType.Normal;
    public RoofType roofType = RoofType.Zan_Jian_Ding2;
    //**********************************************************************************
	public float initPlatformWidth_DownStair = 50;
	public float initPlatformLength_DownStair = 40;


	public float initPlatformHeight_DownStair = 5;
	public float initPlatformHeight_TopStair = 1;
	public float initEaveColumnHeight = 11;
	public float initGoldColumnHeight = 11;

	public float initAllJijaHeight_TopStair = 13f;
	public float initMainRidgeHeightOffset_TopStair = -6;
	public float initAllJijaHeight_DownStair;
	public float initMainRidgeHeightOffset_DownStair;


    public Vector3 buildingCenter = Vector3.zero;
    public float floorScaleRatio=0.9f;
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
            if ( (Buildings.Count> 1 )&& (selectFloor < Buildings.Count))
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
		initAllJijaHeight_DownStair = initAllJijaHeight_TopStair / 3.0f;
		//?????這著值不能亂取
		initMainRidgeHeightOffset_DownStair = initMainRidgeHeightOffset_TopStair * (1.0f - floorScaleRatio);
        
        if (buildingType == BuildingType.CombinTing)
        {
            Vector3 LTingCenter = Vector3.zero - 10 * Vector3.right;
            Vector3 RTingCenter = Vector3.zero + 10 * Vector3.right;

            GameObject LTing = new GameObject("building1");
            GameObject RTing = new GameObject("building2");
            LTing.gameObject.AddComponent<BuildingObj>();
            RTing.gameObject.AddComponent<BuildingObj>();
			LTing.GetComponent<BuildingObj>().InitFunction(LTing, LTingCenter, initPlatformLength_DownStair, initPlatformWidth_DownStair, initPlatformHeight_DownStair, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_TopStair, initAllJijaHeight_TopStair, null, false, (int)roofType, false);
			RTing.GetComponent<BuildingObj>().InitFunction(RTing, RTingCenter, initPlatformLength_DownStair, initPlatformWidth_DownStair, initPlatformHeight_DownStair, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_TopStair, initAllJijaHeight_TopStair, null, false, (int)roofType, false);

            //**** 要搬進去 CombineTing 裡才對 ****//
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
                for (int iIndex = 0; iIndex < RTing.GetComponent<RoofController>().SurfaceList.Count; iIndex++)
                {
                    RTing.GetComponent<RoofController>().CombineTileBySurfaceList(RTing.GetComponent<RoofController>().SurfaceList[iIndex]);
                }
            }
        }
        else
        {
			GameObject building = new GameObject("building" + buildingCount++);
            building.gameObject.AddComponent<BuildingObj>();
			building.GetComponent<BuildingObj>().InitFunction(building, buildingCenter, initPlatformLength_DownStair, initPlatformWidth_DownStair, initPlatformHeight_DownStair, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_TopStair, initAllJijaHeight_TopStair, null, false, (int)roofType, false);
            Buildings.Add(building.GetComponent<BuildingObj>());
        }

        //OnPlamformSliderChange plamMenuDelegate = (Slider slider, float value) => UpdatePlameSliderInfo(slider, value);
    }

	/**
	 * 新增樓層
	 */
	public void CreateNewFloor()
	{
		if ((selectFloor == (Buildings.Count - 1) && (Buildings.Count>1)) || (Buildings.Count == 1))//選的是最頂或只有一層，蓋出新的一層，select那層重算成盝頂
			{
				//紀錄select樓層資訊
				Vector3 pos = Buildings[selectFloor].roofTopCenter;
				int type = (int)Buildings[selectFloor].roofController.roofType;
				float allJijaHeight = (int)Buildings[selectFloor].roofController.allJijaHeight;
				float mainRidgeHeightOffset = (int)Buildings[selectFloor].roofController.mainRidgeHeightOffset;
				//新加入一層樓
				GameObject newBuilding = new GameObject("building" + buildingCount++);
				newBuilding.gameObject.AddComponent<BuildingObj>();
				newBuilding.GetComponent<BuildingObj>().InitFunction(newBuilding, pos + (initPlatformHeight_TopStair / 2.0f - (allJijaHeight - initAllJijaHeight_DownStair)) * Vector3.up, initPlatformLength_DownStair * Mathf.Pow(floorScaleRatio, selectFloor+1), initPlatformWidth_DownStair * Mathf.Pow(floorScaleRatio, selectFloor+1), initPlatformHeight_TopStair, initEaveColumnHeight, initGoldColumnHeight, mainRidgeHeightOffset, allJijaHeight, null, false, type, false);

				//重新更新selectFloor屋頂
				Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);

				Buildings.Add(newBuilding.GetComponent<BuildingObj>());

				selectFloor++;
			}
			else//select處上加入一層盝頂
			{
			
				//將selectFloor以上的樓層向上搬動
				float offset = initPlatformHeight_TopStair + initEaveColumnHeight + initAllJijaHeight_DownStair;
				for(int i=selectFloor+1;i<Buildings.Count;i++)
				{
					Buildings[i].BuildingMove(offset * Vector3.up);
				}
				//紀錄select樓層資訊
				Vector3 pos = Buildings[selectFloor].roofTopCenter;

				//新加入一層樓
				GameObject newBuilding = new GameObject("building" + buildingCount++);
				newBuilding.gameObject.AddComponent<BuildingObj>();
				newBuilding.GetComponent<BuildingObj>().InitFunction(newBuilding, pos + (initPlatformHeight_TopStair / 2.0f) * Vector3.up, initPlatformLength_DownStair * Mathf.Pow(floorScaleRatio, selectFloor+1), initPlatformWidth_DownStair * Mathf.Pow(floorScaleRatio, selectFloor+1), initPlatformHeight_TopStair, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor + 1].platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding, false);
			
				if (selectFloor != 0)
				{
					
					Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);
				}

				selectFloor++;
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
        float LTingWidth = LTing.GetComponent<PlatformController>().platWidth;
        float LTingLength = LTing.GetComponent<PlatformController>().platLength;
        float TingDiagDis = Mathf.Sqrt(LTingWidth* LTingWidth + LTingLength* LTingLength);

        if (Vector3.Distance( LTingCenter , RTingCenter) < TingDiagDis)
        {
            return true;
        }
        return false;
    }

    /**
	 * 刪除樓層
     * 將刪除樓層上方的樓層修改高度資訊
     * 若為頂樓則記錄下當前屋頂資訊，移至下一樓層後Reset
     * 其他則
	 */
    public void DeleteFloor()
    {
		if ((selectFloor == (Buildings.Count - 1) && (Buildings.Count > 1))) 
		{
			//紀錄select樓層資訊
			int type = (int)Buildings[selectFloor].roofController.roofType;
			float allJijaHeight = (int)Buildings[selectFloor].roofController.allJijaHeight;
			float mainRidgeHeightOffset = (int)Buildings[selectFloor].roofController.mainRidgeHeightOffset;

			Buildings[selectFloor - 1].ResetRoofFunction(mainRidgeHeightOffset, allJijaHeight, null, false, type);

			Destroy(Buildings[selectFloor].gameObject);
			Buildings.RemoveAt(selectFloor);
			selectFloor--;
		}
		else if (selectFloor==0)
		{
			//將selectFloor以上的樓層向下搬動
			float offset = -(Buildings[selectFloor].buildingHeight - Buildings[selectFloor].platformController.platHeight / 2.0f + Buildings[selectFloor+1].platformController.platHeight / 2.0f);
			for (int i = selectFloor + 1; i < Buildings.Count; i++)
			{
				Buildings[i].BuildingMove(offset * Vector3.up);
			}
			Destroy(Buildings[selectFloor].gameObject);
			Buildings.RemoveAt(selectFloor);
		}
		else 
		{
			//將selectFloor以上的樓層向下搬動
			float offset = -(Buildings[selectFloor].buildingHeight);
			for (int i = selectFloor + 1; i < Buildings.Count; i++)
			{
				Buildings[i].BuildingMove(offset * Vector3.up);
			}

			Buildings[selectFloor - 1].ResetRoofFunction(Buildings[selectFloor].roofController.mainRidgeHeightOffset, Buildings[selectFloor].roofController.allJijaHeight, Buildings[selectFloor + 1].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, true, (int)RoofType.Lu_Ding);

			Destroy(Buildings[selectFloor].gameObject);
			Buildings.RemoveAt(selectFloor);
			selectFloor--;
		}
    }

    /**
     * 更新基座 
     */
    public void UpdatePlameSliderInfo(Slider slider)
    {
       // print(slider.name + " : "+slider.value);
        PlatformController platform = Buildings[selectFloor].platformController;
        BuildingObj nowBuilding = Buildings[selectFloor];
        float moveOffset;
        switch (slider.name)
        {
            case PlamformHandler.WidthSlider_NAME:
                nowBuilding.ResetPlatformFunction(platform.platLength, Define.initPlatWidth * slider.value , platform.platHeight, platform.isStair);
                break;
            case PlamformHandler.DepthSlider_NAME:
                nowBuilding.ResetPlatformFunction(Define.initPlatLength * slider.value, platform.platWidth, platform.platHeight, platform.isStair);
                break;
            case PlamformHandler.HeightSlider_NAME:
                moveOffset = Define.initPlatHeight * slider.value - platform.platHeight;
                //Vector3 v = new Vector3(0.0f, 0.01234f, 0.0f);
                //print("moveOffset : "+ moveOffset+ "  v = "+v);
                nowBuilding.MoveBuildingBody(new Vector3(0, moveOffset, 0));
                nowBuilding.MoveBuildingRoof(new Vector3(0, moveOffset, 0));
                nowBuilding.ResetPlatformFunction(platform.platLength, platform.platWidth, Define.initPlatHeight * slider.value, platform.isStair);
                break;
            case PlamformHandler.StairNumSlider_NAME:

                break;
            case PlamformHandler.StairLengthSlider_NAME:
                nowBuilding.ResetStair(slider.value, platform.stairWidth);  //***參數未設定好
                break;
            case PlamformHandler.StairWidthSlider_NAME:
                nowBuilding.ResetStair(platform.stairLength, slider.value);  //***參數未設定好
                break;

            default:
                print("!!! Can't Find Slider Name !!!");
                break;
        }
    }
    public void UpdatePlameToggleInfo(Toggle toggle)
    {
        PlatformController platform = Buildings[selectFloor].platformController;
        BuildingObj nowBuilding = Buildings[selectFloor];
        switch (toggle.name)
        {
            case PlamformHandler.StairToggle_NAME:
                nowBuilding.platformController.StartCreateStair(toggle.isOn);
                break;
            case PlamformHandler.BorderToggle_NAME:
                nowBuilding.platformController.StartCreateBorder(toggle.isOn);
                break;
            default:
                print("!!! Can't Find Toggle Name !!!");
                break;
        }
    }

    /**
     * 更新屋身
     */
    public void UpdateBodySliderInfo(Slider slider)
    {
        BuildingObj nowBuilding = Buildings[selectFloor];
        switch (slider.name)
        {
            case BodyMenuHandler.ColumeHeightSlider_NAME:
                float moveOffset = slider.value - nowBuilding.bodyController.eaveColumnHeight;
                nowBuilding.bodyController.eaveColumnHeight = slider.value;
                nowBuilding.bodyController.goldColumnHeight = slider.value;
                nowBuilding.MoveBuildingRoof(new Vector3(0, moveOffset, 0));
                nowBuilding.ResetBodyFunction();
                break;
            case BodyMenuHandler.GoldColNumSlider_NAME:
                nowBuilding.bodyController.goldColumnbayNumber = (int)slider.value;
                nowBuilding.DeleteGoldColumn();
                nowBuilding.ResetGoldColumn(true,true);
                nowBuilding.ResetWindowAndDoorNum();
                break;
            case BodyMenuHandler.WindowNumSlider_NAME:
                nowBuilding.bodyController.unitNumberInBay = (int)slider.value;
                nowBuilding.ResetWindowAndDoorNum();
                break;
            case BodyMenuHandler.DoorNumSlider_NAME:
                nowBuilding.bodyController.doorNumber = (int)slider.value;
                nowBuilding.ResetWindowAndDoorNum();
                break;
            case BodyMenuHandler.BodyWidthSlider_NAME:
                nowBuilding.AdjustBodyWidth(slider.value);
                nowBuilding.ResetBodyFunction();
                //nowBuilding.ResetRoofFunction();
                break;
            case BodyMenuHandler.BodyLengthSlider_NAME:
                nowBuilding.AdjustBodyLength(slider.value);
                nowBuilding.ResetBodyFunction();
                //nowBuilding.ResetRoofFunction();
                break;
        }
    }
    public void UpdateBodyToggleInfo(Toggle toggle)
    {
        BuildingObj nowBuilding = Buildings[selectFloor];
        switch (toggle.name)
        {
            case BodyMenuHandler.GoldColToggle_NAME:
                nowBuilding.ResetGoldColumn(toggle.isOn, false);
                nowBuilding.ResetWindowAndDoorNum();
                break;
            case BodyMenuHandler.FriezeToggle_NAME:
                nowBuilding.ResetFrieze(toggle.isOn);
                break;
            case BodyMenuHandler.BalustradeToggle_NAME:
                nowBuilding.ResetBalustrade(toggle.isOn);
                break;

            default:
                print("!!! Can't Find Toggle Name !!!");
                break;
        }
    }

    /**
     * 更新屋頂
     */
    public void UpdateRoofSliderInfo(Slider slider)
    {
        BuildingObj nowBuilding = Buildings[selectFloor];
        switch (slider.name)
        {
            case RoofMenuHandler.JijaHeightSlider_NAME:
                nowBuilding.roofController.allJijaHeight = slider.value;
                nowBuilding.ResetRoofFunction();
                break;
            case RoofMenuHandler.SurfaceSlider_NAME:
                nowBuilding.roofController.roofSurfaceHeightOffset = slider.value;
                nowBuilding.ResetRoofFunction();
                break;
            case RoofMenuHandler.EaveSlider_NAME:
                nowBuilding.roofController.eaveCurveHeightOffset = slider.value;
                nowBuilding.ResetRoofFunction();
                break;
            case RoofMenuHandler.RidgeSlider_NAME:
                nowBuilding.roofController.mainRidgeHeightOffset = slider.value;
                nowBuilding.ResetRoofFunction();
                break;
            case RoofMenuHandler.WingAngleSlider_NAME:
                nowBuilding.roofController.flyEaveHeightOffset = slider.value;
                nowBuilding.ResetRoofFunction();
                break;
            default:
                break;
        }

    }
    public void UpdateRoofOnSliderPointUp(Slider slider)
    {
        BuildingObj nowBuilding = Buildings[selectFloor];
        nowBuilding.ResetRoofFunction();
    }
    public void UpdateRoofToggleInfo(Toggle toggle)
    {

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
