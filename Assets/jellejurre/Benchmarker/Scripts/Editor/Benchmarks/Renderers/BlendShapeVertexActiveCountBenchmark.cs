using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BlendShapeVertexActiveCountBenchmark : BenchmarkTask1d
{
	public override string GetName()
	{
		return "BlendShapeVertexActiveCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks Active Vertex count on 32 meshes with 32 blendshapes with all vertices impacted";
	}

	public override string GetParameterName()
	{
		return "Sqrt(Vertex Count)/32";
	}

	public SkinnedMeshRenderer[] meshes;

	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		GameObject parent = new GameObject();
		meshes = new SkinnedMeshRenderer[32];
		for (int j = 0; j < 32; j++)
		{
			GameObject meshObj = Instantiate(MeshHelper.GetSquareMesh(value, 1, 32), parent.transform);
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
			for (int j = 0; j < mesh.sharedMesh.blendShapeCount; j++)
			{
				mesh.SetBlendShapeWeight(j, ((float)Math.Sin((i + j) / 10f) + 1));
			}
		}
	}
}