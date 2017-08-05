﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * 建築資訊
 * Name : 變數實際名稱
 * val : x為變數最小值，y實際值，z最大值
 */
public class DataInfo
{
    public String Name;
    public Vector3 Value;

    public DataInfo()
    {
        Value = Vector3.zero;
    }
    public DataInfo(String str, Vector3 val)
    {
        Name = str;
        Value = val;
    }
    public DataInfo Clone()
    {
        DataInfo data = new DataInfo();
        data.Name = this.Name;
        data.Value = this.Value;
        return data;
    }
}

public class DataCenter : Singleton<DataCenter> {

	// Use this for initialization
	void Start () {
        //** 暫放
        //Type objType = this.GetType();
        //objType.GetField("platHeight").SetValue(this, 10);
        //PropertyInfo[] properties = objType.GetProperties();
        //FieldInfo[] tFields = objType.GetFields();
        //print("platWidth : " + objType.GetField("platHeight").GetValue(this));

    }

    // Update is called once per frame
    void Update () {
		
	}

    /**
     * 將建築資料轉換成陣列
     * 目前想到三種方式:
     * 1.用個二維list  List<List<float>>，將各樓層與其參數記錄下來
     * 2.用個二維list  List<List<vector3>>，將其變數最小最大值放在X、Z，而 Y 放參數值
     * 3.用個 Dic 
     */
    void BuildingDataToArray(List<BuildingObj> Buildings)
    {
        List<List<float>> DataList = new List<List<float>>();
        for (int iIndex = 0; iIndex < Buildings.Count; iIndex++)
        {
            BuildingObj nowFloor = Buildings[iIndex];
            List<float> floorDataList = new List<float>();

            floorDataList.Add(nowFloor.platformController.platWidth);
            floorDataList.Add(nowFloor.platformController.platLength);
            floorDataList.Add(nowFloor.platformController.platHeight);
            floorDataList.Add(Convert.ToInt32(nowFloor.platformController.isStair));
            floorDataList.Add(Convert.ToInt32(nowFloor.platformController.isBorder));

            floorDataList.Add(nowFloor.bodyController.eaveColumnHeight);
            floorDataList.Add(nowFloor.bodyController.goldColumnbayNumber);
            floorDataList.Add(nowFloor.bodyController.eaveColumnbayNumber);
            floorDataList.Add(nowFloor.bodyController.unitNumberInBay);
            floorDataList.Add(nowFloor.bodyController.doorNumber);
            floorDataList.Add(nowFloor.bodyController.eaveColumnRadius);
            floorDataList.Add(nowFloor.bodyController.goldColumnRadius);
            floorDataList.Add(Convert.ToInt32(nowFloor.bodyController.isGoldColumn));
            floorDataList.Add(Convert.ToInt32(nowFloor.bodyController.isFrieze));
            floorDataList.Add(Convert.ToInt32(nowFloor.bodyController.isBalustrade));

            floorDataList.Add(nowFloor.roofController.allJijaHeight);
            floorDataList.Add(nowFloor.roofController.mainRidgeHeightOffset);
            floorDataList.Add(nowFloor.roofController.flyEaveHeightOffset);
            floorDataList.Add(nowFloor.roofController.roofSurfaceHeightOffset);
            floorDataList.Add(nowFloor.roofController.eaveCurveHeightOffset);

            DataList.Add(floorDataList);
        }

    }

