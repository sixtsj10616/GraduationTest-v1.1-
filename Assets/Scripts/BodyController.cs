using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ColumnStruct
{
	public GameObject columnObj;
	public CylinderMesh columnMesh;
	public CylinderMesh fundation;
	public Vector3 topPos;
	public Vector3 bottomPos;
}

public class BodyController : MonoBehaviour
{
	BuildingObj buildingObj;
	//Body******************************************************************************
	public enum BodyType { Chuan_Dou = 0, Tai_Liang = 1 };//Chuan_Dou 穿斗式 ,Tai_Liang 抬梁式

	public BodyType bodyType = BodyType.Chuan_Dou;

	public int goldColumnbayNumber = 5;//間數量

	public int eaveColumnbayNumber = 1;
	public int unitNumberInBay = 2;                 //間內有幾個單位(如:一間內有幾個門+窗)
    public int doorNumber =1;                       //門的數量(若間為奇數 則門為單扇 / 偶數 則門為雙扇)

    public float eaveColTopOffset = 0.0f;           //* 簷柱上方位移
    public float eaveColBotOffset = 0.0f;           //* 簷柱下方位移
    public float eaveColumnHeight;
    public float bodyWidth=40;                         //** 面寬
    public float bodyLength=40;                         //** 進深

	public float goldColumnHeight;
	public float eaveColumnRadius = 0.5f;
	public float goldColumnRadius = 0.5f;
    public float eaveColRadInflate = 1.0f;          //* 簷柱膨脹值
	public float columnFundationHeight;//柱礎高度
	public bool isGoldColumn = true;
	public bool isFrieze = true;
	public bool isBalustrade = true;

    public List<GameObject> windowObjList = new List<GameObject>(); //* 窗戶物件列表
    public List<GameObject> doorObjList = new List<GameObject>();   //* 大門物件列表
    public List<GameObject> friezeObjList = new List<GameObject>(); //* 門楣物件列表
    public List<GameObject> balusObjList = new List<GameObject>();  //* 門楣物件列表
    List<float> eaveColRadList;                                     //* 簷柱半徑比例列表
    //***********************************************************************
    public List<ColumnStruct> eaveColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> goldColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> eaveCornerColumnList = new List<ColumnStruct>();//eaveColumnList中角柱
	public List<ColumnStruct> goldCornerColumnList = new List<ColumnStruct>();//goldColumnList中角柱
	//***********************************************************************
	public List<Vector3> GetColumnStructTopPosList(List<ColumnStruct> columnStructList)
	{
		List<Vector3> posList = new List<Vector3>();
		for (int i = 0; i < columnStructList.Count; i++)
		{
			posList.Add(columnStructList[i].topPos);
		}
		return posList;
	}
	public List<Vector3> GetColumnStructBottomPosList(List<ColumnStruct> columnStructList)
	{
		List<Vector3> posList = new List<Vector3>();
		for (int i = 0; i < columnStructList.Count; i++)
		{
			posList.Add(columnStructList[i].bottomPos);
		}
		return posList;
	}

