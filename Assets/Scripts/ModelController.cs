using UnityEngine;
using System.Collections;


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

    public GoldColumnModelStruct(ModelStruct windowModelStruct)
    {
        this.windowModelStruct = windowModelStruct;
    }
}

public class ModelController : Singleton<ModelController>
{

    //************************ 欄杆 ************************
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
    [HideInInspector]
    public GoldColumnModelStruct goldColumnModelStruct;

    public GameObject windowModel;
    public Vector3 windowModelRotation = Vector3.zero;
    public Vector3 windowModelScale = Vector3.one;
    private ModelStruct windowModelStruct;

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
        //*** 欄杆 ***
        friezeModelStruct = new ModelStruct(friezeModel, friezeModelRotation, friezeModelScale);
        balustradeModelStruct = new ModelStruct(balustradeModel, balustradeModelRotation, balustradeModelScale);
		sparrowBraceModelStruct = new ModelStruct(sparrowBraceModel, sparrowBraceModelRotation, sparrowBraceModelScale);
		eaveColumnModelStruct = new EaveColumnModelStruct(friezeModelStruct, balustradeModelStruct, sparrowBraceModelStruct);

        windowModelStruct = new ModelStruct(windowModel, windowModelRotation, windowModelScale);
        goldColumnModelStruct = new GoldColumnModelStruct(windowModelStruct);

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
