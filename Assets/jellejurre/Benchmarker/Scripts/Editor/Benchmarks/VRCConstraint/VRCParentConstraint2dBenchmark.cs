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

public class VRCParentConstraint2dBenchmark : BenchmarkTask2d
{
	public override GameObject PrepareIteration2d(GameObject prefab, int iterationNum1, int iterationNum2)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum1; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			VRCParentConstraint constraint = toggleObject.AddComponent<VRCParentConstraint>();
			for (int j = 0; j < iterationNum2; j++)
			{
				VRCConstraintBase.ConstraintSource source = new VRCConstraintBase.ConstraintSource();
				source.Weight = 1;
				source.SourceTransform = gameObject.transform;
				constraint.Sources = constraint.Sources.AddItem(source).ToArray();
			}
			constraint.enabled = true;
			constraint.IsActive = true;
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
		return "VRCParentConstraint2d";
	}

	public override string GetDescription()
	{
		return "Benchmarks parent constraint count and target count on avatar.\nFirst number is constraint count, second number is target count.";
	}
}