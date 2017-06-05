using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ColumnStruct
{
	public GameObject columnObj;
	public CylinderMesh columnMesh;
	public CylinderMesh fundation;
	public Vector3 topPos;
	public Vector3 bottomPos;
}

public class BodyController : MonoBehaviour
{
	private BuildingObj parentObj;
	//Body******************************************************************************
	const float CUN = 3.33f;
	public enum BodyType { Chuan_Dou = 0, Tai_Liang = 1 };//Chuan_Dou 穿斗式 ,Tai_Liang 抬梁式

	public BodyType bodyType = BodyType.Chuan_Dou;

	public float eaveColumnRatio2platformOffset;

	public float goldColumnRatio2platformOffset;

	public int goldColumnbayNumber = 5;//間數量
	public int eaveColumnbayNumber = 5;
	public int unitNumberInBay = 3;
	public int doorNumber =4;
	public float eaveColumnHeight;
	public float goldColumnHeight;
	public float eaveColumnRadius = 0.5f;
	public float goldColumnRadius = 0.5f;
	public float columnFundationHeight;//柱礎高度

	//***********************************************************************
	public List<ColumnStruct> eaveColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> goldColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> eaveCornerColumnList = new List<ColumnStruct>();
	//***********************************************************************
	public List<Vector3> GetColumnStructTopPosList(List<ColumnStruct> columnStructList)
	{
		List<Vector3> posList = new List<Vector3>();
		for (int i = 0; i < columnStructList.Count; i++)
		{
			posList.Add(columnStructList[i].topPos);
		}
		return posList;
	}
	public List<Vector3> GetColumnStructBottomPosList(List<ColumnStruct> columnStructList)
	{
		List<Vector3> posList = new List<Vector3>();
		for (int i = 0; i < columnStructList.Count; i++)
		{
			posList.Add(columnStructList[i].bottomPos);
		}
		return posList;
	}
	public List<Vector3> GetColumnStructPosList(List<ColumnStruct> columnStructList)
	{
		List<Vector3> posList = new List<Vector3>();
		for (int i = 0; i < columnStructList.Count; i++)
		{
			posList.Add((columnStructList[i].bottomPos + columnStructList[i].topPos) / 2.0f);
		}
		return posList;
	}
	public void InitFunction(BuildingObj parentObj, List<Vector3> bottomPosList, float platformFrontWidth, float platformHeight, float eaveColumnHeight, float goldColumnHeight)
	{
		//初始值******************************************************************************

		this.parentObj = parentObj;
		this.eaveColumnHeight = eaveColumnHeight;
		this.goldColumnHeight = goldColumnHeight;

		columnFundationHeight = eaveColumnHeight * 0.05f;

		eaveColumnRatio2platformOffset = platformFrontWidth * 0.1f;
		goldColumnRatio2platformOffset = platformFrontWidth * 0.2f;

		parentObj.bodyCenter = parentObj.platformCenter + (platformHeight / 2.0f + eaveColumnHeight / 2.0f) * Vector3.up;

		//**************************************************************************************
		switch (bodyType)
		{
			#region Chuan_Dou
			case BodyType.Chuan_Dou:
				CreateBody(bottomPosList, parentObj.entraneIndexList.List, parentObj.bodyCenter);
				if (goldColumnList.Count > 0)
				{
					CreateRingWall(ModelController.Instance.goldColumnModelStruct, GetColumnStructPosList(goldColumnList), goldColumnRadius, unitNumberInBay, doorNumber);
				}
				if (eaveColumnList.Count > 0)
				{
					CreateRingFrieze(ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.7f * eaveColumnHeight);
					CreateRingBalustrade(ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.1f * eaveColumnHeight);
				}
				break;
			#endregion
		}
	}
	//?????????????????????????????不用更新
	public void MoveValueUpdate(Vector3 offset)
	{
		for (int i = 0; i < eaveColumnList.Count; i++)
		{
			eaveColumnList[i].topPos += offset;
			eaveColumnList[i].bottomPos += offset;
		}
		for (int i = 0; i < goldColumnList.Count; i++)
		{
			goldColumnList[i].topPos += offset;
			goldColumnList[i].bottomPos += offset;
		}
	}

