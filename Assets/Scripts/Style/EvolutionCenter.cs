using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class EvolutionCenter : Singleton<EvolutionCenter>
{

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    /**
     * 變異
     * 輸入 : 整棟建築資料、變異機率、變異強度
     * 各部位變數逐一判斷是否變異
     */
    public List<Dictionary<string, List<DataInfo>>> MutateData(List<Dictionary<string, List<DataInfo>>> DataList, float probability, float intensity)
    {
        List<Dictionary<string, List<DataInfo>>> newDataList = new List<Dictionary<string, List<DataInfo>>>();
        //DataList.ForEach(i => newDataList.Add(i));
        copyDataList(newDataList, DataList);

        for (int iIndex = 0; iIndex < newDataList.Count; iIndex++)
        {
            Dictionary<string, List<DataInfo>> dicData = newDataList[iIndex];
            List<DataInfo> platDataList = dicData[Define.PlatformDataList];
            List<DataInfo> bodyDataList = dicData[Define.BodyDataList];
            List<DataInfo> roofDataList = dicData[Define.RoofDataList];
            for (int i = 0; i < platDataList.Count; i++)
            {
                float fThreshold = Random.Range(0.0f, 1.0f);
                if (fThreshold < probability)
                {
                    platDataList[i].Value = Mutate(platDataList[i].Value, intensity);
                }
            }
            for (int i = 0; i < bodyDataList.Count; i++)
            {
                float fThreshold = Random.Range(0.0f, 1.0f);
                if (fThreshold < probability)
                {
                    bodyDataList[i].Value = Mutate(bodyDataList[i].Value, intensity);
                }
            }
            for (int i = 0; i < roofDataList.Count; i++)
            {
                float fThreshold = Random.Range(0.0f, 1.0f);
                if (fThreshold < probability)
                {
                    roofDataList[i].Value = Mutate(roofDataList[i].Value, intensity);
                }
            }
        }
        return newDataList;
    }
    /**
     * 隨機變異資料
     * input 目前都沒用到可隨意給
     * !! 注意各參數放入的順序 !!!
     */
    public List<Dictionary<string, List<DataInfo>>> RamdomMutateData(float probability, float intensity)
    {
        List<Dictionary<string, List<DataInfo>>> newDataList = new List<Dictionary<string, List<DataInfo>>>();
        Dictionary<string, List<DataInfo>> dicData = new Dictionary<string, List<DataInfo>>();
        List<DataInfo> platDataList = new List<DataInfo>();
        List<DataInfo> bodyDataList = new List<DataInfo>();
        List<DataInfo> roofDataList = new List<DataInfo>();

        //** 屋身 **//
        if (Define.Instance.getSetData("eaveColRadInflate") != null)
        {
            DataInfo dicSettingData = Define.Instance.getSetData("eaveColRadInflate");
            bodyDataList.Add(new DataInfo("eaveColRadInflate", Mutate(dicSettingData.Value, intensity)));
        }
        else
        {
            print("In RamdomMutateData can't GET ** eaveColRadInflate ** value");
        }
        if (Define.Instance.getSetData("eaveColOffset") != null)
        {
            DataInfo dicSettingData = Define.Instance.getSetData("eaveColOffset");
            bodyDataList.Add(new DataInfo("eaveColOffset", Mutate(dicSettingData.Value, intensity)));
        }
        else
        {
            print("In RamdomMutateData can't GET ** eaveColOffset ** value");
        }

        //** 屋頂 **//
        if (Define.Instance.getSetData("allJijaHeight") != null)
        {
            DataInfo dicSettingData = Define.Instance.getSetData("allJijaHeight");
            roofDataList.Add(new DataInfo("allJijaHeight", Mutate(dicSettingData.Value, intensity)));
        }
        else
        {
            print("In RamdomMutateData can't GET ** allJijaHeight ** value");
        }
        if (Define.Instance.getSetData("mainRidgeHeightOffset") != null)
        {
            DataInfo dicSettingData = Define.Instance.getSetData("mainRidgeHeightOffset");
            roofDataList.Add(new DataInfo("mainRidgeHeightOffset", Mutate(dicSettingData.Value, intensity)));
        }
        else
        {
            print("In RamdomMutateData can't GET ** mainRidgeHeightOffset ** value");
        }
        if (Define.Instance.getSetData("flyEaveHeightOffset") != null)
        {
            DataInfo dicSettingData = Define.Instance.getSetData("flyEaveHeightOffset");
            roofDataList.Add(new DataInfo("flyEaveHeightOffset", Mutate(dicSettingData.Value, intensity)));
        }
        else
        {
            print("In RamdomMutateData can't GET ** flyEaveHeightOffset ** value");
        }
        if (Define.Instance.getSetData("eaveCurveHeightOffset") != null)
        {
            DataInfo dicSettingData = Define.Instance.getSetData("eaveCurveHeightOffset");
            roofDataList.Add(new DataInfo("eaveCurveHeightOffset", Mutate(dicSettingData.Value, intensity)));
        }
        else
        {
            print("In RamdomMutateData can't GET ** eaveCurveHeightOffset ** value");
        }

        dicData.Add(Define.PlatformDataList, platDataList);
        dicData.Add(Define.BodyDataList, bodyDataList);
        dicData.Add(Define.RoofDataList, roofDataList);
        newDataList.Add(dicData);

        return newDataList;
    }

    /**
    * 變異運算
    * 若為 BOOL 則相反
    * 其餘則亂數 (目前)
    */
    private Vector3 Mutate(Vector3 param, float intensity)
    {
        if (param.x == 0 && param.z == 1)
        {
            param.y = param.y == 0 ? 1 : 0;
        }
        else
        {
            param.y = Random.Range(param.x, param.z);
        }
        return param;
    }


    void copyDataList(List<Dictionary<string, List<DataInfo>>> newDataList, List<Dictionary<string, List<DataInfo>>> DataList)
    {
        for (int iIndex = 0; iIndex < DataList.Count; iIndex++)
        {
            Dictionary<string, List<DataInfo>> dic = DataList[iIndex];
            Dictionary<string, List<DataInfo>> newDic = new Dictionary<string, List<DataInfo>>();
            foreach (var Item in dic)
            {
                List<DataInfo> list = Item.Value;
                List<DataInfo> newList = new List<DataInfo>();
                for (int i = 0; i < list.Count; i++)
                {
                    DataInfo newData = list[i].Clone();
                    newList.Add(newData);
                }
                newDic.Add(Item.Key, newList);
            }
            newDataList.Add(newDic);
        }

    }

    /**
     * 
     */
    public void createAverageDistributedData(int iTotalPart)
    {
        string[] aryDataListName = Define.Instance.aryDataListName;         //** 建築物部位名稱(屋頂、屋身、基座)
        List<List<float>> listAverageData = new List<List<float>>();
        List<List<float>> listAllAvgData = new List<List<float>>();
        int iTotalNum = 1;                                                  //** 總數
        for (int iIndex = 0; iIndex < aryDataListName.Length; iIndex++)
        {
            string[] aryDataName = Define.Instance.queryDataNameArrayByName(aryDataListName[iIndex]);
            //List<float> listData = new List<float>();
            for (int i = 0; i < aryDataName.Length; i++ )
            {
                listAverageData.Add(divideSegmentSettingData(aryDataName[i], iTotalPart));        //** 切幾等等分
            }
        }

        foreach (List<float> list in listAverageData)
        {
            iTotalNum = iTotalNum * list.Count;
        }

        for (int iIndex = 0; iIndex < iTotalNum; iIndex++)
        {
            List<float> listData = new List<float>();
            int iRepeat = 1;
            foreach (List<float> list in listAverageData)
            {
                int _index = (iIndex / iRepeat) % list.Count;
                listData.Add(list[_index]);
                iRepeat = iRepeat * list.Count;
            }
            listAllAvgData.Add(listData);
        }
        print("0..0");

        Dictionary<string, List<List<float>>> dicInfo = new Dictionary<string, List<List<float>>> { { "Test", listAllAvgData } };
        var strJson = MiniJSON.Json.Serialize(dicInfo);
        StreamWriter file = new StreamWriter(Application.dataPath + "/Resources/TestData_Avg.json");
        file.Write(strJson);
        file.Close();
    }

    /**
     * 分割設定資料
     * 輸入 欲取得資料名稱、共要切幾等分
     */
    List<float> divideSegmentSettingData(string strDataName, int iTotalPart)
    {
        List<float> listValue = new List<float>();
        DataInfo info = Define.Instance.getSetData(strDataName);
        float fSegValue = (float)(info.Value.z - info.Value.x) / iTotalPart;
       
        for (int iIndex = 0; iIndex <= iTotalPart; iIndex++)
        {
            if (iIndex == 0)
            {
                listValue.Add(info.Value.x);
            }
            else
            {
                listValue.Add(fSegValue * iIndex + info.Value.x);
            }
        }
        return listValue;
    }


    /**
     * 製作並儲存隨機產生的資料集
     * 目前為1000筆
     */
    public void saveRamdomData()
    {
       
        List<List<float>> listAllInfo = new List<List<float>>();
        for (int iIndex = 0; iIndex < 1000; iIndex++)
        {
            List<Dictionary<string, List<DataInfo>>> listRandom = RamdomMutateData(1.0f, 1.0f);
            List<float> listInfo = new List<float>();

            for (int iDicIndex = 0; iDicIndex < Define.Instance.aryDataListName.Length; iDicIndex++)
            {
                List<DataInfo> roofData = listRandom[0][Define.Instance.aryDataListName[iDicIndex]];
                for (int iListIndex = 0; iListIndex < roofData.Count; iListIndex++)
                {
                    Vector3 val = roofData[iListIndex].Value;
                    listInfo.Add(val.y);
                }
            }
            listAllInfo.Add(listInfo);
        }

        //MyJsonData testData = new MyJsonData() { AllInfo = listAllInfo };
        //string strJson = JsonUtility.ToJson(testData);

        Dictionary<string, List<List<float>>> dicInfo = new Dictionary<string, List<List<float>>> { { "Test",listAllInfo} };
        // dicInfo.Add("Data", listAllInfo);

        var strJson = MiniJSON.Json.Serialize(dicInfo);
        //Debug.Log("serialized: " + strJson);

        StreamWriter file = new StreamWriter(Application.dataPath + "/Resources/TestData.json");
        file.Write(strJson);
        file.Close();
    }

}

