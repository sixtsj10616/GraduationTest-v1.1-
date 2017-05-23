using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MainController : Singleton<MainController>
{

	//public GameObject building;
	public List<BuildingObj> Buildings = new List<BuildingObj>();       //* 建築樓層列表
	public int selectFloor = 0;                                      //* 目前 選擇/建造 的樓層
	public Vector3 buildingCenter = Vector3.zero;
	//FormFactor***********************************************************************
	public enum FormFactorSideType { ThreeSide = 3, FourSide = 4, FiveSide = 5, SixSide = 6, EightSide = 8 };
	public FormFactorSideType sides = FormFactorSideType.ThreeSide;

	public enum RoofType { Zan_Jian_Ding = 0, Wu_Dian_Ding = 1, Lu_Ding = 2, Juan_Peng = 3, Shya_Shan_Ding = 4 };//Zan_Jian_Ding攢尖頂, Wu_Dian_Ding廡殿頂,Lu_Ding盝頂,Juan_Peng卷棚
	public RoofType roofType = RoofType.Zan_Jian_Ding;
	//**********************************************************************************
	public float initPlatformWidth = 40;
	public float initPlatformLength = 50;
	public float initPlatformHeight = 1;
	public float initEaveColumnHeight = 11;
	public float initGoldColumnHeight = 11;

	public float initAllJijaHeight = 15f;
	public float initMainRidgeHeightOffset = -6;
	public float initAllJijaHeight_DownStair;
	public float initMainRidgeHeightOffset_DownStair;

	public float floorScaleRatio = 0.9f;
	private int buildingCount = 0;
	// Use this for initialization

	private void Awake()
	{
		InitFunction();
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			//print("A key was pressed");
			CreateNewFloor();
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			//print("D key was pressed");
			if ((Buildings.Count > 1) && (selectFloor < Buildings.Count))
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
		initAllJijaHeight_DownStair = initAllJijaHeight / 3.0f;
		//?????這著值不能亂取
		initMainRidgeHeightOffset_DownStair = initMainRidgeHeightOffset * (1.0f - floorScaleRatio);

		GameObject building = new GameObject("building" + buildingCount++);
		building.gameObject.AddComponent<BuildingObj>();
		building.GetComponent<BuildingObj>().InitFunction(building, buildingCenter, initPlatformLength, initPlatformWidth, initPlatformHeight, initEaveColumnHeight, initGoldColumnHeight, initMainRidgeHeightOffset, initAllJijaHeight, null, false, (int)roofType);
		Buildings.Add(building.GetComponent<BuildingObj>());



		GameObject flyRafer = new GameObject("flyRafer");
		MeshFilter meshFilter = flyRafer.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = flyRafer.AddComponent<MeshRenderer>();
		meshRenderer.material.color = Color.white;
		flyRafer.transform.parent = transform;
		List<Vector3> posList=new List<Vector3>();
		posList.Add(new Vector3(100,0,100));
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

				for (int iIndex = selectFloor; iIndex < Buildings.Count; iIndex++)
				{
					Buildings[iIndex].BuildingMove(-offsetValue);
				}
			}

			Buildings.Insert(selectFloor, newBuilding.GetComponent<BuildingObj>());
		}


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
				for (int iIndex = selectFloor + 1; iIndex < Buildings.Count; iIndex++)
				{
					Buildings[iIndex].BuildingMove(-offsetValue);
				}

			}
		}

		Destroy(Buildings[selectFloor].gameObject);
		Buildings.RemoveAt(selectFloor);

		selectFloor = Mathf.Max(0, selectFloor-1);


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
