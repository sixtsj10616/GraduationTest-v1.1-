using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EntraneIndexList
{
	public List<int> list;
	public List<int> List { get { return list; } set { list = value; } }
	public EntraneIndexList()
	{
		list = new List<int>();
	}
	public void SetEntranceIndex(params int[] index)
	{
		foreach (int i in index)
		{
			list.Add(i);
		}
	}
	public void SetEntranceIndex(List<int> inp)
	{
		foreach (int i in list)
		{
			list.Add(i);
		}
	}
	public bool Contains(int index)
	{
		return list.Contains(index);
	}
	public void RemoveAllEntranceIndex()
	{
		list.Clear();
	}
}
public class BuildingObj : MonoBehaviour {

    public GameObject building;
   
    public PlatformController platformController;
    public BodyController bodyController;
    public RoofController roofController;
    public GameObject platform;
    public GameObject body;
    public GameObject roof;

	public Vector3 platformCenter;
	public Vector3 bodyCenter;
	public Vector3 roofTopCenter;

	public float buildingHeight;                //建築(樓層)高度
	public EntraneIndexList entraneIndexList = new EntraneIndexList();

	
    /**
     * 初始化
     */
	public void InitFunction(GameObject building, Vector3 position, float platLength, float platWidth, float platHeight, float eaveColumnHeight, float goldColumnHeight, float mainRidgeHeightOffset, float allJijaHeight, List<Vector3> topFloorBorderList, bool isDownFoloor, int roofType,bool isStair)
	{
		this.building=building;
		this.building.transform.parent = building.transform;

		platformCenter=position;

		platform = new GameObject("platform");
		platform.transform.parent = this.building.transform;

		body = new GameObject("body");
		body.transform.parent = this.building.transform;

		roof = new GameObject("roof");
		roof.transform.parent = this.building.transform;

		platformController = building.AddComponent<PlatformController>();
		bodyController = building.AddComponent<BodyController>();
		roofController = building.AddComponent<RoofController>();

		//入口位置
		entraneIndexList.SetEntranceIndex(0);

        if (MainController.Instance.buildingType == MainController.BuildingType.CombinTing)
        {
            platformController.InitFunctionForCombinTing(this, platformCenter, platWidth, platLength, platHeight);
            bodyController.InitFunction(this, platformController.platFormStruct.topPointPosList, platWidth, platHeight, eaveColumnHeight, goldColumnHeight);
            roofController.InitFunction(this, bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), topFloorBorderList, platWidth, eaveColumnHeight, mainRidgeHeightOffset, allJijaHeight, isDownFoloor, roofType);
        }
        else
        {
			platformController.InitFunction(this, platformCenter, platWidth, platLength, platHeight, isStair);
            bodyController.InitFunction(this, platformController.platFormStruct.topPointPosList, platWidth, platHeight, eaveColumnHeight, goldColumnHeight);
            roofController.InitFunction(this, bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), topFloorBorderList, platWidth, eaveColumnHeight, mainRidgeHeightOffset, allJijaHeight, isDownFoloor, roofType);
        }
		buildingHeight = Vector3.Distance(roofTopCenter, platformCenter) + platformController.platHeight / 2.0f;

	}
    /**
     * 重製屋頂
     * 輸入:主脊高度位移、舉架高、上一層基座邊緣
     */
	public void ResetRoofFunction(float mainRidgeHeightOffset, float allJijaHeight, List<Vector3> topFloorBorderList, bool isDownFoloor, int roofType)
	{
		if (roof!=null) Destroy(roof);
		roof = new GameObject("roof");
		roof.transform.parent = this.building.transform;

		roofController.InitFunction(this, this.bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), topFloorBorderList, platformController.platWidth, bodyController.eaveColumnHeight, mainRidgeHeightOffset, allJijaHeight, isDownFoloor, roofType);
		buildingHeight = Vector3.Distance(roofTopCenter, platformCenter) + platformController.platHeight/2.0f;

	}

    public void ResetRoofFunction()
    {
        if (roof != null)
        {
           // yield return Destroy(roof);
            Destroy(roof);
            roof = null;
        } 
        roof = new GameObject("roof");
        roof.transform.parent = this.building.transform;


        //roofController.CreateRoof(this.bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), null, false);
        roofController.InitFunction(this, bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), null, platformController.platWidth, bodyController. eaveColumnHeight,roofController. mainRidgeHeightOffset, roofController. allJijaHeight, false, (int)roofController.roofType);

        buildingHeight = Vector3.Distance(roofTopCenter, platformCenter) + platformController.platHeight / 2.0f;
    }

    /**
     * 重製屋身
     */
    public void ResetBodyFunction()
    {
        if (body != null)
        {
            Destroy(body);
            body = new GameObject("body");
            body.transform.parent = this.building.transform;

            //bodyController.InitFunction(this, platformController.platFormStruct.topPointPosList, platWidth, platHeight, ColumnHeight, ColumnHeight);
            bodyController.ResetWithSimpleInfo(this, this.platformController.platHeight);
            buildingHeight = Vector3.Distance(roofTopCenter, platformCenter) + platformController.platHeight / 2.0f;
        }
    }
    public void AdjustBodyWidth(float width)
    {
        if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
        {
            float orgiWidth = (bodyController.origBotPosList[0] - bodyController.origBotPosList[1]).magnitude;
            float WidthOffset = (width - orgiWidth)/2 ;
            bodyController.UpdateOrigBottonPos(WidthOffset, 0);
        }
    }
    public void AdjustBodyLength(float length)
    {
        if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
        {
            float orgiLength = (bodyController.origBotPosList[0] - bodyController.origBotPosList[3]).magnitude;
            float LengthOffset = (length - orgiLength )/2;
            bodyController.UpdateOrigBottonPos(0, LengthOffset);
        }
    }
    /**
     * 重設金柱
     * 1.isCreate : 是否重建金柱 2.isReCalculate : 是否重算金柱位置
     */
    public void ResetGoldColumn(bool isCreate,bool isReCalculate)
    {
        bodyController.isGoldColumn = isCreate;
        //** 重算金柱位置
        if (isReCalculate)
        {
            bodyController.goldColumnPosList = bodyController.CalculateGoldColumnPos(bodyController.origBotPosList, entraneIndexList.List, bodyCenter);
        }
        //** 重建金柱
        if (isCreate)
        {
            bodyController.goldColumnList = bodyController.CreateRingColumn(this.body, bodyController.goldColumnPosList,
                                                                            bodyController.goldColumnRadius, bodyController.goldColumnRadius,
                                                                            bodyController.goldColumnHeight, bodyController.goldColumnRadius * 1.2f,
                                                                            bodyController.columnFundationHeight, "GoldColumn");

        }
        else
        {
            DeleteGoldColumn();
        }
    }
    /**
     * 刪除金柱
     */
    public void DeleteGoldColumn()
    {
        for (int iIndex = 0; iIndex < bodyController.goldColumnList.Count; iIndex++)
        {
            Destroy(bodyController.goldColumnList[iIndex].columnObj);
        }
        bodyController.goldColumnList.Clear();
    }
    /**
     * 重設窗戶
     */
    public void ResetWindowAndDoorNum()
    {
        for (int iIndex = 0; iIndex < bodyController.windowObjList.Count; iIndex++)
        {
            Destroy(bodyController.windowObjList[iIndex]);
        }
        for (int iIndex = 0; iIndex < bodyController.doorObjList.Count; iIndex++)
        {
            Destroy(bodyController.doorObjList[iIndex]);
        }
        bodyController.windowObjList.Clear();
        bodyController.doorObjList.Clear();
        bodyController.CreateRingWall(ModelController.Instance.goldColumnModelStruct, bodyController.GetColumnStructPosList(bodyController.goldColumnList),
                                        bodyController.goldColumnRadius, bodyController.unitNumberInBay, bodyController.doorNumber);
    }
    /**
     * 重設門楣
     */
    public void ResetFrieze(bool isCreate)
    {
        bodyController.isFrieze = isCreate;
        if (isCreate)
        {
            bodyController.CreateRingFrieze(ModelController.Instance.eaveColumnModelStruct, bodyController.GetColumnStructBottomPosList(bodyController.eaveColumnList), 
                                            bodyController.eaveColumnRadius, 0.7f * bodyController.eaveColumnHeight);
        }
        else
        {
            for (int iIndex = 0; iIndex < bodyController.friezeObjList.Count; iIndex++)
            {
                Destroy(bodyController.friezeObjList[iIndex]);
            }
            bodyController.friezeObjList.Clear();
        }
    }
    /**
     * 重設欄杆
     */
    public void ResetBalustrade(bool isCreate)
    {
        bodyController.isBalustrade = isCreate;
        if (isCreate)
        {
            bodyController.CreateRingBalustrade(ModelController.Instance.eaveColumnModelStruct, bodyController.GetColumnStructBottomPosList(bodyController.eaveColumnList),
                                                bodyController.eaveColumnRadius, 0.1f * bodyController.eaveColumnHeight);
        }
        else
        {
            for (int iIndex = 0; iIndex < bodyController.balusObjList.Count; iIndex++)
            {
                Destroy(bodyController.balusObjList[iIndex]);
            }
            bodyController.balusObjList.Clear();
        }
    }

    /**
     * 重製基座
     * 輸入:
     */
    public void ResetPlatformFunction(float platLength, float platWidth, float platHeight,bool isStair) 
	{
		if (platform != null) Destroy(platform);
		platform = new GameObject("platform");
		platform.transform.parent = this.building.transform;

		platformController.InitFunction(this, platformCenter, platWidth, platLength, platHeight, isStair);
		buildingHeight = Vector3.Distance(roofTopCenter, platformCenter) + platformController.platHeight / 2.0f;
	}
    /**
     * 重製樓梯(長寬)
     */
    public void ResetStair(float StairLength, float StairWidth)
    {
        PlatFormStruct stairInfo = platformController.platFormStruct;
        this.platformController.stairLength = StairLength;
        this.platformController.stairWidth = StairWidth;
        for (int iIndex = 0; iIndex < stairInfo.stairList.Count; iIndex++)
        {
            Destroy(stairInfo.stairList[iIndex]);
        }
        stairInfo.stairList = this.platformController.CreateRingStair(this.platform, stairInfo.borderColumnList, stairInfo.stairPosList, stairInfo.facadeDir, this.platformController.stairHeight, StairLength,StairWidth);
    }
    /**
     * 移動整棟樓層與修改原有資訊
     */
    public void BuildingMove(Vector3 offset)
    {
        building.transform.position += offset;
        platformCenter += offset;
        bodyCenter += offset;
        roofTopCenter += offset;

        platformController.MoveValueUpdate(offset);
        bodyController.MoveValueUpdate(offset);
    }
    /**
     * 移動屋身與修改原有資訊
     */
    public void MoveBuildingBody(Vector3 offset)
    {
        print("offset : "+offset);
        this.body.transform.position += offset;       
        bodyCenter += offset;
        bodyController.MoveValueUpdate(offset);
    }
    /**
     * 移動屋頂與修改原有資訊
     */
    public void MoveBuildingRoof(Vector3 offset)
    {
        this.roof.transform.position += offset;
        roofTopCenter += offset;
        //bodyController.MoveValueUpdate(offset);
    }
}
