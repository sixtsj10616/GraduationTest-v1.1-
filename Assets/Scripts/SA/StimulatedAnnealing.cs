using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulatedAnnealing
{
	public enum TileType { Water = 0, Land = 1 }
	static int[,] map;
	public List<Spot> spots;
	public Chromosome currentSolution;
	public Chromosome bestSolution;
	float fitnessCurrent;
	float fitnessBest;
	public float tempReduction = 0.95f;
	public float temperature = 3000;
	public int iteration = 10000;
	public StimulatedAnnealing(int[,] _map)
	{
		map = _map;
	}
	void InitChromosome()
	{
		currentSolution = new Chromosome(spots);
		fitnessCurrent = Fitness(currentSolution);
		bestSolution = currentSolution;
		fitnessBest = fitnessCurrent;
	}
	public List<Region> ShowResult()
	{
		List<Region> buildingRegionList = new List<Region>();

		for (int i = 0; i < bestSolution.spots.Count; i++)
		{
			Region buildingRegion = new Region(bestSolution.spots[i].regionCoord, 2, map);
			buildingRegionList.Add(buildingRegion);
		}
		return buildingRegionList;
	}
	public void Start()
	{
		SpotCreater spotCreater = new SpotCreater();
		spotCreater.Init();
		spots = spotCreater.spots;
		SA();
	}
	public void SA()
	{
		InitChromosome();

		for (int i = 0; i < iteration; i++)
		{
			Chromosome newSolution = new Chromosome(currentSolution);
			NeighborhoodSearch(ref newSolution);
			float fitnessNew = Fitness(newSolution);

			float fitnessDiff = fitnessNew - fitnessCurrent;
			if (fitnessDiff <= 0)
			{
				if (fitnessNew < fitnessBest)
				{
					bestSolution = newSolution;
					fitnessBest = fitnessNew;
				}
				currentSolution = newSolution;
				fitnessCurrent = fitnessNew;
			}
			else
			{
				float rand = Random.Range(0.0f, 1.0f);
				if (rand <= BoltzmanFunction(fitnessDiff))
				{
					if (fitnessNew < fitnessBest)
					{
						bestSolution = newSolution;
						fitnessBest = fitnessNew;
					}
					currentSolution = newSolution;
					fitnessCurrent = fitnessNew;
				}
			}

			temperature *= tempReduction;
		}

	}
	float BoltzmanFunction(float fitnessDiff)
	{
		return Mathf.Min(1.0f, Mathf.Exp(-fitnessDiff / temperature));
	}
	void NeighborhoodSearch(ref Chromosome _chromosome)
	{
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			if (Random.Range(0.0f, 1.0f) < 0.3f) _chromosome.spots[i].RandomCenter();
		}
	}
	float Fitness(Chromosome _chromosome)
	{
		float fitnessValue = 0;

		float overlapWaterWeight = 100;
		float overlapWeight = 100;
		float disCenterXWeight = 10;
		//檢查是否有重疊到水
		float overlapWaterCount = 0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			for (int j = 0; j < _chromosome.spots[i].regionCoord.Count; j++)
			{
				if (map[_chromosome.spots[i].regionCoord[j].tileX, _chromosome.spots[i].regionCoord[j].tileY] == (int)TileType.Water)
				{
					overlapWaterCount++;
					break;
				}
			}
		}

		fitnessValue += overlapWaterWeight * overlapWaterCount;

		//檢查是否有重疊到彼此
		float overlapCount = 0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			for (int j = i + 1; j < _chromosome.spots.Count; j++)
			{
				if (_chromosome.spots[i].isOverlapOtherRegionCoordTile(_chromosome.spots[j]))
				{
					overlapCount++;
					i = j;
				}
			}
		}
		fitnessValue += overlapWeight * overlapCount;
		//盡可能中軸相同
		float disCenterXCount = 0;
		int avgX = 0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			avgX += _chromosome.spots[i].center.tileX;
		}
		avgX = avgX / _chromosome.spots.Count;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			disCenterXCount += Mathf.Abs(_chromosome.spots[i].center.tileX - avgX);
		}
		fitnessValue += disCenterXWeight * disCenterXCount;

		return 1.0f / fitnessValue;
	}
	public class Chromosome//染色體
	{
		public List<Spot> spots;
		public Chromosome(Chromosome _chromosome)
		{
			this.spots = new List<Spot>(_chromosome.spots);
		}
		public Chromosome(List<Spot> _spots)//初始產生染色體以Random的方式增加變異性
		{
			spots = new List<Spot>(_spots);

			for (int i = 0; i < spots.Count; ++i)
			{
				spots[i].RandomCenter();
			}
		}
	}
	public class Spot //一個區塊當作基因
	{
		public Coord center;
		public int xUnit;
		public int yUnit;
		public int spotType;

		public List<Coord> regionCoord = new List<Coord>();

		public Spot(Coord _center, int _xUnit, int _yUnit, int _spotType)
		{
			this.center = _center;
			this.xUnit = _xUnit;
			this.yUnit = _yUnit;
			this.spotType = _spotType;

			this.regionCoord.Clear();
			regionCoord = GetRegionCoord();
		}
		public Spot(int _xUnit, int _yUnit, int _spotType)
		{
			this.xUnit = _xUnit;
			this.yUnit = _yUnit;
			this.spotType = _spotType;
			RandomCenter();

			this.regionCoord.Clear();
			regionCoord = GetRegionCoord();
		}
		public Spot(Spot _newSpot)
		{
			this.center = _newSpot.center;
			this.xUnit = _newSpot.xUnit;
			this.yUnit = _newSpot.yUnit;
			this.spotType = _newSpot.spotType;

			this.regionCoord.Clear();
			regionCoord = GetRegionCoord();
		}
		public void RandomCenter()
		{
			int randomX = Random.Range(xUnit, map.GetLength(0) - xUnit - 1);
			int randomY = Random.Range(yUnit, map.GetLength(0) - yUnit - 1);
			this.center = new Coord(randomX, randomY);

			this.regionCoord.Clear();
			regionCoord = GetRegionCoord();
		}
		public List<Coord> GetRegionCoord()
		{
			List<Coord> regionCoord = new List<Coord>();
			for (int x = -xUnit; x <= xUnit; x++)
			{
				for (int y = -yUnit; y <= yUnit; y++)
				{
					regionCoord.Add(new Coord(center.tileX + x, center.tileY + y));
				}
			}
			return regionCoord;
		}
		public int OverlapOtherRegionCoordTileCount(Spot _otherSpot)
		{
			int count = 0;
			foreach (Coord coordA in regionCoord)
			{
				foreach (Coord coordB in _otherSpot.regionCoord)
				{
					if (coordA.tileX == coordB.tileX && coordA.tileY == coordB.tileY)
					{
						count++;
					}
				}
			}

			return count;
		}
		public bool isOverlapOtherRegionCoordTile(Spot _otherSpot)
		{
			return (Mathf.Abs(center.tileX - _otherSpot.center.tileX) < (xUnit + _otherSpot.xUnit) || Mathf.Abs(center.tileY - _otherSpot.center.tileY) < (yUnit + _otherSpot.yUnit));

		}
	}
	public class SpotCreater
	{
		public List<Spot> spots = new List<Spot>();

		public void Init()
		{
			spots.Add(new Spot(new Coord(0, 0), 0, 0, 2));
			spots.Add(new Spot(new Coord(0, 0), 1, 1, 2));
			spots.Add(new Spot(new Coord(0, 0), 2, 2, 2));
			spots.Add(new Spot(new Coord(0, 0), 1, 1, 2));
			spots.Add(new Spot(new Coord(0, 0), 2, 1, 2));
			spots.Add(new Spot(new Coord(0, 0), 2, 1, 2));
			spots.Add(new Spot(new Coord(0, 0), 1, 1, 2));
			spots.Add(new Spot(new Coord(0, 0), 3, 3, 2));
		}
	}
}

