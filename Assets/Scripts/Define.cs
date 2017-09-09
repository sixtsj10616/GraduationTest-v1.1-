using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SettingData
{
    float max;
    float min;
    float value;

    public SettingData(float fmax, float fvalue, float fmin)
    {
        max = fmax;
        value = fvalue;
        min = fmin;
    }
}    

public class Define : Singleton<Define>
{

	public const string initDataPath = "SettingData.plist";

    //***           底座          ***//
    public const float initPlatWidth    = 50;
    public const float initPlatLength   = 40;
    public const float initPlatHeight   = 5;
    public const float initStairWidth   = 5;
    public const float initStairLength  = 5;

    //***           屋身          ***//
    public const float initEaveColumnHeight   = 11;
    public const int initColRadSegment = 7;


    //***           屋頂          ***//
    public const float initJijaHeight = 13;


    //***           主脊          ***//
    public const string TopPoint = "TopPoint";
    public const string SecPoint = "SecPoint";
    public const string ThirdPoint = "ThirdPoint";
    public const string LastPoint = "LastPoint";
    public const float mainRidgeTileHeight = 0.3f;
    public const float Do_Kun_Height = 0.8f;

    //***           線段          ***//
    public const int Large = 1000;
    public const int Medium = 500;
    public const int Low = 250;

    //***           資料          ***//
    public const string RoofDataList = "RoofDataList";
    public const string BodyDataList = "BodyDataList";
    public const string PlatformDataList = "PlatformDataList";
    //public enum DataListName {Roof = 0 , Body, Plat };        //*** 當初應該用這種方式
    public string[] aryDataListName = new string[] { "RoofDataList", "BodyDataList", "PlatformDataList" };
    public string[] aryRoofDataName = new string[] { "allJijaHeight", "mainRidgeHeightOffset", "flyEaveHeightOffset", "eaveCurveHeightOffset" , "roofSurfaceHeightOffset" };
    public string[] aryBodyDataName = new string[] { "eaveColRadInflate", "eaveColOffset", "eaveColumnHeight" };
    public string[] aryPlatDataName = new string[] { };

    public Dictionary<string, object> dicSettingData = new Dictionary<string, object>();
    public Dictionary<string, DataInfo> dicSetData = new Dictionary<string, DataInfo>();

    public string[] queryDataNameArrayByName(string strName)
    {
        if (strName.Equals(aryDataListName[0]))
        {
            return aryRoofDataName;
        }
        else if (strName.Equals(aryDataListName[1]))
        {
            return aryBodyDataName;
        }
        else if (strName.Equals(aryDataListName[2]))
        {
            return aryPlatDataName;
        }
        else
            return null;
    }

    void tmpUse()
	{
		//*** 格式 : 參數名稱(須按照實際class參數命名)，內容為Dic 有三個Key :  max、init、min
		dicSettingData.Add("platWidth", new Dictionary<string, object> {{ "max", 80 } ,
																		{ "init", initPlatWidth },
																		{ "min", 30 }
																		});
        dicSettingData.Add("flyEaveHeightOffset", new Dictionary<string, object> {{ "max", 3 } ,
                                                                        { "init", 0 },
                                                                        { "min", -3 }
                                                                        });
        dicSettingData.Add("mainRidgeHeightOffset", new Dictionary<string, object> {{ "max", 3 } ,
                                                                        { "init", 0 },
                                                                        { "min", -3 }
                                                                        });
        dicSettingData.Add("eaveCurveHeightOffset", new Dictionary<string, object> {{ "max", 1 } ,
                                                                        { "init", -1 },
                                                                        { "min", -3 }
                                                                        });
        dicSettingData.Add("allJijaHeight", new Dictionary<string, object> {{ "max", 18 } ,
                                                                        { "init", 13 },
                                                                        { "min", 8 }
                                                                        });
        dicSettingData.Add("面寬", new Dictionary<string, object> {{ "max", 55 } ,
                                                                        { "init", 30 },
                                                                        { "min", 25 }
                                                                        });
        dicSettingData.Add("進深", new Dictionary<string, object> {{ "max", 55 } ,
                                                                        { "init", 30 },
                                                                        { "min", 25 }
                                                                        });
        dicSettingData.Add("eaveColRadInflate", new Dictionary<string, object> {{ "max", 3 } ,
                                                                        { "init", 1 },
                                                                        { "min", 0.5 }
                                                                        });
        dicSettingData.Add("eaveColOffset", new Dictionary<string, object> {{ "max", 4 } ,  //*** 此參數目前沒有，若小於0就代表柱子底部內縮
                                                                        { "init", 0 },      //*** 若大於0就代表柱子頂部內縮
                                                                        { "min", -4 }       //*** 感覺還是要有用比例
                                                                        });
    }

    public void createSettingData()
    {
        dicSetData = new Dictionary<string, DataInfo>();


        dicSetData.Add("flyEaveHeightOffset", new DataInfo("flyEaveHeightOffset",
                                        new Vector3(-3, initPlatWidth, 3)));
        dicSetData.Add("mainRidgeHeightOffset", new DataInfo("mainRidgeHeightOffset",
                                        new Vector3(-3, 0, 3)));
        dicSetData.Add("eaveCurveHeightOffset", new DataInfo("eaveCurveHeightOffset",
                                        new Vector3(-3, -1, 1)));
        dicSetData.Add("allJijaHeight", new DataInfo("allJijaHeight",
                                        new Vector3(8, 13, 18)));
        dicSetData.Add("roofSurfaceHeightOffset", new DataInfo("roofSurfaceHeightOffset",
                                        new Vector3(-2, -1, 2)));


        dicSetData.Add("eaveColRadInflate", new DataInfo("eaveColRadInflate",
                                        new Vector3(0.5f, 1, 3)));
        dicSetData.Add("eaveColOffset", new DataInfo("eaveColOffset",           //*** 此參數目前沒有，若小於0就代表柱子底部內縮
                                        new Vector3(-4, 0, 4)));                //*** 若大於0就代表柱子頂部內縮，感覺還是要有用比例，
        dicSetData.Add("eaveColumnHeight", new DataInfo("eaveColumnHeight",
                                       new Vector3(7.5f, 11, 22)));
        //data.Add("面寬", new DataInfo("面寬",
        //                                new Vector3(25, 30, 55)));
        //data.Add("進深", new DataInfo("進深",
        //                                new Vector3(25, 30, 55)));

        dicSetData.Add("platWidth", new DataInfo("platWidth",
                                        new Vector3(30, initPlatWidth, 80)));


     
    }
    //*** 可砍
    public Dictionary<string, object> getSettingData(string dataName)
    {
        return dicSettingData[dataName] as Dictionary<string, object>;
    }
    /**
     * 取得設定資料的區間與初始值
     * 輸入要找的參數名稱
     */
    public DataInfo getSetData(string dataName)
    {
        if (dicSetData.Count == 0 )
        {
            createSettingData();
        }

        if (dicSetData.ContainsKey(dataName))
        {
            return dicSetData[dataName];
        }
        else
        {
            return null;
        }
    }






}
[System.Serializable]
public class MyJsonData
{
    public List<List<float>> AllInfo;
}

