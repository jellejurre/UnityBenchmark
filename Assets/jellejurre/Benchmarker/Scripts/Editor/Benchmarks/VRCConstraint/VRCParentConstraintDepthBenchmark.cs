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

public class VRCParentConstraintDepthBenchmark : BenchmarkTask1d
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
			VRCParentConstraint constraint = toggleObject.AddComponent<VRCParentConstraint>();
			VRCConstraintBase.ConstraintSource source = new VRCConstraintBase.ConstraintSource();
			source.Weight = 1;
			source.SourceTransform = last.transform;
			last = toggleObject;
			constraint.Sources = constraint.Sources.AddItem(source).ToArray();
			constraint.enabled = true;
			constraint.IsActive = true;
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
		return "VRCParentConstraintDepth";
	}

	public override string GetDescription()
	{
		return "Benchmarks parent constraint depth count on avatar";
	}
}