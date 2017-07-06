using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatLine
{
	private int numberOfPoints = 50;
	public List<Vector3> controlPointPosList = new List<Vector3>();
	public List<Vector3> innerPointList = new List<Vector3>();
	public List<Vector3> anchorInnerPointlist = new List<Vector3>();
	Vector3 p0,p1,p2,p3;
	public void SetLineNumberOfPoints(int number)
	{
		numberOfPoints = number;
	}
	public void SetCatmullRom(float anchorDis = 0)
	{
		DisplayCatmullromSpline(anchorDis);
	}

	Vector3 ReturnCatmullRomPos(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 pos = 0.5f * ((2f * p1) + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t + (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
		return pos;
	}
	Vector3 ReturnCatmullRomPos(float t)
	{
		Vector3 pos = 0.5f * ((2f * p1) + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t + (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
		return pos;
	}
    /**
     * 算錨點(通常用來算放置瓦片的位置)
     * list : 點列表，anchorDis : 瓦片長度
     */
    public List<Vector3> CalculateAnchorPosByInnerPointList(List<Vector3> list, int startIndex,int endIndex,float anchorDis)
	{
		if(list.Count==0)return list;

		float dis = 0;
		List<Vector3> newList = new List<Vector3>();
		newList.Add(list[startIndex]);
		int dir = ((endIndex - startIndex) > 0 ? 1 : -1);
		for (int i = startIndex + dir; ((endIndex - startIndex) > 0 ? (i < (endIndex)) : (i > (endIndex))); i += dir)
		{
			dis += Vector3.Distance(list[i], list[i +dir]);
			if (dis >= anchorDis)
			{
				newList.Add(list[i]);
				dis = 0;
			}
		}
		return newList;
	}
    public List<Vector3> CalculateAnchorPosByInnerPointListPoolingVer(List<Vector3> list, int startIndex, int endIndex, float anchorDis)
    {
        if (list.Count == 0) return list;

        float dis = 0;
        List<Vector3> newList = new List<Vector3>();
        newList.Add(list[startIndex]);
        int dir = ((endIndex - startIndex) > 0 ? 1 : -1);
		for (int i = startIndex + dir; ((endIndex - startIndex) > 0 ? (i < (endIndex)) : (i > (endIndex))); i += dir)
        {
            dis += Vector3.Distance(list[i], list[i + dir]);
            if (dis >= anchorDis)
            {
                newList.Add(list[i]);
                dis = 0;
            }

            if ((endIndex - startIndex) > 0 ? (i == (endIndex - 1)) : (i == (endIndex + 1)))
            {
                if (dis != 0)
                {
                    newList.Add(list[i - 12*dir]);
                    dis = 0;
                }
            }
        }
        return newList;
    }
    /**
     * 
     */
	void ShowPos(Vector3 pos, Color color, float localScale = 0.2f)
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		obj.transform.position = pos;
		obj.transform.localScale = Vector3.one * localScale;
		obj.GetComponent<MeshRenderer>().material.color = color;
	}
    public void CalculateInnerPointByList(List<Vector3> list, float anchorDis) 
	{
		if (list.Count < 2) return;
		else if (list.Count == 2)
		{
			p0 = list[0];
			p1 = list[0];
			p2 = list[1];
			p3 = list[1];

			for (int i = 0; i < numberOfPoints; i++)
			{

				Vector3 newPos = ReturnCatmullRomPos((1.0f / (numberOfPoints)) * i, p0, p1, p2, p3);
				innerPointList.Add(newPos);
			}
			anchorInnerPointlist = CalculateAnchorPosByInnerPointList(innerPointList, 0, innerPointList.Count - 1, anchorDis);
		}
		else
		{
		
			for (int index = 0; index < list.Count-1; index++)
			{	
				p0 = list[Mathf.Max(index - 1, 0)];
				p1 = list[index];
				p2 = list[Mathf.Min(index + 1, list.Count - 1)];
				p3 = list[Mathf.Min(index + 2, list.Count - 1)];

				for (int i = 0; i < numberOfPoints; i++)
				{

					Vector3 newPos = ReturnCatmullRomPos((1.0f / (numberOfPoints)) * i, p0, p1, p2, p3);
					innerPointList.Add(newPos);
				}

			}
			anchorInnerPointlist = CalculateAnchorPosByInnerPointList(innerPointList, 0, innerPointList.Count - 1, anchorDis);
		}
	
	}
	void DisplayCatmullromSpline(float anchorDis)
	{
		innerPointList.Clear();
		anchorInnerPointlist.Clear();

		CalculateInnerPointByList(controlPointPosList, anchorDis);
	}
}
