﻿using System;
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

public class VRCScaleConstraintBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			VRCScaleConstraint constraint = toggleObject.AddComponent<VRCScaleConstraint>();
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
		return "Scale Constraints";
	}
	
	public override string GetName()
	{
		return "VRCScaleConstraint";
	}

	public override string GetDescription()
	{
		return "Benchmarks scale constraint count on avatar";
	}
}