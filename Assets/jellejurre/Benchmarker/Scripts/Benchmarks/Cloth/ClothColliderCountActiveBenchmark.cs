using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class ClothColliderCountActiveBenchmark : BenchmarkTask1d
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
		ClothSphereColliderPair[] colliderPairs = new ClothSphereColliderPair[value];
		GameObject[] colliders = new GameObject[value];
		for (int j = 0; j < value; j++)
		{
			GameObject colliderObject = new GameObject();
			colliderObject.transform.parent = gameObject.transform;
			SphereCollider sphereCollider = colliderObject.AddComponent<SphereCollider>();
			sphereCollider.radius = 0.05f;
			colliderPairs[j] = new ClothSphereColliderPair(sphereCollider);
			colliders[j] = colliderObject;
		}

		this.colliders = colliders;
		cloth.sphereColliders = colliderPairs;
		return gameObject;
	}

	
	public override string GetParameterName()
	{
		return "Collider Count";
	}
	
	public override string GetName()
	{
		return "ClothColliderCountActive";
	}

	public override string GetDescription()
	{
		return "Benchmarks active cloth component collider count on avatar";
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