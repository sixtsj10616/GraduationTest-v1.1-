using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineTing : MonoBehaviour
{

    public float Center;
    public List<Vector3> ColumnList;

    /** 
     * 條整柱子位置並存放至ColumnList中
     * 流程:先將兩亭的柱子列表內縮至適當位置，算出需移動柱子的方向向量
     *      ，由此向量與另一亭的同邊柱子位置做一個平面並計算需位移的距離
     *      兩亭皆完成此步驟後將所有點加入新的列表中，並調整 List 順序
     */
    public void adjustColPos(List<Vector3> LTingColPos, List<Vector3> RTingColPos, Vector3 LTingCenter, Vector3 RTingCenter)
    {
        List<Vector3> newLTingColPos = ScaleColumnPos(LTingColPos, LTingCenter, 1.7f);
        List<Vector3> newRTingColPos = ScaleColumnPos(RTingColPos, RTingCenter, 1.7f);
        Vector3 LTingSiftDir = (newLTingColPos[0] - newLTingColPos[1]).normalized;
        Vector3 RTingSiftDir = (newRTingColPos[2] - newRTingColPos[3]).normalized;
        Plane LTingMovePlan = new Plane(LTingSiftDir, newRTingColPos[0]);
        Plane RTingMovePlan = new Plane(RTingSiftDir, newLTingColPos[2]);
        float LTingSiftDis = LTingMovePlan.GetDistanceToPoint(newLTingColPos[1]);
        float RTingSiftDis = RTingMovePlan.GetDistanceToPoint(newRTingColPos[3]);

        ColumnList = new List<Vector3>();

        for (int iIndex = 0; iIndex < newLTingColPos.Count; iIndex++)
        {
            ColumnList.Add(newLTingColPos[iIndex]);
            if (iIndex == 1)
            {
                ColumnList[ColumnList.Count - 1] = ColumnList[ColumnList.Count - 1] - LTingSiftDis * LTingSiftDir;
            }
        }
        for (int iIndex = 0; iIndex < newRTingColPos.Count; iIndex++)
        {
            ColumnList.Add(newRTingColPos[iIndex]);
            if (iIndex == 3)
            {
                ColumnList[ColumnList.Count - 1] = ColumnList[ColumnList.Count - 1] - RTingSiftDis * RTingSiftDir;
            }
        }
        //** 暫時寫死
        switchColumnListPos(1, 3);
        switchColumnListPos(3, 7);
        switchColumnListPos(4, 6);

    }
    /** 
     * 交換 ColumnList 中兩指定位置內容 (需先用過adjustColPos)
     */
    private void switchColumnListPos(int firstPos, int secondPos)
    {
        Vector3 tmpV3 = ColumnList[firstPos];
        ColumnList[firstPos] = ColumnList[secondPos];
        ColumnList[secondPos] = tmpV3;
    }
    /**
     * 縮放柱子位置
     * 輸入 : 柱子列表，柱子群中心點，向中心縮減的長度
     */
    private List<Vector3> ScaleColumnPos(List<Vector3> ColList, Vector3 Center, float scaleDis)
    {
        List<Vector3> NewColList = new List<Vector3>();
        for (int iIndex = 0; iIndex < ColList.Count; iIndex++)
        {
            Vector2 v2CenterToCol = new Vector2(ColList[iIndex].x - Center.x, ColList[iIndex].z - Center.z);
            Vector3 eaveColumnPos = ColList[iIndex] - new Vector3(v2CenterToCol.x, 0, v2CenterToCol.y).normalized * scaleDis;
            NewColList.Add(eaveColumnPos);
        }
        return NewColList;
    }
    /**
     * 建立柱子群 (從BodyController搬來的，發現那個還是不好用)
     */
    public List<ColumnStruct> CreateRingColumn(GameObject parentObj, float columnTopRadius, float columnDownRadius, float columnHeight, string columnName)
    {
        List<ColumnStruct> columnList = new List<ColumnStruct>();
        for (int i = 0; i < ColumnList.Count; i++)
        {
            ColumnStruct newColumn = CreateColumn(parentObj, ColumnList[i], columnTopRadius, columnDownRadius, columnHeight, columnDownRadius * 1.2f, columnHeight * 0.05f, columnName);
        }
        return columnList;
    }
    /**
     * 建立柱子 (從BodyController搬來的，發現那個還是不好用)
     */
    private ColumnStruct CreateColumn(GameObject parentObj, Vector3 pos, float topRadius, float downRadius, float height, float fundationRadius, float fundationHeight, string name = "Column")
    {
        ColumnStruct columnStruct = new ColumnStruct();

        float columnFundationHeight = height * 0.05f;
        float columnRemainHeight = height - columnFundationHeight;

        GameObject col = new GameObject(name);
        //col.transform.position = pos + new Vector3(0, columnRemainHeight / 2.0f + columnFundationHeight, 0);
        col.transform.parent = parentObj.transform;
        col.AddComponent<CylinderMesh>();

        Vector3 topPos = pos + (columnRemainHeight + columnFundationHeight) * Vector3.up;
        Vector3 bottomPos = pos + columnFundationHeight * Vector3.up;

        col.GetComponent<CylinderMesh>().CylinderInitSetting(pos, topPos, bottomPos, topRadius, downRadius);
        col.GetComponent<CylinderMesh>().SetMesh();
        columnStruct.columnObj = col;
        columnStruct.columnMesh = col.GetComponent<CylinderMesh>();
        columnStruct.topPos = topPos;
        columnStruct.bottomPos = bottomPos;
        //Fundation
        GameObject fun = new GameObject(name + "Fundation");
        fun.transform.parent = col.transform;
        fun.AddComponent<CylinderMesh>();

        topPos = pos + columnFundationHeight * Vector3.up;
        bottomPos = pos;

        fun.GetComponent<CylinderMesh>().CylinderInitSetting(pos - new Vector3(0, height / 2.0f, 0), topPos, bottomPos, fundationRadius, fundationRadius);
        fun.GetComponent<CylinderMesh>().SetMesh();

        columnStruct.fundation = col.GetComponent<CylinderMesh>();

        return columnStruct;
    }
    /**
     * 建立柵欄 (從BodyController搬來的，發現那個還是不好用)
     */
	public void CreateRingBalustrade(ModelController modelController, float columnRadius, float heightOffset, GameObject parent)
    {
		float balustradeWidth = modelController.eaveColumnModelStruct.balustradeModelStruct.bound.size.x;//欄杆長度
		float balustradeHeight = modelController.eaveColumnModelStruct.balustradeModelStruct.bound.size.y;//欄杆長度
		float balustradeLengh = modelController.eaveColumnModelStruct.balustradeModelStruct.bound.size.z;//欄杆深度

		for (int i = 0; i < ColumnList.Count; i++)
		{
			float width = balustradeWidth;
				float dis = Vector3.Distance(ColumnList[i], ColumnList[(i + 1) % ColumnList.Count]) - columnRadius * 2;
				int number = Mathf.Max(Mathf.FloorToInt(dis / width), 1);
				Vector3 dir = ColumnList[(i + 1) % ColumnList.Count] - ColumnList[i];
				float disDiff = (dis - width * number) / number;
				width = dis / number;

				float rotateAngle = (Vector3.Dot(Vector3.forward, dir) < 0 ? Vector3.Angle(dir, Vector3.right) : -Vector3.Angle(dir, Vector3.right));
				for (int j = 0; j < number; j++)
				{
					Vector3 pos = dir.normalized * (width / 2.0f + j * width + columnRadius) + ColumnList[i] + heightOffset * Vector3.up;
					GameObject clone = Instantiate( modelController.eaveColumnModelStruct.balustradeModelStruct.model, pos,  modelController.eaveColumnModelStruct.balustradeModelStruct.model.transform.rotation) as GameObject;
					clone.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up) * Quaternion.Euler( modelController.eaveColumnModelStruct.balustradeModelStruct.rotation);
					clone.transform.GetChild(0).localScale = new Vector3(clone.transform.GetChild(0).localScale.x, clone.transform.GetChild(0).localScale.y, (clone.transform.GetChild(0).localScale.z) * (width) / balustradeWidth);
					//clone.transform.GetChild(0).localScale = Vector3.Scale(clone.transform.GetChild(0).localScale, Quaternion.Euler(clone.transform.GetChild(0).transform.rotation.ToEulerAngles()) * (new Vector3((width) / balustradeWidth, 1, 1)));
					clone.transform.parent = parent.transform;
				}
		}
    }
    /**
     * 檢查主脊
     */
    public void checkMainRidge(BuildingObj Ting, int RidgeIndex, Vector3 LTingCenter, Vector3 RTingCenter)
    {
        RidgeStruct mainRidge = Ting.roofController.MainRidgeList[RidgeIndex];          //* 要被檢測的主脊
        Vector3 planeNorVector = (LTingCenter - RTingCenter).normalized;                //* 兩亭中間切平面法向量
        Plane MidPlane = new Plane(planeNorVector, (LTingCenter + RTingCenter) / 2);    //* 兩亭中間切平面
        int tileListCount = mainRidge.tilePosList.Count;                                //* 暫存主脊瓦片列表總數

        for (int iIndex = 0; iIndex < tileListCount; iIndex++)
        {
            //* 若不同邊
            if (!MidPlane.SameSide(mainRidge.tilePosList[0], Ting.roofTopCenter))
            {
                mainRidge.tilePosList.RemoveAt(0);
            }
            else
                break;
        }
        if (tileListCount != mainRidge.tilePosList.Count)
        {
            foreach (Transform child in mainRidge.body.transform)
            {
                if (child.name.Equals("main_ridge_new(Clone)"))
                {
                    Destroy(child.gameObject);
                }
            }
            Ting.roofController.CreateTileByModelAndRidge(ModelController.Instance.mainRidgeModelStruct, mainRidge);
        }
    }
    /**
     * 檢查屋面瓦片
     */
    public void CheckSurface(BuildingObj Ting, int SurfaceIndex, Vector3 LTingCenter, Vector3 RTingCenter)
    {
        RoofSurfaceStruct surface = Ting.roofController.SurfaceList[SurfaceIndex];
        Vector3 planeNorVector = (LTingCenter - RTingCenter).normalized;                //* 兩亭中間切平面法向量
        Plane MidPlane = new Plane(planeNorVector, (LTingCenter + RTingCenter) / 2);    //* 兩亭中間切平面

        //** 依序檢查每一個表面脊 (左、中、右分開做)
        for (int iIndex = 0; iIndex < surface.leftRoofSurfaceTileRidgeList.Count; iIndex++)
        {
			RoofSurfaceRidgeStruct surfaceRidge = surface.leftRoofSurfaceTileRidgeList[iIndex];
            removeSurfacePosByPlane(MidPlane,surfaceRidge,Ting.roofTopCenter);
        }
        removeSurfacePosByPlane(MidPlane, surface.midRoofSurfaceTileRidge, Ting.roofTopCenter);
        for (int iIndex = 0; iIndex < surface.rightRoofSurfaceTileRidgeList.Count; iIndex++)
        {
			RoofSurfaceRidgeStruct surfaceRidge = surface.rightRoofSurfaceTileRidgeList[iIndex];
            removeSurfacePosByPlane(MidPlane, surfaceRidge, Ting.roofTopCenter);
        }
        print("");
        //** 
    }
    /**
     * 刪除超過平面的瓦片位置
     * 輸入: 切割平面、屋面瓦片用脊、同側的基準點
     */
    private void removeSurfacePosByPlane(Plane MidPlane,RoofSurfaceRidgeStruct surfaceRidge,Vector3 BasePos)
    {
        if (surfaceRidge.tilePosList.Count > 0 && !MidPlane.SameSide(surfaceRidge.tilePosList[0], BasePos))
        {
            int tileListCount = surfaceRidge.tilePosList.Count;
            for (int iPosIndex = 0; iPosIndex < tileListCount ; iPosIndex++)
            {
                //** 若檢查瓦片用脊未到倒數第二個前，檢查方式除了當前瓦片外，還會看下一個瓦片是否也需要刪除，用來讓兩亭交接處不出現鋸齒狀空洞
                if (iPosIndex < tileListCount - 1)
                {
                    if (!MidPlane.SameSide(surfaceRidge.tilePosList[0], BasePos) && !MidPlane.SameSide(surfaceRidge.tilePosList[1], BasePos))
                    {
                        surfaceRidge.tilePosList.RemoveAt(0);
                        surfaceRidge.roundTileModelList[0].transform.parent = null;
                        Destroy(surfaceRidge.roundTileModelList[0].gameObject);
                        surfaceRidge.roundTileModelList.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (!MidPlane.SameSide(surfaceRidge.tilePosList[0], BasePos))
                //if (!MidPlane.SameSide(surfaceRidge.tilePosList[0], BasePos))
                {
                    surfaceRidge.tilePosList.RemoveAt(0);
                    surfaceRidge.roundTileModelList[0].transform.parent = null;
                    Destroy(surfaceRidge.roundTileModelList[0].gameObject);
                    surfaceRidge.roundTileModelList.RemoveAt(0);
                }
                else
                    break;
            }
        }
    }


}
