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

	public int goldColumnbayNumber = 3;//間數量
	public int eaveColumnbayNumber = 5;
	public float eaveColumnHeight;
	public float goldColumnHeight;
	public float eaveColumnRadius = 1f;
	public float goldColumnRadius = 1f;
	public float columnFundationHeight;//柱礎高度
	public float columnFundationRadius;//柱礎半徑
	//**********************************************************************************
	public float friezeWidth;//裝飾物長度
	public float balustradeWidth;//欄杆長度
	//***********************************************************************
	public List<ColumnStruct> eaveColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> goldColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> eaveCornerColumnList = new List<ColumnStruct>();
	//***********************************************************************

	public void InitFunction(BuildingObj parentObj, List<Vector3> bottomPosList,float platformFrontWidth, float platformHeight)
	{
		//初始值******************************************************************************

		this.parentObj = parentObj;
		eaveColumnHeight = eaveColumnRadius * 11;
		goldColumnHeight = eaveColumnRadius * 11;

		columnFundationHeight = eaveColumnHeight * 0.05f;
		columnFundationRadius = eaveColumnRadius*1.2f;

		eaveColumnRatio2platformOffset = (platformFrontWidth * 0.1f);
		goldColumnRatio2platformOffset = eaveColumnRatio2platformOffset * 2.5f;

		parentObj.bodyCenter = parentObj.platformCenter + new Vector3(0, platformHeight / 2.0f + eaveColumnHeight / 2.0f, 0);

		//**************************************************************************************
		switch (bodyType)
		{
			#region Chuan_Dou
			case BodyType.Chuan_Dou:
				CreateBody(bottomPosList, parentObj.bodyCenter);
				if(goldColumnList.Count>0)
				{
					CreateRingWall(GetColumnStructPosList(goldColumnList), goldColumnRadius, goldColumnbayNumber);
				}
				if (eaveColumnList.Count > 0) 
				{
					CreateRingFrieze(ModelController.Instance, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.8f * eaveColumnHeight);
					CreateRingBalustrade(ModelController.Instance, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, -0.8f * eaveColumnHeight);
				}
				break;
			#endregion
		}
	}
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
			posList.Add((columnStructList[i].bottomPos + columnStructList[i].topPos)/2.0f);
		}
		return posList;
	}
    public void UpdateFunction()
	{

	}
	private ColumnStruct CreateColumn(GameObject parentObj, Vector3 pos, float topRadius, float downRadius, float height, float fundationRadius, float fundationHeight, string name = "Column")
	{
		ColumnStruct columnStruct=new ColumnStruct();

		float columnRemainHeight = height - columnFundationHeight;

		GameObject col = new GameObject(name);
		//col.transform.position = pos + new Vector3(0, columnRemainHeight / 2.0f + columnFundationHeight, 0);
		col.transform.parent = parentObj.transform;
		col.AddComponent<CylinderMesh>();

		Vector3 topPos = pos + (columnRemainHeight / 2.0f + columnFundationHeight)*Vector3.up;
		Vector3 bottomPos = pos - (columnRemainHeight / 2.0f + columnFundationHeight)*Vector3.up;

		col.GetComponent<CylinderMesh>().CylinderInitSetting(pos, topPos, bottomPos, topRadius, downRadius);
		col.GetComponent<CylinderMesh>().SetMesh();
		columnStruct.columnObj=col;
		columnStruct.columnMesh=col.GetComponent<CylinderMesh>();
		columnStruct.topPos=topPos;
		columnStruct.bottomPos=bottomPos;
		//Fundation
		GameObject fun = new GameObject(name + "Fundation");
		//fun.transform.position = pos - new Vector3(0, height / 2.0f, 0);
		fun.transform.parent = col.transform;
		fun.AddComponent<CylinderMesh>();

		topPos = pos + (columnFundationHeight / 2.0f) * Vector3.up - (height / 2.0f)*Vector3.up;
		bottomPos = pos - (columnFundationHeight / 2.0f) * Vector3.up - (height / 2.0f) * Vector3.up;

		fun.GetComponent<CylinderMesh>().CylinderInitSetting(pos - new Vector3(0, height / 2.0f, 0), topPos, bottomPos, fundationRadius, fundationRadius);
		fun.GetComponent<CylinderMesh>().SetMesh();

		columnStruct.fundation = col.GetComponent<CylinderMesh>();

		return columnStruct;
	}
	public Vector3 mutiplyVector3(Vector3 a,Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}
	public List<ColumnStruct> CreateRingColumn(GameObject parentObj, List<Vector3> posList, float columnTopRadius, float columnDownRadius, float columnHeight,float fundationRadius,float fundationHeight, string columnName) 
	{
		List<ColumnStruct> columnList = new List<ColumnStruct>();
		for (int i = 0; i < posList.Count; i++)
		{
			ColumnStruct newColumn = CreateColumn(parentObj, posList[i], columnTopRadius, columnDownRadius, columnHeight, fundationRadius,fundationHeight,columnName);
			columnList.Add(newColumn);
		}
		return columnList;
	}
	public void CreateBody(List<Vector3> posList,Vector3 bodyCenter)
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

			//goldColumn
			v = new Vector2(posList[i].x - bodyCenter.x, posList[i].z - bodyCenter.z);
			v = v.normalized * goldColumnRatio2platformOffset;
			Vector3 goldColumnPos = posList[i] - new Vector3(v.x, 0, v.y) + goldColumnHeight/2.0f * Vector3.up;
			goldColumnPosList.Add(goldColumnPos);
		}
		eaveColumnList = CreateRingColumn(parentObj.body, eaveColumnPosList, eaveColumnRadius, eaveColumnRadius, eaveColumnHeight, columnFundationRadius, columnFundationHeight, "EaveColumn");

		goldColumnList = CreateRingColumn(parentObj.body, goldColumnPosList, goldColumnRadius, goldColumnRadius, goldColumnHeight, columnFundationRadius, columnFundationHeight, "GoldColumn");

		if(eaveColumnbayNumber<=0)eaveColumnbayNumber=1;
		for(int i=0;i<(int)MainController.Instance.sides;i++)
		{
			eaveCornerColumnList.Add(eaveColumnList[i * eaveColumnbayNumber]);
		}
		
	}
	public void CreateRingWall(List<Vector3> columnList,float columnRadius,int bayNumber)
	{
		for (int i = 0; i < columnList.Count; i++)
		{
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
			float width = dis / bayNumber;
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			for (int j = 0; j < bayNumber; j++)
			{
				GameObject wall = new GameObject("Wall");
				MeshFilter meshFilter = wall.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = wall.AddComponent<MeshRenderer>();
				wall.transform.parent = parentObj.body.transform;
				meshRenderer.material.color = Color.white;
				float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : 180 - Vector3.Angle(dir, Vector3.right));
				Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i];
                MeshCenter.Instance.CreateTorusMesh(pos, width, eaveColumnHeight, 1, 0.3f, 0.3f, 0.6f, 1.0f, rotateAngle, meshFilter);
			}

			//CreateWindowModel

		}
	}
	public void CreateRingFrieze(ModelController modelController, List<Vector3> columnList, float columnRadius, float heightOffset)
	{

		for (int i = 0; i < columnList.Count; i++)
		{
			float width = friezeWidth;
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
			float number = Mathf.FloorToInt(dis / width);
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			float disDiff = (dis - width * number) / number;
			width = dis / number;
			float rotateAngle = (Vector3.Dot(Vector3.right, dir) > 0 ? Vector3.Angle(dir, Vector3.forward) : 180 - Vector3.Angle(dir, Vector3.forward));
			for (int j = 0; j < number; j++)
			{
				Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + heightOffset * Vector3.up;
				GameObject clone = Instantiate(modelController.eaveColumnModelStruct.friezeModelStruct.model, pos, modelController.eaveColumnModelStruct.friezeModelStruct.model.transform.rotation) as GameObject;
				clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(modelController.eaveColumnModelStruct.friezeModelStruct.rotation);

				clone.transform.GetChild(0).localScale = new Vector3(modelController.eaveColumnModelStruct.friezeModelStruct.scale.x, modelController.eaveColumnModelStruct.friezeModelStruct.scale.y, (modelController.eaveColumnModelStruct.friezeModelStruct.scale.z) * (width + disDiff) / friezeWidth);
			
				clone.transform.parent = parentObj.body.transform;
			}
		}
	}
	public void CreateRingBalustrade(ModelController modelController, List<Vector3> columnList, float columnRadius, float heightOffset)
	{
		for (int i = 0; i < columnList.Count; i++)
		{
			float width = balustradeWidth;
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
			float number = Mathf.FloorToInt(dis / width);
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			float disDiff = (dis - width * number) / number;
			width = dis / number;

			float rotateAngle = (Vector3.Dot(Vector3.right, dir) > 0 ? Vector3.Angle(dir, Vector3.forward) : 180-Vector3.Angle(dir, Vector3.forward));
			for (int j = 0; j < number; j++)
			{
				Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + heightOffset * Vector3.up;
				GameObject clone = Instantiate(modelController.eaveColumnModelStruct.balustradeModelStruct.model, pos, modelController.eaveColumnModelStruct.balustradeModelStruct.model.transform.rotation) as GameObject;
				clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(modelController.eaveColumnModelStruct.balustradeModelStruct.rotation);
				clone.transform.GetChild(0).localScale = new Vector3(modelController.eaveColumnModelStruct.balustradeModelStruct.scale.x, modelController.eaveColumnModelStruct.balustradeModelStruct.scale.y, (modelController.eaveColumnModelStruct.balustradeModelStruct.scale.z) * (width + disDiff) / balustradeWidth);

				clone.transform.parent = parentObj.body.transform;
			}
		}
	}
}
