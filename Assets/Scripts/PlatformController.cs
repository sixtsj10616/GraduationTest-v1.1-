﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlatFormStruct
{
	public List<Vector3> facadeDir = new List<Vector3>();
	public List<Vector3> topPointPosList = new List<Vector3>();
	public List<Vector3> bottomPointPosList = new List<Vector3>();
	public List<Vector3> stairPosList = new List<Vector3>();

	public List<GameObject> stairList = new List<GameObject>();
	public List<GameObject> borderColumnList = new List<GameObject>();
	public List<GameObject> borderWallList = new List<GameObject>();
}
public class PlatformController : MonoBehaviour
{

	public BuildingObj parentObj = null;
	//Platform**************************************************************************
	public enum PlatformType { };
	public float platWidth;
	public float platLength;
	public float platHeight;

	//Stair*****************************************************************************
	public float stairWidth=5;
	public float stairHeight;
	public float stairLength = 5;
	//**********************************************************************************
	public bool isCurvePlatform = false;
	public bool isStair = false;
	public bool isBorder = true;
	//**********************************************************************************
	public int borderColumnNumber = 8;//必須是偶數
	//**********************************************************************************
	public PlatFormStruct platFormStruct;

	/**
	 * 初始化基座
	 */
	public void InitFunction(BuildingObj parentObj, Vector3 platformCenter, float platformFrontWidth, float platformFrontLength, float platformHeight, bool isStair)
	{
		this.parentObj = parentObj;
		this.platWidth = platformFrontWidth;
		this.platLength = platformFrontLength;
		this.platHeight = platformHeight;
		this.isStair = isStair;
		stairHeight = platformHeight;

		parentObj.platformCenter = platformCenter;
		//***********************************************************************
		platFormStruct = CreatePlatform(parentObj.platform, platformCenter);
		StartCreateBorder(isBorder);
		StartCreateStair(isStair);
		//***********************************************************************
	}
	/**
	 * 組合亭初始化基座
	 */
	void ShowPos(Vector3 pos, GameObject parent, Color color, float localScale = 0.2f)
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		obj.transform.position = pos;
		obj.transform.parent = parent.transform;
		obj.transform.localScale = Vector3.one * localScale;
		obj.GetComponent<MeshRenderer>().material.color = color;
	}
	public void InitFunctionForCombinTing(BuildingObj parentObj, Vector3 platformCenter, float platformFrontWidth, float platformFrontLength, float platformHeight)
	{

		this.parentObj = parentObj;
		this.platWidth = platformFrontWidth;
		this.platLength = platformFrontLength;
		this.platHeight = platformHeight;
		stairHeight = platformHeight;
		parentObj.platformCenter = platformCenter;
		platFormStruct = CreatePlatformForCombinTing(parentObj.platform, platformCenter);
		StartCreateStair(false);
		//CreatePlatformForCombinTing(parentObj.platform, RTingCenter);
	}
	/**
	 * 更新移動後資訊
	 */
	public void MoveValueUpdate(Vector3 offset)
	{
		for (int i = 0; i < platFormStruct.bottomPointPosList.Count; i++)
		{
			platFormStruct.bottomPointPosList[i] += offset;
		}
		for (int i = 0; i < platFormStruct.topPointPosList.Count; i++)
		{
			platFormStruct.topPointPosList[i] += offset;
		}
		for (int i = 0; i < platFormStruct.stairPosList.Count; i++)
		{
			platFormStruct.stairPosList[i] += offset;
		}
	}
	//創建或刪除Border
	public void StartCreateBorder(bool isBorder)
	{
		this.isBorder = isBorder;

		if (isBorder)
		{
			if (platFormStruct.borderColumnList.Count == 0)
			{
				//欄杆
				platFormStruct.borderColumnList = CreateRingBorderColumn(ModelController.Instance.borderModelStruct, platFormStruct.topPointPosList, parentObj.platformCenter);
				//欄杆牆
				List<Vector3> borderColumnPosList = new List<Vector3>();
				for (int i = 0; i < platFormStruct.borderColumnList.Count; i++)
				{
					borderColumnPosList.Add(platFormStruct.borderColumnList[i].transform.position);
				}
				platFormStruct.borderWallList = CreateRingBorderWall(ModelController.Instance.borderModelStruct, borderColumnPosList);
			}
		}
		else
		{
			if (platFormStruct.borderColumnList.Count > 0)
			{
				for (int i = 0; i < platFormStruct.borderColumnList.Count; i++)
				{
					Destroy(platFormStruct.borderColumnList[i]);
				}
				platFormStruct.borderColumnList.Clear();
			}
		}
	}
	//創建或刪除樓梯
	public void StartCreateStair(bool isStair)
	{
		this.isStair = isStair;

		if (isStair)
		{
			if (platFormStruct.stairList.Count == 0)
				//platFormStruct.stairList = CreateRingStair(parentObj.platform, platFormStruct.stairPosList, platFormStruct.facadeDir, stairWidth, stairHeight, stairLength);
				platFormStruct.stairList = CreateRingStair(parentObj.platform, platFormStruct.borderColumnList, platFormStruct.stairPosList, platFormStruct.facadeDir, stairHeight, stairLength);
		}
		else
		{
			if (platFormStruct.stairList.Count > 0)
			{
				for (int i = 0; i < platFormStruct.stairList.Count; i++)
				{
					Destroy(platFormStruct.stairList[i]);
				}
				platFormStruct.stairList.Clear();
			}
		}
	}
	/**
	 * 製作基座
	 */
	private PlatFormStruct CreatePlatform(GameObject parent, Vector3 pos)
	{

		PlatFormStruct platFormStruct = new PlatFormStruct();

		GameObject platformBody = new GameObject("PlatformBody");
		//platformBody.transform.position = pos;
		platformBody.transform.parent = parent.transform;
		MeshFilter meshFilter = platformBody.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = platformBody.AddComponent<MeshRenderer>();
		meshRenderer.material.color = Color.white;

		List<Vector3> controlPointPosList = new List<Vector3>();

		//初始值******************************************************************************
		float platformRadius = (platWidth / 2.0f) / Mathf.Sin(2f * Mathf.PI / ((int)MainController.Instance.sides * 2));
		//***********************************************************************************

		if (isCurvePlatform)
		{
			Vector3 centerPos = Vector3.zero;
			List<Vector3> localPosList = new List<Vector3>();
			localPosList.Add(new Vector3(15, 2, 15));
			localPosList.Add(new Vector3(16, 0, 16));
			localPosList.Add(new Vector3(13, -3, 13));
			localPosList.Add(new Vector3(18, -8, 18));
			platHeight = Mathf.Abs(localPosList[0].y - localPosList[localPosList.Count - 1].y);
			controlPointPosList = MeshCenter.Instance.CreateRegularCurveRingMesh(pos, localPosList, Vector3.up, (int)MainController.Instance.sides, 100, 360.0f / (int)MainController.Instance.sides / 2, meshFilter);
		}
		else
		{
			if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
			{
				controlPointPosList = MeshCenter.Instance.CreateCubeMesh(pos, platWidth, platHeight, platLength, -90, meshFilter);
			}
			else
			{
				controlPointPosList = MeshCenter.Instance.CreateRegularRingMesh(pos, (int)MainController.Instance.sides, platformRadius, platHeight, 360.0f / (int)MainController.Instance.sides / 2, meshFilter);
			}
		}
		//計算底部與頂部位置
		platFormStruct.bottomPointPosList.Clear();
		platFormStruct.topPointPosList.Clear();

		for (int i = 0; i < (int)MainController.Instance.sides; i++)
		{
			platFormStruct.bottomPointPosList.Add(controlPointPosList[i]);

			platFormStruct.topPointPosList.Add(controlPointPosList[(controlPointPosList.Count - 1) - ((int)(MainController.Instance.sides - 1)) + i]);
		}

		ShowPos(platFormStruct.bottomPointPosList[0], platformBody, Color.black, 1.0f);
		ShowPos(platFormStruct.bottomPointPosList[1], platformBody, Color.red, 1.0f);
		ShowPos(platFormStruct.bottomPointPosList[2], platformBody, Color.yellow, 1.0f);

		//計算facadeDir與stair位置
		platFormStruct.facadeDir.Clear();
		platFormStruct.stairPosList.Clear();
		for (int index = 0; index < platFormStruct.topPointPosList.Count; index++)
		{
			Vector3 dir = Vector3.Cross(Vector3.up, platFormStruct.topPointPosList[(index + 1) % platFormStruct.topPointPosList.Count] - platFormStruct.topPointPosList[index]).normalized;

			Vector3 stairPos = (platFormStruct.topPointPosList[index] + platFormStruct.topPointPosList[(index + 1) % platFormStruct.topPointPosList.Count]) / 2.0f;
			stairPos.y = (platFormStruct.topPointPosList[index].y + platFormStruct.bottomPointPosList[index].y) / 2.0f;

			platFormStruct.facadeDir.Add(dir);
			platFormStruct.stairPosList.Add(stairPos);
		}
		return platFormStruct;
	}
	/**
	 * 製作組合亭基座
	 */
	private PlatFormStruct CreatePlatformForCombinTing(GameObject parent, Vector3 centerPostion)
	{

		PlatFormStruct platFormStruct = new PlatFormStruct();

		GameObject platformBody = new GameObject("PlatformBody");
		platformBody.transform.parent = parent.transform;
		MeshFilter meshFilter = platformBody.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = platformBody.AddComponent<MeshRenderer>();
		meshRenderer.material.color = Color.white;

		List<Vector3> controlPointPosList = new List<Vector3>();
		controlPointPosList.Clear();

		//初始值******************************************************************************
		float platformRadius = (platWidth / 2.0f) / Mathf.Sin(2f * Mathf.PI / ((int)MainController.Instance.sides * 2));
		//***********************************************************************************

		if (isCurvePlatform)
		{
			Vector3 centerPos = Vector3.zero;
			List<Vector3> localPosList = new List<Vector3>();
			localPosList.Add(new Vector3(15, 2, 15));
			localPosList.Add(new Vector3(16, 0, 16));
			localPosList.Add(new Vector3(13, -3, 13));
			localPosList.Add(new Vector3(18, -8, 18));
			platHeight = Mathf.Abs(localPosList[0].y - localPosList[localPosList.Count - 1].y);
			controlPointPosList = MeshCenter.Instance.CreateRegularCurveRingMesh(centerPostion, localPosList, Vector3.up, (int)MainController.Instance.sides, 100, 360.0f / (int)MainController.Instance.sides / 2, meshFilter);
		}
		else
		{
			if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
			{
				controlPointPosList = MeshCenter.Instance.CreateCubeMesh(centerPostion, platWidth, platHeight, platLength, -45, meshFilter);
			}
			else
			{
				controlPointPosList = MeshCenter.Instance.CreateRegularRingMesh(centerPostion, (int)MainController.Instance.sides, platformRadius, platHeight, 360.0f / (int)MainController.Instance.sides / 2, meshFilter);
			}
		}
		//計算底部與頂部位置
		platFormStruct.bottomPointPosList.Clear();
		platFormStruct.topPointPosList.Clear();
		for (int i = 0; i < (int)MainController.Instance.sides; i++)
		{
			platFormStruct.bottomPointPosList.Add(controlPointPosList[i]);
			platFormStruct.topPointPosList.Add(controlPointPosList[(controlPointPosList.Count - 1) - ((int)(MainController.Instance.sides - 1)) + i]);
		}
		/*
		ShowPos(platFormStruct.bottomPointPosList[0], platformBody, Color.black, 1.0f);
		ShowPos(platFormStruct.bottomPointPosList[1], platformBody, Color.red, 1.0f);
		ShowPos(platFormStruct.bottomPointPosList[2], platformBody, Color.yellow, 1.0f);
		*/
		//計算facadeDir與stair位置
		platFormStruct.facadeDir.Clear();
		platFormStruct.stairPosList.Clear();
		for (int index = 0; index < platFormStruct.topPointPosList.Count; index++)
		{
			Vector3 dir = Vector3.Cross(Vector3.up, platFormStruct.topPointPosList[(index + 1) % platFormStruct.topPointPosList.Count] - platFormStruct.topPointPosList[index]).normalized;

			Vector3 stairPos = (platFormStruct.topPointPosList[index] + platFormStruct.topPointPosList[(index + 1) % platFormStruct.topPointPosList.Count]) / 2.0f;
			stairPos.y = (platFormStruct.topPointPosList[index].y + platFormStruct.bottomPointPosList[index].y) / 2.0f;

			platFormStruct.facadeDir.Add(dir);
			platFormStruct.stairPosList.Add(stairPos);
		}
		return platFormStruct;
	}
	/**
	 * 製作樓梯
	 */
	private GameObject CreateStair(GameObject parentObj, Vector3 pos, Vector3 dir, float width, float height, float length)
	{
		GameObject stair = new GameObject("Stair");
		stair.transform.parent = parentObj.transform;
		MeshFilter meshFilter = stair.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = stair.AddComponent<MeshRenderer>();
		meshRenderer.material.color = Color.white;

		float rotateAngle = (Vector3.Dot(Vector3.right, dir) > 0 ? 1 : -1) * Vector3.Angle(dir, Vector3.forward);
		MeshCenter.Instance.CreateStairMesh(pos, width, height, length, rotateAngle, meshFilter);

		return stair;
	}
	/**
	 * 建築一圈皆製作樓梯
	 */
	private List<GameObject> CreateRingStair(GameObject parent, List<Vector3> posList, List<Vector3> dirList, float stairWidth, float stairHeight, float stairLength)
	{
		List<GameObject> stairList = new List<GameObject>();
		for (int i = 0; i < (int)MainController.Instance.sides; i++)
		{
			if (!parentObj.entraneIndexList.Contains(i)) { continue; }
			stairList.Add(CreateStair(parent, posList[i] + platFormStruct.facadeDir[i] * stairLength / 2.0f, dirList[i], stairWidth, stairHeight, stairLength));
		}
		return stairList;
	}
	private List<GameObject> CreateRingStair(GameObject parent, List<GameObject> borderColumnList,List<Vector3> posList, List<Vector3> dirList,float stairHeight, float stairLength)
	{
		List<GameObject> stairList = new List<GameObject>();
		for (int i = 0; i < (int)MainController.Instance.sides; i++)
		{
			if (!parentObj.entraneIndexList.Contains(i)) { continue; }
			float stairWidth=Vector3.Distance(borderColumnList[i*(borderColumnNumber-1)].transform.position,borderColumnList[i*(borderColumnNumber-1)+1].transform.position);
			stairList.Add(CreateStair(parent, posList[i] + platFormStruct.facadeDir[i] * stairLength / 2.0f, dirList[i], stairWidth, stairHeight, stairLength));
		}
		return stairList;
	}
	private List<GameObject> CreateRingBorderColumn(BorderModelStruct borderModelStruct, List<Vector3> posList, Vector3 platformCenter)
	{
		float borderColumnWidth = borderModelStruct.fenceModelStruct.bound.size.z;//欄杆柱子長度
		float borderColumnHeight = borderModelStruct.fenceModelStruct.bound.size.y;//欄杆柱子長度
		float borderColumnLengh = borderModelStruct.fenceModelStruct.bound.size.x;//欄杆柱子深度

		List<GameObject> borderColumnList = new List<GameObject>();

		for (int i = 0; i < posList.Count; i++)
		{
			Vector3 dir = posList[(i + 1) % posList.Count] - posList[i];
			Vector3 dirFormer = posList[i] - posList[(i - 1 + posList.Count) % posList.Count];
			float dis = Vector3.Distance(posList[i], posList[(i + 1) % posList.Count]);
			float width = dis / (Mathf.Max(borderColumnNumber-1,1));
			float rotateAngle = (Vector3.Dot(Vector3.right, dir) > 0 ? Vector3.Angle(dir, Vector3.forward) : (180 - Vector3.Angle(dir, Vector3.forward)));
			for (int j = 0; j < Mathf.Max(borderColumnNumber - 1, 1); j++)
			{
				//borderColumn	
				Vector3 pos = posList[i] + borderColumnHeight / 2.0f * Vector3.up + dir.normalized * (j * width) - dirFormer.normalized * (borderColumnLengh / 2.0f) + ((j == 0) ? (borderColumnWidth / 2.0f * dir.normalized) : Vector3.zero);
				GameObject clone = Instantiate(borderModelStruct.fenceModelStruct.model, pos, borderModelStruct.fenceModelStruct.model.transform.rotation) as GameObject;
				clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(borderModelStruct.fenceModelStruct.rotation);

				clone.transform.parent = parentObj.body.transform;

				borderColumnList.Add(clone);

			}
		}
		return borderColumnList;
	}
	private List<GameObject> CreateRingBorderWall(BorderModelStruct borderModelStruct, List<Vector3> columnList)
	{
		float borderWallWidth = borderModelStruct.fenceWallModelStruct.bound.size.x;//欄杆柱子長度
		float borderWallHeight = borderModelStruct.fenceWallModelStruct.bound.size.y;//欄杆柱子長度
		float borderWallLengh = borderModelStruct.fenceWallModelStruct.bound.size.z;//欄杆柱子深度

		float borderColumnWidth = borderModelStruct.fenceModelStruct.bound.size.z;//欄杆柱子長度
		float borderColumnHeight = borderModelStruct.fenceModelStruct.bound.size.y;//欄杆柱子長度
		float borderColumnLengh = borderModelStruct.fenceModelStruct.bound.size.x;//欄杆柱子深度

		List<GameObject> borderWallList = new List<GameObject>();

		for (int i = 0; i < columnList.Count; i++)
		{
			if (i % (borderColumnNumber - 1) == ((int)(borderColumnNumber - 1) / 2) && parentObj.entraneIndexList.Contains(i / (borderColumnNumber - 1))) continue;
			float width = borderWallWidth;
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - borderColumnWidth / 2.0f - borderColumnLengh/2.0f;
			int number = Mathf.Max(Mathf.FloorToInt(dis / width),1);
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			float disDiff = (dis - width * number) / number;
			width = dis / number;

			float rotateAngle = (Vector3.Dot(Vector3.right, dir) > 0 ? Vector3.Angle(dir, Vector3.forward) : 180 - Vector3.Angle(dir, Vector3.forward));
			for (int j = 0; j < number; j++)
			{
				Vector3 pos = columnList[i] + dir.normalized * (width / 2.0f + j * width + borderColumnWidth/2.0f) - (borderColumnHeight / 2.0f - borderWallHeight / 2.0f) * Vector3.up;
				GameObject clone = Instantiate(borderModelStruct.fenceWallModelStruct.model, pos, borderModelStruct.fenceWallModelStruct.model.transform.rotation) as GameObject;
				clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(borderModelStruct.fenceWallModelStruct.rotation);
				clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x * (borderWallWidth + disDiff) / borderWallWidth, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z));

				clone.transform.parent = parentObj.body.transform;
				borderWallList.Add(clone);
			}
		}
		return borderWallList;
	}
}
