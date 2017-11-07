using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
public class CombinedInfo
{
	//List<>第幾號亭
	//Dictionary<交第幾號亭,List<交哪幾號點>>
	public Dictionary<int, List<int>> Info;
	//累加可以尋訪的數量 一班來說當為亭的邊數時跳出ColListRecursive遞迴 但可能因為某個colIndex交其他兩亭增加(能被尋訪的次數增加)
	public int count = 0;
	public int startCount = 0;
	//紀錄所有柱子個別交了多少亭
	public List<int> colStates;
}
public class CombineTing : MonoBehaviour
{
	public MainController.FormFactorSideType sides;
	public EntraneIndexList entranceIndexList = new EntraneIndexList();
	public class BodyController4CT:BodyController
	{
		/** 
 * 條整柱子位置並存放至ColumnList中
 * 流程:先將兩亭的柱子列表內縮至適當位置，算出需移動柱子的方向向量
 *      ，由此向量與另一亭的同邊柱子位置做一個平面並計算需位移的距離
 *      兩亭皆完成此步驟後將所有點加入新的列表中，並調整 List 順序
 */
		public void InitFunction(CombineTing combineTing, List<Vector3> eaveCornerColumnPosList, List<Vector3> goldCornerColumnPosList, float eaveColumnHeight)
		{
			this.eaveColumnHeight = eaveColumnHeight;
			this.goldColumnHeight = eaveColumnHeight;

			columnFundationHeight = eaveColumnHeight * 0.05f;

			CreateBody(combineTing.body, (int)combineTing.sides, combineTing.entranceIndexList, eaveCornerColumnPosList, goldCornerColumnPosList);

		}
		private void CreateBody(GameObject parentObj, int sides, EntraneIndexList entranceIndexList, List<Vector3> eaveCornerColumnPosList, List<Vector3> goldCornerColumnPosList)
		{
			Debug.Log("CreateBody");
			eaveCornerColumnList.Clear();
			goldCornerColumnList.Clear();
			goldColumnList.Clear();
			eaveColumnList.Clear();
			//CreateBody
			List<Vector3> eaveColumnPosList = CalculateColumnPos(eaveCornerColumnPosList, entranceIndexList, eaveColumnbayNumber);
			List<Vector3> goldColumnPosList = CalculateColumnPos(goldCornerColumnPosList, entranceIndexList, goldColumnbayNumber);
			eaveColumnPosList = MainController.Instance.Vector3ListAddVector3(eaveColumnPosList, Vector3.up * eaveColumnHeight / 2.0f);
			goldColumnPosList = MainController.Instance.Vector3ListAddVector3(goldColumnPosList, Vector3.up * goldColumnHeight / 2.0f);
			eaveColumnList = CreateRingColumn(parentObj, eaveColumnPosList, eaveColumnRadius, eaveColumnRadius, eaveColumnHeight, eaveColumnRadius * 1.2f, columnFundationHeight, "TingEaveCol");
			goldColumnList = CreateRingColumn(parentObj, goldColumnPosList, goldColumnRadius, goldColumnRadius, goldColumnHeight, goldColumnRadius * 1.2f, columnFundationHeight, "TingGoldCol");
			//建築牆面
			CreateRingWall(parentObj, ModelController.Instance.goldColumnModelStruct, GetColumnStructBottomPosList(goldColumnList), goldColumnRadius, unitNumberInBay, goldColumnbayNumber, doorNumber);
			//建立欄杆
			CreateRingBalustrade(parentObj, (int)sides, entranceIndexList, ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.1f * eaveColumnHeight);
			//建立楣子
			CreateRingFrieze(parentObj, ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.7f * eaveColumnHeight);

			//角柱計算
			for (int i = 0; i < eaveColumnList.Count; i++)
			{
				foreach(Vector3 pos in eaveCornerColumnPosList)
				{
					if (eaveColumnList[i].bottomPos == pos) 
					{
						eaveCornerColumnList.Add(eaveColumnList[i]);
					}

				}	
			}
			if (isGoldColumn)
			{
				for (int i = 0; i < goldColumnList.Count; i++)
				{
					foreach (Vector3 pos in goldCornerColumnPosList)
					{
						if (goldColumnList[i].bottomPos == pos)
						{
							goldCornerColumnList.Add(goldColumnList[i]);
						}

					}
				}
			}
		}
		/**
		 * 計算簷柱位置
		*/
		public List<Vector3> CalculateColumnPos(List<Vector3> posList, EntraneIndexList entranceIndexList, int bayNumber)
		{
			List<Vector3> newPosList = new List<Vector3>();
			for (int i = 0; i < posList.Count; i++)
			{
				Vector3 columnPos = posList[i];
				newPosList.Add(columnPos);

				if (!entranceIndexList.Contains(i))
				{
					//eaveBayColumn
					int nextIndex = (i + 1) % posList.Count;
					Vector3 posNext = posList[nextIndex];

					float disBetweenColumn = Vector3.Distance(columnPos, posNext);
					float bayWidth = disBetweenColumn / bayNumber;
					Vector3 bayDir = posNext - columnPos;

					for (int j = 1; j < bayNumber; j++)
					{
						Vector3 bayColumnPos = bayDir.normalized * (j * bayWidth) + columnPos;
						newPosList.Add(bayColumnPos);
					}
				}
			}
			return newPosList;
		}
		/**
		* 建造整欄杆
		*/
		public void CreateRingBalustrade(GameObject parentObj, int sides, EntraneIndexList entraneIndexList, EaveColumnModelStruct eaveColumnModelStruct, List<Vector3> columnList, float columnRadius, float heightOffset)
		{
			float balustradeWidth = eaveColumnModelStruct.balustradeModelStruct.bound.size.x;//欄杆長度
			float balustradeHeight = eaveColumnModelStruct.balustradeModelStruct.bound.size.y;//欄杆長度
			float balustradeLengh = eaveColumnModelStruct.balustradeModelStruct.bound.size.z;//欄杆深度

			for (int i = 0; i < columnList.Count; i++)
			{
					float width = balustradeWidth;
					float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
					int number = Mathf.Max(Mathf.FloorToInt(dis / width), 1);
					Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
					float disDiff = (dis - width * number) / number;
					width = dis / number;

					float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
					for (int j = 0; j < number; j++)
					{
						Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + heightOffset * Vector3.up;
						GameObject clone = Instantiate(eaveColumnModelStruct.balustradeModelStruct.model, pos, eaveColumnModelStruct.balustradeModelStruct.model.transform.rotation) as GameObject;
						clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.balustradeModelStruct.rotation);
						clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * (width) / balustradeWidth);

						clone.transform.parent = parentObj.transform;
						balusObjList.Add(clone);
					}
			}
		}
	}
	public class PlatformController4CT:PlatformController
	{
		/**
		 * 初始化基座
		 */
		public void InitFunction(CombineTing combineTing, List<Vector3> platTopPosList, float platformHeight, bool isStair)
		{
			this.platHeight = platformHeight;
			this.isStair = isStair;
			stairHeight = platformHeight;
			//***********************************************************************
			platFormStruct = CreatePlatform(combineTing.platform,platTopPosList);
			StartCreateBorder(isBorder);
			StartCreateStair(isStair);

		}
			/**
	 * 製作基座
	 */
		private PlatFormStruct CreatePlatform(GameObject parent, List<Vector3> platTopPosList) 
		{
			PlatFormStruct platFormStruct = new PlatFormStruct();

			GameObject platformBody = new GameObject("PlatformBody");
			platformBody.transform.parent = parent.transform;
			MeshFilter meshFilter = platformBody.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = platformBody.AddComponent<MeshRenderer>();
			meshRenderer.material.color = Color.white;

			return platFormStruct;

		}
	}
	public enum AlignType { EdgeAlign, RidgeAlign };
	public AlignType alignType = AlignType.EdgeAlign;

	public List<BuildingObj> BuildingsList = new List<BuildingObj>();
	public BodyController4CT bodyController;
	public List<CombinedInfo> combinedInfos = new List<CombinedInfo>();

	public GameObject body;
	public GameObject platform;
	public void InitFunction(params BuildingObj[] buildingsList)
	{
		BuildingsList.Clear();
		BuildingsList.AddRange(buildingsList);
		//檢查亭的交界情況
		combinedInfos = CheckConnectedInfo(BuildingsList);
// 		Debug.Log("combinedInfos.Count" + combinedInfos.Count);
// 		for (int n = 0; n < combinedInfos.Count; n++)
// 		{
// 			Debug.Log("***********************************************************************");
// 			Debug.Log("combinedInfos : " + n);
// 			foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[n].Info)
// 			{
// 				int key = kvp.Key;
// 				List<int> value = kvp.Value;
// 				Debug.Log("combinedInfos.Key " + key);
// 				for (int i = 0; i < value.Count; i++)
// 				{
// 					Debug.Log("combinedInfos.Value " + value[i]);
// 				}
// 			}
// 
// 		}
// 		Debug.Log("-----------------------------------------------------------------------");
// 		for (int i = 0; i < combinedInfos.Count; i++)
// 		{
// 			for (int j = 0; j < combinedInfos[i].colStates.Count; j++)
// 			{
// 				Debug.Log("combinedInfos[" + i + "].colStates[" + j + "] : " + combinedInfos[i].colStates[j]);
// 			}
// 
// 		}
		//**************************************屋身*********************************************
		//** 調整組合亭中的柱子列表，再創造出柱子位置
		List<Vector3> eaveColList = new List<Vector3>();
		List<Vector3> goldColList = new List<Vector3>();
		AdjustColPos(BuildingsList, ref eaveColList, ref goldColList);
		body = new GameObject("body");
		body.transform.parent = this.transform;
		if (!bodyController) bodyController = this.gameObject.AddComponent<BodyController4CT>();
		bodyController.InitFunction(this, eaveColList, goldColList, BuildingsList[0].bodyController.eaveColumnHeight);
		sides = BuildingsList[0].sides;
		//** 摧毀亭的body
		for (int j = 0; j < BuildingsList.Count; j++)
		{
			Destroy(BuildingsList[j].body.gameObject);
		}
		//切割交界的脊與屋面
		CheckAllSurface();		
	}
	public void UpdateFunction()
	{
		if (body != null) 
		{
			Destroy(body);
			body=null;
		}
		InitFunction(BuildingsList.ToArray());
	}
	//此buildingIndex號亭中startColIndex是否是交界點
	bool IsConnectCol(int buildingIndex, int startColIndex)
	{
		foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[buildingIndex].Info)
		{
			List<int> value = kvp.Value;
			for (int i = 0; i < value.Count; i++)
			{
				if (value[i] == startColIndex)
				{
					return true;
				}
			}
		}
		return false;
	}
	bool IsConnectCol(int buildingIndex, int connectBuildingIndex, int startColIndex)
	{
		foreach (int value in combinedInfos[buildingIndex].Info[connectBuildingIndex])
		{
			if (value == startColIndex)
			{
				return true;
			}
		}
		return false;
	}
	//buildingIndex亭的startColIndex連接的下一亭編號
	int FindConnectBuildingIndex(int buildingIndex, int startColIndex)
	{
		List<int> keyList = new List<int>();
		foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[buildingIndex].Info)
		{
			int k = kvp.Key;
			if (combinedInfos[buildingIndex].Info[k].Contains(startColIndex))
			{
				keyList.Add(k);
			}
		}
		if (keyList.Count==0) return -1;

		int key = keyList[0];
		//交兩亭
		if (keyList.Count == 2)
		{
			if (Vector3.Distance(BuildingsList[keyList[0]].platformCenter, BuildingsList[buildingIndex].platformCenter) < Vector3.Distance(BuildingsList[keyList[1]].platformCenter, BuildingsList[buildingIndex].platformCenter))
			{
					key = keyList[1];
			}
		}
		return key;
	}

	//檢查亭的交界情況
	List<CombinedInfo> CheckConnectedInfo(List<BuildingObj> BuildingsList)
	{
		List<CombinedInfo> combinedInfos = new List<CombinedInfo>();
		for (int i = 0; i < BuildingsList.Count; i++)
		{
			CombinedInfo combinedInfo = new CombinedInfo();
			combinedInfos.Add(combinedInfo);
		}
		for (int i = 0; i < BuildingsList.Count; i++)
		{
			Dictionary<int, List<int>> info = new Dictionary<int, List<int>>();
			BuildingObj LTing = BuildingsList[i];
			for (int j = BuildingsList.Count - 1; j >= 0; j--)
			{
				if (i == j) continue;
				BuildingObj RTing = BuildingsList[j];
				if (isNeedCombine(LTing, RTing))
				{
					//Debug.Log(i + "&" + j + " Nedd to be combined");
					//檐柱
					List<Vector3> LEaveColPosList = LTing.bodyController.GetColumnStructBottomPosList(LTing.bodyController.eaveCornerColumnList);
					List<Vector3> REaveColPosList = RTing.bodyController.GetColumnStructBottomPosList(RTing.bodyController.eaveCornerColumnList);
					//中心點位置
					Vector3 LTingCenter = LTing.platformCenter;
					Vector3 RTingCenter = RTing.platformCenter;
					//交平面
					Plane midPlan = new Plane((LTingCenter - RTingCenter).normalized, (LTingCenter + RTingCenter) / 2);
					//i亭交j亭的交界點
					List<int> LIntersectionIndexList = new List<int>(FindOtherSideIndex(LEaveColPosList, RTingCenter, midPlan));
					//與j亭交LIntersectionIndexList
					info.Add(j, LIntersectionIndexList);
				}

			}
			combinedInfos[i].Info = info;
		}
		/**
		* 紀錄所有柱子個別交了多少亭
		*/
		for (int i = 0; i < BuildingsList.Count; i++)
		{
			List<int> colStates = new List<int>();
			for (int j = 0; j < BuildingsList[i].bodyController.eaveCornerColumnList.Count; j++)
			{
				colStates.Add(0);
			}
			foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[i].Info)
			{
				List<int> value = kvp.Value;
				for (int j = 0; j < value.Count; j++)
				{
					colStates[value[j]]++;
				}
			}
			combinedInfos[i].colStates = colStates;
			//檢查是否有cycle
			for (int j = 0; j < combinedInfos[i].colStates.Count; j++)
			{
				if (combinedInfos[i].colStates[j] == 3)//為內部點
				{
					combinedInfos[i].count++;
					combinedInfos[i].startCount++;
				}
			}
		}

		return combinedInfos;
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
	void ZZZ(ref List<Vector3> eaveColList, ref List<Vector3> goldColList, int fromBuildingIndex, int buildingIndex, int startColIndex, ref bool counterclockwise)
	{
		//左右亭中心
		Vector3 LTingCenter = BuildingsList[buildingIndex].platformCenter;
		Vector3 RTingCenter = new Vector3();
		//左右亭交界平面法向量與距離
		Vector3 siftDir = new Vector3();
		Plane midPlan = new Plane();
		//調整的檐柱與金柱位置
		Vector3 adjEavePos = Vector3.zero;
		Vector3 adjGoldPos = Vector3.zero;
		List<ColumnStruct> eaveCorColList = BuildingsList[buildingIndex].bodyController.eaveCornerColumnList;
		//檐柱位置
		List<Vector3> eaveColPosList = BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(eaveCorColList);
		//金柱位置
		List<Vector3> golColPosList = BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.goldCornerColumnList);


		AlignType mode = AlignType.RidgeAlign;
		//Debug.Log("buildingIndex" + buildingIndex + "	combinedInfos[buildingIndex].count" + combinedInfos[buildingIndex].count);
		if (combinedInfos[buildingIndex].count < eaveCorColList.Count)
		{
			combinedInfos[buildingIndex].count++;

			//Debug.Log("buildingIndex" + buildingIndex + "startColIndex" + startColIndex);
			//Debug.Log("colStates" + combinedInfos[buildingIndex].colStates[startColIndex]);
			eaveColList.Add(BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(eaveCorColList)[startColIndex]);
			if (BuildingsList[buildingIndex].bodyController.isGoldColumn)
			{
				goldColList.Add(BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.goldCornerColumnList)[startColIndex]);
			}
			//檢查該buildingIndex的startColIndex是否是交界點
			if (IsConnectCol(buildingIndex, startColIndex))
			{

				if (combinedInfos[buildingIndex].colStates[startColIndex] > 0)
				{
					//交界的亭號
					int nextBuildingIndex = FindConnectBuildingIndex(buildingIndex, startColIndex);
					//Debug.Log("fromBuildingIndex " + fromBuildingIndex + " nextBuildingIndex " + nextBuildingIndex);

					if (nextBuildingIndex != -1)//於交界處 調整col位置
					{
						//交界模式(邊或脊對其)
						mode = (combinedInfos[buildingIndex].Info[nextBuildingIndex].Count >= 2 && combinedInfos[nextBuildingIndex].Info[buildingIndex].Count >= 2) ? AlignType.EdgeAlign : AlignType.RidgeAlign;
						//Debug.Log("mode " + mode);
						#region AdjCol
						//下一亭交buildingIndex的號碼
						int nextTingIntersectionColIndexZ = FindOtherSideCloserIndex(buildingIndex, nextBuildingIndex, BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(eaveCorColList)[startColIndex]);
						//Debug.Log("nextTingIntersectionColIndexZ " + nextTingIntersectionColIndexZ);
						//Debug.Log("combinedInfos[buildingIndex].count " + combinedInfos[buildingIndex].count);
						//counterclockwise 需要反向(從逆時針便順時針)
						if (mode == AlignType.EdgeAlign && (combinedInfos[buildingIndex].count - combinedInfos[buildingIndex].startCount) == 1)
							{
								//Debug.Log("counterclockwise ZZZZ");
								if (combinedInfos[nextBuildingIndex].Info[buildingIndex].Contains((nextTingIntersectionColIndexZ + 1) % BuildingsList[nextBuildingIndex].bodyController.eaveCornerColumnList.Count))
								{
									counterclockwise = false;
								}
							}
							//Debug.Log("counterclockwise " + counterclockwise);
						//下一亭檐柱位置
						List<Vector3> nextEaveColPosList = BuildingsList[nextBuildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[nextBuildingIndex].bodyController.eaveCornerColumnList);
						//下一亭金柱位置
						List<Vector3> nextGolColPosList = BuildingsList[nextBuildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[nextBuildingIndex].bodyController.goldCornerColumnList);

						//交界平面
						RTingCenter = BuildingsList[nextBuildingIndex].platformCenter;
						midPlan = new Plane((LTingCenter - RTingCenter).normalized, (LTingCenter + RTingCenter) / 2.0f);
						siftDir = (counterclockwise) ? ((eaveColPosList[(startColIndex - 1 + eaveColPosList.Count) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized) : ((eaveColPosList[(startColIndex + 1) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized);
			
						int nextIntersectionStartIndex = (counterclockwise) ? (nextTingIntersectionColIndexZ + 1) % nextEaveColPosList.Count : (nextTingIntersectionColIndexZ - 1 + nextEaveColPosList.Count) % eaveCorColList.Count;
						//調整位置 非為長寬不同之矩形platform和正規多邊形
						//調整檐柱
						AdjColPos(ref adjEavePos, (int)MainController.Instance.sides, nextEaveColPosList[nextTingIntersectionColIndexZ], eaveColPosList[startColIndex], mode, midPlan, siftDir);
						//調整金柱
						AdjColPos(ref adjGoldPos, (int)MainController.Instance.sides, nextGolColPosList[nextTingIntersectionColIndexZ], golColPosList[startColIndex], mode, midPlan, siftDir);
						#endregion

						if (eaveColList.Count > 0) eaveColList[eaveColList.Count - 1] = adjEavePos;
						if (goldColList.Count > 0) goldColList[goldColList.Count - 1] = adjGoldPos;

						combinedInfos[nextBuildingIndex].count++;
						if (mode == AlignType.RidgeAlign)
						{
							//檢查某亭的colState(startColIndex是否交其他兩個亭 被跳過的亭.count要增加(尋訪次數要減少)
							if (combinedInfos[buildingIndex].colStates[startColIndex] == 2)
							{
								List<int> buildingIndexList = new List<int>();
								foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[buildingIndex].Info)
								{
									int key = kvp.Key;
									if (combinedInfos[buildingIndex].Info[key].Contains(startColIndex))
									{
										buildingIndexList.Add(key);
									}
								}
								for (int i = 0; i < buildingIndexList.Count; i++)
								{
									if (buildingIndexList[i] != nextBuildingIndex)
									{
										combinedInfos[buildingIndexList[i]].count++;
									}
								}
							}
							else 
							{
								combinedInfos[nextBuildingIndex].count--;
							}
						}
						ZZZ(ref eaveColList, ref goldColList, buildingIndex, nextBuildingIndex, nextIntersectionStartIndex, ref counterclockwise);
					}
				}
			}
			int nextStartIndex = (counterclockwise) ? (startColIndex + 1) % eaveCorColList.Count : (startColIndex - 1 + eaveCorColList.Count) % eaveCorColList.Count;
			ZZZ(ref eaveColList, ref goldColList, fromBuildingIndex, buildingIndex, nextStartIndex, ref counterclockwise);
		}

	}
	//計算調整後的檐柱與金柱位置
	void AdjColPos(ref Vector3 adjPos, int sides, Vector3 nextColIntersectPos, Vector3 startColIntersectPos, AlignType mode, Plane midPlan, Vector3 siftDir)
	{

		Plane movePlan = new Plane();
		//多邊形使用(長寬相同)
		Ray ray = new Ray();
		float siftDis = 0;
		//調整位置 非為長寬不同之矩形platform和正規多邊形
		if (sides == (int)MainController.FormFactorSideType.FourSide)//矩形
		{
			//Debug.Log("Adjust ");
			//檐柱
			movePlan = new Plane(siftDir, nextColIntersectPos);
			if (mode == AlignType.EdgeAlign) movePlan = midPlan;
			siftDis = movePlan.GetDistanceToPoint(startColIntersectPos);
			adjPos = startColIntersectPos - siftDis * siftDir;
		}
		else//正規多邊形
		{
			//Debug.Log("Adjust ");
			//檐柱
			ray = new Ray(startColIntersectPos, siftDir);
			if (midPlan.Raycast(ray, out siftDis))
				adjPos = ray.GetPoint(siftDis);
		}
	}
	//** 於init創建並調整組合亭中的角柱列表，再創造出柱子位置(eaveColList與goldColList作為output)
	public void AdjustColPos(List<BuildingObj> buildingsList, ref List<Vector3> eaveColList, ref List<Vector3> goldColList)
	{
		if (buildingsList.Count < 2) return;
		//起始編號
		int startIndex = 0;
		int buildingIndex = 0;
		bool isFind=false;
		for(int i=0;i<combinedInfos.Count;i++)
		{
			if (!isFind)
				for (int j = 0; j < combinedInfos[i].colStates.Count; j++)
				{
					if (combinedInfos[i].colStates[j] < 3)
					{
						startIndex = j;
						buildingIndex = i;
						isFind = true;
						break;
					}
				}
		}
		//Debug.Log("buildingIndex " + buildingIndex + "startIndex " + startIndex);
		bool counterclockwise = true;
		//ColListRecursive(ref eaveColList, ref goldColList, 0, 0, startIndex);
		ZZZ(ref eaveColList, ref goldColList, 0, buildingIndex, startIndex, ref counterclockwise);
		if (counterclockwise == false)
		{
			eaveColList.Reverse();
			goldColList.Reverse();
		}

	}
	//檢查亭與亭中交界點對應編號
	//tingColPos中交界的編號(intersectionIndexList)與pos最近的idex
	int FindOtherSideCloserIndex_Copy(List<int> intersectionIndexList, List<Vector3> nextTingColPos, Vector3 pos)
	{
		float RIntersectionMinDis = float.MaxValue;
		int index = 0;
		for (int i = 0; i < intersectionIndexList.Count; i++)
		{
			float dis = Vector3.Distance(pos, nextTingColPos[intersectionIndexList[i]]);
			if (dis < RIntersectionMinDis)
			{
				index = intersectionIndexList[i];
				RIntersectionMinDis = dis;
			}
		}
		return index;
	}
	int FindOtherSideCloserIndex(int buildingIndex, int nextBuildingIndex, Vector3 pos)
	{
		float RIntersectionMinDis = float.MaxValue;
		int index = 0;
		List<int> intersectionIndexList = combinedInfos[nextBuildingIndex].Info[buildingIndex];
		List<Vector3> nextTingColPos = BuildingsList[nextBuildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[nextBuildingIndex].bodyController.eaveCornerColumnList);
		for (int i = 0; i < intersectionIndexList.Count; i++)
		{
			float dis = Vector3.Distance(pos, nextTingColPos[intersectionIndexList[i]]);
			if (dis < RIntersectionMinDis)
			{
				index = intersectionIndexList[i];
				RIntersectionMinDis = dis;
			}
		}
		return index;
	}
	//檢查亭的交界點(判斷柱子位置是否與另一亭中心同側)
	List<int> FindOtherSideIndex(List<Vector3> tingColPos, Vector3 pos, Plane midplane)
	{
		List<int> indexList = new List<int>();
		for (int iIndex = 0; iIndex < tingColPos.Count; iIndex++)
		{
			if (midplane.SameSide(tingColPos[iIndex], pos))
			{
				indexList.Add(iIndex);
			}
		}
		return indexList;
	}


	/**
	 * 檢查主脊
	 */
	public void CheckMainRidge(BuildingObj Ting, int RidgeIndex, Vector3 LTingCenter, Vector3 RTingCenter)
	{
		RidgeStruct mainRidge = Ting.roofController.MainRidgeList[RidgeIndex];          //* 要被檢測的主脊
		Vector3 planeNorVector = (LTingCenter - RTingCenter).normalized;                //* 兩亭中間切平面法向量
		Plane MidPlane = new Plane(planeNorVector, (LTingCenter + RTingCenter) / 2);    //* 兩亭中間切平面
		int tileListCount = mainRidge.tilePosList.Count;                                //* 暫存主脊瓦片列表總數

		for (int iIndex = 0; iIndex < tileListCount; iIndex++)
		{
			//* 若不同邊
			if (!MidPlane.SameSide(mainRidge.tilePosList[0], Ting.roofTopCenter))
			{
				mainRidge.tilePosList.RemoveAt(0);
			}
			else
				break;
		}
		if (tileListCount != mainRidge.tilePosList.Count)
		{
			foreach (Transform child in mainRidge.body.transform)
			{
				if (child.name.Equals("main_ridge_new(Clone)"))
				{
					Destroy(child.gameObject);
				}
			}
			Ting.roofController.CreateTileByModelAndRidge(ModelController.Instance.mainRidgeModelStruct, mainRidge);
		}
	}
	/**
	 * 檢查屋面瓦片
	 */
	public void CheckAllSurface()
	{
		for (int buildingIndex = 0; buildingIndex < combinedInfos.Count; buildingIndex++)
		{
			BuildingObj LTing = BuildingsList[buildingIndex];
			Vector3 LTingCenter = LTing.platformCenter;
			int LTingSides = LTing.bodyController.eaveCornerColumnList.Count;
			foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[buildingIndex].Info)
			{
				int key = kvp.Key;
				List<int> value = kvp.Value;
				BuildingObj RTing = BuildingsList[key];
				Vector3 RTingCenter = RTing.platformCenter;
				int RTingSides = RTing.bodyController.eaveCornerColumnList.Count;
				switch (LTing.roofController.roofType)
				{
					case MainController.RoofType.Zan_Jian_Ding:
					case MainController.RoofType.Lu_Ding:
					case MainController.RoofType.Wu_Dian_Ding:

						for (int i = 0; i < value.Count; i++)
						{

							//下一亭交buildingIndex的號碼
							int intersectionColIndex = FindOtherSideCloserIndex_Copy(combinedInfos[key].Info[buildingIndex], BuildingsList[key].bodyController.GetColumnStructBottomPosList(BuildingsList[key].bodyController.eaveCornerColumnList), BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[value[i]]);
							CheckMainRidge(LTing, value[i], LTingCenter, RTingCenter);

							CheckMainRidge(RTing, intersectionColIndex, LTingCenter, RTingCenter);

							CheckSurface(LTing, (value[i] - 1 + LTingSides) % LTingSides, LTingCenter, RTingCenter);
							CheckSurface(LTing, value[i], LTingCenter, RTingCenter);

							CheckSurface(RTing, (intersectionColIndex - 1 + RTingSides) % RTingSides, LTingCenter, RTingCenter);
							CheckSurface(RTing, intersectionColIndex, LTingCenter, RTingCenter);
						}
						break;
					case MainController.RoofType.Shya_Shan_Ding:
						for (int i = 0; i < value.Count; i++)
						{
							//下一亭交buildingIndex的號碼
							int intersectionColIndex = FindOtherSideCloserIndex_Copy(combinedInfos[key].Info[buildingIndex], BuildingsList[key].bodyController.GetColumnStructBottomPosList(BuildingsList[key].bodyController.eaveCornerColumnList), BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[value[i]]);



							CheckMainRidge(LTing, value[i] * 2, LTingCenter, RTingCenter);
							CheckMainRidge(LTing, value[i] * 2 + 1, LTingCenter, RTingCenter);

							CheckMainRidge(RTing, intersectionColIndex * 2, LTingCenter, RTingCenter);
							CheckMainRidge(RTing, intersectionColIndex * 2 + 1, LTingCenter, RTingCenter);

							CheckSurface(LTing, (value[i] - 1 + LTingSides) % LTingSides, LTingCenter, RTingCenter);
							CheckSurface(LTing, value[i], LTingCenter, RTingCenter);

							CheckSurface(RTing, (intersectionColIndex - 1 + RTingSides) % RTingSides, LTingCenter, RTingCenter);
							CheckSurface(RTing, intersectionColIndex, LTingCenter, RTingCenter);
						}
						break;
					case MainController.RoofType.Dome:
						CheckSurface(LTing, 0, LTingCenter, RTingCenter);
						CheckSurface(RTing, 0, LTingCenter, RTingCenter);
						break;

				}
			}
		}

	}
	public void CheckSurface(BuildingObj Ting, int SurfaceIndex, Vector3 LTingCenter, Vector3 RTingCenter)
	{
		RoofSurfaceStruct surface = Ting.roofController.SurfaceList[SurfaceIndex];
		Plane midPlane = new Plane((LTingCenter - RTingCenter).normalized, (LTingCenter + RTingCenter) / 2);    //* 兩亭中間切平面

		//** 依序檢查每一個表面脊 (左、中、右分開做)
		for (int iIndex = 0; iIndex < surface.leftRoofSurfaceTileRidgeList.Count; iIndex++)
		{
			RoofSurfaceRidgeStruct surfaceRidge = surface.leftRoofSurfaceTileRidgeList[iIndex];
			RemoveSurfacePosByPlane(midPlane, surfaceRidge, Ting.roofTopCenter);
		}
		if (surface.midRoofSurfaceTileRidge != null) RemoveSurfacePosByPlane(midPlane, surface.midRoofSurfaceTileRidge, Ting.roofTopCenter);
		for (int iIndex = 0; iIndex < surface.rightRoofSurfaceTileRidgeList.Count; iIndex++)
		{
			RoofSurfaceRidgeStruct surfaceRidge = surface.rightRoofSurfaceTileRidgeList[iIndex];
			RemoveSurfacePosByPlane(midPlane, surfaceRidge, Ting.roofTopCenter);
		}
		//** 
	}
	/**
	 * 刪除超過平面的瓦片位置
	 * 輸入: 切割平面、屋面瓦片用脊、同側的基準點
	 */
	private void RemoveSurfacePosByPlane(Plane MidPlane, RoofSurfaceRidgeStruct surfaceRidge, Vector3 BasePos)
	{
		//如果和BasePos不同側表示屋面有被平面切割
		if (surfaceRidge.tilePosList.Count > 0 && !MidPlane.SameSide(surfaceRidge.tilePosList[0], BasePos))
		{
			int tileListCount = surfaceRidge.tilePosList.Count;
			for (int iPosIndex = 0; iPosIndex < tileListCount; iPosIndex++)
			{
				//** 若檢查瓦片用脊未到倒數第二個前，檢查方式除了當前瓦片外，還會看下一個瓦片是否也需要刪除，用來讓兩亭交接處不出現鋸齒狀空洞
				if (iPosIndex < tileListCount - 1)
				{
					if (!MidPlane.SameSide(surfaceRidge.tilePosList[0], BasePos) && !MidPlane.SameSide(surfaceRidge.tilePosList[1], BasePos))
					{
						surfaceRidge.tilePosList.RemoveAt(0);
						surfaceRidge.roundTileModelList[0].transform.parent = null;
						Destroy(surfaceRidge.roundTileModelList[0].gameObject);
						surfaceRidge.roundTileModelList.RemoveAt(0);
					}
					else
					{
						break;
					}
				}//只有一個瓦片的時候
				else if (!MidPlane.SameSide(surfaceRidge.tilePosList[0], BasePos))
				{
					surfaceRidge.tilePosList.RemoveAt(0);
					surfaceRidge.roundTileModelList[0].transform.parent = null;
					Destroy(surfaceRidge.roundTileModelList[0].gameObject);
					surfaceRidge.roundTileModelList.RemoveAt(0);
				}
				else
					break;
			}
		}
	}

	/*************************************************************************************************************/
	/**
 * 重設金柱
 * 1.isCreate : 是否重建金柱 2.isReCalculate : 是否重算金柱位置
 */
	public void ResetGoldColumn(bool isCreate)
	{
		bodyController.isGoldColumn = isCreate;
		//** 重建金柱
		if (isCreate)
		{
			Debug.Log("CTCreateGoldColumn");
			Debug.Log("bodyController.GetColumnStructBottomPosList(bodyController.goldCornerColumnList)" + bodyController.GetColumnStructBottomPosList(bodyController.goldCornerColumnList).Count);
			List<Vector3> goldColumnPosList = bodyController.CalculateColumnPos(bodyController.GetColumnStructBottomPosList(bodyController.goldCornerColumnList), entranceIndexList, bodyController.goldColumnbayNumber);
			goldColumnPosList = MainController.Instance.Vector3ListAddVector3(goldColumnPosList, Vector3.up * bodyController.goldColumnHeight / 2.0f);
			bodyController.goldColumnList = bodyController.CreateRingColumn(this.body, goldColumnPosList,
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
		Debug.Log("ResetWindowAndDoorNum");
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

		bodyController.CreateRingWall(body, ModelController.Instance.goldColumnModelStruct, bodyController.GetColumnStructBottomPosList(bodyController.goldColumnList), bodyController.goldColumnRadius, bodyController.unitNumberInBay, bodyController.goldColumnbayNumber, bodyController.doorNumber);
	}
	/**
   * 重設門楣
   */
	public void ResetFrieze(bool isCreate)
	{
		bodyController.isFrieze = isCreate;
		if (isCreate)
		{
			bodyController.CreateRingFrieze(body, ModelController.Instance.eaveColumnModelStruct, bodyController.GetColumnStructBottomPosList(bodyController.eaveColumnList),
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
			bodyController.CreateRingBalustrade(body, (int)sides, entranceIndexList, ModelController.Instance.eaveColumnModelStruct, bodyController.GetColumnStructBottomPosList(bodyController.eaveColumnList),
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
}
