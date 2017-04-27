using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CatLine : MonoBehaviour
{
	public enum CatLineType { ObjectList = 0, PositionList = 1 };
	private int numberOfPoints = 50;
	public List<GameObject> controlPointList = new List<GameObject>();
	public List<Vector3> controlPointPosList = new List<Vector3>();
	public List<Vector3> innerPointList = new List<Vector3>();
	public List<Vector3> anchorInnerPointlist = new List<Vector3>();
	Vector3 p0,p1,p2,p3;
	public void SetLineNumberOfPoints(int number)
	{
		numberOfPoints = number;
	}
	public void SetCatmullRom(float anchorDis = 0, int catLineType = (int)CatLineType.ObjectList)
	{
		DisplayCatmullromSpline(anchorDis, catLineType);
	}

	Vector3 ReturnCatmullRomPos(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 pos = 0.5f * ((2f * p1) + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t + (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
		return pos;
	}
	public List<Vector3> CalculateAnchorPosByInnerPointList(List<Vector3> list, int startIndex,int endIndex,float anchorDis)
	{
		if(list.Count==0)return list;

		float dis = 0;
		List<Vector3> newList = new List<Vector3>();
		newList.Add(list[startIndex]);
		int dir=((endIndex - startIndex) > 0 ? 1 : -1);
		for (int i = startIndex; ((endIndex - startIndex) > 0 ? (i < (endIndex)) : (i > (endIndex))); i += dir)
		{
			dis += Vector3.Distance(list[i], list[i +dir]);
			if (dis >= anchorDis)
			{
				newList.Add(list[i]);
				dis = 0;
			}
		}
/*
		newList.Add(list[list.Count - 1]);//反著加入
		for(int i=list.Count-1;i>1;i--)
		{
			dis += Vector3.Distance(list[i], list[i-1]);
			if (dis >= anchorDis)
			{
				newList.Add(list[i]);
				dis = 0;
			}	
		}*/
		return newList;
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


			float segmentation = 1 / (float)numberOfPoints;
			float t = 0;
			float dis = 0;
			for (int i = 0; i < numberOfPoints-1; i++)
			{
				t += segmentation;

				Vector3 newPos = ReturnCatmullRomPos(t, p0, p1, p2, p3);
				innerPointList.Add(newPos);
				
			}
			for (int i = 0; i < innerPointList.Count - 1; i++)
			{
				if (anchorDis == 0)
				{
					anchorInnerPointlist.Add(innerPointList[i]);
				}
				else
				{
					if (dis >= anchorDis)
					{
						anchorInnerPointlist.Add(innerPointList[i]);
						dis = 0;
					}
					dis += Vector3.Distance(innerPointList[i], innerPointList[i + 1]);
				}

			}
		}
		else
		{
			for (int index = 0; index < list.Count - 1; index++)
			{

				p0 = list[Mathf.Max(index - 1, 0)];
				p1 = list[index];
				p2 = list[Mathf.Min(index + 1, list.Count - 1)];
				p3 = list[Mathf.Min(index + 2, list.Count - 1)];
				float segmentation = 1 / (float)(numberOfPoints);
				float t = 0;
			
				for (int i = 0; i < numberOfPoints-1; i++)
				{
					t += segmentation;

					Vector3 newPos = ReturnCatmullRomPos(t, p0, p1, p2, p3);
					innerPointList.Add(newPos);		
				}
				
			}
			float dis = 0;
			for (int i = 0; i < innerPointList.Count - 1; i++)
			{
				if (anchorDis == 0)
				{
					anchorInnerPointlist.Add(innerPointList[i]);
				}
				else
				{
					if (dis >= anchorDis)
					{
						anchorInnerPointlist.Add(innerPointList[i]);
						dis = 0;
					}
					dis += Vector3.Distance(innerPointList[i], innerPointList[i + 1]);
				}

			}
		}
	
	}
	void DisplayCatmullromSpline(float anchorDis, int catLineType)
	{
		innerPointList.Clear();
		anchorInnerPointlist.Clear();
		switch (catLineType)
		{
			case (int)CatLineType.ObjectList:
				List<Vector3> positionList = new List<Vector3>();
				for(int i=0;i<controlPointList.Count;i++)
				{
					positionList.Add(controlPointList[i].transform.position);
				}
				CalculateInnerPointByList(positionList, anchorDis);
				break;
			case (int)CatLineType.PositionList:
				CalculateInnerPointByList(controlPointPosList, anchorDis);
				break;
		}

	}
}
