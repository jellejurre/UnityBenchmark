using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class MixedConstraintBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			PositionConstraint constraint = toggleObject.AddComponent<PositionConstraint>();
			ConstraintSource source = new ConstraintSource();
			source.weight = 1;
			source.sourceTransform = gameObject.transform;
			constraint.AddSource(source);
			constraint.enabled = true;
			constraint.constraintActive = true;
			ScaleConstraint constraint2 = toggleObject.AddComponent<ScaleConstraint>();
			ConstraintSource source2 = new ConstraintSource();
			source2.weight = 1;
			source2.sourceTransform = gameObject.transform;
			constraint2.AddSource(source2);
			constraint2.enabled = true;
			constraint2.constraintActive = true;
		}
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Mixed Constraints";
	}
	
	public override string GetName()
	{
		return "MixedConstraint";
	}

	public override string GetDescription()
	{
		return "Benchmarks mixed constraint count on avatar";
	}
}