using System;
using UnityEngine;

public class BlendShapeSingleCountBenchmark : BenchmarkTask1d
{
	public override string GetName()
	{
		return "BlendShapeSingleCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks blendshape count on a 2^16 triangle mesh with single vertices impacted";
	}

	public override string GetParameterName()
	{
		return "Blendshape Count";
	}

	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		int sideLength = 256;
		GameObject parent = new GameObject();
		for (int j = 0; j < 25; j++)
		{
			Instantiate(MeshHelper.GetSquareMesh(sideLength, 1, value, true), parent.transform);
		}
		return parent;
	}
}