	private ColumnStruct CreateColumn(GameObject parentObj, Vector3 pos, float topRadius, float downRadius, float height, float fundationRadius, float fundationHeight, string name = "Column")
	{
		ColumnStruct columnStruct = new ColumnStruct();

		GameObject col = new GameObject(name);
		col.transform.parent = parentObj.transform;
		col.AddComponent<CylinderMesh>();

		Vector3 topPos = pos + (height / 2.0f) * Vector3.up;
		Vector3 bottomPos = pos - (height / 2.0f - fundationHeight) * Vector3.up;

		col.GetComponent<CylinderMesh>().CylinderInitSetting(pos, topPos, bottomPos, topRadius, downRadius);
		col.GetComponent<CylinderMesh>().SetMesh();
		columnStruct.columnObj = col;
		columnStruct.columnMesh = col.GetComponent<CylinderMesh>();
		columnStruct.topPos = topPos;
		columnStruct.bottomPos = pos - (height / 2.0f) * Vector3.up;
		//Fundation
		GameObject fun = new GameObject(name + "Fundation");
		//fun.transform.position = pos - new Vector3(0, height / 2.0f, 0);
		fun.transform.parent = col.transform;
		fun.AddComponent<CylinderMesh>();

		topPos = pos + (fundationHeight) * Vector3.up - (height / 2.0f) * Vector3.up;
		bottomPos = pos - (height / 2.0f) * Vector3.up;

		fun.GetComponent<CylinderMesh>().CylinderInitSetting(pos - new Vector3(0, height / 2.0f, 0), topPos, bottomPos, fundationRadius, fundationRadius);
		fun.GetComponent<CylinderMesh>().SetMesh();

		columnStruct.fundation = col.GetComponent<CylinderMesh>();

		return columnStruct;
	}
	private Vector3 mutiplyVector3(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}
	private List<ColumnStruct> CreateRingColumn(GameObject parentObj, List<Vector3> posList, float columnTopRadius, float columnDownRadius, float columnHeight, float fundationRadius, float fundationHeight, string columnName)
	{
		List<ColumnStruct> columnList = new List<ColumnStruct>();
		for (int i = 0; i < posList.Count; i++)
		{
			ColumnStruct newColumn = CreateColumn(parentObj, posList[i], columnTopRadius, columnDownRadius, columnHeight, fundationRadius, fundationHeight, columnName);
			columnList.Add(newColumn);
		}
		return columnList;
	}
	private void CreateBody(List<Vector3> posList, List<int> entranceIndexList, Vector3 bodyCenter)
	{
		List<Vector3> eaveColumnPosList = new List<Vector3>();
		List<Vector3> goldColumnPosList = new List<Vector3>();
		for (int i = 0; i < posList.Count; i++)
		{
			Vector2 v = new Vector2(posList[i].x - bodyCenter.x, posList[i].z - bodyCenter.z);
			//eaveColumn
			v = v.normalized * eaveColumnRatio2platformOffset;
			Vector3 eaveColumnPos = posList[i] - new Vector3(v.x, 0, v.y) + eaveColumnHeight / 2.0f * Vector3.up;
			eaveColumnPosList.Add(eaveColumnPos);

			//goldColumn
			v = new Vector2(posList[i].x - bodyCenter.x, posList[i].z - bodyCenter.z);
			v = v.normalized * goldColumnRatio2platformOffset;
			Vector3 goldColumnPos = posList[i] - new Vector3(v.x, 0, v.y) + goldColumnHeight / 2.0f * Vector3.up;
			goldColumnPosList.Add(goldColumnPos);

			if (!entranceIndexList.Contains(i))
			{
				//eaveBayColumn
				int nextIndex = (i + 1) % posList.Count;
				Vector2 vNext = new Vector2(posList[nextIndex].x - bodyCenter.x, posList[nextIndex].z - bodyCenter.z);
				vNext = vNext.normalized * eaveColumnRatio2platformOffset;
				Vector3 posNext = posList[nextIndex] - new Vector3(vNext.x, 0, vNext.y) + eaveColumnHeight / 2.0f * Vector3.up;

				float disBetweenEaveColumn = Vector3.Distance(eaveColumnPos, posNext);
				float bayWidth = disBetweenEaveColumn / eaveColumnbayNumber;
				Vector3 bayDir = posNext - eaveColumnPos;

				for (int j = 1; j < eaveColumnbayNumber; j++)
				{
					Vector3 eaveBayColumnPos = bayDir.normalized * (j * bayWidth) + eaveColumnPos;
					eaveColumnPosList.Add(eaveBayColumnPos);
				}
			}
			{
				//goldBayColumn
				int nextIndex = (i + 1) % posList.Count;
				Vector2 vNext = new Vector2(posList[nextIndex].x - bodyCenter.x, posList[nextIndex].z - bodyCenter.z);
				vNext = vNext.normalized * goldColumnRatio2platformOffset;
				Vector3 posNext = posList[nextIndex] - new Vector3(vNext.x, 0, vNext.y) + goldColumnHeight / 2.0f * Vector3.up;

				float disBetweenGoldColumn = Vector3.Distance(goldColumnPos, posNext);
				float bayWidth = disBetweenGoldColumn / goldColumnbayNumber;
				Vector3 bayDir = posNext - goldColumnPos;

				for (int j = 1; j < goldColumnbayNumber; j++)
				{
					Vector3 goldBayColumnPos = bayDir.normalized * (j * bayWidth) + goldColumnPos;
					goldColumnPosList.Add(goldBayColumnPos);
				}
			}
		}
		eaveColumnList = CreateRingColumn(parentObj.body, eaveColumnPosList, eaveColumnRadius, eaveColumnRadius, eaveColumnHeight, eaveColumnRadius * 1.2f, columnFundationHeight, "EaveColumn");


		goldColumnList = CreateRingColumn(parentObj.body, goldColumnPosList, goldColumnRadius, goldColumnRadius, goldColumnHeight, goldColumnRadius * 1.2f, columnFundationHeight, "GoldColumn");

		//角柱計算
		if (eaveColumnbayNumber <= 0) eaveColumnbayNumber = 1;
		for (int i = 0, count = 0; i < (int)MainController.Instance.sides; i++)
		{
			eaveCornerColumnList.Add(eaveColumnList[count]);
			if (entranceIndexList.Contains(i)) count++;
			else count += eaveColumnbayNumber;
		}

	}
	private void CreateRingWall(GoldColumnModelStruct goldColumnModelStruct, List<Vector3> columnList, float columnRadius, int bayNumber, int doorNumber)
	{
		float wallHeight = eaveColumnHeight;//牆長度
		float wallLengh = columnRadius * 2.0f;//牆深度

		float windowWidth = goldColumnModelStruct.windowModelStruct.bound.size.x;//裝飾物長度
		float windowHeight = goldColumnModelStruct.windowModelStruct.bound.size.y;//裝飾物長度
		float windowLengh = goldColumnModelStruct.windowModelStruct.bound.size.z;//裝飾物深度


		float doorWidth = goldColumnModelStruct.doorModelStruct.bound.size.x;//裝飾物長度
		float doorHeight = goldColumnModelStruct.doorModelStruct.bound.size.y;//裝飾物長度
		float doorLengh = goldColumnModelStruct.doorModelStruct.bound.size.z ;//裝飾物深度


		float windowWallWidth = goldColumnModelStruct.windowWallModelStruct.bound.size.x;//裝飾物長度
		float windowWallHeight = goldColumnModelStruct.windowWallModelStruct.bound.size.y;//裝飾物長度
		float windowWallLengh = goldColumnModelStruct.windowWallModelStruct.bound.size.z;//裝飾物深度

		float doorMidIndex = ((float)goldColumnbayNumber / 2);
		doorNumber = Mathf.Clamp(doorNumber,0 ,Mathf.CeilToInt(doorMidIndex));
		int doorMaxIndex = (int)((goldColumnbayNumber % 2 == 1) ? (doorMidIndex + (doorNumber - 1)) : (doorMidIndex + (doorNumber - 1)+0.5f));
		int doorMinIndex = (int)((goldColumnbayNumber % 2 == 1) ? (doorMidIndex - (doorNumber - 1)) : (doorMidIndex - (doorNumber - 1) - 0.5f));
		for (int i = 0; i < columnList.Count; i++)
		{
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
	
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			//門
			if (((i % (goldColumnbayNumber)) <= doorMaxIndex) && ((i % (goldColumnbayNumber) >= doorMinIndex)))
			{
				float width = dis;
				for (int j = 0; j < 1; j++)
				{
					float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i];
					float disDiff = doorWidth - width;
					float doorWidthScale = (width) / (doorWidth);
					float doorHeightScale = wallHeight / (doorHeight);
				//door
				GameObject clone = Instantiate(goldColumnModelStruct.doorModelStruct.model, pos, goldColumnModelStruct.doorModelStruct.model.transform.rotation) as GameObject;
				clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(goldColumnModelStruct.doorModelStruct.rotation);
				clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x * doorWidthScale, clone.transform.GetChild(0).localScale.y * doorHeightScale, (clone.transform.GetChild(0).localScale.z));
				//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, clone.transform.rotation * clone.transform.GetChild(0).transform.rotation * (new Vector3(doorWidthScale, doorHeightScale, 1)));
				clone.transform.parent = parentObj.body.transform;

				}

			}
			else//窗
			{
				float width = dis / bayNumber;
				for (int j = 0; j < bayNumber; j++)
				{
				#region windowWall
					float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i];
					float disDiff = windowWallWidth - width;
					float windowWallWidthScale = (width) / (windowWallWidth);
					float windowWallHeightScale = wallHeight / (windowWallHeight);
					//windowWall
					GameObject clone = Instantiate(goldColumnModelStruct.windowWallModelStruct.model, pos, goldColumnModelStruct.windowWallModelStruct.model.transform.rotation) as GameObject;
					clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(goldColumnModelStruct.windowWallModelStruct.rotation);
					clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x * windowWallWidthScale, clone.transform.GetChild(0).localScale.y * windowWallHeightScale, (clone.transform.GetChild(0).localScale.z));
	
