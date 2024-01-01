using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class BoneCountBenchmark : BenchmarkTask1d
{
	public override string GetName()
	{
		return "BoneCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks bone count on a 2^16 triangle mesh with all vertices impacted";
	}

	public override string GetParameterName()
	{
		return "Bone Count";
	}
	
	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		int sideLength = 256;
		GameObject parent = new GameObject();
		for (int j = 0; j < 25; j++)
		{ 
			Instantiate(MeshHelper.GetSquareMesh(sideLength, 1, 0, true, value), parent.transform);
		}
		return parent;
	}
}