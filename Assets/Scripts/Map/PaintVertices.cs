using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PaintVertices : MonoBehaviour
{
	public float radius = 20.0f;
	public float pull = 300.0f;
	public enum FallOff { Gauss, Linear, Needle };
	public enum OptionType { AddMoutain, RemoveMoutain, AddWaterRegion ,AddLandRegion};
	public FallOff fallOff = FallOff.Gauss;
	public OptionType optionType = OptionType.AddMoutain;
	public AnimationCurve heightCurve;
	public MapGenerator mapGenerator;
	MeshFilter unappliedMesh;

	float LinearFalloff(float distance, float inRadius)
	{
		return Mathf.Clamp01(1.0f - distance / inRadius);
	}
	float GaussFalloff(float distance, float inRadius)
	{
		return Mathf.Clamp01(Mathf.Pow(360.0f, -Mathf.Pow(distance / inRadius, 2.5f) - 0.01f));
	}
	float NeedleFalloff(float dist, float inRadius)
	{
		return -(dist * dist) / (inRadius * inRadius) + 1.0f;
	}
	int[,] SetMap(MeshGenerator meshGenerator, Vector3 position, float power, float inRadius,int type)
	{
		int[,] map = mapGenerator.map;
		float sqrRadius = inRadius * inRadius;

		for (int x = 0; x < meshGenerator.squareGrid.squares.GetLength(0); x++)
		{
			for (int y = 0; y < meshGenerator.squareGrid.squares.GetLength(1); y++)
			{
				float sqrMagnitude = (meshGenerator.squareGrid.squares[x, y].centreTop.position - position).sqrMagnitude;
				// Early out if too far away
				if (sqrMagnitude > sqrRadius)
					continue;
				float distance = Mathf.Sqrt(sqrMagnitude);
				float falloff = LinearFalloff(distance, inRadius);
				switch (fallOff)
				{
					case FallOff.Gauss:
						falloff = GaussFalloff(distance, inRadius);
						break;
					case FallOff.Needle:
						falloff = NeedleFalloff(distance, inRadius);
						break;
					default:
						falloff = LinearFalloff(distance, inRadius);
						break;
				}
				map[x, y] = type;
			}
		}
		return map;
	}
	float[,] SetNoiseOffsetMap(MeshGenerator meshGenerator, Vector3 position, float power, float inRadius) 
	{
		float[,] noiseOffsetMap = mapGenerator.noiseOffsetMap;
		float sqrRadius = inRadius * inRadius;

		for (int x = 0; x < meshGenerator.squareGrid.squares.GetLength(0); x++)
		{
			for (int y = 0; y < meshGenerator.squareGrid.squares.GetLength(1); y++)
			{
				float sqrMagnitude = (meshGenerator.squareGrid.squares[x, y].centreTop.position - position).sqrMagnitude;
				// Early out if too far away
				if (sqrMagnitude > sqrRadius)
					continue;
				float distance = Mathf.Sqrt(sqrMagnitude);
				float falloff = LinearFalloff(distance, inRadius);
				switch (fallOff)
				{
					case FallOff.Gauss:
						falloff = GaussFalloff(distance, inRadius);
						break;
					case FallOff.Needle:
						falloff = NeedleFalloff(distance, inRadius);
						break;
					default:
						falloff = LinearFalloff(distance, inRadius);
						break;
				}
				//Debug.Log("noiseMap[x, y]" + mapGenerator.noiseMap[x, y]);
				switch (optionType)
				{
					case OptionType.AddMoutain:
						if (mapGenerator.falloffMap[x, y] < 0.1f) noiseOffsetMap[x, y] = heightCurve.Evaluate(noiseOffsetMap[x, y] + falloff * power);
					break;
					case OptionType.RemoveMoutain:
						noiseOffsetMap[x, y] =0;
					break;
				}
				//Debug.Log("noiseOffsetMap[x, y]"+noiseOffsetMap[x, y]);
			}
		}
		return noiseOffsetMap;
	}
	void DeformMesh(Mesh mesh, Vector3 position, float power, float inRadius)
	{
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		float sqrRadius = inRadius * inRadius;

		// Calculate averaged normal of all surrounding vertices	
		Vector3 averageNormal = Vector3.zero;
		for (int i = 0; i < vertices.Length; i++)
		{
			float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
			// Early out if too far away
			if (sqrMagnitude > sqrRadius)
				continue;

			float distance = Mathf.Sqrt(sqrMagnitude);
			float falloff = LinearFalloff(distance, inRadius);
			averageNormal += falloff * normals[i];
		}
		averageNormal = averageNormal.normalized;

		// Deform vertices along averaged normal
		for (int i = 0; i < vertices.Length; i++)
		{
			float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
			// Early out if too far away
			if (sqrMagnitude > sqrRadius)
				continue;

			float distance = Mathf.Sqrt(sqrMagnitude);
			float falloff = 0;
			switch (fallOff)
			{
				case FallOff.Gauss:
					falloff = GaussFalloff(distance, inRadius);
					break;
				case FallOff.Needle:
					falloff = NeedleFalloff(distance, inRadius);
					break;
				default:
					falloff = LinearFalloff(distance, inRadius);
					break;
			}

			vertices[i] += averageNormal * falloff * power;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
	public void ApplyMeshCollider()
	{
		if (unappliedMesh && unappliedMesh.GetComponent<MeshFilter>())
		{
			unappliedMesh.GetComponent<MeshFilter>().mesh = unappliedMesh.mesh;
		}
		unappliedMesh = null;
	}
	static Rect RectTransformToScreenSpace(RectTransform transform)
	{
		Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
		return new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
	}

	public void OnClick(GameObject targetTexture) //
	{
		if (Input.GetMouseButton(0))
		{
			RectTransform rectTransform = targetTexture.GetComponent<RectTransform>();
			RaycastHit hitInfo;
			Vector2 hitLocalUV;
			Vector3 localHit = Input.mousePosition;
			Vector3[] corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);
			// normalize
			hitLocalUV.x = (localHit.x - corners[0].x);
			hitLocalUV.y = (localHit.y - corners[0].y);
			Camera uiCamera = GameObject.Find("UI Camera").GetComponent<Camera>();
			Ray srsRay = uiCamera.ScreenPointToRay(new Vector2(hitLocalUV.x, hitLocalUV.y));
			if (Physics.Raycast(srsRay, out hitInfo))
			{
				MeshFilter filter = hitInfo.collider.GetComponent<MeshFilter>();
				if (filter && mapGenerator)
				{
					// Don't update mesh collider every frame since physX
					// does some heavy processing to optimize the collision mesh.
					// So this is not fast enough for real time updating every frame
					if (filter != unappliedMesh)
					{
						ApplyMeshCollider();
						unappliedMesh = filter;
					}
					// Deform mesh
					var relativePoint = filter.transform.InverseTransformPoint(hitInfo.point);
					//DeformMesh(filter.mesh, relativePoint, pull * Time.deltaTime, radius);
					switch (optionType)
					{
						case OptionType.AddMoutain:
						case OptionType.RemoveMoutain:
							mapGenerator.ResetNoiseOffsetMap(SetNoiseOffsetMap(mapGenerator.mapMeshGen, relativePoint, pull * Time.deltaTime, radius));
							mapGenerator.ResetRegion(1);
							break;
						case OptionType.AddWaterRegion:
							int[,] newMap = SetMap(mapGenerator.mapMeshGen, relativePoint, pull * Time.deltaTime, radius, (int)MapGenerator.TileType.Water);
							mapGenerator.ResetMap(newMap);
							mapGenerator.ResetNoiseOffsetMap(newMap);
							mapGenerator.ResetRegion(2);
							break;
						case OptionType.AddLandRegion:
							newMap = SetMap(mapGenerator.mapMeshGen, relativePoint, pull * Time.deltaTime, radius, (int)MapGenerator.TileType.Land);
							mapGenerator.ResetMap(newMap);
							mapGenerator.ResetNoiseOffsetMap(newMap);
							mapGenerator.ResetRegion(2);
							break;
					}
				
				}
			}
		}
	}
}

