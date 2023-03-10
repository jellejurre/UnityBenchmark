using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class AimConstraintBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			AimConstraint constraint = toggleObject.AddComponent<AimConstraint>();
			ConstraintSource source = new ConstraintSource();
			source.weight = 1;
			source.sourceTransform = gameObject.transform;
			constraint.AddSource(source);
			constraint.enabled = true;
			constraint.constraintActive = true;
		}
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Aim Constraints";
	}
	
	public override string GetName()
	{
		return "AimConstraint";
	}

	public override string GetDescription()
	{
		return "Benchmarks aim constraint count on avatar";
	}
}