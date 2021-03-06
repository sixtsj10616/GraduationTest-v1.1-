﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
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
	//方便CT共同存取**********************************************************************************
	protected GameObject parentObj;
	protected int sides;
	protected EntraneIndexList entraneIndexList;
	protected BuildingObj buildingObj;
	//Platform**************************************************************************
	public enum PlatformType { };
	public float platWidth;
	public float platLength;
	public float platHeight;

	//Stair*****************************************************************************
	public float stairWidth = 5;
	public float stairHeight;
	public float stairLength = 5;
	//**********************************************************************************
	public bool isStair = false;
	public bool isBorder = false;
	//**********************************************************************************
	public int borderColumnNumber = 8;//必須是偶數
	//**********************************************************************************
	public PlatFormStruct platFormStruct;

	/**
	 * 初始化基座
	 */
	public void InitFunction(BuildingObj buildingObj, float platformFrontWidth, float platformFrontLength, float platformHeight, bool isStair, float rotateAngle = 0)
	{
		this.buildingObj=buildingObj;
		this.platWidth = platformFrontWidth;
		this.platLength = platformFrontLength;
		this.platHeight = platformHeight;
		this.isStair = isStair;
		stairHeight = platformHeight;

		this.parentObj=buildingObj.platform;
		this.sides=(int)buildingObj.sides;
		this.entraneIndexList = buildingObj.entraneIndexList;

		//***********************************************************************
		platFormStruct = CreatePlatform(buildingObj.platform, (int)buildingObj.sides, buildingObj.platformCenter, rotateAngle);
		StartCreateBorder(isBorder);
		StartCreateStair(isStair);

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
				platFormStruct.borderColumnList = CreateRingBorderColumn(ModelController.Instance.borderModelStruct, parentObj,platFormStruct.topPointPosList);
				//欄杆牆
				List<Vector3> borderColumnPosList = new List<Vector3>();
				for (int i = 0; i < platFormStruct.borderColumnList.Count; i++)
				{
					borderColumnPosList.Add(platFormStruct.borderColumnList[i].transform.position);
				}
				platFormStruct.borderWallList = CreateRingBorderWall(ModelController.Instance.borderModelStruct, parentObj, borderColumnPosList);
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
                for (int iIndex = 0; iIndex < platFormStruct.borderWallList.Count; iIndex++)
                {
                    Destroy(platFormStruct.borderWallList[iIndex]);
                }
				platFormStruct.borderColumnList.Clear();
                platFormStruct.borderWallList.Clear();

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
				platFormStruct.stairList = CreateRingStair(parentObj, platFormStruct.borderColumnList, platFormStruct.stairPosList, platFormStruct.facadeDir, stairHeight, stairLength);
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
	private PlatFormStruct CreatePlatform(GameObject parent,int sides, Vector3 pos,float rotateAngle=0)
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
		float platformRadius = (platWidth / 2.0f) / Mathf.Sin(2f * Mathf.PI / (sides * 2));
		//***********************************************************************************

		if (sides ==(int) MainController.FormFactorSideType.FourSide)
			controlPointPosList = MeshCenter.Instance.CreateCubeMesh(pos, platWidth, platHeight, platLength,rotateAngle, meshFilter);
		else
			controlPointPosList = MeshCenter.Instance.CreateRegularRingMesh(pos, sides, platformRadius, platHeight, rotateAngle, meshFilter);
		//計算底部與頂部位置
		platFormStruct.bottomPointPosList.Clear();
		platFormStruct.topPointPosList.Clear();

		for (int i = 0; i < sides; i++)
		{
			platFormStruct.bottomPointPosList.Add(controlPointPosList[i]);

			platFormStruct.topPointPosList.Add(controlPointPosList[(controlPointPosList.Count - 1) - ((sides - 1)) + i]);
		}

		MainController.ShowPos(platFormStruct.bottomPointPosList[0], platformBody, Color.blue, 1.0f);
        MainController.ShowPos(platFormStruct.bottomPointPosList[1], platformBody, Color.red, 1.0f);
        MainController.ShowPos(platFormStruct.bottomPointPosList[2], platformBody, Color.yellow, 1.0f);

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
     * 邏輯有點怪..有欄杆時的邏輯
	 */
	private List<GameObject> CreateRingStair(GameObject parentObj, List<Vector3> posList, List<Vector3> dirList, float stairWidth, float stairHeight, float stairLength)
	{
		List<GameObject> stairList = new List<GameObject>();
		for (int i = 0; i < sides; i++)
		{
			if (!entraneIndexList.Contains(i)) { continue; }
			stairList.Add(CreateStair(parentObj, posList[i] + platFormStruct.facadeDir[i] * stairLength / 2.0f, dirList[i], stairWidth, stairHeight, stairLength));
		}
		return stairList;
	}
	public List<GameObject> CreateRingStair(GameObject parentObj, List<GameObject> borderColumnList, List<Vector3> posList, List<Vector3> dirList, float stairHeight, float stairLength)
	{
		List<GameObject> stairList = new List<GameObject>();
		for (int i = 0; i < sides; i++)
		{
			if (!entraneIndexList.Contains(i)) { continue; }
            if (borderColumnList.Count > 0)
            {
                stairWidth = Vector3.Distance(borderColumnList[i * (borderColumnNumber - 1)].transform.position, borderColumnList[i * (borderColumnNumber - 1) + 1].transform.position);
            }
            else
            {

            }
			stairList.Add(CreateStair(parentObj, posList[i] + platFormStruct.facadeDir[i] * stairLength / 2.0f, dirList[i], stairWidth, stairHeight, stairLength));
		}
		return stairList;
	}
    /**
     * 製作基座欄杆
     */
	private List<GameObject> CreateRingBorderColumn(BorderModelStruct borderModelStruct, GameObject parentObj, List<Vector3> posList)
	{
		float borderColumnWidth = borderModelStruct.fenceModelStruct.bound.size.x;//欄杆柱子長度
		float borderColumnHeight = borderModelStruct.fenceModelStruct.bound.size.y;//欄杆柱子長度
		float borderColumnLengh = borderModelStruct.fenceModelStruct.bound.size.z;//欄杆柱子深度

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

				clone.transform.parent = parentObj.transform;

				borderColumnList.Add(clone);

			}
		}
		return borderColumnList;
	}
	private List<GameObject> CreateRingBorderWall(BorderModelStruct borderModelStruct,GameObject parentObj, List<Vector3> columnList)
	{
		float borderWallWidth = borderModelStruct.fenceWallModelStruct.bound.size.x;//欄杆柱子長度
		float borderWallHeight = borderModelStruct.fenceWallModelStruct.bound.size.y;//欄杆柱子長度
		float borderWallLengh = borderModelStruct.fenceWallModelStruct.bound.size.z;//欄杆柱子深度

		float borderColumnWidth = borderModelStruct.fenceModelStruct.bound.size.x;//欄杆柱子長度
		float borderColumnHeight = borderModelStruct.fenceModelStruct.bound.size.y;//欄杆柱子長度
		float borderColumnLengh = borderModelStruct.fenceModelStruct.bound.size.z;//欄杆柱子深度

		List<GameObject> borderWallList = new List<GameObject>();

		for (int i = 0; i < columnList.Count; i++)
		{
			if (i % (borderColumnNumber - 1) == ((int)(borderColumnNumber - 1) / 2) && entraneIndexList.Contains(i / (borderColumnNumber - 1))) continue;
			float width = borderWallWidth;
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - borderColumnWidth / 2.0f - borderColumnLengh/2.0f;
			int number = Mathf.Max(Mathf.FloorToInt(dis / width),1);
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			float disDiff = (dis - width * number) / number;
			width = dis / number;

			float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
			for (int j = 0; j < number; j++)
			{
				Vector3 pos = columnList[i] + dir.normalized * (width / 2.0f + j * width + borderColumnWidth/2.0f) - (borderColumnHeight / 2.0f - borderWallHeight / 2.0f) * Vector3.up;
				GameObject clone = Instantiate(borderModelStruct.fenceWallModelStruct.model, pos, borderModelStruct.fenceWallModelStruct.model.transform.rotation) as GameObject;
				clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(borderModelStruct.fenceWallModelStruct.rotation);
				clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x * (width) / borderWallWidth, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z));
				clone.transform.parent = parentObj.transform;
				borderWallList.Add(clone);
			}
		}
		return borderWallList;
	}
}
