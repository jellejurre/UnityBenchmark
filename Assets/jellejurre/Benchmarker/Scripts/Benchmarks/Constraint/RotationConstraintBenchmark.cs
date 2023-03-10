﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Animations;

public class RotationConstraintBenchmark : BenchmarkTask1d
{
	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test"+ i.ToString());
			toggleObject.transform.parent = gameObject.transform;
			RotationConstraint constraint = toggleObject.AddComponent<RotationConstraint>();
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
		return "Rotation Constraints";
	}
	
	public override string GetName()
	{
		return "RotationConstraint";
	}

	public override string GetDescription()
	{
		return "Benchmarks rotation constraint count on avatar";
	}
}