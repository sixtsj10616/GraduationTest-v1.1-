using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public const float initPlatLength   = 50;
    public const float initPlatHeight   = 5;
    public const float initStairWidth   = 5;
    public const float initStairLength  = 5;

    //***           屋身          ***//
    public const float initEaveColumnHeight   = 11;
    public const int initColRadSegment = 7;


    //***           屋頂          ***//
    public const float initJijaHeight = 13;
	public const  float initFlyEaveHeightOffset = 1.0f;        //飛簷上翹程度
	public const float initMainRidgeHeightOffset=0;             //主脊曲線上翹程度
	public const float initRoofSurfaceHeightOffset = -1.0f;   //屋面曲線上翹程度
	public const float initEaveCurveHeightOffset = -2f;       //屋簷高度

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
/*

    void tmpUse()
	{
		// *** 格式 : 參數名稱(須按照實際class參數命名)，內容為Dic 有三個Key :  max、init、min
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
        dicSettingData.Add("eaveColOffset", new Dictionary<string, object> {{ "max", 4 } ,  // *** 此參數目前沒有，若小於0就代表柱子底部內縮
                                                                        { "init", 0 },      // *** 若大於0就代表柱子頂部內縮
                                                                        { "min", -4 }       // *** 感覺還是要有用比例
                                                                        });
    }
*/

    public void createSettingData()
    {
        dicSetData = new Dictionary<string, DataInfo>();

		#region ROOF_PARAMETER

		dicSetData.Add("flyEaveHeightOffset", new DataInfo("flyEaveHeightOffset",
										new Vector3(-3, initFlyEaveHeightOffset, 3),    RoofMenuHandler.WingAngleSlider_NAME));
        dicSetData.Add("mainRidgeHeightOffset", new DataInfo("mainRidgeHeightOffset",
										new Vector3(-3, initMainRidgeHeightOffset, 3), 
							   RoofMenuHandler.RidgeSlider_NAME));
        dicSetData.Add("eaveCurveHeightOffset", new DataInfo("eaveCurveHeightOffset",
										new Vector3(-5, initEaveCurveHeightOffset, 1), 
							   RoofMenuHandler.EaveSlider_NAME));
        dicSetData.Add("allJijaHeight", new DataInfo("allJijaHeight",
										new Vector3(8, initJijaHeight, 18),
							   RoofMenuHandler.JijaHeightSlider_NAME));
        dicSetData.Add("roofSurfaceHeightOffset", new DataInfo("roofSurfaceHeightOffset",
										new Vector3(-2, initRoofSurfaceHeightOffset, 2), RoofMenuHandler.SurfaceSlider_NAME));
		#endregion

		#region BODY_PARAMETER

		dicSetData.Add("eaveColRadInflate", new DataInfo("eaveColRadInflate",
											new Vector3(0.5f, 1, 3), BodyMenuHandler.EaveColRadInflate_NAME));
		//*** 此參數目前沒有，若小於0就代表柱子底部內縮
		//*** 若大於0就代表柱子頂部內縮，感覺還是要有用比例，
        dicSetData.Add("eaveColOffset", new DataInfo("eaveColOffset",         
										new Vector3(-4, 0, 4), BodyMenuHandler.EaveColOffset_NAME));
	
        dicSetData.Add("eaveColumnHeight", new DataInfo("eaveColumnHeight",
										   new Vector3(5f, 11, 22), BodyMenuHandler.ColumeHeightSlider_NAME));

		dicSetData.Add("isBalustrade", new DataInfo("isBalustrade",
									   new Vector3(0, 1, 1), BodyMenuHandler.BalustradeToggle_NAME));
		dicSetData.Add("isFrieze",     new DataInfo("isFrieze",
									   new Vector3(0, 1, 1), BodyMenuHandler.FriezeToggle_NAME));
		dicSetData.Add("isGoldColumn", new DataInfo("isGoldColumn",
		                               new Vector3(0, 1, 1), BodyMenuHandler.GoldColToggle_NAME));

		dicSetData.Add("goldColumnbayNumber", new DataInfo("goldColumnbayNumber",
									          new Vector3(0, 5, 10), BodyMenuHandler.GoldColNumSlider_NAME));

		dicSetData.Add("unitNumberInBay", new DataInfo("unitNumberInBay",
										  new Vector3(0, 2, 5), BodyMenuHandler.UnitInBay_NAME));

		dicSetData.Add("doorNumber", new DataInfo("doorNumber",
	 								 new Vector3(0, 2, 3), BodyMenuHandler.DoorNumSlider_NAME));
		#endregion

		#region PLATFORM_PARAMETER

		dicSetData.Add("platWidth", new DataInfo("platWidth",
										new Vector3(20, initPlatWidth, 300), PlamformMenuHandler.WidthSlider_NAME));
		dicSetData.Add("platLength", new DataInfo("platLength",
											new Vector3(20, initPlatLength, 300)));
		dicSetData.Add("platHeight", new DataInfo("platHeight",
										new Vector3(1, initPlatHeight, 8), PlamformMenuHandler.HeightSlider_NAME));
		dicSetData.Add("isStair", new DataInfo("isStair",
									new Vector3(0, 0, 1), PlamformMenuHandler.StairToggle_NAME));
		dicSetData.Add("isBorder", new DataInfo("isBorder",
									new Vector3(0, 0, 1), PlamformMenuHandler.BorderToggle_NAME));

		#endregion
     
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

