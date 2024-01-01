using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BlendShapeActiveVariableCountBenchmark : BenchmarkTask1d
{
	public override string GetName()
	{
		return "BlendShapeVariableCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks dynamic activity blendshape count on a 2^16 triangle mesh with all vertices impacted";
	}

	public override string GetParameterName()
	{
		return "Blendshape Count";
	}

	public SkinnedMeshRenderer[] meshes;

	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		int sideLength = 256;
		GameObject parent = new GameObject();
		meshes = new SkinnedMeshRenderer[25];
		for (int j = 0; j < 25; j++)
		{
			GameObject meshObj = Instantiate(MeshHelper.GetSquareMesh(sideLength, 1, 512), parent.transform);
			meshes[j] = meshObj.GetComponent<SkinnedMeshRenderer>();
		}
		return parent;
	}
	
	private int i = 0;
	public override void RunPlaymode1d(GameObject prefab, int value)
	{
		i++;
		for (var mi = 0; mi < meshes.Length; mi++)
		{
			SkinnedMeshRenderer mesh = meshes[mi];
			for (int j = 0; j < value; j++)
			{
				mesh.SetBlendShapeWeight(j, ((float)Math.Sin((i + j) / 10f) + 1));
			}
		}
	}
}