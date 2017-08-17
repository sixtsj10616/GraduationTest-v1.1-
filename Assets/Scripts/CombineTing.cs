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
	public int count=0;
	//累加交界的數量
	public int interIndexCount = 0;
	//紀錄所有柱子個別交了多少亭
	public List<int> colStates;

}
public class CombineTing : MonoBehaviour
{
	public class BodyController4CT
	{
		public List<Vector3> eaveColumnPosList = new List<Vector3>();   //* 簷柱位置列表
		public List<Vector3> goldColumnPosList = new List<Vector3>();   //* 金柱位置列表
		public List<Vector3> eaveCornerColumnPosList = new List<Vector3>();   //* 簷柱位置列表
		public List<Vector3> goldCornerColumnPosList = new List<Vector3>();   //* 金柱位置列表
		public List<GameObject> windowObjList = new List<GameObject>(); //* 窗戶物件列表
		public List<GameObject> doorObjList = new List<GameObject>();   //* 大門物件列表
		public int goldColumnBayNumber = 3;//間數量
		public int eaveColumnBayNumber = 1;
		public int unitNumberInBay = 3;//間內有幾個單位(如:一間內有幾個門+窗)
		public int doorNumber = 1;//門的數量(若間為奇數 則門為單扇 / 偶數 則門為雙扇)
		/** 
 * 條整柱子位置並存放至ColumnList中
 * 流程:先將兩亭的柱子列表內縮至適當位置，算出需移動柱子的方向向量
 *      ，由此向量與另一亭的同邊柱子位置做一個平面並計算需位移的距離
 *      兩亭皆完成此步驟後將所有點加入新的列表中，並調整 List 順序
 */
		public void InitFunction(GameObject parent, List<Vector3> eaveCornerColumnPosList, List<Vector3> goldCornerColumnPosList, float eaveColumnHeight)
		{
			this.eaveCornerColumnPosList = eaveCornerColumnPosList;
			this.goldCornerColumnPosList = goldCornerColumnPosList;
			//CreateBody
			eaveColumnPosList.Clear();
			goldColumnPosList.Clear();
			List<int> entranceIndexList = new List<int>();
			eaveColumnPosList = CalculateColumnPos(eaveCornerColumnPosList, entranceIndexList, eaveColumnBayNumber);
			goldColumnPosList = CalculateColumnPos(goldCornerColumnPosList, entranceIndexList, goldColumnBayNumber);
			CreateRingColumn(parent, eaveColumnPosList, 1, 1, eaveColumnHeight, "TingEaveCol");
			CreateRingColumn(parent, goldColumnPosList, 1, 1, eaveColumnHeight, "TingGoldCol");
			//建築牆面
			CreateRingWall(ModelController.Instance.goldColumnModelStruct, goldColumnPosList, eaveColumnHeight, 1, unitNumberInBay, goldColumnBayNumber, doorNumber, parent);
			//** 建立欄杆
			CreateRingBalustrade(ModelController.Instance, eaveColumnPosList, 1.2f, 0.1f * eaveColumnHeight, parent);
			CreateRingFrieze(ModelController.Instance, eaveColumnPosList, 1f, 0.8f * eaveColumnHeight, eaveColumnHeight, parent);

			//*** (meshCombine)
			//                 for (int iIndex = 0; iIndex < LTing.GetComponent<RoofController>().SurfaceList.Count; iIndex++)
			//                 {
			//                     LTing.GetComponent<RoofController>().CombineTileBySurfaceList(LTing.GetComponent<RoofController>().SurfaceList[iIndex]);
			//                 }
			//                 for (int iIndex = 0; iIndex < RTing.GetComponent<RoofController>().SurfaceList.Count; iIndex++)
			//                 {
			//                     RTing.GetComponent<RoofController>().CombineTileBySurfaceList(RTing.GetComponent<RoofController>().SurfaceList[iIndex]);
			//                 }
		}