    /**
     * 初始化
     */
    public void InitFunction(BuildingObj buildingObj,float eaveColumnHeight, float goldColumnHeight)
	{

		//初始值******************************************************************************
		this.buildingObj=buildingObj;
		this.eaveColumnHeight = eaveColumnHeight;
		this.goldColumnHeight = goldColumnHeight;

        columnFundationHeight = eaveColumnHeight * 0.05f;
	
		//**************************************************************************************
		switch (bodyType)
		{
			#region Chuan_Dou
			case BodyType.Chuan_Dou:
				if(buildingObj.sides==MainController.FormFactorSideType.FourSide)
					CreateBody(buildingObj.body, (int)buildingObj.sides, buildingObj.entraneIndexList, bodyWidth, bodyLength, eaveColumnHeight, buildingObj.bodyCenter);
				else
					CreateBody(buildingObj.body, (int)buildingObj.sides, buildingObj.entraneIndexList,buildingObj.platformController.platFormStruct.topPointPosList, bodyWidth, eaveColumnHeight, buildingObj.bodyCenter);

				if (goldColumnList.Count > 0 )
				{
					CreateRingWall(buildingObj.body, ModelController.Instance.goldColumnModelStruct, GetColumnStructBottomPosList(goldColumnList), goldColumnRadius, unitNumberInBay, goldColumnbayNumber, doorNumber);
				}
                if (eaveColumnList.Count > 0 && isFrieze)
                {
					CreateRingFrieze(buildingObj.body, ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.7f * eaveColumnHeight);
                }
                if (eaveColumnList.Count > 0 && isBalustrade)
                {
					CreateRingBalustrade(buildingObj.body, (int)buildingObj.sides, buildingObj.entraneIndexList, ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.1f * eaveColumnHeight);
                }
                break;
			#endregion
		}
	}    /**
     * 調整柱子位置列表 (!!注意目前只有四邊形，方法感覺也可以在調整)
     * 輸入位移(z:面寬，x:進深)
     * 流程 : 取出 0 號柱子位置並加上位移量，之後算與原始位置的放大比率，
     * 將其他的柱子也都乘上此比率
     */
	public void UpdateOrigBottomPos(float offsetZ, float offsetX)
	{
		if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
		{
			Vector3 pos = new Vector3(eaveCornerColumnList[0].bottomPos.x + offsetX, eaveCornerColumnList[0].bottomPos.y, eaveCornerColumnList[0].bottomPos.z - offsetZ);
			Vector3 scaleVec = new Vector3(pos.x / eaveCornerColumnList[0].bottomPos.x, pos.y / eaveCornerColumnList[0].bottomPos.y, pos.z / eaveCornerColumnList[0].bottomPos.z);
			for (int iIndex = 0; iIndex < 4; iIndex++)
			{
				Vector3 newPos = eaveCornerColumnList[iIndex].bottomPos;
				newPos.Scale(scaleVec);
				eaveCornerColumnList[iIndex].bottomPos = newPos;
			}
		}
	}

    public void MoveValueUpdate(Vector3 offset)
	{
		for (int i = 0; i < eaveColumnList.Count; i++)
		{
			eaveColumnList[i].topPos += offset;
			eaveColumnList[i].bottomPos += offset;
		}
		for (int i = 0; i < goldColumnList.Count; i++)
		{
			goldColumnList[i].topPos += offset;
			goldColumnList[i].bottomPos += offset;
		}
	
	}

