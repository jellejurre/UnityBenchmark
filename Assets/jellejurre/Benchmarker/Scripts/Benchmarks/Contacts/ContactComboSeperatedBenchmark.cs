using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

public class ContactComboSeperatedBenchmark: BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			ContactSender sender = toggleObject.AddComponent<VRCContactSender>();
			sender.position = new Vector3(i, 0, 0);
			sender.collisionTags = new List<string>(){"test" + i.ToString()};
			GameObject toggleObject2 = new GameObject("tast"+ i.ToString());
			toggleObject2.transform.parent = gameObject.transform;
			ContactReceiver receiver = toggleObject2.AddComponent<VRCContactReceiver>();
			receiver.position = new Vector3(0, 0, i);
			receiver.parameter = "test" + i.ToString();
			receiver.collisionTags = new List<string>(){"test" + i.ToString()};
		}
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "ContactSendRecPairs";
	}
	
	public override string GetName()
	{
		return "ContactComboOut";
	}

	public override string GetDescription()
	{
		return "Benchmarks seperated Contact Sender & Receiver count on avatar";
	}
}