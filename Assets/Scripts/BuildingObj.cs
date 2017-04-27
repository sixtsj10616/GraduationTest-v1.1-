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


    //public void initFunction(Vector3 position)
    //{
    //    building = new GameObject("building");
    //    building.transform.position = position;
    //    //platform = new PlatformController();
    //    platform = building.AddComponent<PlatformController>();
    //    platform.InitFunction(this);
    //    body = building.AddComponent<BodyController>();
    //    body.InitFunction(this);
    //    roof = building.AddComponent<RoofController>();
    //    roof.InitFunction(this);
    //}

    public void initFunction(GameObject parent,Vector3 position,float length,float width,float height)
    {
        /*
        building = new GameObject("building");
        building.transform.position = position;
        building.transform.parent = parent.transform;
        platform = building.AddComponent<PlatformController>();
        //platform.InitFunction(this);
        platform.InitFunction(this, position, width, length, height);
        body = building.AddComponent<BodyController>();
        body.InitFunction(this);
        roof = building.AddComponent<RoofController>();
        roof.InitFunction(this);
        */
        building = new GameObject("building");
        building.transform.parent = parent.transform;
        this.transform.position = position;
        platform = new GameObject("platform");
        platform.transform.parent = parent.transform;
        platform.AddComponent<PlatformController>();
        platformController = platform.GetComponent<PlatformController>();
        platformController.InitFunction(this, position, width, length, height);
        body = new GameObject("body");
        body.transform.parent = parent.transform;
        body.AddComponent<BodyController>();
        bodyController = body.GetComponent<BodyController>();
        bodyController.InitFunction(this);
        roof = new GameObject("roof");
        roof.transform.parent = parent.transform;
        roof.AddComponent<RoofController>();
        roofController = roof.GetComponent<RoofController>();
        roofController.InitFunction(this);
    }

}
