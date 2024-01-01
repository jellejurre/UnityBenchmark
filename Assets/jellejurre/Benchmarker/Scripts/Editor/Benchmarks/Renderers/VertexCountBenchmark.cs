using System;
using UnityEngine;

public class VertexCountBenchmark : BenchmarkTask1d
{
	public override string GetName()
	{
		return "VertexCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks vertex count on a mesh";
	}

	public override string GetParameterName()
	{
		return "Sqrt(Vertex Count)";
	}

	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		GameObject gameObject = Instantiate(prefab);
		gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		GameObject meshObj = MeshHelper.GetSquareMesh(value, 1);
		Instantiate(meshObj, gameObject.transform, true);
		return gameObject;
	}
}