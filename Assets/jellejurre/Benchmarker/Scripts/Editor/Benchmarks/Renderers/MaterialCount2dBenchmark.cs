using System;
using UnityEngine;

public class MaterialCount2dBenchmark : BenchmarkTask2d
{
	public override string GetName()
	{
		return "MaterialCount2D";
	}

	public override string GetDescription()
	{
		return "Benchmarks material count vs mesh count on a 2^16 triangles total";
	}
	
	public override string[] GetParameterNames()
	{
		return new [] { "Material Count", "Mesh Count" };
	}

	public override GameObject PrepareIteration2d(GameObject prefab, int value1, int value2)
	{
		int sideLength = 256;
		GameObject parent = new GameObject();
		for (int i = 0; i < value2; i++)
		{
			GameObject meshObj = MeshHelper.GetSquareMesh((int)(sideLength/Math.Sqrt(value2)), value1);
			GameObject child = Instantiate(meshObj, parent.transform, true);
			child.transform.localPosition = new Vector3(i / (float) value2, 0, 0);
		}

		return parent;
		
	}
}