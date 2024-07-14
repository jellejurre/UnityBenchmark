using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraintDepthBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		GameObject last = new GameObject("test");
		last.transform.parent = gameObject.transform;
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			ParentConstraint constraint = toggleObject.AddComponent<ParentConstraint>();
			ConstraintSource source = new ConstraintSource();
			source.weight = 1;
			source.sourceTransform = last.transform;
			last = toggleObject;
			constraint.AddSource(source);
			constraint.enabled = true;
			constraint.constraintActive = true;
		}
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Parent Constraints";
	}
	
	public override string GetName()
	{
		return "ParentConstraintDepth";
	}

	public override string GetDescription()
	{
		return "Benchmarks parent constraint depth count on avatar";
	}
}