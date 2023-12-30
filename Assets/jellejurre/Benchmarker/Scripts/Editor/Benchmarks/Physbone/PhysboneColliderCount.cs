using System.Collections.Generic;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.PhysBone.Components;

public class PhysboneColliderCount : BenchmarkTask1d
{
	public override string GetName()
	{
		return "PhysboneColliderCount";
	}

	public override string GetDescription()
	{
		return "Benchmarks active Physbone collider count";
	}

	public override string GetParameterName()
	{
		return "Collider Count";
	}
	
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject root = Instantiate(prefab);
		for (int i = 0; i < 2500; i++)
		{
			GameObject physbone = PhysboneHelper.AddPhysbone(root, depth:1, amount:1);
			physbone.transform.localPosition = new Vector3(0, 0, 1 + ((float)i / 2500));
			VRCPhysBone bone = physbone.GetComponentInChildren<VRCPhysBone>();
			bone.radius = 0.01f;
			List<VRCPhysBoneColliderBase> colliders = new List<VRCPhysBoneColliderBase>();
			for (int j = 0; j < iterationNum; j++)
			{
				VRCPhysBoneCollider collider = physbone.AddComponent<VRCPhysBoneCollider>();
				collider.radius = 0.1f;
				collider.shapeType = VRCPhysBoneColliderBase.ShapeType.Capsule;
				colliders.Add(collider);
			}
			bone.colliders = colliders;
		}
		return root;
	}


	public int i = 0;
	public override void RunPlaymode1d(GameObject prefab, int value)
	{
		i++;
		prefab.transform.position = new Vector3(0, Mathf.Sin(i/100f), 0);
	}
}