		/**
		 * 計算簷柱位置
		*/
		private List<Vector3> CalculateColumnPos(List<Vector3> posList, List<int> entranceIndexList, int bayNumber)
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
		 * 建立柱子群模型 (從BodyController搬來的，發現那個還是不好用)
		 */
		public List<ColumnStruct> CreateRingColumn(GameObject parentObj, List<Vector3> ColumnList, float columnTopRadius, float columnDownRadius, float columnHeight, string columnName)
		{
			List<ColumnStruct> columnList = new List<ColumnStruct>();
			for (int i = 0; i < ColumnList.Count; i++)
			{
				ColumnStruct newColumn = CreateColumn(parentObj, ColumnList[i], columnTopRadius, columnDownRadius, columnHeight, columnDownRadius * 1.2f, columnHeight * 0.05f, columnName);
			}
			return columnList;
		}
		/**
		 * 建立柱子模型  (從BodyController搬來的，發現那個還是不好用)
		 */
		private ColumnStruct CreateColumn(GameObject parentObj, Vector3 pos, float topRadius, float downRadius, float height, float fundationRadius, float fundationHeight, string name = "Column")
		{
			ColumnStruct columnStruct = new ColumnStruct();

			float columnFundationHeight = height * 0.05f;
			float columnRemainHeight = height - columnFundationHeight;

			GameObject col = new GameObject(name);
			//col.transform.position = pos + new Vector3(0, columnRemainHeight / 2.0f + columnFundationHeight, 0);
			col.transform.parent = parentObj.transform;
			col.AddComponent<CylinderMesh>();

			Vector3 topPos = pos + (columnRemainHeight + columnFundationHeight) * Vector3.up;
			Vector3 bottomPos = pos + columnFundationHeight * Vector3.up;

			col.GetComponent<CylinderMesh>().CylinderInitSetting(pos, topPos, bottomPos, topRadius, downRadius);
			col.GetComponent<CylinderMesh>().SetMesh();
			columnStruct.columnObj = col;
			columnStruct.columnMesh = col.GetComponent<CylinderMesh>();
			columnStruct.topPos = topPos;
			columnStruct.bottomPos = bottomPos;
			//Fundation
			GameObject fun = new GameObject(name + "Fundation");
			fun.transform.parent = col.transform;
			fun.AddComponent<CylinderMesh>();

			topPos = pos + columnFundationHeight * Vector3.up;
			bottomPos = pos;

			fun.GetComponent<CylinderMesh>().CylinderInitSetting(pos - new Vector3(0, height / 2.0f, 0), topPos, bottomPos, fundationRadius, fundationRadius);
			fun.GetComponent<CylinderMesh>().SetMesh();

			columnStruct.fundation = col.GetComponent<CylinderMesh>();

			return columnStruct;
		}   /**
     * 建造門楣模型
     */
		public void CreateRingFrieze(ModelController modelController, List<Vector3> ColumnList, float columnRadius, float heightOffset, float eaveColumnHeight, GameObject parent)
		{
			float friezeWidth = modelController.eaveColumnModelStruct.friezeModelStruct.bound.size.x;//裝飾物長度
			float friezeHeight = modelController.eaveColumnModelStruct.friezeModelStruct.bound.size.y;//裝飾物長度
			float friezeLengh = modelController.eaveColumnModelStruct.friezeModelStruct.bound.size.z;//裝飾物深度
			for (int i = 0; i < ColumnList.Count; i++)
			{
				float width = friezeWidth;
				float height = friezeHeight;
				float dis = Vector3.Distance(ColumnList[i], ColumnList[(i + 1) % ColumnList.Count]) - columnRadius * 2;
				int number = Mathf.Max(Mathf.FloorToInt(dis / width), 1);
				Vector3 dir = ColumnList[(i + 1) % ColumnList.Count] - ColumnList[i];
				float disDiff = (dis - width * number) / number;
				width = dis / number;
				float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
				for (int j = 0; j < number; j++)
				{
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + ColumnList[i] + heightOffset * Vector3.up;
					GameObject clone = Instantiate(modelController.eaveColumnModelStruct.friezeModelStruct.model, pos, modelController.eaveColumnModelStruct.friezeModelStruct.model.transform.rotation) as GameObject;
					clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(modelController.eaveColumnModelStruct.friezeModelStruct.rotation);
					clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * (width) / friezeWidth);
					clone.transform.parent = parent.transform;
				}

				//FriezeWall(frieze上方裝飾牆)
				GameObject friezeWall = new GameObject("FriezeWall");
				MeshFilter meshFilter = friezeWall.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = friezeWall.AddComponent<MeshRenderer>();
				friezeWall.transform.parent = parent.transform;
				meshRenderer.material.color = Color.yellow;
				float rotateAngleZ = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
				float friezeWallHeight = (eaveColumnHeight - heightOffset - friezeHeight / 2.0f);
				Vector3 posZ = (ColumnList[i] + ColumnList[(i + 1) % ColumnList.Count]) / 2.0f + (heightOffset + friezeHeight / 2.0f + friezeWallHeight / 2.0f) * Vector3.up;
				MeshCenter.Instance.CreateCubeMesh(posZ, dis, friezeWallHeight, 0.5f, rotateAngleZ, meshFilter);

				if (dis >= modelController.eaveColumnModelStruct.sparrowBraceModelStruct.bound.size.x * 2.5f)
				{
					//sparrowBrace雀替
					Vector3 posX = dir.normalized * (columnRadius) + ColumnList[i] + (heightOffset - friezeHeight / 2.0f) * Vector3.up;
					float rotateAngleX = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
					GameObject sparrowBrace = Instantiate(modelController.eaveColumnModelStruct.sparrowBraceModelStruct.model, posX, modelController.eaveColumnModelStruct.sparrowBraceModelStruct.model.transform.rotation) as GameObject;
					sparrowBrace.transform.rotation = Quaternion.AngleAxis(rotateAngleX, Vector3.up) * Quaternion.Euler(modelController.eaveColumnModelStruct.sparrowBraceModelStruct.rotation);

					sparrowBrace.transform.parent = parent.transform;

					posX = -dir.normalized * (columnRadius) + ColumnList[(i + 1) % ColumnList.Count] + (heightOffset - friezeHeight / 2.0f) * Vector3.up;
					rotateAngleX = (Vector3.Dot(Vector3.forward, -dir) < 0 ? Vector3.Angle(-dir, Vector3.right) : -Vector3.Angle(-dir, Vector3.right));
					sparrowBrace = Instantiate(modelController.eaveColumnModelStruct.sparrowBraceModelStruct.model, posX, modelController.eaveColumnModelStruct.sparrowBraceModelStruct.model.transform.rotation) as GameObject;
					sparrowBrace.transform.rotation = Quaternion.AngleAxis(rotateAngleX, Vector3.up) * Quaternion.Euler(modelController.eaveColumnModelStruct.sparrowBraceModelStruct.rotation);

					sparrowBrace.transform.parent = parent.transform;
				}
			}
		}
		/**
		 * 建立柵欄模型  (從BodyController搬來的，發現那個還是不好用)
		 */
		public void CreateRingBalustrade(ModelController modelController, List<Vector3> ColumnList, float columnRadius, float heightOffset, GameObject parent)
		{
			float balustradeWidth = modelController.eaveColumnModelStruct.balustradeModelStruct.bound.size.x;//欄杆長度
			float balustradeHeight = modelController.eaveColumnModelStruct.balustradeModelStruct.bound.size.y;//欄杆長度
			float balustradeLengh = modelController.eaveColumnModelStruct.balustradeModelStruct.bound.size.z;//欄杆深度

			for (int i = 0; i < ColumnList.Count; i++)
			{
				float width = balustradeWidth;
				float dis = Vector3.Distance(ColumnList[i], ColumnList[(i + 1) % ColumnList.Count]) - columnRadius * 2;
				int number = Mathf.Max(Mathf.FloorToInt(dis / width), 1);
				Vector3 dir = ColumnList[(i + 1) % ColumnList.Count] - ColumnList[i];
				float disDiff = (dis - width * number) / number;
				width = dis / number;

				float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
				for (int j = 0; j < number; j++)
				{
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + ColumnList[i] + heightOffset * Vector3.up;
					GameObject clone = Instantiate(modelController.eaveColumnModelStruct.balustradeModelStruct.model, pos, modelController.eaveColumnModelStruct.balustradeModelStruct.model.transform.rotation) as GameObject;
					clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(modelController.eaveColumnModelStruct.balustradeModelStruct.rotation);
					clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * (width) / balustradeWidth);
					clone.transform.parent = parent.transform;
				}
			}
		}
		/**
		* 建造整圈牆模型 
		 */
		public void CreateRingWall(GoldColumnModelStruct goldColumnModelStruct, List<Vector3> columnList, float goldColumnHeight, float columnRadius, int unit, int goldColumnBayNumber, int doorNumber, GameObject parentObj)
		{
			float wallHeight = goldColumnHeight;//牆長度
			float wallLengh = columnRadius * 2.0f;//牆深度

			float windowWidth = goldColumnModelStruct.windowModelStruct.bound.size.x;//裝飾物長度
			float windowHeight = goldColumnModelStruct.windowModelStruct.bound.size.y;//裝飾物長度
			float windowLengh = goldColumnModelStruct.windowModelStruct.bound.size.z;//裝飾物深度


			float doorWidth = goldColumnModelStruct.doorModelStruct.bound.size.x;//裝飾物長度
			float doorHeight = goldColumnModelStruct.doorModelStruct.bound.size.y;//裝飾物長度
			float doorLengh = goldColumnModelStruct.doorModelStruct.bound.size.z;//裝飾物深度


			float windowWallWidth = goldColumnModelStruct.windowWallModelStruct.bound.size.x;//裝飾物長度
			float windowWallHeight = goldColumnModelStruct.windowWallModelStruct.bound.size.y;//裝飾物長度
			float windowWallLengh = goldColumnModelStruct.windowWallModelStruct.bound.size.z;//裝飾物深度

			float doorMidIndex = ((float)goldColumnBayNumber / 2);
			doorNumber = Mathf.Clamp(doorNumber, 0, Mathf.CeilToInt(doorMidIndex));
			int doorMaxIndex = (int)((goldColumnBayNumber % 2 == 1) ? (doorMidIndex + (doorNumber - 1)) : (doorMidIndex + (doorNumber - 1) + 0.5f));
			int doorMinIndex = (int)((goldColumnBayNumber % 2 == 1) ? (doorMidIndex - (doorNumber - 1)) : (doorMidIndex - (doorNumber - 1) - 0.5f));
			for (int i = 0; i < columnList.Count; i++)
			{
				float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;

				Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
				//門
				if (((i % (goldColumnBayNumber)) <= doorMaxIndex) && (i % (goldColumnBayNumber) >= doorMinIndex))
				{
					float width = dis;
					for (int j = 0; j < 1; j++)
					{
						float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
						Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + wallHeight / 2.0f * Vector3.up;
						float disDiff = doorWidth - width;
						float doorWidthScale = (width) / (doorWidth);
						float doorHeightScale = wallHeight / (doorHeight);
						//door
						GameObject clone = Instantiate(goldColumnModelStruct.doorModelStruct.model, pos, goldColumnModelStruct.doorModelStruct.model.transform.rotation) as GameObject;
						clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(goldColumnModelStruct.doorModelStruct.rotation);
						clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x * doorWidthScale, clone.transform.GetChild(0).localScale.y * doorHeightScale, (clone.transform.GetChild(0).localScale.z));
						//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, clone.transform.rotation * clone.transform.GetChild(0).transform.rotation * (new Vector3(doorWidthScale, doorHeightScale, 1)));
						clone.transform.parent = parentObj.transform;
						doorObjList.Add(clone);
					}

				}
				else//窗
				{
					float width = dis / unit;
					for (int j = 0; j < unit; j++)
					{
						#region windowWall
						float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
						Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + wallHeight / 2.0f * Vector3.up;
						float disDiff = windowWallWidth - width;
						float windowWallWidthScale = (width) / (windowWallWidth);
						float windowWallHeightScale = wallHeight / (windowWallHeight);
						//windowWall
						GameObject clone = Instantiate(goldColumnModelStruct.windowWallModelStruct.model, pos, goldColumnModelStruct.windowWallModelStruct.model.transform.rotation) as GameObject;
						clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(goldColumnModelStruct.windowWallModelStruct.rotation);
						clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x * windowWallWidthScale, clone.transform.GetChild(0).localScale.y * windowWallHeightScale, (clone.transform.GetChild(0).localScale.z));

						clone.transform.parent = parentObj.transform;
						windowObjList.Add(clone);
						#endregion
					}
				}

			}
		}
		/** 
		 * 交換 ColumnList 中兩指定位置內容 (需先用過adjustColPos)
		 */
		private void SwitchColumnListPos(List<Vector3> ColumnList, int firstPos, int secondPos)
		{
			Vector3 tmpV3 = ColumnList[firstPos];
			ColumnList[firstPos] = ColumnList[secondPos];
			ColumnList[secondPos] = tmpV3;
		}
		/**
		 * 縮放柱子位置
		 * 輸入 : 柱子列表，柱子群中心點，向中心縮減的長度
		 */
		private List<Vector3> ScaleColumnPos(List<Vector3> ColList, Vector3 Center, float scaleDis)
		{
			List<Vector3> NewColList = new List<Vector3>();
			for (int iIndex = 0; iIndex < ColList.Count; iIndex++)
			{
				Vector2 v2CenterToCol = new Vector2(ColList[iIndex].x - Center.x, ColList[iIndex].z - Center.z);
				Vector3 eaveColumnPos = ColList[iIndex] - new Vector3(v2CenterToCol.x, 0, v2CenterToCol.y).normalized * scaleDis;
				NewColList.Add(eaveColumnPos);
			}
			return NewColList;
		}
	}

	public enum AlignType { EdgeAlign, RidgeAlign };
	public AlignType alignType = AlignType.EdgeAlign;

	public List<BuildingObj> BuildingsList = new List<BuildingObj>();
	public List<int> listBuildingIndex = new List<int>();
	public BodyController4CT bodyCtrl4CT;

	public List<CombinedInfo> combinedInfos = new List<CombinedInfo>();


	public void InitFunction(params BuildingObj[] buildingsList)
	{

		BuildingsList.AddRange(buildingsList);
		//檢查亭的交界情況
		combinedInfos = CheckConnectedInfo(BuildingsList);
		Debug.Log("combinedInfos.Count" + combinedInfos.Count);
		for (int n = 0; n < combinedInfos.Count; n++)
		{
			Debug.Log("***********************************************************************");
			Debug.Log("combinedInfos : " + n);
			foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[n].Info)
			{
				int key = kvp.Key;
				List<int> value = kvp.Value;
				Debug.Log("combinedInfos.Key " + key);
				for (int i = 0; i < value.Count; i++)
				{
					Debug.Log("combinedInfos.Value " + value[i]);
				}
			}

		}
		Debug.Log("-----------------------------------------------------------------------");
		for (int i = 0; i < combinedInfos.Count; i++)
		{
			for(int j=0;j<combinedInfos[i].colStates.Count;j++)
			{
				Debug.Log("combinedInfos[" + i + "].colStates["+j+"] : " + combinedInfos[i].colStates[j]);
			}
			
		}

		//** 調整組合亭中的柱子列表，再創造出柱子位置
		List<Vector3> eaveColList = new List<Vector3>();
		List<Vector3> goldColList = new List<Vector3>();
		AdjustColPos(BuildingsList, ref eaveColList, ref goldColList);
		Debug.Log("eaveColList.Count" + eaveColList.Count);
		//創建屋身
		bodyCtrl4CT = new BodyController4CT();
		bodyCtrl4CT.InitFunction(this.gameObject, eaveColList, goldColList, BuildingsList[0].bodyController.eaveColumnHeight);
		//切割交界的脊與屋面
		CheckAllSurface();


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
	bool IsConnectCol(int buildingIndex,int connectBuildingIndex, int startColIndex)
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
	//buildingIndex亭交界連接的下一亭編號
	int FindConnectBuildingIndex(int fromBuildingIndex, int buildingIndex, int startColIndex)
	{
		Debug.Log("combinedInfos[:" + buildingIndex + " ].Info.Count:" + combinedInfos[buildingIndex].Info.Count);
		List<int> keyList = new List<int>();
		foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[buildingIndex].Info)
		{
			int key = kvp.Key;
			keyList.Add(key);
		}
		int maxKey=-1;
		for (int i = keyList.Count - 1; i >= 0; i--)
		{
			int key = keyList[i];
			if (combinedInfos[buildingIndex].Info[key].Contains(startColIndex))
			{
				//交一個亭
				if (combinedInfos[buildingIndex].colStates[startColIndex]==1)
				{
					if (key != fromBuildingIndex)
					{
						maxKey=key;
					}
				}
				//交兩個亭
				else if (combinedInfos[buildingIndex].colStates[startColIndex] == 2)
				{
					if (key != fromBuildingIndex && ((combinedInfos[key].count < combinedInfos[buildingIndex].count && buildingIndex < key) || combinedInfos[buildingIndex].count == BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count))
					{
						maxKey = key;
					}
				}
			}
		}
