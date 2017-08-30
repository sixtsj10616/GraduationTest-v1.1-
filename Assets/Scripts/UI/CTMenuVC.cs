using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CTMenuVC : Singleton<CTMenuVC>
{

	public List<UnityEngine.UI.Extensions.UIPolygon> IconList=new List<UnityEngine.UI.Extensions.UIPolygon>();
	float initIConRotateAngle = 0;
	float initBuildingRotateAngle = 0;
	void Start() 
	{
		//預設旋轉角度
		if (((int)MainController.Instance.sides % 2) == 0)
		{
			initIConRotateAngle = 360.0f / (int)MainController.Instance.sides / 2;
		}
		else
		{
			initIConRotateAngle = 360.0f / (int)MainController.Instance.sides;
		}
		CreateTingIcon(this.transform.position, (int)MainController.Instance.sides,initIConRotateAngle);
		MainController.Instance.Buildings = MainController.Instance.AllBuildings[MainController.Instance.selectBuildingsIndex];
	}

	public void CreateTingIcon(Vector2 pos, int sides,float rotateAngle) 
	{
		GameObject icon=new GameObject();
		icon.transform.parent=this.transform;
		icon.transform.position = pos;
		icon.AddComponent<UnityEngine.UI.Extensions.UIPolygon>();
		icon.GetComponent<UnityEngine.UI.Extensions.UIPolygon>().sides = sides;
		icon.GetComponent<UnityEngine.UI.Extensions.UIPolygon>().id = IconList.Count;
		float rotateAngleRevise=0;
		//預設旋轉角度
		if (((int)MainController.Instance.sides % 2) == 0)
		{
				if ((int)MainController.Instance.sides == 4)
					rotateAngleRevise = 0;
				else
					rotateAngleRevise = 360.0f / (int)MainController.Instance.sides;
		}
		else
		{
			rotateAngleRevise = 360.0f / (int)MainController.Instance.sides / 2;
		}
		if (MainController.Instance.AllBuildings.Count == 0) rotateAngleRevise = 0;
		icon.GetComponent<UnityEngine.UI.Extensions.UIPolygon>().rotation = rotateAngle + rotateAngleRevise;
		IconList.Add(icon.GetComponent<UnityEngine.UI.Extensions.UIPolygon>());
		float scale=0.6f;
		//預設旋轉角度
		if (((int)MainController.Instance.sides % 2) == 0)
		{
			rotateAngleRevise = 360.0f / (int)MainController.Instance.sides/2;
		}
		if (MainController.Instance.AllBuildings.Count == 0) rotateAngleRevise = -rotateAngle;
		Vector3 posInWorld = (new Vector3(pos.x - IconList[0].transform.position.x, 0, pos.y - IconList[0].transform.position.y)) * scale + MainController.Instance.buildingCenter;
		MainController.Instance.AddBuilding(posInWorld, rotateAngle + rotateAngleRevise);
	}
}
