using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlatFormStruct 
{
	public List<Vector3> facadeDir = new List<Vector3>();
	public List<Vector3> topPointPosList = new List<Vector3>();
	public List<Vector3> bottomPointPosList = new List<Vector3>();
	public List<Vector3> stairPosList = new List<Vector3>();

	public List<GameObject> stairList = new List<GameObject>();
}
public class PlatformController : MonoBehaviour
{

	public BuildingObj parentObj = null;
	//Platform**************************************************************************
	public enum PlatformType { };
	public float platformFrontWidth;
	public float platformFrontLength;
	public float platformHeight;

	//Stair**************************************************************************
	public float stairWidth = 10;
	public float stairHeight;
	public float stairLength = 5;
	//**********************************************************************************
	public bool isCurvePlatform = false;
    public bool isStair = false;
    //***********************************************************************
	public PlatFormStruct platFormStruct;

    public void InitFunction(BuildingObj parentObj,Vector3 platformCenter, float platformFrontWidth, float platformFrontLength, float platformHeight )
    {
		this.parentObj = parentObj;
        this.platformFrontWidth = platformFrontWidth;
        this.platformFrontLength = platformFrontLength;
        this.platformHeight = platformHeight;
		stairHeight = platformHeight;

		parentObj.platformCenter = platformCenter;
        //***********************************************************************
		 platFormStruct = CreatePlatform(parentObj.platform, platformCenter);
		//***********************************************************************
    }
    /**
     * 組合亭初始化基座
     */
    public void InitFunctionForCombinTing(BuildingObj parentObj, Vector3 platformCenter, float platformFrontWidth, float platformFrontLength, float platformHeight)
    {
        
        this.parentObj = parentObj;
        this.platformFrontWidth = platformFrontWidth;
        this.platformFrontLength = platformFrontLength;
        this.platformHeight = platformHeight;
        stairHeight = platformHeight;
        parentObj.platformCenter = platformCenter;
        platFormStruct = CreatePlatformForCombinTing(parentObj.platform, platformCenter);
        //CreatePlatformForCombinTing(parentObj.platform, RTingCenter);
    }
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
	public void SetStair(bool isStair) 
	{
		if (isStair)
		{
			platFormStruct.stairList = CreateRingStair(parentObj.platform, platFormStruct.stairPosList, platFormStruct.facadeDir, stairWidth, stairHeight, stairLength);
		}
		else 
		{
			if (platFormStruct.stairList.Count>0)
			{
				for(int i=0;i<platFormStruct.stairList.Count;i++)
				{
					Destroy(platFormStruct.stairList[i]);
				}
				platFormStruct.stairList.Clear();
			}
		}
	
	}
	void ShowPos(Vector3 pos, GameObject parent, Color color, float localScale = 0.2f)
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		obj.transform.position = pos;
		obj.transform.parent = parent.transform;
		obj.transform.localScale = Vector3.one * localScale;
		obj.GetComponent<MeshRenderer>().material.color = color;
	}
	private PlatFormStruct CreatePlatform(GameObject parent, Vector3 pos)
	{

		PlatFormStruct platFormStruct=new PlatFormStruct();

		GameObject platformBody = new GameObject("PlatformBody");
		//platformBody.transform.position = pos;
		platformBody.transform.parent = parent.transform;
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
            controlPointPosList = MeshCenter.Instance.CreateRegularCurveRingMesh(centerPostion, localPosList, Vector3.up, (int)MainController.Instance.sides, 100, 360.0f / (int)MainController.Instance.sides / 2, meshFilter);
        }
        else
        {
            if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
            {
                controlPointPosList = MeshCenter.Instance.CreateCubeMesh(centerPostion, platformFrontWidth, platformHeight, platformFrontLength, -45, meshFilter);
            }
            else
            {
                controlPointPosList = MeshCenter.Instance.CreateRegularRingMesh(centerPostion, (int)MainController.Instance.sides, platformRadius, platformHeight, 360.0f / (int)MainController.Instance.sides / 2, meshFilter);
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
	private List<GameObject> CreateRingStair(GameObject parentObj, List<Vector3> posList, List<Vector3> dirList, float stairWidth, float stairHeight, float stairLength)
	{
		List<GameObject> stairList=new List<GameObject>();
		for (int i = 0; i < (int)MainController.Instance.sides; i++)
		{
			stairList.Add(CreateStair(parentObj, posList[i] + platFormStruct.facadeDir[i] * stairLength / 2.0f, dirList[i], stairWidth, stairHeight, stairLength));
		}
		return stairList;
	}
}
