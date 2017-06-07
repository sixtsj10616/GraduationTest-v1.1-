using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour
{
	public float unitSize = 5;
	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range(0, 100)]
	public int landFillPercent = 50;
	public int waterRegionThresholdSize = 10;
	public int landThresholdSize = 10;

	int smoothTime = 3;
	float smoothRatio = 0.5f;
	int borderSize = 0;
	int passageSize = 5;

	int[,] map;

	HashSet<int> checkedVertices = new HashSet<int>();
	//------------------------
	public enum TileType { Water = 0, Land = 1 }
	List<Region> survivingLandRegions;
	List<Region> survivingWaterRegions;
	//------------------------
	void Start()
	{
		GenerateMap();
	}
	void ShowPos(Vector3 pos, GameObject parent, Color color, float localScale = 0.2f)
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		obj.transform.position = pos;
		obj.transform.parent = parent.transform;
		obj.transform.localScale = Vector3.one * localScale;
		obj.GetComponent<MeshRenderer>().material.color = color;
	}
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
		
			for(int i=0;i<survivingWaterRegions.Count;i++)
			{ 
				Destroy(survivingWaterRegions[i].regionObject);
			}
			for (int i = 0; i < survivingLandRegions.Count; i++)
			{
				Destroy(survivingLandRegions[i].regionObject);
			}
			survivingLandRegions.Clear();
			survivingWaterRegions.Clear();
			GenerateMap();
		}
	}
	Vector3 CoordToWorldPoint(Coord tile)
	{
		return new Vector3(-width * unitSize / 2.0f + unitSize * tile.tileX + unitSize / 2.0f, 0, -height * unitSize / 2.0f + unitSize * tile.tileY + unitSize / 2.0f);
	}
	void GenerateMap()
	{
		//隨機填入0(water),1(land)產生地圖
		map = new int[width, height];
		RandomFillMap();
		//檢查周圍
		for (int i = 0; i < smoothTime; i++)
		{
			SmoothMap();
		}

		ProcessMap();

		//創建mesh與計算outline
		survivingLandRegions = new List<Region>();
		survivingLandRegions = GetAllRegion((int)TileType.Land);
		CreateRegionMesh(survivingLandRegions,Color.white);
		survivingWaterRegions = new List<Region>();
		survivingWaterRegions = GetAllRegion((int)TileType.Water);
		CreateRegionMesh(survivingWaterRegions, Color.blue);
		for (int i = 0; i < survivingWaterRegions.Count; i++)
		{
			for (int j = 0; j < survivingWaterRegions[i].outlinePosList.Count; j++)
			{
				for (int k = 0; k < survivingWaterRegions[i].outlinePosList[j].Count-1; k++)
				{
					Debug.DrawLine(survivingWaterRegions[i].outlinePosList[j][k], survivingWaterRegions[i].outlinePosList[j][k+1], Color.green, 1000);
				}
			}
		}

		ShowRegionOutline(survivingLandRegions.ToArray(),Color.green);

		//ShowRegionOutline(survivingWaterRegions.ToArray(), Color.red);

		GeneticAlgorithm ga=new GeneticAlgorithm(map);
		ga.Start();
		List<Region> buildingRegions=ga.ShowResult();
		CreateRegionMesh(buildingRegions, Color.green);
	}
	void CreateRegionMesh(List<Region> regions, Color color)
	{
		for(int i=0;i<regions.Count;i++)
		{
			regions[i].CreateRegionMesh(this.gameObject.transform,unitSize,color);
		}
	}
	void ShowRegionOutline(Region[] regions, Color color)
	{
		for (int i = 0; i < regions.Length; i++)
		{
			for (int j = 0; j < regions[i].outlinePosList.Count; j++)
			{
				for (int k = 0; k < regions[i].outlinePosList[j].Count - 1; k++)
				{
					Debug.DrawLine(regions[i].outlinePosList[j][k], regions[i].outlinePosList[j][k + 1], color, 10);
				}
			}
		}
	}
	void ProcessMap()
	{
		//Water
		List<List<Coord>> waterRegions = GetRegions((int)TileType.Water);

		foreach (List<Coord> waterRegion in waterRegions)
		{
			if (waterRegion.Count < waterRegionThresholdSize)
			{
				foreach (Coord tile in waterRegion)
				{
					map[tile.tileX, tile.tileY] = (int)TileType.Land;
				}
			}

		}
		//Land
		List<List<Coord>> landRegions = GetRegions((int)TileType.Land);
		List<Region> survivingLandRegions = new List<Region>();

		foreach (List<Coord> landRegion in landRegions)
		{
			if (landRegion.Count < landThresholdSize)
			{
				foreach (Coord tile in landRegion)
				{
					map[tile.tileX, tile.tileY] = (int)TileType.Water;
				}

			}
			else
			{
				survivingLandRegions.Add(new Region(landRegion, (int)TileType.Land, map));
			}
		}
		if (survivingLandRegions.Count > 0)
		{
			survivingLandRegions.Sort();
			survivingLandRegions[0].isMainRegion = true;
			survivingLandRegions[0].isAccessibleFromMainRegion = true;

			ConnectClosestRegions(survivingLandRegions, (int)TileType.Land);
		}
// 		int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];
// 
// 		for (int x = 0; x < borderedMap.GetLength(0); x++)
// 		{
// 			for (int y = 0; y < borderedMap.GetLength(1); y++)
// 			{
// 				if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
// 				{
// 					borderedMap[x, y] = map[x - borderSize, y - borderSize];
// 				}
// 				else
// 				{
// 					borderedMap[x, y] = (int)TileType.Land;
// 				}
// 			}
// 		}
	}
	List<Region> GetAllRegion(int regionType)
	{
		List<List<Coord>> regions = GetRegions(regionType);
		List<Region> survivingRegions = new List<Region>();
		foreach (List<Coord> region in regions)
		{
			survivingRegions.Add(new Region(region, regionType, map));
		}
		return survivingRegions;
	}
	void ConnectClosestRegions(List<Region> allRegions, int regionType, bool forceAccessibilityFromMainRegion = false)
	{

		List<Region> regionListA = new List<Region>();
		List<Region> regionListB = new List<Region>();

		if (forceAccessibilityFromMainRegion)
		{
			foreach (Region region in allRegions)
			{
				if (region.isAccessibleFromMainRegion)
				{
					regionListB.Add(region);
				}
				else
				{
					regionListA.Add(region);
				}
			}
		}
		else
		{
			regionListA = allRegions;
			regionListB = allRegions;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord();
		Coord bestTileB = new Coord();
		Region bestRegionA = new Region();
		Region bestRegionB = new Region();
		bool possibleConnectionFound = false;

		foreach (Region regionA in regionListA)
		{
			if (!forceAccessibilityFromMainRegion)
			{
				possibleConnectionFound = false;
				if (regionA.connectedRegions.Count > 0)
				{
					continue;
				}
			}

			foreach (Region regionB in regionListB)
			{
				if (regionA == regionB || regionA.IsConnected(regionB))
				{
					continue;
				}

				for (int tileIndexA = 0; tileIndexA < regionA.edgeTiles.Count; tileIndexA++)
				{
					for (int tileIndexB = 0; tileIndexB < regionB.edgeTiles.Count; tileIndexB++)
					{
						Coord tileA = regionA.edgeTiles[tileIndexA];
						Coord tileB = regionB.edgeTiles[tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
						{
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRegionA = regionA;
							bestRegionB = regionB;
						}
					}
				}
			}
			if (possibleConnectionFound && !forceAccessibilityFromMainRegion)
			{
				CreatePassage(bestRegionA, bestRegionB, bestTileA, bestTileB, regionType);
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMainRegion)
		{
			CreatePassage(bestRegionA, bestRegionB, bestTileA, bestTileB, regionType);
			ConnectClosestRegions(allRegions, regionType, true);
		}

		if (!forceAccessibilityFromMainRegion)
		{
			ConnectClosestRegions(allRegions, regionType, true);
		}
	}

	void CreatePassage(Region regionA, Region regionB, Coord tileA, Coord tileB, int regionType)
	{
		Region.ConnectRegions(regionA, regionB);

		List<Coord> line = GetLine(tileA, tileB);
		foreach (Coord c in line)
		{
			DrawCircle(c, passageSize, regionType);
		}
	}

	void DrawCircle(Coord c, int r, int regionType)
	{
		for (int x = -r; x <= r; x++)
		{
			for (int y = -r; y <= r; y++)
			{
				if (x * x + y * y <= r * r)
				{
					int drawX = c.tileX + x;
					int drawY = c.tileY + y;
					if (IsInMapRange(drawX, drawY))
					{
						map[drawX, drawY] = regionType;
					}
				}
			}
		}
	}

	List<Coord> GetLine(Coord from, Coord to)
	{
		List<Coord> line = new List<Coord>();

		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign(dx);
		int gradientStep = Math.Sign(dy);

		int longest = Mathf.Abs(dx);
		int shortest = Mathf.Abs(dy);

		if (longest < shortest)
		{
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);

			step = Math.Sign(dy);
			gradientStep = Math.Sign(dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i = 0; i < longest; i++)
		{
			line.Add(new Coord(x, y));

			if (inverted)
			{
				y += step;
			}
			else
			{
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest)
			{
				if (inverted)
				{
					x += gradientStep;
				}
				else
				{
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}

	List<List<Coord>> GetRegions(int tileType)
	{
		List<List<Coord>> regions = new List<List<Coord>>();
		int[,] mapFlags = new int[width, height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (mapFlags[x, y] == 0 && map[x, y] == tileType)
				{
					List<Coord> newRegion = GetRegionTiles(x, y);
					regions.Add(newRegion);

					foreach (Coord tile in newRegion)
					{
						mapFlags[tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	List<Coord> GetRegionTiles(int startX, int startY)
	{
		List<Coord> tiles = new List<Coord>();
		int[,] mapFlags = new int[width, height];
		int tileType = map[startX, startY];

		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord(startX, startY));
		mapFlags[startX, startY] = 1;

		while (queue.Count > 0)
		{
			Coord tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
			{
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
				{
					if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
					{
						if (mapFlags[x, y] == 0 && map[x, y] == tileType)
						{
							mapFlags[x, y] = 1;
							queue.Enqueue(new Coord(x, y));
						}
					}
				}
			}
		}
		return tiles;
	}

	bool IsInMapRange(int x, int y)
	{
		return x >= 0 && x < width && y >= 0 && y < height;
	}


	void RandomFillMap()
	{
		if (useRandomSeed)
		{
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
				{
					map[x, y] = (int)TileType.Land;
				}
				else
				{
					map[x, y] = (pseudoRandom.Next(0, 100) < landFillPercent) ? (int)TileType.Land : (int)TileType.Water;
				}
			}
		}
	}

	void SmoothMap()//九宮格周圍8個內(扣除本身)多於4個為1者才設定成陸地
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int neighbourLandTiles = GetSurroundingLandCount(x, y);

				if (neighbourLandTiles > smoothRatio * 8)
					map[x, y] = (int)TileType.Land;
				else if (neighbourLandTiles < smoothRatio * 8)
					map[x, y] = (int)TileType.Water;

			}
		}
	}

	int GetSurroundingLandCount(int gridX, int gridY)
	{
		int landCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
		{
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
			{
				if (IsInMapRange(neighbourX, neighbourY))
				{
					if (neighbourX != gridX || neighbourY != gridY)
					{
						landCount += map[neighbourX, neighbourY];
					}
				}
				else
				{
					landCount++;
				}
			}
		}

		return landCount;
	}

}
public class Region : IComparable<Region>
{
	public List<Coord> tiles;
	public List<Coord> edgeTiles;
	public List<Region> connectedRegions;
	public int regionSize;
	public bool isAccessibleFromMainRegion;
	public bool isMainRegion;
	public int regionType;
	public MeshFilter meshFilter;
	public MeshGenerator meshGen;
	public List<List<Vector3>> outlinePosList;
	int[,] map;
	//List<int> outlines;
	//HashSet<int> checkedVertices;
	public GameObject regionObject;
	public Region()
	{
	}

	public Region(List<Coord> regionTiles, int regionType, int[,] map)
	{
		tiles = regionTiles;
		regionSize = tiles.Count;
		this.regionType = regionType;
		this.map = map;
		connectedRegions = new List<Region>();
		edgeTiles = new List<Coord>();

		foreach (Coord tile in tiles)
		{
			for (int x = Mathf.Max(tile.tileX - 1, 0); x <= Mathf.Min(tile.tileX + 1, map.GetLength(0) - 1); x++)
			{
				for (int y = Mathf.Max(tile.tileY - 1, 0); y <= Mathf.Min(tile.tileY + 1, map.GetLength(1) - 1); y++)
				{
					if (x == tile.tileX || y == tile.tileY)
					{
						if (map[x, y] != regionType)
						{
							edgeTiles.Add(tile);
						}
					}
				}
			}
		}
		#region Test
		//edgeTiles = CalculateEdgeOutlines(edgeTiles);
		#endregion
	}
	#region Test
	// 		List<Coord> CalculateEdgeOutlines(List<Coord> edgeTiles)
	// 		{
	// 			checkedVertices = new HashSet<int>();
	// 			outlines = new List<int>();
	// 
	// 			int startIndex = 0;
	// 			if (!checkedVertices.Contains(startIndex))
	// 			{
	// 				int newOutlineVertex = GetConnectedOutlineTile(startIndex, edgeTiles);
	// 				if (newOutlineVertex != -1)
	// 				{
	// 					checkedVertices.Add(startIndex);
	// 					outlines.Add(startIndex);
	// 					FollowOutline(newOutlineVertex);
	// 					outlines.Add(startIndex);
	// 				}
	// 			}
	// 			List<Coord> newEdgeTiles = new List<Coord>();
	// 			for (int i = 0; i < outlines.Count; i++)
	// 			{
	// 				newEdgeTiles.Add(edgeTiles[outlines[i]]);
	// 			}
	// 			return newEdgeTiles;
	// 		}
	// 
	// 		int GetConnectedOutlineTile(int index, List<Coord> edgeTiles)
	// 		{
	// 			for (int i = 0; i < edgeTiles.Count; i++)
	// 			{
	// 				if ((i != index) && !checkedVertices.Contains(i))
	// 				{
	// 					if ((Math.Abs(edgeTiles[i].tileX - edgeTiles[index].tileX) <= 1 && Math.Abs(edgeTiles[i].tileY - edgeTiles[index].tileY) <=1))
	// 					{
	// 						Debug.Log(i);
	// 						return i;
	// 					}
	// 				}
	// 			}
	// 
	// 			return -1;
	// 		}
	// 		List<int> FollowOutline(int vertexIndex)
	// 		{
	// 			outlines.Add(vertexIndex);
	// 			checkedVertices.Add(vertexIndex);
	// 			int nextVertexIndex = GetConnectedOutlineTile(vertexIndex, edgeTiles);
	// 
	// 			if (nextVertexIndex != -1)
	// 			{
	// 				FollowOutline(nextVertexIndex);
	// 			}
	// 			return outlines;
	// 		}
	#endregion
	public void SetAccessibleFromMainRegion()
	{
		if (!isAccessibleFromMainRegion)
		{
			isAccessibleFromMainRegion = true;
			foreach (Region connectedRegion in connectedRegions)
			{
				connectedRegion.SetAccessibleFromMainRegion();
			}
		}
	}
	public int[,] CaculatedRegionMap()
	{
		int[,] regionMap = new int[map.GetLength(0), map.GetLength(1)];
		for (int i = 0; i < tiles.Count; i++)
		{
			regionMap[tiles[i].tileX, tiles[i].tileY] = 1;
		}
		return regionMap;
	}
	public void CreateRegionMesh(Transform parent, float squareSize, Color color)
	{
		regionObject = new GameObject();
		meshGen = regionObject.AddComponent<MeshGenerator>();
		meshFilter = regionObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = regionObject.AddComponent<MeshRenderer>();
		meshRenderer.material.color = color;
		regionObject.transform.parent = parent.transform;
		meshGen.GenerateMesh(CaculatedRegionMap(), squareSize, meshFilter);
		outlinePosList = meshGen.CalculateMeshOutlinePos();

	}
	public static void ConnectRegions(Region rA, Region rB)
	{
		if (rA.isAccessibleFromMainRegion)
		{
			rB.SetAccessibleFromMainRegion();
		}
		else if (rB.isAccessibleFromMainRegion)
		{
			rA.SetAccessibleFromMainRegion();
		}
		rA.connectedRegions.Add(rB);
		rB.connectedRegions.Add(rA);
	}

	public bool IsConnected(Region otherRegion)
	{
		return connectedRegions.Contains(otherRegion);
	}

	public int CompareTo(Region otherRegion)
	{
		return otherRegion.regionSize.CompareTo(regionSize);
	}
}
public class Coord
{
	public int tileX { get;  set; }
	public int tileY { get;  set; }
	public Coord() 
	{ 
	
	}

	public Coord(int x, int y)
	{
		tileX = x;
		tileY = y;
	}
}
