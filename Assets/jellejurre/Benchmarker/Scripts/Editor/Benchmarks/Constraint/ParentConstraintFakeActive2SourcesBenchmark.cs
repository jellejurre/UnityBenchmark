using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraintFakeActive2SourcesBenchmark : BenchmarkTask1d
{
	private GameObject[] sources;
	private float iter;
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		sources = new GameObject[iterationNum];
		iter = iterationNum;
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			GameObject sourceChild = new GameObject("source"+ i.ToString());
			sourceChild.transform.parent = gameObject.transform;
			GameObject source2Child = new GameObject("secondSource"+ i.ToString());
			source2Child.transform.parent = gameObject.transform;
			sources[i] = sourceChild;
			for (int j = 0; j < 100; j++){
				GameObject lag = new GameObject("lag"+ i.ToString() + j.ToString());
				lag.transform.parent = toggleObject.transform;
			}
			FakeParentConstraint constraint = toggleObject.AddComponent<FakeParentConstraint>();
			constraint.enabled = true;
			constraint.sources = new[] { sourceChild, source2Child };
			constraint.sourceWeights = new[] { 1f, 0.5f };
			constraint.positionOffsets = new [] { Vector3.zero, Vector3.one };
			constraint.rotationOffsets = new [] { Vector3.zero, Vector3.one };
			constraint.defaultPosition = Vector3.zero;
			constraint.defaultRotation = Vector3.zero;
		}
		gameObject.AddComponent<FakeConstraintManager>();
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Active Fake Parent Constraints 2 Sources";
	}
	
	public override string GetName()
	{
		return "FakeActive2SourcesParentConstraint";
	}

	private int j = 0;
	public override void RunPlaymode1d(GameObject prefab, int value)
	{
		j++;
		for (int i = 0; i < iter; i++)
		{
			sources[i].transform.localPosition = new Vector3(Mathf.Sin(i + j), 0, 0);
			sources[i].transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Sin(i + j), 0, 0));
		}
	}

	public override string GetDescription()
	{
		return "Benchmarks fake active 2 source parent constraint count on avatar";
	}
}