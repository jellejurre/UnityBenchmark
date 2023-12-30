using System;
using System.Linq;
using UnityEngine;

public class ClothCapsuleCountActiveBenchmark : BenchmarkTask1d
{
	private GameObject[] colliders;

	public override GameObject PrepareIteration1d(GameObject prefab, int value)
	{
		int sideLength = 400;
		GameObject gameObject = Instantiate(prefab);
		gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		GameObject meshObj = MeshHelper.GetSquareMesh(sideLength);
		meshObj = Instantiate(meshObj, gameObject.transform, true);
		Cloth cloth = meshObj.AddComponent<Cloth>();
		MeshHelper.ProcessCloth(cloth, sideLength);
		CapsuleCollider[] colliderPairs = new CapsuleCollider[value];
		GameObject[] colliders = new GameObject[value];
		for (int j = 0; j < value; j++)
		{
			GameObject colliderObject = new GameObject();
			colliderObject.transform.parent = gameObject.transform;
			CapsuleCollider capsuleCollider = colliderObject.AddComponent<CapsuleCollider>();
			capsuleCollider.radius = 0.05f;
			colliderPairs[j] = capsuleCollider;
			colliders[j] = colliderObject;
		}

		this.colliders = colliders;
		cloth.capsuleColliders = colliderPairs;
		return gameObject;
	}

	
	public override string GetParameterName()
	{
		return "Capsule Collider Count";
	}
	
	public override string GetName()
	{
		return "ClothCapsuleCountActive";
	}

	public override string GetDescription()
	{
		return "Benchmarks active capsule cloth component collider count on avatar";
	}


	private int i = 0;
	public override void RunPlaymode1d(GameObject prefab, int value)
	{
		i++;
		prefab.transform.localPosition = new Vector3(0, (float)Math.Sin(i++ / 100f) ,0);
		for (int j = 0; j < colliders.Length; j++)
		{
			GameObject o = colliders[j];
			o.transform.localPosition = new Vector3((j - colliders.Length / 2f) / colliders.Length + 0.5f, 0, (float)Math.Sin((i + j) / 10f) + 0.5f);
		}
	}
}