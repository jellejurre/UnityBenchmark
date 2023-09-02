using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraintFakeBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			FakeParentConstraint constraint = toggleObject.AddComponent<FakeParentConstraint>();
			constraint.enabled = true;
			constraint.sources = new[] { gameObject };
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
		return "Fake Parent Constraints";
	}
	
	public override string GetName()
	{
		return "FakeParentConstraint";
	}

	public override string GetDescription()
	{
		return "Benchmarks fake parent constraint count on avatar";
	}
}