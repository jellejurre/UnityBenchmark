using System;
using System.Linq;
using UnityEngine;

public class ClothVertexCountMeshCountBenchmark : BenchmarkTask2d
{


	public override GameObject PrepareIteration2d(GameObject prefab, int vertexCount, int meshCount)
	{
		int sideLength = (int)Math.Sqrt(vertexCount);
		GameObject gameObject = Instantiate(prefab);
		gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		for (int i = 0; i < meshCount; i++)
		{
			GameObject meshObj = MeshHelper.GetSquareMesh(sideLength);
			meshObj = Instantiate(meshObj, gameObject.transform, true);
			meshObj.transform.position = new Vector3(i - (meshCount/2), 0);
			meshObj.transform.parent = gameObject.transform;
			Cloth cloth = meshObj.AddComponent<Cloth>();
			MeshHelper.ProcessCloth(cloth, sideLength);
		}
		return gameObject;
	}

	public override string[] GetParameterNames()
	{
		return new[] { "Vertex Count", "Mesh Count" };
	}

	public override string GetName()
	{
		return "ClothVertexMeshBenchmark";
	}

	public override string GetDescription()
	{
		return "Benchmarks inactive cloth component count and mesh count on avatar";
	}
}