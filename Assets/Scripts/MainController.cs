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

	public enum RoofType { Zan_Jian_Ding = 0, Wu_Dian_Ding = 1, Lu_Ding = 2, Juan_Peng = 3, Shya_Shan_Ding = 4, Dome = 8 };//Zan_Jian_Ding攢尖頂, Wu_Dian_Ding廡殿頂,Lu_Ding盝頂,Juan_Peng卷棚,Dome圓頂

    public enum BuildingType {  CombinTing = 0, Normal = 1 };
    public BuildingType buildingType = BuildingType.Normal;
	public RoofType roofType = RoofType.Dome;
    //**********************************************************************************
	public float initPlatformWidth_DownStair = 20;
	public float initPlatformLength_DownStair = 20;


	public float initPlatformHeight_DownStair = 5;
	public float initPlatformHeight_TopStair = 1;
	public float initEaveColumnHeight = Define.initEaveColumnHeight;
	public float initGoldColumnHeight = Define.initEaveColumnHeight;

	public float initAllJijaHeight_TopStair = 13f;
	public float initMainRidgeHeightOffset_TopStair = 0;
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
		initAllJijaHeight_DownStair = initAllJijaHeight_TopStair / 8.0f;
		//?????這著值不能亂取
		initMainRidgeHeightOffset_DownStair = initMainRidgeHeightOffset_TopStair * (1.0f - floorScaleRatio) / (initAllJijaHeight_TopStair / initAllJijaHeight_DownStair);
        
        if (buildingType == BuildingType.CombinTing)
        {
			float rotateAngle=0;
            Vector3 LTingCenter = Vector3.zero;
			ShowPos(LTingCenter,this.gameObject,Color.red,10);

            GameObject LTing = new GameObject("building1");
            GameObject RTing = new GameObject("building2");
            LTing.gameObject.AddComponent<BuildingObj>();
            RTing.gameObject.AddComponent<BuildingObj>();
			LTing.GetComponent<BuildingObj>().InitFunction(LTing, LTingCenter, initPlatformLength_DownStair, initPlatformWidth_DownStair, initPlatformHeight_DownStair, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_TopStair, initAllJijaHeight_TopStair, null,(int)roofType, false);
			//脊對齊
			#region RidgeAlign
				Vector3 RTingCenter = LTing.GetComponent<BuildingObj>().platformController.platFormStruct.topPointPosList[0];
			#endregion
			//邊對齊
			#region EdgeAlign
			//Vector3 RTingCenter = (LTing.GetComponent<BuildingObj>().platformController.platFormStruct.topPointPosList[0] + LTing.GetComponent<BuildingObj>().platformController.platFormStruct.topPointPosList[1]) / 2;
			#endregion
			RTingCenter.y = 0;
			//每一個形狀需要的間距不同
			//RTingCenter -= (RTingCenter - LTingCenter).normalized * 20;
			ShowPos(RTingCenter, this.gameObject, Color.blue, 10);
			//預設旋轉角度
			if (((int)MainController.Instance.sides % 2) == 0)
			{
				if ((int)MainController.Instance.sides == 4)
					rotateAngle = 0;
				else
					rotateAngle = 360.0f / (int)MainController.Instance.sides;
			}
			else
			{
				rotateAngle = 360.0f / (int)MainController.Instance.sides / 2;
			}
			RTing.GetComponent<BuildingObj>().InitFunction(RTing, RTingCenter, initPlatformLength_DownStair, initPlatformWidth_DownStair, initPlatformHeight_DownStair, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_TopStair, initAllJijaHeight_TopStair, null, (int)roofType, false, rotateAngle);
			//是否需要合併
			if (isNeedCombine(LTing.GetComponent<BuildingObj>(), RTing.GetComponent<BuildingObj>()))
            {
                GameObject combinTing = new GameObject("CombinTing");
                CombineTing combineTingCtrl = combinTing.AddComponent<CombineTing>();
                LTing.transform.parent = combinTing.transform;
                RTing.transform.parent = combinTing.transform;

				combineTingCtrl.InitFunction(LTing.GetComponent<BuildingObj>(), RTing.GetComponent<BuildingObj>());
	
				//** 摧毀原先兩亭的body
				Destroy(LTing.GetComponent<BuildingObj>().body);
				Destroy(RTing.GetComponent<BuildingObj>().body);
            }
			Buildings.Add(LTing.GetComponent<BuildingObj>());
        }
        else
        {
			GameObject building = new GameObject("building" + buildingCount++);
            building.gameObject.AddComponent<BuildingObj>();
			building.GetComponent<BuildingObj>().InitFunction(building, buildingCenter, initPlatformLength_DownStair, initPlatformWidth_DownStair, initPlatformHeight_DownStair, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_TopStair, initAllJijaHeight_TopStair, null,(int)roofType, false);
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
				float platLength = ((selectFloor == 0 && (Buildings.Count > 0)) ? -Buildings[selectFloor].bodyController.eaveColumnRatio2platformOffset: 0) + Buildings[selectFloor].platformController.platLength;
				float platWidth = ((selectFloor == 0 && (Buildings.Count > 0)) ? -Buildings[selectFloor].bodyController.eaveColumnRatio2platformOffset : 0) + Buildings[selectFloor].platformController.platWidth;
				//float platLength =  -Buildings[selectFloor].bodyController.eaveColumnRatio2platformOffset + Buildings[selectFloor].platformController.platLength;
				//float platWidth = -Buildings[selectFloor].bodyController.eaveColumnRatio2platformOffset + Buildings[selectFloor].platformController.platWidth;
				//新加入一層樓
				GameObject newBuilding = new GameObject("building" + buildingCount++);
				newBuilding.gameObject.AddComponent<BuildingObj>();
				newBuilding.GetComponent<BuildingObj>().InitFunction(newBuilding, pos + (initPlatformHeight_TopStair / 2.0f - (allJijaHeight - initAllJijaHeight_DownStair)) * Vector3.up, platLength * floorScaleRatio, platWidth * floorScaleRatio, initPlatformHeight_TopStair, initEaveColumnHeight, initGoldColumnHeight, mainRidgeHeightOffset, allJijaHeight, null, type, false);

				//重新更新selectFloor屋頂
				Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, (int)RoofType.Lu_Ding);

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
				float platLength = ((selectFloor == 0 && (Buildings.Count > 0)) ? -Buildings[selectFloor].platformController.platLength * 0.1f : 0) + Buildings[selectFloor].platformController.platLength;
				float platWidth = ((selectFloor == 0 && (Buildings.Count > 0)) ? -Buildings[selectFloor].platformController.platWidth*0.1f : 0) + Buildings[selectFloor].platformController.platWidth;
				//新加入一層樓
				GameObject newBuilding = new GameObject("building" + buildingCount++);
				newBuilding.gameObject.AddComponent<BuildingObj>();
				newBuilding.GetComponent<BuildingObj>().InitFunction(newBuilding, pos + (initPlatformHeight_TopStair / 2.0f) * Vector3.up, platLength * floorScaleRatio, platWidth * floorScaleRatio, initPlatformHeight_TopStair, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, Buildings[selectFloor + 1].platformController.platFormStruct.bottomPointPosList, (int)RoofType.Lu_Ding, false);
			
				if (selectFloor != 0)
				{

					Buildings[selectFloor].ResetRoofFunction(initMainRidgeHeightOffset_DownStair, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, (int)RoofType.Lu_Ding);
				}

				selectFloor++;
				Buildings.Insert(selectFloor, newBuilding.GetComponent<BuildingObj>());
			}

    }

	/**
	* 檢查兩個亭是否需要合併 (只要有另一亭的一個邊點落在亭內)
	*/
    public bool isNeedCombine(BuildingObj LTing , BuildingObj RTing)
    {
		List<Vector3> newLTingColPos = LTing.bodyController.GetColumnStructTopPosList(LTing.bodyController.eaveCornerColumnList);
		List<Vector3> newRTingColPos =RTing.bodyController.GetColumnStructTopPosList(RTing.bodyController.eaveCornerColumnList);
 /**
     * 檢查兩個亭是否需要合併(檢查向量方向)
     */
		int outPointCount2Point = 0;
		int outPointCount2Center = 0;
		for(int j=0;j<newRTingColPos.Count;j++)
		{
			Vector3 lastCross2Point=Vector3.zero;
			Vector3 lastCross2Center = Vector3.zero;
			for(int i=0;i<newLTingColPos.Count;i++)//v和所有的e 外積方向相同 代表在正規凸多邊形內部
			{
				Vector3 e=newLTingColPos[(i+1)%newLTingColPos.Count]-newLTingColPos[i];
				Vector3 v=newLTingColPos[i]- newRTingColPos[j];
				Vector3 newCross=Vector3.Cross(e,v).normalized;
				if (Vector3.Dot(newCross, lastCross2Point) < 0 && lastCross2Point != Vector3.zero)//j點落在亭外
				{
					outPointCount2Point++;
					break;
				}
				lastCross2Point = newCross;
			}
			//邊界中心點落
			for (int i = 0; i < newLTingColPos.Count; i++)
			{
				Vector3 e = newLTingColPos[(i + 1) % newLTingColPos.Count] - newLTingColPos[i];
				Vector3 v = (newLTingColPos[(i + 1) % newLTingColPos.Count] + newLTingColPos[i]) / 2;
				Vector3 newCross = Vector3.Cross(e, v).normalized;
				if (Vector3.Dot(newCross, lastCross2Center) < 0 && lastCross2Center != Vector3.zero)//j點落在亭外
				{
					outPointCount2Center++;
					break;
				}
				lastCross2Center = newCross;
			}
		}
		return !(outPointCount2Point == newRTingColPos.Count) || !(outPointCount2Center == newRTingColPos.Count);
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

			Buildings[selectFloor - 1].ResetRoofFunction(mainRidgeHeightOffset, allJijaHeight, null, type);

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

			Buildings[selectFloor - 1].ResetRoofFunction(Buildings[selectFloor].roofController.mainRidgeHeightOffset, Buildings[selectFloor].roofController.allJijaHeight, Buildings[selectFloor + 1].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList ,(int)RoofType.Lu_Ding);

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
     * UI
     */
    public void OnRoofTypeBtnClick(RoofType roofType)
    {
	print("!!!!"+roofType.ToString());
        BuildingObj nowBuilding = Buildings[selectFloor];
        nowBuilding.roofController.roofType = roofType;
        nowBuilding.ResetRoofFunction();
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
