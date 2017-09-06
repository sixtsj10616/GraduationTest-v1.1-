
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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

		GameObject selectBtn;
		public List<Vector2> VerticesPos = new List<Vector2>();
		public List<GameObject> btnList=new List<GameObject>();
		public List<int> btnDisableIndexList = new List<int>();
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
				Debug.Log("Update");
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
			for (int i = 0; i < btnList.Count; i++)
			{
				btnList[i].GetComponent<Button>().onClick.RemoveAllListeners();
				Destroy(btnList[i].gameObject);
			}
			btnList.Clear();
			List<Vector3> offsetPosList=new List<Vector3>();
			//選擇屋頂狀態的icon
			GameObject newSelectBtn = Instantiate(Resources.Load("UI/IconButton")) as GameObject;
			newSelectBtn.transform.position = transform.position;
			newSelectBtn.transform.SetParent(this.gameObject.transform);
			selectBtn=newSelectBtn;
			UnityEngine.Events.UnityAction actionSelect = () => { MainController.Instance.SelectBuilding(id); };
			newSelectBtn.GetComponent<Button>().onClick.RemoveAllListeners();
			newSelectBtn.GetComponent<Button>().onClick.AddListener(
				actionSelect

			);
			//Icon周圍邊對齊與脊對齊的button
			for (int i = 0; i < VerticesPos.Count - 1; i++)
			{
				GameObject newButton = Instantiate(Resources.Load("UI/IconButton")) as GameObject;
				newButton.transform.position = VerticesPos[i];
				Vector3 offset = (VerticesPos[i]).normalized * Vector2.Distance(newButton.transform.localPosition, Vector2.zero) * 0.5f;
				newButton.transform.SetParent(this.gameObject.transform, false);
				offsetPosList.Add(newButton.transform.position + offset);
				btnList.Add(newButton);

				GameObject newButtonA = Instantiate(Resources.Load("UI/IconButton")) as GameObject;
				newButtonA.transform.position = (VerticesPos[i] + VerticesPos[(i + 1) % (VerticesPos.Count - 1)]) / 2.0f;
				Vector3 offsetA = ((VerticesPos[i] + VerticesPos[(i + 1) % (VerticesPos.Count - 1)]) / 2.0f).normalized * Vector2.Distance(newButton.transform.localPosition, Vector2.zero) * 0.5f;
				newButtonA.transform.SetParent(this.gameObject.transform, false);
				offsetPosList.Add(newButtonA.transform.position + offsetA);
				btnList.Add(newButtonA);
			}
			//Lambdas現象
			for (int i = 0; i < btnList.Count; i++)
			{
				Button but = btnList[i].GetComponent<Button>();
				Vector3 pos=offsetPosList[i];
				CTMenuVC thisScript = transform.parent.GetComponent<CTMenuVC>();
				int btnId = i;
				int btnIdA = i;
				UnityEngine.Events.UnityAction action = () => { thisScript.CreateTingIcon(pos, sides, rotation, btnId);};
				UnityEngine.Events.UnityAction actionA = () => { SetBtnDisableIndex(btnIdA); };
				but.onClick.RemoveAllListeners();
				but.onClick.AddListener(

					action

				);
				but.onClick.AddListener(

					actionA

				);
				if (i == 0)
				{
					var colors = but.colors;
					colors.normalColor = Color.blue;
					but.colors = colors;
				}
				if (i == 2)
				{
					var colors = but.colors;
					colors.normalColor = Color.red;
					but.colors = colors;
				}
			}
			ChectBtnActive();
		}
		public void ChectBtnActive() 
		{ 
			for(int i=0;i<btnDisableIndexList.Count;i++)
			{
				if (btnDisableIndexList[i] < btnList.Count)
					if (btnList[btnDisableIndexList[i]] != null) btnList[btnDisableIndexList[i]].SetActive(false);
			}
		}
		public void SetBtnDisableIndex(int btnId) 
		{
			int size=sides*2;
			btnDisableIndexList.Add(btnId);
			btnDisableIndexList.Add((btnId + 1) % size);
			btnDisableIndexList.Add((btnId - 1 + size) % size);
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
