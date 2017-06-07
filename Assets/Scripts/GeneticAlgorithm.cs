using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {


	public enum TileType { Water = 0, Land = 1 }
	static int[,] map;
	public List<Spot> spots;
	public List<Chromosome> chromosomes = new List<Chromosome>();
	List<float> wheel = new List<float>();
	float crossoverRate = 0.95f, mutationRate = 0.025f;
	int countOfGeneration = 100, countOfChromosome = 100;

	void InitChromosome(int _countOfChromosome)
	{
		chromosomes.Clear();
		for (int i = 0; i < countOfChromosome; i++)
			chromosomes.Add(new Chromosome(spots));
	}
	public GeneticAlgorithm(int[,] _map)
	{
		map = _map;
	}
	public List<Region> ShowResult()
	{
		List<Region> buildingRegionList=new List<Region>();
		float bestValue=0;
		int bestIndex = 0;
		for (int i = 0; i < chromosomes.Count; i++)
		{
			float fitnessValue = Fitness(chromosomes[i]);
			if (fitnessValue > bestValue)
			{
				bestValue=fitnessValue;
				bestIndex=i;
			}
		}
		for (int i = 0; i < chromosomes[bestIndex].spots.Count; i++)
		{
			Region buildingRegion = new Region(chromosomes[bestIndex].spots[i].regionCoord, 2, map);
			buildingRegionList.Add(buildingRegion);
		}
		return buildingRegionList;
	}
	public void Start() 
	{
		SpotCreater spotCreater = new SpotCreater();
		spotCreater.Init();
		spots = spotCreater.spots;
		GA();
	}
	public void GA() 
	{
		InitChromosome(countOfChromosome);
		for (int i = 0; i < countOfGeneration; i++)
		{
			PrepareSelection();
			List<Chromosome> newGeneration = new List<Chromosome>();
			//一次挑選兩個出來交配
			for (int j = 0; j < chromosomes.Count; j += 2)
			{
				// selections
				Chromosome target1 = new Chromosome(chromosomes[Selection()]);
				Chromosome target2 = new Chromosome(chromosomes[Selection()]);
				// crossover
				if (crossoverRate > Random.Range(0.0f, 1.0f))
				{
					Crossover(ref target1, ref target2);
				}
				// MutationRate
				if (mutationRate > Random.Range(0.0f, 1.0f))
				{
					Mutation(ref target1);
				}
				if (mutationRate > Random.Range(0.0f, 1.0f))
				{
					Mutation(ref target2);
				}
				newGeneration.Add(target1);
				newGeneration.Add(target2);
			}
			chromosomes = newGeneration;
		}
		PrepareSelection();
	}
	public float Fitness(Chromosome _chromosome)
	{
		float fitnessValue = 0;
		
		float overlapWaterWeight=1;
		float overlapWeight = 1;
		float disCenterXWeight = 2;
		//檢查是否有重疊到水
		float overlapWaterCount=100;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			for (int j = 0; j < _chromosome.spots[i].regionCoord.Count; j++)
			{
				if (map[_chromosome.spots[i].regionCoord[j].tileX, _chromosome.spots[i].regionCoord[j].tileY] == (int)TileType.Water)
				{
					overlapWaterCount -= overlapWaterWeight*overlapWaterCount;
				}
			}
		}
		fitnessValue += overlapWaterCount;
		//檢查是否有重疊到彼此
		float overlapCount = 100;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			for (int j = 0; j < _chromosome.spots.Count; j++)
			{
				if(i==j)continue;
				overlapCount -= overlapWeight*_chromosome.spots[i].OverlapOtherRegionCoordTileCount(_chromosome.spots[j]);
			}
		}
		fitnessValue += overlapCount;
		//盡可能中軸相同
		float disCenterXCount = 100;
		int avgX=0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			avgX += _chromosome.spots[i].center.tileX;
		}
		avgX = avgX / _chromosome.spots.Count;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			disCenterXCount -= disCenterXWeight * Mathf.Abs(_chromosome.spots[i].center.tileX - avgX);
		}
		fitnessValue += disCenterXCount;
		return fitnessValue;
	}
	public void Mutation(ref Chromosome _chromosome)
	{
		int randomSize = Random.Range(0, spots.Count);
		for (int n = 0; n < randomSize; n++)
		{
			int randomIndex = Random.Range(0, spots.Count);
			_chromosome.spots[randomIndex].RandomCenter();
		}
	
	}
	void PrepareSelection()
	{
		float sum = 0.0f;
		wheel.Clear();
		for (int i = 0; i < chromosomes.Count; i++)
		{
			sum += Fitness(chromosomes[i]);
			wheel.Add(sum);
		}
	}
	int Selection()
	{
		float random = Random.Range(0.0f, wheel[wheel.Count - 1]);
		for (int index = 0; index < wheel.Count; index++)
		{
			if (random <= wheel[index])
				return index;
		}
		return 0;
	}
	public void Crossover(ref Chromosome _c1, ref Chromosome _c2)
	{
		int randomSize = Random.Range(0, spots.Count);
		for(int n=0;n<randomSize;n++)
		{
			int randomIndex = Random.Range(0, spots.Count);
			Spot tmp = new Spot(_c1.spots[randomIndex]);
			_c1.spots[randomIndex] = _c2.spots[randomIndex];
			_c2.spots[randomIndex] = tmp;
		}
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

		public Spot(Coord _center,int _xUnit, int _yUnit, int _spotType)
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
			List<Coord> regionCoord=new List<Coord>();
			for(int x=-xUnit;x<=xUnit;x++)
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
			int count=0;
			foreach(Coord coordA in regionCoord)
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
	}
	public class SpotCreater
	{ 
		public List<Spot> spots = new List<Spot>();

		public void Init()
		{
			spots.Add(new Spot(new Coord(0,0),0, 0, 2));
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

