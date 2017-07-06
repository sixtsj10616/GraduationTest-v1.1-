using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class CurveFitting
{
	int degree = 2;

	List<float> a;
	public CurveFitting(List<float> x, List<float> y)
	{
		int i, j, k;
		int size = Mathf.Min(x.Count, y.Count);

		List<float> X = new List<float>();
		for (i = 0; i < 2 * degree + 1; i++)
		{
			X.Add(0);
			for (j = 0; j < size; j++)
				X[i] = X[i] + Mathf.Pow(x[j], i);        //consecutive positions of the array will store N,sigma(xi),sigma(xi^2),sigma(xi^3)....sigma(xi^2n)
		}

		List<List<float>> B = new List<List<float>>();//B is the Normal matrix(augmented) that will store the equations, 'a' is for value of the final coefficients
		for (i = 0; i < degree + 1; i++)
		{
			List<float> c = new List<float>();
			for (j = 0; j < degree + 2; j++)
			{
				c.Add(0);
			}
			B.Add(c);
		}

		a = new List<float>();
		for (i = 0; i < degree + 1; i++)
		{
			a.Add(0);
		}
		for (i = 0; i <= degree; i++)
			for (j = 0; j <= degree; j++)
				B[i][j] = X[i + j];

		List<float> Y = new List<float>();
		for (i = 0; i < degree + 1; i++)
		{
			Y.Add(0);
			for (j = 0; j < size; j++)
				Y[i] = Y[i] + Mathf.Pow(x[j], i) * y[j];        //consecutive positions will store sigma(yi),sigma(xi*yi),sigma(xi^2*yi)...sigma(xi^n*yi)
		}
		for (i = 0; i <= degree; i++)
			B[i][degree + 1] = Y[i];                //load the values of Y as the last column of B(Normal Matrix but augmented)
		degree = degree + 1;

		for (i = 0; i < degree; i++)                    //From now Gaussian Elimination starts(can be ignored) to solve the set of linear equations (Pivotisation)
			for (k = i + 1; k < degree; k++)
				if (B[i][i] < B[k][i])
					for (j = 0; j <= degree; j++)
					{
						float temp = B[i][j];
						B[i][j] = B[k][j];
						B[k][j] = temp;
					}

		for (i = 0; i < degree - 1; i++)            //loop to perform the gauss elimination
			for (k = i + 1; k < degree; k++)
			{
				float t = B[k][i] / B[i][i];
				for (j = 0; j <= degree; j++)
					B[k][j] = B[k][j] - t * B[i][j];    //make the elements below the pivot elements equal to zero or elimnate the variables
			}
		for (i = degree - 1; i >= 0; i--)                //back-substitution
		{                        //x is an array whose values correspond to the values of x,y,z..
			a[i] = B[i][degree];                //make the variable to be calculated equal to the rhs of the last equation
			for (j = 0; j < degree; j++)
				if (j != i)            //then subtract all the lhs values except the coefficient of the variable whose value                                   is being calculated
					a[i] = a[i] - B[i][j] * a[j];
			a[i] = a[i] / B[i][i];            //now finally divide the rhs by the coefficient of the variable to be calculated
		}
	}
	public float getY(float x)
	{
		float result = 0;
		for (int i = 0; i < degree; i++)
		{
			result += a[i] * Mathf.Pow(x, i);
		}
		return result;
	}

};
class LeastSquare
{
	float a, b;

	public LeastSquare(List<float> x, List<float> y)
	{
		float t1 = 0, t2 = 0, t3 = 0, t4 = 0;
		for (int i = 0; i < x.Count; ++i)
		{
			t1 += x[i] * x[i];
			t2 += x[i];
			t3 += x[i] * y[i];
			t4 += y[i];
		}
		a = (t3 * x.Count - t2 * t4) / (t1 * x.Count - t2 * t2);
		b = (t1 * t4 - t2 * t3) / (t1 * x.Count - t2 * t2);
	}

	public float getY(float x)
	{
		return a * x + b;
	}

};
class LeastSquareExponential
{
	float a, b;

	public LeastSquareExponential(List<float> x, List<float> y)
	{
		float t1 = 0, t2 = 0, t3 = 0, t4 = 0;
		int size = Mathf.Min(x.Count, y.Count);

		for (int i = 0; i < size; i++)
		{
			y[i] = Mathf.Log(y[i]);
		}
		for (int i = 0; i < size; ++i)
		{
			t1 = t1 + x[i];
			t2 = t2 + x[i] * x[i];
			t3 = t3 + y[i];
			t4 = t4 + x[i] * y[i];
		}
		a = ((t2 * t3 - t1 * t4) / (size * t2 - t1 * t1));
		b = ((size * t4 - t1 * t3) / (size * t2 - t1 * t1));
		a = Mathf.Exp(a);
	}
	public float getY(float x)
	{
		//y = ae^(bx)
		return a * Mathf.Exp(b * x);
	}

};
public class RoofSurfaceRidgeStruct : RidgeStruct//屋面脊
{
	public List<GameObject> flatTileModelList;              //** 平瓦實體位置
	public List<GameObject> roundTileModelList;             //** 筒瓦實體位置 (第一個位置放滴水)
	public List<GameObject> flyingRafterModelList;          //** 檐椽實體位置 
	public List<Vector3> tileUpVectorList;                  //tile的UpVector

	public RoofSurfaceRidgeStruct()
	{
		flatTileModelList = new List<GameObject>();
		roundTileModelList = new List<GameObject>();
		flyingRafterModelList = new List<GameObject>();
		tileUpVectorList = new List<Vector3>();
	}
}
public class RidgeStruct //脊
{
	public GameObject body;
	public Dictionary<string, Vector3> controlPointDictionaryList;
	public CatLine ridgeCatLine;
	public List<Vector3> tilePosList;
	public RidgeStruct()
	{
		controlPointDictionaryList = new Dictionary<string, Vector3>();
		tilePosList = new List<Vector3>();
		ridgeCatLine = new CatLine();
	}
}
public class RoofSurfaceStruct//屋面
{
	public GameObject body;
	public List<RoofSurfaceRidgeStruct> rightRoofSurfaceTileRidgeList;
	public List<RoofSurfaceRidgeStruct> leftRoofSurfaceTileRidgeList;
	public RoofSurfaceRidgeStruct midRoofSurfaceTileRidge;
	public RoofSurfaceStruct()
	{
		rightRoofSurfaceTileRidgeList = new List<RoofSurfaceRidgeStruct>();
		leftRoofSurfaceTileRidgeList = new List<RoofSurfaceRidgeStruct>();
		midRoofSurfaceTileRidge = new RoofSurfaceRidgeStruct();
	}
}
//public class RoofController : Singleton<RoofController>
public class RoofController : MonoBehaviour
{
	private BuildingObj parentObj;
	//RoofType************************************************************************************
	public MainController.RoofType roofType = MainController.RoofType.Dome;
	public List<RidgeStruct> MainRidgeList = new List<RidgeStruct>();               //** 主脊列表
	public List<RoofSurfaceStruct> SurfaceList = new List<RoofSurfaceStruct>();     //** 屋面列表
	private enum EaveControlPointType { RightControlPoint, MidRControlPoint, MidControlPoint, MidLControlPoint, LeftControlPoint };//屋簷

	private enum MainRidgeControlPointType { TopControlPoint, MidControlPoint, DownControlPoint ,EaveCtrlPoint};//主脊
    private enum MidRoofSurfaceControlPointType { MidRoofSurfaceTopPoint, MidRoofSurfaceMidPoint, MidRoofSurfaceDownPoint , EaveCtrlPoint };//屋面
    public float allJijaHeight;                     //總舉架高度(總屋頂高度)
	private float eave2eaveColumnOffset;            //主脊方向檐柱至檐出長度
	private float eave2FlyEaveOffset;
	private float beamsHeight;                      //總梁柱高度

	private float Wu_Dian_DingMainRidgeWidth;       //廡殿頂主脊長度
	private float Lu_DingMainRidgeOffset;           //盝頂垂脊長度
	private float Shya_Shan_DingMainRidgeWidth;     //歇山頂主脊長度
	private float Shya_Shan_DingSanhuaHeight;       //山花高度
	private float Shya_Shan_DingSanhuaWidth;        //山花寬度
	//Parameter**********************************************************************************
	

	public float flyEaveHeightOffset = 1.0f;        //飛簷上翹程度
	public float mainRidgeHeightOffset;             //主脊曲線上翹程度
	public float roofSurfaceHeightOffset = -1.0f;   //屋面曲線上翹程度
	public float eaveCurveHeightOffset = -3f;       //屋簷高度
	public float roofSurfaceTileWidth = 1.0f;       //屋面瓦片長度
	public float roofSurfaceTileHeight = 0.95f;     //屋面瓦片高度
	public float mainRidgeTileHeight = 0.3f;        //主脊瓦片高度

	public float flyRafterWidth = 0.3f;
	public float flyRafterHeight = 0.3f;
	//公式變數*************************************************************************************
	float anchorDis = 0f;//曲線innerPoint換算anchorPoint間距
	public bool isDownFloor;

	//******************************************************************************************


	/**
	 * 屋頂初始化
	 * parentObj : BuildingObj物件 ， columnTopPosList : 柱子位置列表 ，  topFloorBorderList : 上一層邊緣點列表
	 * platformWidth : 基底寬 ， columnHeight : 柱高 ， mainRidgeHeightOffset : 主脊高
	 * allJijaHeight : 舉架高 (用來定義roofTopCenter) ， isDownFloor : 有沒有下一層樓 ， roofType : 屋頂類別
	 */
	public void InitFunction(BuildingObj parentObj, List<Vector3> columnTopPosList, List<Vector3> topFloorBorderList, float platformWidth, float columnHeight, float mainRidgeHeightOffset, float allJijaHeight, bool isDownFloor, int roofType)
	{
		//初始值******************************************************************************
		this.parentObj = parentObj;
		this.allJijaHeight = allJijaHeight;
		this.mainRidgeHeightOffset = mainRidgeHeightOffset;
		this.isDownFloor = isDownFloor;
		SetRoofType(roofType);
		eave2eaveColumnOffset = columnHeight * 0.4f;
		eave2FlyEaveOffset = columnHeight * 0.6f;
		beamsHeight = columnHeight * 0.2f;

		parentObj.roofTopCenter = parentObj.bodyCenter + new Vector3(0, columnHeight / 2.0f + this.allJijaHeight, 0);
		ShowPos(parentObj.bodyCenter, parentObj.roof, Color.green, 1.0f);
		ShowPos(parentObj.platformCenter, parentObj.roof, Color.yellow, 1.0f);
		ShowPos(parentObj.roofTopCenter, parentObj.roof, Color.black, 1.0f);


		Wu_Dian_DingMainRidgeWidth = platformWidth * 0.5f;
		Lu_DingMainRidgeOffset = platformWidth * 0.6f;

		Shya_Shan_DingMainRidgeWidth = platformWidth * 0.3f;
		Shya_Shan_DingSanhuaHeight = allJijaHeight * 0.6f;
		Shya_Shan_DingSanhuaWidth = platformWidth * 0.2f;
		//******************************************************************************
		CreateRoof(columnTopPosList, topFloorBorderList, isDownFloor);
	}

