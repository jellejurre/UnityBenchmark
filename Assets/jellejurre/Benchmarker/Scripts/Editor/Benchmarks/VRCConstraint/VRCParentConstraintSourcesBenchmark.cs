using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Constraint.Components;

public class VRCParentConstraintSourcesBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		GameObject toggleObject = new GameObject("test");
		toggleObject.transform.parent = gameObject.transform;
		VRCParentConstraint constraint = toggleObject.AddComponent<VRCParentConstraint>();
		for (int i = 0; i < iterationNum; i++)
		{
			VRCConstraintBase.ConstraintSource source = new VRCConstraintBase.ConstraintSource();
			source.Weight = 1;
			source.SourceTransform = gameObject.transform;
			constraint.Sources = constraint.Sources.AddItem(source).ToArray();
		}
		constraint.enabled = true;
		constraint.IsActive = true;
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Parent Constraint Sources";
	}
	
	public override string GetName()
	{
		return "VRCParentConSources";
	}

	public override string GetDescription()
	{
		return "Benchmarks parent constraint source count on avatar";
	}
}