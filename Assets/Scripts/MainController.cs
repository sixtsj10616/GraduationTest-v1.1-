using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public struct ModelStruct//模型旋轉、縮放
{
	public GameObject model;
	public Vector3 rotation;
	public Vector3 scale;
    
	public ModelStruct(GameObject model, Vector3 rotation, Vector3 scale)
	{
		this.model = model;
		this.rotation = rotation;
		this.scale = scale;
	}
}
public class MainController : Singleton<MainController>
{

    //public GameObject building;
	public List<BuildingObj> Buildings = new List<BuildingObj>();       //* 建築樓層列表
    public int selectFloor = 0;                                       //* 目前 選擇/建造 的樓層
    //FormFactor***********************************************************************
    public enum FormFactorSideType { ThreeSide = 3, FourSide = 4, FiveSide = 5, SixSide = 6, EightSide = 8 };
    public FormFactorSideType sides = FormFactorSideType.ThreeSide;
    //**********************************************************************************


    // Use this for initialization

    private void Awake()
    {
        InitFunction();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            print("A key was pressed");
            createNewFloor();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            print("D key was pressed");
            if (selectFloor != 0 && selectFloor < Buildings.Count)
            { 
                deleteFloor(selectFloor);
            }
        }
    }
    /**
     * 初始化
     */
    public void InitFunction()
    {
       GameObject building = new GameObject("building");
	   building.gameObject.AddComponent<BuildingObj>();
	   building.GetComponent<BuildingObj>().initFunction(building,Vector3.zero, 30, 30, 3);
	   Buildings.Add(building.GetComponent<BuildingObj>());
    }
    /**
     * 新增樓層
     */
    public void createNewFloor()
    {
        selectFloor++;
        GameObject newBuilding = new GameObject("building" + Buildings.Count);

		Buildings[selectFloor - 1].ResetRoofFunction();
        Vector3 nowTopPosition = Buildings[selectFloor - 1].GetComponent<BuildingObj>().roofTopCenter;

        newBuilding.AddComponent<BuildingObj>();
        Buildings.Insert(selectFloor, newBuilding.GetComponent<BuildingObj>());
        for (int iIndex = selectFloor + 1; iIndex < Buildings.Count; iIndex++)
        {
			Buildings[iIndex].building.transform.position += Buildings[selectFloor-1].buildingHeight *Vector3.up;
        }
        newBuilding.GetComponent<BuildingObj>().initFunction(newBuilding,nowTopPosition, 30, 30, 3);
    }
    /**
     * 刪除樓層
     */
    public void deleteFloor(int floor)
    {
		float tmpFloorHeight = Buildings[selectFloor].buildingHeight;
		int roofType = (int)Buildings[selectFloor].roofController.roofType;
		Destroy(Buildings[selectFloor].gameObject);
        Buildings.RemoveAt(selectFloor);
        for (int iIndex = floor; iIndex < Buildings.Count; iIndex++)
        {
			Buildings[iIndex].building.transform.position -=  tmpFloorHeight * Vector3.up;
        }
        selectFloor = floor - 1;

		if (selectFloor == Buildings.Count - 1) Buildings[selectFloor].ResetRoofFunction(roofType);
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