	public void SetRoofType(int roofType)
	{
		switch (roofType)
		{
			case 0:
				this.roofType = MainController.RoofType.Zan_Jian_Ding;
				break;
			case 1:
				this.roofType = MainController.RoofType.Wu_Dian_Ding;
				break;
			case 2:
				this.roofType = MainController.RoofType.Lu_Ding;
				break;
			case 3:
				this.roofType = MainController.RoofType.Juan_Peng;
				break;
			case 4:
				this.roofType = MainController.RoofType.Shya_Shan_Ding;
				break;
		}

	}
	/**
	 * 建立控制點
	 * 輸入 : Parent GameObj、世界座標、控制點名稱
	 * 回傳 : 控制點 (GameObject)
	 * **/
	GameObject CreateControlPoint(GameObject parentObj, Vector3 worldPos, string name = "ControlPoint")//Create控制點
	{
		GameObject newControlPoint = new GameObject(name);
		newControlPoint.transform.position = worldPos;
		newControlPoint.transform.parent = parentObj.transform;
		return newControlPoint;
	}
	RoofSurfaceRidgeStruct CreateRoofSurfaceRidgeSturct(string name, GameObject parent)//初始 屋面脊
	{
		RoofSurfaceRidgeStruct newRoofSurfaceRidgeStruct = new RoofSurfaceRidgeStruct();
		newRoofSurfaceRidgeStruct.body = new GameObject(name);
		newRoofSurfaceRidgeStruct.body.transform.parent = parent.transform;

		return newRoofSurfaceRidgeStruct;
	}
	RidgeStruct CreateRidgeSturct(string name, GameObject parent)//初始 脊
	{
		RidgeStruct newRidgeStruct = new RidgeStruct();
		newRidgeStruct.body = new GameObject(name);
		newRidgeStruct.body.transform.parent = parent.transform;

		return newRidgeStruct;
	}
	RoofSurfaceStruct CreateRoofSurfaceSturct(string name, GameObject parent)//初始 脊
	{
		RoofSurfaceStruct newRoofSurfaceStruct = new RoofSurfaceStruct();
		newRoofSurfaceStruct.body = new GameObject(name);
		newRoofSurfaceStruct.body.transform.parent = parent.transform;
		newRoofSurfaceStruct.midRoofSurfaceTileRidge = new RoofSurfaceRidgeStruct();

		return newRoofSurfaceStruct;
	}
	/**
	* 建立主脊瓦片 
	*/
	RidgeStruct CreateMainRidgeTile(MainRidgeModelStruct mainRidgeModelStructGameObject, RidgeStruct mainRidgeStruct, RoofSurfaceStruct roofSurfaceStruct, RidgeStruct eaveRidgeStruct, GameObject parent)//主脊瓦片
	{
		float mainRidgeTailHeightOffset = 0.0f;

		RidgeStruct baseList = CreateRidgeSturct("MainRidgeTileStruct", parent);

		Vector3 planeNormal = Vector3.Cross(mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()] - mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()], Vector3.up).normalized;

		baseList.ridgeCatLine.controlPointPosList = mainRidgeStruct.ridgeCatLine.controlPointPosList;
		baseList.ridgeCatLine.SetLineNumberOfPoints(10000);
		baseList.ridgeCatLine.SetCatmullRom(0);

		baseList.tilePosList = baseList.ridgeCatLine.CalculateAnchorPosByInnerPointList(baseList.ridgeCatLine.anchorInnerPointlist, baseList.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, mainRidgeTileHeight);

		if (baseList.tilePosList.Count < 1) return baseList;
		//Debug.Log("baseList.tilePosList.Count>=1");
		Vector3 quaternionVector = Vector3.zero;
		for (int p = 0; p < baseList.tilePosList.Count; p++)
		{

			if (baseList.tilePosList.Count > 1)
			{
				if (p == 0)
				{
					quaternionVector = (baseList.tilePosList[0] - baseList.tilePosList[1]);
				}
				else
				{
					if ((p < baseList.tilePosList.Count - 1))
						quaternionVector = (baseList.tilePosList[p - 1] - baseList.tilePosList[p + 1]);
				}

			}
			else//如果baseList.tilePosList只有一個修正
			{
				if (baseList.ridgeCatLine.anchorInnerPointlist.Count > 0) quaternionVector = baseList.ridgeCatLine.anchorInnerPointlist[baseList.ridgeCatLine.anchorInnerPointlist.Count - 1] - baseList.ridgeCatLine.anchorInnerPointlist[0];
			}
			Quaternion rotationVector = Quaternion.LookRotation(quaternionVector.normalized);

			Vector3 upVector = (Vector3.Cross(quaternionVector, planeNormal)).normalized;
			//Vector3 offset = Vector3.zero;
			//Debug.Log("p " + p + " rotation " + rotationVector * Quaternion.Euler(mainRidgeModelStructGameObject.mainRidgeTileModelStruct.rotation));
			GameObject mainModel = Instantiate(mainRidgeModelStructGameObject.mainRidgeTileModelStruct.model, baseList.tilePosList[p] + mainRidgeTailHeightOffset * upVector, mainRidgeModelStructGameObject.mainRidgeTileModelStruct.model.transform.rotation) as GameObject;
			//GameObject mainModel = Instantiate(Resources.Load("Models/main_ridge_new")) as GameObject;
			//mainModel.transform.position = baseList.tilePosList[p] + mainRidgeTailHeightOffset * upVector;
			mainModel.transform.rotation = rotationVector * Quaternion.Euler(mainRidgeModelStructGameObject.mainRidgeTileModelStruct.rotation);
			mainModel.transform.GetChild(0).localScale = mainRidgeModelStructGameObject.mainRidgeTileModelStruct.scale;
			mainModel.transform.parent = baseList.body.transform;
		}
		MeshCenter.Instance.MeshCombineInGameObjectList(baseList.body, Resources.Load("Models/Materials/MainRidgeMat") as Material);
		return baseList;

	}

	/**
	* 建立主脊瓦片 ( 移除 RoofSurfaceStruct 與 eaveRidgeStruct 參數)
	*/
	public RidgeStruct CreateMainRidgeTile(MainRidgeModelStruct mainRidgeModelStructGameObject, RidgeStruct mainRidgeStruct, GameObject parent)//主脊瓦片
	{
		float mainRidgeTailHeightOffset = 0.0f;
		RidgeStruct baseList = CreateRidgeSturct("MainRidgeTileStruct", parentObj.roof);

		Vector3 planeNormal = Vector3.Cross(mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()] - mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()], Vector3.up).normalized;

		baseList.ridgeCatLine.controlPointPosList = mainRidgeStruct.ridgeCatLine.controlPointPosList;
		baseList.ridgeCatLine.SetLineNumberOfPoints(10000);
		baseList.ridgeCatLine.SetCatmullRom(0);

		baseList.tilePosList = baseList.ridgeCatLine.CalculateAnchorPosByInnerPointList(baseList.ridgeCatLine.anchorInnerPointlist, baseList.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, mainRidgeTileHeight);

		if (baseList.tilePosList.Count < 1) return baseList;
		Vector3 quaternionVector = Vector3.zero;
		for (int p = 0; p < baseList.tilePosList.Count; p++)
		{

			if (baseList.tilePosList.Count > 1)
			{
				if (p == 0)
				{
					quaternionVector = (baseList.tilePosList[0] - baseList.tilePosList[1]);
				}
				else
				{
					if ((p < baseList.tilePosList.Count - 1))
						quaternionVector = (baseList.tilePosList[p - 1] - baseList.tilePosList[p + 1]);
				}

			}
			else//如果baseList.tilePosList只有一個修正
			{
				if (baseList.ridgeCatLine.anchorInnerPointlist.Count > 0) quaternionVector = baseList.ridgeCatLine.anchorInnerPointlist[baseList.ridgeCatLine.anchorInnerPointlist.Count - 1] - baseList.ridgeCatLine.anchorInnerPointlist[0];
			}
			Quaternion rotationVector = Quaternion.LookRotation(quaternionVector.normalized);

			Vector3 upVector = (Vector3.Cross(quaternionVector, planeNormal)).normalized;
			//Vector3 offset = Vector3.zero;
			//Debug.Log("p " + p + " rotation " + rotationVector * Quaternion.Euler(mainRidgeModelStructGameObject.mainRidgeTileModelStruct.rotation));
			GameObject mainModel = Instantiate(mainRidgeModelStructGameObject.mainRidgeTileModelStruct.model, baseList.tilePosList[p] + mainRidgeTailHeightOffset * upVector, mainRidgeModelStructGameObject.mainRidgeTileModelStruct.model.transform.rotation) as GameObject;
			//GameObject mainModel = Instantiate(Resources.Load("Models/main_ridge_new")) as GameObject;
			//mainModel.transform.position = baseList.tilePosList[p] + mainRidgeTailHeightOffset * upVector;
			mainModel.transform.rotation = rotationVector * Quaternion.Euler(mainRidgeModelStructGameObject.mainRidgeTileModelStruct.rotation);
			mainModel.transform.GetChild(0).localScale = mainRidgeModelStructGameObject.mainRidgeTileModelStruct.scale;
			mainModel.transform.parent = parent.transform;
		}
		return baseList;

	}

	/**
	* 建立主脊瓦片 ( 移除 RoofSurfaceStruct 、 eaveRidgeStruct 參數 與 parent)
	*/
	public RidgeStruct CreateMainRidgeTile(MainRidgeModelStruct mainRidgeModelStructGameObject, RidgeStruct mainRidgeStruct)//主脊瓦片
	{
		RidgeStruct baseList = CreateRidgeSturct("MainRidgeTileStruct", parentObj.roof);

		Vector3 planeNormal = Vector3.Cross(mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()] - mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()], Vector3.up).normalized;

		baseList.ridgeCatLine.controlPointPosList = mainRidgeStruct.ridgeCatLine.controlPointPosList;
		baseList.ridgeCatLine.SetLineNumberOfPoints(10000);
		baseList.ridgeCatLine.SetCatmullRom(0);

		mainRidgeStruct.tilePosList = mainRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(baseList.ridgeCatLine.anchorInnerPointlist, baseList.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, mainRidgeTileHeight);

		if (mainRidgeStruct.tilePosList.Count < 1)
		{
			return mainRidgeStruct;
		}

		CreateTileByModelAndRidge(mainRidgeModelStructGameObject, mainRidgeStruct);
		return mainRidgeStruct;

	}

	/**
	 * 依據給定的模組與主脊製作瓦片
	 * 
	 */
	public void CreateTileByModelAndRidge(MainRidgeModelStruct mainRidgeModelStructGameObject, RidgeStruct mainRidgeStruct)
	{
		Vector3 quaternionVector = Vector3.zero;
		for (int p = 0; p < mainRidgeStruct.tilePosList.Count; p++)
		{
			if (mainRidgeStruct.tilePosList.Count > 1)
			{
				if (p == 0)
				{
					quaternionVector = (mainRidgeStruct.tilePosList[0] - mainRidgeStruct.tilePosList[1]);
				}
				else if (p < mainRidgeStruct.tilePosList.Count - 1)
				{
					quaternionVector = (mainRidgeStruct.tilePosList[p - 1] - mainRidgeStruct.tilePosList[p + 1]);
				}
			}
			else//如果baseList.tilePosList只有一個修正
			{
				if (mainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count > 0)
				{
					quaternionVector = mainRidgeStruct.ridgeCatLine.anchorInnerPointlist[mainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1] - mainRidgeStruct.ridgeCatLine.anchorInnerPointlist[0];
				}
			}
			Quaternion rotationVector = Quaternion.LookRotation(quaternionVector.normalized);
			GameObject mainModel = Instantiate(mainRidgeModelStructGameObject.mainRidgeTileModelStruct.model, mainRidgeStruct.tilePosList[p], mainRidgeModelStructGameObject.mainRidgeTileModelStruct.model.transform.rotation) as GameObject;
			mainModel.transform.rotation = rotationVector * Quaternion.Euler(mainRidgeModelStructGameObject.mainRidgeTileModelStruct.rotation);
			mainModel.transform.GetChild(0).localScale = mainRidgeModelStructGameObject.mainRidgeTileModelStruct.scale;
			mainModel.transform.parent = mainRidgeStruct.body.transform;
		}
	}

	/**
	 * 建立屋頂表面的瓦片，用相鄰的屋面脊
	 * 輸入 : roofSurfaceModelStructGameObject (欲貼上瓦片模組)、parent (Parent Obj)、baseList(欲產生瓦片的屋面脊)、refList(上一組屋面脊)、midRidgeStruct (屋面中間脊)
     * 、eaveStructList (屋簷)、mainRidgeStruct (生成方向的主脊)、roofSurfaceTileRidgeUpPointPos (屋面脊的上點位置)
	 *      
	 */

	RoofSurfaceRidgeStruct CreateRoofSurfaceTile(RoofSurfaceModelStruct roofSurfaceModelStructGameObject, GameObject parent, RoofSurfaceRidgeStruct baseList, RoofSurfaceRidgeStruct refList, Vector3 roofSurfaceRidgeXZ_Dir,Vector3 eaveDir, int dir)
	{
		//回傳tile的List
		RoofSurfaceRidgeStruct roofSurfaceRidgeStruct = baseList;

		if (refList.tilePosList.Count < 1 || baseList.tilePosList.Count < 1) return roofSurfaceRidgeStruct;


		//tile方向向量
		Vector3 quaternionVector = Vector3.zero;
		//旋轉角
		Quaternion rotationVector = Quaternion.identity;

		//flatTile向UpVector的offset
		float flatTileModelHeightOffset = -0.3f;
		//flyingRafter向UpVector的offset
		float flyingRafterHeightOffset = -1.0f;
		//*** v1 : 屋面中間脊在XZ平片上生長方向向量 (屋面中間脊控制線的上點 - 下點xz座標)
		//*** v2 : 零向量
		//*** v3 : 屋簷的生長方向向量 (屋簷終點 - 起點) * 生長方向(1:正，-1:負)
		//Vector3 v1 = new Vector3(midRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].x - midRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].x, 0, midRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].z - midRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].z);
		//Vector3 v3 = (eaveStructList.controlPointDictionaryList[EaveControlPointType.EndControlPoint.ToString()] - eaveStructList.controlPointDictionaryList[EaveControlPointType.StartControlPoint.ToString()]);
		Vector3 v1 = roofSurfaceRidgeXZ_Dir;
		Vector3 v3 = -eaveDir;
		Vector3 v2 = Vector3.zero;

		int p = 0;
		int angleChange = 1;
		int dirChange = 0;
		int dirAngleChange = 1;
		Vector3 upVector = Vector3.up;
		Vector3 pos = Vector3.zero;
		float lastXAngle = 0;
		int lastDirAngleChange = 0;
		for (; p < baseList.tilePosList.Count; p++)
		{

			GameObject mainModel;
			List<Vector3> sumVector = new List<Vector3>();
			Vector3 sum = Vector3.zero;

			if (baseList.tilePosList.Count > 1)
			{
				if (p == 0)          //**第一片屋簷瓦片
				{
					sum = Vector3.zero;
					sumVector.Clear();
					quaternionVector = (baseList.tilePosList[0] - baseList.tilePosList[1]);     //**第一片瓦片方向
					if (refList.tilePosList.Count > 1)
						sumVector.Add(refList.tilePosList[0] - refList.tilePosList[1]);         //**上一組屋面脊的第一片瓦片方向
				}
				else
				{
					sum = Vector3.zero;
					sumVector.Clear();

					if ((p < refList.tilePosList.Count - 1))
						sumVector.Add((refList.tilePosList[p - 1] - refList.tilePosList[p + 1]));//refList p
					//if ((p < refList.tilePosList.Count - 2)) sumVector.Add((refList.tilePosList[p] - refList.tilePosList[p + 2]));//refList p+1
					//if ((p > 2) && (p < refList.tilePosList.Count)) sumVector.Add((refList.tilePosList[p - 2] - refList.tilePosList[p]));//refList p-1
					if ((p < baseList.tilePosList.Count - 1))
						sumVector.Add((baseList.tilePosList[p - 1] - baseList.tilePosList[p + 1]));//baseList p
					if ((p < baseList.tilePosList.Count - 2))
						sumVector.Add((baseList.tilePosList[p] - baseList.tilePosList[p + 2]));//baseList p+1

				}

			}
			else//如果baseList.tilePosList只有一個修正
			{
				sum = Vector3.zero;
				sumVector.Clear();
				if (baseList.ridgeCatLine.anchorInnerPointlist.Count > 0) quaternionVector = baseList.ridgeCatLine.anchorInnerPointlist[baseList.ridgeCatLine.anchorInnerPointlist.Count - 1] - baseList.ridgeCatLine.anchorInnerPointlist[0];
				if (refList.tilePosList.Count > 1) sumVector.Add(refList.tilePosList[0] - refList.tilePosList[1]);
			}

			for (int i = 0; i < sumVector.Count; i++)
			{
				sum += sumVector[i];            //** 上一組屋面脊瓦片方向加總
			}
			//*** 瓦片方向 = 瓦片方向+   /向量總數
			quaternionVector = (quaternionVector + sum) / (sumVector.Count + 1);

			//*** 若瓦片方向與屋面中間脊xz向量夾角超過90度則修改方向向量 (翻轉)
			dirAngleChange = ((Mathf.Sign(Vector3.Dot(v1.normalized, quaternionVector.normalized)) < 0) ? 1 : -1);
			dirChange = ((Mathf.Sign(Vector3.Dot(v1.normalized, quaternionVector.normalized)) < 0) ? 0 : 180);

			//*** 至上一組屋脊同順序位置瓦片的方向向量 (不算最後一組)
			if (p < refList.tilePosList.Count)
				v2 = (refList.tilePosList[p] - baseList.tilePosList[p]);

			//*** 與上一組屋脊同順序位置瓦片的法向量
			upVector = (dir != 0) ? ((Vector3.Cross(quaternionVector, v2)).normalized * dir) : ((Vector3.Cross(quaternionVector, v3)).normalized);
			roofSurfaceRidgeStruct.tileUpVectorList.Add(upVector);
			//***  屋簷生長方向與法向量夾角若超過90度(反向)則需要調整
			//if (p < refList.tilePosList.Count) angleChange = ((refList.tilePosList[p].y >= baseList.tilePosList[p].y) ? -1 : 1) * dirAngleChange;
			angleChange = ((Vector3.Dot(v3.normalized * dir, upVector) <= 0) ? -1 : 1);
			float xAngle = (Vector3.Angle(v3 * dir, v2) * dir * angleChange + dirChange);
			if ((p != 0) && (lastDirAngleChange == dirAngleChange))
				xAngle = (xAngle + lastXAngle) / 2.0f;

			lastXAngle = xAngle;
			lastDirAngleChange = dirAngleChange;

			//*** 以瓦片方向向量當軸轉指定角度 * ???
			rotationVector = Quaternion.AngleAxis(-xAngle, quaternionVector.normalized) * Quaternion.LookRotation(quaternionVector.normalized);

			//*** 找出屋面中間脊與正上方的垂直向量，由此向量與中間脊上點做一個平面A，平面B由(當前瓦片位置-上個瓦片位置)形成的向量和此屋面脊上點形成
			{
				//RoundTile&EaveTile
				if (p == 0)
				{

					GameObject eaveTileModel = Instantiate(roofSurfaceModelStructGameObject.eaveTileModelStruct.model, baseList.tilePosList[0], roofSurfaceModelStructGameObject.eaveTileModelStruct.model.transform.rotation) as GameObject;
					eaveTileModel.transform.rotation = rotationVector * Quaternion.Euler(roofSurfaceModelStructGameObject.eaveTileModelStruct.rotation);
					eaveTileModel.transform.parent = parent.transform;

					roofSurfaceRidgeStruct.roundTileModelList.Add(eaveTileModel);
					mainModel = eaveTileModel;
				}
				else
				{

					GameObject roundTileModel = Instantiate(roofSurfaceModelStructGameObject.roundTileModelStruct.model, baseList.tilePosList[p], roofSurfaceModelStructGameObject.roundTileModelStruct.model.transform.rotation) as GameObject;
					roundTileModel.transform.rotation = rotationVector * Quaternion.Euler(roofSurfaceModelStructGameObject.roundTileModelStruct.rotation);
					roundTileModel.transform.parent = parent.transform;
					roofSurfaceRidgeStruct.roundTileModelList.Add(roundTileModel);
					mainModel = roundTileModel;
				}

				//FlatTile : 平瓦放置在兩筒瓦中間
				if (dir != 0)
				{
					if (p < refList.tilePosList.Count)
						pos = (baseList.tilePosList[p] + refList.tilePosList[p]) / 2.0f + flatTileModelHeightOffset * upVector;

					GameObject flatTileModel = Instantiate(roofSurfaceModelStructGameObject.flatTileModelStruct.model, pos, roofSurfaceModelStructGameObject.flatTileModelStruct.model.transform.rotation) as GameObject;

					flatTileModel.transform.rotation = rotationVector * Quaternion.Euler(roofSurfaceModelStructGameObject.flatTileModelStruct.rotation);
					if (p < refList.tilePosList.Count && p < baseList.tilePosList.Count)
					{
						float width = roofSurfaceModelStructGameObject.flatTileModelStruct.bound.size.x;
						float dis = Vector3.Distance(baseList.tilePosList[p], refList.tilePosList[p]);
						float disDiff = (dis - width);
						flatTileModel.transform.GetChild(0).localScale = new Vector3(flatTileModel.transform.GetChild(0).localScale.x * (width + disDiff) / width, flatTileModel.transform.GetChild(0).localScale.y, (flatTileModel.transform.GetChild(0).localScale.z));
					}
					flatTileModel.transform.parent = mainModel.transform;

					roofSurfaceRidgeStruct.flatTileModelList.Add(flatTileModel);
					//  flatTileModelList.Add(mainModel);

				}
			}
			//else break;
		}

		for (int i = roofSurfaceRidgeStruct.tilePosList.Count - 1; i > p; )
		{
			roofSurfaceRidgeStruct.tilePosList.RemoveAt(i);
		}

		return roofSurfaceRidgeStruct;

		//===============================          檐椽        ===========================================//
	}
	void CreateFlyRafer(RoofSurfaceStruct roofSurfaceStruct, float flyRafterWidth, float flyRafterHeight, float flyRafterHeightOffset, float rafterDisOverRoofSurfaceStruct, Vector3 rightMainRidgeRafterPos, Vector3 leftMainRidgeRafterPos)
	{
		int flyRaferLayerCount = 1;
		for (int n = 0; n < flyRaferLayerCount; n++)
		{
			//***********************************   VerticalRafter   ***********************************
			GameObject flyRafer = new GameObject("flyRafer");
			flyRafer.transform.parent = roofSurfaceStruct.body.transform;

			ListPairVector3 midPosUpVectorList = new ListPairVector3();
			List<ListPairVector3> rightPosUpVectorList = new List<ListPairVector3>();
			List<ListPairVector3> leftPosUpVectorList = new List<ListPairVector3>();
			//MidRafter
			#region  MidRafter
			{
				GameObject mFlyRafter = new GameObject("M_FlyRafer");
				MeshFilter meshFilter = mFlyRafter.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = mFlyRafter.AddComponent<MeshRenderer>();
				meshRenderer.material.color = Color.white;
				mFlyRafter.transform.parent = flyRafer.transform;
				for (int j = n; j < roofSurfaceStruct.midRoofSurfaceTileRidge.tilePosList.Count; j++)
				{
					midPosUpVectorList.Add(new PairVector3(roofSurfaceStruct.midRoofSurfaceTileRidge.tilePosList[j] + roofSurfaceStruct.midRoofSurfaceTileRidge.tileUpVectorList[j].normalized * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.midRoofSurfaceTileRidge.tileUpVectorList[j].normalized));
				}
				MeshCenter.Instance.CreateCurveCubeMesh(midPosUpVectorList.ListA, roofSurfaceStruct.midRoofSurfaceTileRidge.tileUpVectorList, flyRafterWidth, flyRafterHeight, 1, 1, meshFilter);
			}
			#endregion

			int boundaryIndex = 0;
			//RightRafter
			#region  RightRafter
			float minDis = float.MaxValue;
			for (int j = 0; j < roofSurfaceStruct.rightRoofSurfaceTileRidgeList.Count; j++)
			{
				float dis = Vector3.Distance(roofSurfaceStruct.rightRoofSurfaceTileRidgeList[j].tilePosList[roofSurfaceStruct.rightRoofSurfaceTileRidgeList[j].tilePosList.Count - 1], rightMainRidgeRafterPos);
				if (dis < minDis)
				{
					minDis = dis;
					boundaryIndex = j;
				}
			}

			for (int iIndex = 0; iIndex < roofSurfaceStruct.rightRoofSurfaceTileRidgeList.Count - 1; iIndex++)
			{
				GameObject rFlyRafter = new GameObject("R_FlyRafer");
				MeshFilter meshFilter = rFlyRafter.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = rFlyRafter.AddComponent<MeshRenderer>();
				meshRenderer.material.color = Color.white;
				rFlyRafter.transform.parent = flyRafer.transform;
				ListPairVector3 newListPairVector3 = new ListPairVector3();


				if (iIndex < boundaryIndex)
				{
					for (int j = n; j < roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tilePosList.Count; j++)
					{
						newListPairVector3.Add(new PairVector3(roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tilePosList[j] + roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[j].normalized * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[j].normalized));
					}
					MeshCenter.Instance.CreateCurveCubeMesh(newListPairVector3.ListA, roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tileUpVectorList, flyRafterWidth, flyRafterHeight, 1, 1, meshFilter);
				}
				else
				{
					List<Vector3> upVectorList = new List<Vector3>();
					int lastRidge = (boundaryIndex);
					int lastRidgeTileCount = roofSurfaceStruct.rightRoofSurfaceTileRidgeList[lastRidge].tilePosList.Count - 1;
					int size = Mathf.Min(n, roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tileUpVectorList.Count - 1);

					newListPairVector3.Add(new PairVector3(roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tilePosList[size] + roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[size].normalized * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[size].normalized));
					upVectorList.Add(roofSurfaceStruct.rightRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[size].normalized);

					//newListPairVector3.Add(new PairVector3(roofSurfaceStruct.rightRoofSurfaceTileRidgeList[lastRidge].tilePosList[lastRidgeTileCount] + roofSurfaceStruct.rightRoofSurfaceTileRidgeList[lastRidge].tileUpVectorList[lastRidgeTileCount].normalized * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.rightRoofSurfaceTileRidgeList[lastRidge].tileUpVectorList[lastRidgeTileCount].normalized));
					newListPairVector3.Add(new PairVector3(rightMainRidgeRafterPos + Vector3.up * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.rightRoofSurfaceTileRidgeList[lastRidge].tileUpVectorList[lastRidgeTileCount].normalized));
					upVectorList.Add(roofSurfaceStruct.rightRoofSurfaceTileRidgeList[lastRidge].tileUpVectorList[lastRidgeTileCount].normalized);


					MeshCenter.Instance.CreateCurveCubeMesh(newListPairVector3.ListA, upVectorList, flyRafterWidth, flyRafterHeight, 0, 0, meshFilter);

				}


				rightPosUpVectorList.Add(newListPairVector3);
			}
			#endregion
			//LeftRafer
			#region  LeftRafter
			boundaryIndex = 0;
			minDis = float.MaxValue;
			for (int j = 0; j < roofSurfaceStruct.leftRoofSurfaceTileRidgeList.Count; j++)
			{
				float dis = Vector3.Distance(roofSurfaceStruct.leftRoofSurfaceTileRidgeList[j].tilePosList[roofSurfaceStruct.leftRoofSurfaceTileRidgeList[j].tilePosList.Count - 1], leftMainRidgeRafterPos);
				if (dis < minDis)
				{
					minDis = dis;
					boundaryIndex = j;
				}
			}


			for (int iIndex = 0; iIndex < roofSurfaceStruct.leftRoofSurfaceTileRidgeList.Count - 1; iIndex++)
			{
				GameObject lFlyRafter = new GameObject("L_FlyRafer");
				MeshFilter meshFilter = lFlyRafter.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = lFlyRafter.AddComponent<MeshRenderer>();
				meshRenderer.material.color = Color.white;
				lFlyRafter.transform.parent = flyRafer.transform;
				ListPairVector3 newListPairVector3 = new ListPairVector3();

				if (iIndex < boundaryIndex)
				{
					for (int j = n; j < roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tilePosList.Count; j++)
					{
						newListPairVector3.Add(new PairVector3(roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tilePosList[j] + roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[j].normalized * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[j].normalized));
					}
					MeshCenter.Instance.CreateCurveCubeMesh(newListPairVector3.ListA, roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tileUpVectorList, flyRafterWidth, flyRafterHeight, 1, 1, meshFilter);
				}
				else
				{
					List<Vector3> upVectorList = new List<Vector3>();
					int lastRidge = (boundaryIndex);
					int lastRidgeTileCount = roofSurfaceStruct.leftRoofSurfaceTileRidgeList[lastRidge].tilePosList.Count - 1;
					int size = Mathf.Min(n, roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tileUpVectorList.Count - 1);

					newListPairVector3.Add(new PairVector3(roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tilePosList[size] + roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[size].normalized * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[size].normalized));
					upVectorList.Add(roofSurfaceStruct.leftRoofSurfaceTileRidgeList[iIndex].tileUpVectorList[size].normalized);
					//newListPairVector3.Add(new PairVector3(roofSurfaceStruct.leftRoofSurfaceTileRidgeList[lastRidge].tilePosList[lastRidgeTileCount] + roofSurfaceStruct.leftRoofSurfaceTileRidgeList[lastRidge].tileUpVectorList[lastRidgeTileCount].normalized * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.leftRoofSurfaceTileRidgeList[lastRidge].tileUpVectorList[lastRidgeTileCount].normalized));
					newListPairVector3.Add(new PairVector3(leftMainRidgeRafterPos + Vector3.up * flyRafterHeightOffset * (n + 1), roofSurfaceStruct.leftRoofSurfaceTileRidgeList[lastRidge].tileUpVectorList[lastRidgeTileCount].normalized));
					upVectorList.Add(roofSurfaceStruct.leftRoofSurfaceTileRidgeList[lastRidge].tileUpVectorList[lastRidgeTileCount].normalized);


					MeshCenter.Instance.CreateCurveCubeMesh(newListPairVector3.ListA, upVectorList, flyRafterWidth, flyRafterHeight, 0, 0, meshFilter);
				}


				leftPosUpVectorList.Add(newListPairVector3);
			}
			#endregion
			//***********************************   HorizontalRafter   ***********************************
			List<ListPairVector3> totalPosList = new List<ListPairVector3>();
			rightPosUpVectorList.Reverse();
			totalPosList.AddRange(rightPosUpVectorList);
			totalPosList.Add(midPosUpVectorList);
			totalPosList.AddRange(leftPosUpVectorList);
			for (int iIndex = 0; (iIndex < roofSurfaceStruct.midRoofSurfaceTileRidge.tilePosList.Count); iIndex++)
			{
				//間隔3個tile距離才放置凜
				if ((iIndex % (roofSurfaceStruct.leftRoofSurfaceTileRidgeList[boundaryIndex].tilePosList.Count) != 0)) continue;

				GameObject vFlyRafter = new GameObject("Lin");
				MeshFilter meshFilter = vFlyRafter.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = vFlyRafter.AddComponent<MeshRenderer>();
				meshRenderer.material.color = Color.white;
				vFlyRafter.transform.parent = flyRafer.transform;
				ListPairVector3 newListPairVector3 = new ListPairVector3();
				for (int j = 0; j < totalPosList.Count; j++)
				{
					if (iIndex < totalPosList[j].ListA.Count)
					{
						newListPairVector3.Add(new PairVector3(totalPosList[j].ListA[iIndex] + flyRafterHeight * totalPosList[j].ListB[iIndex], totalPosList[j].ListB[iIndex]));
					}
				}

				MeshCenter.Instance.CreateCurveCubeMesh(newListPairVector3.ListA, newListPairVector3.ListB, flyRafterWidth, flyRafterHeight, 1, 1, meshFilter);
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

	/**
	 * 輸入 : plane平面、list線段中連續點位置、startIndex搜尋起始點、endIndex搜尋終點、threshold距離門檻值
	 * 輸出 : list線段連續點位置中最接近平面之點index
	 */
	int FindNearestPointInList2Plane(Plane plane, List<Vector3> list, int startIndex, int endIndex, float threshold = 0)
	{
		float pointMinDis2Plane = float.MaxValue;
		int index = startIndex;
		int dir = ((endIndex - startIndex) > 0 ? 1 : -1);
		for (int i = startIndex; ((dir == 1) ? (i < endIndex) : (i > endIndex)); i += dir)
		{
			float dis = Mathf.Abs(plane.GetDistanceToPoint(list[i]));
			if (threshold == 0)
			{
				if ((dis < pointMinDis2Plane))
				{
					pointMinDis2Plane = dis;
					index = i;
				}
			}
			else
			{
				if (dis < threshold)
				{
					if ((dis < pointMinDis2Plane))
					{
						pointMinDis2Plane = dis;
						index = i;
					}

				}

			}

		}
		return index;

	}

	int FindNearestPointInList2Plane(Plane plane, List<Vector3> list, int startIndex, int endIndex, Plane constraintPlane, Ray intersectionRay, float constraintDis = 0)
	{
		float pointMinDis2Plane = float.MaxValue;

		int index = startIndex;
		int dir = ((endIndex - startIndex) > 0 ? 1 : -1);

		for (int i = startIndex; ((dir == 1) ? (i < endIndex) : (i > endIndex)); i += dir)
		{
			float dis = Mathf.Abs(plane.GetDistanceToPoint(list[i]));
			float rayDistance = 0;
			if ((constraintPlane.Raycast(intersectionRay, out rayDistance)))
			{
				if (((rayDistance <= constraintDis) && (constraintDis != 0)) || (constraintDis == 0))
				{
					if ((dis < pointMinDis2Plane))
					{
						pointMinDis2Plane = dis;
						index = i;
					}
				}
			}
		}
		return index;

	}
	/**
	 * 找出列表中與指定點有最短距離的索引值
	 * 輸入 : 指定點、點列表、起終點Index
	 * 輸出 : 列表中與指定點最短距離的索引值
	 */
	int FindNearestPointInList2Point(Vector3 point, List<Vector3> list, int startIndex, int endIndex)
	{
		float pointMinDis2Plane = float.MaxValue;

		int index = startIndex;
		int dir = ((endIndex - startIndex) > 0 ? 1 : -1);
		for (int i = startIndex; ((dir == 1) ? (i < endIndex) : (i > endIndex)); i += dir)
		{
			float dis = Vector3.Magnitude((list[i] - point));
			if (dis < pointMinDis2Plane)
			{
				pointMinDis2Plane = dis;
				index = i;
			}
		}
		return index;
	}

    void FindPointOnPlane(Plane plane , Vector3 point)
    {
        Vector3 projPoint = Vector3.ProjectOnPlane(point, plane.normal);
    }

    float CurveInnerPointDis(RidgeStruct list)
	{
		float dis = 0;
		for (int i = 0; i < list.ridgeCatLine.anchorInnerPointlist.Count - 1; i++)
		{
			dis += Vector3.Distance(list.ridgeCatLine.anchorInnerPointlist[i], list.ridgeCatLine.anchorInnerPointlist[i + 1]);
		}
		return dis;
	}

	/**
	 *  建立主脊結構 (用屋頂頂端控制點與圓柱頂端位置)
	 *  輸入:圓柱列表索引、屋頂頂端控制點位置
	 *  流程:建立頭尾與中間控制點後加至控制點列表中，並把曲線的控制點也設為相同點，之後設定畫曲線點數
	 *  
	 */
	/**
	* 建立主脊結構
	* 輸入:頭尾兩控制點與曲率系數
	*/
	RidgeStruct CreateMainRidgeStruct(Vector3 topControlPointPos, Vector3 botControlPointPos, float CurveRate = 1.0f)
	{
		RidgeStruct newRidgeStruct = CreateRidgeSturct("MainRidge", parentObj.roof);

		newRidgeStruct.controlPointDictionaryList.Add(MainRidgeControlPointType.TopControlPoint.ToString(), topControlPointPos);
		newRidgeStruct.controlPointDictionaryList.Add(MainRidgeControlPointType.DownControlPoint.ToString(), botControlPointPos);

		float ratio = 0.5f;
		//Vector3 midControlPointPos = (topControlPointPos * (1 - ratio) + botControlPointPos * ratio) ;
		Vector3 midControlPointPos = (topControlPointPos * (1 - ratio) + botControlPointPos * ratio) + mainRidgeHeightOffset * Vector3.up * CurveRate;
		newRidgeStruct.controlPointDictionaryList.Add(MainRidgeControlPointType.MidControlPoint.ToString(), midControlPointPos);

		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(topControlPointPos);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(midControlPointPos);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(botControlPointPos);
		// 參考用
		//ShowPos(topControlPointPos, newRidgeStruct.body, Color.red, 1f);
		//ShowPos(midControlPointPos, newRidgeStruct.body, Color.yellow, 1f);
		//ShowPos(botControlPointPos, newRidgeStruct.body, Color.red, 1f);

		newRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(1000);
		newRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);
		// 參考用
		//         for (int i = 0; i < newRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; i++)
		//         {
		//             ShowPos(newRidgeStruct.ridgeCatLine.anchorInnerPointlist[i], newRidgeStruct.body, Color.green);
		//         }

		return newRidgeStruct;
	}
    /**
	* 建立主脊結構 - 4個控制點版本
	* 輸入 : 控制點列表 (必須要由上往下依序放入)
	*/
    RidgeStruct CreateMainRidgeStruct4Point(List<Vector3> ctrlPtList)
    {
        RidgeStruct newRidgeStruct = CreateRidgeSturct("MainRidge", parentObj.roof);

		newRidgeStruct.controlPointDictionaryList.Add(MainRidgeControlPointType.TopControlPoint.ToString(), ctrlPtList[0]);
		newRidgeStruct.controlPointDictionaryList.Add(MainRidgeControlPointType.MidControlPoint.ToString(), ctrlPtList[1]);
		newRidgeStruct.controlPointDictionaryList.Add(MainRidgeControlPointType.DownControlPoint.ToString(), ctrlPtList[2]);
		newRidgeStruct.controlPointDictionaryList.Add(MainRidgeControlPointType.EaveCtrlPoint.ToString(), ctrlPtList[3]);

        newRidgeStruct.ridgeCatLine.controlPointPosList.Add(ctrlPtList[0]);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(ctrlPtList[1]);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(ctrlPtList[2]);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(ctrlPtList[3]);
        newRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(Define.Large);
        newRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);
        
        return newRidgeStruct;
    }

    /**
	 * 建立屋簷結構
	 * 輸入 : 右左兩主脊
	 * 流程 : 以右側主脊方向為屋簷生長方向，建立頭尾與中間控制點後，再建立中間至尾端的中間控制點(為了控制屋簷由平坦到上翹的部分)
	 *        ，依序加入至曲線控制點後設定畫線點數，
	 */
	RidgeStruct CreateEaveStruct(RidgeStruct mainRidgeStruct)
	{

		RidgeStruct newRidgeStruct = CreateRidgeSturct("Eave", parentObj.roof);


		Vector3 controlPointPos = mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()];
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.MidControlPoint.ToString(), controlPointPos);

		float radius = Vector3.Distance(new Vector3(mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()].x, 0, mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()].z), new Vector3(mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].x, 0, mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].z));

		int nbSides=1000;
		float _2pi = Mathf.PI * 2f;
		for (int i = 0; i < nbSides; i++)
		{
			float rad = (float)i / nbSides * _2pi;
			Vector3 pos = (new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius) + new Vector3(mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].x, mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()].y, mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].z));

			newRidgeStruct.ridgeCatLine.controlPointPosList.Add(pos);
			//ShowPos(pos, newRidgeStruct.body,Color.red);
		}
		newRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(100);
		newRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);

		return newRidgeStruct;
	}
    RidgeStruct CreateEaveStruct(RidgeStruct RightMainRidgeStruct, RidgeStruct LeftMainRidgeStruct)
	{

		RidgeStruct newRidgeStruct = CreateRidgeSturct("Eave", parentObj.roof);


		//StartControlPoint
		Vector3 rightControlPointPos = RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()];
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.RightControlPoint.ToString(), rightControlPointPos);
		//EndControlPoint
		Vector3 leftControlPointPos = LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()];
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.LeftControlPoint.ToString(), leftControlPointPos);
		//MidControlPoint
		Vector3 midControlPointPos = (rightControlPointPos + leftControlPointPos) / 2.0f + eaveCurveHeightOffset * Vector3.up;
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.MidControlPoint.ToString(), midControlPointPos);
		//MidRightControlPoint
		Vector3 midRControlPointPos = rightControlPointPos - (rightControlPointPos - leftControlPointPos).normalized * eave2FlyEaveOffset + eaveCurveHeightOffset * Vector3.up;
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.MidRControlPoint.ToString(), midRControlPointPos);
		//MidLeftControlPoint
		Vector3 midLControlPointPos = leftControlPointPos + (rightControlPointPos - leftControlPointPos).normalized * eave2FlyEaveOffset + eaveCurveHeightOffset * Vector3.up;
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.MidLControlPoint.ToString(), midLControlPointPos);


		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(rightControlPointPos);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(midRControlPointPos);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(midControlPointPos);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(midLControlPointPos);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(leftControlPointPos);

		newRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(100);
		newRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);
        //修正................不明白為啥需要修正?有請Tony哥
        int midRControlPointIndex = 0;

		midRControlPointIndex = FindNearestPointInList2Point(midRControlPointPos, newRidgeStruct.ridgeCatLine.anchorInnerPointlist, midRControlPointIndex, newRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1);

		for (int n = midRControlPointIndex; n < newRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - midRControlPointIndex; n++)
		{
			float reviseHeight = midControlPointPos.y;
			newRidgeStruct.ridgeCatLine.anchorInnerPointlist[n] = new Vector3(newRidgeStruct.ridgeCatLine.anchorInnerPointlist[n].x, reviseHeight, newRidgeStruct.ridgeCatLine.anchorInnerPointlist[n].z);
		}

		return newRidgeStruct;
	}
    RidgeStruct CreateEaveStruct4Point(RidgeStruct RightMainRidgeStruct, RidgeStruct LeftMainRidgeStruct)
    {

        RidgeStruct newRidgeStruct = CreateRidgeSturct("Eave", parentObj.roof);

		Vector3 rightCtrlPointPos = RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()];
		Vector3 leftCtrlPointPos = LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()];
		Vector3 eaveDir = (rightCtrlPointPos - leftCtrlPointPos).normalized;
		float eaveDist = (rightCtrlPointPos - leftCtrlPointPos).magnitude;
		Vector3 midCtrlPointPos = (rightCtrlPointPos + leftCtrlPointPos) / 2 + eaveCurveHeightOffset * Vector3.up;
        Vector3 midRCtrlPointPos = midCtrlPointPos + 0.35f * eaveDir * eaveDist;
        Vector3 midLCtrlPointPos = midCtrlPointPos - 0.35f * eaveDir * eaveDist;
        //Vector3 midRCtrlPointPos = midCtrlPointPos + eaveDir * eave2FlyEaveOffset;
        //Vector3 midLCtrlPointPos = midCtrlPointPos - eaveDir * eave2FlyEaveOffset;


		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.RightControlPoint.ToString(), rightCtrlPointPos);
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.LeftControlPoint.ToString(), leftCtrlPointPos);
		//newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.MidControlPoint.ToString(), midCtrlPointPos);   
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.MidRControlPoint.ToString(), midRCtrlPointPos);
		newRidgeStruct.controlPointDictionaryList.Add(EaveControlPointType.MidLControlPoint.ToString(), midLCtrlPointPos);

		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(rightCtrlPointPos);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(midRCtrlPointPos);
		//newRidgeStruct.ridgeCatLine.controlPointPosList.Add(midCtrlPoint);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(midLCtrlPointPos);
		newRidgeStruct.ridgeCatLine.controlPointPosList.Add(leftCtrlPointPos);

        newRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(Define.Medium);
        newRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);

        //for (int iIndex = 0; iIndex < newRidgeStruct.ridgeCatLine.innerPointList.Count; iIndex++)
        //{
        //    MainController.ShowPos(newRidgeStruct.ridgeCatLine.innerPointList[iIndex], parentObj.roof, Color.green, 0.5f);
        //}

        return newRidgeStruct;
    }
    /**
	* 建造屋頂表面
	* 輸入: 右側主脊、左側主脊、屋簷
	* 輸出: 屋頂表面結構
	* 流程: 
	* 1.制定出控制線
    * 2.做出屋面中間
    * 3.製作左右邊
	**/
    private RoofSurfaceStruct CreateRoofSurfaceA(RidgeStruct RightMainRidgeStruct, RidgeStruct LeftMainRidgeStruct, RidgeStruct eaveStruct)
	{

		//for (int i = 0; i < (int)MainController.Instance.sides; i++)

		RoofSurfaceStruct newRoofSurfaceStruct = CreateRoofSurfaceSturct("RoofSurface", parentObj.roof);

		RoofSurfaceRidgeStruct newMidRidgeStruct = CreateRoofSurfaceRidgeSturct("MidRoofSurfaceTileRidge", newRoofSurfaceStruct.body);


		/***************************************找兩個mainRidge中間垂直的roofSurfaceTileRidge***********************************************************/
		//FindMidRoofSurfaceMidPoint : 左右主脊的中間控制點 - 上方控制點找出方向向量，找出兩向量夾角
		Vector2 v1 = new Vector2(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].x - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].x, LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].z - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].z);
		Vector2 v2 = new Vector2(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].x - RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].x, RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].z - RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].z);

		float angle = Vector2.Angle(v1, v2);

		//*************** 1 ******************//
		//*** 找出屋頂表面中間控制線上中下點 ***//
		//************************************//
		//midRoofSurfaceTopPoint
		Vector3 midRoofSurfaceTopPointPos = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()] + LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]) / 2.0f;
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString(), midRoofSurfaceTopPointPos);
		//midRoofSurfaceDownPoint
		Vector3 midRoofSurfaceDownPointPos = eaveStruct.controlPointDictionaryList[EaveControlPointType.MidControlPoint.ToString()];
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString(), midRoofSurfaceDownPointPos);
		//midRoofSurfaceMidPoint : 左右兩主脊的中點位置相加除2 + (-2 * 垂直左右主脊的單位向量 ) (04/19 - 修改垂直左右主脊的單位向量公式)
		//Vector3 midRoofSurfaceMidPointPos = (Quaternion.Euler(0, angle / 2.0f, 0) * (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].transform.position)) + (roofSurfaceHeightOffset * Vector3.up) - (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].transform.position - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].transform.position) / 2.0f;

		Vector3 midRoofSurfaceMidPointPos = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()] +
											LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()] + eaveStruct.controlPointDictionaryList[EaveControlPointType.MidLControlPoint.ToString()] + eaveStruct.controlPointDictionaryList[EaveControlPointType.MidRControlPoint.ToString()] + midRoofSurfaceTopPointPos + midRoofSurfaceDownPointPos) / 6.0f +
											(roofSurfaceHeightOffset * Vector3.Cross((RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] -
											RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]),
											(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]-
											LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()])).normalized);

		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceMidPoint.ToString(), midRoofSurfaceMidPointPos);

		//MidRoofSurfaceTileRidge
		newMidRidgeStruct.ridgeCatLine.controlPointPosList.Add(midRoofSurfaceTopPointPos);
		newMidRidgeStruct.ridgeCatLine.controlPointPosList.Add(midRoofSurfaceMidPointPos);
		newMidRidgeStruct.ridgeCatLine.controlPointPosList.Add(midRoofSurfaceDownPointPos);
		newMidRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(1000);
		newMidRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);
		//ShowPos(midRoofSurfaceMidPointPos, newMidRidgeStruct.body, new Color(1, 0, 1), 1f);

		/***************************************用AnchorLength取MidRoofSurfaceTileRidge上的瓦片************************************************************/
		//*** 屋頂表面中間產瓦片 ***//
		newMidRidgeStruct.tilePosList = newMidRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist, newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);

		Vector3 roofSurfaceRidgeXZ_Dir = new Vector3(newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].x - newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].x, 0, newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].z - newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].z);
		Vector3 eaveDir = (eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()]);
		newMidRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newMidRidgeStruct.body, newMidRidgeStruct, newMidRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, 0);

		//MeshCenter.Instance.MeshCombineInGameObjectList(newMidRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);

		/***************************************由Eave上的AnchorPoint切割平面 用於切出其他垂直的roofSurfaceTileRidge(右側)***********************************************************/
		//*** 做一個右半邊屋簷的瘠並將parent設為屋頂表面，並依照瓦片寬度算出內部錨點
		RidgeStruct eaveRightRidgeStruct = CreateRidgeSturct("EaveRightRidgeStruct", newRoofSurfaceStruct.body);
		int iHalfEventPointCount = eaveStruct.ridgeCatLine.anchorInnerPointlist.Count / 2 + 1;
		for (int k = 0; k < iHalfEventPointCount; k++)
		{
			eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Add(eaveStruct.ridgeCatLine.anchorInnerPointlist[k]);
		}
		eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist = eaveRightRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist, eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileWidth);

		//for (int iIndex = 0; iIndex < eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; iIndex++)
		//{
		//    print("anchorInnerPointlist["+iIndex+"] : "+ eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[iIndex]);
		//}

		//VerCutPlane
		//*** 新增三個平面，將傳入的屋簷主脊用該起終點位置相減取得的向量定為法向量並正規化
		//*** verticalCutPlane : 算屋頂表面子瘠用輔助平面 
		//*** verticalMirrorPlane : 以屋簷主脊中點與剛剛的法向量形成一平面
		//*** surfacePlane : 疑問?貌似沒用到
		Plane verticalCutPlane = new Plane();
		Plane verticalMirrorPlane = new Plane();
		Plane surfacePlane = new Plane();
		Vector3 verticalCutPlaneNormal = ((eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()])).normalized;
		verticalMirrorPlane.SetNormalAndPosition(verticalCutPlaneNormal, midRoofSurfaceDownPointPos);

		//(右邊主瘠的上下控制點找出方向向量) 外積 (屋簷中點與右邊主脊上控制點找出方向向量)
		Vector3 surfacePlaneNormal = Vector3.Cross(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] - RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()], midRoofSurfaceDownPointPos - RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]).normalized;
		surfacePlane.SetNormalAndPosition(surfacePlaneNormal, midRoofSurfaceMidPointPos);
		//紀錄前一次Index用於迴圈加速
		int roofSurfaceMidPointStartingIndex_R = 0;
		int roofSurfaceMidPointStartingIndex_R_A = 0;
		int roofSurface2MainRidgeStartingIndex_R = 0;

		RoofSurfaceRidgeStruct lastRightRidgeStruct = newMidRidgeStruct;
		RoofSurfaceRidgeStruct lastLeftRidgeStruct = newMidRidgeStruct;

		// Tony哥的由屋頂中間控制線至屋簷的半圓取線
		CatLine roofSurfaceMidPointLine = new CatLine();
		roofSurfaceMidPointLine.controlPointPosList.Add(eaveStruct.controlPointDictionaryList[EaveControlPointType.MidLControlPoint.ToString()]);
		roofSurfaceMidPointLine.controlPointPosList.Add(midRoofSurfaceMidPointPos);
		roofSurfaceMidPointLine.controlPointPosList.Add(eaveStruct.controlPointDictionaryList[EaveControlPointType.MidRControlPoint.ToString()]);
		roofSurfaceMidPointLine.SetLineNumberOfPoints(1000);
		roofSurfaceMidPointLine.SetCatmullRom(anchorDis);

		//中間主脊控制線中點至右邊主脊下方端點之連線
		CatLine roofSurfaceMidPointLineA = new CatLine();
		roofSurfaceMidPointLineA.controlPointPosList.Add(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
		roofSurfaceMidPointLineA.controlPointPosList.Add(midRoofSurfaceMidPointPos);
		roofSurfaceMidPointLineA.controlPointPosList.Add(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
		roofSurfaceMidPointLineA.SetLineNumberOfPoints(1000);
		roofSurfaceMidPointLineA.SetCatmullRom(anchorDis);

		ShowPos(midRoofSurfaceMidPointPos, newMidRidgeStruct.body, Color.green, 1.0f);
		ShowPos(eaveStruct.controlPointDictionaryList[EaveControlPointType.MidLControlPoint.ToString()], newMidRidgeStruct.body, Color.green, 1.0f);
		ShowPos(eaveStruct.controlPointDictionaryList[EaveControlPointType.MidRControlPoint.ToString()], newMidRidgeStruct.body, Color.green, 1.0f);
		// 		 	for (int f = 0; f < roofSurfaceMidPointLineA.anchorInnerPointlist.Count; f++)
		// 		 	{
		// 		 		ShowPos(roofSurfaceMidPointLineA.anchorInnerPointlist[f], newMidRidgeStruct.body, Color.green, 0.1f);
		// 			}
		//Right&LeftRoofSurfaceTileRidgeList
		for (int n = 1; n < eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; n++)
		{

			verticalCutPlane.SetNormalAndPosition(verticalCutPlaneNormal, eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n]);

			//兩個錨點間方向向量對屋簷主脊上的投影(這樣不就是=兩個錨點間方向向量?)
			Vector3 planeOffsetVector = Vector3.Project((eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n] - eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[0]),
				(eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()]));
			//Right
			RoofSurfaceRidgeStruct newRightRidgeStruct = CreateRoofSurfaceRidgeSturct("RightRoofSurfaceTileRidge", newRoofSurfaceStruct.body);
			RoofSurfaceRidgeStruct newLeftRidgeStruct = CreateRoofSurfaceRidgeSturct("LeftRoofSurfaceTileRidge", newRoofSurfaceStruct.body);

			//*** 屋頂表面放置瓦片用脊
			Vector3 roofSurfaceTileRidgeUpPointPos = Vector3.zero;
			Vector3 roofSurfaceTileRidgeMidPointPos = Vector3.zero;
			Vector3 roofSurfaceTileRidgeDownPointPos = Vector3.zero;

			Vector3 lastRoofSurfaceTileRidgeUpPointPos = Vector3.zero;
			Vector3 lastRoofSurfaceTileRidgeMidPointPos = Vector3.zero;
			Vector3 lastRoofSurfaceTileRidgeDownPointPos = Vector3.zero;


			//FindPointOnMainRidgeCloser2Plane : 找主脊點上離輔助平面最近的點，若沒有的話(ex:無殿頂的主脊需延伸一段後開始)直接取中間控制線上點位移一個屋瓦寬度的位置 
			roofSurface2MainRidgeStartingIndex_R = FindNearestPointInList2Plane(verticalCutPlane, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist, roofSurface2MainRidgeStartingIndex_R, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0.01f);
			roofSurfaceTileRidgeUpPointPos = (roofSurface2MainRidgeStartingIndex_R != 0) ? RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist[roofSurface2MainRidgeStartingIndex_R] : midRoofSurfaceTopPointPos + planeOffsetVector;

			//FindPointOnEaveCloser2Plane : 屋簷主脊目前錨點本身
			roofSurfaceTileRidgeDownPointPos = eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n];


			//FindPointOnRoofSurfaceMidPointLineCloser2Plane
			roofSurfaceMidPointStartingIndex_R = FindNearestPointInList2Plane(verticalCutPlane, roofSurfaceMidPointLine.anchorInnerPointlist, roofSurfaceMidPointStartingIndex_R, roofSurfaceMidPointLine.anchorInnerPointlist.Count - 1);
			roofSurfaceMidPointStartingIndex_R_A = FindNearestPointInList2Plane(verticalCutPlane, roofSurfaceMidPointLineA.anchorInnerPointlist, roofSurfaceMidPointStartingIndex_R_A, roofSurfaceMidPointLineA.anchorInnerPointlist.Count - 1);

			//*** ratioA : 目前偵測點佔整個右側屋簷的比例。 ratioC:暫未用到
			//float ratioC = ((float)Mathf.Abs(n - eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count / 2) / ((eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1 - eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count / 2)));
			float ratioA = ((float)(n - 1) / ((eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1)));


			//*** (舊)屋頂表面放置瓦片用脊的中點，依照比例取半圓取線與切平面焦點位置 + 瓦片用脊上下兩點之中點位置
			//*** 屋頂表面放置瓦片用脊的中點，只用全屋簷中間點曲線
			roofSurfaceTileRidgeMidPointPos = (roofSurfaceMidPointLineA.anchorInnerPointlist[roofSurfaceMidPointStartingIndex_R_A]) * (1.0f - ratioA) + ((roofSurfaceTileRidgeUpPointPos + roofSurfaceTileRidgeDownPointPos) / 2.0f) * ratioA;
			//roofSurfaceTileRidgeMidPointPos = (roofSurfaceTileRidgeUpPointPos + roofSurfaceTileRidgeDownPointPos)/2.0f;
			//*** 調整(感覺怪怪) 瓦片用脊的中點Y座標，依照比例取半圓曲線與切平面焦點位置 + 全屋簷中間點曲線與切平面焦點位置 + 瓦片用脊上下兩點之中點位置

			roofSurfaceTileRidgeMidPointPos.y = (roofSurfaceMidPointLine.anchorInnerPointlist[roofSurfaceMidPointStartingIndex_R].y) * (1.0f - ratioA) + ((roofSurfaceTileRidgeUpPointPos.y + roofSurfaceTileRidgeDownPointPos.y) / 2.0f) * ratioA;

			//roofSurfaceTileRidgeMidPointPos = (roofSurfaceMidPointLine.anchorInnerPointlist[roofSurfaceMidPointStartingIndex_R] * (1.0f - ratioA) + roofSurfaceMidPointLineA.anchorInnerPointlist[roofSurfaceMidPointStartingIndex_R_A] * ratioA) * (1.0f - ratioA) + ((roofSurfaceTileRidgeUpPointPos + roofSurfaceTileRidgeDownPointPos) / 2.0f) * ratioA;

			//觀察用
			/*
			ShowPos(roofSurfaceTileRidgeUpPointPos, newLeftRidgeStruct.body, Color.white, 0.8f);
			ShowPos(roofSurfaceTileRidgeMidPointPos, newLeftRidgeStruct.body, Color.white, 0.8f);
			ShowPos(roofSurfaceTileRidgeDownPointPos, newLeftRidgeStruct.body, Color.white, 0.8f);
			*/
			//設定屋簷表面子瘠，並依照所占屋簷百分比設定點數
			roofSurfaceTileRidgeUpPointPos += verticalCutPlane.GetDistanceToPoint(roofSurfaceTileRidgeUpPointPos) * -verticalCutPlaneNormal;
			roofSurfaceTileRidgeMidPointPos += verticalCutPlane.GetDistanceToPoint(roofSurfaceTileRidgeMidPointPos) * -verticalCutPlaneNormal;
			roofSurfaceTileRidgeDownPointPos += verticalCutPlane.GetDistanceToPoint(roofSurfaceTileRidgeDownPointPos) * -verticalCutPlaneNormal;
			newRightRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceTileRidgeUpPointPos);
			newRightRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceTileRidgeMidPointPos);
			newRightRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceTileRidgeDownPointPos);
			newRightRidgeStruct.ridgeCatLine.SetLineNumberOfPoints((int)(1000 * (1 - ratioA)));
			newRightRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);

			newLeftRidgeStruct.ridgeCatLine.controlPointPosList.Add(Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeUpPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeUpPointPos);
			newLeftRidgeStruct.ridgeCatLine.controlPointPosList.Add(Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeMidPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeMidPointPos);
			newLeftRidgeStruct.ridgeCatLine.controlPointPosList.Add(Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeDownPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeDownPointPos);
			newLeftRidgeStruct.ridgeCatLine.SetLineNumberOfPoints((int)(1000 * (1 - ratioA)));
			newLeftRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);

			/***************************************用AnchorLength取roofSurfaceTileRidge上的瓦片************************************************************/
			/*
			 for (int f = 0; f < newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; f++)
						{
							ShowPos(newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[f], newRightRidgeStruct.body, Color.green, 0.1f);
						}
			 */

			//設定瓦片位置
			newRightRidgeStruct.tilePosList = newRightRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist, newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);
			newLeftRidgeStruct.tilePosList = newLeftRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist, newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);
			/*
				for (int f = 0; f < newLeftRidgeStruct.tilePosList.Count; f++)
				{
					ShowPos(newLeftRidgeStruct.tilePosList[f], newLeftRidgeStruct.body, Color.blue, 0.3f);
				}
				for (int f = 0; f < newRightRidgeStruct.tilePosList.Count; f++)
				{
					ShowPos(newRightRidgeStruct.tilePosList[f], newRightRidgeStruct.body, Color.blue, 0.3f);
				}
				*/
			//Debug.Log(lastRightRidgeStruct.body.name + "   " + lastRightRidgeStruct.tilePosList.Count);


			//*** 暫時不顯示瓦片

			// newRightRidgeStruct.tilePosList = CreateRoofSurfaceTile(roofSurfaceModelStruct, newRightRidgeStruct.body, newRightRidgeStruct, lastRightRidgeStruct, newMidRidgeStruct, eaveStruct, 1, RightMainRidgeStruct, roofSurfaceTileRidgeUpPointPos);
			//newLeftRidgeStruct.tilePosList = CreateRoofSurfaceTile(roofSurfaceModelStruct, newLeftRidgeStruct.body, newLeftRidgeStruct, lastLeftRidgeStruct, newMidRidgeStruct, eaveStruct, -1, LeftMainRidgeStruct, Vector3.Reflect(roofSurfaceTileRidgeUpPointPos, (eaveStruct.controlPointDictionaryList[EaveControlPointType.EndControlPoint.ToString()].transform.position - eaveStruct.controlPointDictionaryList[EaveControlPointType.StartControlPoint.ToString()].transform.position).normalized));

			newRightRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newRightRidgeStruct.body, newRightRidgeStruct, lastRightRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, 1);
			newLeftRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newLeftRidgeStruct.body, newLeftRidgeStruct, lastLeftRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, -1);


			// 			Debug.Log("size" + size);
			// 			Debug.Log(" newRightRidgeStruct.tilePosList.Count" + newRightRidgeStruct.tilePosList.Count);
			// 			newLeftRidgeStruct.tilePosList.RemoveRange(size, newLeftRidgeStruct.tilePosList.Count - size);
			// 			newRightRidgeStruct.tilePosList.RemoveRange(size, newRightRidgeStruct.tilePosList.Count - size);
			// 			Debug.Log(" newRightRidgeStruct.tilePosList.Count" + newRightRidgeStruct.tilePosList.Count);

			//MeshCenter.Instance.MeshCombineInGameObjectList(newRightRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);
			//MeshCenter.Instance.MeshCombineInGameObjectList(newLeftRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);

			lastRoofSurfaceTileRidgeUpPointPos = roofSurfaceTileRidgeUpPointPos;
			lastRoofSurfaceTileRidgeMidPointPos = roofSurfaceTileRidgeMidPointPos;
			lastRoofSurfaceTileRidgeDownPointPos = roofSurfaceTileRidgeDownPointPos;
			lastRightRidgeStruct = newRightRidgeStruct;
			lastLeftRidgeStruct = newLeftRidgeStruct;

			newRoofSurfaceStruct.rightRoofSurfaceTileRidgeList.Add(newRightRidgeStruct);
			newRoofSurfaceStruct.leftRoofSurfaceTileRidgeList.Add(newLeftRidgeStruct);
		}
		newRoofSurfaceStruct.midRoofSurfaceTileRidge = newMidRidgeStruct;


		CreateFlyRafer(newRoofSurfaceStruct, flyRafterWidth, flyRafterHeight, -(2 * flyRafterHeight + ModelController.Instance.roofSurfaceModelStruct.flatTileModelStruct.bound.size.y), eave2FlyEaveOffset, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist[(int)(RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count * 0.8f)], LeftMainRidgeStruct.ridgeCatLine.anchorInnerPointlist[(int)(LeftMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count * 0.8f)]);

		return newRoofSurfaceStruct;
	}
    private RoofSurfaceStruct CreateRoofSurfaceAA(RidgeStruct RightMainRidgeStruct, RidgeStruct LeftMainRidgeStruct, RidgeStruct eaveStruct)
    {
        RoofSurfaceStruct newRoofSurfaceStruct = CreateRoofSurfaceSturct("RoofSurface", parentObj.roof);
        RoofSurfaceRidgeStruct newMidRidgeStruct = CreateRoofSurfaceRidgeSturct("MidRoofSurfaceTileRidge", newRoofSurfaceStruct.body);

        Vector3 eaveDir = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()] - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()]).normalized;
        Vector3 roofSurfHeightOffset = (roofSurfaceHeightOffset * Vector3.Cross((RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] -
                                            RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]),
                                            (LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] -
                                            LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()])).normalized);
        Vector3 SurfaceMidPt = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()] + LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()]) / 2 + roofSurfHeightOffset;
        Vector3 midRoofSurfaceTopPointPos = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()] + LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]) / 2.0f;
        float SurfaceMidDis = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()] - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()]).magnitude;
        CatLine roofSurfaceMidPointLine = new CatLine();
        CatLine roofSurfDownPointLine = new CatLine();
        Plane verticalCutPlane = new Plane();
        Plane verticalMirrorPlane = new Plane();
        Vector3 verticalCutPlaneNormal = ((eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()])).normalized;
        verticalMirrorPlane.SetNormalAndPosition(verticalCutPlaneNormal, SurfaceMidPt);

        //** 屋面中間控制線
        roofSurfaceMidPointLine.controlPointPosList.Add(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
        roofSurfaceMidPointLine.controlPointPosList.Add(SurfaceMidPt + 0.25f * SurfaceMidDis * eaveDir);
        roofSurfaceMidPointLine.controlPointPosList.Add(SurfaceMidPt - 0.25f * SurfaceMidDis * eaveDir);
        roofSurfaceMidPointLine.controlPointPosList.Add(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);

        roofSurfaceMidPointLine.SetLineNumberOfPoints(1000);
        roofSurfaceMidPointLine.SetCatmullRom(anchorDis);
        //** 屋面下方線
        roofSurfDownPointLine.controlPointPosList.Add(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
        roofSurfDownPointLine.controlPointPosList.Add(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
        roofSurfDownPointLine.SetLineNumberOfPoints(1000);
        roofSurfDownPointLine.SetCatmullRom(anchorDis);

        //** 製作屋頂表面中間脊 
        List<CatLine> ctrlLineList = new List<CatLine>();       //** 存放四條控制線用 (須由上而下依序擺放:主脊、中間弧線、簷柱上方、屋簷 )
        ctrlLineList.Add(RightMainRidgeStruct.ridgeCatLine);
        ctrlLineList.Add(roofSurfaceMidPointLine);
        ctrlLineList.Add(roofSurfDownPointLine);
        ctrlLineList.Add(eaveStruct.ridgeCatLine);
        newMidRidgeStruct = CreateSurfaceRidges(newRoofSurfaceStruct.body, "MidRoofSurfaceTileRidge", verticalMirrorPlane, ctrlLineList);

		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString(), midRoofSurfaceTopPointPos);
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceMidPoint.ToString(), newMidRidgeStruct.ridgeCatLine.controlPointPosList[1]);
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString(), newMidRidgeStruct.ridgeCatLine.controlPointPosList[2]);
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.EaveCtrlPoint.ToString(), newMidRidgeStruct.ridgeCatLine.controlPointPosList[3]);

        //for (int iIndex = 0; iIndex < roofSurfaceMidPointLine.innerPointList.Count; iIndex++)
        //{
        //    MainController.ShowPos(roofSurfaceMidPointLine.innerPointList[iIndex], parentObj.roof, Color.gray, 0.5f);
        //}
        //for (int iIndex = 0; iIndex < roofSurfDownPointLine.innerPointList.Count; iIndex++)
        //{
        //    MainController.ShowPos(roofSurfDownPointLine.innerPointList[iIndex], parentObj.roof, Color.red, 0.5f);
        //}
        //*** 屋頂表面中間產瓦片 ***//
        newMidRidgeStruct.tilePosList = newMidRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist, newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);

		Vector3 roofSurfaceRidgeXZ_Dir = new Vector3(newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].x - newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].x, 0, newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].z - newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].z);

		newMidRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newMidRidgeStruct.body, newMidRidgeStruct, newMidRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, 0);
        MeshCenter.Instance.MeshCombineInGameObjectList(newMidRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);

       
 
        //紀錄前一次Index用於迴圈加速
        int roofSurface2MainRidgeStartingIndex_R = 0;
        int MidPointLineIndex = 0;
        int DownPointLineIndex = 0;
        int EavePointLineIndex = 0;
        //*** 做一個右半邊屋簷的瘠並將parent設為屋頂表面，並依照瓦片寬度算出內部錨點
        RidgeStruct eaveRightRidgeStruct = CreateRidgeSturct("EaveRightRidgeStruct", newRoofSurfaceStruct.body);
        int iHalfEventPointCount = eaveStruct.ridgeCatLine.anchorInnerPointlist.Count / 2 + 1;
        for (int k = 0; k < iHalfEventPointCount; k++)
        {
            eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Add(eaveStruct.ridgeCatLine.anchorInnerPointlist[k]);
        }
        //for (int k = iHalfEventPointCount; k < eaveStruct.ridgeCatLine.anchorInnerPointlist.Count; k++)
        //{
        //    eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Add(eaveStruct.ridgeCatLine.anchorInnerPointlist[k]);
        //}
        eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist = eaveRightRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist, eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileWidth);


        RoofSurfaceRidgeStruct lastRightRidgeStruct = newMidRidgeStruct;
        RoofSurfaceRidgeStruct lastLeftRidgeStruct = newMidRidgeStruct;
        List<int> LastIndex = new List<int>{ 0,0,0,0 };

        //** 製作左右邊屋面脊
        for (int n = 1; n < eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; n++)
        //for (int n = 24; n < eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; n++)
        {
            //*** 先設定新的切平面
            verticalCutPlane.SetNormalAndPosition(verticalCutPlaneNormal, eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n]);

            //兩個錨點間方向向量對屋簷主脊上的投影(這樣不就是=兩個錨點間方向向量?)
            Vector3 planeOffsetVector = Vector3.Project((eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n] - eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[0]),
                (eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()]));
            Vector3 roofSurfaceTileRidgeUpPointPos = Vector3.zero;
            Vector3 roofSurfaceTileRidgeMidPointPos = Vector3.zero;
            Vector3 roofSurfaceTileRidgeDownPointPos = Vector3.zero;
            Vector3 roofSurfaceTileRidgeEavePointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeUpPointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeMidPointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeDownPointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeEavePointPos = Vector3.zero;

            RoofSurfaceRidgeStruct newLeftRidgeStruct = CreateRoofSurfaceRidgeSturct("LeftRoofSurfaceTileRidge", newRoofSurfaceStruct.body);
            RoofSurfaceRidgeStruct newRightRidgeStruct = CreateSurfaceRidges(newRoofSurfaceStruct.body, "RightRoofSurfaceTileRidge", verticalCutPlane, ctrlLineList);
            //RoofSurfaceRidgeStruct newRightRidgeStruct = CreateSurfaceRidges(newRoofSurfaceStruct.body, "RightRoofSurfaceTileRidge", verticalCutPlane, ctrlLineList);

            //**製作左邊屋面脊，使用對襯於右邊完成的脊
            for (int iIndex = 0; iIndex < newRightRidgeStruct.ridgeCatLine.controlPointPosList.Count; iIndex++)
            {
                newLeftRidgeStruct.ridgeCatLine.controlPointPosList.Add(Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(newRightRidgeStruct.ridgeCatLine.controlPointPosList[iIndex])) * -verticalCutPlaneNormal + newRightRidgeStruct.ridgeCatLine.controlPointPosList[iIndex]);
            }
            newLeftRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(Define.Medium);
            newLeftRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);


            //** 設定瓦片位置與貼上瓦片
            newRightRidgeStruct.tilePosList = newRightRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist, newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);
            newLeftRidgeStruct.tilePosList = newLeftRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist, newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);
            newRightRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newRightRidgeStruct.body, newRightRidgeStruct, lastRightRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, 1);
			newLeftRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newLeftRidgeStruct.body, newLeftRidgeStruct, lastLeftRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, -1);

            lastRoofSurfaceTileRidgeUpPointPos = roofSurfaceTileRidgeUpPointPos;
            lastRoofSurfaceTileRidgeMidPointPos = roofSurfaceTileRidgeMidPointPos;
            lastRoofSurfaceTileRidgeDownPointPos = roofSurfaceTileRidgeDownPointPos;
            lastRoofSurfaceTileRidgeEavePointPos = roofSurfaceTileRidgeEavePointPos;
            lastRightRidgeStruct = newRightRidgeStruct;
            lastLeftRidgeStruct = newLeftRidgeStruct;

            newRoofSurfaceStruct.rightRoofSurfaceTileRidgeList.Add(newRightRidgeStruct);
            newRoofSurfaceStruct.leftRoofSurfaceTileRidgeList.Add(newLeftRidgeStruct);
        }

        newRoofSurfaceStruct.midRoofSurfaceTileRidge = newMidRidgeStruct;
        //CreateFlyRafer(newRoofSurfaceStruct, flyRafterWidth, flyRafterHeight, -(2 * flyRafterHeight + ModelController.Instance.roofSurfaceModelStruct.flatTileModelStruct.bound.size.y), eave2FlyEaveOffset, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist[(int)(RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count * 0.8f)], LeftMainRidgeStruct.ridgeCatLine.anchorInnerPointlist[(int)(LeftMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count * 0.8f)]);

        return newRoofSurfaceStruct;
    }
	private RoofSurfaceStruct CreateRoofSurfaceForDome(RidgeStruct mainRidgeStruct, RidgeStruct eaveStruct)
	{
		RoofSurfaceStruct newRoofSurfaceStruct = CreateRoofSurfaceSturct("RoofSurface", parentObj.roof);

		//*** 做一個右半邊屋簷的瘠並將parent設為屋頂表面，並依照瓦片寬度算出內部錨點
		RidgeStruct eaveRidgeStruct = CreateRidgeSturct("EaveRidgeStruct", newRoofSurfaceStruct.body);
		
		float radius=0;

		for (int k = 0; k < eaveStruct.ridgeCatLine.anchorInnerPointlist.Count; k++)
		{
			radius += Vector3.Distance(eaveStruct.ridgeCatLine.anchorInnerPointlist[k], eaveStruct.ridgeCatLine.anchorInnerPointlist[(k + 1) % eaveStruct.ridgeCatLine.anchorInnerPointlist.Count]);
			eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist.Add(eaveStruct.ridgeCatLine.anchorInnerPointlist[k]);
		}
		int num=(int)(radius/roofSurfaceTileWidth);
		float unitRevise = radius / num;
		eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist = eaveRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist, 0, eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, unitRevise);

		for (int n = 0; n < eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; n++)
		{
			RoofSurfaceRidgeStruct newRidgeStruct = CreateRoofSurfaceRidgeSturct("RoofSurfaceTileRidge", newRoofSurfaceStruct.body);

			//產生屋面
			Vector3 roofSurfHeightOffset = (roofSurfaceHeightOffset * Vector3.Cross(
												(eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist[(n + 1) % eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count] -
												mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]), (eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist[n] -
												mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()])).normalized);

			Vector3 roofSurfaceTopPointPos = mainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()];
			Vector3 roofSurfaceDownPointPos = eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist[n];
			Vector3 roofSurfaceMidPointPos = (roofSurfaceTopPointPos + roofSurfaceDownPointPos) / 2.0f + roofSurfHeightOffset;
			newRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString(), roofSurfaceTopPointPos);
			newRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceMidPoint.ToString(), roofSurfaceMidPointPos);
			newRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString(), roofSurfaceDownPointPos);
			//** 屋面中間控制線
			newRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceTopPointPos);
			newRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceMidPointPos);
			newRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceDownPointPos);
			ShowPos(roofSurfaceMidPointPos, newRidgeStruct.body, Color.green);
			newRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(Define.Large);
			newRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);

			//** 設定瓦片位置與貼上瓦片
			newRidgeStruct.tilePosList = newRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newRidgeStruct.ridgeCatLine.anchorInnerPointlist, newRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);

			Vector3 eaveDir = (eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist[(n + 1) % eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count] - eaveRidgeStruct.ridgeCatLine.anchorInnerPointlist[n]).normalized;
			Vector3 roofSurfaceRidgeXZ_Dir = new Vector3(newRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].x - newRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].x, 0, newRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].z - newRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].z);
			newRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newRidgeStruct.body, newRidgeStruct, (newRoofSurfaceStruct.rightRoofSurfaceTileRidgeList.Count == 0) ? newRidgeStruct : newRoofSurfaceStruct.rightRoofSurfaceTileRidgeList[newRoofSurfaceStruct.rightRoofSurfaceTileRidgeList.Count - 1], roofSurfaceRidgeXZ_Dir, eaveDir,1);
			newRoofSurfaceStruct.rightRoofSurfaceTileRidgeList.Add(newRidgeStruct);

		}

		return newRoofSurfaceStruct;
	}

    /**
   * 建造歇山頂正面屋頂表面
   * 輸入: 右側主脊、左側主脊、屋簷
   * 輸出: 屋頂表面結構
   * 流程: 
   * 1.找出屋頂表面中間控制線上中下點
   **/
    private RoofSurfaceStruct CreateRoofSurfaceForShyShan(RidgeStruct RightMainRidgeStruct, RidgeStruct LeftMainRidgeStruct, RidgeStruct eaveStruct)
    {
        RoofSurfaceStruct newRoofSurfaceStruct = CreateRoofSurfaceSturct("RoofSurface", parentObj.roof);
        RoofSurfaceRidgeStruct newMidRidgeStruct = CreateRoofSurfaceRidgeSturct("MidRoofSurfaceTileRidge", newRoofSurfaceStruct.body);


        /***************************************找兩個mainRidge中間垂直的roofSurfaceTileRidge***********************************************************/
        //FindMidRoofSurfaceMidPoint : 左右主脊的中間控制點 - 上方控制點找出方向向量，找出兩向量夾角
        Vector2 v1 = new Vector2(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].x - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].x, LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].z - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].z);
        Vector2 v2 = new Vector2(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].x - RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].x, RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].z - RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].z);

        float angle = Vector2.Angle(v1, v2);

        //*************** 1 ******************//
        //*** 找出屋頂表面中間控制線上中下點 ***//
        //************************************//
        //midRoofSurfaceTopPoint
        Vector3 midRoofSurfaceTopPointPos = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()] + LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]) / 2.0f;
        newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString(), midRoofSurfaceTopPointPos);
        //midRoofSurfaceDownPoint
        Vector3 midRoofSurfaceDownPointPos = eaveStruct.controlPointDictionaryList[EaveControlPointType.MidControlPoint.ToString()];
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString(), midRoofSurfaceDownPointPos);
        //midRoofSurfaceMidPoint : 左右兩主脊的中點位置相加除2 + (-2 * 垂直左右主脊的單位向量 ) (04/19 - 修改垂直左右主脊的單位向量公式)
        //Vector3 midRoofSurfaceMidPointPos = (Quaternion.Euler(0, angle / 2.0f, 0) * (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()].transform.position)) + (roofSurfaceHeightOffset * Vector3.up) - (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].transform.position - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()].transform.position) / 2.0f;
        Vector3 midRoofSurfaceMidPointPos = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()] +
                                                LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()]) / 2.0f +
                                                (roofSurfaceHeightOffset * Vector3.Cross((RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] -
                                                RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]),
                                                (LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] -
                                                LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()])).normalized);
        newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceMidPoint.ToString(), midRoofSurfaceMidPointPos);

        //MidRoofSurfaceTileRidge
        newMidRidgeStruct.ridgeCatLine.controlPointPosList.Add(midRoofSurfaceTopPointPos);
        newMidRidgeStruct.ridgeCatLine.controlPointPosList.Add(midRoofSurfaceMidPointPos);
        newMidRidgeStruct.ridgeCatLine.controlPointPosList.Add(midRoofSurfaceDownPointPos);
        newMidRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(1000);
        newMidRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);
        //** 做懸山部分的延長線
        CatLine Topline_M = new CatLine();
        Topline_M.controlPointPosList.Add(parentObj.roofTopCenter);
        Topline_M.controlPointPosList.Add((parentObj.roofTopCenter + midRoofSurfaceTopPointPos) / 2);
        Topline_M.controlPointPosList.Add(midRoofSurfaceTopPointPos);
        Topline_M.SetLineNumberOfPoints(1000);
        Topline_M.SetCatmullRom(anchorDis);
        newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist.InsertRange(0, Topline_M.anchorInnerPointlist);

        /***************************************用AnchorLength取MidRoofSurfaceTileRidge上的瓦片************************************************************/
        //*** 屋頂表面中間產瓦片 ***//
        newMidRidgeStruct.tilePosList = newMidRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist, newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);

		Vector3 roofSurfaceRidgeXZ_Dir = new Vector3(newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].x - newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].x, 0, newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].z - newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].z);
		Vector3 eaveDir = (eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()]);

		CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newMidRidgeStruct.body, newMidRidgeStruct, newMidRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, 0);
        MeshCenter.Instance.MeshCombineInGameObjectList(newMidRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);

        /***************************************由Eave上的AnchorPoint切割平面 用於切出其他垂直的roofSurfaceTileRidge(右側)***********************************************************/
        //*** 做一個右半邊屋簷的瘠並將parent設為屋頂表面，並依照瓦片寬度算出內部錨點
        RidgeStruct eaveRightRidgeStruct = CreateRidgeSturct("EaveRightRidgeStruct", newRoofSurfaceStruct.body);
        int iHalfEventPointCount = eaveStruct.ridgeCatLine.anchorInnerPointlist.Count / 2 + 1;
        for (int k = 0; k < iHalfEventPointCount; k++)
        {
            eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Add(eaveStruct.ridgeCatLine.anchorInnerPointlist[k]);
        }
        eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist = eaveRightRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist, eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileWidth);

        //for (int iIndex = 0; iIndex < eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; iIndex++)
        //{
        //    print("anchorInnerPointlist["+iIndex+"] : "+ eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[iIndex]);
        //}

        //VerCutPlane
        //*** 新增三個平面，將傳入的屋簷主脊用該起終點位置相減取得的向量定為法向量並正規化
        //*** verticalCutPlane : 算屋頂表面子瘠用輔助平面 
        //*** verticalMirrorPlane : 以屋簷主脊中點與剛剛的法向量形成一平面
        //*** surfacePlane : 疑問?貌似沒用到
        Plane verticalCutPlane = new Plane();
        Plane verticalMirrorPlane = new Plane();
        Plane surfacePlane = new Plane();
        Vector3 verticalCutPlaneNormal = ((eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()])).normalized;
        verticalMirrorPlane.SetNormalAndPosition(verticalCutPlaneNormal, midRoofSurfaceDownPointPos);

        //(右邊主瘠的上下控制點找出方向向量) 外積 (屋簷中點與右邊主脊上控制點找出方向向量)
        Vector3 surfacePlaneNormal = Vector3.Cross(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] - RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()], midRoofSurfaceDownPointPos - RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]).normalized;
        surfacePlane.SetNormalAndPosition(surfacePlaneNormal, midRoofSurfaceMidPointPos);
        //紀錄前一次Index用於迴圈加速
        int roofSurfaceMidPointStartingIndex_R = 0;
        int roofSurfaceMidPointStartingIndex_R_A = 0;
        int roofSurface2MainRidgeStartingIndex_R = 0;

        RoofSurfaceRidgeStruct lastRightRidgeStruct = newMidRidgeStruct;
        RoofSurfaceRidgeStruct lastLeftRidgeStruct = newMidRidgeStruct;

        // Tony哥的由屋頂中間控制線至屋簷的半圓取線
        CatLine roofSurfaceMidPointLine = new CatLine();
        //roofSurfaceMidPointLine.controlPointPosList.Add(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()].transform.position);
        roofSurfaceMidPointLine.controlPointPosList.Add(eaveStruct.controlPointDictionaryList[EaveControlPointType.MidLControlPoint.ToString()]);
        roofSurfaceMidPointLine.controlPointPosList.Add(midRoofSurfaceMidPointPos);
        //roofSurfaceMidPointLine.controlPointPosList.Add(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()].transform.position);
        roofSurfaceMidPointLine.controlPointPosList.Add(eaveStruct.controlPointDictionaryList[EaveControlPointType.MidRControlPoint.ToString()]);
        roofSurfaceMidPointLine.SetLineNumberOfPoints(1000);
        roofSurfaceMidPointLine.SetCatmullRom(anchorDis);

        //中間主脊控制線中點至右邊主脊下方端點之連線
        CatLine roofSurfaceMidPointLineA = new CatLine();
        roofSurfaceMidPointLineA.controlPointPosList.Add(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
        roofSurfaceMidPointLineA.controlPointPosList.Add(midRoofSurfaceMidPointPos);
        roofSurfaceMidPointLineA.controlPointPosList.Add(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
        roofSurfaceMidPointLineA.SetLineNumberOfPoints(1000);
        roofSurfaceMidPointLineA.SetCatmullRom(anchorDis);

        //         for (int f = 0; f < roofSurfaceMidPointLine.anchorInnerPointlist.Count; f++)
        //         {
        //             ShowPos(roofSurfaceMidPointLine.anchorInnerPointlist[f], newMidRidgeStruct.body, Color.green, 0.1f);
        //         }
        //Right&LeftRoofSurfaceTileRidgeList
        for (int n = 1; n < eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; n++)
        {

            verticalCutPlane.SetNormalAndPosition(verticalCutPlaneNormal, eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n]);

            //兩個錨點間方向向量對屋簷主脊上的投影(這樣不就是=兩個錨點間方向向量?)
            Vector3 planeOffsetVector = Vector3.Project((eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n] - eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[0]),
                (eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()]));
            //Right
            RoofSurfaceRidgeStruct newRightRidgeStruct = CreateRoofSurfaceRidgeSturct("RightRoofSurfaceTileRidge", newRoofSurfaceStruct.body);
            RoofSurfaceRidgeStruct newLeftRidgeStruct = CreateRoofSurfaceRidgeSturct("LeftRoofSurfaceTileRidge", newRoofSurfaceStruct.body);

            //*** 屋頂表面放置瓦片用脊
            Vector3 roofSurfaceTileRidgeUpPointPos = Vector3.zero;
            Vector3 roofSurfaceTileRidgeMidPointPos = Vector3.zero;
            Vector3 roofSurfaceTileRidgeDownPointPos = Vector3.zero;

            Vector3 lastRoofSurfaceTileRidgeUpPointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeMidPointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeDownPointPos = Vector3.zero;


            //FindPointOnMainRidgeCloser2Plane : 找主脊點上離輔助平面最近的點，若沒有的話(ex:無殿頂的主脊需延伸一段後開始)直接取中間控制線上點位移一個屋瓦寬度的位置 
            roofSurface2MainRidgeStartingIndex_R = FindNearestPointInList2Plane(verticalCutPlane, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist, roofSurface2MainRidgeStartingIndex_R, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0.01f);
            roofSurfaceTileRidgeUpPointPos = (roofSurface2MainRidgeStartingIndex_R != 0) ? RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist[roofSurface2MainRidgeStartingIndex_R] : midRoofSurfaceTopPointPos + planeOffsetVector;

            //FindPointOnEaveCloser2Plane : 屋簷主脊目前錨點本身
            roofSurfaceTileRidgeDownPointPos = eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n];


            //FindPointOnRoofSurfaceMidPointLineCloser2Plane
            roofSurfaceMidPointStartingIndex_R = FindNearestPointInList2Plane(verticalCutPlane, roofSurfaceMidPointLine.anchorInnerPointlist, roofSurfaceMidPointStartingIndex_R, roofSurfaceMidPointLine.anchorInnerPointlist.Count - 1);
            roofSurfaceMidPointStartingIndex_R_A = FindNearestPointInList2Plane(verticalCutPlane, roofSurfaceMidPointLineA.anchorInnerPointlist, roofSurfaceMidPointStartingIndex_R_A, roofSurfaceMidPointLineA.anchorInnerPointlist.Count - 1);

            //*** ratioA : 目前偵測點佔整個右側屋簷的比例。 ratioC:暫未用到
            float ratioA = ((float)(n) / ((eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1)));
            //float ratioC = ((float)Mathf.Abs(n - eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count / 2) / ((eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1 - eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count / 2)));

            //*** (舊)屋頂表面放置瓦片用脊的中點，依照比例取半圓取線與切平面焦點位置 + 瓦片用脊上下兩點之中點位置
            //*** 屋頂表面放置瓦片用脊的中點，只用全屋簷中間點曲線
            roofSurfaceTileRidgeMidPointPos = (roofSurfaceMidPointLineA.anchorInnerPointlist[roofSurfaceMidPointStartingIndex_R_A]) * (1.0f - ratioA) + ((roofSurfaceTileRidgeUpPointPos + roofSurfaceTileRidgeDownPointPos) / 2.0f) * ratioA;

            //*** 調整(感覺怪怪) 瓦片用脊的中點Y座標，依照比例取半圓曲線與切平面焦點位置 + 全屋簷中間點曲線與切平面焦點位置 + 瓦片用脊上下兩點之中點位置
            roofSurfaceTileRidgeMidPointPos.y = (roofSurfaceMidPointLine.anchorInnerPointlist[roofSurfaceMidPointStartingIndex_R].y * (1.0f - ratioA) + roofSurfaceMidPointLineA.anchorInnerPointlist[roofSurfaceMidPointStartingIndex_R_A].y * ratioA) * (1.0f - ratioA) + ((roofSurfaceTileRidgeUpPointPos.y + roofSurfaceTileRidgeDownPointPos.y) / 2.0f) * ratioA;
            //roofSurfaceTileRidgeMidPointPos.y = (roofSurfaceMidPointLineA.anchorInnerPointlist[roofSurfaceMidPointStartingIndex_R].y) * (1.0f - ratioA) + ((roofSurfaceTileRidgeUpPointPos.y + roofSurfaceTileRidgeDownPointPos.y) / 2.0f) * ratioA;
            //觀察用
            /*
			ShowPos(roofSurfaceTileRidgeUpPointPos, newLeftRidgeStruct.body, Color.white, 0.8f);
			ShowPos(roofSurfaceTileRidgeMidPointPos, newLeftRidgeStruct.body, Color.white, 0.8f);
			ShowPos(roofSurfaceTileRidgeDownPointPos, newLeftRidgeStruct.body, Color.white, 0.8f);
			*/
            //設定屋簷表面子瘠，並依照所占屋簷百分比設定點數
            newRightRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceTileRidgeUpPointPos);
            newRightRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceTileRidgeMidPointPos);
            newRightRidgeStruct.ridgeCatLine.controlPointPosList.Add(roofSurfaceTileRidgeDownPointPos);
            newRightRidgeStruct.ridgeCatLine.SetLineNumberOfPoints((int)(1000 * (1 - ratioA)));
            newRightRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);

            newLeftRidgeStruct.ridgeCatLine.controlPointPosList.Add(Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeUpPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeUpPointPos);
            newLeftRidgeStruct.ridgeCatLine.controlPointPosList.Add(Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeMidPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeMidPointPos);
            newLeftRidgeStruct.ridgeCatLine.controlPointPosList.Add(Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeDownPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeDownPointPos);
            newLeftRidgeStruct.ridgeCatLine.SetLineNumberOfPoints((int)(1000 * (1 - ratioA)));
            newLeftRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);

            if (roofSurface2MainRidgeStartingIndex_R == 0)
            {
                CatLine Topline_R = new CatLine();
                CatLine Topline_L = new CatLine();
                Vector3 newLRidgeTopCtrlPt = Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeUpPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeUpPointPos;

                Topline_R.controlPointPosList.Add(parentObj.roofTopCenter + planeOffsetVector);
                Topline_R.controlPointPosList.Add((parentObj.roofTopCenter + planeOffsetVector + roofSurfaceTileRidgeUpPointPos) / 2);
                Topline_R.controlPointPosList.Add(roofSurfaceTileRidgeUpPointPos);
                Topline_L.controlPointPosList.Add(parentObj.roofTopCenter - planeOffsetVector);
                Topline_L.controlPointPosList.Add((parentObj.roofTopCenter - planeOffsetVector + newLRidgeTopCtrlPt) / 2);
                Topline_L.controlPointPosList.Add(newLRidgeTopCtrlPt);
                Topline_R.SetLineNumberOfPoints(1000);
                Topline_R.SetCatmullRom(anchorDis);
                Topline_L.SetLineNumberOfPoints(1000);
                Topline_L.SetCatmullRom(anchorDis);
                newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.InsertRange(0, Topline_R.anchorInnerPointlist);
                newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist.InsertRange(0, Topline_L.anchorInnerPointlist);
            }
            /***************************************用AnchorLength取roofSurfaceTileRidge上的瓦片************************************************************/
            /*
			 for (int f = 0; f < newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; f++)
						{
							ShowPos(newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[f], newRightRidgeStruct.body, Color.green, 0.1f);
						}
			 */

            //設定瓦片位置
            newRightRidgeStruct.tilePosList = newRightRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist, newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);
            newLeftRidgeStruct.tilePosList = newLeftRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist, newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);
            /*
				for (int f = 0; f < newLeftRidgeStruct.tilePosList.Count; f++)
				{
					ShowPos(newLeftRidgeStruct.tilePosList[f], newLeftRidgeStruct.body, Color.blue, 0.3f);
				}
				for (int f = 0; f < newRightRidgeStruct.tilePosList.Count; f++)
				{
					ShowPos(newRightRidgeStruct.tilePosList[f], newRightRidgeStruct.body, Color.blue, 0.3f);
				}
				*/
            //Debug.Log(lastRightRidgeStruct.body.name + "   " + lastRightRidgeStruct.tilePosList.Count);


            //*** 暫時不顯示瓦片

            // newRightRidgeStruct.tilePosList = CreateRoofSurfaceTile(roofSurfaceModelStruct, newRightRidgeStruct.body, newRightRidgeStruct, lastRightRidgeStruct, newMidRidgeStruct, eaveStruct, 1, RightMainRidgeStruct, roofSurfaceTileRidgeUpPointPos);
            //newLeftRidgeStruct.tilePosList = CreateRoofSurfaceTile(roofSurfaceModelStruct, newLeftRidgeStruct.body, newLeftRidgeStruct, lastLeftRidgeStruct, newMidRidgeStruct, eaveStruct, -1, LeftMainRidgeStruct, Vector3.Reflect(roofSurfaceTileRidgeUpPointPos, (eaveStruct.controlPointDictionaryList[EaveControlPointType.EndControlPoint.ToString()].transform.position - eaveStruct.controlPointDictionaryList[EaveControlPointType.StartControlPoint.ToString()].transform.position).normalized));

            CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newRightRidgeStruct.body, newRightRidgeStruct, lastRightRidgeStruct, roofSurfaceRidgeXZ_Dir, -eaveDir, 1);
			CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newLeftRidgeStruct.body, newLeftRidgeStruct, lastLeftRidgeStruct, roofSurfaceRidgeXZ_Dir, -eaveDir, -1);

            // 			Debug.Log("size" + size);
            // 			Debug.Log(" newRightRidgeStruct.tilePosList.Count" + newRightRidgeStruct.tilePosList.Count);
            // 			newLeftRidgeStruct.tilePosList.RemoveRange(size, newLeftRidgeStruct.tilePosList.Count - size);
            // 			newRightRidgeStruct.tilePosList.RemoveRange(size, newRightRidgeStruct.tilePosList.Count - size);
            // 			Debug.Log(" newRightRidgeStruct.tilePosList.Count" + newRightRidgeStruct.tilePosList.Count);

            MeshCenter.Instance.MeshCombineInGameObjectList(newRightRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);
            MeshCenter.Instance.MeshCombineInGameObjectList(newLeftRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);

            lastRoofSurfaceTileRidgeUpPointPos = roofSurfaceTileRidgeUpPointPos;
            lastRoofSurfaceTileRidgeMidPointPos = roofSurfaceTileRidgeMidPointPos;
            lastRoofSurfaceTileRidgeDownPointPos = roofSurfaceTileRidgeDownPointPos;
            lastRightRidgeStruct = newRightRidgeStruct;
            lastLeftRidgeStruct = newLeftRidgeStruct;

            newRoofSurfaceStruct.rightRoofSurfaceTileRidgeList.Add(newRightRidgeStruct);
            newRoofSurfaceStruct.leftRoofSurfaceTileRidgeList.Add(newLeftRidgeStruct);
        }
        newRoofSurfaceStruct.midRoofSurfaceTileRidge = newMidRidgeStruct;


        return newRoofSurfaceStruct;
    }
    private RoofSurfaceStruct CreateRoofSurfaceForShyShanAA(RidgeStruct RightMainRidgeStruct, RidgeStruct LeftMainRidgeStruct, RidgeStruct eaveStruct)
    {
        RoofSurfaceStruct newRoofSurfaceStruct = CreateRoofSurfaceSturct("RoofSurface", parentObj.roof);
        RoofSurfaceRidgeStruct newMidRidgeStruct = CreateRoofSurfaceRidgeSturct("MidRoofSurfaceTileRidge", newRoofSurfaceStruct.body);

        Vector3 eaveDir = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()] - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.EaveCtrlPoint.ToString()]).normalized;
        Vector3 roofSurfHeightOffset = (roofSurfaceHeightOffset * Vector3.Cross((RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] -
                                            RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]),
                                            (LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()] -
                                            LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()])).normalized);
        Vector3 SurfaceMidPt = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()] + LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()]) / 2 + roofSurfHeightOffset;
        Vector3 midRoofSurfaceTopPointPos = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()] + LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.TopControlPoint.ToString()]) / 2.0f;
        float SurfaceMidDis = (RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()] - LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.MidControlPoint.ToString()]).magnitude;
        CatLine roofSurfaceMidPointLine = new CatLine();
        CatLine roofSurfDownPointLine = new CatLine();
        Plane verticalCutPlane = new Plane();
        Plane verticalMirrorPlane = new Plane();
        Vector3 verticalCutPlaneNormal = ((eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()])).normalized;
        verticalMirrorPlane.SetNormalAndPosition(verticalCutPlaneNormal, SurfaceMidPt);

        //** 屋面中間控制線
        roofSurfaceMidPointLine.controlPointPosList.Add(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
        roofSurfaceMidPointLine.controlPointPosList.Add(SurfaceMidPt + 0.25f * SurfaceMidDis * eaveDir);
        roofSurfaceMidPointLine.controlPointPosList.Add(SurfaceMidPt - 0.25f * SurfaceMidDis * eaveDir);
        roofSurfaceMidPointLine.controlPointPosList.Add(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);

        roofSurfaceMidPointLine.SetLineNumberOfPoints(1000);
        roofSurfaceMidPointLine.SetCatmullRom(anchorDis);
        //** 屋面下方線
        roofSurfDownPointLine.controlPointPosList.Add(RightMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
        roofSurfDownPointLine.controlPointPosList.Add(LeftMainRidgeStruct.controlPointDictionaryList[MainRidgeControlPointType.DownControlPoint.ToString()]);
        roofSurfDownPointLine.SetLineNumberOfPoints(1000);
        roofSurfDownPointLine.SetCatmullRom(anchorDis);

        //** 製作屋頂表面中間脊 
        List<CatLine> ctrlLineList = new List<CatLine>();   //** 存放四條控制線用 (須由上而下依序擺放:主脊、中間弧線、簷柱上方、屋簷 )
        CatLine Topline_Mid = new CatLine();                //** 補充山花部分屋面脊用
        ctrlLineList.Add(RightMainRidgeStruct.ridgeCatLine);
        ctrlLineList.Add(roofSurfaceMidPointLine);
        ctrlLineList.Add(roofSurfDownPointLine);
        ctrlLineList.Add(eaveStruct.ridgeCatLine);
        newMidRidgeStruct = CreateSurfaceRidges(newRoofSurfaceStruct.body, "MidRoofSurfaceTileRidge", verticalMirrorPlane, ctrlLineList);

		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString(), midRoofSurfaceTopPointPos);
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceMidPoint.ToString(), newMidRidgeStruct.ridgeCatLine.controlPointPosList[1]);
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString(), newMidRidgeStruct.ridgeCatLine.controlPointPosList[2]);
		newMidRidgeStruct.controlPointDictionaryList.Add(MidRoofSurfaceControlPointType.EaveCtrlPoint.ToString(), newMidRidgeStruct.ridgeCatLine.controlPointPosList[3]);

        Topline_Mid.controlPointPosList.Add(parentObj.roofTopCenter);
        Topline_Mid.controlPointPosList.Add(newMidRidgeStruct.ridgeCatLine.controlPointPosList[0]);
        Topline_Mid.SetLineNumberOfPoints(Define.Medium);
        Topline_Mid.SetCatmullRom(anchorDis);
        newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist.InsertRange(0, Topline_Mid.anchorInnerPointlist);

        //*** 屋頂表面中間產瓦片 ***//
        newMidRidgeStruct.tilePosList = newMidRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist, newMidRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);

		Vector3 roofSurfaceRidgeXZ_Dir = new Vector3(newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].x - newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].x, 0, newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceTopPoint.ToString()].z - newMidRidgeStruct.controlPointDictionaryList[MidRoofSurfaceControlPointType.MidRoofSurfaceDownPoint.ToString()].z);

		newMidRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newMidRidgeStruct.body, newMidRidgeStruct, newMidRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, 0);
        MeshCenter.Instance.MeshCombineInGameObjectList(newMidRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);


        //*** 做一個右半邊屋簷的瘠並將parent設為屋頂表面，並依照瓦片寬度算出內部錨點
        RidgeStruct eaveRightRidgeStruct = CreateRidgeSturct("EaveRightRidgeStruct", newRoofSurfaceStruct.body);
        int iHalfEventPointCount = eaveStruct.ridgeCatLine.anchorInnerPointlist.Count / 2 + 1;
        for (int k = 0; k < iHalfEventPointCount; k++)
        {
            eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Add(eaveStruct.ridgeCatLine.anchorInnerPointlist[k]);
        }
        eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist = eaveRightRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist, eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileWidth);

        RoofSurfaceRidgeStruct lastRightRidgeStruct = newMidRidgeStruct;
        RoofSurfaceRidgeStruct lastLeftRidgeStruct = newMidRidgeStruct;
        List<int> LastIndex = new List<int> { 0, 0, 0, 0 };
        int roofSurface2MainRidgeStartingIndex_R = 0;

        //** 製作左右邊屋面脊
        for (int n = 1; n < eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count; n++)
        {
            //*** 先設定新的切平面
            verticalCutPlane.SetNormalAndPosition(verticalCutPlaneNormal, eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n]);

            //兩個錨點間方向向量對屋簷主脊上的投影(這樣不就是=兩個錨點間方向向量?)
            Vector3 planeOffsetVector = Vector3.Project((eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[n] - eaveRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[0]),
                (eaveStruct.controlPointDictionaryList[EaveControlPointType.RightControlPoint.ToString()] - eaveStruct.controlPointDictionaryList[EaveControlPointType.LeftControlPoint.ToString()]));
            Vector3 roofSurfaceTileRidgeUpPointPos = Vector3.zero;
            Vector3 roofSurfaceTileRidgeMidPointPos = Vector3.zero;
            Vector3 roofSurfaceTileRidgeDownPointPos = Vector3.zero;
            Vector3 roofSurfaceTileRidgeEavePointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeUpPointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeMidPointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeDownPointPos = Vector3.zero;
            Vector3 lastRoofSurfaceTileRidgeEavePointPos = Vector3.zero;

            RoofSurfaceRidgeStruct newLeftRidgeStruct = CreateRoofSurfaceRidgeSturct("LeftRoofSurfaceTileRidge", newRoofSurfaceStruct.body);
            RoofSurfaceRidgeStruct newRightRidgeStruct = CreateSurfaceRidges(newRoofSurfaceStruct.body, "RightRoofSurfaceTileRidge", verticalCutPlane, ctrlLineList);
            //RoofSurfaceRidgeStruct newRightRidgeStruct = CreateSurfaceRidges(newRoofSurfaceStruct.body, "RightRoofSurfaceTileRidge", verticalCutPlane, ctrlLineList);

            //**製作左邊屋面脊，使用對襯於右邊完成的脊
            for (int iIndex = 0; iIndex < newRightRidgeStruct.ridgeCatLine.controlPointPosList.Count; iIndex++)
            {
                newLeftRidgeStruct.ridgeCatLine.controlPointPosList.Add(Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(newRightRidgeStruct.ridgeCatLine.controlPointPosList[iIndex])) * -verticalCutPlaneNormal + newRightRidgeStruct.ridgeCatLine.controlPointPosList[iIndex]);
            }
            newLeftRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(Define.Medium);
            newLeftRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);


            //*** 為了歇山
            if (roofType == MainController.RoofType.Shya_Shan_Ding)
            {
                roofSurface2MainRidgeStartingIndex_R = FindNearestPointInList2Plane(verticalCutPlane, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist, roofSurface2MainRidgeStartingIndex_R, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0.01f);
                if (roofSurface2MainRidgeStartingIndex_R == 0)
                {
                    CatLine Topline_R = new CatLine();
                    CatLine Topline_L = new CatLine();
                    Vector3 newLRidgeTopCtrlPt = Mathf.Abs(2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeUpPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeUpPointPos;

                    Topline_R.controlPointPosList.Add(parentObj.roofTopCenter + planeOffsetVector);
                    //Topline_R.controlPointPosList.Add((parentObj.roofTopCenter + planeOffsetVector + roofSurfaceTileRidgeUpPointPos) / 2);
                    Topline_R.controlPointPosList.Add(newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist[0]);
                    Topline_L.controlPointPosList.Add(parentObj.roofTopCenter - planeOffsetVector);
                    //Topline_L.controlPointPosList.Add((parentObj.roofTopCenter - planeOffsetVector + newLRidgeTopCtrlPt) / 2);
                    Topline_L.controlPointPosList.Add(newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist[0]);
                    Topline_R.SetLineNumberOfPoints(1000);
                    Topline_R.SetCatmullRom(anchorDis);
                    Topline_L.SetLineNumberOfPoints(1000);
                    Topline_L.SetCatmullRom(anchorDis);
                    newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.InsertRange(0, Topline_R.anchorInnerPointlist);
                    newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist.InsertRange(0, Topline_L.anchorInnerPointlist);
                }

            }

            //** 設定瓦片位置與貼上瓦片
            newRightRidgeStruct.tilePosList = newRightRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist, newRightRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);
            newLeftRidgeStruct.tilePosList = newLeftRidgeStruct.ridgeCatLine.CalculateAnchorPosByInnerPointList(newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist, newLeftRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count - 1, 0, roofSurfaceTileHeight);
            newRightRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newRightRidgeStruct.body, newRightRidgeStruct, lastRightRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, 1);
			newLeftRidgeStruct = CreateRoofSurfaceTile(ModelController.Instance.roofSurfaceModelStruct, newLeftRidgeStruct.body, newLeftRidgeStruct, lastLeftRidgeStruct, roofSurfaceRidgeXZ_Dir, eaveDir, -1);

            lastRoofSurfaceTileRidgeUpPointPos = roofSurfaceTileRidgeUpPointPos;
            lastRoofSurfaceTileRidgeMidPointPos = roofSurfaceTileRidgeMidPointPos;
            lastRoofSurfaceTileRidgeDownPointPos = roofSurfaceTileRidgeDownPointPos;
            lastRoofSurfaceTileRidgeEavePointPos = roofSurfaceTileRidgeEavePointPos;
            lastRightRidgeStruct = newRightRidgeStruct;
            lastLeftRidgeStruct = newLeftRidgeStruct;

            newRoofSurfaceStruct.rightRoofSurfaceTileRidgeList.Add(newRightRidgeStruct);
            newRoofSurfaceStruct.leftRoofSurfaceTileRidgeList.Add(newLeftRidgeStruct);
        }

        newRoofSurfaceStruct.midRoofSurfaceTileRidge = newMidRidgeStruct;
        //CreateFlyRafer(newRoofSurfaceStruct, flyRafterWidth, flyRafterHeight, -(2 * flyRafterHeight + ModelController.Instance.roofSurfaceModelStruct.flatTileModelStruct.bound.size.y), eave2FlyEaveOffset, RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist[(int)(RightMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count * 0.8f)], LeftMainRidgeStruct.ridgeCatLine.anchorInnerPointlist[(int)(LeftMainRidgeStruct.ridgeCatLine.anchorInnerPointlist.Count * 0.8f)]);

        return newRoofSurfaceStruct;
    }

    /**
     * 製作屋面瓦片脊.
     */
    private RoofSurfaceRidgeStruct CreateSurfaceRidges(GameObject parent,string name, Plane verticalMirrorPlane, List<CatLine> PtLineList)
    {
        RoofSurfaceRidgeStruct newRidgeStruct = CreateRoofSurfaceRidgeSturct(name, parent);

        for (int iIndex = 0; iIndex < PtLineList.Count; iIndex++)
        {
            int lineIndex = FindNearestPointInList2Plane(verticalMirrorPlane, PtLineList[iIndex].innerPointList, 0, PtLineList[iIndex].innerPointList.Count - 1);
            if (iIndex == 0 && lineIndex == 0)
            {

                //Vector3 projPoint = Vector3.ProjectOnPlane(parentObj.roofTopCenter, verticalMirrorPlane.normal);
                //2 * verticalMirrorPlane.GetDistanceToPoint(roofSurfaceTileRidgeDownPointPos)) * -verticalCutPlaneNormal + roofSurfaceTileRidgeDownPointPos
                Vector3 projPoint = verticalMirrorPlane.GetDistanceToPoint(PtLineList[0].innerPointList[0]) * -verticalMirrorPlane.normal + PtLineList[0].innerPointList[0];
                newRidgeStruct.ridgeCatLine.controlPointPosList.Add(projPoint);
            }
            //if (lineIndex != 0 || iIndex == 0)
            else if (lineIndex != 0 )
            {
                newRidgeStruct.ridgeCatLine.controlPointPosList.Add(PtLineList[iIndex].innerPointList[lineIndex]);
            }
        }
        newRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(Define.Medium);
        newRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);
        return newRidgeStruct;
    }
    private RoofSurfaceRidgeStruct CreateSurfaceRidges(GameObject parent, string name, Plane verticalMirrorPlane, List<CatLine> PtLineList,ref List<int> IndexList)
    {
        RoofSurfaceRidgeStruct newRidgeStruct = CreateRoofSurfaceRidgeSturct(name, parent);

        for (int iIndex = 0; iIndex < PtLineList.Count; iIndex++)
        {
            int lineIndex = FindNearestPointInList2Plane(verticalMirrorPlane, PtLineList[iIndex].innerPointList, IndexList[iIndex], PtLineList[iIndex].innerPointList.Count - 1);
            IndexList[iIndex] = lineIndex;
            if (iIndex == 0 && lineIndex == 0)
            {
                Vector3 projPoint = Vector3.ProjectOnPlane(parentObj.roofTopCenter, verticalMirrorPlane.normal);
                newRidgeStruct.ridgeCatLine.controlPointPosList.Add(projPoint);
            }
            //if (lineIndex != 0 || iIndex == 0)
            else if (lineIndex != 0)
            {
                newRidgeStruct.ridgeCatLine.controlPointPosList.Add(PtLineList[iIndex].innerPointList[lineIndex]);
            }
        }
        newRidgeStruct.ridgeCatLine.SetLineNumberOfPoints(Define.Medium);
        newRidgeStruct.ridgeCatLine.SetCatmullRom(anchorDis);
        return newRidgeStruct;
    }

    /**
	 * 依照屋面列表將瓦片的Mesh結合起來
	 */
    public void CombineTileBySurfaceList(RoofSurfaceStruct RoofSurface)
	{
		//MeshCenter.Instance.MeshCombineInGameObjectList(newRightRidgeStruct.body, Resources.Load("Models/Materials/RoofMat") as Material);
		for (int iIndex = 0; iIndex < RoofSurface.leftRoofSurfaceTileRidgeList.Count; iIndex++)
		{
			MeshCenter.Instance.MeshCombineInGameObjectList(RoofSurface.leftRoofSurfaceTileRidgeList[iIndex].body, Resources.Load("Models/Materials/RoofMat") as Material);
		}
		MeshCenter.Instance.MeshCombineInGameObjectList(RoofSurface.midRoofSurfaceTileRidge.body, Resources.Load("Models/Materials/RoofMat") as Material);
		for (int iIndex = 0; iIndex < RoofSurface.rightRoofSurfaceTileRidgeList.Count; iIndex++)
		{
			MeshCenter.Instance.MeshCombineInGameObjectList(RoofSurface.rightRoofSurfaceTileRidgeList[iIndex].body, Resources.Load("Models/Materials/RoofMat") as Material);
		}

	}

	
	/**
	* 依照輸入的主脊列表建立主脊瓦片
	*/
	void createMainRidgeTileFromList(MainRidgeModelStruct mainRidgeModelStructGameObject, List<RidgeStruct> MainRidgeList)
	{
		switch (roofType)
		{
			case MainController.RoofType.Shya_Shan_Ding:

				for (int iIndex = 0; iIndex < 4; iIndex++)
				{
					CreateMainRidgeTile(ModelController.Instance.mainRidgeModelStruct, MainRidgeList[iIndex * 2], MainRidgeList[iIndex * 2].body);              //* 垂脊瓦片
					CreateMainRidgeTile(ModelController.Instance.mainRidgeModelStruct, MainRidgeList[iIndex * 2 + 1], MainRidgeList[iIndex * 2 + 1].body);      //* 戧脊屋瓦
				}

				break;
			default:
				for (int iIndex = 0; iIndex < MainRidgeList.Count; iIndex++)
				{
					MainRidgeList[iIndex] = CreateMainRidgeTile(mainRidgeModelStructGameObject, MainRidgeList[iIndex]);
				}
				break;
		}
	}

	void CopyRoofFunction(GameObject parent, float angle, Vector3 rotationCenter, int times, Vector3 offsetVector, params GameObject[] cloneObject)
	{
		for (int n = 0; n < cloneObject.Length; n++)
		{
			for (int i = 1; i < times; i++)
			{
				GameObject clone = Instantiate(cloneObject[n], cloneObject[n].transform.position, cloneObject[n].transform.rotation) as GameObject;
				clone.transform.RotateAround(rotationCenter, Vector3.up, angle * i);
				clone.transform.position += offsetVector;
				clone.transform.parent = parent.transform;
			}
		}

	}

	public void CreateRoof(List<Vector3> columnTopPosList, List<Vector3> topFloorBorderList, bool isDownFloor)
	{
		RidgeStruct RightMainRidgeStruct;
		RidgeStruct LeftMainRidgeStruct;
		RidgeStruct RightMainRidgeStructA;
		RidgeStruct LeftMainRidgeStructA;
		RidgeStruct eaveStruct;
		RidgeStruct eaveStructA;
		RoofSurfaceStruct roofSurfaceStruct;
		RoofSurfaceStruct roofSurfaceStructA;
		Vector3 offsetVector;
		Vector3 eave2eaveColumnOffsetVector;
		 List<Vector3> ctrlPointList;

		int ColumnIndex_Zero = 0;
		int ColumnIndex_One = 1;
		int ColumnIndex_Two = 2;
		int ColumnIndex_Three = 3;

		switch (roofType)
		{
			//攢尖頂
			case MainController.RoofType.Zan_Jian_Ding:
				#region  Zan_Jian_Ding

				if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
				{
					//主脊-MainRidge輔助線 
					eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_One].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_One].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
					RightMainRidgeStruct = CreateMainRidgeStruct(parentObj.roofTopCenter, columnTopPosList[ColumnIndex_One] + eave2eaveColumnOffsetVector);
					eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
					LeftMainRidgeStruct = CreateMainRidgeStruct(parentObj.roofTopCenter, columnTopPosList[ColumnIndex_Zero] + eave2eaveColumnOffsetVector);

					//Eave輔助線
					eaveStruct = CreateEaveStruct(RightMainRidgeStruct, LeftMainRidgeStruct);//檐出

					//RoofSurface
					roofSurfaceStruct = CreateRoofSurfaceA(RightMainRidgeStruct, LeftMainRidgeStruct, eaveStruct);//屋面

					CreateMainRidgeTile(ModelController.Instance.mainRidgeModelStruct, RightMainRidgeStruct, roofSurfaceStruct, eaveStruct, RightMainRidgeStruct.body);
					//主脊-MainRidge輔助線 
					eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Two].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Two].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
					RightMainRidgeStructA = CreateMainRidgeStruct(parentObj.roofTopCenter, columnTopPosList[ColumnIndex_Two] + eave2eaveColumnOffsetVector);
					LeftMainRidgeStructA = RightMainRidgeStruct;
					//Eave輔助線
					eaveStructA = CreateEaveStruct(RightMainRidgeStructA, LeftMainRidgeStructA);//檐出

					//RoofSurface
					roofSurfaceStructA = CreateRoofSurfaceA(RightMainRidgeStructA, LeftMainRidgeStructA, eaveStructA);//屋面

					CreateMainRidgeTile(ModelController.Instance.mainRidgeModelStruct, RightMainRidgeStructA, roofSurfaceStructA, eaveStructA, RightMainRidgeStructA.body);

					//複製出其他屋面
					CopyRoofFunction(parentObj.roof, 180, parentObj.roofTopCenter, 2, parentObj.roofTopCenter - parentObj.roofTopCenter, roofSurfaceStruct.body, roofSurfaceStructA.body, RightMainRidgeStruct.body, RightMainRidgeStructA.body);
					Destroy(LeftMainRidgeStruct.body);

				}
				else
				{
					//主脊-MainRidge輔助線 
					eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_One].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_One].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
					RightMainRidgeStruct = CreateMainRidgeStruct(parentObj.roofTopCenter, columnTopPosList[ColumnIndex_One] + eave2eaveColumnOffsetVector);
					eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
					LeftMainRidgeStruct = CreateMainRidgeStruct(parentObj.roofTopCenter, columnTopPosList[ColumnIndex_Zero] + eave2eaveColumnOffsetVector);

					//Eave輔助線
					eaveStruct = CreateEaveStruct(RightMainRidgeStruct, LeftMainRidgeStruct);//檐出

					//RoofSurface
					roofSurfaceStruct = CreateRoofSurfaceA(RightMainRidgeStruct, LeftMainRidgeStruct, eaveStruct);//屋面


					CreateMainRidgeTile(ModelController.Instance.mainRidgeModelStruct, RightMainRidgeStruct, roofSurfaceStruct, eaveStruct, RightMainRidgeStruct.body);

					//複製出其他屋面
					CopyRoofFunction(parentObj.roof, 360.0f / (int)MainController.Instance.sides, parentObj.roofTopCenter, (int)MainController.Instance.sides, parentObj.roofTopCenter - parentObj.roofTopCenter, roofSurfaceStruct.body, RightMainRidgeStruct.body);
					Destroy(LeftMainRidgeStruct.body);
				}

				#endregion
				break;
			case MainController.RoofType.Wu_Dian_Ding:
				#region  Wu_Dian_Ding

				if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
				{
                    List<RidgeStruct> Wu_Dian_eaveList = new List<RidgeStruct>();
                    MainRidgeList.Clear();
                    SurfaceList.Clear();
                    offsetVector = (columnTopPosList[ColumnIndex_One] - columnTopPosList[ColumnIndex_Zero]).normalized * Wu_Dian_DingMainRidgeWidth * 0.5f;
					Vector3 rightRoofTopCenter = parentObj.roofTopCenter + offsetVector;
					Vector3 leftRoofTopCenter = parentObj.roofTopCenter - offsetVector;

                    //** 主脊 - 
                    for (int iIndex = 0; iIndex < columnTopPosList.Count; iIndex++)
                    {
                        Vector3 RidgeTopPos = (iIndex == 1 || iIndex == 2) ? rightRoofTopCenter : leftRoofTopCenter;
                        eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[iIndex].x - RidgeTopPos.x, 0, columnTopPosList[iIndex].z - RidgeTopPos.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;

                        List<Vector3> ctrlPointList = new List<Vector3>();
                        ctrlPointList.Add(RidgeTopPos);
                        ctrlPointList.Add((RidgeTopPos + columnTopPosList[iIndex]) / 2 + mainRidgeHeightOffset * Vector3.up);
                        ctrlPointList.Add(columnTopPosList[iIndex] + Define.Do_Kun_Height * Vector3.up);
                        ctrlPointList.Add(columnTopPosList[iIndex] + eave2eaveColumnOffsetVector);

                        MainRidgeList.Add(CreateMainRidgeStruct4Point(ctrlPointList));
                    }
                    createMainRidgeTileFromList(ModelController.Instance.mainRidgeModelStruct, MainRidgeList);

                    //** 屋簷 與 屋面
                    for (int iIndex = 0; iIndex < MainRidgeList.Count; iIndex++)
                    {
                        Wu_Dian_eaveList.Add(CreateEaveStruct4Point(MainRidgeList[(iIndex+1) % MainRidgeList.Count], MainRidgeList[iIndex]));
                        SurfaceList.Add(CreateRoofSurfaceAA(MainRidgeList[(iIndex + 1) % MainRidgeList.Count], MainRidgeList[iIndex], Wu_Dian_eaveList[iIndex]));
                    }
				}
				#endregion
				break;

			case MainController.RoofType.Lu_Ding:
				#region  Lu_Ding
				if ((int)MainController.Instance.sides == (int)MainController.FormFactorSideType.FourSide)
				{
                    List<RidgeStruct> Lu_Ding_eaveList = new List<RidgeStruct>();
                    MainRidgeList.Clear();
                    SurfaceList.Clear();
                    //* 建立各主脊
                    for (int iIndex = 0; iIndex < columnTopPosList.Count; iIndex++)
                    {
                        offsetVector = (new Vector3(columnTopPosList[iIndex].x - parentObj.roofTopCenter.x, 0, columnTopPosList[iIndex].z - parentObj.roofTopCenter.z)).normalized * Lu_DingMainRidgeOffset * 0.5f;
                        Vector3 RidgeTopPos = (isDownFloor)? topFloorBorderList[ColumnIndex_Two] : parentObj.roofTopCenter + offsetVector;
                        eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[iIndex].x - parentObj.roofTopCenter.x, 0, columnTopPosList[iIndex].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;

                        List<Vector3> ctrlPointList = new List<Vector3>();
                        ctrlPointList.Add(RidgeTopPos);
                        ctrlPointList.Add((RidgeTopPos + columnTopPosList[iIndex]) / 2 + mainRidgeHeightOffset * Vector3.up);
                        ctrlPointList.Add(columnTopPosList[iIndex] + Define.Do_Kun_Height * Vector3.up);
                        ctrlPointList.Add(columnTopPosList[iIndex] + eave2eaveColumnOffsetVector);

                        MainRidgeList.Add(CreateMainRidgeStruct4Point(ctrlPointList));
                    }
                    createMainRidgeTileFromList(ModelController.Instance.mainRidgeModelStruct, MainRidgeList);

                    for (int iIndex = 0; iIndex < MainRidgeList.Count; iIndex++)
                    {
                        Lu_Ding_eaveList.Add(CreateEaveStruct4Point(MainRidgeList[(iIndex + 1) % MainRidgeList.Count], MainRidgeList[iIndex]));
                        SurfaceList.Add(CreateRoofSurfaceAA(MainRidgeList[(iIndex + 1) % MainRidgeList.Count], MainRidgeList[iIndex], Lu_Ding_eaveList[iIndex]));
                    }

                    /**
                    //主脊-MainRidge輔助線 
                    if (isDownFloor)
					{
						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_One].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_One].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						RightMainRidgeStruct = CreateMainRidgeStruct(topFloorBorderList[ColumnIndex_One], columnTopPosList[ColumnIndex_One] + eave2eaveColumnOffsetVector);
						//RightMainRidgeStruct = CreateMainRidgeStruct(topFloorBorderList[ColumnIndex_One], columnTopPosList[ColumnIndex_One] + eave2eaveColumnOffsetVector);

						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						LeftMainRidgeStruct = CreateMainRidgeStruct(topFloorBorderList[ColumnIndex_Zero], columnTopPosList[ColumnIndex_Zero] + eave2eaveColumnOffsetVector);
						//主脊-MainRidge輔助線 
						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Two].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Two].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						RightMainRidgeStructA = CreateMainRidgeStruct(topFloorBorderList[ColumnIndex_Two], columnTopPosList[ColumnIndex_Two] + eave2eaveColumnOffsetVector);
					}
					else
					{
						offsetVector = (new Vector3(columnTopPosList[ColumnIndex_One].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_One].z - parentObj.roofTopCenter.z)).normalized * Lu_DingMainRidgeOffset * 0.5f;
						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_One].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_One].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						RightMainRidgeStruct = CreateMainRidgeStruct(parentObj.roofTopCenter + offsetVector, columnTopPosList[ColumnIndex_One] + eave2eaveColumnOffsetVector);

						offsetVector = (new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)).normalized * Lu_DingMainRidgeOffset * 0.5f;
						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						LeftMainRidgeStruct = CreateMainRidgeStruct(parentObj.roofTopCenter + offsetVector, columnTopPosList[ColumnIndex_Zero] + eave2eaveColumnOffsetVector);
						//主脊-MainRidge輔助線 
						offsetVector = (new Vector3(columnTopPosList[ColumnIndex_Two].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Two].z - parentObj.roofTopCenter.z)).normalized * Lu_DingMainRidgeOffset * 0.5f;
						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Two].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Two].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						RightMainRidgeStructA = CreateMainRidgeStruct(parentObj.roofTopCenter + offsetVector, columnTopPosList[ColumnIndex_Two] + eave2eaveColumnOffsetVector);

					}

					//Eave輔助線
					eaveStruct = CreateEaveStruct(RightMainRidgeStruct, LeftMainRidgeStruct);//檐出

					//RoofSurface
					roofSurfaceStruct = CreateRoofSurfaceA(RightMainRidgeStruct, LeftMainRidgeStruct, eaveStruct);//屋面

					CreateMainRidgeTile(ModelController.Instance.mainRidgeModelStruct, RightMainRidgeStruct, roofSurfaceStruct, eaveStruct, RightMainRidgeStruct.body);



					LeftMainRidgeStructA = RightMainRidgeStruct;
					//Eave輔助線
					eaveStructA = CreateEaveStruct(RightMainRidgeStructA, LeftMainRidgeStructA);//檐出

					//RoofSurface
					roofSurfaceStructA = CreateRoofSurfaceA(RightMainRidgeStructA, LeftMainRidgeStructA, eaveStructA);//屋面

					CreateMainRidgeTile(ModelController.Instance.mainRidgeModelStruct, RightMainRidgeStructA, roofSurfaceStructA, eaveStructA, RightMainRidgeStructA.body);
					//複製出其他屋面
					CopyRoofFunction(parentObj.roof, 180, parentObj.roofTopCenter, 2, parentObj.roofTopCenter - parentObj.roofTopCenter, roofSurfaceStruct.body, roofSurfaceStructA.body, RightMainRidgeStruct.body, RightMainRidgeStructA.body);
					Destroy(LeftMainRidgeStruct.body);
                    */
				}
				else //FormFactorSideType.ThreeSide
				{
					if (isDownFloor)
					{
						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_One].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_One].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						RightMainRidgeStruct = CreateMainRidgeStruct(topFloorBorderList[ColumnIndex_One], columnTopPosList[ColumnIndex_One] + eave2eaveColumnOffsetVector);

						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						LeftMainRidgeStruct = CreateMainRidgeStruct(topFloorBorderList[ColumnIndex_Zero], columnTopPosList[ColumnIndex_Zero] + eave2eaveColumnOffsetVector);

					}
					else
					{
						offsetVector = (new Vector3(columnTopPosList[ColumnIndex_One].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_One].z - parentObj.roofTopCenter.z)).normalized * Lu_DingMainRidgeOffset * 0.5f;
						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_One].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_One].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						RightMainRidgeStruct = CreateMainRidgeStruct(parentObj.roofTopCenter + offsetVector, columnTopPosList[ColumnIndex_One] + eave2eaveColumnOffsetVector);

						offsetVector = (new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)).normalized * Lu_DingMainRidgeOffset * 0.5f;
						eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
						LeftMainRidgeStruct = CreateMainRidgeStruct(parentObj.roofTopCenter + offsetVector, columnTopPosList[ColumnIndex_Zero] + eave2eaveColumnOffsetVector);
					}


					//Eave輔助線
					eaveStruct = CreateEaveStruct(RightMainRidgeStruct, LeftMainRidgeStruct);//檐出

					//RoofSurface
					roofSurfaceStruct = CreateRoofSurfaceA(RightMainRidgeStruct, LeftMainRidgeStruct, eaveStruct);//屋面

					CreateMainRidgeTile(ModelController.Instance.mainRidgeModelStruct, RightMainRidgeStruct, roofSurfaceStruct, eaveStruct, RightMainRidgeStruct.body);

					//複製出其他屋面
					CopyRoofFunction(parentObj.roof, 360.0f / (int)MainController.Instance.sides, parentObj.roofTopCenter, (int)MainController.Instance.sides, parentObj.roofTopCenter - parentObj.roofTopCenter, roofSurfaceStruct.body, RightMainRidgeStruct.body);
					Destroy(LeftMainRidgeStruct.body);
				}
				#endregion
				break;

			case MainController.RoofType.Shya_Shan_Ding:
                #region  Shya_Shan_Ding
                MainRidgeList.Clear();
                if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
				{

					List<Vector3> VerRidgeBotPosList = new List<Vector3>();         //** 垂脊下方控制點點列表
					List<Vector3> DownCtrlPointPosList = new List<Vector3>();       //** 垂脊下方控制點點列表
                    
                    offsetVector = (columnTopPosList[ColumnIndex_Zero] - columnTopPosList[ColumnIndex_One]).normalized * Shya_Shan_DingMainRidgeWidth;

					//***  v3VerticalOffsetVec : 垂直屋面的向量，eave2eaveColumnOffsetVector : 屋頂中心頂點到各個角柱的xz平面方向向量
					Vector3 v3VerticalOffsetVec = (columnTopPosList[ColumnIndex_Zero] - columnTopPosList[ColumnIndex_Three]).normalized;
					Vector3 VerRidgeBotPos = parentObj.roofTopCenter - Vector3.up * Shya_Shan_DingSanhuaHeight;
					VerRidgeBotPos = VerRidgeBotPos + v3VerticalOffsetVec * Shya_Shan_DingSanhuaWidth;

					VerRidgeBotPosList.Add(VerRidgeBotPos + offsetVector);
					VerRidgeBotPosList.Add(VerRidgeBotPos - offsetVector);
					VerRidgeBotPosList.Add(Quaternion.AngleAxis(180, Vector3.up) * VerRidgeBotPosList[0]);
					VerRidgeBotPosList.Add(Quaternion.AngleAxis(180, Vector3.up) * VerRidgeBotPosList[1]);

                    //** 建立主脊結構 (MainRidgeList中依序存放一側的垂脊(0,2,4,6)和戧脊(1,3,5,7))
                    for (int iIndex = 0; iIndex < 4; iIndex++)
                    {
                        List<Vector3> VerRidgeCtrlPtList = new List<Vector3>();     //* 垂脊控制點列表
                        List<Vector3> CtrlPointList = new List<Vector3>();          //* 戗脊控制點列表
                        Vector3 topCenter = (iIndex == 0 || iIndex == 3) ? parentObj.roofTopCenter + offsetVector : parentObj.roofTopCenter - offsetVector;
                        eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[iIndex].x - parentObj.roofTopCenter.x, 0, columnTopPosList[iIndex].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;

                        VerRidgeCtrlPtList.Add(topCenter);
                        VerRidgeCtrlPtList.Add((topCenter + VerRidgeBotPosList[iIndex]) * 0.5f);
                        VerRidgeCtrlPtList.Add((topCenter + VerRidgeBotPosList[iIndex]) * 0.5f);
                        VerRidgeCtrlPtList.Add(VerRidgeBotPosList[iIndex]);

                        CtrlPointList.Add(VerRidgeBotPosList[iIndex]);
                        CtrlPointList.Add((VerRidgeBotPosList[iIndex] + columnTopPosList[iIndex]) / 2 + mainRidgeHeightOffset * Vector3.up);
                        CtrlPointList.Add(columnTopPosList[iIndex] + Define.Do_Kun_Height * Vector3.up);
                        CtrlPointList.Add(columnTopPosList[iIndex] + eave2eaveColumnOffsetVector);

                        MainRidgeList.Add(CreateMainRidgeStruct4Point(VerRidgeCtrlPtList));
                        MainRidgeList.Add(CreateMainRidgeStruct4Point(CtrlPointList));
                    }
                    createMainRidgeTileFromList(ModelController.Instance.mainRidgeModelStruct, MainRidgeList);

                    ////** 屋簷
                    eaveStruct = CreateEaveStruct4Point(MainRidgeList[3], MainRidgeList[1]); //* 正面屋簷
                    eaveStructA = CreateEaveStruct4Point(MainRidgeList[5], MainRidgeList[3]); //* 側面屋簷
                    roofSurfaceStruct = CreateRoofSurfaceForShyShanAA(MainRidgeList[3], MainRidgeList[1], eaveStruct);          //* 屋面
                    roofSurfaceStructA = CreateRoofSurfaceAA(MainRidgeList[5], MainRidgeList[3], eaveStructA);
                    ////** 複製表面瓦片
                    CopyRoofFunction(parentObj.roof, 180, parentObj.roofTopCenter, 2, Vector3.zero, roofSurfaceStruct.body);
                    CopyRoofFunction(parentObj.roof, 180, parentObj.roofTopCenter, 2, Vector3.zero, roofSurfaceStructA.body);
                   
                }
                #endregion
                break;
			case MainController.RoofType.Zan_Jian_Ding2:
                #region  Zan_Jian_Ding2
                List<RidgeStruct> eaveList = new List<RidgeStruct>();

				MainRidgeList.Clear();
				//* 建立各主脊
				for (int iIndex = 0; iIndex < columnTopPosList.Count; iIndex++)
				{
					eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[iIndex].x - parentObj.roofTopCenter.x, 0, columnTopPosList[iIndex].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;
					MainRidgeList.Add(CreateMainRidgeStruct(parentObj.roofTopCenter, columnTopPosList[iIndex] + eave2eaveColumnOffsetVector));
				}
				createMainRidgeTileFromList(ModelController.Instance.mainRidgeModelStruct, MainRidgeList);
				//* 建立各屋簷
				for (int iIndex = 0; iIndex < MainRidgeList.Count; iIndex++)
				{
					if (iIndex == MainRidgeList.Count - 1)
					{
						//eaveList.Add(CreateEaveStruct(MainRidgeList[iIndex], MainRidgeList[0]));
						eaveList.Add(CreateEaveStruct(MainRidgeList[0], MainRidgeList[iIndex]));
					}
					else
					{
						//eaveList.Add(CreateEaveStruct(MainRidgeList[iIndex], MainRidgeList[iIndex + 1]));
						eaveList.Add(CreateEaveStruct(MainRidgeList[iIndex + 1], MainRidgeList[iIndex]));
					}
				}
				//* 建立個屋頂表面
				for (int iIndex = 0; iIndex < MainRidgeList.Count; iIndex++)
				{
					if (iIndex == MainRidgeList.Count - 1)
					{
						//SurfaceList.Add(CreateRoofSurfaceA(MainRidgeList[iIndex], MainRidgeList[0],eaveList[iIndex]));
						SurfaceList.Add(CreateRoofSurfaceA(MainRidgeList[0], MainRidgeList[iIndex], eaveList[iIndex]));
					}
					else
					{
						//SurfaceList.Add(CreateRoofSurfaceA(MainRidgeList[iIndex], MainRidgeList[iIndex + 1],eaveList[iIndex]));
						SurfaceList.Add(CreateRoofSurfaceA(MainRidgeList[iIndex + 1], MainRidgeList[iIndex], eaveList[iIndex]));
					}
				}
                #endregion
                break;
            case MainController.RoofType.Zan_Jian_Ding3:
                #region  Zan_Jian_Ding3
                List<RidgeStruct> eaveList1 = new List<RidgeStruct>();

                MainRidgeList.Clear();
                //* 建立各主脊
                for (int iIndex = 0; iIndex < columnTopPosList.Count; iIndex++)
                {
                    eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[iIndex].x - parentObj.roofTopCenter.x, 0, columnTopPosList[iIndex].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;

                   ctrlPointList = new List<Vector3>();
                    ctrlPointList.Add(parentObj.roofTopCenter);
                    ctrlPointList.Add((parentObj.roofTopCenter + columnTopPosList[iIndex]) / 2 + mainRidgeHeightOffset *  Vector3.up);
                    ctrlPointList.Add(columnTopPosList[iIndex] + Define.Do_Kun_Height * Vector3.up);
                    ctrlPointList.Add(columnTopPosList[iIndex] + eave2eaveColumnOffsetVector);

                    MainRidgeList.Add(CreateMainRidgeStruct4Point(ctrlPointList));
                }
                createMainRidgeTileFromList(ModelController.Instance.mainRidgeModelStruct, MainRidgeList);
                //* 建立各屋簷
                for (int iIndex = 0; iIndex < MainRidgeList.Count; iIndex++)
                {

					eaveList1.Add(CreateEaveStruct4Point(MainRidgeList[(iIndex + 1) % MainRidgeList.Count], MainRidgeList[iIndex]));                        
					//eaveList1.Add(CreateEaveStruct4Point(MainRidgeList[iIndex], MainRidgeList[(iIndex + 1) % MainRidgeList.Count]));
                }
                //* 建立個屋頂表面
                for (int iIndex = 0; iIndex < MainRidgeList.Count; iIndex++)
                {
               
					SurfaceList.Add(CreateRoofSurfaceAA(MainRidgeList[(iIndex + 1) % MainRidgeList.Count], MainRidgeList[iIndex], eaveList1[iIndex]));
                }
                #endregion
                break;

				case MainController.RoofType.Dome:

				Debug.Log("Dome");
				#region  Dome

				MainRidgeList.Clear();
				//主脊-MainRidge輔助線 
				eave2eaveColumnOffsetVector = Vector3.Normalize(new Vector3(columnTopPosList[ColumnIndex_Zero].x - parentObj.roofTopCenter.x, 0, columnTopPosList[ColumnIndex_Zero].z - parentObj.roofTopCenter.z)) * eave2eaveColumnOffset + flyEaveHeightOffset * Vector3.up + beamsHeight * Vector3.up;

                    ctrlPointList = new List<Vector3>();
                    ctrlPointList.Add(parentObj.roofTopCenter);
					ctrlPointList.Add((parentObj.roofTopCenter + columnTopPosList[ColumnIndex_Zero]) / 2 + mainRidgeHeightOffset * Vector3.up);
					ctrlPointList.Add(columnTopPosList[ColumnIndex_Zero] + Define.Do_Kun_Height * Vector3.up);
					ctrlPointList.Add(columnTopPosList[ColumnIndex_Zero] + eave2eaveColumnOffsetVector);

                    MainRidgeList.Add(CreateMainRidgeStruct4Point(ctrlPointList));
					//Eave輔助線
					eaveStruct = CreateEaveStruct(MainRidgeList[0]);//檐出

					//RoofSurface
					roofSurfaceStruct = CreateRoofSurfaceForDome(MainRidgeList[0], eaveStruct);//屋面

				#endregion
					break;
        }
	}

}