﻿using UnityEngine;
using System.Collections;


public struct ModelStruct//模型旋轉、縮放 
{
	// x  (Vector3.right)     :width 
	// y  (Vector3.up)        :height
	// z  (Vector3.forward)   :lengh
	public GameObject model;
	public Vector3 rotation;
	public Vector3 scale;
	public Bounds bound;

	public ModelStruct(GameObject model, Vector3 rotation, Vector3 scale)
	{
		this.model = model;
		this.rotation = rotation;
		this.scale = scale;

		this.bound = model.GetComponentInChildren<Collider>().bounds;
		model.transform.GetChild(0).localScale = Vector3.Scale(scale, model.transform.GetChild(0).localScale);
		bound.SetMinMax(Vector3.Scale(bound.min, scale), Vector3.Scale(bound.max, scale));
	}
}
public struct BorderModelStruct
{
	public ModelStruct fenceModelStruct;
	public ModelStruct fenceWallModelStruct;

	public BorderModelStruct(ModelStruct fenceModelStruct, ModelStruct fenceWallModelStruct)
	{
		this.fenceModelStruct = fenceModelStruct;
		this.fenceWallModelStruct = fenceWallModelStruct;
	}
}
public struct EaveColumnModelStruct
{
    public ModelStruct friezeModelStruct;
    public ModelStruct balustradeModelStruct;
	public ModelStruct sparrowBraceModelStruct;

    public EaveColumnModelStruct(ModelStruct friezeModelStruct, ModelStruct balustradeModelStruct,ModelStruct sparrowBraceModelStruct)
    {
        this.friezeModelStruct = friezeModelStruct;
        this.balustradeModelStruct = balustradeModelStruct;
		this.sparrowBraceModelStruct=sparrowBraceModelStruct;
    }
}
public struct GoldColumnModelStruct
{
    public ModelStruct windowModelStruct;
	public ModelStruct doorModelStruct;
	public ModelStruct windowWallModelStruct;
	public GoldColumnModelStruct(ModelStruct windowModelStruct, ModelStruct doorModelStruct, ModelStruct windowWallModelStruct)
    {
        this.windowModelStruct = windowModelStruct;
		this.doorModelStruct = doorModelStruct;
		this.windowWallModelStruct=windowWallModelStruct;
    }
}
public struct MainRidgeModelStruct//主脊模型
{
	public ModelStruct mainRidgeTileModelStruct;

	public MainRidgeModelStruct(ModelStruct mainRidgeTileModelStruct)
	{
		this.mainRidgeTileModelStruct = mainRidgeTileModelStruct;
	}
}
public struct RoofSurfaceModelStruct//屋面模型
{
	public ModelStruct roundTileModelStruct;
	public ModelStruct flatTileModelStruct;
	public ModelStruct eaveTileModelStruct;
	public ModelStruct flyingRafterModelStruct;


	public RoofSurfaceModelStruct(ModelStruct roundTileModelStruct, ModelStruct flatTileModelStruct, ModelStruct eaveTileModelStruct, ModelStruct flyingRafterModelStruct)
	{
		this.roundTileModelStruct = roundTileModelStruct;
		this.flatTileModelStruct = flatTileModelStruct;
		this.eaveTileModelStruct = eaveTileModelStruct;
		this.flyingRafterModelStruct = flyingRafterModelStruct;
	}
}
public class ModelController : Singleton<ModelController>
{
	//************************ 女兒牆 ************************
	[HideInInspector]
	public BorderModelStruct borderModelStruct;

	public GameObject fenceModel;
	public Vector3 fenceModelRotation = Vector3.zero;
	public Vector3 fenceModelScale = new Vector3(1, 1, 1);
	private ModelStruct fenceModelStruct;

	public GameObject fenceWallModel;
	public Vector3 fenceWallModelRotation = Vector3.zero;
	public Vector3 fenceWallModelScale = new Vector3(1, 1, 1);
	private ModelStruct fenceWallModelStruct;
    //************************ 檐柱 ************************
    [HideInInspector]
    public EaveColumnModelStruct eaveColumnModelStruct;

    public GameObject friezeModel;
    public Vector3 friezeModelRotation = Vector3.zero;
    public Vector3 friezeModelScale = new Vector3(5, 5, 5);
    private ModelStruct friezeModelStruct;

    public GameObject balustradeModel;
    public Vector3 balustradeModelRotation = Vector3.zero;
    public Vector3 balustradeModelScale = new Vector3(5, 5, 5);
    private ModelStruct balustradeModelStruct;

	public GameObject sparrowBraceModel;
	public Vector3 sparrowBraceModelRotation = Vector3.zero;
	public Vector3 sparrowBraceModelScale = new Vector3(5, 5, 5);
	private ModelStruct sparrowBraceModelStruct;
	//************************ 金柱 ************************
    [HideInInspector]
    public GoldColumnModelStruct goldColumnModelStruct;

