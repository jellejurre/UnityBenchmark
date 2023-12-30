using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;

public class ContactComboSameNameBenchmark: BenchmarkTask1d
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
			sender.collisionTags = new List<string>(){"test"};
			GameObject toggleObject2 = new GameObject("tast"+ i.ToString());
			toggleObject2.transform.parent = gameObject.transform;
			ContactReceiver receiver = toggleObject2.AddComponent<VRCContactReceiver>();
			receiver.position = new Vector3(i, 0, 0);
			receiver.parameter = "test" + i.ToString();
			receiver.collisionTags = new List<string>(){"test"};
		}
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "ContactSendRecPairs";
	}
	
	public override string GetName()
	{
		return "ContactComboSameName";
	}

	public override string GetDescription()
	{
		return "Benchmarks seperated Contact Sender & Receiver count with same name on avatar";
	}
}