/*
		List<int> acuurateKeyList = new List<int>();
		for (int i = keyList.Count-1; i >=0; i--)
		{
			int key = keyList[i];
			//if (combinedInfos[key].parentIndex == buildingIndex )
			//檢查交於buildingIndex亭startColIndex的下一亭其交界點
			if (combinedInfos[buildingIndex].Info[key].Contains(startColIndex))
			{
				if (key != fromBuildingIndex && ((combinedInfos[key].count < combinedInfos[buildingIndex].count && buildingIndex < key) || combinedInfos[buildingIndex].count == BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count))
				{
					acuurateKeyList.Add(key);			
				}
			}
		}
		int maxKey=-1;
		for (int i = 0; i < acuurateKeyList.Count; i++)
		{
			if(maxKey<acuurateKeyList[i])maxKey=acuurateKeyList[i];
		}*/
		return maxKey;
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
			for (int j = BuildingsList.Count-1; j >=0; j--)
			{
				if (i == j) continue;
				BuildingObj RTing = BuildingsList[j];
				if (isNeedCombine(LTing, RTing))
				{
					Debug.Log(i + "&" + j + " Nedd to be combined");
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
			List<int> colStates=new List<int>();
			for(int j=0;j<BuildingsList[i].bodyController.eaveCornerColumnList.Count;j++)
			{
				colStates.Add(0);
			}
			foreach (KeyValuePair<int, List<int>> kvp in combinedInfos[i].Info)
			{
				List<int> value = kvp.Value;
				for(int j=0;j<value.Count;j++)
				{
					colStates[value[j]]++;
				}
			}
			combinedInfos[i].colStates = colStates;
			//檢查是否有cycle
			for(int j=0;j<combinedInfos[i].colStates.Count;j++)
			{
				if (combinedInfos[i].colStates[j] == 3)//為內部點
				{
					combinedInfos[i].count ++;
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
	void ColListRecursive_COPY(ref List<Vector3> eaveColList, ref List<Vector3> goldColList, int fromBuildingIndex, int buildingIndex, int startColIndex, bool counterclockwise = true)
	{

		//四邊形使用(長寬不同)
		Plane movePlan = new Plane();
		//多邊形使用(長寬相同)
		Ray ray = new Ray();
		//左右亭中心
		Vector3 LTingCenter = BuildingsList[buildingIndex].platformCenter;
		Vector3 RTingCenter = new Vector3();
		//左右亭交界平面法向量與距離
		Vector3 siftDir = new Vector3();
		float siftDis = 0;
		Plane midPlan = new Plane();
		AlignType mode = AlignType.RidgeAlign;
		//調整的檐柱與金柱位置
		Vector3 adjEavePos = Vector3.zero;
		Vector3 adjGoldPos = Vector3.zero;
		//檐柱位置
		List<Vector3> eaveColPosList = BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList);
		//金柱位置
		List<Vector3> golColPosList = BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.goldCornerColumnList);
		Debug.Log("****************************************************************************");
		if (combinedInfos[buildingIndex].count < BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count)
		{
			combinedInfos[buildingIndex].count++;
			Debug.Log("buildingIndex " + buildingIndex + " combinedInfos[buildingIndex].count " + combinedInfos[buildingIndex].count);
			Debug.Log("buildingIndex " + buildingIndex + " startColIndex " + startColIndex);

			eaveColList.Add(BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[startColIndex]);
			goldColList.Add(BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.goldCornerColumnList)[startColIndex]);
			//此變數用來檢測是否為該亭中最後一個點
			int RTingIndex = -1;
			//檢查該buildingIndex的startColIndex是否是交界點
			if (IsConnectCol(buildingIndex, startColIndex))
			{

				combinedInfos[buildingIndex].interIndexCount++;
				Debug.Log("IsConnectCol ");
				//交界的亭號
				int nextBuildingIndex = FindConnectBuildingIndex(fromBuildingIndex, buildingIndex, startColIndex);
				Debug.Log("fromBuildingIndex" + fromBuildingIndex + "nextBuildingIndex" + nextBuildingIndex);
				RTingIndex = nextBuildingIndex;
				//為該亭中最後一個點
				if (nextBuildingIndex == -1)
				{
					Debug.Log("Remove ");
					//先移除此點 等尋訪完此多邊形後再加入此點(先用pos變數記下)
					eaveColList.RemoveAt(eaveColList.Count - 1);
					goldColList.RemoveAt(goldColList.Count - 1);
					RTingIndex = fromBuildingIndex;
				}
				mode = (combinedInfos[buildingIndex].Info[RTingIndex].Count >= 2 && combinedInfos[RTingIndex].Info[buildingIndex].Count >= 2) ? AlignType.EdgeAlign : AlignType.RidgeAlign;
				RTingCenter = BuildingsList[RTingIndex].platformCenter;
				midPlan = new Plane((LTingCenter - RTingCenter).normalized, (LTingCenter + RTingCenter) / 2);

				/*	//第０個交點
					if (startColIndex == combinedInfos[buildingIndex].Info[RTingIndex][0])
						siftDir = (eaveColPosList[(startColIndex - 1 + eaveColPosList.Count) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;
					else//第１個交點
						siftDir = (eaveColPosList[(startColIndex + 1) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;*/

				Debug.Log("RTingIndex" + RTingIndex);

				List<Vector3> nextEaveColPosList = BuildingsList[RTingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[RTingIndex].bodyController.eaveCornerColumnList);
				List<Vector3> nextGolColPosList = BuildingsList[RTingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[RTingIndex].bodyController.goldCornerColumnList);
				int buildingIndex4Adjust = buildingIndex;
				if (buildingIndex == 0 && combinedInfos[buildingIndex].colStates[startColIndex] == 2 && combinedInfos[buildingIndex].count == 1)
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
							buildingIndex4Adjust = buildingIndexList[i];
						}
					}
					eaveColPosList = BuildingsList[buildingIndex4Adjust].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex4Adjust].bodyController.eaveCornerColumnList);

				}

				//換至下一亭
				if (nextBuildingIndex != -1)
				{
					#region AdjustPos
					//下一亭交buildingIndex的號碼

					int nextTingIntersectionColIndex = FindOtherSideCloserIndex(combinedInfos[RTingIndex].Info[buildingIndex], nextEaveColPosList, eaveColPosList[startColIndex]);
					if (counterclockwise)
					{
						if (IsConnectCol(buildingIndex, RTingIndex, (startColIndex + 1) % eaveColPosList.Count) || combinedInfos[buildingIndex].Info[RTingIndex].Count == 1)
							siftDir = (eaveColPosList[(startColIndex - 1 + eaveColPosList.Count) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;
						else
							siftDir = (eaveColPosList[(startColIndex + 1) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;
					}
					else
					{
						if (IsConnectCol(buildingIndex, RTingIndex, (startColIndex + 1) % eaveColPosList.Count) || combinedInfos[buildingIndex].Info[RTingIndex].Count == 1)
							siftDir = (eaveColPosList[(startColIndex + 1) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;

						else
							siftDir = (eaveColPosList[(startColIndex - 1 + eaveColPosList.Count) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;
					}

					//siftDir = (nextEaveColPosList[nextTingIntersectionColIndex] - eaveColPosList[startColIndex]).normalized;
					//調整位置 非為長寬不同之矩形platform和正規多邊形
					if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)//矩形
					{
						Debug.Log("Adjust ");
						//檐柱

						movePlan = new Plane(siftDir, nextEaveColPosList[nextTingIntersectionColIndex]);
						if (mode == AlignType.EdgeAlign) movePlan = midPlan;
						siftDis = movePlan.GetDistanceToPoint(eaveColPosList[startColIndex]);
						adjEavePos = eaveColPosList[startColIndex] - siftDis * siftDir;
						//金柱
						movePlan = new Plane(siftDir, nextGolColPosList[nextTingIntersectionColIndex]);
						if (mode == AlignType.EdgeAlign) movePlan = midPlan;
						siftDis = movePlan.GetDistanceToPoint(golColPosList[startColIndex]);
						adjGoldPos = golColPosList[startColIndex] - siftDis * siftDir;
					}
					else//正規多邊形
					{

						//檐柱
						ray = new Ray(eaveColPosList[startColIndex], siftDir);
						if (midPlan.Raycast(ray, out siftDis))
							adjEavePos = ray.GetPoint(siftDis);
						//金柱
						ray = new Ray(golColPosList[startColIndex], siftDir);
						if (midPlan.Raycast(ray, out siftDis))
							adjGoldPos = ray.GetPoint(siftDis);


					}

					#endregion

					eaveColList[eaveColList.Count - 1] = adjEavePos;
					goldColList[goldColList.Count - 1] = adjGoldPos;
					Debug.Log("Next " + nextBuildingIndex);
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
					//下一亭交buildingIndex的號碼
					int nextTingIntersectionColIndexZ = FindOtherSideCloserIndex(combinedInfos[nextBuildingIndex].Info[buildingIndex], nextEaveColPosList, BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[startColIndex]);
					ColListRecursive(ref eaveColList, ref goldColList, buildingIndex, nextBuildingIndex, nextTingIntersectionColIndexZ, counterclockwise);

				}

			}
			if (combinedInfos[buildingIndex].count == 1)
			{
				if (IsConnectCol(buildingIndex, (startColIndex + 1) % eaveColPosList.Count) && IsConnectCol(buildingIndex, (startColIndex) % eaveColPosList.Count))
				{
					counterclockwise = false;
				}
			}
			int nextColIndex = (counterclockwise) ? (startColIndex + 1) % BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count : (startColIndex - 1 + BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count) % BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count;
			ColListRecursive(ref eaveColList, ref goldColList, fromBuildingIndex, buildingIndex, nextColIndex, counterclockwise);
		}
	}

	void ColListRecursive(ref List<Vector3> eaveColList, ref List<Vector3> goldColList, int fromBuildingIndex, int buildingIndex, int startColIndex, bool counterclockwise = true)
	{

		//四邊形使用(長寬不同)
		Plane movePlan = new Plane();
		//多邊形使用(長寬相同)
		Ray ray = new Ray();
		//左右亭中心
		Vector3 LTingCenter = BuildingsList[buildingIndex].platformCenter;
		Vector3 RTingCenter = new Vector3();
		//左右亭交界平面法向量與距離
		Vector3 siftDir = new Vector3();
		float siftDis = 0;
		Plane midPlan = new Plane();
		AlignType mode = AlignType.RidgeAlign;
		//調整的檐柱與金柱位置
		Vector3 adjEavePos = Vector3.zero;
		Vector3 adjGoldPos = Vector3.zero;
		//檐柱位置
		List<Vector3> eaveColPosList = BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList);
		//金柱位置
		List<Vector3> golColPosList = BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.goldCornerColumnList);
		Debug.Log("****************************************************************************");
		if (combinedInfos[buildingIndex].count < BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count)
		{
			combinedInfos[buildingIndex].count++;
			Debug.Log("buildingIndex " + buildingIndex + " combinedInfos[buildingIndex].count " + combinedInfos[buildingIndex].count);
			Debug.Log("buildingIndex " + buildingIndex + " startColIndex " + startColIndex);

			eaveColList.Add(BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[startColIndex]);
			goldColList.Add(BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.goldCornerColumnList)[startColIndex]);
			//此變數用來檢測是否為該亭中最後一個點
			int RTingIndex = -1;
			//檢查該buildingIndex的startColIndex是否是交界點
			if (IsConnectCol(buildingIndex, startColIndex))
			{

				combinedInfos[buildingIndex].interIndexCount++;
				Debug.Log("IsConnectCol ");
				//交界的亭號
				int nextBuildingIndex = FindConnectBuildingIndex(fromBuildingIndex, buildingIndex, startColIndex);
				Debug.Log("fromBuildingIndex" + fromBuildingIndex + "nextBuildingIndex" + nextBuildingIndex);
				RTingIndex = nextBuildingIndex;
				//為該亭中最後一個點
				if (nextBuildingIndex == -1)
				{
					Debug.Log("Remove ");
					//先移除此點 等尋訪完此多邊形後再加入此點(先用pos變數記下)
					eaveColList.RemoveAt(eaveColList.Count - 1);
					goldColList.RemoveAt(goldColList.Count - 1);
					RTingIndex = fromBuildingIndex;
				}
				mode = (combinedInfos[buildingIndex].Info[RTingIndex].Count >= 2 && combinedInfos[RTingIndex].Info[buildingIndex].Count >= 2) ? AlignType.EdgeAlign : AlignType.RidgeAlign;
				RTingCenter = BuildingsList[RTingIndex].platformCenter;
				midPlan = new Plane((LTingCenter - RTingCenter).normalized, (LTingCenter + RTingCenter) / 2);

				/*	//第０個交點
					if (startColIndex == combinedInfos[buildingIndex].Info[RTingIndex][0])
						siftDir = (eaveColPosList[(startColIndex - 1 + eaveColPosList.Count) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;
					else//第１個交點
						siftDir = (eaveColPosList[(startColIndex + 1) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;*/

				Debug.Log("RTingIndex" + RTingIndex);

				List<Vector3> nextEaveColPosList = BuildingsList[RTingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[RTingIndex].bodyController.eaveCornerColumnList);
				List<Vector3> nextGolColPosList = BuildingsList[RTingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[RTingIndex].bodyController.goldCornerColumnList);
				int buildingIndex4Adjust = buildingIndex;
				if (buildingIndex == 0 && combinedInfos[buildingIndex].colStates[startColIndex] == 2 && combinedInfos[buildingIndex].count == 1)
				{
					eaveColList.RemoveAt(eaveColList.Count - 1);
					goldColList.RemoveAt(goldColList.Count - 1);
					//下一亭交buildingIndex的號碼
					int nextTingIntersectionColIndexZ = FindOtherSideCloserIndex(combinedInfos[nextBuildingIndex].Info[buildingIndex], nextEaveColPosList, BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[startColIndex]);
					ColListRecursive(ref eaveColList, ref goldColList, buildingIndex, nextBuildingIndex, nextTingIntersectionColIndexZ, counterclockwise);
					return;
				}

				//換至下一亭
				if (nextBuildingIndex != -1)
				{
					#region AdjustPos
					//下一亭交buildingIndex的號碼

					int nextTingIntersectionColIndex = FindOtherSideCloserIndex(combinedInfos[RTingIndex].Info[buildingIndex], nextEaveColPosList, eaveColPosList[startColIndex]);
					if (counterclockwise)
					{
						if (IsConnectCol(buildingIndex, RTingIndex, (startColIndex + 1) % eaveColPosList.Count) || combinedInfos[buildingIndex].Info[RTingIndex].Count == 1)
							siftDir = (eaveColPosList[(startColIndex - 1 + eaveColPosList.Count) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;
						else
							siftDir = (eaveColPosList[(startColIndex + 1) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;
					}
					else
					{
						if (IsConnectCol(buildingIndex, RTingIndex, (startColIndex + 1) % eaveColPosList.Count) || combinedInfos[buildingIndex].Info[RTingIndex].Count == 1)
							siftDir = (eaveColPosList[(startColIndex + 1) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;

						else
							siftDir = (eaveColPosList[(startColIndex - 1 + eaveColPosList.Count) % eaveColPosList.Count] - eaveColPosList[startColIndex]).normalized;
					}

					//siftDir = (nextEaveColPosList[nextTingIntersectionColIndex] - eaveColPosList[startColIndex]).normalized;
					//調整位置 非為長寬不同之矩形platform和正規多邊形
					if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)//矩形
					{
						Debug.Log("Adjust ");
						//檐柱

						movePlan = new Plane(siftDir, nextEaveColPosList[nextTingIntersectionColIndex]);
						if (mode == AlignType.EdgeAlign) movePlan = midPlan;
						siftDis = movePlan.GetDistanceToPoint(eaveColPosList[startColIndex]);
						adjEavePos = eaveColPosList[startColIndex] - siftDis * siftDir;
						//金柱
						movePlan = new Plane(siftDir, nextGolColPosList[nextTingIntersectionColIndex]);
						if (mode == AlignType.EdgeAlign) movePlan = midPlan;
						siftDis = movePlan.GetDistanceToPoint(golColPosList[startColIndex]);
						adjGoldPos = golColPosList[startColIndex] - siftDis * siftDir;
					}
					else//正規多邊形
					{

						//檐柱
						ray = new Ray(eaveColPosList[startColIndex], siftDir);
						if (midPlan.Raycast(ray, out siftDis))
							adjEavePos = ray.GetPoint(siftDis);
						//金柱
						ray = new Ray(golColPosList[startColIndex], siftDir);
						if (midPlan.Raycast(ray, out siftDis))
							adjGoldPos = ray.GetPoint(siftDis);


					}

					#endregion

					eaveColList[eaveColList.Count - 1] = adjEavePos;
					goldColList[goldColList.Count - 1] = adjGoldPos;
					Debug.Log("Next " + nextBuildingIndex);
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
					//下一亭交buildingIndex的號碼
					int nextTingIntersectionColIndexZ = FindOtherSideCloserIndex(combinedInfos[nextBuildingIndex].Info[buildingIndex], nextEaveColPosList, BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[startColIndex]);
					ColListRecursive(ref eaveColList, ref goldColList, buildingIndex, nextBuildingIndex, nextTingIntersectionColIndexZ, counterclockwise);

				}

			}
			if (combinedInfos[buildingIndex].count == 1)
			{
				if (IsConnectCol(buildingIndex, (startColIndex + 1) % eaveColPosList.Count) && IsConnectCol(buildingIndex, (startColIndex) % eaveColPosList.Count))
				{
					counterclockwise = false;
				}
			}
			int nextColIndex = (counterclockwise) ? (startColIndex + 1) % BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count : (startColIndex - 1 + BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count) % BuildingsList[buildingIndex].bodyController.eaveCornerColumnList.Count;
			ColListRecursive(ref eaveColList, ref goldColList, fromBuildingIndex, buildingIndex, nextColIndex, counterclockwise);
		}
	}
	//** 調整組合亭中的柱子列表，再創造出柱子位置(eaveColList與goldColList作為output)
	public void AdjustColPos(List<BuildingObj> buildingsList, ref List<Vector3> eaveColList, ref List<Vector3> goldColList)
	{
		if (buildingsList.Count < 2) return;
		//起始編號
		int startIndex = 0;
		ColListRecursive(ref eaveColList, ref goldColList, 0, 0, startIndex);
		if (Vector3.Dot(Vector3.Cross(eaveColList[1] - eaveColList[0], eaveColList[eaveColList.Count-1] - eaveColList[0]).normalized,Vector3.up)>0)
		{
			eaveColList.Reverse();
			goldColList.Reverse();
		}
	
	}
	//檢查亭與亭中交界點對應編號
	//tingColPos中交界的編號(intersectionIndexList)與pos最近的idex
	int FindOtherSideCloserIndex(List<int> intersectionIndexList, List<Vector3> tingColPos, Vector3 pos)
	{
		float RIntersectionMinDis = float.MaxValue;
		int index = 0;
		for (int i = 0; i < intersectionIndexList.Count; i++)
		{
			float dis = Vector3.Distance(pos, tingColPos[intersectionIndexList[i]]);
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
			int LTingSides= LTing.bodyController.eaveCornerColumnList.Count;
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

					for(int i=0;i<value.Count;i++)
					{

						//下一亭交buildingIndex的號碼
						int intersectionColIndex = FindOtherSideCloserIndex(combinedInfos[key].Info[buildingIndex], BuildingsList[key].bodyController.GetColumnStructBottomPosList(BuildingsList[key].bodyController.eaveCornerColumnList), BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[value[i]]);
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
						int intersectionColIndex = FindOtherSideCloserIndex(combinedInfos[key].Info[buildingIndex], BuildingsList[key].bodyController.GetColumnStructBottomPosList(BuildingsList[key].bodyController.eaveCornerColumnList), BuildingsList[buildingIndex].bodyController.GetColumnStructBottomPosList(BuildingsList[buildingIndex].bodyController.eaveCornerColumnList)[value[i]]);

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


}
