
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
	public class UIPolygon : MaskableGraphic
	{
		[SerializeField]
		Texture m_Texture;
		public bool fill = true;
		public float thickness = 5;
		[Range(3, 8)]
		public int sides = 4;
		[Range(0, 360)]
		public float rotation = 0;
		[Range(0, 1)]
		public float[] VerticesDistances = new float[3];
		public List<Vector2> VerticesPos = new List<Vector2>();
		public List<GameObject> btnList=new List<GameObject>();
		private float size = 0;
		private bool setBtnUpdate = false;
		public int id;
		public override Texture mainTexture
		{
			get
			{
				return m_Texture == null ? s_WhiteTexture : m_Texture;
			}
		}
		public Texture texture
		{
			get
			{
				return m_Texture;
			}
			set
			{
				if (m_Texture == value) return;
				m_Texture = value;
				SetVerticesDirty();
				SetMaterialDirty();
			}
		}
		public void DrawPolygon(int _sides)
		{
			sides = _sides;
			VerticesDistances = new float[_sides + 1];
			for (int i = 0; i < _sides; i++) VerticesDistances[i] = 1;
			for (int i = 0; i < _sides; i++) VerticesPos[i] = Vector2.zero;
			rotation = 0;
		}
		public void DrawPolygon(int _sides, float[] _VerticesDistances)
		{
			sides = _sides;
			VerticesDistances = _VerticesDistances;
			rotation = 0;
		}
		public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
		{
			sides = _sides;
			VerticesDistances = _VerticesDistances;
			rotation = _rotation;
		}
		void Update()
		{
			Init();

			if (setBtnUpdate)
			{
				CreateAllBtn();
				setBtnUpdate = false;
			}
		}
		void Start()
		{
			Init();
			CreateAllBtn();
		}
		void Init()
		{
			size = rectTransform.rect.width;
			if (rectTransform.rect.width > rectTransform.rect.height)
				size = rectTransform.rect.height;
			else
				size = rectTransform.rect.width;
			thickness = (float)Mathf.Clamp(thickness, 0, size / 2);

		}
		protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
		{
			UIVertex[] vbo = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				var vert = UIVertex.simpleVert;
				vert.color = color;
				vert.position = vertices[i];
				vert.uv0 = uvs[i];
				vbo[i] = vert;
			}
			return vbo;
		}
		public void CreateAllBtn()
		{
			ClearAllBtn();
			btnList.Clear();
			Debug.Log("CTMenuVC.Instance.IconList.Count" + CTMenuVC.Instance.IconList.Count);
			for (int i = 0; i < VerticesPos.Count - 1; i++)
			{

				GameObject newButton = Instantiate(Resources.Load("UI/IconButton")) as GameObject;
				//newButton.transform.position = VerticesPos[i];
				newButton.transform.position = (VerticesPos[i] + VerticesPos[(i + 1) % (VerticesPos.Count - 1)])/2.0f;
				Vector3 offset = ((VerticesPos[i] + VerticesPos[(i + 1) % (VerticesPos.Count - 1)]) / 2.0f).normalized * Vector2.Distance(newButton.transform.localPosition, Vector2.zero) * 0.5f;
				newButton.transform.SetParent(this.gameObject.transform, false);
				newButton.GetComponent<Button>().onClick.AddListener(delegate { transform.parent.GetComponent<CTMenuVC>().CreateTingIcon(newButton.GetComponent<Button>().transform.position + offset, (int)MainController.Instance.sides, rotation); });
				btnList.Add(newButton);
				GameObject newButtonA = Instantiate(Resources.Load("UI/IconButton")) as GameObject;
				newButtonA.transform.position = VerticesPos[i];
				Vector3 offsetA = (VerticesPos[i]).normalized * Vector2.Distance(newButtonA.transform.localPosition, Vector2.zero) * 0.5f;
				newButtonA.transform.SetParent(this.gameObject.transform, false);
				newButtonA.GetComponent<Button>().onClick.AddListener(delegate { transform.parent.GetComponent<CTMenuVC>().CreateTingIcon(newButtonA.GetComponent<Button>().transform.position + offsetA, (int)MainController.Instance.sides,rotation); });
				btnList.Add(newButtonA);
/*

				if(i==0)
				{
					var colors = newButton.GetComponent<Button>().colors;
					  colors.normalColor = Color.blue;
				  newButton.GetComponent<Button> ().colors = colors;
				}
				if (i == VerticesPos.Count - 2)
				{
					var colors = newButton.GetComponent<Button>().colors;
					colors.normalColor = Color.red;
					newButton.GetComponent<Button>().colors = colors;
				}*/
			}
			CheckBtnActive() ;
		}
		void CheckBtnActive() 
		{
			for(int i=0;i<btnList.Count;i++)
			{
				if(btnList[i]==null)continue;
				GameObject newButton =btnList[i];
				int insideIconIndex = isInside(newButton.GetComponent<Button>());
				if (insideIconIndex != -1)
				{
					//Debug.Log("id: " + id + " i: " + i + " insideIconIndex" + insideIconIndex);
					newButton.SetActive(false);
					if (i % 2 == 0)
					{
						if (btnList[(i + 1) % btnList.Count] != null) btnList[(i + 1) % btnList.Count].SetActive(false);
						if (btnList[(i +3 ) % btnList.Count] != null) btnList[(i +3) % btnList.Count].SetActive(false);
					}
					else		
					{
						if (btnList[(i - 1 + btnList.Count) % btnList.Count] != null) btnList[(i - 1 + btnList.Count) % btnList.Count].SetActive(false);
						if (btnList[(i - 3 + btnList.Count) % btnList.Count] != null) btnList[(i - 3 + btnList.Count) % btnList.Count].SetActive(false);
					}
				}
			}
		}
		/**
* 檢查button是否落在其他的icon區域內
*/
		bool pointInPolygon(int side, Vector2 point, List<Vector2> p)
		{
			bool bInside = false;
			for (int i = 0; i < side; i++)
			{
				//Debug.Log(" p[" + i + "].x:" + p[i].x + " p[" + i + "].y:" + p[i].y);
				if (Cross(point.x, point.y, p[i].x, p[i].y, p[(i + 1) % side].x, p[(i + 1) % side].y))
					bInside = !bInside;
			}
			return (bInside);
		}
		bool Cross(float x, float y, float x1, float y1, float x2, float y2)
		{
			bool bCross;

			if (((y < y1) == (y < y2)) || ((x >= x1) && (x >= x2)))
			{
				/* Side entirely above, below or entirely to left of point */
				bCross = false;
			}
			else if ((x < x1) && (x < x2))
			{
				/* Side to right of point, but not entirely above or below */
				bCross = true;
			}
			else if (x1 < x2)
			{
				/* Side not entirely above or below point AND              */
				/* Side not entirely to left or right                      */
				bCross = x < (x1 + (y - y1) * (x2 - x1) / (y2 - y1));
			}
			else
			{
				/* Same as above                                           */
				bCross = x < (x2 + (y - y2) * (x1 - x2) / (y1 - y2));
			}

			return (bCross);
		}  /* End of Cross */
		public int isInside(Button button)
		{
			int inside = -1;
			for (int i = 0; i < CTMenuVC.Instance.IconList.Count; i++)
			{
				if(id==i)continue;
				int veticesSize = CTMenuVC.Instance.IconList[i].VerticesPos.Count - 1;
				List<Vector2> wordPosList=new List<Vector2>();
				for(int j=0;j<veticesSize;j++)
				{
					Vector2 pos=CTMenuVC.Instance.IconList[i].transform.position;
					wordPosList.Add(CTMenuVC.Instance.IconList[i].VerticesPos[j] + pos);
				}
				if (pointInPolygon(veticesSize, button.transform.position, wordPosList)) inside = i;
			}

			return inside;
		}
		void ClearAllBtn()
		{
			List<Transform> parent = new List<Transform>(transform.GetComponentsInChildren<Transform>());
			int count = parent.Count;
			for (int i = 1; i < count; i++)
			{
				Transform child = parent[parent.Count - 1];
				if (Application.isPlaying) Destroy(child.gameObject);
				parent.RemoveAt(parent.Count - 1);

			}
		}
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			VerticesPos.Clear();
			Vector2 prevX = Vector2.zero;
			Vector2 prevY = Vector2.zero;
			Vector2 uv0 = new Vector2(0, 0);
			Vector2 uv1 = new Vector2(0, 1);
			Vector2 uv2 = new Vector2(1, 1);
			Vector2 uv3 = new Vector2(1, 0);
			Vector2 pos0;
			Vector2 pos1;
			Vector2 pos2;
			Vector2 pos3;
			float degrees = 360f / sides;
			int vertices = sides + 1;
			if (VerticesDistances.Length != vertices)
			{
				VerticesDistances = new float[vertices];
				for (int i = 0; i < vertices - 1; i++) VerticesDistances[i] = 1;
			}
			// last vertex is also the first!
			VerticesDistances[vertices - 1] = VerticesDistances[0];
			for (int i = 0; i < vertices; i++)
			{
				float outer = -rectTransform.pivot.x * size * VerticesDistances[i];
				float inner = -rectTransform.pivot.x * size * VerticesDistances[i] + thickness;
				float rad = Mathf.Deg2Rad * (i * degrees + rotation);
				float c = Mathf.Cos(rad);
				float s = Mathf.Sin(rad);
				uv0 = new Vector2(0, 1);
				uv1 = new Vector2(1, 1);
				uv2 = new Vector2(1, 0);
				uv3 = new Vector2(0, 0);
				pos0 = prevX;
				pos1 = new Vector2(outer * c, outer * s);
				if (fill)
				{
					pos2 = Vector2.zero;
					pos3 = Vector2.zero;
				}
				else
				{
					pos2 = new Vector2(inner * c, inner * s);
					pos3 = prevY;
				}
				prevX = pos1;
				prevY = pos2;
				VerticesPos.Add(pos1);
				vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
			}
			setBtnUpdate = true;
		}
	}
}
