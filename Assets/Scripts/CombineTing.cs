using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
public class CombineTing : MonoBehaviour
{

	public float Center;
	public List<Vector3> ColumnList;
	public enum AlignType { EdgeAlign, RidgeAlign };
	public AlignType alignType = AlignType.EdgeAlign;

	public BuildingObj mainBuilding;
	public BuildingObj subBuilding;
	/** 
	 * 條整柱子位置並存放至ColumnList中
	 * 流程:先將兩亭的柱子列表內縮至適當位置，算出需移動柱子的方向向量
	 *      ，由此向量與另一亭的同邊柱子位置做一個平面並計算需位移的距離
	 *      兩亭皆完成此步驟後將所有點加入新的列表中，並調整 List 順序
	 */
	public void AdjustColPos(BuildingObj LTing, BuildingObj RTing, Vector3 LTingCenter, Vector3 RTingCenter)
	{
		int LTingIntersectionColIndex = 0;
		int RTingIntersectionColIndex = 0;
		List<Vector3> newLTingColPos = LTing.bodyController.GetColumnStructTopPosList(LTing.bodyController.eaveCornerColumnList);
		List<Vector3> newRTingColPos = RTing.bodyController.GetColumnStructTopPosList(RTing.bodyController.eaveCornerColumnList);
		Plane midPlan = new Plane((LTingCenter - RTingCenter).normalized, (LTingCenter + RTingCenter) / 2);
		switch (alignType)
		{
			//脊對齊(一組交界點)
			case AlignType.RidgeAlign:
				#region RidgeAlign
				//交界點
				LTingIntersectionColIndex = FindOtherSideIndex(newLTingColPos, RTingCenter, midPlan)[0];
				RTingIntersectionColIndex = FindOtherSideIndex(newRTingColPos, LTingCenter, midPlan)[0];
				Vector3 LTingSiftDir = (newLTingColPos[(LTingIntersectionColIndex - 1 + newLTingColPos.Count) % newLTingColPos.Count] - newLTingColPos[LTingIntersectionColIndex]).normalized;
				//Vector3 RTingSiftDir = -LTingSiftDir;
				Vector3 RTingSiftDir = (newRTingColPos[(RTingIntersectionColIndex - 1 + newLTingColPos.Count) % newRTingColPos.Count] - newRTingColPos[RTingIntersectionColIndex]).normalized;
				//四邊形使用(長寬不同)
				Plane LTingMovePlan = new Plane(LTingSiftDir, newRTingColPos[RTingIntersectionColIndex]);
				Plane RTingMovePlan = new Plane(RTingSiftDir, newLTingColPos[LTingIntersectionColIndex]);
				float LTingSiftDis = LTingMovePlan.GetDistanceToPoint(newLTingColPos[LTingIntersectionColIndex]);
				float RTingSiftDis = RTingMovePlan.GetDistanceToPoint(newRTingColPos[RTingIntersectionColIndex]);
				//多邊形使用(長寬相同)
				Ray LRay = new Ray(newLTingColPos[LTingIntersectionColIndex], LTingSiftDir);
				Ray RRay = new Ray(newRTingColPos[RTingIntersectionColIndex], RTingSiftDir);
				ColumnList = new List<Vector3>();
				//放入0至LTingIntersectionColIndex到左亭
				for (int iIndex = 0; iIndex <= LTingIntersectionColIndex; iIndex++)
				{
					ColumnList.Add(newLTingColPos[iIndex] - LTing.bodyController.eaveColumnHeight * Vector3.up);
					if (iIndex == LTingIntersectionColIndex)
					{
						if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
						{
							ColumnList[ColumnList.Count - 1] = ColumnList[ColumnList.Count - 1] - LTingSiftDis * LTingSiftDir;
						}
						else
						{
							if (midPlan.Raycast(LRay, out LTingSiftDis))
								ColumnList[ColumnList.Count - 1] = LRay.GetPoint(LTingSiftDis) - LTing.bodyController.eaveColumnHeight * Vector3.up;
						}
					}
				}
				//放入所有右亭
				for (int iIndex = 0; iIndex < newRTingColPos.Count; iIndex++)
				{
					int RIndex = (iIndex + RTingIntersectionColIndex + 1) % newRTingColPos.Count;
					ColumnList.Add(newRTingColPos[RIndex] - RTing.bodyController.eaveColumnHeight * Vector3.up);
					if (RIndex == RTingIntersectionColIndex)
					{
						if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
						{
							ColumnList[ColumnList.Count - 1] = ColumnList[ColumnList.Count - 1] - RTingSiftDis * RTingSiftDir;
						}
						else
						{
							if (midPlan.Raycast(RRay, out RTingSiftDis))
								ColumnList[ColumnList.Count - 1] = RRay.GetPoint(RTingSiftDis) - RTing.bodyController.eaveColumnHeight * Vector3.up;
						}
					}
				}
				//放入LTingIntersectionColIndex至 newRTingColPos.Count-1到左亭
				for (int iIndex = LTingIntersectionColIndex + 1; iIndex < newLTingColPos.Count; iIndex++)
				{
					ColumnList.Add(newLTingColPos[iIndex] - LTing.bodyController.eaveColumnHeight * Vector3.up);
				}
				#endregion
				break;
			//邊對齊(兩組交界點)
			case AlignType.EdgeAlign:
				#region EdgeAlign
				//第一組交界點
				LTingIntersectionColIndex = FindOtherSideIndex(newLTingColPos, RTingCenter, midPlan)[0];
				Debug.Log("First LTingIntersectionColIndex" + LTingIntersectionColIndex);
				List<int> RIntersectionIndexList = FindOtherSideIndex(newRTingColPos, LTingCenter, midPlan);

				RTingIntersectionColIndex = FindOtherSideCloserIndex(RIntersectionIndexList, newRTingColPos, newLTingColPos[LTingIntersectionColIndex]);

				LTingSiftDir = (newLTingColPos[(LTingIntersectionColIndex - 1 + newLTingColPos.Count) % newLTingColPos.Count] - newLTingColPos[LTingIntersectionColIndex]).normalized;
				//多邊形使用(長寬相同)
				LRay = new Ray(newLTingColPos[LTingIntersectionColIndex], LTingSiftDir);

				Debug.Log("First RTingIntersectionColIndex" + RTingIntersectionColIndex);
				ColumnList = new List<Vector3>();
				//放入0至LTingIntersectionColIndex到左亭
				for (int iIndex = 0; iIndex <= LTingIntersectionColIndex; iIndex++)
				{
					ColumnList.Add(newLTingColPos[iIndex] - LTing.bodyController.eaveColumnHeight * Vector3.up);
					if (iIndex == LTingIntersectionColIndex)
					{
						if (midPlan.Raycast(LRay, out LTingSiftDis))
							ColumnList[ColumnList.Count - 1] = LRay.GetPoint(LTingSiftDis) - LTing.bodyController.eaveColumnHeight * Vector3.up;
					}
				}

				//第二組交界點
				LTingIntersectionColIndex = FindOtherSideIndex(newLTingColPos, RTingCenter, midPlan)[1];
				Debug.Log("Second RTingIntersectionColIndex" + RTingIntersectionColIndex);
				LTingSiftDir = (newLTingColPos[(LTingIntersectionColIndex + 1) % newLTingColPos.Count] - newLTingColPos[LTingIntersectionColIndex]).normalized;
				//多邊形使用(長寬相同)
				LRay = new Ray(newLTingColPos[LTingIntersectionColIndex], LTingSiftDir);
				//放入所有右亭
				for (int iIndex = 1; iIndex < newRTingColPos.Count; iIndex++)
				{
					int RIndex = (iIndex + RTingIntersectionColIndex) % newRTingColPos.Count;
					ColumnList.Add(newRTingColPos[RIndex] - RTing.bodyController.eaveColumnHeight * Vector3.up);
					Debug.Log("RIndex" + RIndex);
					if (iIndex == newRTingColPos.Count - 1)
					{
						if (midPlan.Raycast(LRay, out LTingSiftDis))
							ColumnList[ColumnList.Count - 1] = LRay.GetPoint(LTingSiftDis) - RTing.bodyController.eaveColumnHeight * Vector3.up;
					}
				}
				//放入LTingIntersectionColIndex至 newRTingColPos.Count-1到左亭
				for (int iIndex = LTingIntersectionColIndex + 1; iIndex < newLTingColPos.Count; iIndex++)
				{
					ColumnList.Add(newLTingColPos[iIndex] - LTing.bodyController.eaveColumnHeight * Vector3.up);
				}
				#endregion
				break;


		}

	}
	//檢查亭亭與亭中交界點對應編號
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
	 * 交換 ColumnList 中兩指定位置內容 (需先用過adjustColPos)
	 */
	private void switchColumnListPos(int firstPos, int secondPos)
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
	/**
	 * 建立柱子群 (從BodyController搬來的，發現那個還是不好用)
	 */
	public List<ColumnStruct> CreateRingColumn(GameObject parentObj, float columnTopRadius, float columnDownRadius, float columnHeight, string columnName)
	{
		List<ColumnStruct> columnList = new List<ColumnStruct>();
		for (int i = 0; i < ColumnList.Count; i++)
		{
			ColumnStruct newColumn = CreateColumn(parentObj, ColumnList[i], columnTopRadius, columnDownRadius, columnHeight, columnDownRadius * 1.2f, columnHeight * 0.05f, columnName);
		}
		return columnList;
	}
	/**
	 * 建立柱子 (從BodyController搬來的，發現那個還是不好用)
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
     * 建造整門楣
     */
	public void CreateRingFrieze(ModelController modelController, float columnRadius, float heightOffset, float eaveColumnHeight, GameObject parent)
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
	 * 建立柵欄 (從BodyController搬來的，發現那個還是不好用)
	 */
	public void CreateRingBalustrade(ModelController modelController, float columnRadius, float heightOffset, GameObject parent)
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
	public void CheckAllSurface(BuildingObj LTing, BuildingObj RTing, Vector3 LTingCenter, Vector3 RTingCenter)
	{
		List<Vector3> newLTingColPos = LTing.bodyController.GetColumnStructTopPosList(LTing.bodyController.eaveCornerColumnList);
		List<Vector3> newRTingColPos = RTing.bodyController.GetColumnStructTopPosList(RTing.bodyController.eaveCornerColumnList);

		Plane midPlan = new Plane((LTingCenter - RTingCenter).normalized, (LTingCenter + RTingCenter) / 2);//* 兩亭中間切平面
		int LTingIntersectionColIndex = FindOtherSideIndex(newLTingColPos, RTingCenter, midPlan)[0];
		int RTingIntersectionColIndex = FindOtherSideIndex(newRTingColPos, LTingCenter, midPlan)[0];
		Debug.Log("LTingIntersectionColIndex " + LTingIntersectionColIndex);
		Debug.Log("RTingIntersectionColIndex " + RTingIntersectionColIndex);
		switch (alignType)
		{
			//脊對齊(一組交界點)
			case AlignType.RidgeAlign:
				#region RidgeAlign
				switch (MainController.Instance.roofType)
				{
					case MainController.RoofType.Zan_Jian_Ding:
						//一條脊相接
						CheckMainRidge(LTing, LTingIntersectionColIndex, LTingCenter, RTingCenter);
						
						CheckMainRidge(RTing, RTingIntersectionColIndex, LTingCenter, RTingCenter);
						
						CheckSurface(LTing, (LTingIntersectionColIndex - 1 + newLTingColPos.Count) % newLTingColPos.Count, LTingCenter, RTingCenter);
						CheckSurface(LTing, LTingIntersectionColIndex, LTingCenter, RTingCenter);
						
						CheckSurface(RTing, (RTingIntersectionColIndex - 1 + newRTingColPos.Count) % newRTingColPos.Count, LTingCenter, RTingCenter);
						CheckSurface(RTing, RTingIntersectionColIndex, LTingCenter, RTingCenter);
						break;

				}
				break;
				#endregion
			//邊對齊(兩組交界點)
			case AlignType.EdgeAlign:
				#region EdgeAlign

				switch (MainController.Instance.roofType)
				{
					case MainController.RoofType.Zan_Jian_Ding:
					case MainController.RoofType.Lu_Ding:
					case MainController.RoofType.Wu_Dian_Ding:
						//兩條脊相接
						CheckMainRidge(LTing, LTingIntersectionColIndex, LTingCenter, RTingCenter);
						CheckMainRidge(LTing, (LTingIntersectionColIndex + 1) % newLTingColPos.Count, LTingCenter, RTingCenter);
						
						CheckMainRidge(RTing, RTingIntersectionColIndex, LTingCenter, RTingCenter);
						CheckMainRidge(RTing, (RTingIntersectionColIndex + 1) % newRTingColPos.Count, LTingCenter, RTingCenter);
						
						CheckSurface(LTing, (LTingIntersectionColIndex - 1 + newLTingColPos.Count) % newLTingColPos.Count, LTingCenter, RTingCenter);
						CheckSurface(LTing, LTingIntersectionColIndex, LTingCenter, RTingCenter);
						CheckSurface(LTing, (LTingIntersectionColIndex + 1) % newLTingColPos.Count, LTingCenter, RTingCenter);
						
						CheckSurface(RTing, (RTingIntersectionColIndex - 1 + newRTingColPos.Count) % newRTingColPos.Count, LTingCenter, RTingCenter);
						CheckSurface(RTing, RTingIntersectionColIndex, LTingCenter, RTingCenter);
						CheckSurface(RTing, (RTingIntersectionColIndex + 1) % newRTingColPos.Count, LTingCenter, RTingCenter);
						break;
						case MainController.RoofType.Shya_Shan_Ding:

						CheckMainRidge(LTing, LTingIntersectionColIndex * 2, LTingCenter, RTingCenter);
						MainController.ShowPos(LTing.roofController.MainRidgeList[LTingIntersectionColIndex * 2].controlPointDictionaryList["MidControlPoint"], this.gameObject, Color.blue, 3);
						CheckMainRidge(LTing, LTingIntersectionColIndex * 2+1, LTingCenter, RTingCenter);
						MainController.ShowPos(LTing.roofController.MainRidgeList[LTingIntersectionColIndex * 2+1].controlPointDictionaryList["MidControlPoint"], this.gameObject, Color.red, 3);
						CheckMainRidge(LTing, ((LTingIntersectionColIndex + 1) % newLTingColPos.Count) * 2, LTingCenter, RTingCenter);
						MainController.ShowPos(LTing.roofController.MainRidgeList[((LTingIntersectionColIndex + 1) % newLTingColPos.Count) * 2].controlPointDictionaryList["MidControlPoint"], this.gameObject, Color.yellow, 3);
						CheckMainRidge(LTing, ((LTingIntersectionColIndex + 1) % newLTingColPos.Count) * 2 + 1, LTingCenter, RTingCenter);

						CheckMainRidge(RTing, RTingIntersectionColIndex * 2, LTingCenter, RTingCenter);
						CheckMainRidge(RTing, RTingIntersectionColIndex * 2+1, LTingCenter, RTingCenter);
						CheckMainRidge(RTing, ((RTingIntersectionColIndex + 1) % newRTingColPos.Count) * 2, LTingCenter, RTingCenter);
						CheckMainRidge(RTing, ((RTingIntersectionColIndex + 1) % newRTingColPos.Count) * 2 + 1, LTingCenter, RTingCenter);

						CheckSurface(LTing, (LTingIntersectionColIndex - 1 + newLTingColPos.Count) % newLTingColPos.Count, LTingCenter, RTingCenter);
						CheckSurface(LTing, LTingIntersectionColIndex, LTingCenter, RTingCenter);
						CheckSurface(LTing, (LTingIntersectionColIndex + 1) % newLTingColPos.Count, LTingCenter, RTingCenter);
						
						CheckSurface(RTing, (RTingIntersectionColIndex - 1 + newRTingColPos.Count) % newRTingColPos.Count, LTingCenter, RTingCenter);
						CheckSurface(RTing, RTingIntersectionColIndex, LTingCenter, RTingCenter);
						CheckSurface(RTing, (RTingIntersectionColIndex + 1) % newRTingColPos.Count, LTingCenter, RTingCenter);
						break;
						case MainController.RoofType.Dome:
						CheckSurface(LTing,0, LTingCenter, RTingCenter);
						CheckSurface(RTing, 0, LTingCenter, RTingCenter);
						break;


				}
				#endregion
				break;
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
		if (surface.midRoofSurfaceTileRidge!=null) RemoveSurfacePosByPlane(midPlane, surface.midRoofSurfaceTileRidge, Ting.roofTopCenter);
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
