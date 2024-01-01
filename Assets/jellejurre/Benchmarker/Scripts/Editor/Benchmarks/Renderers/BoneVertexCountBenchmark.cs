using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class BoneVertexCountBenchmark : BenchmarkTask1d
{
	public override string GetName()
	{
		return "BoneVertexCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks vertex count on a mesh with all vertices impacted with 1024 bones";
	}

	public override string GetParameterName()
	{
		return "Sqrt(Vertex Count)/32";
	}
	
	public GameObject[] meshes;

	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		GameObject parent = new GameObject();
		meshes = new GameObject[32];
		for (int j = 0; j < 32; j++)
		{
			meshes[j] = Instantiate(MeshHelper.GetSquareMesh(value, 1, 0, true, 32), parent.transform);
		}
		return parent;
	}
	
	private int i = 0;
	public override void RunPlaymode1d(GameObject prefab, int value)
	{
		i++;
		for (var mi = 0; mi < meshes.Length; mi++)
		{
			GameObject mesh = meshes[mi];
			Transform[] children = Enumerable.Range(0, mesh.transform.childCount)
				.Select(x => mesh.transform.GetChild(x)).ToArray();
			for (var j = 0; j < children.Length; j++)
			{
				children[j].localPosition = new Vector3(0, 0, ((float)Math.Sin((i + j) / 10f)));
			}
		}
	}
}