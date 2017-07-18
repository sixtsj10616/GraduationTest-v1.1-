using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRidgeStruct //脊
{
    public GameObject body;
    public Dictionary<string, Vector3> dicCtrlPoint;
   // public CatLine ridgeCatLine;
    public List<Vector3> tilePosList;               //* 瓦片放置位置
    public List<Vector3> innerPosList;              //* 原線段
    public NewRidgeStruct()
    {
        dicCtrlPoint = new Dictionary<string, Vector3>();
        tilePosList = new List<Vector3>();
        innerPosList = new List<Vector3>();
        //ridgeCatLine = new CatLine();
    }
}

public class RoofBuilder : Singleton<RoofBuilder>
{

    public void CreateRidge(Vector3 topCtrlPoint , Vector3 secCtrlPoint, Vector3 thirdCtrlPoint, Vector3 lastCtrlPoint)
    {
        NewRidgeStruct ridge = new NewRidgeStruct();
        CatLine catLine = new CatLine();

        ridge.dicCtrlPoint.Add(Define.TopPoint , topCtrlPoint);
        ridge.dicCtrlPoint.Add(Define.SecPoint, secCtrlPoint);
        ridge.dicCtrlPoint.Add(Define.ThirdPoint, thirdCtrlPoint);
        ridge.dicCtrlPoint.Add(Define.LastPoint, lastCtrlPoint);

    }

    public NewRidgeStruct CreateRidge(List<Vector3> ctrlList)
    {
        NewRidgeStruct ridge = new NewRidgeStruct();
        CatLine catLine = new CatLine();

        ridge.dicCtrlPoint.Add(Define.TopPoint, ctrlList[0]);
        ridge.dicCtrlPoint.Add(Define.SecPoint, ctrlList[1]);
        ridge.dicCtrlPoint.Add(Define.ThirdPoint, ctrlList[2]);
        ridge.dicCtrlPoint.Add(Define.LastPoint, ctrlList[3]);

        catLine.CalculateInnerPointByList(ctrlList, 0);
        ridge.tilePosList = catLine.CalculateAnchorPosByInnerPointList(catLine.innerPointList, catLine.innerPointList.Count - 1, 0, Define.mainRidgeTileHeight);

        return ridge;
    }
    public NewRidgeStruct CreateRidgeNoTile(List<Vector3> ctrlList)
    {
        NewRidgeStruct ridge = new NewRidgeStruct();
        CatLine catLine = new CatLine();

        ridge.dicCtrlPoint.Add(Define.TopPoint, ctrlList[0]);
        ridge.dicCtrlPoint.Add(Define.SecPoint, ctrlList[1]);
        ridge.dicCtrlPoint.Add(Define.ThirdPoint, ctrlList[2]);
        ridge.dicCtrlPoint.Add(Define.LastPoint, ctrlList[3]);

        catLine.CalculateInnerPointByList(ctrlList, 0);
        //ridge.tilePosList = catLine.CalculateAnchorPosByInnerPointList(catLine.innerPointList, catLine.innerPointList.Count - 1, 0, Define.mainRidgeTileHeight);
        ridge.innerPosList = catLine.innerPointList;
        return ridge;
    }

    public NewRidgeStruct CreateEaveRidge(List<Vector3> ctrlList)
    {
        NewRidgeStruct ridge = new NewRidgeStruct();
        CatLine catLine = new CatLine();

        ridge.dicCtrlPoint.Add(Define.TopPoint, ctrlList[0]);
        ridge.dicCtrlPoint.Add(Define.SecPoint, ctrlList[1]);
        ridge.dicCtrlPoint.Add(Define.ThirdPoint, ctrlList[2]);

        catLine.CalculateInnerPointByList(ctrlList, 0);
        ridge.tilePosList = catLine.CalculateAnchorPosByInnerPointList(catLine.innerPointList, catLine.innerPointList.Count - 1, 0, Define.mainRidgeTileHeight);



        return ridge;
    }
    //*** 應該要傳入一個法向量
    /**
     * 建立屋面水平基準線
     */
    public NewRidgeStruct CreateSufaceHorizontalRidge(Vector3 RCtrlPoint , Vector3 LCtrlPoint , float rate)
    {
        List<Vector3> ctrlPointList = new List<Vector3>();
        Vector3 midPos, eaveDir;
        float eaveDist;
        midPos = (RCtrlPoint + LCtrlPoint) / 2;
        eaveDir = (RCtrlPoint - LCtrlPoint).normalized;
        eaveDist = (RCtrlPoint - LCtrlPoint).magnitude;
        ctrlPointList.Add(RCtrlPoint);
        ctrlPointList.Add(midPos + 0.25f * eaveDir * eaveDist + rate * Vector3.up);
        ctrlPointList.Add(midPos - 0.25f * eaveDir * eaveDist + rate * Vector3.up);
        ctrlPointList.Add(LCtrlPoint);

        return CreateRidgeNoTile(ctrlPointList);
    }
    /**
     * 建立屋面水平基準線
     */
    public NewRidgeStruct CreateSufaceDownHorizontalRidge(Vector3 RCtrlPoint, Vector3 LCtrlPoint, float rate)
    {
        List<Vector3> ctrlPointList = new List<Vector3>();
        Vector3 midPos, eaveDir;
        float eaveDist;
        midPos = (RCtrlPoint + LCtrlPoint) / 2;
        eaveDir = (RCtrlPoint - LCtrlPoint).normalized;
        eaveDist = (RCtrlPoint - LCtrlPoint).magnitude;
        ctrlPointList.Add(RCtrlPoint);
        ctrlPointList.Add(midPos + 0.25f * eaveDir * eaveDist + rate * Vector3.up);
        ctrlPointList.Add(midPos - 0.25f * eaveDir * eaveDist + rate * Vector3.up);
        ctrlPointList.Add(LCtrlPoint);

        return CreateRidge(ctrlPointList);
    }
    public NewRidgeStruct CreateSufaceDownHorizontalRidge(List<Vector3> CtrlPointList)
    {
        List<Vector3> ctrlPointList = new List<Vector3>();
        Vector3 midPos, eaveDir;
        float eaveDist;
        
        //ctrlPointList.Add(CtrlPointList[0]);
        //ctrlPointList.Add(CtrlPointList[1]);
        //ctrlPointList.Add(CtrlPointList[2]);
        //ctrlPointList.Add(LCtrlPoint);

        return CreateRidge(CtrlPointList);
    }

    public void CreateRoof(MainController.RoofType roofType , List<NewRidgeStruct> mainRidgeList)
    {
        switch (roofType)
        {
            case MainController.RoofType.Zan_Jian_Ding:

                break;
        }
    }
}
