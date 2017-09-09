using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CTMenuVC : Singleton<CTMenuVC>
{

	public List<UnityEngine.UI.Extensions.UIPolygon> IconList=new List<UnityEngine.UI.Extensions.UIPolygon>();
	float initIConRotateAngle = 0;
	void Start() 
	{
	
		initIConRotateAngle = 180.0f-360.0f / (int)MainController.Instance.sides/2;

		CreateTingIcon(this.transform.position, (int)MainController.Instance.sides, initIConRotateAngle);

	}

	public void CreateTingIcon(Vector2 pos, int sides,float rotateAngle,int id=-1) 
	{
		//**************************************Icon*****************************************
		GameObject icon=new GameObject();
		icon.transform.parent=this.transform;
		icon.transform.position = pos;
		icon.AddComponent<UnityEngine.UI.Extensions.UIPolygon>();
		UnityEngine.UI.Extensions.UIPolygon uIPolygon=icon.GetComponent<UnityEngine.UI.Extensions.UIPolygon>();
		uIPolygon.sides = sides;
		uIPolygon.id = IconList.Count;
		float rotateAngleRevise=0;
		//預設旋轉角度
		if (id==-1)
		{
			rotateAngleRevise=0;
		}
		else 
		{
			rotateAngleRevise = (360.0f / sides / 2) * (float)((sides+id-((id%2==0)?0:1)));
		}
		uIPolygon.rotation = (rotateAngle + rotateAngleRevise) % 360;
		//關閉部分按鈕
		if (id != -1)
		{
			uIPolygon.SetBtnDisableIndex((id%2==0)?0:1);
		}
		IconList.Add(uIPolygon);


		//**************************************Model*****************************************
		float scale=0.4f;

		Vector3 posInWorld = (new Vector3(pos.x - IconList[0].transform.position.x, 0, pos.y - IconList[0].transform.position.y)) * scale + MainController.Instance.buildingCenter;
		MainController.Instance.AddBuilding(posInWorld, (-uIPolygon.rotation + initIConRotateAngle) % 360);
		MainController.Instance.SelectBuilding(	MainController.Instance.AllBuildings.Count-1);
	}
}
