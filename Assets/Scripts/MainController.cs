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
	public List<CombineTing> AllCT;
	public List<List<BuildingObj>> AllBuildings = new List<List<BuildingObj>>(); //* 所有建築樓層列表
	//public GameObject building;
	public List<BuildingObj> Buildings = new List<BuildingObj>();       //* 目前正在編輯建築樓層列表
	public int selectFloor = 0;                                         //* 目前 選擇/建造 的樓層
	public int selectBuildingsIndex = 0;
	//FormFactor***********************************************************************
	public enum FormFactorSideType { ThreeSide = 3, FourSide = 4, FiveSide = 5, SixSide = 6, EightSide = 8 };
	public FormFactorSideType sides = FormFactorSideType.FourSide;

	public enum RoofType { Zan_Jian_Ding = 0, Wu_Dian_Ding = 1, Lu_Ding = 2, Juan_Peng = 3, Shya_Shan_Ding = 4, Dome = 8 };//Zan_Jian_Ding攢尖頂, Wu_Dian_Ding廡殿頂,Lu_Ding盝頂,Juan_Peng卷棚,Dome圓頂

	public enum BuildingType { CombinTing = 0, Normal = 1 };
	public BuildingType buildingType = BuildingType.CombinTing;
	public RoofType roofType = RoofType.Zan_Jian_Ding;
	//**********************************************************************************
	float initPlatformWidth_DownStair = Define.initPlatWidth;
	float initPlatformLength_DownStair = Define.initPlatLength;


	float initPlatformHeight_DownStair = Define.initPlatHeight;
	float initPlatformHeight_TopStair = Define.initPlatHeight;
	float initEaveColumnHeight = Define.initEaveColumnHeight;
	float initGoldColumnHeight = Define.initEaveColumnHeight;

	float initAllJijaHeight_TopStair = Define.initJijaHeight;
	float initAllJijaHeight_DownStair = Define.initJijaHeight * 0.1f;


	public Vector3 buildingCenter = Vector3.zero;
	public float floorScaleRatio = 1.0f;
	public int buildingCount = 0;
	// Use this for initialization

	public GameObject StyleMainVC;
	public GameObject MainUI;

	private void Awake()
	{
		InitFunction();
		if (StyleMainVC.activeInHierarchy)
		{
			StyleMainVC.GetComponent<StyleMainVC>().initBuildingsInfo();
		}
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
			if ((Buildings.Count > 1) && (selectFloor < Buildings.Count))
			{
				DeleteFloor();
			}
		}
	}
	public List<Vector3> Vector3ListAddVector3(List<Vector3> list, Vector3 value)
	{
		List<Vector3> newList = null;
		if (list != null)
		{
			newList = new List<Vector3>(list);
			for (int i = 0; i < newList.Count; i++)
			{
				newList[i] += value;
			}
		}
		return newList;
	}
	public List<BuildingObj> AddBuilding(Vector3 pos, float rotateAngle)
	{
		List<BuildingObj> buildings = new List<BuildingObj>();

		//檢查是否為組合亭
		if (AllBuildings.Count > 0)
		{
			List<BuildingObj> selectBuildings = MainController.Instance.AllBuildings[selectBuildingsIndex];
			for (int i = 0; i < selectBuildings.Count; i++)
			{
				GameObject building = new GameObject("building" + buildingCount++);
				building.gameObject.AddComponent<BuildingObj>();
				Vector3 revisePos = pos;
				revisePos.y = selectBuildings[i].platformCenter.y;
				building.GetComponent<BuildingObj>().InitFunction(building, revisePos, sides, selectBuildings[i].platformController.platLength, selectBuildings[i].platformController.platWidth, selectBuildings[i].platformController.platHeight, selectBuildings[i].bodyController.eaveColumnHeight, selectBuildings[i].bodyController.goldColumnHeight, selectBuildings[i].roofController.mainRidgeHeightOffset, selectBuildings[i].roofController.allJijaHeight, Vector3ListAddVector3(selectBuildings[i].roofController.topFloorBorderList, pos), (int)selectBuildings[i].roofController.roofType, selectBuildings[i].platformController.isStair, rotateAngle);
				buildings.Add(building.GetComponent<BuildingObj>());

				BuildingObj LTing = building.GetComponent<BuildingObj>();
				//檢查與其他組合亭有無需要合併
				for (int j = AllBuildings.Count - 1; j >= 0; j--)
				{
					BuildingObj RTing = AllBuildings[j][i];
					if (isNeedCombine(LTing, RTing))
					{
						GameObject combinTing = new GameObject("CombinTing");
						CombineTing combineTingCtrl = combinTing.AddComponent<CombineTing>();
						List<BuildingObj> buildingsList = new List<BuildingObj>();
						if (RTing.transform.GetComponentInParent<CombineTing>())
						{
							Debug.Log("Already Has CombineTingoooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo");
							Destroy(combinTing);
							combinTing = RTing.transform.GetComponentInParent<CombineTing>().gameObject;
							combineTingCtrl = RTing.transform.GetComponentInParent<CombineTing>();
							LTing.transform.parent = combinTing.transform;
							buildingsList.AddRange(combineTingCtrl.BuildingsList);
							Destroy(combineTingCtrl.body.gameObject);
							Destroy(combineTingCtrl.platform.gameObject);
						}
						else
						{
							LTing.transform.parent = combinTing.transform;
							RTing.transform.parent = combinTing.transform;
							buildingsList.Add(RTing);

						}
						buildingsList.Add(LTing);

						combineTingCtrl.InitFunction(buildingsList.ToArray());

						AllCT.Add(combineTingCtrl);
						break;
					}
				}

			}
		}
		else
		{
			Debug.Log("FirstBuilding");
			GameObject building = new GameObject("building" + buildingCount++);
			building.gameObject.AddComponent<BuildingObj>();
			building.GetComponent<BuildingObj>().InitFunction(building, pos, sides, initPlatformLength_DownStair, initPlatformWidth_DownStair, initPlatformHeight_DownStair, initEaveColumnHeight, initGoldColumnHeight, 0, initAllJijaHeight_TopStair, null, (int)roofType, false, rotateAngle);
			buildings.Add(building.GetComponent<BuildingObj>());
			Debug.Log("----------------------------------- ----------------------------------- -----------------------------------");
			Debug.Log("AllBuildings.Count : " + AllBuildings.Count);
		}

		AllBuildings.Add(buildings);

		return buildings;
	}
	/**
	 * 初始化
	 */
	public void InitFunction()
	{
		if (StyleMainVC.activeInHierarchy && MainUI.activeInHierarchy)
		{
			AllBuildings.Add(AddBuilding(buildingCenter, 0));
			Buildings = AllBuildings[selectBuildingsIndex];
		}
		else
		{

		}
		//Buildings=AllBuildings[selectBuildingsIndex];
		//OnPlamformSliderChange plamMenuDelegate = (Slider slider, float value) => UpdatePlameSliderInfo(slider, value);
	}
	public void SelectBuilding(int id)
	{
		selectBuildingsIndex = id;
		Buildings = AllBuildings[selectBuildingsIndex];
		Debug.Log("SelectBuilding : " + id);
	}
	public void SelectFloor(int id)
	{
		selectFloor = id;
	}
	/**
	 * 新增樓層
	 */
	public void CreateNewFloor()
	{
		if ((selectFloor == (Buildings.Count - 1) && (Buildings.Count > 1)) || (Buildings.Count == 1))//選的是最頂或只有一層，蓋出新的一層，select那層重算成盝頂
		{
			//紀錄select樓層資訊
			Vector3 pos = Buildings[selectFloor].roofTopCenter;
			float rotateAngle = Buildings[selectFloor].rotateAngle;
			int type = (int)Buildings[selectFloor].roofController.roofType;
			float allJijaHeight = (int)Buildings[selectFloor].roofController.allJijaHeight;
			float mainRidgeHeightOffset = (int)Buildings[selectFloor].roofController.mainRidgeHeightOffset;
			float platLength = Buildings[selectFloor].platformController.platLength;
			float platWidth = Buildings[selectFloor].platformController.platWidth;
			//新加入一層樓
			GameObject newBuilding = new GameObject("building" + buildingCount++);
			newBuilding.gameObject.AddComponent<BuildingObj>();
			newBuilding.GetComponent<BuildingObj>().InitFunction(newBuilding, pos + (initPlatformHeight_TopStair / 2.0f - (allJijaHeight - initAllJijaHeight_DownStair)) * Vector3.up, sides, platLength * floorScaleRatio, platWidth * floorScaleRatio, initPlatformHeight_TopStair, initEaveColumnHeight, initGoldColumnHeight, mainRidgeHeightOffset, allJijaHeight, null, type, false, rotateAngle);

			//重新更新selectFloor屋頂
			Buildings[selectFloor].ResetRoofFunction(0, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, (int)RoofType.Lu_Ding);

			Buildings.Add(newBuilding.GetComponent<BuildingObj>());

			selectFloor++;
		}
		else//select處上加入一層盝頂
		{

			//將selectFloor以上的樓層向上搬動
			float offset = initPlatformHeight_TopStair + initEaveColumnHeight + initAllJijaHeight_DownStair;
			for (int i = selectFloor + 1; i < Buildings.Count; i++)
			{
				Buildings[i].BuildingMove(offset * Vector3.up);
			}
			//紀錄select樓層資訊
			Vector3 pos = Buildings[selectFloor].roofTopCenter;
			float rotateAngle = Buildings[selectFloor].rotateAngle;
			float platLength = Buildings[selectFloor].platformController.platLength;
			float platWidth = Buildings[selectFloor].platformController.platWidth;
			//新加入一層樓
			GameObject newBuilding = new GameObject("building" + buildingCount++);
			newBuilding.gameObject.AddComponent<BuildingObj>();
			newBuilding.GetComponent<BuildingObj>().InitFunction(newBuilding, pos + (initPlatformHeight_TopStair / 2.0f) * Vector3.up, sides, platLength * floorScaleRatio, platWidth * floorScaleRatio, initPlatformHeight_TopStair, initEaveColumnHeight, initGoldColumnHeight, 0, initAllJijaHeight_DownStair, Buildings[selectFloor + 1].platformController.platFormStruct.bottomPointPosList, (int)RoofType.Lu_Ding, false, rotateAngle);

			if (selectFloor != 0)
			{

				Buildings[selectFloor].ResetRoofFunction(0, initAllJijaHeight_DownStair, newBuilding.GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, (int)RoofType.Lu_Ding);
			}

			selectFloor++;
			Buildings.Insert(selectFloor, newBuilding.GetComponent<BuildingObj>());
		}

	}

	/**
	* 檢查兩個亭是否需要合併 (只要有另一亭的一個邊點落在亭內)
	*/
	public bool isNeedCombine(BuildingObj LTing, BuildingObj RTing)
	{
		List<Vector3> newLTingColPos = LTing.bodyController.GetColumnStructBottomPosList(LTing.bodyController.eaveCornerColumnList);
		List<Vector3> newRTingColPos = RTing.bodyController.GetColumnStructBottomPosList(RTing.bodyController.eaveCornerColumnList);
		/**
			* 檢查兩個亭是否需要合併(檢查向量方向)
			*/
		int outPointCount2Point = 0;
		int outPointCount2Center = 0;
		for (int j = 0; j < newRTingColPos.Count; j++)
		{
			Vector3 lastCross2Point = Vector3.zero;
			Vector3 lastCross2Center = Vector3.zero;
			for (int i = 0; i < newLTingColPos.Count; i++)//v和所有的e 外積方向相同 代表在正規凸多邊形內部
			{
				Vector3 e = newLTingColPos[(i + 1) % newLTingColPos.Count] - newLTingColPos[i];
				Vector3 v = newRTingColPos[j] - newLTingColPos[i];
				Vector3 newCross = Vector3.Cross(e, v).normalized;
				if ((Vector3.Dot(newCross, lastCross2Point) <= 0) && (lastCross2Point != Vector3.zero))//j點落在亭外
				{
					outPointCount2Point++;
					break;
				}
				lastCross2Point = newCross;
			}
			//邊界中心點
			for (int i = 0; i < newLTingColPos.Count; i++)
			{
				Vector3 e = newLTingColPos[(i + 1) % newLTingColPos.Count] - newLTingColPos[i];
				Vector3 v = (newRTingColPos[(j + 1) % newRTingColPos.Count] + newRTingColPos[j]) / 2 - newLTingColPos[i];
				Vector3 newCross = Vector3.Cross(e, v).normalized;
				if ((Vector3.Dot(newCross, lastCross2Center)) <= 0 && (lastCross2Center != Vector3.zero))//j點落在亭外
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
		else if (selectFloor == 0)
		{
			//將selectFloor以上的樓層向下搬動
			float offset = -(Buildings[selectFloor].buildingHeight - Buildings[selectFloor].platformController.platHeight / 2.0f + Buildings[selectFloor + 1].platformController.platHeight / 2.0f);
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

			Buildings[selectFloor - 1].ResetRoofFunction(Buildings[selectFloor].roofController.mainRidgeHeightOffset, Buildings[selectFloor].roofController.allJijaHeight, Buildings[selectFloor + 1].GetComponent<BuildingObj>().platformController.platFormStruct.bottomPointPosList, (int)RoofType.Lu_Ding);

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
		PlatformController platform = Buildings[selectFloor].platformController;
		BuildingObj nowBuilding = Buildings[selectFloor];
		CombineTing CT = nowBuilding.transform.GetComponentInParent<CombineTing>();
		switch (slider.name)
		{
			case PlamformMenuHandler.WidthSlider_NAME:
				if (CT)
				{
					CT.platformController.platWidth = slider.value;
					CT.UpdatePlatformFunction();
				}
				else
					nowBuilding.ResetPlatformFunction(platform.platLength, slider.value, platform.platHeight, platform.isStair);
				break;
			case PlamformMenuHandler.DepthSlider_NAME:
					nowBuilding.ResetPlatformFunction(slider.value, platform.platWidth, platform.platHeight, platform.isStair);
				break;
			case PlamformMenuHandler.HeightSlider_NAME:
			
				if (CT)
				{

					float moveOffset = (slider.value - CT.platformController.platHeight) / 2;
					CT.MoveBuildingBody(new Vector3(0, moveOffset, 0));
					for (int i = 0; i < CT.BuildingsList.Count; i++)
					{
						CT.BuildingsList[i].MoveBuildingRoof(new Vector3(0, moveOffset, 0));
					}
					CT.platformController.platHeight = slider.value;
					CT.UpdatePlatformFunction();
				}
				else
				{
					float moveOffset = (slider.value - nowBuilding.platformController.platHeight)/2;
					nowBuilding.MoveBuildingBody(new Vector3(0, moveOffset, 0));
					nowBuilding.MoveBuildingRoof(new Vector3(0, moveOffset, 0));
					nowBuilding.ResetPlatformFunction(platform.platLength, platform.platWidth, slider.value, platform.isStair);
				}
				break;
			case PlamformMenuHandler.StairNumSlider_NAME:

				break;
			case PlamformMenuHandler.StairLengthSlider_NAME:
				nowBuilding.ResetStair(slider.value, platform.stairWidth);  //***參數未設定好
				break;
			case PlamformMenuHandler.StairWidthSlider_NAME:
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
		CombineTing CT = nowBuilding.transform.GetComponentInParent<CombineTing>();
		switch (toggle.name)
		{
			case PlamformMenuHandler.StairToggle_NAME:
				if (CT)
				{
					CT.platformController.StartCreateStair(toggle.isOn);
				}
				else
					nowBuilding.platformController.StartCreateStair(toggle.isOn);
				break;
			case PlamformMenuHandler.BorderToggle_NAME:
				if (CT)
				{
					CT.platformController.StartCreateBorder(toggle.isOn);
				}
				else
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
		CombineTing CT = nowBuilding.transform.GetComponentInParent<CombineTing>();
		switch (slider.name)
		{
			case BodyMenuHandler.ColumeHeightSlider_NAME:
				
				if (CT)
				{
					float moveOffset = slider.value - CT.bodyController.eaveColumnHeight;
					for (int i = 0; i < CT.BuildingsList.Count; i++)
					{
						CT.BuildingsList[i].MoveBuildingRoof(new Vector3(0, moveOffset, 0));
					}
					CT.bodyController.eaveColumnHeight = slider.value;
					CT.bodyController.goldColumnHeight = slider.value;
					CT.UpdateBodyFunction();
				}
				else
				{
					float moveOffset = slider.value - nowBuilding.bodyController.eaveColumnHeight;
					nowBuilding.bodyController.eaveColumnHeight = slider.value;
					nowBuilding.bodyController.goldColumnHeight = slider.value;
					nowBuilding.MoveBuildingRoof(new Vector3(0, moveOffset, 0));
					nowBuilding.ResetBodyFunction();
				}
				break;
			case BodyMenuHandler.GoldColNumSlider_NAME:
				if (CT)
				{
					CT.bodyController.goldColumnbayNumber = (int)slider.value;
					CT.UpdateBodyFunction();
				}
				else
				{
					nowBuilding.bodyController.goldColumnbayNumber = (int)slider.value;
					nowBuilding.DeleteGoldColumn();
					nowBuilding.ResetGoldColumn(true);
					nowBuilding.ResetWindowAndDoorNum();
				}
				break;
			case BodyMenuHandler.UnitInBay_NAME:

				if (CT)
				{
					CT.bodyController.unitNumberInBay = (int)slider.value;
					CT.UpdateBodyFunction();
				}
				else
				{
					nowBuilding.bodyController.unitNumberInBay = (int)slider.value;
					nowBuilding.ResetWindowAndDoorNum();
				}
				break;
			case BodyMenuHandler.DoorNumSlider_NAME:

				if (CT)
				{
					CT.bodyController.doorNumber = (int)slider.value;
					CT.UpdateBodyFunction();
				}
				else
				{
					nowBuilding.bodyController.doorNumber = (int)slider.value;
					nowBuilding.ResetWindowAndDoorNum();
				}
				break;
			case BodyMenuHandler.BodyWidthSlider_NAME:

				if (CT)
				{
					nowBuilding.bodyController.bodyWidth = slider.value;
					nowBuilding.ResetBodyFunction();
					CT.UpdateBodyFunction();
					CT.UpdatePlatformFunction();
				}
				else
				{
					nowBuilding.bodyController.bodyWidth = slider.value;
					nowBuilding.ResetBodyFunction();
				}
				break;
			case BodyMenuHandler.BodyLengthSlider_NAME:

				if (CT)
				{
					nowBuilding.bodyController.bodyLength = slider.value;
					nowBuilding.ResetBodyFunction();
					CT.UpdateBodyFunction();
					CT.UpdatePlatformFunction();
				}
				else
				{
					nowBuilding.bodyController.bodyLength = slider.value;
					nowBuilding.ResetBodyFunction();
				}
				break;
		}

	}
	public void UpdateBodyToggleInfo(Toggle toggle)
	{
		BuildingObj nowBuilding = Buildings[selectFloor];
		CombineTing CT = nowBuilding.transform.GetComponentInParent<CombineTing>();
		switch (toggle.name)
		{
			case BodyMenuHandler.GoldColToggle_NAME:
				if (CT)
				{
					CT.ResetGoldColumn(toggle.isOn);
					CT.ResetWindowAndDoorNum();
				}
				else
				{
					nowBuilding.ResetGoldColumn(toggle.isOn);
					nowBuilding.ResetWindowAndDoorNum();
				}
				break;
			case BodyMenuHandler.FriezeToggle_NAME:
				if (CT)
				{
					CT.ResetFrieze(toggle.isOn);
				}
				else
					nowBuilding.ResetFrieze(toggle.isOn);
				break;
			case BodyMenuHandler.BalustradeToggle_NAME:
				if (CT)
				{
					CT.ResetBalustrade(toggle.isOn);
				}
				else
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
	 * 更新整體
	 */
	public void tmpUpdateRoof()
	{
		BuildingObj nowBuilding = Buildings[selectFloor];
		nowBuilding.roofController.mainRidgeHeightOffset = Random.Range(-3, 3);
		nowBuilding.ResetRoofFunction();
	}
	public void UpdateALL(List<Dictionary<string, List<DataInfo>>> DataList)
	{
		DataCenter.Instance.ArrayDataToBuildingDataMethod3(DataList);
		for (int iIndex = 0; iIndex < Buildings.Count; iIndex++)
		{
			BuildingObj nowBuilding = Buildings[iIndex];
			PlatformController platform = nowBuilding.platformController;

			//nowBuilding.ResetPlatformFunction(platform.platLength, platform.platWidth, platform.platHeight, platform.isStair);
			nowBuilding.ResetBodyFunction();
			nowBuilding.ResetRoofFunction();
		}

	}
	/**
	 * UI
	 */
	public void OnRoofTypeBtnClick(RoofType roofType)
	{
		print("!!!!" + roofType.ToString());
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
