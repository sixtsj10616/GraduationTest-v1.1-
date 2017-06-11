using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {


	public enum TileType { Water = 0, Land = 1 }
	static int[,] map;
	public List<Spot> ga_spots=new List<Spot>();
	public List<Chromosome> ga_chromosomes = new List<Chromosome>();
	List<float> wheel = new List<float>();
	float crossoverRate = 0.95f, mutationRate = 0.025f;
	int countOfGeneration = 5000, countOfChromosome = 200;

	void InitChromosome()
	{
		ga_chromosomes.Clear();
		for (int i = 0; i < countOfChromosome; i++)
		{
			ga_chromosomes.Add(new Chromosome(ga_spots));
		}

	}
	public GeneticAlgorithm(int[,] _map)
	{
		map = _map;
	}
	public List<Region> ShowResult()
	{
		List<Region> buildingRegionList=new List<Region>();
		int bestIndex = 0;
		float bestValue = float.MaxValue;
		for (int i = 0; i < ga_chromosomes.Count; i++)
		{
			float fitnessValue = Cost(ga_chromosomes[i]);
			Debug.Log("fitnessValue" + fitnessValue);
			if (fitnessValue < bestValue)
			{
				bestValue=fitnessValue;
				bestIndex=i;
			}
		}
		for (int i = 0; i < ga_chromosomes[bestIndex].spots.Count; i++)
		{
			Region buildingRegion = new Region(ga_chromosomes[bestIndex].spots[i].regionCoord, 2, map);
			buildingRegionList.Add(buildingRegion);
		}
		return buildingRegionList;
	}
	public void Start() 
	{
		SpotCreater spotCreater = new SpotCreater();
		spotCreater.Init();
		ga_spots = spotCreater.spots;
		GA();
	}
	public void GA() 
	{
		InitChromosome();
		for (int i = 0; i < countOfGeneration; i++)
		{
			PrepareSelection();
			List<Chromosome> newGeneration = new List<Chromosome>();
			//一次挑選兩個出來交配
			for (int j = 0; j < ga_chromosomes.Count; j +=2)
			{
				// selections
				Chromosome target1 = new Chromosome(ga_chromosomes[Selection()]);
				Chromosome target2 = new Chromosome(ga_chromosomes[Selection()]);
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
			ga_chromosomes =newGeneration;

		}
	}
	public float Cost(Chromosome _chromosome)
	{
		float fitnessValue = 0;
		
		float overlapWaterWeight=1;
		float overlapWeight = 10;
		float disCenterXWeight = 0.1f;
		//檢查是否有重疊到水
		float overlapWaterCount=0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
		
			//overlapWaterCount += (float)_chromosome.spots[i].regionCoordInWater.Count * 1.0f/_chromosome.spots[i].regionCoord.Count;
			overlapWaterCount += (float)_chromosome.spots[i].regionCoordInWater.Count * Mathf.Exp(-1.0f / _chromosome.spots[i].regionCoord.Count);
		}
		fitnessValue += overlapWaterWeight*overlapWaterCount;


		//檢查是否有重疊到彼此
		float overlapCount = 0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			for (int j = i; j < _chromosome.spots.Count; j++)
			{
				if (_chromosome.spots[i].isOverlapOtherSpot(_chromosome.spots[j])) 
				{
					//overlapCount += (float)_chromosome.spots[i].OverlapOtherSpotTileCount(_chromosome.spots[j]) * 1.0f / _chromosome.spots[i].regionCoord.Count;
					overlapCount += (float)_chromosome.spots[i].OverlapOtherSpotTileCount(_chromosome.spots[j]) * Mathf.Exp(-1.0f / _chromosome.spots[i].regionCoord.Count);
				}
			}
		}
		fitnessValue += overlapWeight*overlapCount;
		//盡可能中軸相同
		float disCenterXCount = 0;
		int avgX=0;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			avgX += _chromosome.spots[i].center.tileX;
		}
		avgX = avgX / _chromosome.spots.Count;
		for (int i = 0; i < _chromosome.spots.Count; i++)
		{
			disCenterXCount += Mathf.Abs(_chromosome.spots[i].center.tileX - avgX);
		}
		fitnessValue += disCenterXWeight*disCenterXCount;
		return fitnessValue;
	}
	public void Mutation(ref Chromosome _chromosome)
	{
		int count=0;
		while (count<10) 
		{
			int randomIndex = Random.Range(0, _chromosome.spots.Count);
			int randomNum = Random.Range(0, _chromosome.spots.Count);
			int nextIndex = (randomIndex + randomNum) % _chromosome.spots.Count;
			if (
			(_chromosome.spots[randomIndex].center.tileX + _chromosome.spots[nextIndex].xUnit < map.GetLength(0)-1)      &&
			(_chromosome.spots[randomIndex].center.tileX - _chromosome.spots[nextIndex].xUnit > 0                 )      &&
			(_chromosome.spots[randomIndex].center.tileY + _chromosome.spots[nextIndex].yUnit < map.GetLength(1)-1)      &&
			(_chromosome.spots[randomIndex].center.tileY - _chromosome.spots[nextIndex].yUnit > 0                 )      &&
			(_chromosome.spots[nextIndex].center.tileX + _chromosome.spots[randomIndex].xUnit < map.GetLength(0)-1)      &&
			(_chromosome.spots[nextIndex].center.tileX - _chromosome.spots[randomIndex].xUnit > 0                 )      &&
			(_chromosome.spots[nextIndex].center.tileY + _chromosome.spots[randomIndex].yUnit < map.GetLength(1)-1)      &&
			(_chromosome.spots[nextIndex].center.tileY - _chromosome.spots[randomIndex].yUnit > 0				  )
			)
			{
				Coord tmp = new Coord (_chromosome.spots[randomIndex].center);
				_chromosome.spots[randomIndex].center = new Coord( _chromosome.spots[nextIndex].center);
				_chromosome.spots[randomIndex].GetRegionCoord();
				_chromosome.spots[nextIndex].center = tmp;
				_chromosome.spots[nextIndex].GetRegionCoord();
				break;
			}
			count++;
		}
	}

	void PrepareSelection()
	{
		float sum = 0.0f;
		wheel.Clear();
		for (int i = 0; i < ga_chromosomes.Count; i++)
		{		
			float cost=Cost(ga_chromosomes[i]);
			sum += 1.0f / cost;
			wheel.Add(sum);
		}
		

	}
	int Selection()
	{
		float random = Random.Range(0.0f, wheel[(wheel.Count - 1)]);
		for (int index = 0; index < wheel.Count; index++)
		{
			if (random < wheel[index]) 
			{	
				return index;		
			}
		}
		return 0;
	}
	public void Crossover(ref Chromosome _c1, ref Chromosome _c2)
	{
		for (int i = 0; i < _c1.spots.Count; i++)
		{
			if (_c1.spots[i].regionCoordInWater.Count > _c2.spots[i].regionCoordInWater.Count || (_c1.SpotOverlapTileCount(i) > _c2.SpotOverlapTileCount(i)))
			{
				SwapSpot(ref _c1, ref _c2, i);
			}
			
		}
	}
	public void SwapSpot(ref Chromosome _c1, ref Chromosome _c2, int index)
	{
   		Spot tmp = new Spot(_c1.spots[index]);
   		_c1.spots[index] = new Spot(_c2.spots[index]);
   		_c2.spots[index] = tmp;

	}
	public class Chromosome//染色體
	{
		public List<Spot> spots=new List<Spot>();
		public Chromosome(Chromosome _chromosome)
		{
			this.spots = new List<Spot>(_chromosome.spots);
		}
		public Chromosome(List<Spot> _spots)//初始產生染色體以Random的方式增加變異性
		{
			spots.Clear();
			for (int i = 0; i < _spots.Count; i++)
			{
				spots.Add(new Spot(_spots[i]));
				spots[i].RandomCenter();
			}
		}
		public int SpotOverlapTileCount(int _index) 
		{ 
			int count=0;
			for(int i=0;i<spots.Count;i++)
			{
				if(i ==_index)continue;
				count += spots[_index].OverlapOtherSpotTileCount(spots[i]);
			}
			return count;
		}
	}

	public class Spot //一個區塊當作基因
	{
		public Coord center=new Coord();
		public int xUnit;
		public int yUnit;
		public int spotType;
		public List<Coord> regionCoord = new List<Coord>();
		public List<Coord> regionCoordInWater = new List<Coord>();
		
		public Spot(Coord _center,int _xUnit, int _yUnit, int _spotType)
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
			this.center = new Coord( _newSpot.center);
			this.xUnit = _newSpot.xUnit;
			this.yUnit = _newSpot.yUnit; 
			this.spotType = _newSpot.spotType;

			this.regionCoord=new List<Coord> (_newSpot.regionCoord);
			this.regionCoordInWater = new List<Coord> (_newSpot.regionCoordInWater);
		}
		public void RandomCenter()
		{
			int randomX = Random.Range(xUnit+1, map.GetLength(0) - xUnit-1);
			int randomY = Random.Range(yUnit+1, map.GetLength(1) - yUnit-1);
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
			for(int x=-xUnit;x<=xUnit;x++)
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
			int count=0;
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
				count=x*y;
			}
			return count;
		}
		public bool isOverlapOtherSpot(Spot _otherSpot)
		{
			return  ((Mathf.Abs(center.tileX - _otherSpot.center.tileX) < (xUnit + _otherSpot.xUnit) )&& (Mathf.Abs(center.tileY - _otherSpot.center.tileY) < (yUnit + _otherSpot.yUnit)));

		}
		public int OverlapOtherRegionCoordTileCountInMap(int  _tileType)
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
 			spots.Add(new Spot( 2, 2, 2));
 			spots.Add(new Spot(3, 3, 2));
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
			spots.Add(new Spot(2, 2, 2));
			spots.Add(new Spot(3, 3, 2));
			spots.Add(new Spot(1, 1, 2));
		}
	}
}

