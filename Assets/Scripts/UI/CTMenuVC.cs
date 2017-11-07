using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CTMenuVC : Singleton<CTMenuVC>
{

	public List<UnityEngine.UI.Extensions.UIPolygon> IconList=new List<UnityEngine.UI.Extensions.UIPolygon>();
	//初始的icon旋轉角度
	float initIConRotateAngle;
	//初始的模型旋轉角度修正
	float initModelRotateAngle;
	void Start() 
	{
	
		initIConRotateAngle = 180.0f-180.0f / (int)MainController.Instance.sides;
		CreateTingIcon(this.transform.position, (int)MainController.Instance.sides, initIConRotateAngle);

	}
	// btnId為-1代表不是依據亭icon上的按鈕產生另一亭的icon 是由Start()產生
	public void CreateTingIcon(Vector2 pos, int sides,float rotateAngle,int btnId=-1) 
	{
		//**************************************Icon*****************************************
		GameObject icon=new GameObject();
		icon.transform.parent=this.transform;
		icon.transform.position = pos;
		icon.AddComponent<UnityEngine.UI.Extensions.UIPolygon>();
		UnityEngine.UI.Extensions.UIPolygon uIPolygon=icon.GetComponent<UnityEngine.UI.Extensions.UIPolygon>();
		uIPolygon.sides = sides;
		uIPolygon.id = IconList.Count;
		if(sides==(int)MainController.FormFactorSideType.FourSide)
		{
			initModelRotateAngle = 45;
		}
		else
		{
			initModelRotateAngle =180 ;
		}
		float rotateAngleRevise = 0;
		//預設旋轉角度
		if (btnId>0)
		{
			rotateAngleRevise = (360.0f / sides / 2) * (float)((sides+btnId-((btnId%2==0)?0:1)));
		}
		uIPolygon.rotation = (rotateAngle + rotateAngleRevise) % 360;
		//關閉部分按鈕
		if (btnId != -1)
		{
			uIPolygon.SetBtnDisableIndex((btnId%2==0)?0:1);
		}
		IconList.Add(uIPolygon);


		//**************************************Model*****************************************
		float scale=0.5f;

		Vector3 posInWorld = (new Vector3(pos.x - IconList[0].transform.position.x, 0, pos.y - IconList[0].transform.position.y)) * scale + MainController.Instance.buildingCenter;
		MainController.Instance.AddBuilding(posInWorld, (-uIPolygon.rotation + initModelRotateAngle+360) % 360);
		MainController.Instance.SelectBuilding(uIPolygon.id);
	}
}
