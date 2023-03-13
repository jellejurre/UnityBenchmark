﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;

public class DirectBlendTreeToggleBenchmark : BenchmarkTask1d
{

	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		for (int i = 0; i < iterationNum; i++)
		{
			GameObject toggleObject = new GameObject("test" + i.ToString());
			toggleObject.transform.SetParent(gameObject.transform);
		}
		AnimatorController controller = AnimatorHelpers.SetupDirectBlendTree(iterationNum);
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "Toggles";
	}
	
	public override string GetName()
	{
		return "DBTToggle";
	}

	public override string GetDescription()
	{
		return "Benchmarks direct blend tree sub toggle count on humanoid avatar";
	}
}