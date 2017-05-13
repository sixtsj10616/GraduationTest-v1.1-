using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObj : MonoBehaviour {

    public GameObject building;
   
    public PlatformController platformController;
    public BodyController bodyController;
    public RoofController roofController;
    public GameObject platform;
    public GameObject body;
    public GameObject roof;

	public Vector3 platformCenter;
	public Vector3 bodyCenter;
	public Vector3 roofTopCenter;

	public float buildingHeight;                //建築(樓層)高度
	public void BuildingMove(Vector3 offset) 
	{
		building.transform.position += offset;
		platformCenter += offset;
		bodyCenter+=offset;
		roofTopCenter += offset;
	}
	public void initFunction(GameObject building , Vector3 position, float platLength, float platWidth, float platHeight,float eaveColumnHeight,float goldColumnHeight,float mainRidgeHeightOffset,float allJijaHeight)
	{
		this.building=building;
		this.building.transform.parent = building.transform;

		platformCenter=position;

		platform = new GameObject("platform");
		platform.transform.parent = this.building.transform;

		body = new GameObject("body");
		body.transform.parent = this.building.transform;

		roof = new GameObject("roof");
		roof.transform.parent = this.building.transform;

		platformController = building.AddComponent<PlatformController>();
		bodyController = building.AddComponent<BodyController>();
		roofController = building.AddComponent<RoofController>();

		platformController.InitFunction(this, platformCenter, platWidth, platLength, platHeight);
		bodyController.InitFunction(this, platformController.platFormStruct.topPointPosList, platWidth, platHeight, eaveColumnHeight, goldColumnHeight);
		roofController.InitFunction(this, bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), eaveColumnHeight, mainRidgeHeightOffset, allJijaHeight);

		buildingHeight = Vector3.Distance(roofTopCenter, platformCenter);
	}
	public void ResetRoofFunction(float mainRidgeHeightOffset, float allJijaHeight, int roofType = 2)
	{
		Destroy(roof);

		roof = new GameObject("roof");
		roof.transform.parent = this.building.transform;
		roofController.SetRoofType(roofType);
		roofController.InitFunction(this, bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), bodyController.eaveColumnHeight,mainRidgeHeightOffset, allJijaHeight);

		buildingHeight = Vector3.Distance(roofTopCenter, platformCenter);
	}
}