    public GameObject windowModel;
    public Vector3 windowModelRotation = Vector3.zero;
    public Vector3 windowModelScale = Vector3.one;
    private ModelStruct windowModelStruct;

	public GameObject doorModel;
	public Vector3 doorModelRotation = Vector3.zero;
	public Vector3 doorModelScale = Vector3.one;
	private ModelStruct doorModelStruct;

	public GameObject windowWallModel;
	public Vector3 windowWallModelRotation = Vector3.zero;
	public Vector3 windowWallModelScale = Vector3.one;
	private ModelStruct windowWallModelStruct;

    //************************ 屋瓦 ************************
    [HideInInspector]
    public RoofSurfaceModelStruct roofSurfaceModelStruct;

    public GameObject roundTileModel;
    public Vector3 roundTileModelRotation = new Vector3(0, 90, 0);
    public Vector3 roundTileModelScale = Vector3.one;
    private ModelStruct roundTileModelStruct;

    public GameObject flatTileModel;
    public Vector3 flatTileModelRotation = Vector3.zero;
    public Vector3 flatTileModelScale = new Vector3(1, 1, 1.2f);
    private ModelStruct flatTileModelStruct;

    public GameObject eaveTileModel;
    public Vector3 eaveTileModelRotation = Vector3.zero;
    public Vector3 eaveTileModelScale = Vector3.one;
    private ModelStruct eaveTileModelStruct;

	public GameObject flyingRafterModel;
	public Vector3 flyingRafterModelRotation = Vector3.zero;
	public Vector3 flyingRafterModelScale = Vector3.one;
	private ModelStruct flyingRafterModelStruct;

    //************************ 主脊 ************************
    [HideInInspector]
    public MainRidgeModelStruct mainRidgeModelStruct;

    public GameObject mainRidgeTileModel;
    public Vector3 mainRidgeTileModelRotation = Vector3.zero;
    public Vector3 mainRidgeTileModelScale = new Vector3(2, 2, 0.5f);
    private ModelStruct mainRidgeTileModelStruct;

    
    void Awake()
    {
		//*** Border ***
		fenceModelStruct = new ModelStruct(fenceModel, fenceModelRotation, fenceModelScale);
		fenceWallModelStruct = new ModelStruct(fenceWallModel, fenceWallModelRotation, fenceWallModelScale);
		borderModelStruct = new BorderModelStruct(fenceModelStruct, fenceWallModelStruct);

        //*** 欄杆 ***
        friezeModelStruct = new ModelStruct(friezeModel, friezeModelRotation, friezeModelScale);
        balustradeModelStruct = new ModelStruct(balustradeModel, balustradeModelRotation, balustradeModelScale);
		sparrowBraceModelStruct = new ModelStruct(sparrowBraceModel, sparrowBraceModelRotation, sparrowBraceModelScale);
		eaveColumnModelStruct = new EaveColumnModelStruct(friezeModelStruct, balustradeModelStruct, sparrowBraceModelStruct);

        windowModelStruct = new ModelStruct(windowModel, windowModelRotation, windowModelScale);
		doorModelStruct = new ModelStruct(doorModel, doorModelRotation, doorModelScale);
		windowWallModelStruct = new ModelStruct(windowWallModel, windowWallModelRotation, windowWallModelScale);
		goldColumnModelStruct = new GoldColumnModelStruct(windowModelStruct, doorModelStruct, windowWallModelStruct);

        //*** 瓦片結構 : 設定好筒瓦、平瓦、簷瓦模組與個別的旋轉與尺寸向量
        roundTileModelStruct = new ModelStruct(roundTileModel, roundTileModelRotation, roundTileModelScale);
        flatTileModelStruct = new ModelStruct(flatTileModel, flatTileModelRotation, flatTileModelScale);
        eaveTileModelStruct = new ModelStruct(eaveTileModel, eaveTileModelRotation, eaveTileModelScale);
		flyingRafterModelStruct = new ModelStruct(flyingRafterModel, flyingRafterModelRotation, flyingRafterModelScale);
		roofSurfaceModelStruct = new RoofSurfaceModelStruct(roundTileModelStruct, flatTileModelStruct, eaveTileModelStruct, flyingRafterModelStruct);

        //*** 主瘠結構 : 設定好主脊模組跟旋轉、尺寸向量
        mainRidgeTileModelStruct = new ModelStruct(mainRidgeTileModel, mainRidgeTileModelRotation, mainRidgeTileModelScale);
        mainRidgeModelStruct = new MainRidgeModelStruct(mainRidgeTileModelStruct);

    }
}
