using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class BoneActiveCountBenchmark : BenchmarkTask1d
{
	public override string GetName()
	{
		return "BoneActiveCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks Active bone count on a 2^16 triangle mesh with all vertices impacted";
	}

	public override string GetParameterName()
	{
		return "Bone Count / 25";
	}

	public GameObject[] meshes;

	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		int sideLength = 256;
		GameObject parent = new GameObject();
		meshes = new GameObject[25];
		for (int j = 0; j < 25; j++)
		{
			meshes[j] = Instantiate(MeshHelper.GetSquareMesh(sideLength, 1, 0, true, value), parent.transform);
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