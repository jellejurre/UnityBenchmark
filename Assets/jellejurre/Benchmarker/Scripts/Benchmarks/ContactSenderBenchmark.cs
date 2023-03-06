using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

public class ContactSenderBenchmark: BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			ContactSender constraint = toggleObject.AddComponent<VRCContactSender>();
			constraint.position = new Vector3(i, 0, 0);
			constraint.collisionTags = new List<string>(){"test" + i.ToString()};
		}
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "ContactSender";
	}
	
	public override string GetName()
	{
		return "ContactSender";
	}

	public override string GetDescription()
	{
		return "Benchmarks Contact Sender count on avatar";
	}
}