    /**
     * 製作單一柱子 (包誇柱子與柱基)
     * 輸入 : 1. parentObj , 2. 位置 , 3. topRadius : 上半徑 , 4. downRadius : 下半徑 
     *        5. height : 柱高 ,  6. fundationRadius : 柱基半徑 , 7. fundationHeight : 柱基高 
     *        8. name : 物件名稱 , 9. 柱子膨脹半徑比例列表
     *        
     */
    public ColumnStruct CreateColumn(GameObject parentObj, Vector3 pos, float topRadius, float downRadius, float height, float fundationRadius, float fundationHeight, string name = "Column" , List<float> RadRateList = null)
	{
        GameObject col = new GameObject(name);
        ColumnStruct columnStruct = new ColumnStruct();
  
        Vector3 topPos = pos + (height / 2.0f) * Vector3.up;
        Vector3 bottomPos = pos - (height / 2.0f - fundationHeight) * Vector3.up;

        col.transform.parent = parentObj.transform;
		col.AddComponent<CylinderMesh>();
		col.GetComponent<CylinderMesh>().CylinderInitSetting(pos, topPos, bottomPos, topRadius, downRadius);
        if ( this.eaveColBotOffset != 0 || this.eaveColTopOffset != 0 || RadRateList != null)
        {
            col.GetComponent<CylinderMesh>().SetMesh(RadRateList);
        }
        else
        {
            col.GetComponent<CylinderMesh>().SetMesh();
        }
        
        columnStruct.columnObj = col;
		columnStruct.columnMesh = col.GetComponent<CylinderMesh>();
		columnStruct.topPos = topPos;
		columnStruct.bottomPos = pos - (height / 2.0f) * Vector3.up;
		
        //*** Fundation 柱杵
		GameObject fun = new GameObject(name + "Fundation");
		//fun.transform.position = pos - new Vector3(0, height / 2.0f, 0);
		fun.transform.parent = col.transform;
		fun.AddComponent<CylinderMesh>();

        topPos = pos + (fundationHeight) * Vector3.up - (height / 2.0f) * Vector3.up;
        bottomPos = pos - (height / 2.0f) * Vector3.up;
 

        fun.GetComponent<CylinderMesh>().CylinderInitSetting(pos - new Vector3(0, height / 2.0f, 0), topPos, bottomPos, fundationRadius, fundationRadius);
        fun.GetComponent<CylinderMesh>().SetMesh();
        columnStruct.fundation = fun.GetComponent<CylinderMesh>();

		return columnStruct;
	}
	private Vector3 mutiplyVector3(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}
    /**
     * 建造整圈柱子
     * 若 eaveColRadInflate (簷柱半徑膨脹值) 為 0 則為一般
     * 否則計算 簷柱半徑膨脹列表 後製造柱子
     */
	public List<ColumnStruct> CreateRingColumn(GameObject parentObj, List<Vector3> posList, float columnTopRadius, float columnDownRadius, float columnHeight, float fundationRadius, float fundationHeight, string columnName)
	{
		List<ColumnStruct> columnList = new List<ColumnStruct>();
		for (int i = 0; i < posList.Count; i++)
		{
            ColumnStruct newColumn = CreateColumn(parentObj, posList[i], columnTopRadius, columnDownRadius, columnHeight, fundationRadius, fundationHeight, columnName,this.eaveColRadList);
            columnList.Add(newColumn);
		}
		return columnList;
	}

