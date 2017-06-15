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
	public float temperature = 30000;
	public int iteration = 100000;
	public StimulatedAnnealing(int[,] _map)
	{
		map = _map;
	}
	void InitChromosome()
	{
		currentSolution = new Chromosome(spots);
		fitnessCurrent = Cost(currentSolution);
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
			//NeighborhoodSearch(ref newSolution);
			float fitnessNew = Cost(newSolution);

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
			//Debug.Log("fitnessCurrent" + fitnessCurrent);
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
	public float Cost(Chromosome _chromosome)
	{
		float fitnessValue = 0;

		float overlapWaterWeight = 10.0f;
		float overlapWeight = 100.0f;
		float disCenterXWeight = 1.0f;
		//檢查是否有重疊到水
		float overlapWaterCount = 0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			overlapWaterCount += (float)_chromosome.spots[i].regionCoordInWater.Count * 100.0f / _chromosome.spots[i].regionCoord.Count;
		}
		fitnessValue += overlapWaterWeight * overlapWaterCount;


		//檢查是否有重疊到彼此
		float overlapCount = 0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			for (int j = i; j < _chromosome.spots.Count; j++)
			{
				if (_chromosome.spots[i].isOverlapOtherSpot(_chromosome.spots[j]))
				{
					overlapCount += (float)_chromosome.spots[i].OverlapOtherSpotTileCount(_chromosome.spots[j]) * 100.0f / _chromosome.spots[i].regionCoord.Count;
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
		return fitnessValue;
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
		public Coord center = new Coord();
		public int xUnit;
		public int yUnit;
		public int spotType;
		public List<Coord> regionCoord = new List<Coord>();
		public List<Coord> regionCoordInWater = new List<Coord>();

		public Spot(Coord _center, int _xUnit, int _yUnit, int _spotType)
		{
			this.center = new Coord(_center);
			this.xUnit = _xUnit;
			this.yUnit = _yUnit;
			this.spotType = _spotType;
		}
		public Spot(int _xUnit, int _yUnit, int _spotType)
		{
			this.xUnit = _xUnit;
			this.yUnit = _yUnit;
			this.spotType = _spotType;
		}
		public Spot(Spot _newSpot)
		{
			this.center = new Coord(_newSpot.center);
			this.xUnit = _newSpot.xUnit;
			this.yUnit = _newSpot.yUnit;
			this.spotType = _newSpot.spotType;

			this.regionCoord = new List<Coord>(_newSpot.regionCoord);
			this.regionCoordInWater = new List<Coord>(_newSpot.regionCoordInWater);
		}
		public void RandomCenter()
		{
			int randomX = Random.Range(xUnit + 1, map.GetLength(0) - xUnit - 1);
			int randomY = Random.Range(yUnit + 1, map.GetLength(1) - yUnit - 1);
			this.center = new Coord(randomX, randomY);
			GetRegionCoord();
		}
		public void SetCenter(Coord _center)
		{
			this.center = _center;

			GetRegionCoord();
		}
		public void GetRegionCoord()
		{
			this.regionCoord.Clear();
			this.regionCoordInWater.Clear();
			for (int x = -xUnit; x <= xUnit; x++)
			{
				int X = center.tileX + x;
				for (int y = -yUnit; y <= yUnit; y++)
				{
					int Y = center.tileY + y;
					this.regionCoord.Add(new Coord(X, Y));
					if (map[X, Y] == (int)TileType.Water)
					{
						this.regionCoordInWater.Add(new Coord(X, Y));
					}
				}
			}
		}
		public int OverlapOtherSpotTileCount(Spot _otherSpot)
		{
			int count = 0;
			// 			foreach(Coord coordA in regionCoord)
			// 			{
			// 				foreach (Coord coordB in _otherSpot.regionCoord)
			// 				{
			// 					if ((coordA.tileX == coordB.tileX) && (coordA.tileY == coordB.tileY))
			// 					{
			// 						count++;
			// 					}
			// 				}
			// 			}
			if (isOverlapOtherSpot(_otherSpot))
			{
				int x = Mathf.Min(_otherSpot.center.tileX + _otherSpot.xUnit, center.tileX + xUnit) - Mathf.Max(_otherSpot.center.tileX - _otherSpot.xUnit, center.tileX - xUnit);
				int y = Mathf.Min(_otherSpot.center.tileY + _otherSpot.yUnit, center.tileY + yUnit) - Mathf.Max(_otherSpot.center.tileY - _otherSpot.yUnit, center.tileY - yUnit);
				count = x * y;
			}
			return count;
		}
		public bool isOverlapOtherSpot(Spot _otherSpot)
		{
			return ((Mathf.Abs(center.tileX - _otherSpot.center.tileX) < (xUnit + _otherSpot.xUnit)) && (Mathf.Abs(center.tileY - _otherSpot.center.tileY) < (yUnit + _otherSpot.yUnit)));

		}
		public int OverlapOtherRegionCoordTileCountInMap(int _tileType)
		{
			int count = 0;
			foreach (Coord coord in regionCoord)
			{
				if (map[coord.tileX, coord.tileY] == _tileType)
				{
					count++;
				}
			}
			return count;
		}
	}
	public class SpotCreater
	{
		public List<Spot> spots = new List<Spot>();

		public void Init()
		{
			spots.Add(new Spot(4, 4, 2));
			spots.Add(new Spot(1, 1, 2));
			spots.Add(new Spot(2, 2, 2));
			spots.Add(new Spot(3, 3, 2));
			spots.Add(new Spot(1, 1, 2));
			spots.Add(new Spot(2, 2, 2));
			spots.Add(new Spot(3, 3, 2));
			spots.Add(new Spot(1, 1, 2));
			spots.Add(new Spot(2, 2, 2));
			spots.Add(new Spot(3, 3, 2));
			spots.Add(new Spot(1, 1, 2));
		}
	}
}

