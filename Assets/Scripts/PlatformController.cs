﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlatFormStruct 
{
	public List<Vector3> facadeDir = new List<Vector3>();
	public List<Vector3> topPointPosList = new List<Vector3>();
	public List<Vector3> bottomPointPosList = new List<Vector3>();
	public List<Vector3> stairPosList = new List<Vector3>();
}
public class PlatformController : MonoBehaviour
{

	public BuildingObj parentObj = null;
	//Platform**************************************************************************
	public enum PlatformType { };
	public float platformFrontWidth = 50;
	public float platformFrontLength = 50;
	public float platformHeight = 3;

	//Stair**************************************************************************
	public float stairWidth = 10;
	public float stairHeight;
	public float stairLength = 5;
	//**********************************************************************************
	public bool isCurvePlatform = false;
    public bool isStair = true;
    //***********************************************************************
	public PlatFormStruct platFormStruct;

    public void InitFunction(BuildingObj parentObj,Vector3 platformCenter, float platformFrontWidth, float platformFrontLength, float platformHeight )
    {
		this.parentObj = parentObj;
        this.platformFrontWidth = platformFrontWidth;
        this.platformFrontLength = platformFrontLength;
        this.platformHeight = platformHeight;
		stairHeight = platformHeight;
        //***********************************************************************
		 platFormStruct = CreatePlatform(parentObj.platform, platformCenter);
	
		//***********************************************************************

        if (isStair)
        {
			CreateRingStair(parentObj.platform, platFormStruct.stairPosList, platFormStruct.facadeDir, stairWidth, stairHeight, stairLength);
        }
    }
	public PlatFormStruct CreatePlatform(GameObject parentObj, Vector3 pos)
	{

		PlatFormStruct platFormStruct=new PlatFormStruct();

		GameObject platformBody = new GameObject("PlatformBody");
		//platformBody.transform.position = pos;
		platformBody.transform.parent = parentObj.transform;
		MeshFilter meshFilter = platformBody.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = platformBody.AddComponent<MeshRenderer>();
		meshRenderer.material.color = Color.white;

		List<Vector3> controlPointPosList = new List<Vector3>();
		controlPointPosList.Clear();

		//初始值******************************************************************************
		float platformRadius = (platformFrontWidth / 2.0f) / Mathf.Sin(2f * Mathf.PI / ((int)MainController.Instance.sides * 2));
		//***********************************************************************************

		if (isCurvePlatform)
		{
			Vector3 centerPos = Vector3.zero;
			List<Vector3> localPosList = new List<Vector3>();
			localPosList.Add(new Vector3(15, 2, 15));
			localPosList.Add(new Vector3(16, 0, 16));
			localPosList.Add(new Vector3(13, -3, 13));
			localPosList.Add(new Vector3(18, -8, 18));
            platformHeight = Mathf.Abs(localPosList[0].y - localPosList[localPosList.Count - 1].y);
            controlPointPosList = MeshCenter.Instance.CreateRegularCurveRingMesh(pos, localPosList, Vector3.up, (int)MainController.Instance.sides, 100, 360.0f / (int)MainController.Instance.sides / 2, meshFilter);
        }
        else
		{
            if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
            {
                controlPointPosList = MeshCenter.Instance.CreateCubeMesh(pos, platformFrontWidth, platformHeight, platformFrontLength, -90, meshFilter);
            }
            else
            {
                controlPointPosList = MeshCenter.Instance.CreateRegularRingMesh(pos, (int)MainController.Instance.sides, platformRadius, platformHeight, 360.0f / (int)MainController.Instance.sides / 2, meshFilter);
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
	public void CreateStair(GameObject parentObj, Vector3 pos,Vector3 dir, float width, float height, float length)
	{
        GameObject stair = new GameObject("Stair");
		stair.transform.parent = parentObj.transform;
        MeshFilter meshFilter = stair.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = stair.AddComponent<MeshRenderer>();
        meshRenderer.material.color = Color.white;
 
        float rotateAngle = (Vector3.Dot(Vector3.right, dir) > 0 ? 1 : -1) * Vector3.Angle(dir, Vector3.forward);
        MeshCenter.Instance.CreateStairMesh(pos, width, height, length, rotateAngle, meshFilter);
    }
	public void CreateRingStair(GameObject parentObj, List<Vector3> posList, List<Vector3> dirList, float stairWidth, float stairHeight, float stairLength)
	{
		for (int i = 0; i < (int)MainController.Instance.sides; i++)
		{
			CreateStair(parentObj, posList[i] + platFormStruct.facadeDir[i] * stairLength / 2.0f, dirList[i], stairWidth, stairHeight, stairLength);
		}
	}
}
