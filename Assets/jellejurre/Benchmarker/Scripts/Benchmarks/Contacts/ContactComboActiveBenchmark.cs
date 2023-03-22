using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Contact.Components;
using Random = System.Random;

public class ContactComboActiveBenchmark: BenchmarkTask1d
{
	public bool[][] data;
	public int current;
	public GameObject[] objects;
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		objects = new GameObject[iterationNum];
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			ContactSender sender = toggleObject.AddComponent<VRCContactSender>();
			objects[i] = toggleObject;
			sender.position = new Vector3(i, 0, 0);
			sender.collisionTags = new List<string>(){"test" + i.ToString()};
			GameObject toggleObject2 = new GameObject("tast"+ i.ToString());
			toggleObject2.transform.parent = gameObject.transform;
			ContactReceiver receiver = toggleObject2.AddComponent<VRCContactReceiver>();
			receiver.position = new Vector3(i, 0, 0);
			receiver.parameter = "test" + i.ToString();
			receiver.collisionTags = new List<string>(){"test" + i.ToString()};
		}
		data = new bool[100][];
		
		Random r = new Random();
		for (int j = 0; j < 100; j++)
		{
			data[j] = new bool[iterationNum];
			for (int i = 0; i < iterationNum; i++)
			{
				data[j][i] = (!(r.NextDouble() > 0.5));
			}
		}
		return gameObject;
	}
	
	public override void RunPlaymode1d(GameObject gameObject, int iterationNum)
	{
		current = (current + 1) % 100;
		for (int i = 0; i < iterationNum; i++)
		{
			if (data[current][i])
			{
				objects[i].transform.localPosition = new Vector3(1, 0, 0);
			}
			else
			{
				objects[i].transform.localPosition = new Vector3(0, 0, 0);
			}
		}
	}
	
	public override string GetParameterName()
	{
		return "ContactSendRecPairs";
	}
	
	public override string GetName()
	{
		return "ContactComboActive";
	}

	public override string GetDescription()
	{
		return "Benchmarks changing Contact Sender & Receiver count on avatar";
	}
}