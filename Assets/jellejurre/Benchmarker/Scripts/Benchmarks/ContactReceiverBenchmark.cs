using System;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

public class ContactReceiverBenchmark: BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			ContactReceiver constraint = toggleObject.AddComponent<VRCContactReceiver>();
			constraint.position = new Vector3(0, 0, i);
			constraint.parameter = "test" + i.ToString();
		}
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "ContactReceivers";
	}
	
	public override string GetName()
	{
		return "ContactReceiver";
	}

	public override string GetDescription()
	{
		return "Benchmarks Contact Receiver count on avatar";
	}
}