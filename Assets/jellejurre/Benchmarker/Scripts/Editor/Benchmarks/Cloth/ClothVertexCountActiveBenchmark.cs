using System;
using System.Linq;
using UnityEngine;

public class ClothVertexCountActiveBenchmark : BenchmarkTask1d
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
		return "ClothVertexCountActive";
	}

	public override string GetDescription()
	{
		return "Benchmarks active cloth component count on avatar";
	}


	private int i = 0;
	public override void RunPlaymode1d(GameObject prefab, int value)
	{
		i++;
		prefab.transform.localPosition = new Vector3(0, (float)Math.Sin(i++ / 100f) ,0);
	}
}