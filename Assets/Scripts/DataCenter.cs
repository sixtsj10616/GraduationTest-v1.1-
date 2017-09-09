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

            
            //bodyDataList.Add(new DataInfo("goldColumnbayNumber",
            //                                new Vector3(0, nowFloor.bodyController.goldColumnbayNumber, 10)));
            //bodyDataList.Add(new DataInfo("eaveColumnbayNumber",
            //                               new Vector3(0, nowFloor.bodyController.eaveColumnbayNumber, 10)));
            //bodyDataList.Add(new DataInfo("unitNumberInBay",
            //                                new Vector3(0, nowFloor.bodyController.unitNumberInBay, 10)));
            //bodyDataList.Add(new DataInfo("doorNumber",
            //                                new Vector3(0, nowFloor.bodyController.doorNumber, 10)));
            //bodyDataList.Add(new DataInfo("eaveColumnRadius",
            //                                new Vector3(0, nowFloor.bodyController.eaveColumnRadius, 10)));
            //bodyDataList.Add(new DataInfo("goldColumnRadius",
            //                                new Vector3(0, nowFloor.bodyController.goldColumnRadius, 10)));
            bodyDataList.Add(new DataInfo("eaveColOffset",
                                            new Vector3(-4, nowFloor.bodyController.eaveColOffset, 4)));
            bodyDataList.Add(new DataInfo("eaveColRadInflate",
                                            new Vector3(0.5f, nowFloor.bodyController.eaveColRadInflate, 3)));
            bodyDataList.Add(new DataInfo("eaveColumnHeight",
                                            new Vector3(7.5f, nowFloor.bodyController.eaveColumnHeight, 22)));
            //bodyDataList.Add(new DataInfo("eaveColTopOffset",
            //                               new Vector3(0, nowFloor.bodyController.eaveColTopOffset, 8)));
            //bodyDataList.Add(new DataInfo("eaveColBotOffset",
            //                               new Vector3(0, nowFloor.bodyController.eaveColBotOffset, 8)));
            //bodyDataList.Add(new DataInfo("isGoldColumn",
            //                                 new Vector3(0, Convert.ToInt32(nowFloor.bodyController.isGoldColumn), 1)));
            //bodyDataList.Add(new DataInfo("isFrieze",
            //                                 new Vector3(0, Convert.ToInt32(nowFloor.bodyController.isFrieze), 1)));
            //bodyDataList.Add(new DataInfo("isBalustrade",
            //                                 new Vector3(0, Convert.ToInt32(nowFloor.bodyController.isBalustrade), 1)));

            roofDataList.Add(new DataInfo("allJijaHeight",
                                           new Vector3(8, nowFloor.roofController.allJijaHeight, 18)));
            roofDataList.Add(new DataInfo("mainRidgeHeightOffset",
                                           new Vector3(-3, nowFloor.roofController.mainRidgeHeightOffset, 3)));
            roofDataList.Add(new DataInfo("flyEaveHeightOffset",
                                           new Vector3(-3, nowFloor.roofController.flyEaveHeightOffset, 3))); 
            roofDataList.Add(new DataInfo("eaveCurveHeightOffset",
                                           new Vector3(-3, nowFloor.roofController.eaveCurveHeightOffset, 1)));
            roofDataList.Add(new DataInfo("roofSurfaceHeightOffset",
                                           new Vector3(-2, nowFloor.roofController.roofSurfaceHeightOffset, 2)));

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

        //Type objType = this.GetType();
        //objType.GetField("platHeight").SetValue(this, 10);
        //PropertyInfo[] properties = objType.GetProperties();
        //FieldInfo[] tFields = objType.GetFields();
        //print("platWidth : " + objType.GetField("platHeight").GetValue(this));
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


	public class JsonCreator
	{
		public void SetJson(BuildingObj buildingObj)
		{
			Wrapper wrapper = new Wrapper(buildingObj);
			string json2 = JsonUtility.ToJson(wrapper);
			StreamWriter fileWrite = new StreamWriter(Application.dataPath + "/Resources/myPlayer.json");
			fileWrite.Write(json2);
			fileWrite.Close();
		}
		[Serializable]
		public class TestA
		{
			Wrapper[] testAInfoArray;
			public TestA(List<BuildingObj> buildingObjList) 
			{
				List<Wrapper> testAInfo = new List<Wrapper>();
				for(int i=0;i<buildingObjList.Count;i++)
				{
					Wrapper newTestB = new Wrapper(buildingObjList[i]);
					testAInfo.Add(newTestB);
				}
				testAInfoArray = testAInfo.ToArray();
			}
		}
		[Serializable]
		public class Wrapper
		{
			public List<Vector3>array;
			public Dictionary<string, object> dicArray = new Dictionary<string, object>();
			public List<List<Vector3>> listttt =new List<List<Vector3>>();
			public Wrapper(BuildingObj buildingObj)
			{
				array = buildingObj.GetComponent<BodyController>().eaveColumnPosList;
				dicArray.Add("zzz", array);
				for(int i=0;i<3;i++)
				{
					List<Vector3> listZZZ=new List<Vector3>();
					listZZZ = new List<Vector3>(buildingObj.GetComponent<BodyController>().eaveColumnPosList);
					listttt.Add(listZZZ);
				}
			}
			
		}
	}



}
