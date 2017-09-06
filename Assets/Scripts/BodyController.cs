﻿using UnityEngine;
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
	private BuildingObj parentObj;
	//Body******************************************************************************
	const float CUN = 3.33f;
	public enum BodyType { Chuan_Dou = 0, Tai_Liang = 1 };//Chuan_Dou 穿斗式 ,Tai_Liang 抬梁式

	public BodyType bodyType = BodyType.Chuan_Dou;

	public float eaveColumnRatio2platformOffset;

	public float goldColumnRatio2platformOffset;

	public int goldColumnbayNumber = 5;//間數量

	public int eaveColumnbayNumber = 1;
	public int unitNumberInBay = 2;                 //間內有幾個單位(如:一間內有幾個門+窗)
    public int doorNumber =1;                       //門的數量(若間為奇數 則門為單扇 / 偶數 則門為雙扇)
    public int eaveColOffset = 0;                   //* 簷柱的位移 ( 0:為一般 ，> 0 表簷柱上方位移 ，0 < 表簷柱下方位移)，完成後可砍掉下面兩個參數
    public float eaveColTopOffset = 0.0f;           //* 簷柱上方位移
    public float eaveColBotOffset = 0.0f;           //* 簷柱下方位移
    public float eaveColumnHeight;

	public float goldColumnHeight;
	public float eaveColumnRadius = 0.5f;
	public float goldColumnRadius = 0.5f;
    public float eaveColRadInflate = 1.0f;          //* 簷柱膨脹值
	public float columnFundationHeight;//柱礎高度
	public bool isGoldColumn = true;
	public bool isFrieze = true;
	public bool isBalustrade = true;

    public List<Vector3> origBotPosList = new List<Vector3>();      //* 初始化時所傳入的底座位置列表
    public List<Vector3> eaveColumnPosList = new List<Vector3>();   //* 簷柱位置列表
    public List<Vector3> goldColumnPosList = new List<Vector3>();   //* 金柱位置列表
    public List<GameObject> windowObjList = new List<GameObject>(); //* 窗戶物件列表
    public List<GameObject> doorObjList = new List<GameObject>();   //* 大門物件列表
    public List<GameObject> friezeObjList = new List<GameObject>(); //* 門楣物件列表
    public List<GameObject> balusObjList = new List<GameObject>();  //* 門楣物件列表
    List<float> eaveColRadList;                                     //* 簷柱半徑比例列表
    //***********************************************************************
    public List<ColumnStruct> eaveColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> goldColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> eaveCornerColumnList = new List<ColumnStruct>();
	public List<ColumnStruct> goldCornerColumnList = new List<ColumnStruct>();
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
	public List<Vector3> GetColumnStructPosList(List<ColumnStruct> columnStructList)
	{
		List<Vector3> posList = new List<Vector3>();
		for (int i = 0; i < columnStructList.Count; i++)
		{
			posList.Add((columnStructList[i].bottomPos + columnStructList[i].topPos) / 2.0f);
		}
		return posList;
	}
    /**
     * 初始化
     */
    public void InitFunction(BuildingObj parentObj, List<Vector3> bottomPosList, float platformFrontWidth, float platformHeight, float eaveColumnHeight, float goldColumnHeight)
	{
		//初始值******************************************************************************

		this.parentObj = parentObj;
		this.eaveColumnHeight = eaveColumnHeight;
		this.goldColumnHeight = goldColumnHeight;
        this.origBotPosList = bottomPosList;                //** 初始時先記下原始底座的 PosList

        columnFundationHeight = eaveColumnHeight * 0.05f;

		//eaveColumnRatio2platformOffset = ((MainController.Instance.Buildings.Count == 0) ? (platformFrontWidth * 0.1f) : parentObj.bodyController.eaveColumnRadius * 3);
		//goldColumnRatio2platformOffset = ((MainController.Instance.Buildings.Count == 0) ? (eaveColumnRatio2platformOffset * 2) : parentObj.bodyController.eaveColumnRadius * 3);
		eaveColumnRatio2platformOffset = (platformFrontWidth * 0.1f);
		goldColumnRatio2platformOffset = (eaveColumnRatio2platformOffset * 2);
		Debug.Log("eaveColumnRatio2platformOffset" + eaveColumnRatio2platformOffset);
		parentObj.bodyCenter = parentObj.platformCenter + (platformHeight / 2.0f + eaveColumnHeight / 2.0f) * Vector3.up;

		//**************************************************************************************
		switch (bodyType)
		{
			#region Chuan_Dou
			case BodyType.Chuan_Dou:
				CreateBody(bottomPosList, parentObj.entraneIndexList.List, parentObj.bodyCenter);
				if (goldColumnList.Count > 0 )
				{
					CreateRingWall(ModelController.Instance.goldColumnModelStruct, GetColumnStructBottomPosList(goldColumnList), goldColumnRadius, unitNumberInBay, goldColumnbayNumber, doorNumber);
				}
                if (eaveColumnList.Count > 0 && isFrieze)
                {
					CreateRingFrieze(ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.7f * eaveColumnHeight);
                }
                if (eaveColumnList.Count > 0 && isBalustrade)
                {
					CreateRingBalustrade(ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.1f * eaveColumnHeight);
                }
                break;
			#endregion
		}
	}
    /**
     * 重製屋身以簡易的參數
     * 原本的初始化所需參數過多，目前重製只需要柱高
     */
    public void ResetWithSimpleInfo(BuildingObj parentObj, float platformHeight)
    {
        //初始值******************************************************************************

        this.parentObj = parentObj;

        columnFundationHeight = eaveColumnHeight * 0.05f;

        parentObj.bodyCenter = parentObj.platformCenter + (platformHeight / 2.0f + eaveColumnHeight / 2.0f) * Vector3.up;

        //**************************************************************************************
        switch (bodyType)
        {
            #region Chuan_Dou
            case BodyType.Chuan_Dou:
				//計算bay柱子群位置
				CreateBody(origBotPosList, parentObj.entraneIndexList.List, parentObj.bodyCenter);
                if (goldColumnList.Count > 0 )
                {
					//建造整圈牆模型
					CreateRingWall(ModelController.Instance.goldColumnModelStruct, GetColumnStructBottomPosList(goldColumnList), goldColumnRadius, unitNumberInBay, goldColumnbayNumber, doorNumber);
                }
                if (eaveColumnList.Count > 0 && isFrieze)
                {
					//建造門楣模型
                    CreateRingFrieze(ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.7f * eaveColumnHeight);
                }
                if (eaveColumnList.Count > 0 && isBalustrade)
                {
					//建立柵欄模型
                    CreateRingBalustrade(ModelController.Instance.eaveColumnModelStruct, GetColumnStructBottomPosList(eaveColumnList), eaveColumnRadius, 0.1f * eaveColumnHeight);
                }
                break;
                #endregion
        }
    }
    /**
     * 調整柱子位置列表 (!!注意目前只有四邊形，方法感覺也可以在調整)
     * 輸入位移(z:面寬，x:進深)
     * 流程 : 取出 0 號柱子位置並加上位移量，之後算與原始位置的放大比率，
     * 將其他的柱子也都乘上此比率
     */
    public void UpdateOrigBottonPos(float offsetZ, float offsetX)
    {
        if (MainController.Instance.sides == MainController.FormFactorSideType.FourSide)
        {
            //print("offsetZ : " + offsetZ + " offsetX : " + offsetX);
            //** 第一行暫時這樣，感覺會有問題
            Vector3 pos = new Vector3(origBotPosList[0].x + offsetX, origBotPosList[0].y, origBotPosList[0].z - offsetZ);
            Vector3 scaleVec = new Vector3(pos.x / origBotPosList[0].x, pos.y / origBotPosList[0].y, pos.z / origBotPosList[0].z);
            for (int iIndex = 0; iIndex < 4; iIndex++)
            {
                Vector3 newPos = origBotPosList[iIndex];
                newPos.Scale(scaleVec);
                origBotPosList[iIndex] = newPos;
				Debug.Log("origBotPosList[" + iIndex + "] " + origBotPosList[iIndex]);
            }
         
            //for (int iIndex = 1; iIndex < 4; iIndex++)
            //{
            //    pos = Quaternion.AngleAxis(90, Vector3.up) * pos;
            //    origBotPosList[iIndex] = pos;
            //}
        }
    }

    //?????????????????????????????不用更新
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
        //Vector3 topPos = AdjustPostionToCenterOffset(pos + (height / 2.0f) * Vector3.up , eaveColTopOffset);
        //Vector3 bottomPos = AdjustPostionToCenterOffset(pos - (height / 2.0f - fundationHeight) * Vector3.up , eaveColBotOffset);
        Vector3 topPos = pos + (height / 2.0f) * Vector3.up;
        Vector3 bottomPos = pos - (height / 2.0f - fundationHeight) * Vector3.up;
        if (this.eaveColOffset != 0)
        {
            if (eaveColOffset > 0)
            {
                topPos = AdjustPostionToCenterOffset(topPos , eaveColOffset);
            }
            else
            {
                bottomPos = AdjustPostionToCenterOffset(bottomPos, -eaveColOffset);
            }
        }

        //print("orig pos :" + (pos + (height / 2.0f) * Vector3.up));
        //print("topPos :" + topPos);
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
		//topPos = AdjustPostionToCenterOffset(pos + (fundationHeight) * Vector3.up - (height / 2.0f) * Vector3.up , eaveColBotOffset);
		//bottomPos = AdjustPostionToCenterOffset(pos - (height / 2.0f) * Vector3.up , eaveColBotOffset);
        topPos = pos + (fundationHeight) * Vector3.up - (height / 2.0f) * Vector3.up;
        bottomPos = pos - (height / 2.0f) * Vector3.up;
        if (eaveColOffset < 0)
        {
            topPos = AdjustPostionToCenterOffset(topPos , -eaveColOffset);
            bottomPos = AdjustPostionToCenterOffset(bottomPos , -eaveColOffset);
        }

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
    private void CreateBody(List<Vector3> posList, List<int> entranceIndexList, Vector3 bodyCenter)
    {
        eaveCornerColumnList.Clear();
		goldCornerColumnList.Clear();
        goldColumnList.Clear();
        eaveColumnList.Clear();
        eaveColumnPosList = CalculateEveaColumnPos(posList, entranceIndexList, bodyCenter);
        goldColumnPosList = CalculateGoldColumnPos(posList, entranceIndexList, bodyCenter);

        //*** 屋身柱子膨脹，直接做一個柱身各節半徑列表
        if (this.eaveColRadInflate != 0)
        {
            this.eaveColRadList = CalculateColumeRadiusInflateList(this.eaveColRadInflate, Define.initColRadSegment); 
        }
        else
        {
            this.eaveColRadList = new List<float>() { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
        }
        eaveColumnList = CreateRingColumn(parentObj.body, eaveColumnPosList, eaveColumnRadius, eaveColumnRadius, eaveColumnHeight, eaveColumnRadius * 1.2f, columnFundationHeight, "EaveColumn");
        if (isGoldColumn)
        {
            goldColumnList = CreateRingColumn(parentObj.body, goldColumnPosList, goldColumnRadius, goldColumnRadius, goldColumnHeight, goldColumnRadius * 1.2f, columnFundationHeight, "GoldColumn");
        }

        //角柱計算
        if (eaveColumnbayNumber <= 0) eaveColumnbayNumber = 1;
        for (int i = 0, count = 0; i < (int)MainController.Instance.sides; i++)
        {
            eaveCornerColumnList.Add(eaveColumnList[count]);
            if (entranceIndexList.Contains(i)) count++;
            else count += eaveColumnbayNumber;
        }
		if (isGoldColumn)
		{
			if (goldColumnbayNumber <= 0) goldColumnbayNumber = 1;
			for (int i = 0, count = 0; i < (int)MainController.Instance.sides; i++)
			{
				goldCornerColumnList.Add(goldColumnList[count]);
				count += goldColumnbayNumber;
			}
		}
    }
	 /**
  * 計算簷柱位置
  */
    private List<Vector3> CalculateEveaColumnPos(List<Vector3> posList, List<int> entranceIndexList, Vector3 bodyCenter)
    {
        List<Vector3> eaveColumnPosList = new List<Vector3>();
        for (int i = 0; i < posList.Count; i++)
        {
            Vector2 v = new Vector2(posList[i].x - bodyCenter.x, posList[i].z - bodyCenter.z);
            //eaveColumn
            v = v.normalized * eaveColumnRatio2platformOffset;
            Vector3 eaveColumnPos = posList[i] - new Vector3(v.x, 0, v.y) + eaveColumnHeight / 2.0f * Vector3.up;
            eaveColumnPosList.Add(eaveColumnPos);

            if (!entranceIndexList.Contains(i))
            {
                //eaveBayColumn
                int nextIndex = (i + 1) % posList.Count;
                Vector2 vNext = new Vector2(posList[nextIndex].x - bodyCenter.x, posList[nextIndex].z - bodyCenter.z);
                vNext = vNext.normalized * eaveColumnRatio2platformOffset;
                Vector3 posNext = posList[nextIndex] - new Vector3(vNext.x, 0, vNext.y) + eaveColumnHeight / 2.0f * Vector3.up;

                float disBetweenEaveColumn = Vector3.Distance(eaveColumnPos, posNext);
                float bayWidth = disBetweenEaveColumn / eaveColumnbayNumber;
                Vector3 bayDir = posNext - eaveColumnPos;

                for (int j = 1; j < eaveColumnbayNumber; j++)
                {
                    Vector3 eaveBayColumnPos = bayDir.normalized * (j * bayWidth) + eaveColumnPos;
                    eaveColumnPosList.Add(eaveBayColumnPos);
                }
            }
        }
        return eaveColumnPosList;
    }
    /**
     * 計算金柱位置
     */
    public List<Vector3> CalculateGoldColumnPos(List<Vector3> posList, List<int> entranceIndexList, Vector3 bodyCenter)
    {
        List<Vector3> goldColumnPosList = new List<Vector3>();
        for (int i = 0; i < posList.Count; i++)
        {
            Vector2 v = new Vector2(posList[i].x - bodyCenter.x, posList[i].z - bodyCenter.z);
            v = new Vector2(posList[i].x - bodyCenter.x, posList[i].z - bodyCenter.z);
            v = v.normalized * goldColumnRatio2platformOffset;
            Vector3 goldColumnPos = posList[i] - new Vector3(v.x, 0, v.y) + goldColumnHeight / 2.0f * Vector3.up;
            goldColumnPosList.Add(goldColumnPos);

            int nextIndex = (i + 1) % posList.Count;
            Vector2 vNext = new Vector2(posList[nextIndex].x - bodyCenter.x, posList[nextIndex].z - bodyCenter.z);
            vNext = vNext.normalized * goldColumnRatio2platformOffset;
            Vector3 posNext = posList[nextIndex] - new Vector3(vNext.x, 0, vNext.y) + goldColumnHeight / 2.0f * Vector3.up;

            float disBetweenGoldColumn = Vector3.Distance(goldColumnPos, posNext);
            float bayWidth = disBetweenGoldColumn / goldColumnbayNumber;
            Vector3 bayDir = posNext - goldColumnPos;

            for (int j = 1; j < goldColumnbayNumber; j++)
            {
                Vector3 goldBayColumnPos = bayDir.normalized * (j * bayWidth) + goldColumnPos;
                goldColumnPosList.Add(goldBayColumnPos);
            }
        }
            return goldColumnPosList;
    }
	/**
	 * 建造整圈牆(columnList為bottom位置)
	 */
	public void CreateRingWall(GoldColumnModelStruct goldColumnModelStruct, List<Vector3> columnList, float columnRadius, int unit,int goldColumnbayNumber, int doorNumber)
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
				    //clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, clone.transform.rotation * clone.transform.GetChild(0).transform.rotation * (new Vector3(doorWidthScale, doorHeightScale, 1)));
				    clone.transform.parent = parentObj.body.transform;
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
	
					clone.transform.parent = parentObj.body.transform;
                    windowObjList.Add(clone);
                #endregion
                    #region windowInWall
                    /*
                        GameObject wall = new GameObject("Wall");
                        MeshFilter meshFilter = wall.AddComponent<MeshFilter>();
                        MeshRenderer meshRenderer = wall.AddComponent<MeshRenderer>();
                        wall.transform.parent = parentObj.body.transform;
                        meshRenderer.material.color = Color.white;
                        float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
                        Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + columnList[i];
                        //MeshCenter.Instance.CreateDoorMeshByRatio(pos, width, eaveColumnHeight, 1, 0.3f, 0.3f, 0.6f, 1.0f, rotateAngle, meshFilter);

                        //創建牆 保留部分寬度
                        float disDiff = ((width * 0.8f) > windowWidth) ? 0 : (windowWidth - (width * 0.8f));
                        float windowWidthScale = (windowWidth - disDiff) / (windowWidth);
                        float windowLengthScale = wallLengh/windowLengh;
                        MeshCenter.Instance.CreateDoorMeshByUnit(pos, width, eaveColumnHeight, wallLengh, windowWidth * windowWidthScale, windowHeight, wallLengh, 0.5f, rotateAngle, meshFilter);
                        //Window
                        GameObject clone = Instantiate(goldColumnModelStruct.windowModelStruct.model, pos, goldColumnModelStruct.windowModelStruct.model.transform.rotation) as GameObject;
                        clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler(goldColumnModelStruct.windowModelStruct.rotation);

                        clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * windowWidthScale);

                        //clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, clone.transform.rotation * clone.transform.GetChild(0).transform.rotation * (new Vector3(windowWidthScale, 1, windowLengthScale)));
                        clone.transform.parent = wall.transform;*/
                    #endregion
                }
			}
			//CreateWindowModel

		}
	}
	/**
	 * 建造整門楣(columnList為bottom位置)
	 */
	public void CreateRingFrieze(EaveColumnModelStruct eaveColumnModelStruct, List<Vector3> columnList, float columnRadius, float heightOffset)
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
				//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, clone.transform.rotation * clone.transform.GetChild(0).transform.rotation * (new Vector3((width) / friezeWidth, 1, 1)));

				//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, Quaternion.Euler(clone.transform.GetChild(0).transform.rotation.ToEulerAngles()) * (new Vector3((width) / friezeWidth, 1, 1)));
				clone.transform.parent = parentObj.body.transform;
                friezeObjList.Add(clone);
			}

			//FriezeWall(frieze上方裝飾牆)
			GameObject friezeWall = new GameObject("FriezeWall");
			MeshFilter meshFilter = friezeWall.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = friezeWall.AddComponent<MeshRenderer>();
			friezeWall.transform.parent = parentObj.body.transform;
			meshRenderer.material.color = Color.yellow;
			float rotateAngleZ = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) :  - Vector3.Angle(dir, Vector3.right));
			float friezeWallHeight = (eaveColumnHeight - heightOffset - friezeHeight / 2.0f);
			Vector3 posZ = (columnList[i] + columnList[(i + 1) % columnList.Count]) / 2.0f + (heightOffset + friezeHeight / 2.0f + friezeWallHeight / 2.0f) * Vector3.up;
			MeshCenter.Instance.CreateCubeMesh(posZ, dis, friezeWallHeight, 0.5f, rotateAngleZ, meshFilter);
            friezeObjList.Add(friezeWall);

            if (dis >= eaveColumnModelStruct.sparrowBraceModelStruct.bound.size.x * 2.5f)
			{
				//sparrowBrace雀替
				Vector3 posX = dir.normalized * (columnRadius) + columnList[i] + (heightOffset - friezeHeight / 2.0f) * Vector3.up;
				float rotateAngleX = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
				GameObject sparrowBrace = Instantiate(eaveColumnModelStruct.sparrowBraceModelStruct.model, posX, eaveColumnModelStruct.sparrowBraceModelStruct.model.transform.rotation) as GameObject;
				sparrowBrace.transform.rotation = Quaternion.AngleAxis(rotateAngleX, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.sparrowBraceModelStruct.rotation);

				sparrowBrace.transform.parent = parentObj.body.transform;

				posX = -dir.normalized * (columnRadius) + columnList[(i + 1) % columnList.Count] + (heightOffset - friezeHeight / 2.0f) * Vector3.up;
				rotateAngleX = (Vector3.Dot(Vector3.forward, -dir) < 0 ? Vector3.Angle(-dir, Vector3.right) : -Vector3.Angle(-dir, Vector3.right));
				sparrowBrace = Instantiate(eaveColumnModelStruct.sparrowBraceModelStruct.model, posX, eaveColumnModelStruct.sparrowBraceModelStruct.model.transform.rotation) as GameObject;
				sparrowBrace.transform.rotation = Quaternion.AngleAxis(rotateAngleX, Vector3.up) * Quaternion.Euler(eaveColumnModelStruct.sparrowBraceModelStruct.rotation);

				sparrowBrace.transform.parent = parentObj.body.transform;
                friezeObjList.Add(sparrowBrace);
            }
		}
	}
    /**
     * 建造整欄杆
     */
    public void CreateRingBalustrade(EaveColumnModelStruct eaveColumnModelStruct, List<Vector3> columnList, float columnRadius, float heightOffset)
	{
		float balustradeWidth = eaveColumnModelStruct.balustradeModelStruct.bound.size.x;//欄杆長度
		float balustradeHeight = eaveColumnModelStruct.balustradeModelStruct.bound.size.y;//欄杆長度
		float balustradeLengh = eaveColumnModelStruct.balustradeModelStruct.bound.size.z;//欄杆深度

		//n:MainController.Instance.sides
		//i:
		for (int n = 0, i = 0; n < (int)MainController.Instance.sides; n++)
		{
			if (parentObj.entraneIndexList.Contains(n)) { i++; continue; }

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
					//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, Quaternion.Euler(clone.transform.GetChild(0).transform.rotation.ToEulerAngles()) * (new Vector3((width) / balustradeWidth, 1, 1)));
					clone.transform.parent = parentObj.body.transform;
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
    public Vector3 AdjustPostionToCenterOffset(Vector3 pos , float offset)
    {
        if (offset == 0)
        {
            return pos;
        }
        Vector3 centerDir = new Vector3(parentObj.bodyCenter.x - pos.x, 0, parentObj.bodyCenter.z - pos.z).normalized;
        Vector3 newPos = pos + centerDir * offset;
        return newPos;
    }
}
