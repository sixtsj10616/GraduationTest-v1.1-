using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/**
 * 建築資訊
 * Name : 變數實際名稱
 * val : x為變數最小值，y實際值，z最大值
 */
public class DataInfo
{
    public String Name;
    public Vector3 Value;
	public string UIName;

    public DataInfo()
    {
        Value = Vector3.zero;
    }
    public DataInfo(String str, Vector3 val,string uiName)
    {
        Name = str;
        Value = val;
		UIName = uiName;
    }
	public DataInfo(String str, Vector3 val)
	{
		Name = str;
		Value = val;
		UIName = null;
	}
    public DataInfo Clone()
    {
        DataInfo data = new DataInfo();
        data.Name = this.Name;
        data.Value = this.Value;
		data.UIName = this.UIName;
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
    void BuildingDataToArray(List<BuildingObj> Buildings)           //*** 方法一:這個廢棄
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
    public List<Dictionary<string, List<DataInfo>>> BuildingDataToArrayMethod3(List<BuildingObj> Buildings)     //*** 方法三:目前
    {
        List<Dictionary<string, List<DataInfo>>> DataList = new List<Dictionary<string, List<DataInfo>>>();
        

        for (int iIndex = 0; iIndex < Buildings.Count; iIndex++)
        {
            BuildingObj nowFloor = Buildings[iIndex];
            Dictionary<string, List<DataInfo>> dicData = new Dictionary<string, List<DataInfo>>();
            List<DataInfo> roofDataList = new List<DataInfo>();
            List<DataInfo> bodyDataList = new List<DataInfo>();
            List<DataInfo> platDataList = new List<DataInfo>();
			#region PLATFORM_PARAMETER
            platDataList.Add(new DataInfo("platWidth",
											new Vector3(Define.Instance.getSetData("platWidth").Value.x,										 nowFloor.platformController.platWidth,
														Define.Instance.getSetData("platWidth").Value.z),
											Define.Instance.getSetData("platWidth").UIName));
            platDataList.Add(new DataInfo("platLength",
											new Vector3(Define.Instance.getSetData("platLength").Value.x, 
														nowFloor.platformController.platLength,
														Define.Instance.getSetData("platLength").Value.z),
											Define.Instance.getSetData("platLength").UIName));
            platDataList.Add(new DataInfo("platHeight",
											new Vector3(Define.Instance.getSetData("platHeight").Value.x,
														nowFloor.platformController.platHeight,
														Define.Instance.getSetData("platHeight").Value.z),
											Define.Instance.getSetData("platHeight").UIName));
            platDataList.Add(new DataInfo("isStair",
											new Vector3(Define.Instance.getSetData("isStair").Value.x,
														Convert.ToInt32(nowFloor.platformController.isStair),
														Define.Instance.getSetData("isStair").Value.z),
											Define.Instance.getSetData("isStair").UIName));
            platDataList.Add(new DataInfo("isBorder",
											new Vector3(Define.Instance.getSetData("isBorder").Value.x, 
														Convert.ToInt32(nowFloor.platformController.isBorder),
														Define.Instance.getSetData("isBorder").Value.z),
											Define.Instance.getSetData("isBorder").UIName));

            #endregion
			#region BODY_PARAMETER

            bodyDataList.Add(new DataInfo("goldColumnbayNumber",
											new Vector3(Define.Instance.getSetData("goldColumnbayNumber").Value.x, 
														nowFloor.bodyController.goldColumnbayNumber,
														Define.Instance.getSetData("goldColumnbayNumber").Value.z),
											Define.Instance.getSetData("goldColumnbayNumber").UIName));
            //bodyDataList.Add(new DataInfo("eaveColumnbayNumber",
            //                               new Vector3(0, nowFloor.bodyController.eaveColumnbayNumber, 10)));
            bodyDataList.Add(new DataInfo("unitNumberInBay",
											new Vector3(Define.Instance.getSetData("unitNumberInBay").Value.x, 
														nowFloor.bodyController.unitNumberInBay,
														Define.Instance.getSetData("unitNumberInBay").Value.z),
											Define.Instance.getSetData("unitNumberInBay").UIName));
			//門的最大數量為(金柱數量的一半加一)
			bodyDataList.Add(new DataInfo("doorNumber",
											new Vector3(Define.Instance.getSetData("doorNumber").Value.x,
														 nowFloor.bodyController.doorNumber,
														Define.Instance.getSetData("doorNumber").Value.y / 2 + 1),
											Define.Instance.getSetData("doorNumber").UIName));
            //bodyDataList.Add(new DataInfo("eaveColumnRadius",
            //                                new Vector3(0, nowFloor.bodyController.eaveColumnRadius, 10)));
            //bodyDataList.Add(new DataInfo("goldColumnRadius",
            //                                new Vector3(0, nowFloor.bodyController.goldColumnRadius, 10)));

            bodyDataList.Add(new DataInfo("eaveColRadInflate",
											new Vector3(Define.Instance.getSetData("eaveColRadInflate").Value.x, 
														nowFloor.bodyController.eaveColRadInflate,
														Define.Instance.getSetData("eaveColRadInflate").Value.z),
											Define.Instance.getSetData("eaveColRadInflate").UIName));
            bodyDataList.Add(new DataInfo("eaveColumnHeight",
											new Vector3(Define.Instance.getSetData("eaveColumnHeight").Value.x,
														nowFloor.bodyController.eaveColumnHeight,
														Define.Instance.getSetData("eaveColumnHeight").Value.z),
											Define.Instance.getSetData("eaveColumnHeight").UIName));
            //bodyDataList.Add(new DataInfo("eaveColTopOffset",
            //                               new Vector3(0, nowFloor.bodyController.eaveColTopOffset, 8)));
            //bodyDataList.Add(new DataInfo("eaveColBotOffset",
            //                               new Vector3(0, nowFloor.bodyController.eaveColBotOffset, 8)));
            bodyDataList.Add(new DataInfo("isGoldColumn",
											 new Vector3(Define.Instance.getSetData("isGoldColumn").Value.x, 
														 Convert.ToInt32(nowFloor.bodyController.isGoldColumn),
														 Define.Instance.getSetData("isGoldColumn").Value.z),
											 Define.Instance.getSetData("isGoldColumn").UIName));
            bodyDataList.Add(new DataInfo("isFrieze",
											 new Vector3(Define.Instance.getSetData("isFrieze").Value.x, 
														 Convert.ToInt32(nowFloor.bodyController.isFrieze),
														 Define.Instance.getSetData("isFrieze").Value.z),
											 Define.Instance.getSetData("isFrieze").UIName));
            bodyDataList.Add(new DataInfo("isBalustrade",
											 new Vector3(Define.Instance.getSetData("isBalustrade").Value.x,
														 Convert.ToInt32(nowFloor.bodyController.isBalustrade),
														 Define.Instance.getSetData("isBalustrade").Value.z),
											 Define.Instance.getSetData("isBalustrade").UIName));
			#endregion
			#region ROOF_PARAMETER 
            roofDataList.Add(new DataInfo("allJijaHeight",
										   new Vector3(Define.Instance.getSetData("allJijaHeight").Value.x, 
													   nowFloor.roofController.allJijaHeight,
													   Define.Instance.getSetData("allJijaHeight").Value.z),
										   Define.Instance.getSetData("allJijaHeight").UIName));
            roofDataList.Add(new DataInfo("mainRidgeHeightOffset",
										   new Vector3(Define.Instance.getSetData("mainRidgeHeightOffset").Value.x, 
													   nowFloor.roofController.mainRidgeHeightOffset,
													   Define.Instance.getSetData("mainRidgeHeightOffset").Value.z),
										  Define.Instance.getSetData("mainRidgeHeightOffset").UIName));
            roofDataList.Add(new DataInfo("flyEaveHeightOffset",
										   new Vector3(Define.Instance.getSetData("flyEaveHeightOffset").Value.x,
													   nowFloor.roofController.flyEaveHeightOffset,
													   Define.Instance.getSetData("flyEaveHeightOffset").Value.z),
											Define.Instance.getSetData("flyEaveHeightOffset").UIName)); 
            roofDataList.Add(new DataInfo("eaveCurveHeightOffset",
										   new Vector3(Define.Instance.getSetData("eaveCurveHeightOffset").Value.x, 
													   nowFloor.roofController.eaveCurveHeightOffset,
													   Define.Instance.getSetData("eaveCurveHeightOffset").Value.z),
											Define.Instance.getSetData("eaveCurveHeightOffset").UIName));
            roofDataList.Add(new DataInfo("roofSurfaceHeightOffset",
										   new Vector3(Define.Instance.getSetData("roofSurfaceHeightOffset").Value.x, 
													   nowFloor.roofController.roofSurfaceHeightOffset,
													   Define.Instance.getSetData("roofSurfaceHeightOffset").Value.z),
											Define.Instance.getSetData("roofSurfaceHeightOffset").UIName));
			#endregion
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
                    Type tryType = objType.GetField(bodyDataList[i].Name).FieldType;
                    if (tryType == typeof(int))
                    {
                        objType.GetField(bodyDataList[i].Name).SetValue(nowBuilding.bodyController, (int)bodyDataList[i].Value.y);
                    }
                    else if (tryType == typeof(float))
                    {
                        objType.GetField(bodyDataList[i].Name).SetValue(nowBuilding.bodyController, bodyDataList[i].Value.y);
                    }
                    else
                    {
                        objType.GetField(bodyDataList[i].Name).SetValue(nowBuilding.bodyController, bodyDataList[i].Value.y);
                    }
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
    }
	//
	public void BuildingDataToMenu(List<BuildingObj> Buildings,int floorIndex) 
	{
		List<Dictionary<string, List<DataInfo>>> DataList = BuildingDataToArrayMethod3(Buildings);
		Debug.Log("UpdateArrayDataToBuildingData");
		Dictionary<string, List<DataInfo>> Data = DataList[floorIndex];
		MainMenuController.Instance.plamformHandler.UpdateMenuInfo(Data);
		MainMenuController.Instance.bodyMenuHandler.UpdateMenuInfo(Data);
		MainMenuController.Instance.roofMenuHandler.UpdateMenuInfo(Data);

	}
    /**
     * 將初步得到的檔案資料化為建築資料
     * 目前讀出來初步化簡的格式為 List<object>
     * 需要轉化成 List<Dictionary<string, List<DataInfo>>>
     */
    public List<Dictionary<string, List<DataInfo>>> changeFileDataToBuildingData(List<object> fileData ,int iDataIndex)
    {
        List<object> data = fileData[iDataIndex] as List<object>;
        List<Dictionary<string, List<DataInfo>>> newDataList = new List<Dictionary<string, List<DataInfo>>>();
        Dictionary<string, List<DataInfo>> dicData = new Dictionary<string, List<DataInfo>>();
        string[] aryDataListName = Define.Instance.aryDataListName;         //** 建築物部位名稱(屋頂、屋身、基座)
        int iDataCount = 0;                                                 //** 目前在data中的參數索引
        for (int iIndex = 0; iIndex < aryDataListName.Length; iIndex++)
        {
            string[] arySetDataName = Define.Instance.queryDataNameArrayByName(aryDataListName[iIndex]);
            List<DataInfo> SetDataList = new List<DataInfo>();

            for (int i = 0; i <arySetDataName.Length; i ++)
            {
                string strName = arySetDataName[i];
                DataInfo dicSettingData = Define.Instance.getSetData(strName);
                double value = Convert.ToDouble(data[iDataCount]);
                Vector3 v3Data = new Vector3(dicSettingData.Value.x, (float)value, dicSettingData.Value.z);
                SetDataList.Add(new DataInfo(strName, v3Data));
                iDataCount++;
            }
            dicData.Add(aryDataListName[iIndex], SetDataList);
        }
        newDataList.Add(dicData);
        return newDataList;
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
