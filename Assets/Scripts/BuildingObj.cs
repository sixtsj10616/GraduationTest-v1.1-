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

	public void initFunction(GameObject building , Vector3 position, float length, float width, float height)
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

		platformController.InitFunction(this, platformCenter, width, length, height);
		bodyController.InitFunction(this, platformController.platFormStruct.topPointPosList, width, length);
		roofController.InitFunction(this, bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), bodyController.eaveColumnHeight, bodyController.eaveColumnHeight);

		buildingHeight = Vector3.Distance(roofTopCenter, platformCenter);
	}
	public void ResetRoofFunction(int roofType=2)
	{
		Destroy(roof);

		roof = new GameObject("roof");
		roof.transform.parent = this.building.transform;

		roofController.SetRoofType(roofType);
		roofController.InitFunction(this, bodyController.GetColumnStructTopPosList(bodyController.eaveCornerColumnList), bodyController.eaveColumnHeight, roofController.allJijaHeight);

		buildingHeight = Vector3.Distance(roofTopCenter, platformCenter);
	}

}
