using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraint2dBenchmark : BenchmarkTask2d
{
	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum1, int iterationNum2)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum1; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			ParentConstraint constraint = toggleObject.AddComponent<ParentConstraint>();
			for (int j = 0; j < iterationNum2; j++)
			{
				ConstraintSource source = new ConstraintSource();
				source.weight = 1;
				source.sourceTransform = gameObject.transform;
				constraint.AddSource(source);
			}
			constraint.enabled = true;
			constraint.constraintActive = true;
		}
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string[] GetParameterNames()
	{
		return new string[] {"Parent Constraints", "Target Count"};
	}
	
	public override string GetName()
	{
		return "ParentConstraint2d";
	}

	public override string GetDescription()
	{
		return "Benchmarks parent constraint count and target count on avatar.\nFirst number is constraint count, second number is target count.";
	}
}