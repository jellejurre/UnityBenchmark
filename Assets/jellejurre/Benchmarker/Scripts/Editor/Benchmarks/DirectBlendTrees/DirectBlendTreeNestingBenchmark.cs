using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;

public class DirectBlendTreeNestingBenchmark : BenchmarkTask1d
{

	public override GameObject PrepareIteration1d(GameObject prefab, int iterationNum)
	{
		GameObject gameObject = Instantiate(prefab);
		Animator animator = gameObject.GetOrAddComponent<Animator>();
		for (int i = 0; i < 2048; i++)
		{
			GameObject toggleObject = new GameObject("test" + i.ToString());
			toggleObject.transform.SetParent(gameObject.transform);
		}

		int splitCount = (int)Math.Round(Math.Log(iterationNum, 2)) + 1;
		AnimatorController controller = AnimatorHelpers.SetupNestedBlendTree(splitCount, 2048);
		animator.runtimeAnimatorController = controller;
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		AssetDatabase.SaveAssets();
		return gameObject;
	}
	
	public override string GetParameterName()
	{
		return "2^(Nest Layers Deep)";
	}
	
	public override string GetName()
	{
		return "DBTNesting";
	}

	public override string GetDescription()
	{
		return "Benchmarks nested direct blend tree sub toggle count on humanoid avatar";
	}
}