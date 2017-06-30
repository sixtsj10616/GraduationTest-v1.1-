using UnityEngine;
using System.Collections;

public static class FalloffGenerator {

	public static float[,] GenerateFalloffMap(int[,] map) {
		float[,] newMap = new float[map.GetLength(0), map.GetLength(1)];

		for (int i = 0; i < map.GetLength(0); i++)
		{
			for (int j = 0; j < map.GetLength(1); j++)
			{	
				float x=0;
				for(int k=-2;k<=2;k++)
				{
					for (int t = -2; t <= 2; t++)
					{
						x += map[Mathf.Clamp(i + k, 0, map.GetLength(0) - 1), Mathf.Clamp(j + t, 0, map.GetLength(1) - 1)];
					}
				}
// 				float x = map[i, j]
// 						+ map[Mathf.Min(i + 1, map.GetLength(0) - 1), j]
// 						+ map[Mathf.Min(i + 1, map.GetLength(0) - 1), Mathf.Min(j + 1, map.GetLength(1) - 1)]
// 						+ map[Mathf.Min(i + 1, map.GetLength(0) - 1), Mathf.Max(j - 1,0)]
// 						+ map[i, Mathf.Min(j + 1, map.GetLength(1) - 1)]
// 						+ map[i, Mathf.Max(j - 1,0)]
// 						+ map[Mathf.Max(i - 1,0), j]
// 						+ map[Mathf.Max(i - 1,0), Mathf.Min(j + 1, map.GetLength(1) - 1)]
// 						+ map[Mathf.Max(i - 1,0), Mathf.Max(j - 1, 0)];

				newMap[i, j] = 1-(x / 25);
				//Debug.Log(newMap[i, j]);
			}
		}

		return newMap;
	}
	public static float[,] GenerateFalloffMap(int size)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size * 2 - 1;
				float y = j / (float)size * 2 - 1;

				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				map[i, j] = Evaluate(value);
			}
		}

		return map;
	}
	static float Evaluate(float value) {
		float a = 3;
		float b = 2.2f;

		return Mathf.Pow (value, a) / (Mathf.Pow (value, a) + Mathf.Pow (b - b * value, a));
	}
}
