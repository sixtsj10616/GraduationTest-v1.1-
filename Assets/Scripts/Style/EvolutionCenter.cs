using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionCenter : Singleton<EvolutionCenter>
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * 變異
     * 輸入 : 整棟建築資料、變異機率、變異強度
     * 各部位變數逐一判斷是否變異
     */
    public List<Dictionary<string, List<DataInfo>>> MutateData(List<Dictionary<string, List<DataInfo>>> DataList,float probability,float intensity)
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


    void copyDataList(List<Dictionary<string, List<DataInfo>>> newDataList , List<Dictionary<string, List<DataInfo>>> DataList)
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
}