					clone.transform.parent = parentObj.body.transform;

				#endregion
				#region windowInWall
				/*
					GameObject wall = new GameObject("Wall");
					MeshFilter meshFilter = wall.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = wall.AddComponent<MeshRenderer>();
					wall.transform.parent = parentObj.body.transform;
					meshRenderer.material.color = Color.white;
					float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i];
					//MeshCenter.Instance.CreateDoorMeshByRatio(pos, width, eaveColumnHeight, 1, 0.3f, 0.3f, 0.6f, 1.0f, rotateAngle, meshFilter);

					//創建牆 保留部分寬度
					float disDiff = ((width * 0.8f) > windowWidth) ? 0 : (windowWidth - (width * 0.8f));
					float windowWidthScale = (windowWidth - disDiff) / (windowWidth);
					float windowLengthScale = wallLengh/windowLengh;
					MeshCenter.Instance.CreateDoorMeshByUnit(pos, width, eaveColumnHeight, wallLengh, windowWidth * windowWidthScale, windowHeight, wallLengh, 0.5f, rotateAngle, meshFilter);
					//Window
					GameObject clone = Instantiate(goldColumnModelStruct.windowModelStruct.model, pos, goldColumnModelStruct.windowModelStruct.model.transform.rotation) as GameObject;
					clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(goldColumnModelStruct.windowModelStruct.rotation);

					clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * windowWidthScale);

					//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, clone.transform.rotation * clone.transform.GetChild(0).transform.rotation * (new Vector3(windowWidthScale, 1, windowLengthScale)));
					clone.transform.parent = wall.transform;*/
					#endregion
				}
			}
			//CreateWindowModel

		}
	}
	private void CreateRingFrieze(EaveColumnModelStruct eaveColumnModelStruct, List<Vector3> columnList, float columnRadius, float heightOffset)
	{
		float friezeWidth = eaveColumnModelStruct.friezeModelStruct.bound.size.x;//裝飾物長度
		float friezeHeight = eaveColumnModelStruct.friezeModelStruct.bound.size.y;//裝飾物長度
		float friezeLengh = eaveColumnModelStruct.friezeModelStruct.bound.size.z;//裝飾物深度
		for (int i = 0; i < columnList.Count; i++)
		{
			float width = friezeWidth;
			float height = friezeHeight;
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
			int number = Mathf.Max(Mathf.FloorToInt(dis / width), 1);
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			float disDiff = (dis - width * number) / number;
			width = dis / number;
			float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : - Vector3.Angle(dir, Vector3.right));
			for (int j = 0; j < number; j++)
			{
				Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + heightOffset * Vector3.up;
				GameObject clone = Instantiate(eaveColumnModelStruct.friezeModelStruct.model, pos, eaveColumnModelStruct.friezeModelStruct.model.transform.rotation) as GameObject;
				clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.friezeModelStruct.rotation);
				clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * (width) / friezeWidth);
				//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, clone.transform.rotation * clone.transform.GetChild(0).transform.rotation * (new Vector3((width) / friezeWidth, 1, 1)));

				//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, Quaternion.Euler(clone.transform.GetChild(0).transform.rotation.ToEulerAngles()) * (new Vector3((width) / friezeWidth, 1, 1)));
				clone.transform.parent = parentObj.body.transform;
			}

			//FriezeWall(frieze上方裝飾牆)
			GameObject friezeWall = new GameObject("FriezeWall");
			MeshFilter meshFilter = friezeWall.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = friezeWall.AddComponent<MeshRenderer>();
			friezeWall.transform.parent = parentObj.body.transform;
			meshRenderer.material.color = Color.yellow;
			float rotateAngleZ = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) :  - Vector3.Angle(dir, Vector3.right));
			float friezeWallHeight = (eaveColumnHeight - heightOffset - friezeHeight / 2.0f);
			Vector3 posZ = (columnList[i] + columnList[(i + 1) % columnList.Count]) / 2.0f + (heightOffset + friezeHeight / 2.0f + friezeWallHeight / 2.0f) * Vector3.up;
			MeshCenter.Instance.CreateCubeMesh(posZ, dis, friezeWallHeight, 0.5f, rotateAngleZ, meshFilter);

			if (dis >= eaveColumnModelStruct.sparrowBraceModelStruct.bound.size.x * 2.5f)
			{
				//sparrowBrace雀替
				Vector3 posX = dir.normalized * (columnRadius) + columnList[i] + (heightOffset - friezeHeight / 2.0f) * Vector3.up;
				float rotateAngleX = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
				GameObject sparrowBrace = Instantiate(eaveColumnModelStruct.sparrowBraceModelStruct.model, posX, eaveColumnModelStruct.sparrowBraceModelStruct.model.transform.rotation) as GameObject;
				sparrowBrace.transform.rotation = Quaternion.AngleAxis(rotateAngleX, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.sparrowBraceModelStruct.rotation);

				sparrowBrace.transform.parent = parentObj.body.transform;

				posX = -dir.normalized * (columnRadius) + columnList[(i + 1) % columnList.Count] + (heightOffset - friezeHeight / 2.0f) * Vector3.up;
				rotateAngleX = (Vector3.Dot(Vector3.forward, -dir) < 0 ? Vector3.Angle(-dir, Vector3.right) : -Vector3.Angle(-dir, Vector3.right));
				sparrowBrace = Instantiate(eaveColumnModelStruct.sparrowBraceModelStruct.model, posX, eaveColumnModelStruct.sparrowBraceModelStruct.model.transform.rotation) as GameObject;
				sparrowBrace.transform.rotation = Quaternion.AngleAxis(rotateAngleX, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.sparrowBraceModelStruct.rotation);

				sparrowBrace.transform.parent = parentObj.body.transform;
			}
		}

	}
	private void CreateRingBalustrade(EaveColumnModelStruct eaveColumnModelStruct, List<Vector3> columnList, float columnRadius, float heightOffset)
	{
		float balustradeWidth = eaveColumnModelStruct.balustradeModelStruct.bound.size.x;//欄杆長度
		float balustradeHeight = eaveColumnModelStruct.balustradeModelStruct.bound.size.y;//欄杆長度
		float balustradeLengh = eaveColumnModelStruct.balustradeModelStruct.bound.size.z;//欄杆深度

		for (int n = 0, i = 0; n < (int)MainController.Instance.sides; n++)
		{
			if (parentObj.entraneIndexList.Contains(n)) { i++; continue; }

			for (int k = 0; k < eaveColumnbayNumber; i++, k++)
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
					//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, Quaternion.Euler(clone.transform.GetChild(0).transform.rotation.ToEulerAngles()) * (new Vector3((width) / balustradeWidth, 1, 1)));
					clone.transform.parent = parentObj.body.transform;
				}
			}
		}
	}
}
