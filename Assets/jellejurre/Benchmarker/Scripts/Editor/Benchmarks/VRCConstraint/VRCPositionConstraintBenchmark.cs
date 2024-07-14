using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Constraint.Components;

public class VRCPositionConstraintBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			VRCPositionConstraint constraint = toggleObject.AddComponent<VRCPositionConstraint>();
			VRCConstraintBase.ConstraintSource source = new VRCConstraintBase.ConstraintSource();
			source.Weight = 1;
			source.SourceTransform = gameObject.transform;
			constraint.Sources = constraint.Sources.AddItem(source).ToArray();
			constraint.enabled = true;
			constraint.IsActive = true;
		}
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Position Constraints";
	}
	
	public override string GetName()
	{
		return "VRCPositionConstraint";
	}

	public override string GetDescription()
	{
		return "Benchmarks position constraint count on avatar";
	}
}