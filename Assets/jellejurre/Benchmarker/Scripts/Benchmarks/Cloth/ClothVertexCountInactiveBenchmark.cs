using System;
using System.Linq;
using UnityEngine;

public class ClothVertexCountInactiveBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		int sideLength = (int)Math.Sqrt(value);
		GameObject gameObject = Instantiate(prefab);
		gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		GameObject meshObj = MeshHelper.GetSquareMesh(sideLength);
		meshObj = Instantiate(meshObj, gameObject.transform, true);
		Cloth cloth = meshObj.AddComponent<Cloth>();
		MeshHelper.ProcessCloth(cloth, sideLength);
		return gameObject;
	}
	public override string GetParameterName()
	{
		return "Vertex Count";
	}
	
	public override string GetName()
	{
		return "ClothVertexCountInactive";
	}

	public override string GetDescription()
	{
		return "Benchmarks inactive cloth component count on avatar";
	}
}