    /**
     * 製作屋身
     */
	private void CreateBody(GameObject parentObj, int sides, EntraneIndexList entraneIndexList, List<Vector3> posList, float bodyWidth, float bodyHeight, Vector3 bodyCenter)
	{
		Debug.Log("CreateBody");
		eaveCornerColumnList.Clear();
		goldCornerColumnList.Clear();
		goldColumnList.Clear();
		eaveColumnList.Clear();
		List<Vector3> eaveColumnPosList;
		List<Vector3> goldColumnPosList;
		eaveColumnPosList = CalculateColumnPos(posList,bodyWidth, bodyHeight, eaveColumnbayNumber, bodyCenter);
		goldColumnPosList = CalculateColumnPos(posList,bodyWidth*0.9f, bodyHeight, goldColumnbayNumber, bodyCenter);

		eaveColumnList = CreateRingColumn( parentObj, eaveColumnPosList, eaveColumnRadius, eaveColumnRadius, eaveColumnHeight, eaveColumnRadius * 1.2f, columnFundationHeight, "EaveColumn");
		if (isGoldColumn)
		{
			goldColumnList = CreateRingColumn(parentObj, goldColumnPosList, goldColumnRadius, goldColumnRadius, goldColumnHeight, goldColumnRadius * 1.2f, columnFundationHeight, "GoldColumn");
		}

		//角柱計算
		if (eaveColumnbayNumber <= 0) eaveColumnbayNumber = 1;
		for (int i = 0, count = 0; i < sides; i++)
		{
			eaveCornerColumnList.Add(eaveColumnList[count]);
			if (entraneIndexList.Contains(i)) count++;
			else count += eaveColumnbayNumber;
		}
		if (isGoldColumn)
		{
			if (goldColumnbayNumber <= 0) goldColumnbayNumber = 1;
			for (int i = 0, count = 0; i < sides; i++)
			{
				goldCornerColumnList.Add(goldColumnList[count]);
				count += goldColumnbayNumber;
			}
		}
	}
	private void CreateBody(GameObject parentObj, int sides, EntraneIndexList entraneIndexList, float bodyWidth, float bodyLength, float bodyHeight, Vector3 bodyCenter)
    {
		Debug.Log("CreateBody");
        eaveCornerColumnList.Clear();
		goldCornerColumnList.Clear();
        goldColumnList.Clear();
        eaveColumnList.Clear();
		List<Vector3> eaveColumnPosList;
		List<Vector3> goldColumnPosList;
		eaveColumnPosList = CalculateColumnPos(bodyLength,bodyWidth, bodyHeight, eaveColumnbayNumber, bodyCenter);
		goldColumnPosList = CalculateColumnPos(bodyLength*0.9f,bodyWidth*0.9f, bodyHeight, goldColumnbayNumber, bodyCenter);

		eaveColumnList = CreateRingColumn( parentObj, eaveColumnPosList, eaveColumnRadius, eaveColumnRadius, eaveColumnHeight, eaveColumnRadius * 1.2f, columnFundationHeight, "EaveColumn");
        if (isGoldColumn)
        {
			goldColumnList = CreateRingColumn( parentObj, goldColumnPosList, goldColumnRadius, goldColumnRadius, goldColumnHeight, goldColumnRadius * 1.2f, columnFundationHeight, "GoldColumn");
        }

        //角柱計算
        if (eaveColumnbayNumber <= 0) eaveColumnbayNumber = 1;
        for (int i = 0, count = 0; i < sides; i++)
        {
            eaveCornerColumnList.Add(eaveColumnList[count]);
			if (entraneIndexList.Contains(i)) count++;
			else count += eaveColumnbayNumber;
        }
		if (isGoldColumn)
		{
			if (goldColumnbayNumber <= 0) goldColumnbayNumber = 1;
			for (int i = 0, count = 0; i <sides; i++)
			{
				goldCornerColumnList.Add(goldColumnList[count]);
				count += goldColumnbayNumber;
			}
		}
    }
    /**
     * 計算柱位置
     */
	public List<Vector3> CalculateColumnPos(float bodyLength, float bodyWidth, float bodyHeight, int bayNum, Vector3 bodyCenter)
    {
		List<Vector3> columnPosList = new List<Vector3>();
		List<Vector3> posList = new List<Vector3>();
		Vector3 v1 = new Vector3(bodyWidth / 2.0f + bodyCenter.x, bodyCenter.y, -bodyLength / 2.0f + bodyCenter.z);
		Vector3 v2 = new Vector3(bodyWidth / 2.0f + bodyCenter.x, bodyCenter.y, bodyLength / 2.0f + bodyCenter.z);
		Vector3 v3 = new Vector3(-bodyWidth / 2.0f + bodyCenter.x, bodyCenter.y, bodyLength / 2.0f + bodyCenter.z);
		Vector3 v4 = new Vector3(-bodyWidth / 2.0f + bodyCenter.x, bodyCenter.y, -bodyLength / 2.0f + bodyCenter.z);
		posList.Add(v1);
		posList.Add(v2);
		posList.Add(v3);
		posList.Add(v4);
		for (int i = 0; i < posList.Count; i++)
		{
			Vector3 columnPos = posList[i];
			columnPosList.Add(columnPos);

			int nextIndex = (i + 1) % posList.Count;
			Vector3 posNext = posList[nextIndex];

			float disBetweenColumn = Vector3.Distance(columnPos, posNext);
			float bayWidth = disBetweenColumn / bayNum;
			Vector3 bayDir = posNext - columnPos;

			for (int j = 1; j < bayNum; j++)
			{
				Vector3 bayColumnPos = bayDir.normalized * (j * bayWidth) + columnPos;
				columnPosList.Add(bayColumnPos);
			}
		}
		return columnPosList;
    }
	public List<Vector3> CalculateColumnPos(List<Vector3> posList, float bodyWidth, float bodyHeight, int bayNum, Vector3 bodyCenter)
	{
		List<Vector3> columnPosList = new List<Vector3>();

		for (int i = 0; i < posList.Count; i++)
		{
			Vector2 v = new Vector2(posList[i].x - bodyCenter.x, posList[i].z - bodyCenter.z);
			v = new Vector2(posList[i].x - bodyCenter.x, posList[i].z - bodyCenter.z);
			v = v.normalized * bodyWidth;
			Vector3 columnPos = bodyCenter + new Vector3(v.x, 0, v.y);
			columnPosList.Add(columnPos);

			int nextIndex = (i + 1) % posList.Count;
			Vector2 vNext = new Vector2(posList[nextIndex].x - bodyCenter.x, posList[nextIndex].z - bodyCenter.z);
			vNext = vNext.normalized * bodyWidth;
			Vector3 posNext = bodyCenter + new Vector3(vNext.x, 0, vNext.y);

			float disBetweenColumn = Vector3.Distance(columnPos, posNext);
			float bayWidth = disBetweenColumn / bayNum;
			Vector3 bayDir = posNext - columnPos;

			for (int j = 1; j < bayNum; j++)
			{
				Vector3 bayColumnPos = bayDir.normalized * (j * bayWidth) + columnPos;
				columnPosList.Add(bayColumnPos);
			}
		}

		return columnPosList;
	}
	/**
	 * 建造整圈牆(columnList為bottom位置)
	 */
	public void CreateRingWall(GameObject parentObj, GoldColumnModelStruct goldColumnModelStruct, List<Vector3> columnList, float columnRadius, int unit, int goldColumnbayNumber, int doorNumber)
	{
		float wallHeight = goldColumnHeight;//牆長度
		float wallLengh = columnRadius * 2.0f;//牆深度

		float windowWidth = goldColumnModelStruct.windowModelStruct.bound.size.x;//裝飾物長度
		float windowHeight = goldColumnModelStruct.windowModelStruct.bound.size.y;//裝飾物長度
		float windowLengh = goldColumnModelStruct.windowModelStruct.bound.size.z;//裝飾物深度


		float doorWidth = goldColumnModelStruct.doorModelStruct.bound.size.x;//裝飾物長度
		float doorHeight = goldColumnModelStruct.doorModelStruct.bound.size.y;//裝飾物長度
		float doorLengh = goldColumnModelStruct.doorModelStruct.bound.size.z ;//裝飾物深度


		float windowWallWidth = goldColumnModelStruct.windowWallModelStruct.bound.size.x;//裝飾物長度
		float windowWallHeight = goldColumnModelStruct.windowWallModelStruct.bound.size.y;//裝飾物長度
		float windowWallLengh = goldColumnModelStruct.windowWallModelStruct.bound.size.z;//裝飾物深度

		float doorMidIndex = ((float)goldColumnbayNumber / 2);
		doorNumber = Mathf.Clamp(doorNumber,0 ,Mathf.CeilToInt(doorMidIndex));
		int doorMaxIndex = (int)((goldColumnbayNumber % 2 == 1) ? (doorMidIndex + (doorNumber - 1)) : (doorMidIndex + (doorNumber - 1)+0.5f));
		int doorMinIndex = (int)((goldColumnbayNumber % 2 == 1) ? (doorMidIndex - (doorNumber - 1)) : (doorMidIndex - (doorNumber - 1) - 0.5f));
		for (int i = 0; i < columnList.Count; i++)
		{
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
	
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			//門
			if (((i % (goldColumnbayNumber)) <= doorMaxIndex) && (i % (goldColumnbayNumber) >= doorMinIndex))
			{
				float width = dis;
				for (int j = 0; j < 1; j++)
				{
					float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + goldColumnHeight/2.0f*Vector3.up;
					float disDiff = doorWidth - width;
					float doorWidthScale = (width) / (doorWidth);
					float doorHeightScale = wallHeight / (doorHeight);
				    //door
				    GameObject clone = Instantiate(goldColumnModelStruct.doorModelStruct.model, pos, goldColumnModelStruct.doorModelStruct.model.transform.rotation) as GameObject;
				    clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(goldColumnModelStruct.doorModelStruct.rotation);
				    clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x * doorWidthScale, clone.transform.GetChild(0).localScale.y * doorHeightScale, (clone.transform.GetChild(0).localScale.z));
				    clone.transform.parent =parentObj.transform;
                    doorObjList.Add(clone);
				}

			}
			else//窗
			{
				float width = dis / unit;
				for (int j = 0; j < unit; j++)
				{
					#region windowWall
					float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + goldColumnHeight / 2.0f * Vector3.up;
					float disDiff = windowWallWidth - width;
					float windowWallWidthScale = (width) / (windowWallWidth);
					float windowWallHeightScale = wallHeight / (windowWallHeight);
					//windowWall
					GameObject clone = Instantiate(goldColumnModelStruct.windowWallModelStruct.model, pos, goldColumnModelStruct.windowWallModelStruct.model.transform.rotation) as GameObject;
					clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(goldColumnModelStruct.windowWallModelStruct.rotation);
					clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x * windowWallWidthScale, clone.transform.GetChild(0).localScale.y * windowWallHeightScale, (clone.transform.GetChild(0).localScale.z));
	
					clone.transform.parent = parentObj.transform;
                    windowObjList.Add(clone);
                #endregion
                }
			}
		}
	}
	/**
	 * 建造整門楣(columnList為bottom位置)
	 */
	public void CreateRingFrieze(GameObject parentObj, EaveColumnModelStruct eaveColumnModelStruct, List<Vector3> columnList, float columnRadius, float heightOffset)
	{
		float friezeWidth = eaveColumnModelStruct.friezeModelStruct.bound.size.x;//裝飾物長度
		float friezeHeight = eaveColumnModelStruct.friezeModelStruct.bound.size.y;//裝飾物長度
		float friezeLengh = eaveColumnModelStruct.friezeModelStruct.bound.size.z;//裝飾物深度
		for (int i = 0; i < columnList.Count; i++)
		{
			float width = friezeWidth;
			float height = friezeHeight;
			float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
			int number = Mathf.Max(Mathf.FloorToInt(dis / width), 1);
			Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
			float disDiff = (dis - width * number) / number;
			width = dis / number;
			float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : - Vector3.Angle(dir, Vector3.right));
			for (int j = 0; j < number; j++)
			{
				Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + heightOffset * Vector3.up;
				GameObject clone = Instantiate(eaveColumnModelStruct.friezeModelStruct.model, pos, eaveColumnModelStruct.friezeModelStruct.model.transform.rotation) as GameObject;
				clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.friezeModelStruct.rotation);
				clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * (width) / friezeWidth);

				clone.transform.parent = parentObj.transform;
                friezeObjList.Add(clone);
			}

			//FriezeWall(frieze上方裝飾牆)
			GameObject friezeWall = new GameObject("FriezeWall");
			MeshFilter meshFilter = friezeWall.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = friezeWall.AddComponent<MeshRenderer>();
			friezeWall.transform.parent = parentObj.transform;
			meshRenderer.material.color = Color.yellow;
			float rotateAngleZ = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) :  - Vector3.Angle(dir, Vector3.right));
			float friezeWallHeight = (eaveColumnHeight - heightOffset - friezeHeight / 2.0f);
			Vector3 posZ = (columnList[i] + columnList[(i + 1) % columnList.Count]) / 2.0f + (heightOffset + friezeHeight / 2.0f + friezeWallHeight / 2.0f) * Vector3.up;
			MeshCenter.Instance.CreateCubeMesh(posZ, dis, friezeWallHeight, 0.5f, rotateAngleZ, meshFilter);
            friezeObjList.Add(friezeWall);

           
				//sparrowBrace雀替
				Vector3 posX = dir.normalized * (columnRadius) + columnList[i] + (heightOffset - friezeHeight / 2.0f) * Vector3.up;
				float rotateAngleX = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
				GameObject sparrowBrace = Instantiate(eaveColumnModelStruct.sparrowBraceModelStruct.model, posX, eaveColumnModelStruct.sparrowBraceModelStruct.model.transform.rotation) as GameObject;
				sparrowBrace.transform.rotation = Quaternion.AngleAxis(rotateAngleX, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.sparrowBraceModelStruct.rotation);

				sparrowBrace.transform.parent =parentObj.transform;
				friezeObjList.Add(sparrowBrace);

				posX = -dir.normalized * (columnRadius) + columnList[(i + 1) % columnList.Count] + (heightOffset - friezeHeight / 2.0f) * Vector3.up;
				rotateAngleX = (Vector3.Dot(Vector3.forward, -dir) < 0 ? Vector3.Angle(-dir, Vector3.right) : -Vector3.Angle(-dir, Vector3.right));
				sparrowBrace = Instantiate(eaveColumnModelStruct.sparrowBraceModelStruct.model, posX, eaveColumnModelStruct.sparrowBraceModelStruct.model.transform.rotation) as GameObject;
				sparrowBrace.transform.rotation = Quaternion.AngleAxis(rotateAngleX, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.sparrowBraceModelStruct.rotation);

				sparrowBrace.transform.parent = parentObj.transform;
                friezeObjList.Add(sparrowBrace);
           
		}
	}
    /**
     * 建造整欄杆
     */
	public void CreateRingBalustrade(GameObject parentObj, int sides, EntraneIndexList entraneIndexList, EaveColumnModelStruct eaveColumnModelStruct, List<Vector3> columnList, float columnRadius, float heightOffset)
	{
		float balustradeWidth = eaveColumnModelStruct.balustradeModelStruct.bound.size.x;//欄杆長度
		float balustradeHeight = eaveColumnModelStruct.balustradeModelStruct.bound.size.y;//欄杆長度
		float balustradeLengh = eaveColumnModelStruct.balustradeModelStruct.bound.size.z;//欄杆深度

		//n:MainController.Instance.sides
		//i:
		for (int n = 0, i = 0; n < sides; n++)
		{
			if (entraneIndexList.Contains(n)) { i++; continue; }

			for (int k = 0; k < eaveColumnbayNumber; i++, k++)
			{
				float width = balustradeWidth;
				float dis = Vector3.Distance(columnList[i], columnList[(i + 1) % columnList.Count]) - columnRadius * 2;
				int number = Mathf.Max(Mathf.FloorToInt(dis / width), 1);
				Vector3 dir = columnList[(i + 1) % columnList.Count] - columnList[i];
				float disDiff = (dis - width * number) / number;
				width = dis / number;

				float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
				for (int j = 0; j < number; j++)
				{
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i] + heightOffset * Vector3.up;
					GameObject clone = Instantiate(eaveColumnModelStruct.balustradeModelStruct.model, pos, eaveColumnModelStruct.balustradeModelStruct.model.transform.rotation) as GameObject;
					clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.balustradeModelStruct.rotation);
					clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * (width) / balustradeWidth);
				
					clone.transform.parent = parentObj.transform;
                    balusObjList.Add(clone);
				}
			}
		}
	}

    /**
     * 計算膨脹列比率表
     * 輸入 : 1. fInflated : 最大膨脹值 , 2. segCount : 段數
     */
    public List<float> CalculateColumeRadiusInflateList(float fInflated , int segCount)
    {
        List<float> listRad = new List<float>() { 1.0f };
        int halfSeg = segCount / 2;
        float InflateVal = fInflated / halfSeg;
        CatLine rateLine = new CatLine();
        rateLine.controlPointPosList.Add(new Vector3(1.0f ,0.0f ,0.0f));
        rateLine.controlPointPosList.Add(new Vector3(fInflated, this.eaveColumnHeight/2, 0.0f));
        rateLine.controlPointPosList.Add(new Vector3(1.0f, this.eaveColumnHeight, 0.0f));
        rateLine.SetLineNumberOfPoints(Define.Medium);
        rateLine.SetCatmullRom(0.0f);

        for (int iIndex = 1; iIndex <= segCount; iIndex++)
        {
            //** 不知為何用轉型會失敗
            // float Rate = iIndex / segCount;
            // int index = (int)((rateLine.innerPointList.Count-1) * Rate);
            listRad.Add(rateLine.innerPointList[(rateLine.innerPointList.Count-1) / (segCount + 1) * iIndex].x);  
        }

        //for (int iIndex = 0; iIndex < segCount; iIndex++)
        //{
        //    if (iIndex <= halfSeg)
        //    {
        //        listRad.Add(listRad[listRad.Count-1] + InflateVal);
        //    }
        //    else
        //    {
        //        listRad.Add(listRad[listRad.Count-1] - InflateVal);
        //    }
        //}
        listRad.Add(1.0f);
        return listRad;
    }
    /**
     * 計算膨脹列比率表 " 2 "
     * 輸入 : 1. fInflated : 最大膨脹值 , 2. fatOffset : 最大膨脹位置位移 ,3. segCount : 段數
     * 最大膨脹位置位移為 (-1~1 表示柱子的 1/4高處 到 3/4處)
     */
    public List<float> CalculateColumeRadiusInflateList(float fInflated , float fatOffset ,int segCount)
    {
        List<float> listRad = new List<float>() { 1.0f };
        int halfSeg = segCount / 2;
        float InflateVal = fInflated / halfSeg;
        CatLine rateLine = new CatLine();
        rateLine.controlPointPosList.Add(new Vector3(1.0f, 0.0f, 0.0f));
        rateLine.controlPointPosList.Add(new Vector3(fInflated, this.eaveColumnHeight / 2 + (this.eaveColumnHeight / 4 * fatOffset) , 0.0f));
        rateLine.controlPointPosList.Add(new Vector3(1.0f, this.eaveColumnHeight, 0.0f));
        rateLine.SetLineNumberOfPoints(Define.Medium);
        rateLine.SetCatmullRom(0.0f);

        for (int iIndex = 1; iIndex <= segCount; iIndex++)
        {
            //** 不知為何用轉型會失敗
            // float Rate = iIndex / segCount;
            // int index = (int)((rateLine.innerPointList.Count-1) * Rate);
            listRad.Add(rateLine.innerPointList[(rateLine.innerPointList.Count - 1) / (segCount + 1) * iIndex].x);
        }

        //for (int iIndex = 0; iIndex < segCount; iIndex++)
        //{
        //    if (iIndex <= halfSeg)
        //    {
        //        listRad.Add(listRad[listRad.Count-1] + InflateVal);
        //    }
        //    else
        //    {
        //        listRad.Add(listRad[listRad.Count-1] - InflateVal);
        //    }
        //}
        listRad.Add(1.0f);
        return listRad;
    }

    /**
     * 調整位置往屋身中心位移
     */
    public Vector3 AdjustPostionToCenterOffset(Vector3 bodyCenter,Vector3 pos , float offset)
    {
        if (offset == 0)
        {
            return pos;
        }
		Vector3 centerDir = new Vector3(bodyCenter.x - pos.x, 0, bodyCenter.z - pos.z).normalized;
        Vector3 newPos = pos + centerDir * offset;
        return newPos;
    }
}
