using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class MapGenerator : MonoBehaviour
{
	public int unitSize = 5;
	public int width;
	public int height;
	//Seed
	public string seed;
	public bool useRandomSeed;
	//Map
	[Range(0, 100)]
	public int landFillPercent = 50;
	public int waterRegionThresholdSize = 10;
	public int landThresholdSize = 10;
	public bool autoUpdate=false;
	int smoothTime = 3;
	int passageSize = 5;

	[Range(0, 100)]
	public int waterExpandPercent=50;
	//NoiseMap
	public Noise.NormalizeMode normalizeMode;
	public int octaves;
	[Range(0, 1)]
	public float persistance;
	public float lacunarity;
	public float noiseScale;
	public Vector2 offset;
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public TerrainsType[] terrains;
	//Map
	public int[,] map{ get; set; }
	//FalloffMap
	public float[,] falloffMap{ get; set; }
	//NoiseMap
	public float[,] noiseMap{ get; set; }
	public float[,] noiseOffsetMap { get; set; }
	//ColorMap
	public Color[] colorMap{ get; set; }
	public Texture2D textureMap{ get; set; }

	HashSet<int> checkedVertices = new HashSet<int>();
	//map
	public MeshGenerator mapMeshGen=null;
	private MeshFilter meshFilter=null;
	//------------------------
	public enum TileType { Water = 0, Land = 1 }
	public List<Region> survivingLandRegions = new List<Region>();
	public List<Region> survivingWaterRegions = new List<Region>();
	//GA algorithm------------------------
	public List<Region> buildingRegions = new List<Region>();
	GeneticAlgorithm ga;
	StimulatedAnnealing sa;
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
	public void ResetNoiseOffsetMap(float[,] _noiseOffsetMap)
	{
		noiseOffsetMap=_noiseOffsetMap;
	}
	public void ResetNoiseOffsetMap(int[,] _map)
	{
		for(int x=0;x<width;x++)
		{
			for(int y=0;y<height;y++)
			{
				if (_map[x, y] == (int)TileType.Water)
				{
					noiseOffsetMap[x,y]=0;
				}
			}
		}
	}
	public void ResetMap(int[,] _map)
	{
		map = _map;
	}
	public void ResetRegion(int updateType=0)//0:皆空值為重新計算地圖，1:map空為建造山，2:noiseOffsetMap空為建造水
	{
		for (int i = 0; i < survivingWaterRegions.Count; i++)
		{
			if (survivingWaterRegions[i].regionObject != null) Destroy(survivingWaterRegions[i].regionObject);
		}
		for (int i = 0; i < survivingLandRegions.Count; i++)
		{
			if (survivingLandRegions[i].regionObject!=null) Destroy(survivingLandRegions[i].regionObject);
		}
		survivingLandRegions.Clear();
		survivingWaterRegions.Clear();
		GenerateMap(updateType);
	}
	public void SetGA()
	{
		if (buildingRegions.Count > 0)
		{
			foreach (Region region in buildingRegions)
			{
				Destroy(region.regionObject);
			}
		}
		buildingRegions.Clear();


		ga = new GeneticAlgorithm(map);
		ga.Start();
		buildingRegions = ga.ShowResult();
		// 			sa = new StimulatedAnnealing(map);
		// 			sa.Start();
		// 			buildingRegions = sa.ShowResult();
		CreateRegionMesh(buildingRegions, Color.green, meshHeightCurve, meshHeightMultiplier, (noiseMap), (falloffMap));
	}
	Vector3 CoordToWorldPoint(Coord tile)
	{
		return new Vector3(-width * unitSize / 2.0f + unitSize * tile.tileX + unitSize / 2.0f, 0, -height * unitSize / 2.0f + unitSize * tile.tileY + unitSize / 2.0f);
	}
	void GenerateMap(int updateType=0)
	{
		switch (updateType)//皆空值為重新計算地圖
		{
			case 0:
			Debug.Log("CreateNewMap");
			map = new int[width, height];
			noiseOffsetMap = new float[width, height];
			//隨機填入0(water),1(land)產生地圖
			RandomFillMap();
			//檢查周圍
			for (int i = 0; i < smoothTime; i++)
			{
				SmoothMap();
			}

			//ProcessMap();
			//
			//產生falloffMap
			falloffMap = FalloffGenerator.GenerateFalloffMap(map);
			//產生noiseMap
			noiseMap = Noise.GenerateNoiseMap(width, height, seed.GetHashCode(), noiseScale, octaves, persistance, lacunarity, offset, normalizeMode, (falloffMap));
			colorMap = ColorMapGenerator(noiseMap, map, width, height, noiseOffsetMap);
			textureMap = TextureGenerator.TextureFromColourMap(colorMap, width, height);
			CreateMapMesh(textureMap, meshHeightCurve, meshHeightMultiplier, (noiseMap), (falloffMap), noiseOffsetMap);
			/*	//創建mesh與計算outline
		survivingLandRegions = new List<Region>();
		survivingLandRegions = GetAllRegion((int)TileType.Land);
		CreateRegionMesh(survivingLandRegions, textureMap, meshHeightCurve, meshHeightMultiplier, (noiseMap), (falloffMap));
		survivingWaterRegions = new List<Region>();
		survivingWaterRegions = GetAllRegion((int)TileType.Water);
		Color waterColor;
		UnityEngine.ColorUtility.TryParseHtmlString("#05336000",out waterColor);
		CreateRegionMesh(survivingWaterRegions, waterColor, meshHeightCurve, meshHeightMultiplier, (noiseMap), (falloffMap));
		ShowRegionOutline(survivingLandRegions.ToArray(), Color.green);*/
			ShowRegionOutline(survivingLandRegions.ToArray(), Color.green);
			//ShowRegionOutline(survivingWaterRegions.ToArray(), Color.red);
		break;
		case 1://map空為建造山
		
			Debug.Log("CreateMountain");
			//產生falloffMap
			falloffMap = FalloffGenerator.GenerateFalloffMap(map);
			//產生noiseMap
			noiseMap = Noise.GenerateNoiseMap(width, height, seed.GetHashCode(), noiseScale, octaves, persistance, lacunarity, offset, normalizeMode, (falloffMap));
			colorMap = ColorMapGenerator(noiseMap, map, width, height, noiseOffsetMap);
			textureMap = TextureGenerator.TextureFromColourMap(colorMap, width, height);

			CreateMapMesh(textureMap, meshHeightCurve, meshHeightMultiplier, (noiseMap), (falloffMap), noiseOffsetMap);
		break;
		case 2://noiseOffsetMap空為建造水
		
			Debug.Log("CreateWater");
			//
			//產生falloffMap
			falloffMap = FalloffGenerator.GenerateFalloffMap(map);
			//產生noiseMap
			noiseMap = Noise.GenerateNoiseMap(width, height, seed.GetHashCode(), noiseScale, octaves, persistance, lacunarity, offset, normalizeMode, (falloffMap));
			colorMap = ColorMapGenerator(noiseMap, map, width, height, noiseOffsetMap);
			textureMap = TextureGenerator.TextureFromColourMap(colorMap, width, height);

			CreateMapMesh(textureMap, meshHeightCurve, meshHeightMultiplier, (noiseMap), (falloffMap), noiseOffsetMap);
		break;
		}

	}
	void CreateMapMesh(Texture2D texture, AnimationCurve heightCurve = null, float heightMultiplier = 0, float[,] noiseMap = null, float[,] fallofMap = null, float[,] _noiseOffsetMap = null) 
	{
		int[,] array = new int[map.GetLength(0), map.GetLength(1)];

		for (int i = 0; i < map.GetLength(0); i++)
		{
			for (int j = 0; j < map.GetLength(1); j++)
			{
				array[i, j] = 1;
			}
		}
		if(meshFilter==null)
		{ 
			GameObject mapObject = new GameObject();
			mapMeshGen = mapObject.AddComponent<MeshGenerator>();
			meshFilter = mapObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = mapObject.AddComponent<MeshRenderer>();
			mapObject.transform.parent = this.transform;

			mapMeshGen.GenerateMesh(array, unitSize, meshFilter, heightCurve, heightMultiplier, noiseMap, fallofMap, _noiseOffsetMap);
			meshRenderer.material.mainTexture = textureMap;
			mapObject.AddComponent<MeshCollider>();
			mapObject.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
		}
		else 
		{
			mapMeshGen.GenerateMesh(array, unitSize, meshFilter, heightCurve, heightMultiplier, noiseMap, fallofMap, _noiseOffsetMap);
			mapMeshGen.gameObject.GetComponent<MeshRenderer>().material.mainTexture = textureMap;
			mapMeshGen.gameObject.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
		}
		survivingLandRegions = new List<Region>();
		survivingLandRegions = GetAllRegion((int)TileType.Land);
		for (int i = 0; i < survivingLandRegions.Count; i++)
		{
			survivingLandRegions[i].CalculateRegionOutline(unitSize, meshHeightCurve, meshHeightMultiplier, (noiseMap), (falloffMap));
		}
		survivingWaterRegions = new List<Region>();
		survivingWaterRegions = GetAllRegion((int)TileType.Water);
		for (int i = 0; i < survivingWaterRegions.Count; i++)
		{
			survivingWaterRegions[i].CalculateRegionOutline(unitSize, meshHeightCurve, meshHeightMultiplier, (noiseMap), (falloffMap));
		}
	}
	void CreateRegionMesh(List<Region> regions, Texture2D texture, AnimationCurve heightCurve = null, float heightMultiplier = 0, float[,] noiseMap = null, float[,] fallofMap = null)
	{
		for (int i = 0; i < regions.Count; i++)
		{
			regions[i].CreateRegionMesh(this.gameObject.transform, unitSize, texture, heightCurve, heightMultiplier, noiseMap, fallofMap);
		}
	}
	void CreateRegionMesh(List<Region> regions, Color color, AnimationCurve heightCurve = null, float heightMultiplier = 0, float[,] noiseMap = null, float[,] fallofMap = null)
	{
		for (int i = 0; i < regions.Count; i++)
		{
			regions[i].CreateRegionMesh(this.gameObject.transform, unitSize, color, heightCurve, heightMultiplier, noiseMap, fallofMap);
		}
	}

	public void ShowRegionOutline(Region[] regions, Color color,float duration=1)
	{
		for (int i = 0; i < regions.Length; i++)
		{
			for (int j = 0; j < regions[i].outlinePosList.Count; j++)
			{
				for (int k = 0; k < regions[i].outlinePosList[j].Count - 1; k++)
				{
					Debug.DrawLine(regions[i].outlinePosList[j][k], regions[i].outlinePosList[j][k + 1], color, duration);
				}
			}
		}
	}
	void ExpandSizeOfWater()
	{
	
		//Water
		List<List<Coord>> waterRegions = GetRegions((int)TileType.Water);

		foreach (List<Coord> waterRegion in waterRegions)
		{
			if (waterRegion.Count < waterRegionThresholdSize)
			{
				foreach (Coord tile in waterRegion)
				{
					map[tile.tileX, tile.tileY] = (int)TileType.Water;
				}
			}

		}

	}
	void ProcessMap()
	{
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

		waterRegions = GetRegions((int)TileType.Water);
		landRegions = GetRegions((int)TileType.Land);

	}
	Color[] ColorMapGenerator(float[,] noiseMap, int[,] map, int width, int height, float[,] noiseOffsetMap) 
	{
		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float currentHeight = noiseMap[x, y] + noiseOffsetMap[x,y];
				if (map[x, y] == 0) colourMap[y * height + x] = terrains[0].colour;
				else 
				{ 
				colourMap[y * height + x] = terrains[1].colour;
				for (int n = 1; n < terrains.Length; n++)
				{
					if (currentHeight >= terrains[n].height)
					{
						colourMap[y * height + x] = terrains[n].colour;
					}
					else
					{
						break;
					}
				}
				}
			}
		}
		return colourMap;
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
		//Debug.Log(map.GetLength(0) * map.GetLength(1));
		if (useRandomSeed)
		{
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < map.GetLength(0); x++)
		{
			for (int y = 0; y < map.GetLength(1); y++)
			{
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
				{
					map[x, y] = (int)TileType.Land;
				}
				else
				{
					map[x, y] = (pseudoRandom.Next(0, 100) <= landFillPercent) ? (int)TileType.Land : (int)TileType.Water;					
				}
				//Debug.Log(map[x, y]);
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

				if (neighbourLandTiles > 4)
					map[x, y] = (int)TileType.Land;
				else if (neighbourLandTiles < 4)
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
	public bool isActive=false;
	public List<Coord> tiles;
	public List<Coord> edgeTiles;
	public List<Region> connectedRegions;
	public int regionSize;
	public bool isAccessibleFromMainRegion;
	public bool isMainRegion;
	public int regionType;
	public List<List<Vector3>> outlinePosList;

	public GameObject regionObject = null;
	MeshFilter meshFilter=null;
	MeshGenerator meshGen = null;




	int[,] map;

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
	public void CalculateRegionOutline(float squareSize, AnimationCurve heightCurve = null, float heightMultiplier = 0, float[,] noiseMap = null, float[,] fallofMap = null)
	{
		GameObject newObject = new GameObject();
		MeshGenerator newMeshGen = newObject.AddComponent<MeshGenerator>();
		MeshFilter newMeshFilter = newObject.AddComponent<MeshFilter>();
		newMeshGen.GenerateMesh(CaculatedRegionMap(), squareSize, newMeshFilter, heightCurve, heightMultiplier, noiseMap, fallofMap);
		outlinePosList = newMeshGen.CalculateMeshOutlinePos();
		UnityEngine.Object.Destroy(newObject);
	}
	public void CreateRegionMesh(Transform parent, float squareSize, Texture2D texture, AnimationCurve heightCurve = null, float heightMultiplier = 0, float[,] noiseMap = null, float[,] fallofMap = null)
	{
		regionObject = new GameObject();
		meshGen = regionObject.AddComponent<MeshGenerator>();
		meshFilter = regionObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = regionObject.AddComponent<MeshRenderer>();
		regionObject.transform.parent = parent.transform;
		meshGen.GenerateMesh(CaculatedRegionMap(), squareSize, meshFilter, heightCurve, heightMultiplier, noiseMap, fallofMap);
		meshRenderer.material.mainTexture = texture;
		outlinePosList = meshGen.CalculateMeshOutlinePos();
		regionObject.AddComponent<MeshCollider>();
		regionObject.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
	}
	public void CreateRegionMesh(Transform parent, float squareSize, Color color, AnimationCurve heightCurve = null, float heightMultiplier = 0, float[,] noiseMap = null, float[,] fallofMap = null)
	{
		regionObject = new GameObject();
		meshGen = regionObject.AddComponent<MeshGenerator>();
		meshFilter = regionObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = regionObject.AddComponent<MeshRenderer>();
		meshRenderer.material.color = color;
		regionObject.transform.parent = parent.transform;
		meshGen.GenerateMesh(CaculatedRegionMap(), squareSize, meshFilter, heightCurve, heightMultiplier, noiseMap, fallofMap);
		outlinePosList = meshGen.CalculateMeshOutlinePos();
		regionObject.AddComponent<MeshCollider>();
		regionObject.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;

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
	public int tileX;
	public int tileY;
	public Coord()
	{

	}
	public Coord(Coord _coord)
	{
		tileX = _coord.tileX;
		tileY = _coord.tileY;
	}
	public Coord(int _x, int _y)
	{
		tileX = _x;
		tileY = _y;
	}
}
[System.Serializable]
public struct TerrainsType
{
	public string name;
	public float height;
	public Color colour;
}
