using System;
using UnityEngine;

public class MaterialCountBenchmark : BenchmarkTask1d
{
	public override string GetName()
	{
		return "MaterialCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks material count on a 2^16 triangle mesh";
	}

	public override string GetParameterName()
	{
		return "Material Count";
	}

	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		int sideLength = 256;
		GameObject gameObject = Instantiate(prefab);
		gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		GameObject meshObj = MeshHelper.GetSquareMesh(sideLength, value);
		Instantiate(meshObj, gameObject.transform, true);
		return gameObject;
	}
}