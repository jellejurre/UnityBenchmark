using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraintFakeActiveBenchmark : BenchmarkTask1d
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
			sources[i] = sourceChild;
			FakeParentConstraint constraint = toggleObject.AddComponent<FakeParentConstraint>();
			constraint.enabled = true;
			constraint.sources = new[] { sourceChild };
			constraint.sourceWeights = new[] { 1f };
			constraint.positionOffsets = new [] { Vector3.zero };
			constraint.rotationOffsets = new [] { Vector3.zero };
			constraint.defaultPosition = Vector3.zero;
			constraint.defaultRotation = Vector3.zero;
		}

		gameObject.AddComponent<FakeConstraintManager>();
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Active Fake Parent Constraints";
	}
	
	public override string GetName()
	{
		return "FakeActiveParentConstraint";
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
		return "Benchmarks fake parent active constraint count on avatar";
	}
}