    /**
     * 每個樓層皆用一個Dic ，內部分為屋頂屋身基座三個部分(皆為List)，最後再放入List中
     */
    public List<Dictionary<string, List<DataInfo>>> BuildingDataToArrayMethod3(List<BuildingObj> Buildings)
    {
        List<Dictionary<string, List<DataInfo>>> DataList = new List<Dictionary<string, List<DataInfo>>>();
        

        for (int iIndex = 0; iIndex < Buildings.Count; iIndex++)
        {
            BuildingObj nowFloor = Buildings[iIndex];
            Dictionary<string, List<DataInfo>> dicData = new Dictionary<string, List<DataInfo>>();
            List<DataInfo> roofDataList = new List<DataInfo>();
            List<DataInfo> bodyDataList = new List<DataInfo>();
            List<DataInfo> platDataList = new List<DataInfo>();

            platDataList.Add(new DataInfo("platWidth", 
                                            new Vector3(0, nowFloor.platformController.platWidth, 10)));
            platDataList.Add(new DataInfo("platLength",
                                            new Vector3(0, nowFloor.platformController.platLength, 10)));
            platDataList.Add(new DataInfo("platHeight",
                                            new Vector3(0, nowFloor.platformController.platHeight, 10)));
            platDataList.Add(new DataInfo("isStair",
                                            new Vector3(0, Convert.ToInt32(nowFloor.platformController.isStair), 1)));
            platDataList.Add(new DataInfo("isBorder",
                                            new Vector3(0, Convert.ToInt32(nowFloor.platformController.isBorder), 1)));

            bodyDataList.Add(new DataInfo("eaveColumnHeight",
                                            new Vector3(0, nowFloor.bodyController.eaveColumnHeight,10)));
            bodyDataList.Add(new DataInfo("goldColumnbayNumber",
                                            new Vector3(0, nowFloor.bodyController.goldColumnbayNumber, 10)));
            bodyDataList.Add(new DataInfo("eaveColumnbayNumber",
                                           new Vector3(0, nowFloor.bodyController.eaveColumnbayNumber, 10)));
            bodyDataList.Add(new DataInfo("unitNumberInBay",
                                            new Vector3(0, nowFloor.bodyController.unitNumberInBay, 10)));
            bodyDataList.Add(new DataInfo("doorNumber",
                                            new Vector3(0, nowFloor.bodyController.doorNumber, 10)));
            bodyDataList.Add(new DataInfo("eaveColumnRadius",
                                            new Vector3(0, nowFloor.bodyController.eaveColumnRadius, 10)));
            bodyDataList.Add(new DataInfo("goldColumnRadius",
                                            new Vector3(0, nowFloor.bodyController.goldColumnRadius, 10)));
            bodyDataList.Add(new DataInfo("isGoldColumn",
                                             new Vector3(0, Convert.ToInt32(nowFloor.bodyController.isGoldColumn), 1)));
            bodyDataList.Add(new DataInfo("isFrieze",
                                             new Vector3(0, Convert.ToInt32(nowFloor.bodyController.isFrieze), 1)));
            bodyDataList.Add(new DataInfo("isBalustrade",
                                             new Vector3(0, Convert.ToInt32(nowFloor.bodyController.isBalustrade), 1)));

            roofDataList.Add(new DataInfo("allJijaHeight",
                                           new Vector3(8, nowFloor.roofController.allJijaHeight, 20)));
            roofDataList.Add(new DataInfo("mainRidgeHeightOffset",
                                           new Vector3(-3, nowFloor.roofController.mainRidgeHeightOffset, 3)));
            roofDataList.Add(new DataInfo("flyEaveHeightOffset",
                                           new Vector3(-3, nowFloor.roofController.flyEaveHeightOffset, 3)));
            roofDataList.Add(new DataInfo("roofSurfaceHeightOffset",
                                           new Vector3(-3, nowFloor.roofController.roofSurfaceHeightOffset, 3)));
            roofDataList.Add(new DataInfo("eaveCurveHeightOffset",
                                           new Vector3(-3, nowFloor.roofController.eaveCurveHeightOffset, 3)));

            dicData.Add(Define.PlatformDataList, platDataList);
            dicData.Add(Define.BodyDataList, bodyDataList);
            dicData.Add(Define.RoofDataList, roofDataList);
            DataList.Add(dicData);
        }
        return DataList;
    }

    /**
     * 將資料陣列轉換成建築資訊
     */
    public void ArrayDataToBuildingDataMethod3(List<Dictionary<string, List<DataInfo>>> DataList)
    {
        for (int iIndex = 0; iIndex < DataList.Count; iIndex++)
        {
            Dictionary<string, List<DataInfo>> dicData = DataList[iIndex];
            List<DataInfo> platDataList = dicData[Define.PlatformDataList];
            List<DataInfo> bodyDataList = dicData[Define.BodyDataList];
            List<DataInfo> roofDataList = dicData[Define.RoofDataList];
            BuildingObj nowBuilding = MainController.Instance.Buildings[iIndex];
            for (int i = 0; i < platDataList.Count; i++)
            {
                Type objType = nowBuilding.platformController.GetType();
                try
                {
                    objType.GetField(platDataList[i].Name).SetValue(nowBuilding.platformController, platDataList[i].Value.y);
                }
                catch
                {
                    print("Error :"+ platDataList[i].Name+" have problem");
                }
            }
            for (int i = 0; i < bodyDataList.Count; i++)
            {
                Type objType = nowBuilding.bodyController.GetType();
                try
                {
                    objType.GetField(bodyDataList[i].Name).SetValue(nowBuilding.bodyController, bodyDataList[i].Value.y);
                }
                catch
                {
                    print("Error :" + bodyDataList[i].Name + " have problem");
                }
            }
            for (int i = 0; i < roofDataList.Count; i++)
            {
                Type objType = nowBuilding.roofController.GetType();
                try
                {
                    objType.GetField(roofDataList[i].Name).SetValue(nowBuilding.roofController, roofDataList[i].Value.y);
                }
                catch
                {
                    print("Error :" + roofDataList[i].Name + " have problem");
                }
            }


        }

        //Type objType = this.GetType();
        //objType.GetField("platHeight").SetValue(this, 10);
        //PropertyInfo[] properties = objType.GetProperties();
        //FieldInfo[] tFields = objType.GetFields();
        //print("platWidth : " + objType.GetField("platHeight").GetValue(this));
    }

	public void ReadXml(string fileName)
	{
		PlistCS.Plist.readPlist(fileName);
	}


	public void WriteXml(string fileName, Dictionary<string, object> dicFile)
	{
		PlistCS.Plist.writeXml(dicFile, fileName);
		//CheckDictionary((Dictionary<string, object>)Plist.readPlist(targetXmlPath));
	}





}
