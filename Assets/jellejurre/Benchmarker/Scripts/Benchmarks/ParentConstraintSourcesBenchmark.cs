using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraintSourcesBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		GameObject toggleObject = new GameObject("test");
		toggleObject.transform.parent = gameObject.transform;
		ParentConstraint constraint = toggleObject.AddComponent<ParentConstraint>();
		for (int i = 0; i < iterationNum; i++)
		{
			ConstraintSource source = new ConstraintSource();
			source.weight = 1;
			source.sourceTransform = gameObject.transform;
			constraint.AddSource(source);	
		}
		constraint.enabled = true;
		constraint.constraintActive = true;
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Parent Constraint Sources";
	}
	
	public override string GetName()
	{
		return "ParentConSources";
	}

	public override string GetDescription()
	{
		return "Benchmarks parent constraint source count on avatar";
	}
}