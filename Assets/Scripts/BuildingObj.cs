﻿using System.Collections;
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

	public void BuildingMove(Vector3 offset) 
	{
		building.transform.position += offset;
		platformCenter += offset;
		bodyCenter += offset;
		roofTopCenter += offset;

		platformController.MoveValueUpdate(offset);
		bodyController.MoveValueUpdate(offset);
	}
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
		//entraneIndexList.SetEntranceIndex(1, 2);

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
	public void ResetPlatformFunction(float platLength, float platWidth, float platHeight,bool isStair) 
	{
		if (platform != null) Destroy(platform);
		platform = new GameObject("platform");
		platform.transform.parent = this.building.transform;

		platformController.InitFunction(this, platformCenter, platWidth, platLength, platHeight, isStair);
		buildingHeight = Vector3.Distance(roofTopCenter, platformCenter) + platformController.platHeight / 2.0f;
	}
}
